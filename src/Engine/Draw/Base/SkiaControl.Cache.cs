using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw;

public partial class SkiaControl
{


    private readonly LimitedQueue<Action> _offscreenCacheRenderingQueue = new(1);

    public static readonly BindableProperty UseCacheProperty = BindableProperty.Create(nameof(UseCache),
        typeof(SkiaCacheType),
        typeof(SkiaControl),
        SkiaCacheType.None,
        propertyChanged: NeedDraw);

    /// <summary>
    /// Never reuse the rendering result. Actually true for ScrollLooped SkiaLayout viewport container to redraw its content several times for creating a looped aspect.
    /// </summary>
    public SkiaCacheType UseCache
    {
        get { return (SkiaCacheType)GetValue(UseCacheProperty); }
        set { SetValue(UseCacheProperty, value); }
    }

    public static readonly BindableProperty AllowCachingProperty = BindableProperty.Create(nameof(AllowCaching),
        typeof(bool),
        typeof(SkiaControl),
        true,
        propertyChanged: NeedDraw);

    /// <summary>
    /// Might want to set this to False for certain cases..
    /// </summary>
    public bool AllowCaching
    {
        get { return (bool)GetValue(AllowCachingProperty); }
        set { SetValue(AllowCachingProperty, value); }
    }

    /// <summary>
    /// Used by the UseCacheDoubleBuffering process. 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CachedObject RenderObjectPrevious
    {
        get
        {
            return _renderObjectPrevious;
        }
        set
        {
            RenderObjectNeedsUpdate = false;
            if (_renderObjectPrevious != value)
            {
                var kill = _renderObjectPrevious;
                _renderObjectPrevious = value;
                if (kill != null
                    && UsingCacheType != SkiaCacheType.Image
                    && UsingCacheType == SkiaCacheType.ImageComposite)
                    DisposeObject(kill);
            }
        }
    }
    CachedObject _renderObjectPrevious;


    /// <summary>
    /// The cached representation of the control. Will be used on redraws without calling Paint etc, until the control is requested to be updated.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CachedObject RenderObject
    {
        get
        {
            return _renderObject;
        }
        set
        {
            RenderObjectNeedsUpdate = false;
            if (_renderObject != value)
            {
                //lock both RenderObjectPrevious and RenderObject
                lock (LockDraw)
                {
                    if (_renderObject != null) //if we already have something in actual cache then
                    {
                        if (UsesCacheDoubleBuffering
                            //|| UsingCacheType == SkiaCacheType.Image //to just reuse same surface
                            || UsingCacheType == SkiaCacheType.ImageComposite)
                        {
                            RenderObjectPrevious = _renderObject; //send it to back for special cases
                        }
                        else
                        {
                            DisposeObject(_renderObject);
                        }
                    }
                    _renderObject = value;
                    OnPropertyChanged();

                    if (value != null)
                        OnCacheCreated();

                    Monitor.PulseAll(LockDraw);
                }

            }
        }
    }
    CachedObject _renderObject;

    protected virtual void OnCacheCreated()
    {
        CreatedCache?.Invoke(this, RenderObject);
    }

    /// <summary>
    /// Indended to prohibit background rendering, useful for streaming controls like camera, gif etc. SkiaBackdrop has it set to True as well.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool CanUseCacheDoubleBuffering => true;

    /// <summary>
    /// Read-only computed flag for internal use.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool UsesCacheDoubleBuffering
    {
        get
        {
            return CanUseCacheDoubleBuffering
                   && Super.Multithreaded
                   //&& Parent is SkiaControl
                   || UsingCacheType == SkiaCacheType.ImageDoubleBuffered;
        }
    }

    public event EventHandler<CachedObject> CreatedCache;

    public void DestroyRenderingObject()
    {
        RenderObject = null;
    }

    protected virtual bool CheckCachedObjectValid(CachedObject cache, SKRect recordingArea, SkiaDrawingContext context)
    {
        if (cache != null)
        {
            if (cache.Bounds.Size != recordingArea.Size)
                return false;

            //check hardware context maybe changed
            if (UsingCacheType == SkiaCacheType.GPU && cache.Surface != null &&
                cache.Surface.Context != null &&
                context.Superview?.CanvasView is SkiaViewAccelerated hardware)
            {
                //hardware context might change if we returned from background..
                if (hardware.GRContext == null || cache.Surface.Context == null
                                               || (int)hardware.GRContext.Handle != (int)cache.Surface.Context.Handle)
                {
                    return false;
                }
            }

            return true;
        }
        return false;
    }

