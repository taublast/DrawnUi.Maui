using AppoMobi.Maui.Gestures;
using AppoMobi.Specials;
using System.Collections.Concurrent;

namespace Sandbox.Views.Controls;

public class MultiRippleWithTouchEffect : SkiaShaderEffect, IStateEffect, ISkiaGestureProcessor
{
    public MultiRippleWithTouchEffect()
    {
        ShaderSource = "Shaders/ripples.sksl";
    }

    bool _initialized;
    private PointF _mouse;
    private SkiaControl _controlSource;

    public void UpdateState()
    {
        if (Parent != null && !_initialized && Parent.IsLayoutReady)
        {
            _initialized = true;
        }

        base.Update();
    }

    public override void Attach(SkiaControl parent)
    {
        base.Attach(parent);

        UpdateState();
    }

    protected override SKRuntimeEffectUniforms CreateUniforms(SKRect destination)
    {
        var uniforms = base.CreateUniforms(destination);

        var activeRipples = GetActiveRipples();

        var mouseArray = new float[10 * 2];
        var progressArray = new float[10];

        for (int i = 0; i < 10; i++)
        {
            if (i < activeRipples.Count)
            {
                var ripple = activeRipples[i];
                mouseArray[i * 2] = ripple.Origin.X;
                mouseArray[i * 2 + 1] = ripple.Origin.Y;
                progressArray[i] = (float)ripple.Progress;
            }
            else
            {
                mouseArray[i * 2] = 0;
                mouseArray[i * 2 + 1] = 0;
                progressArray[i] = -1f; // inactive
            }
        }

        uniforms["origins"] = mouseArray;
        uniforms["progresses"] = progressArray;

        //was for just one ripple
        //uniforms["progress"] = (float)Progress;
        //uniforms["iMouse"] = new[] { _mouse.X, _mouse.Y, 0f, 0f };

        return uniforms;
    }

    #region REFLECTION

    private SKShader _textureBackground;

    protected override SKRuntimeEffectChildren CreateTexturesUniforms(SkiaDrawingContext ctx, SKRect destination, SKImage snapshot)
    {
        var texture2 = GetReflectionTexture();

        if (snapshot != null && texture2 != null)
        {
            var texture1 = snapshot.ToShader();

            return new SKRuntimeEffectChildren(CompiledShader)
            {
                { "iImage1", texture1 }, //background
                { "iImage2", texture2 } //reflection
            };
        }
        else
        {
            return new SKRuntimeEffectChildren(CompiledShader)
            {
            };
        }
    }

    private void OnCacheCreatedTo(object sender, CachedObject e)
    {
        Update();
    }

    #region ReflectionFromFile

    public static readonly BindableProperty BacksideSourceProperty = BindableProperty.Create(
        nameof(BacksideSource),
        typeof(string),
        typeof(MultiRippleWithTouchEffect),
        defaultValue: null,
        propertyChanged: ApplyBacksideSourceProperty);

    public string BacksideSource
    {
        get { return (string)GetValue(BacksideSourceProperty); }
        set { SetValue(BacksideSourceProperty, value); }
    }

