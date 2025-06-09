global using DrawnUi.Draw;
global using SkiaSharp;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AppoMobi.Specials;
using DrawnUi.Views;
using Microsoft.Maui.Controls;
using static Microsoft.Maui.ApplicationModel.Permissions;
using Color = Microsoft.Maui.Graphics.Color;

namespace DrawnUi.Camera;


public struct CameraExposureBaseline
{
    public float ISO { get; set; }
    public float ShutterSpeed { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public CameraExposureBaseline(float iso, float shutterSpeed, string name, string description)
    {
        ISO = iso;
        ShutterSpeed = shutterSpeed;
        Name = name;
        Description = description;
    }
}

public struct CameraManualExposureRange
{
    public float MinISO { get; set; }
    public float MaxISO { get; set; }
    public float MinShutterSpeed { get; set; }
    public float MaxShutterSpeed { get; set; }
    public bool IsManualExposureSupported { get; set; }
    public CameraExposureBaseline[] RecommendedBaselines { get; set; }

    public CameraManualExposureRange(float minISO, float maxISO, float minShutter, float maxShutter, bool isSupported, CameraExposureBaseline[] baselines)
    {
        MinISO = minISO;
        MaxISO = maxISO;
        MinShutterSpeed = minShutter;
        MaxShutterSpeed = maxShutter;
        IsManualExposureSupported = isSupported;
        RecommendedBaselines = baselines ?? new CameraExposureBaseline[0];
    }
}

public partial class SkiaCamera : SkiaControl
{

    public override bool CanUseCacheDoubleBuffering => false;

#if (!ANDROID && !IOS && !MACCATALYST && !WINDOWS && !TIZEN)

    public virtual void SetZoom(double value)
    {
        throw new NotImplementedException();
    }

#endif


    #region HELPERS

    /// <summary>
    /// Analyzes pixel luminance in a specific area of the frame (shared across all platforms)
    /// </summary>
    /// <param name="frame">The camera frame to analyze</param>
    /// <param name="meteringMode">Spot (10x10 points) or CenterWeighted (50x50 points)</param>
    /// <param name="renderingScale">Rendering scale to convert points to pixels</param>
    /// <returns>Average luminance value (0-255 scale)</returns>
    public double AnalyzeFrameLuminance(SKImage frame, MeteringMode meteringMode)
    {
        if (frame == null)
            throw new ArgumentNullException(nameof(frame));

        float renderingScale = this.RenderingScale;
        var width = frame.Width;
        var height = frame.Height;

        // Define sampling area based on metering mode - in points, then convert to pixels
        int sampleSizePoints = meteringMode == MeteringMode.Spot ? 10 : 50;
        int sampleSizePixels = (int)(sampleSizePoints * renderingScale);
        
        int centerX = width / 2;
        int centerY = height / 2;
        
        int startX = Math.Max(0, centerX - sampleSizePixels / 2);
        int startY = Math.Max(0, centerY - sampleSizePixels / 2);
        int endX = Math.Min(width, centerX + sampleSizePixels / 2);
        int endY = Math.Min(height, centerY + sampleSizePixels / 2);

        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Analyzing frame: {width}x{height}");
        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Sampling: {sampleSizePoints}x{sampleSizePoints} pts * {renderingScale:F1} = {sampleSizePixels}x{sampleSizePixels} px");
        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Sampling area: ({startX},{startY}) to ({endX},{endY})");

        // Sample pixels from the target area
        using var bitmap = SKBitmap.FromImage(frame);
        
        double totalLuminance = 0;
        int pixelCount = 0;
        
        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                
                // Calculate luminance using standard formula: 0.299*R + 0.587*G + 0.114*B
                var luminance = (0.299 * pixel.Red + 0.587 * pixel.Green + 0.114 * pixel.Blue);
                totalLuminance += luminance;
                pixelCount++;
            }
        }

        if (pixelCount == 0)
            throw new InvalidOperationException("No pixels to analyze in the specified area");

        var averageLuminance = totalLuminance / pixelCount;
        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Average luminance: {averageLuminance:F1} (0-255 scale), pixels: {pixelCount}");
        
        return averageLuminance;
    }

    /// <summary>
    /// Converts normalized luminance to estimated lux value (shared across all platforms)
    /// </summary>
    /// <param name="pixelLuminance">Raw pixel luminance (0-255)</param>
    /// <param name="exposureDuration">Camera exposure duration in seconds</param>
    /// <param name="iso">Camera ISO value</param>
    /// <param name="aperture">Camera aperture (f-number)</param>
    /// <returns>Estimated brightness in lux</returns>
    public static double CalculateBrightnessFromExposure(double pixelLuminance, double exposureDuration, float iso, float aperture)
    {
        // Normalize pixel luminance to account for camera exposure settings
        // Formula: Actual_Luminance = Pixel_Luminance * (ISO/100) * (1/exposure_duration) / (aperture^2)
        var normalizedLuminance = pixelLuminance * (iso / 100.0) * (1.0 / exposureDuration) / (aperture * aperture);
        
        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Exposure compensation: Duration={exposureDuration:F6}s, ISO={iso:F0}, Aperture=f/{aperture:F1}");
        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Raw luminance: {pixelLuminance:F1} → Normalized: {normalizedLuminance:F1}");
        
        // Convert normalized luminance to lux using calibrated scale
        double estimatedLux;
        
        if (normalizedLuminance < 1)
        {
            // Very dark: 0.1 - 1 lux (moonlight, deep shadow)
            estimatedLux = 0.1 + normalizedLuminance * 0.9;
        }
        else if (normalizedLuminance < 10)
        {
            // Dark: 1 - 10 lux (candlelight, dim room)
            estimatedLux = 1 + (normalizedLuminance - 1) * 1.0;
        }
        else if (normalizedLuminance < 100)
        {
            // Medium: 10 - 100 lux (living room, restaurant)
            estimatedLux = 10 + (normalizedLuminance - 10) * 1.0;
        }
        else if (normalizedLuminance < 1000)
        {
            // Bright: 100 - 1000 lux (office, bright room)
            estimatedLux = 100 + (normalizedLuminance - 100) * 1.0;
        }
        else if (normalizedLuminance < 10000)
        {
            // Very bright: 1000 - 10000 lux (daylight, bright office)
            estimatedLux = 1000 + (normalizedLuminance - 1000) * 1.0;
        }
        else
        {
            // Extremely bright: 10000+ lux (direct sunlight)
            estimatedLux = 10000 + (normalizedLuminance - 10000) * 10.0;
        }

        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Final estimated brightness: {estimatedLux:F0} lux");
        return estimatedLux;
    }

    /// <summary>
    /// Direct pixel-to-lux mapping for platforms where camera exposure settings are unreliable
    /// </summary>
    /// <param name="pixelLuminance">Raw pixel luminance (0-255)</param>
    /// <returns>Estimated brightness in lux</returns>
    public static double CalculateBrightnessFromPixelsOnly(double pixelLuminance)
    {
        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Direct pixel mapping: {pixelLuminance:F1} (0-255 scale)");
        
        // Direct pixel luminance to lux mapping with wide dynamic range
        double estimatedLux;
        
        if (pixelLuminance < 5)
        {
            // Very dark: 0.1 - 5 lux (deep shadow, moonlight)
            estimatedLux = 0.1 + (pixelLuminance / 5.0) * 4.9;
        }
        else if (pixelLuminance < 25)
        {
            // Dark: 5 - 50 lux (dim room, candlelight)
            estimatedLux = 5 + ((pixelLuminance - 5) / 20.0) * 45;
        }
        else if (pixelLuminance < 75)
        {
            // Medium: 50 - 500 lux (living room, restaurant)
            estimatedLux = 50 + ((pixelLuminance - 25) / 50.0) * 450;
        }
        else if (pixelLuminance < 150)
        {
            // Bright: 500 - 2000 lux (office, bright room)
            estimatedLux = 500 + ((pixelLuminance - 75) / 75.0) * 1500;
        }
        else if (pixelLuminance < 200)
        {
            // Very bright: 2000 - 10000 lux (daylight indoors)
            estimatedLux = 2000 + ((pixelLuminance - 150) / 50.0) * 8000;
        }
        else
        {
            // Extremely bright: 10000+ lux (direct sunlight)
            estimatedLux = 10000 + ((pixelLuminance - 200) / 55.0) * 90000;
        }

        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Direct pixel brightness: {estimatedLux:F0} lux");
        return estimatedLux;
    }