    public virtual SkiaCacheType UsingCacheType
    {
        get
        {
            if (!AllowCaching)
            {
                return SkiaCacheType.None;
            }

            if (CanUseCacheDoubleBuffering && Super.Multithreaded)
            {
                //if (Parent is SkiaControl)
                {
                    if (UseCache == SkiaCacheType.None)
                        return SkiaCacheType.OperationsFull;
                }

                if (UseCache == SkiaCacheType.ImageDoubleBuffered || UseCache == SkiaCacheType.GPU)
                    return SkiaCacheType.Image;

                if (UseCache == SkiaCacheType.ImageComposite)
                    return SkiaCacheType.Operations;
            }

            if (UseCache == SkiaCacheType.GPU && !Super.GpuCacheEnabled)
                return SkiaCacheType.Image;

            //if (EffectPostRenderer != null 
            //    && (UseCache == SkiaCacheType.None || UseCache == SkiaCacheType.Operations))
            //    return SkiaCacheType.Image;

            if (UseCache == SkiaCacheType.None && CanUseCacheDoubleBuffering && Super.Multithreaded && Parent is SkiaControl)
                return SkiaCacheType.Operations;

            return UseCache;
        }
    }

    protected virtual CachedObject CreateRenderingObject(
    SkiaDrawingContext context,
    SKRect drawingArea,
    SKRect recordingArea,
    CachedObject reuseSurfaceFrom,
    Action<SkiaDrawingContext> action)
    {
        if (recordingArea.Height == 0 || recordingArea.Width == 0 || IsDisposed || IsDisposing)
        {
            return null;
        }

        CachedObject renderObject = null;

        try
        {
            var recordArea = GetCacheArea(recordingArea);

            //todo
            //if (UsingCacheType == SkiaCacheType.OperationsFull)
            //{
            //    recordArea = destination;
            //}

            NeedUpdate = false; //if some child changes this while rendering to cache we will erase resulting RenderObject

            var usingCacheType = UsingCacheType;

            GRContext grContext = null;

            if (IsCacheImage)
            {
                var width = (int)recordArea.Width;
                var height = (int)recordArea.Height;

                bool needCreateSurface = !CheckCachedObjectValid(reuseSurfaceFrom, recordingArea, context) || usingCacheType == SkiaCacheType.GPU; //never reuse GPU surfaces

                SKSurface surface = null;

                if (!needCreateSurface)
                {
                    //reusing existing surface
                    surface = reuseSurfaceFrom.Surface;
                    if (surface == null)
                    {
                        return null; //would be unexpected
                    }
                    if (usingCacheType != SkiaCacheType.ImageComposite)
                        surface.Canvas.Clear();
                }
                else
                {

                    var kill = surface;

                    var cacheSurfaceInfo = new SKImageInfo(width, height);

                    if (usingCacheType == SkiaCacheType.GPU)
                    {
                        if (context.Superview != null && context.Superview?.CanvasView is SkiaViewAccelerated accelerated
                            && accelerated.GRContext != null)
                        {
                            grContext = accelerated.GRContext;
                            //hardware accelerated
                            surface = SKSurface.Create(accelerated.GRContext,
                                false,
                                cacheSurfaceInfo);
                        }
                    }
                    if (surface == null) //fallback if gpu failed
                    {
                        //non-gpu
                        surface = SKSurface.Create(cacheSurfaceInfo);
                    }

                    if (kill != surface)
                        DisposeObject(kill);

                    // if (usingCacheType == SkiaCacheType.GPU)
                    //     surface.Canvas.Clear(SKColors.Red);
                }

                if (surface == null)
                {
                    return null; //would be totally unexpected
                }

                var recordingContext = context.CreateForRecordingImage(surface, recordArea.Size);

                recordingContext.IsRecycled = !needCreateSurface;

                // Translate the canvas to start drawing at (0,0)

                recordingContext.Canvas.Translate(-recordArea.Left, -recordArea.Top);

                // Perform the drawing action
                action(recordingContext);

                surface.Canvas.Flush();
                //grContext?.Flush();

                recordingContext.Canvas.Translate(recordArea.Left, recordArea.Top);

                renderObject = new(usingCacheType, surface, recordArea, recordArea)
                {
                    SurfaceIsRecycled = recordingContext.IsRecycled
                };

            }
            else
            if (IsCacheOperations)
            {
                var cacheRecordingArea = GetCacheRecordingArea(recordingArea);

                using (var recorder = new SKPictureRecorder())
                {
                    var recordingContext = context.CreateForRecordingOperations(recorder, cacheRecordingArea);

                    action(recordingContext);

                    // End the recording and obtain the SKPicture
                    var skPicture = recorder.EndRecording();

                    renderObject = new(UsingCacheType, skPicture, drawingArea, cacheRecordingArea);
                }
            }

            //else we landed here with no cache type at all..
        }
        catch (Exception e)
        {
            Super.Log(e);
        }

        return renderObject;
    }