    private static void ApplyBacksideSourceProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (oldvalue != newvalue && bindable is MultiRippleWithTouchEffect control)
        {
            control.ApplyBacksideSource((string)newvalue);
        }
    }

    void ApplyBacksideSource(string source)
    {
        Task.Run(async () =>
        {
            await LoadSource(source);
            if (ParentReady() && _loadedReflectionBitmap != null)
            {
                SetBackside();
            }

        });
    }

    private bool _reflectionSet;

    public void SetBackside()
    {
        SKImage image = null;

        if (_loadedReflectionBitmap != null)
        {
            var outRect = Parent.DrawingRect;
            var info = new SKImageInfo((int)outRect.Width, (int)outRect.Height);
            var resizedBitmap = new SKBitmap(info);
            using (var canvas = new SKCanvas(resizedBitmap))
            {
                // This will stretch the original image to fill the new size
                var rect = new SKRect(0, 0, (int)outRect.Width, (int)outRect.Height);
                canvas.DrawBitmap(_loadedReflectionBitmap, rect);
                canvas.Flush();
            }

            image = SKImage.FromBitmap(resizedBitmap);
        }

        var dispose = _textureReflection;

        if (image != null)
        {
            _textureReflection = image.ToShader(SKShaderTileMode.Mirror, SKShaderTileMode.Mirror); ;
        }

        if (dispose != _textureReflection)
            dispose?.Dispose();

        _reflectionSet = true;

        Update();
    }

    private SKShader _textureReflection;

    SKShader GetReflectionTexture()
    {
        if (!_reflectionSet && ParentReady())
        {
            SetBackside();
        }
        return _textureReflection;
    }

    //todo move this to helper whatever to share code

    protected bool ParentReady()
    {
        return !(Parent == null || Parent.DrawingRect.Width <= 0 || Parent.DrawingRect.Height <= 0);
    }

    private SemaphoreSlim _semaphoreLoadFile = new(1, 1);

    /// <summary>
    /// Loading from local files only
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>
    public async Task LoadSource(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return;

        await _semaphoreLoadFile.WaitAsync();

        try
        {
            if (fileName.SafeContainsInLower("file://"))
            {
                var fullFilename = fileName.Replace("file://", "", StringComparison.InvariantCultureIgnoreCase);
                using var stream = new FileStream(fullFilename, System.IO.FileMode.Open);
                _loadedReflectionBitmap = SKBitmap.Decode(stream);
            }
            else
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                _loadedReflectionBitmap = SKBitmap.Decode(stream);
            }

            _reflectionSet = false;
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine($"LoadSource failed to load animation {fileName}");
            Console.WriteLine(e);
            return;
        }
        finally
        {
            _semaphoreLoadFile.Release();
        }
    }

    protected override void OnDisposing()
    {
        base.OnDisposing();

        _textureReflection?.Dispose();
        _loadedReflectionBitmap?.Dispose();
    }

    SKBitmap _loadedReflectionBitmap;

    #endregion


    #region ReflectionFromControl



    /*

     SKShader GetReflectionTexture()
       {
           if (ReflectionSourceControl == null || ReflectionSourceControl.RenderObject == null)
           {
               return null;
           }
           return ReflectionSourceControl.RenderObject.Image.ToShader(SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);;;
       }

    void DetachTo()
    {
        if (_controlSource != null)
        {
            _controlSource.CreatedCache -= OnCacheCreatedTo;
            _controlSource = null;
        }
    }

    private static void ApplyReflectionSourceControlProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (oldvalue != newvalue && bindable is MultiRippleWithTouchEffect control)
        {
            control.ApplyReflectionSourceControl(newvalue as SkiaControl);
        }
    }

    void ApplyReflectionSourceControl(SkiaControl control)
    {
        if (_controlSource == control)
            return;

        DetachTo();
        _controlSource = control;
        if (_controlSource != null)
        {
            _controlSource.CreatedCache += OnCacheCreatedTo;
        }
    }

    public static readonly BindableProperty ReflectionSourceControlProperty = BindableProperty.Create(
        nameof(ReflectionSourceControl),
        typeof(SkiaControl), typeof(TestLoopEffect),
        null,
        propertyChanged: ApplyReflectionSourceControlProperty);

    public SkiaControl ReflectionSourceControl
    {
        get { return (SkiaControl)GetValue(ReflectionSourceControlProperty); }
        set { SetValue(ReflectionSourceControlProperty, value); }
    }

    protected override void OnDisposing()
    {
        base.OnDisposing();
        DetachTo();
    }
    */

    #endregion

    #endregion

    #region RIPPLES

    public class Ripple
    {
        public Guid Uid { get; set; }
        public PointF Origin { get; set; }
        public long Time { get; set; }
        public double Progress { get; set; }
    }

    ConcurrentDictionary<Guid, Ripple> Ripples = new();

    public Ripple CreateRipple(PointF origin)
    {
        var ripple = new Ripple
        {
            Uid = Guid.NewGuid(),
            Origin = origin,
            Time = Super.GetCurrentTimeNanos()
        };
        Ripples[ripple.Uid] = ripple;
        return ripple;
    }

    public void RemoveRipple(Guid key)
    {
        Ripples.TryRemove(key, out _);
    }


    public List<Ripple> GetActiveRipples()
    {
        return Ripples.Values.OrderByDescending(x => x.Time).Take(10).ToList();
    }

    public virtual ISkiaGestureListener ProcessGestures(
        SkiaGesturesParameters args,
        GestureEventProcessingInfo apply)
    {
        _mouse = args.Event.Location;

        if (args.Type == TouchActionResult.Down && _initialized)
        {

            var ripple = CreateRipple(_mouse);

            //run new animator for every Down
            //we use this helper task so that every new rangeanimator is disposed properly at the end
            Task.Run(async () =>
            {
                await Parent.AnimateRangeAsync((v) =>
                {
                    ripple.Progress = v;
                    Update();
                }, 0, 1, 4500);

                RemoveRipple(ripple.Uid);

            }).ConfigureAwait(false);

        }

        return null;
    }

    #endregion
}