#if ONPLATFORM

    /// <summary>
    /// Measures scene brightness using pixel luminance analysis (moved from native implementations to eliminate redundancy)
    /// </summary>
    /// <param name="meteringMode">Spot (10x10 points) or CenterWeighted (50x50 points)</param>
    /// <returns>Brightness measurement result</returns>
    public async Task<BrightnessResult> MeasureSceneBrightness(MeteringMode meteringMode)
    {
        try
        {
            if (NativeControl == null)
                return new BrightnessResult { Success = false, ErrorMessage = "Camera not initialized" };

            System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Starting brightness measurement with {meteringMode} mode");

            // A - Get preview to check what camera has chosen (auto exposure)
            CapturedImage autoFrame = null;
            for (int attempts = 0; attempts < 10; attempts++)
            {
                autoFrame = NativeControl.GetPreviewImage();
                if (autoFrame?.Image != null)
                    break;
                await Task.Delay(100);
            }

            if (autoFrame?.Image == null)
                return new BrightnessResult { Success = false, ErrorMessage = "Could not capture frame for analysis" };

            // Check auto exposure metadata to understand lighting conditions
            var autoISO = CameraDevice.Meta.ISO;
            var autoShutter = CameraDevice.Meta.Shutter;
            var autoAperture = CameraDevice.Meta.Aperture;

            System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Auto exposure detected: ISO {autoISO}, Shutter {autoShutter}s, Aperture f/{autoAperture}");

            autoFrame.Dispose(); // We only needed this for metadata

            // Get possible exposure ranges
            var exposureRange = NativeControl.GetExposureRange();
            double brightness = 0.0;
            double pixelLuminance;

            CapturedImage frame = null;
            bool manualExposureWasSet = false;

            if (exposureRange.IsManualExposureSupported)
            {
                System.Diagnostics.Debug.WriteLine("[SHARED CAMERA] Manual exposure supported - using scene-adaptive approach");

                // B - Set manual values according to probable light conditions we just read
                float manualISO, manualShutter;

                if (autoISO < 200 && autoShutter > 1/100f)
                {
                    // Bright conditions - use bright baseline
                    manualISO = 50f;
                    manualShutter = 1/250f;
                    System.Diagnostics.Debug.WriteLine("[SHARED CAMERA] Detected bright conditions - using bright baseline");
                }
                else if (autoISO > 800 || autoShutter < 1/100f)
                {
                    // Dark conditions - use dark baseline
                    manualISO = 400f;
                    manualShutter = 1/30f;
                    System.Diagnostics.Debug.WriteLine("[SHARED CAMERA] Detected dark conditions - using dark baseline");
                }
                else
                {
                    // Mid/indoor conditions - use indoor baseline
                    manualISO = 100f;
                    manualShutter = 1/60f;
                    System.Diagnostics.Debug.WriteLine("[SHARED CAMERA] Detected mid/indoor conditions - using indoor baseline");
                }

                System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Setting manual exposure: ISO {manualISO}, Shutter {manualShutter}s");

                bool manualExposureSet = NativeControl.SetManualExposure(manualISO, manualShutter);
                if (manualExposureSet)
                {
                    manualExposureWasSet = true;
                    // Wait for camera to apply settings
                    await Task.Delay(1500);
                }
            }

            // C - Get frame for analysis (either with manual settings applied or auto)
            for (int attempts = 0; attempts < 10; attempts++)
            {
                frame = NativeControl.GetPreviewImage();
                if (frame?.Image != null)
                    break;
                await Task.Delay(100);
            }

            // Restore auto exposure AFTER getting the frame
            if (manualExposureWasSet)
            {
                NativeControl.SetAutoExposure();
            }

            if (frame?.Image == null)
                return new BrightnessResult { Success = false, ErrorMessage = "Could not capture frame for analysis" };

            using (frame)
            {
                pixelLuminance = AnalyzeFrameLuminance(frame.Image, meteringMode);

                if (exposureRange.IsManualExposureSupported && manualExposureWasSet)
                {
                    // Check if manual settings were actually applied
                    var actualISO = CameraDevice.Meta.ISO;
                    var actualShutter = CameraDevice.Meta.Shutter;
                    var baseline = exposureRange.RecommendedBaselines[0];

                    bool isoMatches = Math.Abs(actualISO - baseline.ISO) < 10;
                    bool shutterMatches = Math.Abs(actualShutter - baseline.ShutterSpeed) < 0.005;

                    if (isoMatches && shutterMatches)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Manual settings verified: ISO {actualISO}, Shutter {actualShutter}s");

                        // D - Use manual exposure calculation with verified settings
                        brightness = CalculateSceneBrightnessFromPixels(
                            pixelLuminance,
                            actualISO,
                            CameraDevice.Meta.Aperture,
                            actualShutter
                        );

                        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Manual exposure brightness: {brightness:F0} lux");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Manual settings not applied - Expected ISO {baseline.ISO}, got {actualISO}; Expected shutter {baseline.ShutterSpeed}, got {actualShutter}");
                        brightness = CalculateBrightnessFromPixelsOnly(pixelLuminance);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[SHARED CAMERA] Manual exposure not supported - using direct pixel approach");

                    // Use direct pixel-to-lux mapping (all platforms now use this approach)
                    // This approach directly correlates pixel brightness to real-world lux values
                    brightness = CalculateBrightnessFromPixelsOnly(pixelLuminance);

                    // Alternative: Camera exposure-based calculation (less reliable for actual scene brightness)
                    //var brightness = CalculateSceneBrightnessFromPixels(pixelLuminance, CameraDevice.Meta.ISO, CameraDevice.Meta.Aperture, CameraDevice.Meta.Shutter);
                }

                System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Brightness measurement complete: {brightness:F0} lux");

                if (brightness > 0)
                {
                    return new BrightnessResult
                    {
                        Success = true,
                        Brightness = brightness
                    };
                }
                else
                {
                    return new BrightnessResult
                    {
                        Success = false,
                    };
                }

            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA ERROR] MeasureSceneBrightness failed: {ex.Message}");
            return new BrightnessResult { Success = false, ErrorMessage = ex.Message };
        }
    }

