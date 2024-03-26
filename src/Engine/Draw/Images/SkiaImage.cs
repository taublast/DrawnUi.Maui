using DrawnUi.Maui.Infrastructure.Xaml;
using Microsoft.Maui;
using System.ComponentModel;


namespace DrawnUi.Maui.Draw;

/// <summary>
/// This control has IsClippedToBounds set to true by default
/// </summary>
public class SkiaImage : SkiaControl
{

    public SkiaImage()
    {

    }

    /// <summary>
    /// Do not call this directly, use InstancedBitmap prop
    /// </summary>
    /// <param name="loaded"></param>
    protected virtual LoadedImageSource SetImage(LoadedImageSource loaded)
    {
        if (loaded == ApplyNewSource)
        {
            return loaded;
        }

        var kill = ApplyNewSource;
        ApplyNewSource = loaded;
        if (kill != null)
        {
            if (SkiaImageManager.ReuseBitmaps)
                kill.Bitmap = null; //do not dispose shared cached image
            DisposeObject(kill);
        }

        Update();

        return loaded;
    }

    /// <summary>
    /// Will containt all the effects and other rendering properties applied, size will correspond to source.
    /// </summary>
    /// <returns></returns>
    public SKImage GetRenderedSource()
    {
        if (LoadedSource != null)
        {
            var info = new SKImageInfo(LoadedSource.Width, LoadedSource.Height);
            using var surface = SKSurface.Create(info);
            if (surface != null)
            {
                var context = new SkiaDrawingContext()
                {
                    Canvas = surface.Canvas,
                    Width = info.Width,
                    Height = info.Height
                };
                var destination = new SKRect(0, 0, info.Width, info.Height);
                Render(context, destination, 1);
                surface.Canvas.Flush();
                return surface.Snapshot();
            }
        }
        return null;
    }

    public override bool IsClippedToBounds => true;

    public CancellationTokenSource CancelLoading;

    public LoadedImageSource LoadedSource { get; set; }

    public int RetriesOnError { get; set; } = 3;

    protected int RetriesLeft { get; set; }


    public static bool LogEnabled = false;

    static void TraceLog(string message)
    {
        if (LogEnabled)
        {
#if WINDOWS
            Trace.WriteLine(message);
#else
            Console.WriteLine(message);
#endif
        }
    }

    #region PROPERTIES

    public static readonly BindableProperty UseAssemblyProperty = BindableProperty.Create(
        nameof(UseAssembly),
        typeof(object),
        typeof(SkiaImage),
        null);

    public object UseAssembly
    {
        get { return GetValue(UseAssemblyProperty); }
        set { SetValue(UseAssemblyProperty, value); }
    }

    public static readonly BindableProperty AspectProperty = BindableProperty.Create(
        nameof(Aspect),
        typeof(TransformAspect),
        typeof(SkiaImage),
        TransformAspect.AspectFitFill,
        propertyChanged: NeedInvalidateMeasure);

    /// <summary>
    /// Apspect to render image with, default is AspectFitFill. 
    /// </summary>
    public TransformAspect Aspect
    {
        get { return (TransformAspect)GetValue(AspectProperty); }
        set { SetValue(AspectProperty, value); }
    }

    public static readonly BindableProperty VerticalAlignmentProperty = BindableProperty.Create(
        nameof(VerticalAlignment),
        typeof(DrawImageAlignment),
        typeof(SkiaImage),
        DrawImageAlignment.Center,
        propertyChanged: NeedDraw);

    public DrawImageAlignment VerticalAlignment
    {
        get { return (DrawImageAlignment)GetValue(VerticalAlignmentProperty); }
        set { SetValue(VerticalAlignmentProperty, value); }
    }

    public static readonly BindableProperty HorizontalAlignmentProperty = BindableProperty.Create(
        nameof(HorizontalAlignment),
        typeof(DrawImageAlignment),
        typeof(SkiaImage),
        DrawImageAlignment.Center,
        propertyChanged: NeedDraw);

    public DrawImageAlignment HorizontalAlignment
    {
        get { return (DrawImageAlignment)GetValue(HorizontalAlignmentProperty); }
        set { SetValue(HorizontalAlignmentProperty, value); }
    }


    //private static void OnSetSource(BindableObject bindable, object oldvalue, object newvalue)
    //{
    //	if (oldvalue != newvalue)
    //	{
    //		var me = bindable as SkiaImage;
    //		me?.SetSource(newvalue as string);
    //	}
    //}

