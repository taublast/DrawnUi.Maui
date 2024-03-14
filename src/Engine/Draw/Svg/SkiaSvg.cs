using Svg.Skia;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace DrawnUi.Maui.Draw
{
    public class SkiaSvg : SkiaControl
    {

        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(SkiaSvg),
            Colors.Transparent,
            propertyChanged: NeedDraw);
        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }

        #region SHADOW

        //-------------------------------------------------------------
        // ShadowColor
        //-------------------------------------------------------------
        private const string nameShadowColor = "ShadowColor";
        public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(nameShadowColor, typeof(Color), typeof(SkiaSvg),
            Colors.Transparent,
            propertyChanged: NeedDraw);
        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }

        //-------------------------------------------------------------
        // ShadowX
        //-------------------------------------------------------------
        private const string nameShadowX = "ShadowX";
        public static readonly BindableProperty ShadowXProperty = BindableProperty.Create(nameShadowX, typeof(double), typeof(SkiaSvg),
            2.0,
            propertyChanged: NeedDraw);
        public double ShadowX
        {
            get { return (double)GetValue(ShadowXProperty); }
            set { SetValue(ShadowXProperty, value); }
        }

        //-------------------------------------------------------------
        // ShadowY
        //-------------------------------------------------------------
        private const string nameShadowY = "ShadowY";
        public static readonly BindableProperty ShadowYProperty = BindableProperty.Create(nameShadowY, typeof(double), typeof(SkiaSvg),
            2.0,
            propertyChanged: NeedDraw);
        public double ShadowY
        {
            get { return (double)GetValue(ShadowYProperty); }
            set { SetValue(ShadowYProperty, value); }
        }

        //-------------------------------------------------------------
        // ShadowBlur
        //-------------------------------------------------------------
        private const string nameShadowBlur = "ShadowBlur";
        public static readonly BindableProperty ShadowBlurProperty = BindableProperty.Create(nameShadowBlur, typeof(double), typeof(SkiaSvg),
            5.0,
            propertyChanged: NeedDraw);
        public double ShadowBlur
        {
            get { return (double)GetValue(ShadowBlurProperty); }
            set { SetValue(ShadowBlurProperty, value); }
        }



        #endregion

        void AddShadow(SKPaint paint, double scale)
        {
            if (ShadowColor != Colors.Transparent)
            {
                paint.ImageFilter = SKImageFilter.CreateDropShadow(
                    (float)Math.Round(ShadowX * scale), (float)Math.Round(ShadowY * scale), (float)(ShadowBlur), (float)(ShadowBlur),
                    ShadowColor.ToSKColor());
            }
        }

        public SkiaSvg()
        {
            UseCache = SkiaCacheType.Operations;

            _assembly = Application.Current?.GetType()?.Assembly;//(); //Assembly.GetExecutingAssembly();

            if (_assembly == null)
            {

            }

            _part1 = _assembly.GetName().Name + $".Resources.Images.";
        }


        protected static void NeedUpdateIcon(BindableObject bindable, object oldvalue, object newvalue)
        {

            var control = bindable as SkiaSvg;
            {
                if (control != null && !control.IsDisposed)
                {
                    control.UpdateIcon();
                }
            }
        }

        #region PROPERTIES

        public static readonly BindableProperty AspectProperty = BindableProperty.Create(
            nameof(Aspect),
            typeof(TransformAspect),
            typeof(SkiaSvg),
            TransformAspect.AspectFitFill,
            propertyChanged: NeedDraw);

        public TransformAspect Aspect
        {
            get { return (TransformAspect)GetValue(AspectProperty); }
            set { SetValue(AspectProperty, value); }
        }

        public static readonly BindableProperty UseGradientProperty = BindableProperty.Create(
            nameof(UseGradient),
            typeof(bool),
            typeof(SkiaSvg),
            false,
            propertyChanged: NeedDraw);

        public bool UseGradient
        {
            get { return (bool)GetValue(UseGradientProperty); }
            set { SetValue(UseGradientProperty, value); }
        }


        public static readonly BindableProperty FontAwesomePrimaryColorProperty = BindableProperty.Create(
            nameof(FontAwesomePrimaryColor),
            typeof(Color),
            typeof(SkiaSvg),
            Colors.Black);

        public Color FontAwesomePrimaryColor
        {
            get { return (Color)GetValue(FontAwesomePrimaryColorProperty); }
            set { SetValue(FontAwesomePrimaryColorProperty, value); }
        }

        public static readonly BindableProperty FontAwesomeSecondaryColorProperty = BindableProperty.Create(
            nameof(FontAwesomeSecondaryColor),
            typeof(Color),
            typeof(SkiaSvg),
            Colors.Black);

        public Color FontAwesomeSecondaryColor
        {
            get { return (Color)GetValue(FontAwesomeSecondaryColorProperty); }
            set { SetValue(FontAwesomeSecondaryColorProperty, value); }
        }


        public static readonly BindableProperty StartColorProperty = BindableProperty.Create(
    nameof(StartColor),
    typeof(Color),
    typeof(SkiaSvg),
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
            typeof(SkiaSvg),
            Colors.Gray,
            propertyChanged: NeedDraw);

        public Color EndColor
        {
            get { return (Color)GetValue(EndColorProperty); }
            set { SetValue(EndColorProperty, value); }
        }

        public static readonly BindableProperty GradientBlendModeProperty = BindableProperty.Create(
            nameof(GradientBlendMode),
            typeof(SKBlendMode),
            typeof(SkiaSvg),
            SKBlendMode.SrcIn,
            propertyChanged: NeedDraw);

        public SKBlendMode GradientBlendMode
        {
            get { return (SKBlendMode)GetValue(GradientBlendModeProperty); }
            set { SetValue(GradientBlendModeProperty, value); }
        }

        public static readonly BindableProperty StartXRatioProperty = BindableProperty.Create(
            nameof(StartXRatio),
            typeof(float),
            typeof(SkiaSvg),
            0.1f,
            propertyChanged: NeedDraw);

        public float StartXRatio
        {
            get { return (float)GetValue(StartXRatioProperty); }
            set { SetValue(StartXRatioProperty, value); }
        }

        public static readonly BindableProperty StartYRatioProperty = BindableProperty.Create(
            nameof(StartYRatio),
            typeof(float),
            typeof(SkiaSvg),
            1.0f,
            propertyChanged: NeedDraw);

        public float StartYRatio
        {
            get { return (float)GetValue(StartYRatioProperty); }
            set { SetValue(StartYRatioProperty, value); }
        }

        public static readonly BindableProperty EndXRatioProperty = BindableProperty.Create(
            nameof(EndXRatio),
            typeof(float),
            typeof(SkiaSvg),
            0.9f,
            propertyChanged: NeedDraw);

        public float EndXRatio
        {
            get { return (float)GetValue(EndXRatioProperty); }
            set { SetValue(EndXRatioProperty, value); }
        }

        public static readonly BindableProperty EndYRatioProperty = BindableProperty.Create(
            nameof(EndYRatio),
            typeof(float),
            typeof(SkiaSvg),
            0.1f,
            propertyChanged: NeedDraw);

        public float EndYRatio
        {
            get { return (float)GetValue(EndYRatioProperty); }
            set { SetValue(EndYRatioProperty, value); }
        }


        public static readonly BindableProperty SvgHorizontalOptionsProperty = BindableProperty.Create(
            nameof(SvgHorizontalOptions),
            typeof(LayoutAlignment),
            typeof(SkiaSvg),
            LayoutAlignment.Center);

        public LayoutAlignment SvgHorizontalOptions
        {
            get { return (LayoutAlignment)GetValue(SvgHorizontalOptionsProperty); }
            set { SetValue(SvgHorizontalOptionsProperty, value); }
        }

        public static readonly BindableProperty SvgVerticalOptionsProperty = BindableProperty.Create(
            nameof(SvgVerticalOptions),
            typeof(LayoutAlignment),
            typeof(SkiaSvg),
            LayoutAlignment.Center);

        public LayoutAlignment SvgVerticalOptions
        {
            get { return (LayoutAlignment)GetValue(SvgVerticalOptionsProperty); }
            set { SetValue(SvgVerticalOptionsProperty, value); }
        }

        public static readonly BindableProperty SvgStringProperty = BindableProperty.Create(
            nameof(SvgString),
            typeof(string),
            typeof(SkiaSvg),
            string.Empty,
            BindingMode.OneWay,
            propertyChanged: NeedUpdateIcon);

        public string SvgString
        {
            get { return (string)GetValue(SvgStringProperty); }
            set { SetValue(SvgStringProperty, value); }
        }
        /*
        public static readonly BindableProperty SvgDataProperty = BindableProperty.Create(
            nameof(SvgData),
            typeof(string),
            typeof(SkiaSvg),
            string.Empty,
            BindingMode.OneWay,
            propertyChanged: NeedUpdateIcon);

        public string SvgData
        {
            get { return (string)GetValue(SvgDataProperty); }
            set { SetValue(SvgDataProperty, value); }
        }
        */
        public static readonly BindableProperty ZoomXProperty = BindableProperty.Create(
            nameof(ZoomX),
            typeof(double),
            typeof(SkiaSvg),
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
            typeof(SkiaSvg),
            1.0,
            propertyChanged: NeedDraw);

        public double ZoomY
        {
            get { return (double)GetValue(ZoomYProperty); }
            set { SetValue(ZoomYProperty, value); }
        }

        public static readonly BindableProperty InflateAmountProperty = BindableProperty.Create(
            nameof(InflateAmount),
            typeof(double),
            typeof(SkiaSvg),
            0.0,
            propertyChanged: NeedDraw);

        public double InflateAmount
        {
            get { return (double)GetValue(InflateAmountProperty); }
            set { SetValue(InflateAmountProperty, value); }
        }

        public static readonly BindableProperty VerticalAlignmentProperty = BindableProperty.Create(
            nameof(VerticalAlignment),
            typeof(DrawImageAlignment),
            typeof(SkiaSvg),
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
            typeof(SkiaSvg),
            DrawImageAlignment.Center,
            propertyChanged: NeedDraw);

        public DrawImageAlignment HorizontalAlignment
        {
            get { return (DrawImageAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        public static readonly BindableProperty HorizontalOffsetProperty = BindableProperty.Create(
            nameof(HorizontalOffset),
            typeof(double),
            typeof(SkiaSvg),
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
            typeof(SkiaSvg),
            0.0,
            propertyChanged: NeedDraw);

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        private const string nameHasContent = "HasContent";
        public static readonly BindableProperty HasContentProperty = BindableProperty.Create(
            nameHasContent,
            typeof(bool),
            typeof(SkiaSvg),
            false,
            BindingMode.OneWayToSource);

        public bool HasContent
        {
            get { return (bool)GetValue(HasContentProperty); }
            set { SetValue(HasContentProperty, value); }
        }


        public static readonly BindableProperty IconFilePathProperty = BindableProperty.Create(
            nameof(IconFilePath),
            typeof(string),
            typeof(SkiaSvg),
            default(string), propertyChanged: NeedUpdateIcon);

        private string _part1;

        public string IconFilePath
        {
            get => (string)GetValue(IconFilePathProperty);
            set => SetValue(IconFilePathProperty, value);
        }



        //public static SKTypeface GetTypeface(string fullFontName)
        //{
        //    SKTypeface result;

        //    var assembly = Assembly.GetExecutingAssembly();
        //    var stream = assembly.GetManifestResourceStream("ClassLibrary1.Font." + fullFontName);
        //    if (stream == null)
        //        return null;

        //    result = SKTypeface.FromStream(stream);
        //    return result;
        //}

        #endregion



        private string _loadedString;

        private readonly Assembly _assembly;

        protected string LoadedString
        {
            get
            {
                return _loadedString;
            }
            set
            {
                if (_loadedString != value)
                {
                    _loadedString = value;

                    HasContent = !string.IsNullOrEmpty(value);
                }
            }
        }

        public override void OnDisposing()
        {
            LoadedString = null;

            Svg?.Dispose();
            Svg = null;

            base.OnDisposing();
        }

        public SKSvg Svg { get; protected set; }

        //protected SKMatrix DrawingMatrix;

        protected void Clear()
        {
            var svg = Svg;
            Svg = null;
            svg?.Dispose();
        }

        public void UpdateIcon()
        {
            Clear();

            if (!string.IsNullOrEmpty(SvgString))
                UpdateImageFromString(SvgString);

            if (!string.IsNullOrEmpty(LoadedString))
            {
                CreateSvg(LoadedString);
            }

            Update();
        }

        //public static readonly BindableProperty IconStringProperty = BindableProperty.Create("IconString", typeof(string), typeof(SkiaSvg), null, BindingMode.OneWay, null, NeedUpdateIcon);
        //public string IconString
        //{
        //    get
        //    {
        //        return (string)GetValue(IconStringProperty);
        //    }
        //    set
        //    {
        //        SetValue(IconStringProperty, value);
        //    }
        //}


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);


            if (propertyName == nameof(FontAwesomePrimaryColor) ||
                propertyName == nameof(FontAwesomeSecondaryColor))
            {
                UpdateIcon();
            }


            //else
            //if (propertyName == "Height" || propertyName == "Width")
            //{
            //    AnchorX = Width / 2.0f;
            //    AnchorY = Height / 2.0f;
            //}

        }

        protected void UpdateImageFromString(string source)
        {

            if (string.IsNullOrEmpty(source))
            {
                LoadedString = "";
                return;
            }

            //replace anything
            if (FontAwesomePrimaryColor != Colors.Black)
            {
                source = source.Replace("class=\"fa-primary\"", $"fill=\"{FontAwesomePrimaryColor.ToHex()}\"");
            }
            if (FontAwesomeSecondaryColor != Colors.Gray)
            {
                source = source.Replace("class=\"fa-secondary\"", $"fill=\"{FontAwesomeSecondaryColor.ToHex()}\"");
            }

            LoadedString = source;

        }

        private static void ApplySourceProperty(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaSvg control)
            {
                Task.Run(async () =>
                {
                    await control.LoadSource(control.Source);
                });
            }
        }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source),
            typeof(string),
            typeof(SkiaSvg),
            string.Empty,
            propertyChanged: ApplySourceProperty);

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private SemaphoreSlim _semaphoreLoadFile = new(1, 1);


        /// <summary>
        /// This is not replacing current animation, use SetAnimation for that.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task LoadSource(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            await _semaphoreLoadFile.WaitAsync();

            try
            {
                string json;
                if (Uri.TryCreate(fileName, UriKind.Absolute, out var uri))
                {
                    var client = new WebClient();
                    var data = await client.DownloadDataTaskAsync(uri);
                    json = Encoding.UTF8.GetString(data);
                }
                else
                {
                    using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                    using var reader = new StreamReader(stream);
                    json = await reader.ReadToEndAsync();
                }

                UpdateImageFromString(json);
                UpdateIcon();
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[SkiaSvg] LoadSource failed to load {fileName}");
                Trace.WriteLine(e);
            }
            finally
            {
                _semaphoreLoadFile.Release();
            }
        }

        protected void UpdateImageFromFile()
        {

            SvgString = "";

            if (string.IsNullOrEmpty(IconFilePath))
            {
                LoadedString = "";
                return;
            }

            var fullname = _part1 + IconFilePath;
            using (Stream file = GetType().Assembly.GetManifestResourceStream(fullname))
            {
                if (file == null)
                {
                    LoadedString = "";
                    return;
                }

                var content = "";
                using (var reader = new System.IO.StreamReader(file))
                {
                    content = reader.ReadToEnd();

                    UpdateImageFromString(content);
                }
            }

        }

        /*
        protected void UpdateImageFromFile()
        {
            //    SvgString = "";

            if (string.IsNullOrEmpty(IconFilePath))
            {
                image = "";
                return;
            }

            if (IconFilePath.StartsWith("resource://", StringComparison.OrdinalIgnoreCase))
            {
                Task.Run(() =>
                {

                    var uri = new Uri(IconFilePath);
                    Assembly assembly = null;

                    var parts = uri.OriginalString.Substring(11).Split('?');
                    var resourceName = parts.First();

                    if (parts.Count() > 1)
                    {
                        var name = Uri.UnescapeDataString(uri.Query.Substring(10));
                        var assemblyName = new AssemblyName(name);
                        assembly = Assembly.Load(assemblyName);
                    }

                    if (assembly == null)
                    {
                        var callingAssemblyMethod = typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly");
                        assembly = (Assembly)callingAssemblyMethod.Invoke(null, new object[0]);
                    }

                    var fullPath = $"{assembly.GetName().Name}.{resourceName}";

                    using (var stream = SkiaFontManager.GetEmbeddedResourceStream(assembly, fullPath))
                    {
                        if (stream == null)
                        {
                            Debug.WriteLine($"[SkiaSvg] Failed to load {fullPath} from {_assembly.FullName}");
                            return;
                        }

                        var content = "";

                        using (var reader = new System.IO.StreamReader(stream))
                        {
                            content = reader.ReadToEnd();

                            UpdateImageFromString(content);
                        }
                    }

                }).ConfigureAwait(false);

                return;
            }

            if (IconFilePath.ToLower().Contains("http://") || IconFilePath.ToLower().Contains("https://"))
            {
                Task.Run(ActionLoadFromUrl).ConfigureAwait(false);
            }
            else
            {
                // resource://{resourceName}?assembly={Uri.EscapeUriString(resourceAssembly.FullName)}";
                //if (IconFilePath.ToLower().Contains("resource://"))
                //{
                //    var uri = new Uri(IconFilePath);

                //    var text = uri.OriginalString;
                //    if (string.IsNullOrWhiteSpace(uri.Query))
                //    {
                //        if (_cachedMainAssembly == null)
                //            _cachedMainAssembly = Application.Current?.GetType()?.GetTypeAssemblyFullName();

                //        Uri = new Uri(_cachedMainAssembly == null ? text : $"{text}?assembly={Uri.EscapeUriString(_cachedMainAssembly)}");
                //    }
                //    else if (!uri.Query.Contains("assembly=", StringComparison.OrdinalIgnoreCase))
                //    {
                //        var assemblyName = Application.Current?.GetType()?.GetTypeAssemblyFullName();
                //        Uri = new Uri(assemblyName == null ? text : $"{text}?assembly={Uri.EscapeUriString(assemblyName)}");
                //    }
                //    else
                //    {
                //        Uri = uri;
                //    }
                //    return;
                //}

                var fullname = _part1 + IconFilePath;

                using (var stream = SkiaFontManager.GetEmbeddedResourceStream(_assembly, fullname))
                {
                    if (stream == null)
                    {
                        Debug.WriteLine($"[SkiaSvg] Failed to load {fullname} from {_assembly.FullName}");
                        return;
                    }

                    var content = "";

                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        content = reader.ReadToEnd();

                        UpdateImageFromString(content);
                    }
                }
            }

        }
        */
        public static Func<HttpClient> CreateHttpClient = () =>
        {
            return new HttpClient();
        };

        private async void ActionLoadFromUrl()
        {
            // throw new NotImplementedException();

            try
            {
                var client = CreateHttpClient();
                SvgString = await client.GetStringAsync(IconFilePath);

                //using (var stream = await ImageService.Instance.LoadUrl(IconFilePath).AsPNGStreamAsync())
                //{
                //    // convert stream to string
                //    StreamReader reader = new StreamReader(stream);
                //    SvgString = reader.ReadToEnd();
                //}

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public static SKRect CalculateDisplayRect(SKRect dest, float bmpWidth, float bmpHeight,
            DrawImageAlignment horizontal, DrawImageAlignment vertical)
        {
            float x = 0;
            float y = 0;

            switch (horizontal)
            {
            case DrawImageAlignment.Center:
            x = (dest.Width - bmpWidth) / 2.0f;
            break;

            case DrawImageAlignment.Start:
            break;

            case DrawImageAlignment.End:
            x = dest.Width - bmpWidth;
            break;
            }

            switch (vertical)
            {
            case DrawImageAlignment.Center:
            y = (dest.Height - bmpHeight) / 2.0f;
            break;

            case DrawImageAlignment.Start:
            break;

            case DrawImageAlignment.End:
            y = dest.Height - bmpHeight;
            break;
            }

            x += dest.Left;
            y += dest.Top;

            return new SKRect(x, y, x + bmpWidth, y + bmpHeight);
        }

        protected void DrawPicture(SKCanvas canvas, SKPicture picture, SKRect dest,
        TransformAspect stretch,
        DrawImageAlignment horizontal = DrawImageAlignment.Center,
        DrawImageAlignment vertical = DrawImageAlignment.Center,
        SKPaint paint = null)
        {



            var pxWidth = picture.CullRect.Width;
            var pxHeight = picture.CullRect.Height;

            var scaled = RescaleAspect(pxWidth, pxHeight, dest, stretch);

            var scaleX = scaled.X * (float)ZoomX;
            var scaleY = scaled.Y * (float)ZoomY;

            //MaxWidth = PixelsToDeviceUnits(scaleX * bitmap.Width);

            SKRect display = CalculateDisplayRect(dest, scaleX * pxWidth, scaleY * pxHeight,
                horizontal, vertical);

            //if (this.BlurAmount > 0)
            display.Inflate(new SKSize((float)InflateAmount, (float)InflateAmount));

            //using matrix
            //display.Offset(SkiaControl.DeviceUnitsToPixels(HorizontalOffset), SkiaControl.DeviceUnitsToPixels(VerticalOffset));

            //todo apply clipping here
            if (IsClippedToBounds || Clipping != null)
            {
                using (SKPath path = new SKPath())
                {

                    if (Clipping != null)
                    {
                        Clipping.Invoke(path, dest);
                    }
                    else
                    {
                        path.MoveTo(dest.Left, dest.Top);
                        path.LineTo(dest.Right, dest.Top);
                        path.LineTo(dest.Right, dest.Bottom);
                        path.LineTo(dest.Left, dest.Bottom);
                        path.MoveTo(dest.Left, dest.Top);
                        path.Close();
                    }

                    //var ShadowSize = 2 * scale;
                    //var ShadowBlurAmount = 5 * scale;
                    //var ColorShadow = Colors.FromHex("#cccccc");

                    //paint.IsAntialias = true;
                    //paint.ImageFilter = SKImageFilter.CreateDropShadowOnly(0, (float)ShadowSize, 0, (float)ShadowBlurAmount,
                    //    ColorShadow.ToSKColor());
                    canvas.DrawPath(path, paint);

                    paint.ImageFilter = null;

                    canvas.Save();

                    canvas.ClipPath(path, SKClipOperation.Intersect, true);

                    canvas.DrawPicture(picture, display.Left, display.Top, paint);

                    canvas.Restore();
                }
            }
            else
            {
                canvas.DrawPicture(picture, display.Left, display.Top, paint);
            }

        }



        //-------------------------------------------------------------
        // Zoom
        //-------------------------------------------------------------
        private const string nameZoom = "Zoom";
        public static readonly BindableProperty ZoomProperty = BindableProperty.Create(nameZoom, typeof(double), typeof(SkiaSvg), 1.0,
            propertyChanged: NeedDraw);
        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        protected bool CreateSvg(string loadedString)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(loadedString);
            using (Stream stream = new MemoryStream(byteArray))
            {
                var svg = new SKSvg();
                try
                {
                    svg.Load(stream);
                    Svg = svg;
                    if (Svg == null)
                    {
                        throw new Exception("[SkiaSvg] Failed to load string");
                    }
                    Update();
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
                return false;
            }
        }


        SKMatrix CreateSvgMatrix(SKRect destination, double scale)
        {

            #region Layout

            SKRect contentSize = Svg.Picture.CullRect;
            float scaledContentWidth = (float)(contentSize.Width);// * Density);
            float scaledContentHeight = (float)(contentSize.Height);// * Density);

            //multipliers to reduce
            float xRatio = destination.Width / scaledContentWidth;
            float yRatio = destination.Height / scaledContentHeight;

            var aspectX = xRatio;
            var aspectY = yRatio;
            var adjustX = 0f;
            var adjustY = 0f;

            if (Aspect == TransformAspect.Fill)
            {

                //multipliers to enlarge
                if (destination.Width > scaledContentWidth && aspectX < 1)
                {
                    aspectX = 1 + (1 - aspectX);
                }
                if (destination.Height > scaledContentHeight && aspectY < 1)
                {
                    aspectY = 1 + (1 - aspectY);
                }

                //todo can add property ResizeAnchor, actually its Center
                adjustX = (destination.Width - scaledContentWidth * aspectX) / 2.0f;
                adjustY = (destination.Height - scaledContentHeight * aspectY) / 2.0f;

            }
            else
            if (Aspect == TransformAspect.AspectFill)
            {
                var needMoreY = destination.Height - scaledContentHeight * xRatio;
                var needMoreX = destination.Width - scaledContentWidth * yRatio;
                var needMore = Math.Max(needMoreX, needMoreY);
                if (needMore > 0)
                {
                    var moreX = needMore / scaledContentWidth;
                    var moreY = needMore / scaledContentHeight;
                    xRatio += moreX;
                    yRatio += moreY;
                }

                if (destination.Width < destination.Height)
                {
                    aspectX = xRatio;
                    aspectY = xRatio;
                }
                else
                {
                    aspectX = yRatio;
                    aspectY = yRatio;
                }

                adjustX = (destination.Width - scaledContentWidth * aspectX) / 2.0f;
                adjustY = (destination.Height - scaledContentHeight * aspectY) / 2.0f;


            }
            else //FIT
            {
                //keep aspect
                var aspectFit = Math.Min(xRatio / ZoomX, yRatio / ZoomY);

                var aspectFitX = (float)(aspectFit * ZoomX * Zoom);
                var aspectFitY = (float)(aspectFit * ZoomY * Zoom);

                if (yRatio == aspectFit) // was fit for by height, need to center x
                {
                    adjustX = (destination.Width - scaledContentWidth * aspectFitX) / 2.0f;
                }
                else// was fit for by width, need to center y
                {
                    adjustY = (destination.Height - scaledContentHeight * aspectFitY) / 2.0f;
                }

                aspectX = aspectFitX;
                aspectY = aspectFitY;
            }

            var matrix = new SKMatrix
            {
                ScaleX = aspectX,
                SkewX = 0,
                TransX = destination.Left + adjustX + (float)Math.Round(HorizontalOffset * scale),
                SkewY = 0,
                ScaleY = aspectY,
                TransY = destination.Top + adjustY + (float)Math.Round(VerticalOffset * scale),
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1
            };

            return matrix;

            #endregion

        }


        protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
        {
            base.Paint(ctx, destination, scale, arguments); //paint background color

            var area = ContractPixelsRect(destination, scale, Padding);

            if (Svg != null)// !string.IsNullOrEmpty(LoadedString))
            {
                SKMatrix matrix = CreateSvgMatrix(area, scale);

                SKPath clipPath = null;

                if (TintColor != Colors.Transparent && !UseGradient)
                {

                    using (var paint = new SKPaint())
                    {
                        //drop shadow
                        AddShadow(paint, scale);
                        paint.IsAntialias = true;
                        paint.ColorFilter = SKColorFilter.CreateBlendMode(TintColor.ToSKColor(), SKBlendMode.SrcIn);

                        ctx.Canvas.DrawPicture(Svg.Picture, ref matrix, paint);
                    }

                }
                else
                if (UseGradient) //todo use standart gradient property instead
                {
                    ctx.Canvas.DrawPicture(Svg.Picture, ref matrix);

                    //draw gradient rectangle above
                    using (var paint = new SKPaint())
                    {
                        //drop shadow
                        AddShadow(paint, scale);
                        paint.IsAntialias = true;
                        paint.Shader = SKShader.CreateLinearGradient(
                            new SKPoint(Destination.Width * StartXRatio, Destination.Height * StartYRatio),
                            new SKPoint(Destination.Width * EndXRatio, Destination.Height * EndYRatio),
                            new SKColor[]
                            {
                                    StartColor.ToSKColor(),
                                    EndColor.ToSKColor()
                            },
                            null,
                            SKShaderTileMode.Clamp);

                        paint.BlendMode = GradientBlendMode;

                        ctx.Canvas.DrawRect(Destination, paint);
                    }
                }
                else
                {
                    using (var paint = new SKPaint())
                    {
                        paint.IsAntialias = true;
                        //drop shadow
                        AddShadow(paint, scale);

                        //                            paint.ColorFilter = SKColorFilter.CreateBlendMode(BackgroundColors.ToSKColor(), SKBlendMode.SrcIn);

                        if (Clipping != null)
                        {
                            if (clipPath == null)
                            {
                                clipPath = new SKPath();
                                Clipping.Invoke(clipPath, Destination);
                            }

                            ctx.Canvas.Save();
                            ctx.Canvas.ClipPath(clipPath, SKClipOperation.Intersect, true);

                            ctx.Canvas.DrawPicture(Svg.Picture, ref matrix, paint);

                            ctx.Canvas.Restore();

                            clipPath.Dispose();
                        }
                        else
                        {
                            ctx.Canvas.DrawPicture(Svg.Picture, ref matrix, paint);
                        }
                    }
                }
            }

        }

        protected override void Draw(SkiaDrawingContext context,
            SKRect destination, float scale)
        {
            var widthRequest = SizeRequest.Width + (float)(Margins.Left + Margins.Right);
            var heightRequest = SizeRequest.Height + (float)(Margins.Top + Margins.Bottom);

            var drawn = DrawUsingRenderObject(context,
                widthRequest, heightRequest,
                destination, scale);
        }


    }

}