#endif

    /// <summary>
    /// Calculates actual scene brightness using the camera's current settings and pixel analysis
    /// </summary>
    /// <param name="pixelLuminance">Average pixel luminance from AnalyzeFrameLuminance (0-255 scale)</param>
    /// <param name="iso">Current camera ISO</param>
    /// <param name="aperture">Current camera aperture (f-stop)</param>
    /// <param name="shutter">Current camera shutter speed (seconds)</param>
    /// <returns>Estimated scene brightness in lux</returns>
    public static double CalculateSceneBrightnessFromPixels(
        double pixelLuminance,
        double iso,
        double aperture,
        double shutter)
    {
        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] CalculateSceneBrightnessFromPixels: pixels {pixelLuminance:F0}, iso {iso:0}, aperture {aperture:0.00}, shutter {shutter:0.0000}");

        // Camera exposure settings tell us what the camera thinks is "proper exposure"
        // This represents the brightness level the camera is targeting (middle gray = 18% reflectance)

        // Calculate the EV that the camera is using for "proper exposure"
        double cameraEV = Math.Log2((aperture * aperture) / shutter) + Math.Log2(iso / 100.0);

        // Convert camera EV to the luminance it's targeting (what it thinks middle gray should be)
        const double K = 12.5; // Standard photometric constant
        double cameraTargetLuminance = K * Math.Pow(2, cameraEV);

        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Camera EV: {cameraEV:F1}, Target luminance: {cameraTargetLuminance:F0}");

        // Now use actual pixel values to determine how bright the scene really is
        // Middle gray (18% reflectance) should appear as ~128 on 0-255 scale
        // If pixels are darker/brighter than 128, the scene is darker/brighter than camera's target
        double actualBrightnessRatio = pixelLuminance / 128.0;

        // Apply gamma correction - camera sensors apply ~2.2 gamma curve
        // This converts from camera's gamma-corrected values back to linear light
        actualBrightnessRatio = Math.Pow(Math.Max(0.001, actualBrightnessRatio), 2.2);

        // Calculate final scene brightness
        // If ratio = 1.0: scene matches camera's expectation
        // If ratio < 1.0: scene is darker than camera expected
        // If ratio > 1.0: scene is brighter than camera expected
        double finalSceneBrightness = cameraTargetLuminance * actualBrightnessRatio;

        System.Diagnostics.Debug.WriteLine($"[SHARED CAMERA] Brightness ratio: {actualBrightnessRatio:F3}, Final: {finalSceneBrightness:F0} lux");

        return finalSceneBrightness;
    }

    /// <summary>
    /// Going to overlay any SkiaLayout over the captured photo and return a new bitmap.
    /// So do not forget to dispose the old one if not needed anymore.
    /// </summary>
    /// <param name="captured"></param>
    /// <param name="overlay"></param>
    /// <returns></returns>
    public virtual SKImage RenderCapturedPhoto(CapturedImage captured, SkiaLayout overlay)
    {
        var scaleOverlay = GetRenderingScaleFor(captured.Image.Width, captured.Image.Height);
        double zoomCapturedPhotoX = TextureScale;
        double zoomCapturedPhotoY = TextureScale;

        var width = captured.Image.Width;
        var height = captured.Image.Height;

        if (captured.Orientation == 90 || captured.Orientation == 270)
        {
            height = captured.Image.Width;
            width = captured.Image.Height;
        }

        var info = new SKImageInfo(width, height);

        using (var surface = SKSurface.Create(info))
        {
            if (surface == null)
            {
                //Trace.WriteLine($"Cannot create SKSurface {width}x{height}");
                return null;
            }

            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.Black);

            //create offscreen rendering context
            var context = new SkiaDrawingContext()
            {
                Canvas = surface.Canvas,
                Width = info.Width,
                Height = info.Height
            };
            var destination = new SKRect(0, 0, info.Width, info.Height);

            //render image
            var image = new SkiaImage
            {
                Tag = "Render",
                LoadSourceOnFirstDraw = true,
                WidthRequest = info.Width,
                HeightRequest = info.Height,
                VerticalOptions = LayoutOptions.Fill,
                IsClippedToBounds = false, //do not clip sides after rotation if any
                AddEffect = this.Effect,
                Aspect = TransformAspect.None,
                ZoomX = zoomCapturedPhotoX,
                ZoomY = zoomCapturedPhotoY,
                ImageBitmap = new LoadedImageSource(captured.Image) //must not dispose bitmap after that, it's used by preview outside
            };

            if (captured.Orientation != 0)
            {
                var transfromRotation = (float)captured.Orientation;
                if (captured.Facing == CameraPosition.Selfie)
                {
                    transfromRotation = (float)((360 - captured.Orientation) % 360);
                }
                image.Rotation = transfromRotation;
            }

            var ctx = new DrawingContext(context, destination, 1, null);
            image.Render(ctx);
            overlay.Render(ctx.WithScale(scaleOverlay));

            surface.Canvas.Flush();
            return surface.Snapshot();
        }

    }