    private static void OnSetSource(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (oldvalue != newvalue)
        {
            var me = bindable as SkiaImage;
            me?.SetSource(newvalue as ImageSource);
        }
    }

    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(ImageSource),
        typeof(SkiaImage),
        defaultValue: null, propertyChanged: OnSetSource);

    [TypeConverter(typeof(FrameworkImageSourceConverter))]
    public ImageSource Source
    {
        get { return (ImageSource)GetValue(SourceProperty); }
        set { SetValue(SourceProperty, value); }
    }

    public static readonly BindableProperty ImageBitmapProperty = BindableProperty.Create(
        nameof(ImageBitmap),
        typeof(LoadedImageSource),
        typeof(SkiaImage),
        defaultValue: null,
        propertyChanged: OnSetInstancedBitmap);

    /// <summary>
    /// this is the source loaded, doesn't reflect any effects or any other rendering properties 
    /// </summary>
    public LoadedImageSource ImageBitmap
    {
        get { return (LoadedImageSource)GetValue(ImageBitmapProperty); }
        set { SetValue(ImageBitmapProperty, value); }
    }

    public static readonly BindableProperty LoadSourceOnFirstDrawProperty = BindableProperty.Create(
        nameof(LoadSourceOnFirstDraw),
        typeof(bool),
        typeof(SkiaImage),
        true,
        propertyChanged: OnLoadSourceChanged);

    /// <summary>
    /// Should the source be loaded on the first draw, useful for the first fast rendering of the screen and loading images after,
    /// default is True.
    /// Set this to False if you need an off-screen loading and to True to make the screen load faster.
    /// While images are loaded in async manner this still has its impact.
    /// Useful to set True for for SkiaCarousel cells etc..
    /// </summary>
    public bool LoadSourceOnFirstDraw
    {
        get { return (bool)GetValue(LoadSourceOnFirstDrawProperty); }
        set { SetValue(LoadSourceOnFirstDrawProperty, value); }
    }

    public static readonly BindableProperty RescalingQualityProperty = BindableProperty.Create(
        nameof(RescalingQuality),
        typeof(SKFilterQuality),
        typeof(SkiaImage),
        SKFilterQuality.Medium,
        propertyChanged: NeedInvalidateMeasure);

    /// <summary>
    /// Default value is Medium.
    /// You might want to set this to None for large a quick changing images like camera preview etc.
    /// </summary>
    public SKFilterQuality RescalingQuality
    {
        get { return (SKFilterQuality)GetValue(RescalingQualityProperty); }
        set { SetValue(RescalingQualityProperty, value); }
    }

    private static void OnLoadSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaImage control)
        {
            control.LoadSourceIfNeeded();
        }
    }

    public static readonly BindableProperty PreviewBase64Property = BindableProperty.Create(nameof(PreviewBase64), typeof(string), typeof(SkiaImage), defaultValue: string.Empty
        );
    /// <summary>
    /// If setting in code-behind must be set BEFORE you change the Source
    /// </summary>
    public string PreviewBase64
    {
        get
        {
            return (string)GetValue(PreviewBase64Property);
        }
        set
        {
            SetValue(PreviewBase64Property, value);
        }
    }

    public void SetSource(Func<CancellationToken, Task<Stream>> getStream)
    {
        var source = new StreamImageSource { Stream = getStream };
        SetImageSource(source);
    }

    public void SetSource(ImageSource source)
    {
        TraceLog($"[SkiaImage] Creating Source from {source}");
        SetImageSource(source);
    }

    public void SetSource(string source)
    {

        if (string.IsNullOrEmpty(source))
        {
            SetImageSource(null);
        }
        else
        {
            TraceLog($"[SkiaImage] Creating Source from {source}");
            //lastImageSource = source;
            var imageSource = FrameworkImageSourceConverter.FromInvariantString(source);
            //if (imageSource is UriImageSource uri)
            //{
            //	uri.CachingEnabled = true;
            //	uri.CacheValidity = TimeSpan.FromDays(1);
            //}
            SetImageSource(imageSource);
        }
    }

    public void SetFromBase64(string input)
    {
        var pixelArray = Convert.FromBase64String(input);

        var bitmap = SKBitmap.Decode(pixelArray);

        ImageBitmap = new LoadedImageSource(bitmap);
        //SetImage(new InstancedBitmap(bitmap));
    }

    protected LoadedImageSource ApplyNewSource { get; set; }

    private static void OnSetInstancedBitmap(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaImage control)
        {
            control.SetImage(newvalue as LoadedImageSource);
        }
    }


    /// <summary>
    /// Use only if you know what to do, this internally just sets the new bitmap without any invalidations and not forcing an update.
    /// You would want to set InstancedBitmap prop for a usual approach.
    /// </summary>
    /// <param name="bitmap"></param>
    public LoadedImageSource SetBitmapInternal(SKBitmap bitmap)
    {
        return SetImage(new LoadedImageSource(bitmap));
    }

    public LoadedImageSource SetImageInternal(SKImage image)
    {
        return SetImage(new LoadedImageSource(image));
    }

    private bool _IsLoading;
    public bool IsLoading
    {
        get
        {
            return _IsLoading;
        }
        set
        {
            if (_IsLoading != value)
            {
                _IsLoading = value;
                OnPropertyChanged();
            }
        }
    }

    public Action LoadSource { get; protected set; }

    public virtual void StopLoading()
    {
        CancelLoading?.Cancel();
        LoadSource = null;
    }

    public virtual void ReloadSource()
    {
        if (LastSource != null)
            SetImageSource(LastSource);
    }

    public event EventHandler OnCleared;


    public void SetImageSource(ImageSource source)
    {
        StopLoading();

        RetriesLeft = RetriesOnError;

        if (source == null)
        {
            TraceLog($"[SkiaImage] source is null");
            ClearBitmap();
            OnSuccess?.Invoke(this, new ContentLoadedEventArgs(null));
        }
        else
        {
            if (EraseChangedContent || Source == null)
            {
                ClearBitmap(); //erase old image..
            }

            if (!string.IsNullOrEmpty(PreviewBase64))
            {
                SetFromBase64(PreviewBase64);
                //Debug.WriteLine($"[SkiaImage] preview set");
            }

            if (!source.IsEmpty)
            {
                CancelLoading?.Cancel(); //todo check this out
                LastSource = source;
                string url = FrameworkImageSourceConverter.ConvertToString(source);
                var cancel = new CancellationTokenSource();
                CancelLoading = cancel;
                IsLoading = true;

                LoadSource = async () =>
                {

                    try
                    {
                        SKBitmap bitmap = null;
                        SkiaImageManager.Instance.CanReload -= OnCanReloadSource;

                        //while (bitmap == null && RetriesLeft > -1) TODO
                        {
                            if (cancel.Token.IsCancellationRequested)
                            {
                                //Debug.WriteLine($"[SkiaImage] {Id} loading canceled for {url} - ({retries})");
                                cancel?.Cancel();
                                IsLoading = false;
                                if (SkiaImageManager.LogEnabled)
                                    TraceLog($"[SkiaImage] Canceled loading {source}");
                                return;
                            }


                            async Task LoadAction()
                            {
                                try
                                {
                                    //bitmap = await SkiaImageManager.LoadImageOnPlatformAsync(source, cancel.Token);

                                    bitmap = await SkiaImageManager.Instance.LoadImageManagedAsync(source, cancel);

                                    if (cancel.Token.IsCancellationRequested)
                                    {
                                        //Debug.WriteLine($"[SkiaImage] {Id} loading canceled for {url}");
                                        cancel?.Cancel();
                                        IsLoading = false;
                                        TraceLog($"[SkiaImage] Canceled already loaded {source}");
                                        if (bitmap != null && !SkiaImageManager.ReuseBitmaps)
                                            SafeAction(() =>
                                            {
                                                bitmap.Dispose();
                                            });
                                        return;
                                    }

                                    if (bitmap != null)
                                    {
                                        CancelLoading?.Cancel();
                                        ImageBitmap = new LoadedImageSource(bitmap); //at the end will use SetImage(new InstancedBitmap(bitmap));
                                        TraceLog($"[SkiaImage] Loaded {source}");
                                        OnSuccess?.Invoke(this, new ContentLoadedEventArgs(url));
                                        OnSourceSuccess();
                                        return;
                                    }

                                    TraceLog($"[SkiaImage] Error loading {url} as {source} for tag {Tag} try {RetriesOnError - RetriesLeft + 1}");

                                    //ClearBitmap(); //erase old image anyway even if EraseChangedContent is false

                                    if (RetriesLeft > 0)
                                    {
                                        await Task.Delay(1000);
                                        RetriesLeft--;
                                        await LoadAction();
                                    }
                                }
                                catch (Exception e)
                                {
                                    Super.Log(e);
                                }
                                finally
                                {
                                    IsLoading = false;
                                }

                            }


                            await LoadAction();

                        }

                        OnError?.Invoke(this, new ContentLoadedEventArgs(url));
                        OnSourceError();

                    }
                    catch (Exception e)
                    {
                        TraceLog(e.Message);

                        OnError?.Invoke(this, new ContentLoadedEventArgs(url));
                        OnSourceError();
                    }

                };

                if (!LoadSourceOnFirstDraw)
                {
                    var action = LoadSource;
                    if (action != null)
                    {
                        if (action != null)
                        {
                            LoadSource = null;
                            Tasks.StartDelayedAsync(TimeSpan.FromMicroseconds(1), async () =>
                            {
                                await Task.Run(action);
                            });
                        }
                    }
                }
                else
                {
                    Update();
                }

            }
            else
            {
                TraceLog($"[SkiaImage] Source already loaded {source}");

                OnSuccess?.Invoke(this, new ContentLoadedEventArgs(null));
            }
        }


    }

    /// <summary>
    /// Last source that we where loading. Td be reused for reload..
    /// </summary>
    public ImageSource LastSource { get; protected set; }

    public ImageSource ImageSource { get; protected set; }

    private static void NeedChangeColorFIlter(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaImage control)
        {
            control.InvalidateColorFilter();
            NeedDraw(bindable, oldvalue, newvalue);
        }
    }
    private static void NeedChangeImageFilter(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaImage control)
        {
            control.InvalidateImageFilter();
            NeedDraw(bindable, oldvalue, newvalue);
        }
    }

    public static readonly BindableProperty AddEffectProperty = BindableProperty.Create(
nameof(AddEffect),
typeof(SkiaImageEffect),
typeof(SkiaImage),
SkiaImageEffect.None,
propertyChanged: NeedChangeColorFIlter);


    public SkiaImageEffect AddEffect
    {
        get { return (SkiaImageEffect)GetValue(AddEffectProperty); }
        set { SetValue(AddEffectProperty, value); }
    }

    public static readonly BindableProperty ColorTintProperty = BindableProperty.Create(
        nameof(ColorTint),
        typeof(Color),
        typeof(SkiaImage),
        Colors.Transparent,
        propertyChanged: NeedChangeColorFIlter);

    public Color ColorTint
    {
        get { return (Color)GetValue(ColorTintProperty); }
        set { SetValue(ColorTintProperty, value); }
    }

    public static readonly BindableProperty ZoomXProperty = BindableProperty.Create(
        nameof(ZoomX),
        typeof(double),
        typeof(SkiaImage),
        1.0,
        propertyChanged: NeedDraw);

    public double ZoomX
    {
        get { return (double)GetValue(ZoomXProperty); }
        set { SetValue(ZoomXProperty, value); }
    }

    public static readonly BindableProperty ZoomYProperty = BindableProperty.Create(
        nameof(ZoomY),
        typeof(double),
        typeof(SkiaImage),
        1.0,
        propertyChanged: NeedDraw);

    public double ZoomY
    {
        get { return (double)GetValue(ZoomYProperty); }
        set { SetValue(ZoomYProperty, value); }
    }

    public static readonly BindableProperty DarkenProperty = BindableProperty.Create(
        nameof(Darken),
        typeof(double),
        typeof(SkiaImage),
        5.0,
        propertyChanged: NeedChangeColorFIlter);

    public double Darken
    {
        get { return (double)GetValue(DarkenProperty); }
        set { SetValue(DarkenProperty, value); }
    }

    public static readonly BindableProperty LightenProperty = BindableProperty.Create(
        nameof(Lighten),
        typeof(double),
        typeof(SkiaImage),
        5.0,
        propertyChanged: NeedDraw);

    public double Lighten
    {
        get { return (double)GetValue(LightenProperty); }
        set { SetValue(LightenProperty, value); }
    }

    public static readonly BindableProperty ContrastProperty = BindableProperty.Create(
        nameof(Contrast),
        typeof(double),
        typeof(SkiaImage),
        1.0,
        propertyChanged: NeedChangeColorFIlter);

    public double Contrast
    {
        get { return (double)GetValue(ContrastProperty); }
        set { SetValue(ContrastProperty, value); }
    }

    public static readonly BindableProperty BrightnessProperty = BindableProperty.Create(
        nameof(Brightness),
        typeof(double),
        typeof(SkiaImage),
        1.0,
        propertyChanged: NeedChangeColorFIlter);

    public double Brightness
    {
        get { return (double)GetValue(BrightnessProperty); }
        set { SetValue(BrightnessProperty, value); }
    }

    public static readonly BindableProperty GammaProperty = BindableProperty.Create(
        nameof(Gamma),
        typeof(double),
        typeof(SkiaImage),
        1.0,
        propertyChanged: NeedChangeColorFIlter);

    public double Gamma
    {
        get { return (double)GetValue(GammaProperty); }
        set { SetValue(GammaProperty, value); }
    }


    public static readonly BindableProperty BlurProperty = BindableProperty.Create(
        nameof(Blur),
        typeof(double),
        typeof(SkiaImage),
        0.0,
        propertyChanged: NeedChangeImageFilter);


    public double Blur
    {
        get { return (double)GetValue(BlurProperty); }
        set { SetValue(BlurProperty, value); }
    }

    public static readonly BindableProperty SaturationProperty = BindableProperty.Create(
        nameof(Saturation),
        typeof(double),
        typeof(SkiaImage),
        0.0,
        propertyChanged: NeedChangeColorFIlter);

    public double Saturation
    {
        get { return (double)GetValue(SaturationProperty); }
        set { SetValue(SaturationProperty, value); }
    }

    public static readonly BindableProperty InflateAmountProperty = BindableProperty.Create(
        nameof(InflateAmount),
        typeof(double),
        typeof(SkiaImage),
        0.0,
        propertyChanged: NeedDraw);

    public double InflateAmount
    {
        get { return (double)GetValue(InflateAmountProperty); }
        set { SetValue(InflateAmountProperty, value); }
    }

    public static readonly BindableProperty UseGradientProperty = BindableProperty.Create(
        nameof(UseGradient),
        typeof(bool),
        typeof(SkiaImage),
        false,
        propertyChanged: NeedDraw);

    public bool UseGradient
    {
        get { return (bool)GetValue(UseGradientProperty); }
        set { SetValue(UseGradientProperty, value); }
    }

    public static readonly BindableProperty EraseChangedContentProperty = BindableProperty.Create(
        nameof(EraseChangedContent),
        typeof(bool),
        typeof(SkiaImage),
        false,
        propertyChanged: NeedDraw
        );
    /// <summary>
    /// Should we erase the existing image when another Source is set but wasn't applied yet (not loaded yet)
    /// </summary>
    public bool EraseChangedContent
    {
        get { return (bool)GetValue(EraseChangedContentProperty); }
        set { SetValue(EraseChangedContentProperty, value); }
    }

    public static readonly BindableProperty StartColorProperty = BindableProperty.Create(
        nameof(StartColor),
        typeof(Color),
        typeof(SkiaImage),
        Colors.DarkGray,
        propertyChanged: NeedDraw);

    public Color StartColor
    {
        get { return (Color)GetValue(StartColorProperty); }
        set { SetValue(StartColorProperty, value); }
    }

    public static readonly BindableProperty EndColorProperty = BindableProperty.Create(
        nameof(EndColor),
        typeof(Color),
        typeof(SkiaImage),
        Colors.Gray,
        propertyChanged: NeedDraw);

    public Color EndColor
    {
        get { return (Color)GetValue(EndColorProperty); }
        set { SetValue(EndColorProperty, value); }
    }

    public static readonly BindableProperty EffectBlendModeProperty = BindableProperty.Create(
        nameof(EffectBlendMode),
        typeof(SKBlendMode),
        typeof(SkiaImage),
        SKBlendMode.SrcIn,
        propertyChanged: NeedChangeColorFIlter);

    public SKBlendMode EffectBlendMode
    {
        get { return (SKBlendMode)GetValue(EffectBlendModeProperty); }
        set { SetValue(EffectBlendModeProperty, value); }
    }

    public static readonly BindableProperty HorizontalOffsetProperty = BindableProperty.Create(
        nameof(HorizontalOffset),
        typeof(double),
        typeof(SkiaImage),
        0.0,
        propertyChanged: NeedDraw);

    public double HorizontalOffset
    {
        get { return (double)GetValue(HorizontalOffsetProperty); }
        set { SetValue(HorizontalOffsetProperty, value); }
    }

    public static readonly BindableProperty VerticalOffsetProperty = BindableProperty.Create(
        nameof(VerticalOffset),
        typeof(double),
        typeof(SkiaImage),
        0.0,
        propertyChanged: NeedDraw);

    public double VerticalOffset
    {
        get { return (double)GetValue(VerticalOffsetProperty); }
        set { SetValue(VerticalOffsetProperty, value); }
    }

    public static readonly BindableProperty DrawWhenEmptyProperty = BindableProperty.Create(nameof(Tag),
        typeof(bool),
        typeof(SkiaImage),
        true, propertyChanged: NeedInvalidateMeasure);
    public bool DrawWhenEmpty
    {
        get { return (bool)GetValue(DrawWhenEmptyProperty); }
        set { SetValue(DrawWhenEmptyProperty, value); }
    }

    public static readonly BindableProperty SpriteHeightProperty = BindableProperty.Create(
        nameof(SpriteHeight),
        typeof(double),
        typeof(SkiaImage),
        0.0);

    public double SpriteHeight
    {
        get { return (double)GetValue(SpriteHeightProperty); }
        set { SetValue(SpriteHeightProperty, value); }
    }

    public static readonly BindableProperty SpriteWidthProperty = BindableProperty.Create(
        nameof(SpriteWidth),
        typeof(double),
        typeof(SkiaImage),
        0.0);

    public double SpriteWidth
    {
        get { return (double)GetValue(SpriteWidthProperty); }
        set { SetValue(SpriteWidthProperty, value); }
    }

    public static readonly BindableProperty SpriteIndexProperty = BindableProperty.Create(
        nameof(SpriteIndex),
        typeof(int),
        typeof(SkiaImage),
        -1);

    public int SpriteIndex
    {
        get { return (int)GetValue(SpriteIndexProperty); }
        set { SetValue(SpriteIndexProperty, value); }
    }

    #endregion

    public override bool CanDraw
    {
        get
        {
            if (Source == null)
            {
                return DrawWhenEmpty && base.CanDraw;
            }

            return base.CanDraw;
        }
    }

    public void ClearBitmap()
    {
        var safeDispose = LoadedSource;
        LoadedSource = null;
        Update(); //will delete old image and paint background

        OnCleared?.Invoke(this, null);


        if (safeDispose != null)
        {
            DisposeObject(safeDispose);
        }

    }

    public virtual void OnSourceError()
    {
        HasError = true;
    }

    public virtual void OnSourceSuccess()
    {
        HasError = false;
    }

    protected virtual void SubscribeToCanReload()
    {
        SkiaImageManager.Instance.CanReload -= OnCanReloadSource;
        SkiaImageManager.Instance.CanReload += OnCanReloadSource;
    }

    private void OnCanReloadSource(object sender, EventArgs e)
    {
        ReloadSource();
    }

    public bool HasError
    {
        get => _hasError;
        protected set
        {
            if (value == _hasError) return;
            _hasError = value;
            OnPropertyChanged();
        }
    }

    public event EventHandler<ContentLoadedEventArgs> OnError;
    public event EventHandler<ContentLoadedEventArgs> OnSuccess;


    #region RENDERiNG

    /// <summary>
    /// Reusing this
    /// </summary>
    protected SKPaint ImagePaint;

    /// <summary>
    /// Reusing this
    /// </summary>
    protected SKImageFilter PaintImageFilter;

    /// <summary>
    /// Reusing this
    /// </summary>
    protected SKColorFilter PaintColorFilter;

    protected bool NeedInvalidateImageFilter { get; set; }

    public virtual void InvalidateImageFilter()
    {
        NeedInvalidateImageFilter = true;
    }

    protected bool NeedInvalidateColorFilter { get; set; }

    public virtual void InvalidateColorFilter()
    {
        NeedInvalidateColorFilter = true;
    }

    protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
    {
        base.Paint(ctx, destination, scale, arguments);

        var source = LoadedSource;

        if (NeedInvalidateImageFilter)
        {
            NeedInvalidateImageFilter = false;
            //var d = PaintImageFilter;
            PaintImageFilter = null;
            //d?.Dispose(); //might be used in double buffered!
        }

        if (NeedInvalidateColorFilter)
        {
            NeedInvalidateColorFilter = false;
            //var d = PaintColorFilter;
            PaintColorFilter = null;
            //d?.Dispose(); //might be used in double buffered!
        }

        if (source != null && !CheckIsGhost())
        {
            if (ImagePaint == null)
            {
                ImagePaint = new()
                {
                    IsAntialias = true,
                    FilterQuality = SKFilterQuality.High
                };
            }
            else
            {
                ImagePaint.ImageFilter = PaintImageFilter;
                ImagePaint.ColorFilter = PaintColorFilter;
            }


            //ImageFilter
            if (PaintImageFilter == null && Blur > 0)
            {
                PaintImageFilter = SKImageFilter.CreateBlur((float)Blur, (float)Blur, SKShaderTileMode.Mirror);
            }

            ImagePaint.ImageFilter = PaintImageFilter;

            //ColorFilter
            if (PaintColorFilter == null)
            {
                PaintColorFilter = AddEffect switch
                {
                    SkiaImageEffect.Tint when ColorTint != Colors.Transparent
                        => SkiaImageEffects.Tint(ColorTint, EffectBlendMode),

                    SkiaImageEffect.Darken when Darken != 0.0
                        => SkiaImageEffects.Darken((float)Darken),

                    SkiaImageEffect.BlackAndWhite
                        => SkiaImageEffects.Grayscale(),

                    SkiaImageEffect.Pastel
                        => SkiaImageEffects.Pastel(),

                    SkiaImageEffect.Lighten when Lighten != 0.0
                        => SkiaImageEffects.Lighten((float)Lighten),

                    SkiaImageEffect.Sepia
                        => SkiaImageEffects.Sepia(),

                    SkiaImageEffect.InvertColors
                        => SkiaImageEffects.InvertColors(),

                    SkiaImageEffect.Gamma when Gamma >= 0
                        => SkiaImageEffects.Gamma((float)Gamma),

                    SkiaImageEffect.Contrast when Contrast >= 1.0
                        => SkiaImageEffects.Contrast((float)Contrast),

                    SkiaImageEffect.Saturation when Saturation >= 0
                        => SkiaImageEffects.Saturation((float)Saturation),

                    SkiaImageEffect.Brightness when Brightness >= 1.0
                        => SkiaImageEffects.Brightness((float)Brightness),

                    SkiaImageEffect.TSL when BackgroundColor != Colors.Transparent
                        => SkiaImageEffects.TintSL(BackgroundColor, (float)Saturation, (float)Brightness, this.EffectBlendMode),

                    SkiaImageEffect.HSL when BackgroundColor != Colors.Transparent
                        => SkiaImageEffects.HSL((float)Gamma, (float)Saturation, (float)Brightness, this.EffectBlendMode),

                    _ => null
                };
            }
            ImagePaint.ColorFilter = PaintColorFilter;

            TransformAspect stretch = Aspect;
            DrawImageAlignment horizontal = HorizontalAlignment;
            DrawImageAlignment vertical = VerticalAlignment;

            DrawSource(ctx, source, destination, scale, stretch,
                horizontal, vertical, ImagePaint);
        }
    }

    object lockDraw = new();
    private bool _hasError;

    public virtual void LoadSourceIfNeeded()
    {
        if (LoadSourceOnFirstDraw)
        {
            var action = LoadSource;
            if (action != null)
            {
                LoadSource = null;
                Tasks.StartDelayedAsync(TimeSpan.FromMicroseconds(1), async () =>
                {
                    await Task.Run(action);
                });
            }
        }
    }

    public override void Invalidate()
    {
        base.Invalidate();

        Update();
    }

    protected override void Draw(SkiaDrawingContext context,
        SKRect destination, float scale)
    {
        LoadSourceIfNeeded();

        var apply = ApplyNewSource;
        ApplyNewSource = null;
        if (apply != null && apply != LoadedSource)
        {
            var kill = LoadedSource;
            LoadedSource = apply;
            var source = LoadedSource;
            {

                if (kill != null)
                {
                    if (SkiaImageManager.ReuseBitmaps)
                        kill.Bitmap = null;
                    DisposeObject(kill);
                }

                if (NeedAutoSize)
                {
                    Invalidate(); //resize on next frame
                    return;
                }

                if (DrawingRect == SKRect.Empty || source == null)
                {
                    NeedMeasure = true;
                }
                else
                {
                    //fast insert new image into presized rect
                    SetAspectScale(source.Width, source.Height, DrawingRect, this.Aspect, scale);
                }

            }

            Update(); //gamechanger for doublebuffering and other complicated cases
        }

        var widthRequest = SizeRequest.Width + (float)(Margins.Left + Margins.Right);
        var heightRequest = SizeRequest.Height + (float)(Margins.Top + Margins.Bottom);

        var drawn = DrawUsingRenderObject(context,
            widthRequest, heightRequest,
            destination, scale);
    }

    public override void OnDisposing()
    {
        LastSource = null;
        CancelLoading?.Cancel();
        ClearBitmap();
        ImagePaint?.Dispose();
        PaintColorFilter?.Dispose();
        PaintImageFilter?.Dispose();
        LoadedSource?.Dispose();
        ImagePaint = null;
        PaintColorFilter = null;
        PaintImageFilter = null;
        LoadedSource = null;
        ScaledSource?.Dispose();
        ScaledSource = null;

        base.OnDisposing();
    }

    public SKPoint AspectScale { get; protected set; }

    public RescaledBitmap ScaledSource { get; protected set; }

    public class RescaledBitmap : IDisposable
    {
        public SKBitmap Bitmap { get; set; }
        public SKFilterQuality Quality { get; set; }
        public Guid Source { get; set; }

        public void Dispose()
        {
            Bitmap?.Dispose();
        }
    }

    protected virtual void DrawSource(
        SkiaDrawingContext ctx,
        LoadedImageSource source,
        SKRect dest,
        float scale,
        TransformAspect stretch,
        DrawImageAlignment horizontal = DrawImageAlignment.Center,
        DrawImageAlignment vertical = DrawImageAlignment.Center,
        SKPaint paint = null)
    {

        try
        {
            if (AspectScale == SKPoint.Empty)
            {
                throw new ApplicationException("AspectScale is not set");
            }

            var aspectScaleX = AspectScale.X * (float)(ZoomX);
            var aspectScaleY = AspectScale.Y * (float)(ZoomY);

            SKRect display = CalculateDisplayRect(dest,
                aspectScaleX * source.Width, aspectScaleY * source.Height,
                horizontal, vertical);

            //if (this.BlurAmount > 0)
            display.Inflate(new SKSize((float)InflateAmount, (float)InflateAmount));

            display.Offset((float)Math.Round(scale * HorizontalOffset), (float)Math.Round(scale * VerticalOffset));

            TextureScale = new(dest.Width / display.Width, dest.Height / display.Height);

            if (source.Bitmap != null)
            {
                if (this.RescalingQuality != SKFilterQuality.None)
                {
                    if (ScaledSource == null
                        || ScaledSource.Source != source.Id
                        || ScaledSource.Quality != this.RescalingQuality
                         || ScaledSource.Bitmap.Width != (int)display.Width
                         || ScaledSource.Bitmap.Height != (int)display.Height)
                    {
                        var bmp = source.Bitmap.Resize(new SKSizeI((int)display.Width, (int)display.Height), RescalingQuality);
                        var kill = ScaledSource;
                        ScaledSource = new()
                        {
                            Source = source.Id,
                            Bitmap = bmp,
                            Quality = RescalingQuality
                        };
                        kill?.Dispose(); //todo with delay?
                    }

                    if (ScaledSource != null)
                    {
                        ctx.Canvas.DrawBitmap(ScaledSource.Bitmap, display, paint);
                    }
                }
                else
                {
                    ctx.Canvas.DrawBitmap(source.Bitmap, display, paint);
                }
            }
            else
            if (source.Image != null)
                ctx.Canvas.DrawImage(source.Image, display, paint);

        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
        }
        finally
        {

        }
    }

    public SKPoint TextureScale { get; protected set; }

    public ScaledRect SourceImageSize { get; protected set; }

    private void SetAspectScale(int pxWidth, int pxHeight, SKRect dest, TransformAspect stretch, float scale)
    {
        var scaled = RescaleAspect(pxWidth, pxHeight, dest, stretch);

        SourceImageSize = ScaledRect.FromPixels(new SKRect(0, 0, pxWidth, pxHeight), scale);

        SourceWidth = SourceImageSize.Units.Width;
        SourceHeight = SourceImageSize.Units.Height;

        AspectScale = new SKPoint(scaled.X, scaled.Y);
    }

    protected ScaledSize SetMeasuredAsEmpty(float scale)
    {
        AspectScale = SKPoint.Empty;
        return SetMeasured(0, 0, scale);
    }


    //public override void Arrange(SKRect destination, float widthRequest, float heightRequest, float scale)
    //{
    //    if (IsDisposed || !CanDraw)
    //        return;

    //    if (NeedMeasure)
    //    {
    //        var adjustedDestination = CalculateLayout(destination, widthRequest, heightRequest, scale);
    //        Measure(adjustedDestination.Width, adjustedDestination.Height, scale);
    //    }

    //    base.Arrange(destination, MeasuredSize.Units.Width, MeasuredSize.Units.Height, scale);
    //}


    public override ScaledSize Measure(float widthRequest, float heightRequest, float dscale)
    {
        //background measuring or invisible or self measure from draw because layout will never pass -1
        if (IsMeasuring || !CanDraw)
        {
            return MeasuredSize;
        }

        var request = CreateMeasureRequest(widthRequest, heightRequest, dscale);
        if (request.IsSame)
        {
            return MeasuredSize;
        }

        if (request.WidthRequest == 0 || request.HeightRequest == 0)
        {
            return SetMeasured(0, 0, request.Scale);
        }

        var widthConstraint = request.WidthRequest;
        var heightConstraint = request.HeightRequest;

        if ((float.IsInfinity(widthConstraint) || float.IsInfinity(heightConstraint)) && LoadedSource != null)
        {
            var aspect = LoadedSource.Width / (double)LoadedSource.Height;
            if (widthConstraint > 0)
            {
                heightConstraint = (float)(widthConstraint / aspect);
            }
            else
            if (heightConstraint > 0)
            {
                widthConstraint = (float)(heightConstraint * aspect);
            }
        }

        if (widthConstraint < 0 || heightConstraint < 0)
        {
            //set auto-size from image using newly image dimensions
            //this is the case of one dimension being Fill or explicit and the other being Auto (-1)
            if (LoadedSource != null && (widthConstraint > 0 || heightConstraint > 0))
            {
                var aspect = LoadedSource.Width / (double)LoadedSource.Height;
                if (widthConstraint > 0)
                {
                    heightConstraint = (float)(widthConstraint / aspect);
                }
                else
                if (heightConstraint > 0)
                {
                    widthConstraint = (float)(heightConstraint * aspect);
                }
                //Invalidate(); //remeasure us - disabled works superfine so far!!!
            }
            else
            {
                //not setting NeedMeasure=false; to force remeasurement on next frame
                AspectScale = SKPoint.Empty;

                return ScaledSize.Empty;
                //return ScaledSize.FromPixels(0, 0, request.Scale);
            }
        }

        if (widthConstraint == 0 || heightConstraint == 0)
        {
            return SetMeasuredAsEmpty(request.Scale);
        }

        var constraints = GetMeasuringConstraints(new(widthConstraint, heightConstraint, request.Scale));

        //we measured no children, simulated !
        ContentSize = ScaledSize.FromPixels(constraints.Content.Width, constraints.Content.Height, request.Scale);

        if (constraints.Content.Width == 0 || constraints.Content.Height == 0)
        {
            return SetMeasuredAsEmpty(request.Scale);
        }

        var width = AdaptWidthConstraintToContentRequest(constraints.Request.Width, ContentSize, constraints.Margins.Left + constraints.Margins.Right);
        var height = AdaptHeightConstraintToContentRequest(constraints.Request.Height, ContentSize, constraints.Margins.Top + constraints.Margins.Bottom);

        if (LoadedSource != null)
        {
            SetAspectScale(LoadedSource.Width, LoadedSource.Height, constraints.Content, this.Aspect, request.Scale);

            if (NeedAutoHeight)
                height = SourceImageSize.Pixels.Height * this.AspectScale.Y;
            if (NeedAutoWidth)
                width = SourceImageSize.Pixels.Width * this.AspectScale.X;
        }
        else
        {
            AspectScale = SKPoint.Empty;
        }

        return SetMeasured(width, height, request.Scale);
    }

    #endregion



    /// <summary>
    /// From current set Source in points
    /// </summary>
    public float SourceWidth { get; protected set; }

    /// <summary>
    /// From current set Source in points
    /// </summary>
    public float SourceHeight { get; protected set; }


    public static SKRect CalculateDisplayRect(SKRect dest,
        float destWidth, float destHeight,
        DrawImageAlignment horizontal, DrawImageAlignment vertical)
    {
        float x = 0;
        float y = 0;

        switch (horizontal)
        {
        case DrawImageAlignment.Center:
        x = (dest.Width - destWidth) / 2.0f;
        break;

        case DrawImageAlignment.Start:
        break;

        case DrawImageAlignment.End:
        x = dest.Width - destWidth;
        break;
        }

        switch (vertical)
        {
        case DrawImageAlignment.Center:
        y = (dest.Height - destHeight) / 2.0f;
        break;

        case DrawImageAlignment.Start:
        break;

        case DrawImageAlignment.End:
        y = dest.Height - destHeight;
        break;
        }

        x += dest.Left;
        y += dest.Top;

        return new SKRect(x, y, x + destWidth, y + destHeight);
    }



}