    public Action<CachedObject, SkiaDrawingContext, SKRect> DelegateDrawCache { get; set; }

    protected virtual void DrawRenderObjectInternal(
        CachedObject cache,
        SkiaDrawingContext ctx,
        SKRect destination)
    {
        if (DelegateDrawCache != null)
        {
            DelegateDrawCache(cache, ctx, destination);
        }
        else
        {
            DrawRenderObject(cache, ctx, destination);
        }
    }

    public bool IsCacheImage
    {
        get
        {
            var cache = UsingCacheType;
            return cache == SkiaCacheType.Image
                   || cache == SkiaCacheType.GPU
                   || cache == SkiaCacheType.ImageComposite
                   || cache == SkiaCacheType.ImageDoubleBuffered;
        }
    }

    public bool IsCacheOperations
    {
        get
        {
            var cache = UsingCacheType;
            return cache == SkiaCacheType.Operations
                   || cache == SkiaCacheType.OperationsFull;
        }
    }


    protected virtual bool UseRenderingObject(SkiaDrawingContext context, SKRect drawingArea, SKRect recordArea, float scale)
    {

        //lock (LockDraw)
        {
            var cache = RenderObject;
            var cacheOffscreen = RenderObjectPrevious;

            if (RenderObjectPrevious != null && RenderObjectPreviousNeedsUpdate)
            {
                var kill = RenderObjectPrevious;
                RenderObjectPrevious = null;
                RenderObjectPreviousNeedsUpdate = false;

                if (kill != null)
                {
                    DisposeObject(kill);
                }
            }

            if (cache != null)
            {
                if (!UsesCacheDoubleBuffering && !CheckCachedObjectValid(cache, recordArea, context))
                {
                    return false;
                }

                //draw existing front cache
                lock (LockDraw)
                {
                    DrawRenderObjectInternal(cache, context, drawingArea);
                    Monitor.PulseAll(LockDraw);
                }

                if (!UsesCacheDoubleBuffering || !NeedUpdateFrontCache)
                    return true;
            }

            if (UsesCacheDoubleBuffering)
            {
                lock (LockDraw)
                {
                    if (cache == null && cacheOffscreen != null)
                    {
                        DrawRenderObjectInternal(cacheOffscreen, context, drawingArea);
                    }
                    Monitor.PulseAll(LockDraw);
                }

                NeedUpdateFrontCache = false;

                var args = CreatePaintArguments();
                PushToOffscreenRendering(() =>
                {
                    //will be executed on background thread in parallel
                    var oldObject = RenderObjectPreparing;
                    RenderObjectPreparing = CreateRenderingObject(context, drawingArea, recordArea, oldObject, (ctx) =>
                    {
                        PaintWithEffects(ctx, drawingArea, scale, args);
                    });
                    RenderObject = RenderObjectPreparing;
                    _renderObjectPreparing = null;

                    if (Parent != null && !Parent.UpdateLocked)
                    {
                        Parent?.UpdateByChild(this); //repaint us
                    }
                });

                return !NeedUpdateFrontCache;
            }

            return false;
        }

    }

    public Action GetOffscreenRenderingAction()
    {
        var action = _offscreenCacheRenderingQueue.Pop();
        return action;
    }

