using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Maui.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;

namespace DrawnUi.Camera;

public partial class NativeCamera : IDisposable, INativeCamera, INotifyPropertyChanged
{
    private readonly SkiaCamera _formsControl;
    private MediaCapture _mediaCapture;
    private MediaFrameReader _frameReader;
    private CameraProcessorState _state = CameraProcessorState.None;
    private bool _flashSupported;
    private bool _isCapturingStill;
    private double _zoomScale = 1.0;
    private readonly object _lockPreview = new();
    private CapturedImage _preview;
    private DeviceInformation _cameraDevice;
    private MediaFrameSource _frameSource;

    // Improved frame processing synchronization
    private readonly SemaphoreSlim _frameSemaphore = new(1, 1);
    private volatile bool _isProcessingFrame = false;

    public NativeCamera(SkiaCamera formsControl)
    {
        _formsControl = formsControl;
        Setup();
    }

    #region Properties

    public CameraProcessorState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                OnPropertyChanged();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    switch (value)
                    {
                        case CameraProcessorState.Enabled:
                            _formsControl.State = CameraState.On;
                            break;
                        case CameraProcessorState.Error:
                            _formsControl.State = CameraState.Error;
                            break;
                        default:
                            _formsControl.State = CameraState.Off;
                            break;
                    }
                });
            }
        }
    }

    public Action<CapturedImage> PreviewCaptureSuccess { get; set; }
    public Action<CapturedImage> StillImageCaptureSuccess { get; set; }
    public Action<Exception> StillImageCaptureFailed { get; set; }

    #endregion

    #region Setup

    private async void Setup()
    {
        try
        {
            //Debug.WriteLine("[NativeCameraWindows] Starting setup...");
            await SetupHardware();
            //Debug.WriteLine("[NativeCameraWindows] Hardware setup completed successfully");
            State = CameraProcessorState.Enabled;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] Setup error: {e}");
            State = CameraProcessorState.Error;
        }
    }

    private async Task SetupHardware()
    {
        //Debug.WriteLine("[NativeCameraWindows] Finding camera devices...");

        var cameraPosition = _formsControl.Facing == CameraPosition.Selfie
            ? Panel.Front
            : Panel.Back;

        var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        Debug.WriteLine($"[NativeCameraWindows] Found {devices.Count} camera devices");

        foreach (var device in devices)
        {
            Debug.WriteLine($"[NativeCameraWindows] Device: {device.Name}, Id: {device.Id}, Panel: {device.EnclosureLocation?.Panel}");
        }

        _cameraDevice = devices.FirstOrDefault(d =>
        {
            var location = d.EnclosureLocation;
            return location?.Panel == cameraPosition;
        }) ?? devices.FirstOrDefault();

        if (_cameraDevice == null)
        {
            throw new InvalidOperationException("No camera device found");
        }

        Debug.WriteLine($"[NativeCameraWindows] Selected camera: {_cameraDevice.Name}");

        //Debug.WriteLine("[NativeCameraWindows] Initializing MediaCapture...");
        _mediaCapture = new MediaCapture();
        var settings = new MediaCaptureInitializationSettings
        {
            VideoDeviceId = _cameraDevice.Id,
            StreamingCaptureMode = StreamingCaptureMode.Video,
            PhotoCaptureSource = PhotoCaptureSource.VideoPreview
        };



        await _mediaCapture.InitializeAsync(settings);
        Debug.WriteLine("[NativeCameraWindows] MediaCapture initialized successfully");

        _flashSupported = _mediaCapture.VideoDeviceController.FlashControl.Supported;
        Debug.WriteLine($"[NativeCameraWindows] Flash supported: {_flashSupported}");

        //Debug.WriteLine("[NativeCameraWindows] Setting up frame reader...");
        await SetupFrameReader();
        Debug.WriteLine("[NativeCameraWindows] Frame reader setup completed");

        // Create and assign CameraUnit to parent control using real frame source data
        CreateCameraUnit();

        Debug.WriteLine("[NativeCameraWindows] Auto-starting frame reader...");
        await StartFrameReaderAsync();
        Debug.WriteLine("[NativeCameraWindows] Frame reader auto-start completed");
    }

    private void CreateCameraUnit()
    {
        try
        {
            Debug.WriteLine("[NativeCameraWindows] Creating CameraUnit from real MediaFrameSource data...");

            // Extract real camera data from the MediaFrameSource and selected format
            var cameraSpecs = ExtractCameraSpecsFromFrameSource();

            // Create CameraUnit with real Windows camera information
            var cameraUnit = new CameraUnit
            {
                Id = _cameraDevice.Id,
                Facing = _formsControl.Facing,
                FocalLengths = cameraSpecs.FocalLengths,
                FocalLength = cameraSpecs.FocalLength,
                FieldOfView = cameraSpecs.FieldOfView,
                SensorWidth = cameraSpecs.SensorWidth,
                SensorHeight = cameraSpecs.SensorHeight,
                MinFocalDistance = cameraSpecs.MinFocalDistance,
                Meta = _formsControl.CreateMetadata()
            };

            // Assign to parent control
            _formsControl.CameraDevice = cameraUnit;

            Debug.WriteLine($"[NativeCameraWindows] CameraUnit created from real data:");
            Debug.WriteLine($"  - Id: {cameraUnit.Id}");
            Debug.WriteLine($"  - Facing: {cameraUnit.Facing}");
            Debug.WriteLine($"  - Focal Length: {cameraUnit.FocalLength}mm");
            Debug.WriteLine($"  - FOV: {cameraUnit.FieldOfView}°");
            Debug.WriteLine($"  - Sensor: {cameraUnit.SensorWidth}x{cameraUnit.SensorHeight}mm");
            Debug.WriteLine($"  - FocalLengths count: {cameraUnit.FocalLengths.Count}");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] CreateCameraUnitFromRealData error: {e}");
        }
    }

    private WindowsCameraSpecs ExtractCameraSpecsFromFrameSource()
    {
        var specs = new WindowsCameraSpecs();

        try
        {
            Debug.WriteLine("[NativeCameraWindows] Extracting camera specs from MediaFrameSource...");

            // Get the current format that was set
            var currentFormat = _frameSource?.CurrentFormat;
            if (currentFormat != null)
            {
                // Extract real resolution data
                var width = currentFormat.VideoFormat.Width;
                var height = currentFormat.VideoFormat.Height;
                var fps = currentFormat.FrameRate.Numerator / (double)currentFormat.FrameRate.Denominator;

                Debug.WriteLine($"[NativeCameraWindows] Current format: {width}x{height} @ {fps:F1} FPS");

                // Calculate sensor size from resolution (using typical pixel pitch for cameras)
                // Most modern cameras have pixel pitch between 1.0-3.0 micrometers
                var pixelPitchMicrometers = 1.4f; // Typical for modern cameras
                specs.SensorWidth = (width * pixelPitchMicrometers) / 1000.0f; // Convert to mm
                specs.SensorHeight = (height * pixelPitchMicrometers) / 1000.0f; // Convert to mm

                Debug.WriteLine($"[NativeCameraWindows] Calculated sensor size: {specs.SensorWidth:F2}x{specs.SensorHeight:F2}mm");
            }

            // Extract zoom capabilities for focal length estimation
            if (_mediaCapture?.VideoDeviceController?.ZoomControl?.Supported == true)
            {
                var zoomControl = _mediaCapture.VideoDeviceController.ZoomControl;
                var minZoom = zoomControl.Min;
                var maxZoom = zoomControl.Max;
                var currentZoom = zoomControl.Value;

                Debug.WriteLine($"[NativeCameraWindows] Zoom capabilities: {minZoom}x - {maxZoom}x (current: {currentZoom}x)");

                // Estimate base focal length from sensor size and typical field of view
                if (specs.SensorWidth > 0)
                {
                    // Assume typical webcam FOV of 60-70 degrees
                    var estimatedFOV = 65.0f;
                    var fovRadians = estimatedFOV * Math.PI / 180.0;
                    specs.FocalLength = (float)(specs.SensorWidth / (2.0 * Math.Tan(fovRadians / 2.0)));
                    specs.FieldOfView = estimatedFOV;

                    // Add focal lengths for zoom range
                    specs.FocalLengths.Add(specs.FocalLength * (float)minZoom);
                    if (maxZoom > minZoom)
                    {
                        specs.FocalLengths.Add(specs.FocalLength * (float)maxZoom);
                    }

                    Debug.WriteLine($"[NativeCameraWindows] Calculated focal length: {specs.FocalLength:F2}mm, FOV: {specs.FieldOfView}°");
                }
            }

            // Extract focus capabilities
            if (_mediaCapture?.VideoDeviceController?.FocusControl?.Supported == true)
            {
                var focusControl = _mediaCapture.VideoDeviceController.FocusControl;
                if (focusControl.SupportedFocusRanges?.Contains(AutoFocusRange.Macro) == true)
                {
                    specs.MinFocalDistance = 0.1f; // 10cm for macro
                }
                else if (focusControl.SupportedFocusRanges?.Contains(AutoFocusRange.Normal) == true)
                {
                    specs.MinFocalDistance = 0.3f; // 30cm for normal
                }
                else
                {
                    specs.MinFocalDistance = 0.5f; // 50cm default
                }
                Debug.WriteLine($"[NativeCameraWindows] Min focus distance: {specs.MinFocalDistance}m");
            }

            // Ensure we have at least basic values
            if (specs.FocalLength <= 0)
            {
                specs.FocalLength = 4.0f; // Reasonable default based on sensor size
                specs.FocalLengths.Add(specs.FocalLength);
            }
            if (specs.FieldOfView <= 0)
            {
                specs.FieldOfView = 65.0f; // Typical webcam FOV
            }
            if (specs.MinFocalDistance <= 0)
            {
                specs.MinFocalDistance = 0.3f;
            }

            Debug.WriteLine($"[NativeCameraWindows] Final specs: Focal={specs.FocalLength:F2}mm, FOV={specs.FieldOfView}°, FocalLengths={specs.FocalLengths.Count}");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] ExtractCameraSpecsFromFrameSource error: {e}");
            // Set minimal defaults
            specs.FocalLength = 4.0f;
            specs.FocalLengths.Add(specs.FocalLength);
            specs.FieldOfView = 65.0f;
            specs.SensorWidth = 5.6f;
            specs.SensorHeight = 4.2f;
            specs.MinFocalDistance = 0.3f;
        }

        return specs;
    }

    private class WindowsCameraSpecs
    {
        public float FocalLength { get; set; }
        public List<float> FocalLengths { get; set; } = new List<float>();
        public float FieldOfView { get; set; }
        public float SensorWidth { get; set; }
        public float SensorHeight { get; set; }
        public float MinFocalDistance { get; set; }
    }

    private async Task SetupFrameReader()
    {
        //Debug.WriteLine("[NativeCameraWindows] Getting frame source groups...");

        var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();
        Debug.WriteLine($"[NativeCameraWindows] Found {frameSourceGroups.Count} frame source groups");

        var selectedGroup = frameSourceGroups.FirstOrDefault(g =>
            g.SourceInfos.Any(si => si.DeviceInformation?.Id == _cameraDevice.Id));

        if (selectedGroup == null)
        {
            //Debug.WriteLine("[NativeCameraWindows] Could not find frame source group for camera, trying alternative approach...");

            if (_mediaCapture.FrameSources.Count > 0)
            {
                Debug.WriteLine($"[NativeCameraWindows] Found {_mediaCapture.FrameSources.Count} frame sources in MediaCapture");
                _frameSource = _mediaCapture.FrameSources.Values.FirstOrDefault(fs => fs.Info.SourceKind == MediaFrameSourceKind.Color);

                if (_frameSource == null)
                {
                    throw new InvalidOperationException("Could not find color frame source in MediaCapture");
                }
            }
            else
            {
                throw new InvalidOperationException("Could not find frame source group for camera");
            }
        }
        else
        {
            Debug.WriteLine($"[NativeCameraWindows] Selected frame source group: {selectedGroup.DisplayName}");

            var colorSourceInfo = selectedGroup.SourceInfos.FirstOrDefault(si =>
                si.SourceKind == MediaFrameSourceKind.Color);

            if (colorSourceInfo == null)
            {
                throw new InvalidOperationException("Could not find color frame source");
            }

            Debug.WriteLine($"[NativeCameraWindows] Selected color source: {colorSourceInfo.Id}");
            _frameSource = _mediaCapture.FrameSources[colorSourceInfo.Id];
        }

        Debug.WriteLine($"[NativeCameraWindows] Frame source info: {_frameSource.Info.Id}, Kind: {_frameSource.Info.SourceKind}");

        // Get supported formats and their frame rates
        foreach (var format in _frameSource.SupportedFormats)
        {
            var fps = format.FrameRate.Numerator / (double)format.FrameRate.Denominator;
            Debug.WriteLine($"[NativeCameraWindows] Available format: {format.VideoFormat.Width}x{format.VideoFormat.Height} @ {fps:F1} FPS");
        }

        var preferredFormat = _frameSource.SupportedFormats.FirstOrDefault(format =>
            format.VideoFormat.Width >= 640 &&
            format.VideoFormat.Height >= 480);

        if (preferredFormat != null)
        {
            var fps = preferredFormat.FrameRate.Numerator / (double)preferredFormat.FrameRate.Denominator;
            Debug.WriteLine($"[NativeCameraWindows] Setting frame source format: {preferredFormat.VideoFormat.Width}x{preferredFormat.VideoFormat.Height} @ {fps:F1} FPS");
            await _frameSource.SetFormatAsync(preferredFormat);
        }
        else
        {
            Debug.WriteLine("[NativeCameraWindows] No suitable format found, using default");
        }

        //Debug.WriteLine("[NativeCameraWindows] Creating frame reader...");
        _frameReader = await _mediaCapture.CreateFrameReaderAsync(_frameSource, MediaEncodingSubtypes.Bgra8);
        _frameReader.FrameArrived += OnFrameArrived;
        //Debug.WriteLine("[NativeCameraWindows] Frame reader created and event handler attached");
    }

    #endregion

    #region Improved Frame Processing

    /// <summary>
    /// Improved frame arrival handler with better synchronization
    /// </summary>
    private void OnFrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
    {
        // Skip if already processing a frame to prevent backlog
        if (_isProcessingFrame)
            return;

        try
        {
            using var frame = sender.TryAcquireLatestFrame();
            if (frame?.VideoMediaFrame != null)
            {
                var videoFrame = frame.VideoMediaFrame;

                if (videoFrame.SoftwareBitmap != null)
                {
                    //Debug.WriteLine("[NativeCameraWindows] Frame arrived with software bitmap, processing...");
                    ProcessFrameAsync(videoFrame.SoftwareBitmap);
                }
                else if (videoFrame.Direct3DSurface != null)
                {
                    //Debug.WriteLine("[NativeCameraWindows] Frame arrived with Direct3D surface, converting...");
                    ConvertDirect3DToSoftwareBitmapAsync(videoFrame);
                }
                else
                {
                    //Debug.WriteLine("[NativeCameraWindows] Frame arrived but no usable bitmap format available");
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] Frame processing error: {e}");
        }
    }

    /// <summary>
    /// Improved frame processing with direct pixel access (like iOS)
    /// </summary>
    private async void ProcessFrameAsync(SoftwareBitmap softwareBitmap)
    {
        if (!await _frameSemaphore.WaitAsync(1)) // Don't wait, just skip if busy
            return;

        _isProcessingFrame = true;

        try
        {
            //Debug.WriteLine($"[NativeCameraWindows] Processing frame: {softwareBitmap.PixelWidth}x{softwareBitmap.PixelHeight}, Format: {softwareBitmap.BitmapPixelFormat}");

            var skImage = await ConvertToSKImageDirectAsync(softwareBitmap);
            if (skImage != null)
            {
                //Debug.WriteLine($"[NativeCameraWindows] SKImage created successfully: {skImage.Width}x{skImage.Height}");

                var capturedImage = new CapturedImage()
                {
                    Facing = _formsControl.Facing,
                    Time = DateTime.UtcNow,
                    Image = skImage,
                    Orientation = _formsControl.DeviceRotation
                };

                // Update preview safely
                lock (_lockPreview)
                {
                    _preview?.Dispose();
                    _preview = capturedImage;
                }

                //PREVIEW FRAME READY
                PreviewCaptureSuccess?.Invoke(capturedImage);
                _formsControl.UpdatePreview();

            }
            else
            {
                //Debug.WriteLine("[NativeCameraWindows] Failed to convert SoftwareBitmap to SKImage");
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] ProcessFrameAsync error: {e}");
        }
        finally
        {
            _isProcessingFrame = false;
            _frameSemaphore.Release();
        }
    }

    /// <summary>
    /// Direct pixel access conversion using proper Windows Runtime APIs
    /// </summary>
    private async Task<SKImage> ConvertToSKImageDirectAsync(SoftwareBitmap softwareBitmap)
    {
        try
        {
            //Debug.WriteLine($"[NativeCameraWindows] Converting SoftwareBitmap to SKImage directly...");

            // Ensure correct format
            if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                softwareBitmap.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
            {
                softwareBitmap = SoftwareBitmap.Convert(softwareBitmap,
                    BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }

            var width = softwareBitmap.PixelWidth;
            var height = softwareBitmap.PixelHeight;

            // Try direct memory access first
            try
            {
                using var buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read);
                using var reference = buffer.CreateReference();

                // Try to get IMemoryBufferByteAccess interface
                if (reference is IMemoryBufferByteAccess memoryAccess)
                {
                    unsafe
                    {
                        memoryAccess.GetBuffer(out byte* dataInBytes, out uint capacity);

                        // Get proper stride from buffer plane description
                        var planeDescription = buffer.GetPlaneDescription(0);
                        var stride = planeDescription.Stride;

                        var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
                        var skImage = SKImage.FromPixels(info, new IntPtr(dataInBytes), stride);

                        //Debug.WriteLine($"[NativeCameraWindows] Created SKImage directly: {skImage?.Width}x{skImage?.Height}");
                        return skImage;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NativeCameraWindows] Direct access failed, falling back: {ex.Message}");
            }

            // Fallback: Use more efficient stream approach
            return await ConvertToSKImageStreamAsync(softwareBitmap);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] ConvertToSKImageDirectAsync error: {e}");
            return null;
        }
    }

    /// <summary>
    /// Fallback method using optimized stream conversion
    /// </summary>
    private async Task<SKImage> ConvertToSKImageStreamAsync(SoftwareBitmap softwareBitmap)
    {
        try
        {
            //Debug.WriteLine($"[NativeCameraWindows] Using stream-based conversion...");

            // Use PNG instead of BMP for better performance and smaller size
            using var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetSoftwareBitmap(softwareBitmap);

            // Optimize encoder settings for speed
            encoder.BitmapTransform.ScaledWidth = (uint)softwareBitmap.PixelWidth;
            encoder.BitmapTransform.ScaledHeight = (uint)softwareBitmap.PixelHeight;
            encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.NearestNeighbor;

            await encoder.FlushAsync();

            // Convert to byte array more efficiently
            var size = (int)stream.Size;
            var bytes = new byte[size];
            stream.Seek(0);
            var buffer = await stream.ReadAsync(bytes.AsBuffer(), (uint)size, InputStreamOptions.None);

            var skImage = SKImage.FromEncodedData(bytes);
            //Debug.WriteLine($"[NativeCameraWindows] Created SKImage from stream: {skImage?.Width}x{skImage?.Height}");
            return skImage;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] ConvertToSKImageStreamAsync error: {e}");
            return null;
        }
    }

    private async void ConvertDirect3DToSoftwareBitmapAsync(VideoMediaFrame videoFrame)
    {
        try
        {
            var softwareBitmap = await SoftwareBitmap.CreateCopyFromSurfaceAsync(videoFrame.Direct3DSurface);
            if (softwareBitmap != null)
            {
                //Debug.WriteLine("[NativeCameraWindows] Successfully converted Direct3D surface to software bitmap");
                ProcessFrameAsync(softwareBitmap);
            }
            else
            {
                //Debug.WriteLine("[NativeCameraWindows] Failed to convert Direct3D surface to software bitmap");
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] ConvertDirect3DToSoftwareBitmapAsync error: {e}");
        }
    }

    private async Task StartFrameReaderAsync()
    {
        if (_frameReader == null)
        {
            //Debug.WriteLine("[NativeCameraWindows] Frame reader is null, cannot start");
            State = CameraProcessorState.Error;
            return;
        }

        try
        {
            //Debug.WriteLine("[NativeCameraWindows] Starting frame reader...");
            var result = await _frameReader.StartAsync();
            Debug.WriteLine($"[NativeCameraWindows] Frame reader start result: {result}");

            if (result == MediaFrameReaderStartStatus.Success)
            {
                State = CameraProcessorState.Enabled;
                //Debug.WriteLine("[NativeCameraWindows] Camera started successfully");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DeviceDisplay.Current.KeepScreenOn = true;
                });
            }
            else
            {
                Debug.WriteLine($"[NativeCameraWindows] Failed to start frame reader: {result}");
                State = CameraProcessorState.Error;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] StartFrameReaderAsync error: {e}");
            State = CameraProcessorState.Error;
        }
    }

    #endregion

    #region INativeCamera Implementation

    public async void Start()
    {
        if (State == CameraProcessorState.Enabled && _frameReader != null)
        {
            //Debug.WriteLine("[NativeCameraWindows] Camera already started");
            return;
        }

        await StartFrameReaderAsync();
    }

    public async void Stop()
    {
        try
        {
            //Debug.WriteLine("[NativeCameraWindows] Stopping frame reader...");
            if (_frameReader != null)
            {
                await _frameReader.StopAsync();
                //Debug.WriteLine("[NativeCameraWindows] Frame reader stopped");
            }

            State = CameraProcessorState.None;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                DeviceDisplay.Current.KeepScreenOn = false;
            });
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] Stop error: {e}");
            State = CameraProcessorState.Error;
        }
    }

    public void TurnOnFlash()
    {
        if (_flashSupported && _mediaCapture != null)
        {
            try
            {
                _mediaCapture.VideoDeviceController.FlashControl.Enabled = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[NativeCameraWindows] TurnOnFlash error: {e}");
            }
        }
    }

    public void TurnOffFlash()
    {
        if (_flashSupported && _mediaCapture != null)
        {
            try
            {
                _mediaCapture.VideoDeviceController.FlashControl.Enabled = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[NativeCameraWindows] TurnOffFlash error: {e}");
            }
        }
    }

    public CapturedImage GetPreviewImage()
    {
        lock (_lockPreview)
        {
            var preview = _preview;
            _preview = null; // Transfer ownership like Android does
            return preview;
        }
    }

    public void ApplyDeviceOrientation(int orientation)
    {
        // Windows handles orientation automatically in most cases
    }

    public void SetZoom(float value)
    {
        _zoomScale = value;

        if (_mediaCapture?.VideoDeviceController?.ZoomControl?.Supported == true)
        {
            try
            {
                var zoomControl = _mediaCapture.VideoDeviceController.ZoomControl;
                var clampedValue = Math.Max(zoomControl.Min, Math.Min(zoomControl.Max, value));
                zoomControl.Value = clampedValue;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[NativeCameraWindows] SetZoom error: {e}");
            }
        }
    }

    public async void TakePicture()
    {
        if (_isCapturingStill || _mediaCapture == null)
            return;

        _isCapturingStill = true;

        try
        {
            Debug.WriteLine("[NativeCameraWindows] Taking picture...");

            // Create image encoding properties for high quality JPEG
            var imageProperties = ImageEncodingProperties.CreateJpeg();
            imageProperties.Width = 1920; // Set higher resolution for still capture
            imageProperties.Height = 1080;

            // Capture photo to stream
            using var stream = new InMemoryRandomAccessStream();
            await _mediaCapture.CapturePhotoToStreamAsync(imageProperties, stream);
            Debug.WriteLine($"[NativeCameraWindows] Photo captured to stream, size: {stream.Size} bytes");

            // Read stream data
            stream.Seek(0);
            var bytes = new byte[stream.Size];
            await stream.AsStream().ReadAsync(bytes, 0, bytes.Length);

            // Create SKImage from captured data
            var skImage = SKImage.FromEncodedData(bytes);
            Debug.WriteLine($"[NativeCameraWindows] SKImage created: {skImage?.Width}x{skImage?.Height}");

            var capturedImage = new CapturedImage()
            {
                Facing = _formsControl.Facing,
                Time = DateTime.UtcNow,
                Image = skImage,
                Orientation = _formsControl.DeviceRotation
            };

            MainThread.BeginInvokeOnMainThread(() =>
            {
                StillImageCaptureSuccess?.Invoke(capturedImage);
            });

            // IMPORTANT: Restart frame reader to resume preview
            Debug.WriteLine("[NativeCameraWindows] Restarting frame reader to resume preview...");
            await RestartFrameReaderAsync();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] TakePicture error: {e}");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StillImageCaptureFailed?.Invoke(e);
            });

            // Try to restart frame reader even if capture failed
            try
            {
                await RestartFrameReaderAsync();
            }
            catch (Exception restartEx)
            {
                Debug.WriteLine($"[NativeCameraWindows] Failed to restart frame reader: {restartEx}");
            }
        }
        finally
        {
            _isCapturingStill = false;
        }
    }

    private async Task RestartFrameReaderAsync()
    {
        try
        {
            if (_frameReader != null)
            {
                Debug.WriteLine("[NativeCameraWindows] Stopping frame reader...");
                await _frameReader.StopAsync();

                Debug.WriteLine("[NativeCameraWindows] Starting frame reader...");
                var result = await _frameReader.StartAsync();
                Debug.WriteLine($"[NativeCameraWindows] Frame reader restart result: {result}");

                if (result == MediaFrameReaderStartStatus.Success)
                {
                    Debug.WriteLine("[NativeCameraWindows] Frame reader restarted successfully - preview should resume");
                }
                else
                {
                    Debug.WriteLine($"[NativeCameraWindows] Failed to restart frame reader: {result}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] RestartFrameReaderAsync error: {e}");
        }
    }

    public async Task<string> SaveJpgStreamToGallery(Stream stream, string filename, double cameraSavedRotation, string album)
    {
        try
        {
            var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            var saveFolder = picturesLibrary.SaveFolder;

            // Create album subfolder if specified, similar to Android/iOS behavior
            if (!string.IsNullOrEmpty(album))
            {
                Debug.WriteLine($"[NativeCameraWindows] Creating album folder: {album}");
                saveFolder = await saveFolder.CreateFolderAsync(album, CreationCollisionOption.OpenIfExists);
            }
            else
            {
                // Default to "Camera" folder like Android does when no album specified
                Debug.WriteLine("[NativeCameraWindows] Using default Camera folder");
                saveFolder = await saveFolder.CreateFolderAsync("Camera", CreationCollisionOption.OpenIfExists);
            }

            var file = await saveFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
            Debug.WriteLine($"[NativeCameraWindows] Saving to: {file.Path}");

            using var fileStream = await file.OpenStreamForWriteAsync();
            await stream.CopyToAsync(fileStream);

            Debug.WriteLine($"[NativeCameraWindows] Successfully saved image to: {file.Path}");
            return file.Path;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] SaveJpgStreamToGallery error: {e}");
            return null;
        }
    }

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        try
        {
            Stop();

            _frameReader?.Dispose();
            _mediaCapture?.Dispose();
            _frameSemaphore?.Dispose();

            lock (_lockPreview)
            {
                _preview?.Dispose();
                _preview = null;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[NativeCameraWindows] Dispose error: {e}");
        }
    }

    #endregion
}

