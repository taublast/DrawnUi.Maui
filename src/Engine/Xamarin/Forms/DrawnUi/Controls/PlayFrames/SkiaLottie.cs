using DrawnUi.Maui.Infrastructure.Extensions;
using ExCSS;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Xamarin.Essentials;
using Animation = SkiaSharp.Skottie.Animation;
using Color = Xamarin.Forms.Color;

namespace DrawnUi.Maui.Controls;

public class SkiaLottie : AnimatedFramesRenderer
{
    /// <summary>
    ///     To avoid reloading same files multiple times..
    /// </summary>
    public static ConcurrentDictionary<string, string> CachedAnimations = new();

    public override void Stop()
    {
        base.Stop();

        SeekToDefaultFrame();
    }

    protected override void OnFinished()
    {
        base.OnFinished();

        SeekToDefaultFrame();
    }

    private bool needSeek;

    public virtual void SeekToDefaultFrame()
    {
        if (Animator != null)
        {
            lock (_lockSource)
            {
                if (IsOn)
                    Seek(DefaultFrameWhenOn);
                else
                    Seek(DefaultFrame);

                Monitor.PulseAll(_lockSource);
            }
        }
        else
        {
            needSeek = true;
        }
    }

    protected override void OnLayoutChanged()
    {
        base.OnLayoutChanged();

        if (needSeek)
        {
            needSeek = false;
            if (!IsPlaying)
            {
                SeekToDefaultFrame();
            }
        }
    }

    protected override void OnAnimatorSeeking(double frame)
    {
        if (LottieAnimation != null)
        {
            if (frame < 0)
            {
                frame = LottieAnimation.OutPoint;
            }
            LottieAnimation.SeekFrame(frame);
        }

        base.OnAnimatorSeeking(frame);
    }

    private static void ApplyDefaultFrameWhenOnProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaLottie control && !control.IsPlaying)
        {
            if (control.Animator != null && control.IsOn)
                control.Seek((int)newvalue);
        }
    }

    private static void ApplyDefaultFrameProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaLottie control && !control.IsPlaying)
        {
            if (control.Animator != null && !control.IsOn)
                control.Seek((int)newvalue);
        }
    }

    private static void ApplyIsOnProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaLottie control)
        {
            if (control.Animator != null)
            {
                var value = (bool)newvalue;
                if (control.IsPlaying || !value)
                    control.Stop();
                else
                {
                    if (control.ApplyIsOnWhenNotPlaying)
                        control.SeekToDefaultFrame();
                }
            }
        }
    }

    public static readonly BindableProperty ApplyIsOnWhenNotPlayingProperty = BindableProperty.Create(nameof(ApplyIsOnWhenNotPlaying),
    typeof(bool),
    typeof(SkiaLottie),
    true);
    public bool ApplyIsOnWhenNotPlaying
    {
        get { return (bool)GetValue(ApplyIsOnWhenNotPlayingProperty); }
        set { SetValue(ApplyIsOnWhenNotPlayingProperty, value); }
    }


    public static readonly BindableProperty DefaultFrameProperty = BindableProperty.Create(nameof(DefaultFrame),
        typeof(int),
        typeof(SkiaLottie),
        0, propertyChanged: ApplyDefaultFrameProperty
    );
    /// <summary>
    /// For the case IsOn = False. What frame should we display at start or when stopped. 0 (START) is default, can specify other number. if value is less than 0 then will seek to the last available frame (END).
    /// </summary>
    public int DefaultFrame
    {
        get => (int)GetValue(DefaultFrameProperty);
        set => SetValue(DefaultFrameProperty, value);
    }

    public static readonly BindableProperty IsOnProperty = BindableProperty.Create(nameof(IsOn),
    typeof(bool),
    typeof(SkiaLottie),
    false, propertyChanged: ApplyIsOnProperty);
    public bool IsOn
    {
        get { return (bool)GetValue(IsOnProperty); }
        set { SetValue(IsOnProperty, value); }
    }

    public static readonly BindableProperty DefaultFrameWhenOnProperty = BindableProperty.Create(nameof(DefaultFrameWhenOn),
        typeof(int),
        typeof(SkiaLottie),
        0, propertyChanged: ApplyDefaultFrameWhenOnProperty
    );
    /// <summary>
    /// For the case IsOn = True. What frame should we display at start or when stopped. 0 (START) is default, can specify other number. if value is less than 0 then will seek to the last available frame (END).
    /// </summary>
    public int DefaultFrameWhenOn
    {
        get => (int)GetValue(DefaultFrameWhenOnProperty);
        set => SetValue(DefaultFrameWhenOnProperty, value);
    }

    public static readonly BindableProperty SpeedRatioProperty = BindableProperty.Create(nameof(SpeedRatio),
        typeof(double),
        typeof(SkiaLottie),
        1.0, propertyChanged: (b, o, n) =>
        {
            if (b is SkiaLottie control) control.ApplySpeed();
        });

    public static readonly BindableProperty ColorTintProperty = BindableProperty.Create(
        nameof(ColorTint),
        typeof(Color),
        typeof(SkiaLottie),
        Xamarin.Forms.Color.Transparent, propertyChanged: ApplySourceProperty);

    public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source),
        typeof(string),
        typeof(SkiaLottie),
        string.Empty,
        propertyChanged: ApplySourceProperty);

    private readonly object _lockSource = new();
    private Animation _lottieAnimation;

    private readonly SemaphoreSlim _semaphoreLoadFile = new(1, 1);

    public Animation LottieAnimation
    {
        get => _lottieAnimation;

        set
        {
            if (_lottieAnimation != value)
            {
                _lottieAnimation = value;
                OnPropertyChanged();
            }
        }
    }

    public double SpeedRatio
    {
        get => (double)GetValue(SpeedRatioProperty);
        set => SetValue(SpeedRatioProperty, value);
    }

    public Color ColorTint
    {
        get => (Color)GetValue(ColorTintProperty);
        set => SetValue(ColorTintProperty, value);
    }

    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    #region COLORS


    public static readonly BindableProperty ColorsProperty = BindableProperty.Create(
        nameof(Colors),
        typeof(IList<Color>),
        typeof(SkiaLottie),
        defaultValueCreator: (instance) =>
        {
            var created = new ObservableCollection<Color>();
            ColorsPropertyChanged(instance, null, created);
            return created;
        },
        validateValue: (bo, v) => v is IList<Color>,
        propertyChanged: ColorsPropertyChanged,
        coerceValue: CoerceColors);


    public IList<Color> Colors
    {
        get => (IList<Color>)GetValue(ColorsProperty);
        set => SetValue(ColorsProperty, value);
    }

    private static object CoerceColors(BindableObject bindable, object value)
    {
        if (!(value is ReadOnlyCollection<Color> readonlyCollection))
        {
            return value;
        }

        return new ReadOnlyCollection<Color>(
            readonlyCollection.Select(s => s)
                .ToList());
    }

    private static void ColorsPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaLottie control)
        {
            if (oldvalue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= control.OnSkiaPropertyColorCollectionChanged;
            }
            if (newvalue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += control.OnSkiaPropertyColorCollectionChanged;
            }

            control.ReloadSource();
        }

    }

    public virtual void ReloadSource()
    {
        Tasks.StartDelayedAsync(TimeSpan.FromMilliseconds(1), async () =>
        {
            var animation = await LoadSource(Source);
            if (animation != null) SetAnimation(animation, true);
        });
    }

    private void OnSkiaPropertyColorCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        this.ReloadSource();
    }

    #endregion



    protected override void RenderFrame(SkiaDrawingContext ctx, SKRect destination, float scale,
        object arguments)
    {
        if (IsDisposing || IsDisposed)
            return;

        if (LottieAnimation is Animation lottie)
            lock (_lockSource)
            {
                lottie.Render(ctx.Canvas, DrawingRect);
                Monitor.PulseAll(_lockSource);
            }
    }

    /*
    /// <summary>
    ///     This is not replacing current animation, use SetAnimation for that.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>
    public async Task<Animation> LoadSource(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;

        await _semaphoreLoadFile.WaitAsync();

        try
        {
            string json = null;
            if (CachedAnimations.TryGetValue(fileName, out json))
            {
                Debug.WriteLine($"Loaded {fileName} from cache");
            }
            else
            {
                if (Uri.TryCreate(fileName, UriKind.Absolute, out var uri) && uri.Scheme != "file")
                {
                    var client = new WebClient();
                    var data = await client.DownloadDataTaskAsync(uri);
                    json = Encoding.UTF8.GetString(data);
                }
                else
                {
                    if (fileName.SafeContainsInLower(SkiaImageManager.NativeFilePrefix))
                    {
                        var fullFilename = fileName.Replace(SkiaImageManager.NativeFilePrefix, "");
                        using var stream = new FileStream(fullFilename, FileMode.Open);
                        using var reader = new StreamReader(stream);
                        json = await reader.ReadToEndAsync();
                    }
                    else
                    {
                        using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                        using var reader = new StreamReader(stream);
                        json = await reader.ReadToEndAsync();
                    }

                    CachedAnimations.TryAdd(fileName, json);
                }
            }

            if (ColorTint != Color.Transparent) json = ApplyTint(json, ColorTint);

            if (ProcessJson != null) json = ProcessJson(json);

            return await LoadAnimationFromJson(OnJsonLoaded(json));
        }
        catch (Exception e)
        {
            Trace.WriteLine($"[SkiaLottie] LoadSource failed to load animation {fileName}");
            Trace.WriteLine(e);
            return null;
        }
        finally
        {
            _semaphoreLoadFile.Release();
        }
    }

    public static Stream StreamFromResourceUrl(string url, Assembly assembly = null)
    {
        var uri = new Uri(url);

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
            assembly = Assembly.GetCallingAssembly();
        }

        var fullPath = $"{assembly.GetName().Name}.{resourceName}";

        Console.WriteLine($"[StreamFromResourceUrl] loading {fullPath}..");

        return assembly.GetManifestResourceStream(fullPath);
    }
    */


    /// <summary>
    ///     This is not replacing current animation, use SetAnimation for that.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>
    public async Task<Animation> LoadSource(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;

        await _semaphoreLoadFile.WaitAsync();

        try
        {
            string json = null;
            if (CachedAnimations.TryGetValue(fileName, out json))
            {
                Debug.WriteLine($"Loaded {fileName} from cache");
            }
            else
            {
                if (Uri.TryCreate(fileName, UriKind.Absolute, out var uri) && uri.Scheme != "file")
                {
                    var client = new WebClient();
                    var data = await client.DownloadDataTaskAsync(uri);
                    json = Encoding.UTF8.GetString(data);
                }
                else
                {
                    if (fileName.SafeContainsInLower(SkiaImageManager.NativeFilePrefix))
                    {
                        var fullFilename = fileName.Replace(SkiaImageManager.NativeFilePrefix, "");
                        using var stream = new FileStream(fullFilename, FileMode.Open);
                        using var reader = new StreamReader(stream);
                        json = await reader.ReadToEndAsync();
                    }
                    else
                    {
                        //using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);

                        var assembly = Super.AppAssembly;
                        if (assembly == null)
                        {
                            assembly = assembly = Assembly.GetCallingAssembly();
                        }
                        var fullPath = $"{assembly.GetName().Name}.{fileName.Replace(@"\", ".").Replace(@"/", ".")}";
                        using var stream = assembly.GetManifestResourceStream(fullPath);

                        using var reader = new StreamReader(stream);
                        json = await reader.ReadToEndAsync();
                    }

                    CachedAnimations.TryAdd(fileName, json);
                }
            }
            if (Colors.Any())
            {
                json = ApplyTint(json, this.Colors);
            }
            else
            if (ColorTint.A > 0)
            {
                json = ApplyTint(json, new List<Color>()
                {
                    ColorTint
                });
            }

            if (ProcessJson != null) json = ProcessJson(json);

            return await LoadAnimationFromJson(OnJsonLoaded(json));
        }
        catch (Exception e)
        {
            Super.Log($"[SkiaLottie] LoadSource failed to load animation {fileName}");
            return null;
        }
        finally
        {
            _semaphoreLoadFile.Release();
        }
    }

    public static Stream StreamFromResourceUrl(string url, Assembly assembly = null)
    {
        var uri = new Uri(url);

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
            assembly = assembly = Assembly.GetCallingAssembly();
        }

        var fullPath = $"{assembly.GetName().Name}.{resourceName}";

        Console.WriteLine($"[StreamFromResourceUrl] loading {fullPath}..");

        return assembly.GetManifestResourceStream(fullPath);
    }




    /// <summary>
    ///     Called by LoadAnimationFromResources after file was loaded so we can modify json if needed before it it consumed.
    ///     Return json to be used.
    ///     This is not called by LoadAnimationFromJson.
    /// </summary>
    protected virtual string OnJsonLoaded(string json)
    {
        return json;
    }

    /// <summary>
    ///     This is not replacing current animation, use SetAnimation for that.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public async Task<Animation> LoadAnimationFromJson(string json)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            var data = SKData.CreateCopy(bytes);

            if (Animation.TryCreate(data, out var animation))
                return animation;
            throw new Exception("SkiaSharp.Skottie.Animation.TryParse failed.");
        }
        catch (Exception e)
        {
            Trace.WriteLine("[SkiaLottie] failed to load animation from json");
            Trace.WriteLine(e);
        }

        return null;
    }

    public void SetAnimation(Animation animation, bool disposePrevious)
    {
        lock (_lockSource)
        {
            if (animation == null || animation == LottieAnimation || IsDisposed) return;

            var wasPlaying = IsPlaying;
            Animation kill = null;

            if (wasPlaying)
            {
                kill = LottieAnimation;
                Stop();
            }

            LottieAnimation = animation;

            if (IsOn)
            {
                OnAnimatorSeeking(DefaultFrameWhenOn);
            }
            else
            {
                OnAnimatorSeeking(DefaultFrame);
            }

            //Debug.WriteLine($"[SkiaLottie] Loaded animation: Version:{Animation.Version} Duration:{Animation.Duration} Fps:{Animation.Fps} InPoint:{Animation.InPoint} OutPoint:{Animation.OutPoint}");

            InitializeAnimator(); //autoplay applied inside

            if (wasPlaying && !IsPlaying) Start();

            if (kill != null && disposePrevious)
                Tasks.StartDelayed(TimeSpan.FromSeconds(2), () =>
                {
                    kill.Dispose();
                });

            Monitor.PulseAll(_lockSource);

        }
    }


    private void Free(Animation skottieAnimation)
    {
        skottieAnimation.Dispose();
    }

    public override void OnDisposing()
    {
        lock (_lockSource)
        {
            base.OnDisposing();

            if (LottieAnimation != null)
            {
                Free(LottieAnimation);
                LottieAnimation = null;
            }

            Monitor.PulseAll(_lockSource);
        }
    }

    protected override void OnAnimatorUpdated(double value)
    {
        base.OnAnimatorUpdated(value);

        if (LottieAnimation != null)
        {
            LottieAnimation.SeekFrame(value);
            Update();
        }
    }

    protected virtual void ApplySpeed()
    {
        if (LottieAnimation == null)
            return;

        var speed = 1.0;
        if (SpeedRatio < 1)
            speed = LottieAnimation.Duration.TotalMilliseconds * (1 + SpeedRatio);
        else
            speed = LottieAnimation.Duration.TotalMilliseconds / SpeedRatio;
        Animator.Speed = speed;
    }

    protected override void OnAnimatorInitializing()
    {
        if (LottieAnimation != null)
        {
            ApplySpeed();

            Animator.mValue = LottieAnimation.InPoint;
            Animator.mMinValue = LottieAnimation.InPoint;
            Animator.mMaxValue = LottieAnimation.OutPoint;
            Animator.Distance = Animator.mMaxValue - Animator.mMinValue;
        }
    }

    protected override bool CheckCanStartAnimator()
    {
        return LottieAnimation != null;
    }

    protected override void OnAnimatorStarting()
    {
        Animator.SetValue(LottieAnimation.InPoint);
    }

    public virtual void GoToStart()
    {
        if (LottieAnimation != null)
        {
            LottieAnimation.Seek(0.0);
            Update();
        }
    }

    public virtual void GoToEnd()
    {
        if (LottieAnimation != null)
        {
            LottieAnimation.Seek(100.0);
            Update();
        }
    }


    private static void ApplySourceProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaLottie control)
            control.ReloadSource();
    }


    #region PROCESSING

    public Func<string, string> ProcessJson;

    public static string ApplyTint(string json, IList<Color> newTints)
    {
        var lottieJson = JObject.Parse(json);
        List<(JToken Token, Color Color)> colorList = new();

        FindColorProperties(lottieJson, colorList);

        // Get the unique colors using a custom equality comparer
        var uniqueColors = colorList.Select(c => c.Color)
            .Distinct(new ColorEqualityComparer())
            .ToList();

        // Create a mapping from original colors to new tints
        Dictionary<Color, Color> colorMapping = new Dictionary<Color, Color>(new ColorEqualityComparer());
        for (int i = 0; i < uniqueColors.Count; i++)
        {
            var originalColor = uniqueColors[i];
            Color newTint;
            if (i < newTints.Count)
            {
                newTint = newTints[i];
            }
            else
            {
                // If there are more unique colors than tints, use the first tint for remaining colors
                newTint = newTints[0];
            }
            colorMapping[originalColor] = newTint;
        }

        // Replace colors in the JSON
        foreach (var (token, color) in colorList)
        {
            // Get the mapped new color
            Color newColor = colorMapping[color];
            // Preserve the original alpha
            newColor = newColor.WithAlpha(color.A);

            if (token.Type == JTokenType.String)
            {
                // Handle "sc" or "fc" properties
                token.Replace(new JValue(ColorToHexString(newColor)));
            }
            else if (token.Type == JTokenType.Array)
            {
                // Handle "k" properties with color arrays
                token.Replace(JToken.Parse(ColorToArrayString(newColor)));
            }
        }

        return lottieJson.ToString();
    }

    public class ColorEqualityComparer : IEqualityComparer<Color>
    {
        private const float Tolerance = 0.0001f;

        public bool Equals(Color x, Color y)
        {
            return Math.Abs(x.R - y.R) < Tolerance &&
                   Math.Abs(x.G - y.G) < Tolerance &&
                   Math.Abs(x.B - y.B) < Tolerance &&
                   Math.Abs(x.A - y.A) < Tolerance;
        }

        public int GetHashCode(Color obj)
        {
            int hash = 17;
            hash = hash * 23 + obj.R.GetHashCode();
            hash = hash * 23 + obj.G.GetHashCode();
            hash = hash * 23 + obj.B.GetHashCode();
            hash = hash * 23 + obj.A.GetHashCode();
            return hash;
        }
    }

    public static string ColorToHexString(Color color)
    {
        // Format the color as a hex string with alpha
        var a = (int)(color.A * 255);
        var r = (int)(color.R * 255);
        var g = (int)(color.G * 255);
        var b = (int)(color.B * 255);
        return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
    }
    public static string ColorToArrayString(Color color)
    {
        // Format the color as an array string
        var r = color.R.ToString(CultureInfo.InvariantCulture);
        var g = color.G.ToString(CultureInfo.InvariantCulture);
        var b = color.B.ToString(CultureInfo.InvariantCulture);
        var a = color.A.ToString(CultureInfo.InvariantCulture);
        return $"[\n  {r},\n  {g},\n  {b},\n  {a}\n]";
    }

    public static void FindColorProperties(JToken token, List<(JToken Token, Color Color)> colorList)
    {
        if (token is JObject obj)
        {
            foreach (var property in obj.Properties())
            {
                if (property.Name == "k")
                {
                    if (IsColorArray(property.Value))
                    {
                        var color = StringToXamarinColor(property.Value.ToString());
                        colorList.Add((property.Value, color));
                    }
                }
                else if (property.Name == "sc" || property.Name == "fc")
                {
                    var color = StringToXamarinColorFromHex(property.Value.ToString());
                    colorList.Add((property.Value, color));
                }

                // Recursively process the property's value
                FindColorProperties(property.Value, colorList);
            }
        }
        else if (token is JArray arr)
        {
            foreach (var item in arr)
            {
                // Recursively process each item in the array
                FindColorProperties(item, colorList);
            }
        }
    }

    public static Color StringToXamarinColorFromHex(string hexColor)
    {
        // Remove the '#' if present
        if (hexColor.StartsWith("#"))
            hexColor = hexColor.Substring(1);

        // Parse RGB values
        if (hexColor.Length == 6)
        {
            var r = int.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber) / 255.0;
            var g = int.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber) / 255.0;
            var b = int.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber) / 255.0;
            return new Color((float)r, (float)g, (float)b);
        }
        else if (hexColor.Length == 8)
        {
            var a = int.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber) / 255.0;
            var r = int.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber) / 255.0;
            var g = int.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber) / 255.0;
            var b = int.Parse(hexColor.Substring(6, 2), NumberStyles.HexNumber) / 255.0;
            return new Color((float)r, (float)g, (float)b, (float)a);
        }
        else
        {
            throw new FormatException("Invalid color format");
        }
    }


    public static Color StringToXamarinColor(string colorString)
    {
        // Remove whitespace and square brackets from the string
        colorString = colorString.Replace(" ", "").Replace("[", "").Replace("]", "");

        // Split the string into individual color components
        var components = colorString.Split(',');

        // Parse the color components as floats
        var r = float.Parse(components[0], CultureInfo.InvariantCulture);
        var g = float.Parse(components[1], CultureInfo.InvariantCulture);
        var b = float.Parse(components[2], CultureInfo.InvariantCulture);
        var a = float.Parse(components[3], CultureInfo.InvariantCulture);

        // Create and return a Xamarin.Forms Color object
        return new Color(r, g, b, a);
    }

    public static string ColorToString(Color color)
    {
        // Extract the color components as floats
        var r = color.R;
        var g = color.G;
        var b = color.B;
        var a = color.A;

        // Format the string color representation
        var colorString =
            $"[\n  {r.ToString(CultureInfo.InvariantCulture)},\n  {g.ToString(CultureInfo.InvariantCulture)},\n  {b.ToString(CultureInfo.InvariantCulture)},\n  {a.ToString(CultureInfo.InvariantCulture)}\n]";

        return colorString;
    }


    public static float CalculateSaturation(Color color)
    {
        var max = Math.Max(Math.Max(color.R, color.G), color.B);
        var min = Math.Min(Math.Min(color.R, color.G), color.B);
        var delta = max - min;
        var sum = max + min;

        if (Math.Abs(delta) < 0.001) return 0;

        var saturation = sum <= 1 ? delta / sum : delta / (2 - sum);
        return (float)saturation;
    }

    private static bool IsColorArray(JToken token)
    {
        if (token is JArray arr && arr.Count == 4)
        {
            foreach (var item in arr)
                if (item.Type != JTokenType.Float && item.Type != JTokenType.Integer)
                    return false;
            return true;
        }

        return false;
    }

    private static bool IsStrokeWidthProperty(JObject parent, JProperty property)
    {
        if (parent.TryGetValue("ty", out var type) && type.Value<string>() == "st") return true;
        return false;
    }

    #endregion
}

///////////////-----------