    public void PushToOffscreenRendering(Action action)
    {
        if (Super.OffscreenRenderingAtCanvasLevel)
        {
            _offscreenCacheRenderingQueue.Push(action);
            Superview?.PushToOffscreenRendering(this);
        }
        else
        {
            _offscreenCacheRenderingQueue.Push(action);
            if (!_processingOffscrenRendering)
            {
                _processingOffscrenRendering = true;
                Task.Run(async () =>
                {
                    await ProcessOffscreenCacheRenderingAsync();
                }).ConfigureAwait(false);
            }
        }
    }



    private bool _processingOffscrenRendering = false;
    protected SemaphoreSlim semaphoreOffsecreenProcess = new(1);

    public async Task ProcessOffscreenCacheRenderingAsync()
    {

        await semaphoreOffsecreenProcess.WaitAsync();

        if (_offscreenCacheRenderingQueue.Count == 0)
            return;

        _processingOffscrenRendering = true;

        try
        {
            Action action = _offscreenCacheRenderingQueue.Pop();
            while (!IsDisposed && !IsDisposing && action != null)
            {
                try
                {
                    action.Invoke();

                    if (_offscreenCacheRenderingQueue.Count > 0)
                        action = _offscreenCacheRenderingQueue.Pop();
                    else
                        break;
                }
                catch (Exception e)
                {
                    Super.Log(e);
                }
            }

            //if (NeedUpdate || RenderObjectNeedsUpdate) //someone changed us while rendering inner content
            //{
            //    Update(); //kick
            //}

        }
        finally
        {
            _processingOffscrenRendering = false;
            semaphoreOffsecreenProcess.Release();
        }

    }



    /// <summary>
    /// Used by the UseCacheDoubleBuffering process. This is the new cache beign created in background. It will be copied to RenderObject when ready.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CachedObject RenderObjectPreparing
    {
        get
        {
            return _renderObjectPreparing;
        }
        set
        {
            RenderObjectNeedsUpdate = false;
            if (_renderObjectPreparing != value)
            {
                _renderObjectPreparing = value;
            }
        }
    }
    CachedObject _renderObjectPreparing;


    /// <summary>
    /// Used by ImageDoubleBuffering cache
    /// </summary>
    protected bool NeedUpdateFrontCache
    {
        get => _needUpdateFrontCache;
        set => _needUpdateFrontCache = value;
    }