#endregion

    #region EVENTS

    public event EventHandler<CapturedImage> CaptureSuccess;

    public event EventHandler<Exception> CaptureFailed;

    public event EventHandler<LoadedImageSource> NewPreviewSet;

    public event EventHandler<string> OnError;

    public event EventHandler<double> Zoomed;

    internal void RaiseError(string error)
    {
        OnError?.Invoke(this, error);
    }

    #endregion

    #region METHODS

    /// <summary>
    /// Stops the camera
    /// </summary>
    public void Stop(bool force=false)
    {
        if (IsDisposing || IsDisposed)
            return;

        System.Diagnostics.Debug.WriteLine($"[CAMERA] Stopped {Uid} {Tag}");

        NativeControl?.Stop(force);
        State = CameraState.Off;
        //DisplayImage.IsVisible = false;
    }

   /// <summary>
    /// Starts the camera
    /// </summary>
    public void Start()
    {
        if (IsDisposing || IsDisposed)
            return;

        if (NativeControl == null)
        {
#if ANDROID || IOS || WINDOWS
            CreateNative();
#endif
        }

        //var rotation = ((Superview.DeviceRotation + 45) / 90) % 4;
        //NativeControl?.ApplyDeviceOrientation(rotation);

        if (Display != null)
        {
            //DestroyRenderingObject();
            Display.IsVisible = true;
        }

        //IsOn = true;

        NativeControl?.Start();
    }

    /// <summary>
    /// Play shutter sound
    /// </summary>
    public virtual void PlaySound()
    {
        //todo
    }

    private static int filenamesCounter = 0;

    /// <summary>
    /// Override this to set your own DisplayInfo
    /// </summary>
    public virtual void UpdateInfo()
    {
        var info = $"Position: {Facing}" +
                   $"\nState: {State}" +
                   //$"\nSize: {Width}x{Height} pts" +
                   $"\nPreview: {PreviewSize} px" +
                   $"\nPhoto: {CapturePhotoSize} px" +
                   $"\nRotation: {this.DeviceRotation}";

        if (Display != null)
        {
            info += $"\nAspect: {Display.Aspect}";
        }

        DisplayInfo = info;
    }

    /// <summary>
    /// Apply effects on preview
    /// </summary>
    public virtual void ApplyPreviewProperties()
    {
        if (Display != null)
        {
            Display.AddEffect = Effect;
        }
    }

    /// <summary>
    /// Generate Jpg filename
    /// </summary>
    /// <returns></returns>
    public virtual string GenerateJpgFileName()
    {
        var add = $"{DateTime.Now:MM/dd/yyyy HH:mm:ss}{++filenamesCounter}";
        var filename = $"skiacamera-{add.Replace("/", "").Replace(":", "").Replace(" ", "").Replace(",", "").Replace(".", "").Replace("-", "")}.jpg";

        return filename;
    }

    /// <summary>
    /// Save captured bitmap to native gallery
    /// </summary>
    /// <param name="captured"></param>
    /// <param name="reorient"></param>
    /// <param name="album"></param>
    /// <returns></returns>
    public async Task<string> SaveToGallery(CapturedImage captured, bool reorient, string album = null)
    {
        var filename = GenerateJpgFileName();

        await using var stream = CreateOutputStream(captured, reorient);

        if (stream != null)
        {
            var filenameOutput = GenerateJpgFileName();

            var path = await NativeControl.SaveJpgStreamToGallery(stream, filename, captured.Orientation, album);

            stream.Dispose();

            if (!string.IsNullOrEmpty(path))
            {
                captured.Path = path;
                Debug.WriteLine($"[SkiaCamera] saved photo: {filenameOutput}");
                return path;
            }
        }

        Debug.WriteLine($"[SkiaCamera] failed to save photo");
        return null;
    }

    public Stream CreateOutputStream(CapturedImage captured,
        bool reorient,
        SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg,
        int quality = 90)
    {
        try
        {
            var rotated = Reorient();
            var data = rotated.Encode(format, quality);
            return data.AsStream();

            SKBitmap Reorient()
            {

                var bitmap = SKBitmap.FromImage(captured.Image);

                if (!reorient)
                    return bitmap;

                SKBitmap rotated;

                switch (captured.Orientation)
                {
                    case 180:
                        using (var surface = new SKCanvas(bitmap))
                        {
                            surface.RotateDegrees(180, bitmap.Width / 2.0f, bitmap.Height / 2.0f);
                            surface.DrawBitmap(bitmap.Copy(), 0, 0);
                        }
                        return bitmap;
                    case 270:
                        rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                        using (var surface = new SKCanvas(rotated))
                        {
                            surface.Translate(rotated.Width, 0);
                            surface.RotateDegrees(90);
                            surface.DrawBitmap(bitmap, 0, 0);
                        }
                        return rotated;
                    case 90:
                        rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                        using (var surface = new SKCanvas(rotated))
                        {
                            surface.Translate(0, rotated.Height);
                            surface.RotateDegrees(270);
                            surface.DrawBitmap(bitmap, 0, 0);
                        }
                        return rotated;
                    default:
                        return bitmap;
                }
            }
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
            return null;
        }
    }


    /// <summary>
    /// Take camera picture. Run this in background thread!
    /// </summary>
    /// <returns></returns>
    public async Task TakePicture()
    {
        if (IsBusy)
            return;

        Debug.WriteLine($"[TakePicture] IsMainThread {MainThread.IsMainThread}");

        IsBusy = true;

        IsTakingPhoto = true;

        NativeControl.StillImageCaptureFailed = ex =>
        {
            OnCaptureFailed(ex);

            IsTakingPhoto = false;
        };

        NativeControl.StillImageCaptureSuccess = captured =>
        {
            CapturedStillImage = captured;

            OnCaptureSuccess(captured);

            IsTakingPhoto = false;
        };

        NativeControl.TakePicture();

        while (IsTakingPhoto)
        {
            await Task.Delay(150);
        }

        IsBusy = false;
    }

    /// <summary>
    /// Flash screen with color
    /// </summary>
    /// <param name="color"></param>
    public virtual void FlashScreen(Color color)
    {
        var layer = new SkiaControl()
        {
            Tag = "Flash",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = color,
            ZIndex = int.MaxValue,
        };

        layer.SetParent(this);

        layer.FadeToAsync(0).ContinueWith(_ =>
        {
            layer.Parent = null;
        });
    }

    #endregion

    #region ENGINE

    protected virtual void OnCaptureSuccess(CapturedImage captured)
    {
        CaptureSuccess?.Invoke(this, captured);
    }

    protected virtual void OnCaptureFailed(Exception ex)
    {
        CaptureFailed?.Invoke(this, ex);
    }

    protected virtual SkiaImage CreatePreview()
    {
        return new SkiaImage()
        {
            //Parent = this,
            LoadSourceOnFirstDraw = true,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            //BackgroundColor = Colors.Red, 
            Aspect = TransformAspect.AspectCover,
        };
    }

    public override bool WillClipBounds => true;


    public SkiaCamera()
    {
        Instances.Add(this);
        Super.OnNativeAppResumed += Super_OnNativeAppResumed;
        Super.OnNativeAppPaused += Super_OnNativeAppPaused;
    }

    public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
    {
        if (IsDisposed || IsDisposing)
            return ScaledSize.Default;

        if (Display == null)
        {
            //will serve as preview wrapper
            Display = CreatePreview();
            Display.SetParent(this);
        }

        return base.Measure(widthConstraint, heightConstraint, scale);
    }

    public SkiaImage Display { get; protected set; }

 
    public INativeCamera NativeControl;


    protected override void OnLayoutReady()
    {
        base.OnLayoutReady();

        if (State == CameraState.Error)
            Start();
    }

    bool subscribed;

    public override void SuperViewChanged()
    {
        if (Superview != null && !subscribed)
        {
            subscribed = true;
            Superview.DeviceRotationChanged += DeviceRotationChanged;
        }

        base.SuperViewChanged();
    }


    private void DeviceRotationChanged(object sender, int orientation)
    {
        var rotation = ((orientation + 45) / 90) * 90 % 360;

        DeviceRotation = rotation;
    }

    private int _DeviceRotation;
    public int DeviceRotation
    {
        get
        {
            return _DeviceRotation;
        }
        set
        {
            if (_DeviceRotation != value)
            {
                _DeviceRotation = value;
                OnPropertyChanged();
                NativeControl?.ApplyDeviceOrientation(value);
                UpdateInfo();
            }
        }
    }

    object lockFrame = new();

    public bool FrameAquired { get; set; }

    public virtual void UpdatePreview()
    {
        FrameAquired = false;
        Update();
    }

    public SKSurface FrameSurface { get; protected set; }
    public SKImageInfo FrameSurfaceInfo { get; protected set; }

    public bool AllocatedFrameSurface(int width, int height)
    {
        if (Superview == null || width == 0 || height == 0)
        {
            return false;
        }

        var kill = FrameSurface;
        FrameSurfaceInfo = new SKImageInfo(width, height);
        if (Superview.CanvasView is SkiaViewAccelerated accelerated)
        {
            FrameSurface = SKSurface.Create(accelerated.GRContext, true, FrameSurfaceInfo);
        }
        else
        {
            //normal one
            FrameSurface = SKSurface.Create(FrameSurfaceInfo);
        }
        kill?.Dispose();

        return true;
    }


    protected virtual void OnNewFrameSet(LoadedImageSource source)
    {
        NewPreviewSet?.Invoke(this, source);
    }

    protected override void Paint(DrawingContext ctx)
    {
        base.Paint(ctx);

        if (NativeControl != null && State == CameraState.On && !FrameAquired)
        {
            //aquire latest image from camera
            var image = NativeControl.GetPreviewImage();
            if (image != null)
            {
                FrameAquired = true;
                OnNewFrameSet(Display.SetImageInternal(image.Image));
            }
        }

        //draw DisplayImage
        DrawViews(ctx);
    }


    #endregion

