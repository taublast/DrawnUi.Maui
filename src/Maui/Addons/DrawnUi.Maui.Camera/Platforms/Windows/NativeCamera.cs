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

        Debug.WriteLine("[NativeCameraWindows] Auto-starting frame reader...");
        await StartFrameReaderAsync();
        Debug.WriteLine("[NativeCameraWindows] Frame reader auto-start completed");
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
            var imageProperties = ImageEncodingProperties.CreateJpeg();

            using var stream = new InMemoryRandomAccessStream();
            await _mediaCapture.CapturePhotoToStreamAsync(imageProperties, stream);

            stream.Seek(0);
            var bytes = new byte[stream.Size];
            await stream.AsStream().ReadAsync(bytes, 0, bytes.Length);

            var skImage = SKImage.FromEncodedData(bytes);

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
        }
        catch (Exception e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StillImageCaptureFailed?.Invoke(e);
            });
        }
        finally
        {
            _isCapturingStill = false;
        }
    }

    public async Task<string> SaveJpgStreamToGallery(Stream stream, string filename, double cameraSavedRotation, string album)
    {
        try
        {
            var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            var saveFolder = picturesLibrary.SaveFolder;

            if (!string.IsNullOrEmpty(album))
            {
                saveFolder = await saveFolder.CreateFolderAsync(album, CreationCollisionOption.OpenIfExists);
            }

            var file = await saveFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

            using var fileStream = await file.OpenStreamForWriteAsync();
            await stream.CopyToAsync(fileStream);

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