    private bool _RenderObjectPreviousNeedsUpdate;
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool RenderObjectPreviousNeedsUpdate
    {
        get
        {
            return _RenderObjectPreviousNeedsUpdate;
        }
        set
        {
            if (_RenderObjectPreviousNeedsUpdate != value)
            {
                _RenderObjectPreviousNeedsUpdate = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Should delete RenderObject when starting new frame rendering
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool RenderObjectNeedsUpdate
    {
        get
        {
            return _renderObjectNeedsUpdate;
        }

        set
        {
            if (_renderObjectNeedsUpdate != value)
            {
                _renderObjectNeedsUpdate = value;
            }
        }
    }
    bool _renderObjectNeedsUpdate = true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void InvalidateCache()
    {
        RenderObjectNeedsUpdate = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void InvalidateCacheWithPrevious()
    {
        RenderObjectNeedsUpdate = true;
        if (UsingCacheType == SkiaCacheType.ImageComposite)
        {
            RenderObjectPreviousNeedsUpdate = true;
        }
    }


    /// <summary>
    /// Used for the Operations cache type to record inside the changed area, if your control is not inside the DrawingRect due to transforms/translations. This is NOT changing the rendering object 
    /// </summary>
    protected virtual SKRect GetCacheRecordingArea(SKRect drawingRect)
    {
        return drawingRect;
    }

    /// <summary>
    /// Normally cache is recorded inside DrawingRect, but you might want to exapnd this to include shadows around, for example.
    /// </summary>
    protected virtual SKRect GetCacheArea(SKRect drawingRect)
    {
        var pixels = ExpandCacheRecordingArea * RenderingScale;
        return new SKRect(
            (float)Math.Round(drawingRect.Left - pixels),
            (float)Math.Round(drawingRect.Top - pixels),
            (float)Math.Round(drawingRect.Right + pixels),
            (float)Math.Round(drawingRect.Bottom + pixels));
    }

    /// <summary>
    /// Returns true if had drawn.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="widthRequest"></param>
    /// <param name="heightRequest"></param>
    /// <param name="destination"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    protected virtual bool DrawUsingRenderObject(SkiaDrawingContext context,
        float widthRequest, float heightRequest,
        SKRect destination,
        float scale)
    {
        Arrange(destination, widthRequest, heightRequest, scale);

        bool willDraw = !CheckIsGhost();
        if (willDraw)
        {
            if (UsingCacheType != SkiaCacheType.None)
            {
                var drawingArea = DrawingRect;
                var recordArea = drawingArea;
                if (UsingCacheType == SkiaCacheType.OperationsFull)
                {
                    //recordArea = destination;
                    recordArea = context.Canvas.LocalClipBounds;
                }

                //paint from cache
                if (!UseRenderingObject(context, drawingArea, recordArea, scale))
                {
                    //record to cache and paint 
                    if (UsesCacheDoubleBuffering)
                    {
                        var args = CreatePaintArguments();
                        PushToOffscreenRendering(() =>
                        {
                            //will be executed on background thread in parallel
                            var oldObject = RenderObjectPreparing;

                            RenderObjectPreparing = CreateRenderingObject(context, drawingArea, recordArea, oldObject, (ctx) =>
                            {
                                PaintWithEffects(ctx, drawingArea, scale, args);
                            });
                            RenderObject = RenderObjectPreparing;
                            _renderObjectPreparing = null;

                            if (Parent != null && !Parent.UpdateLocked)
                            {
                                Parent?.UpdateByChild(this); //repaint us
                            }
                        });
                    }
                    else
                        CreateRenderingObjectAndPaint(context, drawingArea, recordArea, (ctx) =>
                        {
                            PaintWithEffects(ctx, DrawingRect, scale, CreatePaintArguments());
                        });
                }
            }
            else
            {
                DrawWithClipAndTransforms(context, DrawingRect, DrawingRect, true, true, (ctx) =>
                {
                    PaintWithEffects(ctx, DrawingRect, scale, CreatePaintArguments());

                    if (EffectPostRenderer != null)
                    {
                        EffectPostRenderer.Render(ctx, destination);
                    }

                });
            }
        }

        FinalizeDrawingWithRenderObject(context, scale); //NeedUpdate will go false

        return willDraw;
    }

    /// <summary>
    /// This is NOT calling FinalizeDraw()!
    /// parameter 'area' Usually is equal to DrawingRect
    /// </summary>
    /// <param name="context"></param>
    /// <param name="recordArea"></param>
    /// <param name="action"></param>
    protected void CreateRenderingObjectAndPaint(
        SkiaDrawingContext context,
        SKRect drawingArea,
        SKRect recordingArea,
        Action<SkiaDrawingContext> action)
    {

        if (recordingArea.Width <= 0 || recordingArea.Height <= 0 || float.IsInfinity(recordingArea.Height) || float.IsInfinity(recordingArea.Width) || IsDisposed || IsDisposing)
        {
            return;
        }

        if (RenderObject != null && !UsesCacheDoubleBuffering)
        {
            //we might come here with an existing RenderingObject if UseRenderingObject returned False
            RenderObject = null;
        }

        RenderObjectNeedsUpdate = false;

        var usingCacheType = UsingCacheType;

        CachedObject oldObject = null; //reusing this
        if (UsesCacheDoubleBuffering)
        {
            oldObject = RenderObject;
        }
        else
        if (usingCacheType == SkiaCacheType.Image
        || usingCacheType == SkiaCacheType.ImageComposite)
        {
            oldObject = RenderObjectPrevious;
        }

        var created = CreateRenderingObject(context, drawingArea, recordingArea, oldObject, action);

        if (created == null)
        {
            return;
        }

        if (oldObject != null)
        {
            if (created.SurfaceIsRecycled)
            {
                oldObject.Surface = null;
            }
            if (!UsesCacheDoubleBuffering && usingCacheType != SkiaCacheType.ImageComposite)
            {
                DisposeObject(oldObject);
            }
        }

        var notValid = RenderObjectNeedsUpdate;
        RenderObject = created;


        if (RenderObject != null)
        {
            DrawRenderObjectInternal(RenderObject, context, RenderObject.RecordingArea);
        }
        else
        {
            notValid = true;
        }


        if (NeedUpdate || notValid) //someone changed us while rendering inner content
        {
            InvalidateCache();
            //Update();
        }
    }


}