#if (!ANDROID && !IOS && !MACCATALYST && !WINDOWS && !TIZEN)


    public SKBitmap GetPreviewBitmap()
    {
        throw new NotImplementedException();
    }


#endif

    public ICommand CommandCheckPermissions
    {
        get
        {
            return new Command((object context) =>
            {
                Startup();
            });
        }
    }

    bool lockStartup;


    /// <summary>
    /// Request permissions and start camera
    /// </summary>
    public void Startup()
    {
        if (lockStartup)
        {
            Debug.WriteLine("[SkiaCamera] Startup locked.");
            return;
        }

        lockStartup = true;

        try
        {
            Debug.WriteLine("[SkiaCamera] Requesting permissions...");

            SkiaCamera.CheckPermissions((presented) =>
                {
                    Debug.WriteLine("[SkiaCamera] Starting..");
                    PermissionsWarning = false;
                    Start();

                    //if (Geotag)
                    //	CommandGetLocation.Execute(null);
                    //else
                    //{
                    //	CanDetectLocation = false;
                    //}
                },
                (presented) =>
                {
                    Trace.WriteLine("[SkiaCamera] Permissions denied");
                    IsOn = false;
                    PermissionsWarning = true;
                });

        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
        }
        finally
        {
            Tasks.StartDelayed(TimeSpan.FromSeconds(1), () =>
            {
                Debug.WriteLine("[SkiaCamera] Startup UNlocked.");
                lockStartup = false;
            });
        }
    }



    #region SkiaCamera xam control




    private bool _PermissionsWarning;
    public bool PermissionsWarning
    {
        get
        {
            return _PermissionsWarning;
        }
        set
        {
            if (_PermissionsWarning != value)
            {
                _PermissionsWarning = value;
                OnPropertyChanged();
            }
        }
    }




    public class CameraQueuedPictured
    {
        public string Filename { get; set; }

        public double SensorRotation { get; set; }

        /// <summary>
        /// Set by renderer after work
        /// </summary>
        public bool Processed { get; set; }

    }

    public class CameraPicturesQueue : Queue<CameraQueuedPictured>
    {


    }

    private bool _IsBusy;
    public bool IsBusy
    {
        get { return _IsBusy; }
        set
        {
            if (_IsBusy != value)
            {
                _IsBusy = value;
                OnPropertyChanged();
            }
        }
    }


    private bool _IsTakingPhoto;
    public bool IsTakingPhoto
    {
        get { return _IsTakingPhoto; }
        set
        {
            if (_IsTakingPhoto != value)
            {
                _IsTakingPhoto = value;
                OnPropertyChanged();
            }
        }
    }


    public CameraPicturesQueue PicturesQueue { get; } = new CameraPicturesQueue();



    #region PERMISSIONS

    protected static bool ChecksBusy = false;

    private static DateTime lastTimeChecked = DateTime.MinValue;

    public static bool PermissionsGranted { get; protected set; }


    public static void CheckGalleryPermissions(Action granted, Action notGranted)
    {
        if (lastTimeChecked + TimeSpan.FromSeconds(5) < DateTime.Now) //avoid spam
        {
            lastTimeChecked = DateTime.Now;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (ChecksBusy)
                    return;

                bool okay1 = false;


                ChecksBusy = true;
                // Update the UI
                try
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.Camera>();


                        if (status == PermissionStatus.Granted)
                        {
                            okay1 = true;
                        }
                    }
                    else
                    {
                        okay1 = true;
                    }



                    // Additionally could prompt the user to turn on in settings
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
                finally
                {
                    if (okay1)
                    {
                        PermissionsGranted = true;
                        granted?.Invoke();
                    }
                    else
                    {
                        PermissionsGranted = false;
                        notGranted?.Invoke();
                    }
                    ChecksBusy = false;
                }
            });

        }



    }

    private bool _GpsBusy;
    public bool GpsBusy
    {
        get
        {
            return _GpsBusy;
        }
        set
        {
            if (_GpsBusy != value)
            {
                _GpsBusy = value;
                OnPropertyChanged();
            }
        }
    }

    private double _LocationLat;
    public double LocationLat
    {
        get
        {
            return _LocationLat;
        }
        set
        {
            if (_LocationLat != value)
            {
                _LocationLat = value;
                OnPropertyChanged();
            }
        }
    }

    private double _LocationLon;
    public double LocationLon
    {
        get
        {
            return _LocationLon;
        }
        set
        {
            if (_LocationLon != value)
            {
                _LocationLon = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _CanDetectLocation;
    public bool CanDetectLocation
    {
        get
        {
            return _CanDetectLocation;
        }
        set
        {
            if (_CanDetectLocation != value)
            {
                _CanDetectLocation = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Safe and if CanDetectLocation
    /// </summary>
    /// <returns></returns>
    public async Task RefreshLocation(int msTimeout)
    {
        if (CanDetectLocation)
        {
            //my ACTUAL location
            try
            {
                GpsBusy = true;

                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var cancel = new CancellationTokenSource();
                cancel.CancelAfter(msTimeout);
                var location = await Geolocation.GetLocationAsync(request, cancel.Token);

                if (location != null)
                {
                    Debug.WriteLine(
                        $"ACTUAL Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");

                    this.LocationLat = location.Latitude;
                    this.LocationLon = location.Longitude;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                //Toast.ShortMessage("GPS не поддерживается на устройстве");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                //Toast.ShortMessage("GPS отключен на устройстве");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                //Toast.ShortMessage("Вы не дали разрешение на использование GPS");
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
            finally
            {
                GpsBusy = false;
            }
        }
    }

    //public ICommand CommandGetLocation
    //{
    //	get
    //	{
    //		return new Command((object context) =>
    //		{
    //			if (GpsBusy || !App.Native.CheckGpsEnabled())
    //				return;

    //			MainThread.BeginInvokeOnMainThread(async () =>
    //			{
    //				// Update the UI
    //				try
    //				{
    //					GpsBusy = true;

    //					var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
    //					if (status != PermissionStatus.Granted)
    //					{
    //						CanDetectLocation = false;

    //						await App.Current.MainPage.DisplayAlert(Core.Current.MyCompany.Name, ResStrings.X_NeedMoreForGeo, ResStrings.ButtonOk);

    //						status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    //						if (status != PermissionStatus.Granted)
    //						{
    //							// Additionally could prompt the user to turn on in settings
    //							return;
    //						}
    //						else
    //						{
    //							CanDetectLocation = true;
    //						}
    //					}
    //					else
    //					{
    //						CanDetectLocation = true;
    //					}

    //					if (CanDetectLocation)
    //					{
    //						//my LAST location:
    //						try
    //						{
    //							if (App.Native.CheckGpsEnabled())
    //							{
    //								var location = await Geolocation.GetLastKnownLocationAsync();

    //								if (location != null)
    //								{
    //									Debug.WriteLine(
    //										$"LAST Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");

    //									LocationLat = location.Latitude;
    //									LocationLon = location.Longitude;
    //								}
    //							}
    //						}
    //						catch (FeatureNotSupportedException fnsEx)
    //						{
    //							// Handle not supported on device exception
    //							//Toast.ShortMessage("GPS не поддерживается на устройстве");
    //						}
    //						catch (FeatureNotEnabledException fneEx)
    //						{
    //							// Handle not enabled on device exception
    //							//Toast.ShortMessage("GPS отключен на устройстве");
    //						}
    //						catch (PermissionException pEx)
    //						{
    //							// Handle permission exception
    //							//Toast.ShortMessage("Вы не дали разрешение на использование GPS");
    //						}
    //						catch (Exception ex)
    //						{
    //							// Unable to get location
    //						}

    //						await Task.Run(async () =>
    //						{
    //							await RefreshLocation(1200);

    //						}).ConfigureAwait(false);

    //					}
    //					else
    //					{
    //						GpsBusy = false;
    //					}


    //				}
    //				catch (Exception ex)
    //				{
    //					//Something went wrong
    //					Trace.WriteLine(ex);
    //					CanDetectLocation = false;
    //					GpsBusy = false;
    //				}
    //				finally
    //				{

    //				}

    //			});


    //		});
    //	}
    //}

    /// <summary>
    /// Will pass the fact if permissions dialog was diplayed as bool
    /// </summary>
    /// <param name="granted"></param>
    /// <param name="notGranted"></param>
    public static void CheckPermissions(Action<bool> granted, Action<bool> notGranted)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (ChecksBusy)
                return;

            bool grantedCam = false;
            bool grantedStorage = false;
            bool presented = false;

            ChecksBusy = true;
            // Update the UI
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    presented = true;

                    status = await Permissions.RequestAsync<Permissions.Camera>();


                    if (status == PermissionStatus.Granted)
                    {
                        grantedCam = true;
                    }
                }
                else
                {
                    grantedCam = true;
                }

                var needStorage = true;
                if (Device.RuntimePlatform == Device.Android && DeviceInfo.Version.Major > 9)
                {
                    needStorage = false;
                }

                if (needStorage)
                {
                    status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                    if (status != PermissionStatus.Granted)
                    {
                        presented = true;

                        status = await Permissions.RequestAsync<Permissions.StorageWrite>();

                        if (status == PermissionStatus.Granted)
                        {
                            grantedStorage = true;
                        }
                    }
                    else
                    {
                        grantedStorage = true;
                    }
                }
                else
                {
                    grantedStorage = true;
                }


                // Additionally could prompt the user to turn on in settings
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            finally
            {
                if (grantedCam && grantedStorage)
                {
                    PermissionsGranted = true;
                    granted?.Invoke(presented);
                }
                else
                {
                    PermissionsGranted = false;
                    notGranted?.Invoke(presented);
                }
                ChecksBusy = false;
            }
        });
    }

    #endregion


    /// <summary>
    /// This is filled by renderer  
    /// </summary>
    public string SavedFilename
    {
        get { return _SavedFilename; }
        set
        {
            if (_SavedFilename != value)
            {
                _SavedFilename = value;
                OnPropertyChanged("SavedFilename");
            }
        }
    }
    private string _SavedFilename;

    public static readonly BindableProperty CaptureLocationProperty = BindableProperty.Create(
        nameof(CaptureLocation),
        typeof(CaptureLocationType),
        typeof(SkiaCamera),
        CaptureLocationType.Gallery);

    public CaptureLocationType CaptureLocation
    {
        get { return (CaptureLocationType)GetValue(CaptureLocationProperty); }
        set { SetValue(CaptureLocationProperty, value); }
    }

    public static readonly BindableProperty CaptureCustomFolderProperty = BindableProperty.Create(
        nameof(CaptureCustomFolder),
        typeof(string),
        typeof(SkiaCamera),
        string.Empty);

    public string CaptureCustomFolder
    {
        get { return (string)GetValue(CaptureCustomFolderProperty); }
        set { SetValue(CaptureCustomFolderProperty, value); }
    }


    public static readonly BindableProperty FacingProperty = BindableProperty.Create(
        nameof(Facing),
        typeof(CameraPosition),
        typeof(SkiaCamera),
        CameraPosition.Default, propertyChanged: NeedRestart);

    private static void NeedRestart(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaCamera control)
        {
            if (control.State == CameraState.On)
            {
                control.Stop();
                control.Start();
            }
            else
            {
                control.Start();
            }
        }
    }

    public CameraPosition Facing
    {
        get { return (CameraPosition)GetValue(FacingProperty); }
        set { SetValue(FacingProperty, value); }
    }

    public static readonly BindableProperty CapturePhotoQualityProperty = BindableProperty.Create(
        nameof(CapturePhotoQuality),
        typeof(CaptureQuality),
        typeof(SkiaCamera),
        CaptureQuality.Max);

    /// <summary>
    /// Photo capture quality
    /// </summary>
    public CaptureQuality CapturePhotoQuality
    {
        get { return (CaptureQuality)GetValue(CapturePhotoQualityProperty); }
        set { SetValue(CapturePhotoQualityProperty, value); }
    }


    public static readonly BindableProperty TypeProperty = BindableProperty.Create(
        nameof(Type),
        typeof(CameraType),
        typeof(SkiaCamera),
        CameraType.Default);

    /// <summary>
    /// To be implemented
    /// </summary>
    public CameraType Type
    {
        get { return (CameraType)GetValue(TypeProperty); }
        set { SetValue(TypeProperty, value); }
    }



    /// <summary>
    /// Will be applied to viewport for focal length etc
    /// </summary>
    public CameraUnit CameraDevice
    {
        get
        {
            return _virtualCameraUnit;
        }
        set
        {
            if (_virtualCameraUnit != value)
            {
                if (_virtualCameraUnit != value)
                {
                    _virtualCameraUnit = value;
                    AssignFocalLengthInternal(value);
                }
            }
        }
    }
    private CameraUnit _virtualCameraUnit;

    public void AssignFocalLengthInternal(CameraUnit value)
    {
        if (value != null)
        {
            FocalLength = (float)(value.FocalLength * value.SensorCropFactor);
        }
        OnPropertyChanged(nameof(CameraDevice));
    }

    private int _PreviewWidth;
    public int PreviewWidth
    {
        get { return _PreviewWidth; }
        set
        {
            if (_PreviewWidth != value)
            {
                _PreviewWidth = value;
                OnPropertyChanged("PreviewWidth");
            }
        }
    }

    private int _PreviewHeight;
    public int PreviewHeight
    {
        get { return _PreviewHeight; }
        set
        {
            if (_PreviewHeight != value)
            {
                _PreviewHeight = value;
                OnPropertyChanged("PreviewHeight");
            }
        }
    }

    private int _CaptureWidth;
    public int CaptureWidth
    {
        get { return _CaptureWidth; }
        set
        {
            if (_CaptureWidth != value)
            {
                _CaptureWidth = value;
                OnPropertyChanged("CaptureWidth");
            }
        }
    }

    private int _CaptureHeight;
    public int CaptureHeight
    {
        get { return _CaptureHeight; }
        set
        {
            if (_CaptureHeight != value)
            {
                _CaptureHeight = value;
                OnPropertyChanged("CaptureHeight");
            }
        }
    }


    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(FocalLength) || propertyName == nameof(FocalLengthAdjustment))
        {
            FocalLengthAdjusted = FocalLength + FocalLengthAdjustment;
        }


    }

    public static double GetSensorRotation(DeviceOrientation orientation)
    {
        if (orientation == DeviceOrientation.PortraitUpsideDown)
            return 180.0;

        if (orientation == DeviceOrientation.LandscapeLeft)
            return 90.0;

        if (orientation == DeviceOrientation.LandscapeRight)
            return 270.0;

        return 0.0;
    }



    public static readonly BindableProperty CapturedStillImageProperty = BindableProperty.Create(
        nameof(CapturedStillImage),
        typeof(CapturedImage),
        typeof(SkiaCamera),
        null);

    public CapturedImage CapturedStillImage
    {
        get { return (CapturedImage)GetValue(CapturedStillImageProperty); }
        set { SetValue(CapturedStillImageProperty, value); }
    }


    public static readonly BindableProperty CustomAlbumProperty = BindableProperty.Create(nameof(CustomAlbum),
        typeof(string),
        typeof(SkiaCamera),
        string.Empty);
    /// <summary>
    /// If not null will use this instead of Camera Roll folder for photos output
    /// </summary>
    public string CustomAlbum
    {
        get { return (string)GetValue(CustomAlbumProperty); }
        set { SetValue(CustomAlbumProperty, value); }
    }


    public static readonly BindableProperty GeotagProperty = BindableProperty.Create(nameof(Geotag),
        typeof(bool),
        typeof(SkiaCamera),
        false);
    /// <summary>
    /// try to inject location metadata if to photos if GPS succeeds
    /// </summary>
    public bool Geotag
    {
        get { return (bool)GetValue(GeotagProperty); }
        set { SetValue(GeotagProperty, value); }
    }



    public static readonly BindableProperty FocalLengthProperty = BindableProperty.Create(
       nameof(FocalLength),
       typeof(double),
       typeof(SkiaCamera),
       0.0);

    public double FocalLength
    {
        get { return (double)GetValue(FocalLengthProperty); }
        set { SetValue(FocalLengthProperty, value); }
    }

    public static readonly BindableProperty FocalLengthAdjustedProperty = BindableProperty.Create(
        nameof(FocalLengthAdjusted),
        typeof(double),
        typeof(SkiaCamera),
        0.0);

    public double FocalLengthAdjusted
    {
        get { return (double)GetValue(FocalLengthAdjustedProperty); }
        set { SetValue(FocalLengthAdjustedProperty, value); }
    }

    public static readonly BindableProperty FocalLengthAdjustmentProperty = BindableProperty.Create(
        nameof(FocalLengthAdjustment),
        typeof(double),
        typeof(SkiaCamera),
        0.0);

    public double FocalLengthAdjustment
    {
        get { return (double)GetValue(FocalLengthAdjustmentProperty); }
        set { SetValue(FocalLengthAdjustmentProperty, value); }
    }

    public static readonly BindableProperty ManualZoomProperty = BindableProperty.Create(
        nameof(ManualZoom),
        typeof(bool),
        typeof(SkiaCamera),
        false);

    public bool ManualZoom
    {
        get { return (bool)GetValue(ManualZoomProperty); }
        set { SetValue(ManualZoomProperty, value); }
    }

    public static readonly BindableProperty ZoomProperty = BindableProperty.Create(
        nameof(Zoom),
        typeof(double),
        typeof(SkiaCamera),
        1.0,
        propertyChanged: NeedSetZoom);

    public double Zoom
    {
        get { return (double)GetValue(ZoomProperty); }
        set { SetValue(ZoomProperty, value); }
    }

    public static readonly BindableProperty ViewportScaleProperty = BindableProperty.Create(
        nameof(ViewportScale),
        typeof(double),
        typeof(SkiaCamera),
        1.0);

    public double ViewportScale
    {
        get { return (double)GetValue(ViewportScaleProperty); }
        set { SetValue(ViewportScaleProperty, value); }
    }

    public static readonly BindableProperty TextureScaleProperty = BindableProperty.Create(
        nameof(TextureScale),
        typeof(double),
        typeof(SkiaCamera),
        1.0, defaultBindingMode: BindingMode.OneWayToSource);

    public double TextureScale
    {
        get { return (double)GetValue(TextureScaleProperty); }
        set { SetValue(TextureScaleProperty, value); }
    }

    public static readonly BindableProperty ZoomLimitMinProperty = BindableProperty.Create(
        nameof(ZoomLimitMin),
        typeof(double),
        typeof(SkiaCamera),
        1.0);

    public double ZoomLimitMin
    {
        get { return (double)GetValue(ZoomLimitMinProperty); }
        set { SetValue(ZoomLimitMinProperty, value); }
    }

    public static readonly BindableProperty ZoomLimitMaxProperty = BindableProperty.Create(
        nameof(ZoomLimitMax),
        typeof(double),
        typeof(SkiaCamera),
        10.0);

    public double ZoomLimitMax
    {
        get { return (double)GetValue(ZoomLimitMaxProperty); }
        set { SetValue(ZoomLimitMaxProperty, value); }
    }

 

    private static void NeedSetZoom(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaCamera control)
        {
            var zoom = (double)newvalue;
            if (zoom < control.ZoomLimitMin)
            {
                zoom = control.ZoomLimitMin;
            }
            else
            if (zoom > control.ZoomLimitMax)
            {
                zoom = control.ZoomLimitMax;
            }
            control.SetZoom(zoom);
        }
    }

    protected override void OnLayoutChanged()
    {
        base.OnLayoutChanged();

        Display.Aspect = this.Aspect;
    }

    //public static readonly BindableProperty DisplayModeProperty = BindableProperty.Create(
    //    nameof(DisplayMode),
    //    typeof(StretchModes),
    //    typeof(SkiaCamera),
    //    StretchModes.Fill);

    //public StretchModes DisplayMode
    //{
    //    get { return (StretchModes)GetValue(DisplayModeProperty); }
    //    set { SetValue(DisplayModeProperty, value); }
    //}

    public static readonly BindableProperty AspectProperty = BindableProperty.Create(
        nameof(Aspect),
        typeof(TransformAspect),
        typeof(SkiaImage),
        TransformAspect.AspectCover,
        propertyChanged: NeedInvalidateMeasure);

    /// <summary>
    /// Apspect to render image with, default is AspectCover. 
    /// </summary>
    public TransformAspect Aspect
    {
        get { return (TransformAspect)GetValue(AspectProperty); }
        set { SetValue(AspectProperty, value); }
    }


    public static readonly BindableProperty StateProperty = BindableProperty.Create(
        nameof(State),
        typeof(CameraState),
        typeof(SkiaCamera),
        CameraState.Off,
        BindingMode.OneWayToSource, propertyChanged: ControlStateChanged);

    private static void ControlStateChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaCamera control)
        {
            control.StateChanged?.Invoke(control, control.State);
            control.UpdateInfo();
        }
    }

    public CameraState State
    {
        get { return (CameraState)GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    public event EventHandler<CameraState> StateChanged;

    public static readonly BindableProperty IsOnProperty = BindableProperty.Create(
        nameof(IsOn),
        typeof(bool),
        typeof(SkiaCamera),
        false,
        propertyChanged: PowerChanged);

    private static void PowerChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaCamera control)
        {
            if (control.IsOn)
            {
                control.Startup();
            }
            else
            {
                control.Stop();
            }
        }
    }

    public bool IsOn
    {
        get { return (bool)GetValue(IsOnProperty); }
        set { SetValue(IsOnProperty, value); }
    }


    public static readonly BindableProperty PickerModeProperty = BindableProperty.Create(
        nameof(PickerMode),
        typeof(CameraPickerMode),
        typeof(SkiaCamera),
        CameraPickerMode.None);

    public CameraPickerMode PickerMode
    {
        get { return (CameraPickerMode)GetValue(PickerModeProperty); }
        set { SetValue(PickerModeProperty, value); }
    }

    public static readonly BindableProperty FilterProperty = BindableProperty.Create(
        nameof(Filter),
        typeof(CameraEffect),
        typeof(SkiaCamera),
        CameraEffect.None);

    public CameraEffect Filter
    {
        get { return (CameraEffect)GetValue(FilterProperty); }
        set { SetValue(FilterProperty, value); }
    }


    public double SavedRotation { get; set; }


    //public bool
    //ShowSettings
    //{
    //    get { return (bool)GetValue(PageCamera.ShowSettingsProperty); }
    //    set { SetValue(PageCamera.ShowSettingsProperty, value); }
    //}

    #endregion

    /// <summary>
    /// The size of the camera preview in pixels 
    /// </summary>

    public SKSize PreviewSize
    {
        get
        {
            return _previewSize;
        }
        set
        {
            if (_previewSize != value)
            {
                _previewSize = value;
                OnPropertyChanged();
            }
        }
    }
    SKSize _previewSize;


    public SKSize CapturePhotoSize
    {
        get
        {
            return _capturePhotoSize;
        }

        set
        {
            if (_capturePhotoSize != value)
            {
                _capturePhotoSize = value;
                OnPropertyChanged();
                //UpdateInfo();
            }
        }
    }
    SKSize _capturePhotoSize;

    public void SetRotatedContentSize(SKSize size, int cameraRotation)
    {
        if (size.Width < 0 || size.Height < 0)
        {
            throw new Exception("Camera preview size cannot be negative.");
        }

        PreviewSize = size;

        Invalidate();
    }

    private string _DisplayInfo;
    private bool _hasPermissions;

    public string DisplayInfo
    {
        get
        {
            return _DisplayInfo;
        }
        set
        {
            if (_DisplayInfo != value)
            {
                _DisplayInfo = value;
                OnPropertyChanged();
            }
        }
    }

    #region PROPERTIES




    public static readonly BindableProperty EffectProperty = BindableProperty.Create(
        nameof(Effect),
        typeof(SkiaImageEffect),
        typeof(SkiaCamera),
        SkiaImageEffect.None,
        propertyChanged: NeedSetupPreview);

    private static void NeedSetupPreview(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaCamera control)
        {
            control.ApplyPreviewProperties();
        }
    }

    public SkiaImageEffect Effect
    {
        get { return (SkiaImageEffect)GetValue(EffectProperty); }
        set { SetValue(EffectProperty, value); }
    }

    #endregion

    private void Super_OnNativeAppPaused(object sender, EventArgs e)
    {
        StopAll();
    }

    private void Super_OnNativeAppResumed(object sender, EventArgs e)
    {
        ResumeIfNeeded();
    }

    public void ResumeIfNeeded()
    {
        if (IsOn)
            Start();
    }

    public override void OnWillDisposeWithChildren()
    {
        base.OnWillDisposeWithChildren();

        Super.OnNativeAppResumed -= Super_OnNativeAppResumed;
        Super.OnNativeAppPaused -= Super_OnNativeAppPaused;

        if (Superview != null)
        {
            Superview.DeviceRotationChanged -= DeviceRotationChanged;
        }

        if (NativeControl != null)
        {
            Stop(true);

            NativeControl?.Dispose();
        }

        NativeControl = null;

        Instances.Remove(this);
    }

    public static List<SkiaCamera> Instances = new();

    /// <summary>
    /// Stops all instances
    /// </summary>
    public static void StopAll()
    {
        foreach (var renderer in Instances)
        {
            renderer.Stop(true);
        }
    }

}
