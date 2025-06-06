#if IOS || MACCATALYST
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AppoMobi.Specials;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using DrawnUi.Controls;
using Foundation;
using ImageIO;
using Microsoft.Maui.Media;
using Photos;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using UIKit;
using static AVFoundation.AVMetadataIdentifiers;

namespace DrawnUi.Camera;

// Lightweight container for raw frame data - no SKImage creation
internal class RawFrameData : IDisposable
{
    public IntPtr BaseAddress { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int BytesPerRow { get; set; }
    public DateTime Time { get; set; }
    public Rotation CurrentRotation { get; set; }
    public CameraPosition Facing { get; set; }
    public int Orientation { get; set; }
    public SKData Data { get; set; }

    public void Dispose()
    {
        Data?.Dispose();
        Data = null;
    }
}

public partial class NativeCamera : NSObject, IDisposable, INativeCamera, INotifyPropertyChanged, IAVCaptureVideoDataOutputSampleBufferDelegate
{
    protected readonly SkiaCamera FormsControl;
    private AVCaptureSession _session;
    private AVCaptureVideoDataOutput _videoDataOutput;
    private AVCaptureStillImageOutput _stillImageOutput;
    private AVCaptureDeviceInput _deviceInput;
    private DispatchQueue _videoDataOutputQueue;
    private CameraProcessorState _state = CameraProcessorState.None;
    private bool _flashSupported;
    private bool _isCapturingStill;
    private double _zoomScale = 1.0;
    private readonly object _lockPreview = new();
    private CapturedImage _preview;
    bool _cameraUnitInitialized;

    // Frame processing throttling - only prevent concurrent processing
    private volatile bool _isProcessingFrame = false;
    private int _skippedFrameCount = 0;
    private int _processedFrameCount = 0;

    // Raw frame data for lazy SKImage creation
    private readonly object _lockRawFrame = new();
    private RawFrameData _latestRawFrame;
    private RawFrameData _oldRawFrame;
    
    // Orientation tracking properties
    private UIInterfaceOrientation _uiOrientation;
    private UIDeviceOrientation _deviceOrientation;
    private AVCaptureVideoOrientation _videoOrientation;
    private UIImageOrientation _imageOrientation;
    private NSObject _orientationObserver;
    
    public Rotation CurrentRotation { get; private set; } = Rotation.rotate0Degrees;

    public AVCaptureDevice CaptureDevice
    {
        get
        {
            if (_deviceInput == null)
                return null;

            return _deviceInput.Device;
        }
    }

    public NativeCamera(SkiaCamera formsControl)
    {
        FormsControl = formsControl;
        _session = new AVCaptureSession();
        _videoDataOutput = new AVCaptureVideoDataOutput();
        _videoDataOutputQueue = new DispatchQueue("VideoDataOutput", false);

        SetupOrientationObserver();

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
                            FormsControl.State = CameraState.On;
                            break;
                        case CameraProcessorState.Error:
                            FormsControl.State = CameraState.Error;
                            break;
                        default:
                            FormsControl.State = CameraState.Off;
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

    private void Setup()
    {
        try
        {
            SetupHardware();
            State = CameraProcessorState.Enabled;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[NativeCameraiOS] Setup error: {e}");
            State = CameraProcessorState.Error;
        }
    }

    private void SetupOrientationObserver()
    {
        // Initialize orientation values
        _uiOrientation = UIApplication.SharedApplication.StatusBarOrientation;
        _deviceOrientation = UIDevice.CurrentDevice.Orientation;
        _videoOrientation = AVCaptureVideoOrientation.Portrait;
        
        System.Diagnostics.Debug.WriteLine($"[CAMERA SETUP] Initial orientations - UI: {_uiOrientation}, Device: {_deviceOrientation}, Video: {_videoOrientation}");
        
        // Set up orientation change observer
        _orientationObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            UIDevice.OrientationDidChangeNotification, 
            (notification) =>
            {
                System.Diagnostics.Debug.WriteLine($"[CAMERA ORIENTATION] Device orientation changed notification received");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateOrientationFromMainThread();
                    // Also update the SkiaCamera's DeviceRotation to ensure both systems are in sync
                    var deviceOrientation = UIDevice.CurrentDevice.Orientation;
                    var rotation = 0;
                    switch (deviceOrientation)
                    {
                        case UIDeviceOrientation.Portrait:
                            rotation = 0;
                            break;
                        case UIDeviceOrientation.LandscapeLeft:
                            rotation = 90;
                            break;
                        case UIDeviceOrientation.PortraitUpsideDown:
                            rotation = 180;
                            break;
                        case UIDeviceOrientation.LandscapeRight:
                            rotation = 270;
                            break;
                        default:
                            rotation = 0;
                            break;
                    }
                    System.Diagnostics.Debug.WriteLine($"[CAMERA ORIENTATION] Setting SkiaCamera DeviceRotation to {rotation} degrees");
                    FormsControl.DeviceRotation = rotation;
                });
            });
    }

    private void SetupHardware()
    {
        _session.BeginConfiguration();
        _cameraUnitInitialized = false;

        // Set session preset
        if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
        {
            _session.SessionPreset = AVCaptureSession.PresetHigh;
        }
        else
        {
            _session.SessionPreset = AVCaptureSession.PresetInputPriority;
        }

        // Configure camera position
        var cameraPosition = FormsControl.Facing == CameraPosition.Selfie 
            ? AVCaptureDevicePosition.Front 
            : AVCaptureDevicePosition.Back;

        AVCaptureDevice videoDevice = null;
        
        if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && FormsControl.Type == CameraType.Max)
        {
            videoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInTripleCamera, AVMediaTypes.Video, cameraPosition);
        }

        if (videoDevice == null)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 2) && FormsControl.Type == CameraType.Max)
            {
                videoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInDualCamera, AVMediaTypes.Video, cameraPosition);
            }

            if (videoDevice == null)
            {
                var videoDevices = AVCaptureDevice.DevicesWithMediaType(AVMediaTypes.Video.GetConstant());
                videoDevice = videoDevices.FirstOrDefault(d => d.Position == cameraPosition);
                
                if (videoDevice == null)
                {
                    State = CameraProcessorState.Error;
                    _session.CommitConfiguration();
                    return;
                }
            }
        }

        var allFormats = videoDevice.Formats.ToList();
        AVCaptureDeviceFormat format = null;
        
        if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
        {
            format = allFormats.Where(x => x.MultiCamSupported)
                .OrderByDescending(x => x.HighResolutionStillImageDimensions.Width)
                .FirstOrDefault();
        }

        if (format == null)
        {
            format = allFormats.OrderByDescending(x => x.HighResolutionStillImageDimensions.Width)
                .FirstOrDefault();
        }

        NSError error;
        if (videoDevice.LockForConfiguration(out error))
        {
            if (videoDevice.SmoothAutoFocusSupported)
                videoDevice.SmoothAutoFocusEnabled = true;
                
            videoDevice.ActiveFormat = format;
            
            // Ensure exposure is set to continuous auto exposure during setup
            if (videoDevice.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                videoDevice.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                System.Diagnostics.Debug.WriteLine($"[CAMERA SETUP] Set initial exposure mode to ContinuousAutoExposure");
            }
            
            // Reset exposure bias to neutral
            if (videoDevice.MinExposureTargetBias != videoDevice.MaxExposureTargetBias)
            {
                videoDevice.SetExposureTargetBias(0, null);
                System.Diagnostics.Debug.WriteLine($"[CAMERA SETUP] Reset exposure bias to 0");
            }
            
            videoDevice.UnlockForConfiguration();
        }

        while (_session.Inputs.Any())
        {
            _session.RemoveInput(_session.Inputs[0]);
        }

        _deviceInput = new AVCaptureDeviceInput(videoDevice, out error);
        if (error != null)
        {
            Console.WriteLine($"Could not create video device input: {error.LocalizedDescription}");
            _session.CommitConfiguration();
            State = CameraProcessorState.Error;
            return;
        }

        _session.AddInput(_deviceInput);

        var dictionary = new NSMutableDictionary();
        dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
        _stillImageOutput = new AVCaptureStillImageOutput()
        {
            OutputSettings = new NSDictionary()
        };
        _stillImageOutput.HighResolutionStillImageOutputEnabled = true;
        _session.AddOutput(_stillImageOutput);

        if (_session.CanAddOutput(_videoDataOutput))
        {
            _session.AddOutput(_videoDataOutput);
            _videoDataOutput.AlwaysDiscardsLateVideoFrames = true;
            _videoDataOutput.WeakVideoSettings = new NSDictionary(CVPixelBuffer.PixelFormatTypeKey, 
                CVPixelFormatType.CV32BGRA);
            _videoDataOutput.SetSampleBufferDelegate(this, _videoDataOutputQueue);
            
            // Set initial video orientation from the connection
            var videoConnection = _videoDataOutput.ConnectionFromMediaType(AVMediaTypes.Video.GetConstant());
            if (videoConnection != null && videoConnection.SupportsVideoOrientation)
            {
                _videoOrientation = videoConnection.VideoOrientation;
                System.Diagnostics.Debug.WriteLine($"[CAMERA SETUP] Initial video orientation: {_videoOrientation}");
            }
        }
        else
        {
            Console.WriteLine("Could not add video data output to the session");
            _session.CommitConfiguration();
            State = CameraProcessorState.Error;
            return;
        }

        _flashSupported = videoDevice.FlashAvailable;

        var focalLengths = new List<float>();
        //var physicalFocalLength = 4.15f;
        //focalLengths.Add(physicalFocalLength);

        var cameraUnit = new CameraUnit
        {
            Id = videoDevice.UniqueID,
            Facing = FormsControl.Facing,
            FocalLengths = focalLengths,
            FieldOfView = videoDevice.ActiveFormat.VideoFieldOfView,
            Meta = FormsControl.CreateMetadata()
        };

        //other data will be filled when camera starts working..

        FormsControl.CameraDevice = cameraUnit;

        var formatDescription = videoDevice.ActiveFormat.FormatDescription as CMVideoFormatDescription;
        if (formatDescription != null)
        {
            var dimensions = formatDescription.Dimensions;
            FormsControl.SetRotatedContentSize(new SKSize(dimensions.Width, dimensions.Height), 0);
        }

        _session.CommitConfiguration();

        UpdateDetectOrientation();
    }

 

    #endregion

    #region INativeCamera Implementation

    public void Start()
    {
        if (State == CameraProcessorState.Enabled && _session.Running)
            return;

        try
        {
            _session.StartRunning();
            State = CameraProcessorState.Enabled;
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DeviceDisplay.Current.KeepScreenOn = true;
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[NativeCameraiOS] Start error: {e}");
            State = CameraProcessorState.Error;
        }
    }

    public void Stop(bool force = false)
    {
        SetCapture(null);

        // Clear raw frame data
        lock (_lockRawFrame)
        {
            _latestRawFrame?.Dispose();
            _oldRawFrame?.Dispose();
            _latestRawFrame = null;
            _oldRawFrame = null;
        }

        if (State == CameraProcessorState.None && !force)
            return;

        if (State != CameraProcessorState.Enabled && !force)
            return; //avoid spam

        try
        {
            _session.StopRunning();
            State = CameraProcessorState.None;
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DeviceDisplay.Current.KeepScreenOn = false;
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[NativeCameraiOS] Stop error: {e}");
            State = CameraProcessorState.Error;
        }
    }

    public void TurnOnFlash()
    {
        if (!_flashSupported || _deviceInput?.Device == null)
            return;

        NSError error;
        if (_deviceInput.Device.LockForConfiguration(out error))
        {
            try
            {
                if (_deviceInput.Device.HasTorch)
                {
                    _deviceInput.Device.TorchMode = AVCaptureTorchMode.On;
                }
                if (_deviceInput.Device.HasFlash)
                {
                    _deviceInput.Device.FlashMode = AVCaptureFlashMode.On;
                }
            }
            finally
            {
                _deviceInput.Device.UnlockForConfiguration();
            }
        }
    }

    public void TurnOffFlash()
    {
        if (!_flashSupported || _deviceInput?.Device == null)
            return;

        NSError error;
        if (_deviceInput.Device.LockForConfiguration(out error))
        {
            try
            {
                if (_deviceInput.Device.HasTorch)
                {
                    _deviceInput.Device.TorchMode = AVCaptureTorchMode.Off;
                }
                if (_deviceInput.Device.HasFlash)
                {
                    _deviceInput.Device.FlashMode = AVCaptureFlashMode.Off;
                }
            }
            finally
            {
                _deviceInput.Device.UnlockForConfiguration();
            }
        }
    }

    public void SetZoom(float zoom)
    {
        if (_deviceInput?.Device == null)
            return;

        _zoomScale = zoom;

        NSError error;
        if (_deviceInput.Device.LockForConfiguration(out error))
        {
            try
            {
                var clampedZoom = (nfloat)Math.Max(_deviceInput.Device.MinAvailableVideoZoomFactor,
                    Math.Min(zoom, _deviceInput.Device.MaxAvailableVideoZoomFactor));
                    
                _deviceInput.Device.VideoZoomFactor = clampedZoom;
            }
            finally
            {
                _deviceInput.Device.UnlockForConfiguration();
            }
        }
    }

    public void ApplyDeviceOrientation(int orientation)
    {
        UpdateOrientationFromMainThread();
    }

    public void TakePicture()
    {
        if (_isCapturingStill || _stillImageOutput == null)
            return;

        Task.Run(async () =>
        {
            try
            {
                _isCapturingStill = true;

                var status = PHPhotoLibrary.AuthorizationStatus;
                if (status != PHAuthorizationStatus.Authorized)
                {
                    status = await PHPhotoLibrary.RequestAuthorizationAsync();
                    if (status != PHAuthorizationStatus.Authorized)
                    {
                        StillImageCaptureFailed?.Invoke(new UnauthorizedAccessException("Photo library access denied"));
                        return;
                    }
                }

                var videoConnection = _stillImageOutput.ConnectionFromMediaType(AVMediaTypes.Video.GetConstant());
                var sampleBuffer = await _stillImageOutput.CaptureStillImageTaskAsync(videoConnection);
                var jpegData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);

                using var uiImage = UIImage.LoadFromData(jpegData);
                var skImage = uiImage.ToSKImage();

                var capturedImage = new CapturedImage()
                {
                    Facing = FormsControl.Facing,
                    Time = DateTime.UtcNow,
                    Image = skImage,
                    Orientation = FormsControl.DeviceRotation
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
        });
    }

    public CapturedImage GetPreviewImage()
    {
        // First check if we have a ready preview
        lock (_lockPreview)
        {
            if (_preview != null)
            {
                var get = _preview;
                _preview = null;

                // If we're returning an image, make sure we don't have it queued for disposal
                if (_kill == get)
                {
                    _kill = null;
                }

                return get;
            }
        }

        // No ready preview, create one from raw frame data if available
        lock (_lockRawFrame)
        {
            if (_latestRawFrame == null)
                return null;

            try
            {
                // Create SKImage on demand from raw data
                var info = new SKImageInfo(_latestRawFrame.Width, _latestRawFrame.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
                using var rawImage = SKImage.FromPixels(info, _latestRawFrame.Data, _latestRawFrame.BytesPerRow);

                // Apply rotation if needed
                SKImage rotatedImage;
                if (_latestRawFrame.CurrentRotation != Rotation.rotate0Degrees)
                {
                    using var bitmap = SKBitmap.FromImage(rawImage);
                    using var rotatedBitmap = HandleOrientation(bitmap, (double)_latestRawFrame.CurrentRotation);
                    rotatedImage = SKImage.FromBitmap(rotatedBitmap);
                }
                else
                {
                    rotatedImage = rawImage.Subset(SKRectI.Create(0, 0, _latestRawFrame.Width, _latestRawFrame.Height));
                }

                var capturedImage = new CapturedImage()
                {
                    Facing = _latestRawFrame.Facing,
                    Time = _latestRawFrame.Time,
                    Image = rotatedImage,
                    Orientation = _latestRawFrame.Orientation
                };

                // Clear the raw frame since we've used it
                _oldRawFrame?.Dispose();
                _oldRawFrame = _latestRawFrame;
                _latestRawFrame = null;

                return capturedImage;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[NativeCameraiOS] GetPreviewImage error: {e}");
                return null;
            }
        }
    }

    public async Task<string> SaveJpgStreamToGallery(Stream stream, string filename, double cameraSavedRotation, string album)
    {
        try
        {
            var data = NSData.FromStream(stream);
            
            bool complete = false;
            string resultPath = null;

            PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
            {
                var options = new PHAssetResourceCreationOptions
                {
                    OriginalFilename = filename
                };

                var creationRequest = PHAssetCreationRequest.CreationRequestForAsset();
                creationRequest.AddResource(PHAssetResourceType.Photo, data, options);

            }, (success, error) =>
            {
                if (success)
                {
                    resultPath = filename;
                }
                else
                {
                    Console.WriteLine($"SaveJpgStreamToGallery error: {error}");
                }
                complete = true;
            });

            while (!complete)
            {
                await Task.Delay(10);
            }

            return resultPath;
        }
        catch (Exception e)
        {
            Console.WriteLine($"SaveJpgStreamToGallery error: {e}");
            return null;
        }
    }

    #endregion

    #region AVCaptureVideoDataOutputSampleBufferDelegate

    [Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
    public void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
    {
        if (FormsControl == null || _isCapturingStill || State != CameraProcessorState.Enabled)
            return;

        // THROTTLING: Only skip if previous frame is still being processed (prevents thread overwhelm)
        if (_isProcessingFrame)
        {
            _skippedFrameCount++;
            return;
        }

        _isProcessingFrame = true;
        _processedFrameCount++;

        // Log stats every 300 frames
        if (_processedFrameCount % 300 == 0)
        {
            System.Diagnostics.Debug.WriteLine($"[NativeCameraiOS] Frame stats - Processed: {_processedFrameCount}, Skipped: {_skippedFrameCount}");
        }

        try
        {
            using var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer;
            if (pixelBuffer == null)
                return;

            pixelBuffer.Lock(CVPixelBufferLock.ReadOnly);

            try
            {
                var attachments = sampleBuffer.GetAttachments(CMAttachmentMode.ShouldPropagate);
                var exif = attachments["{Exif}"] as NSDictionary;
                var focal = exif["FocalLength"].ToString().ToFloat();
                var iso = exif["ISOSpeedRatings"]?.ToString().ToFloat() ?? 100f;
                var aperture = exif["FNumber"]?.ToString().ToFloat() ?? 1.8f;
                var shutterSpeed = exif["ExposureTime"]?.ToString().ToFloat() ?? (1f / 60f);

                if (!_cameraUnitInitialized)
                {
                    _cameraUnitInitialized = true;

                    var focals = new List<float>();
                    var focal35mm = exif["FocalLenIn35mmFilm"].ToString().ToFloat();
                    var name = exif["LensModel"].ToString();
                    var lenses = exif["LensSpecification "] as NSDictionary;
                    if (lenses != null)
                    {
                        foreach (var lens in lenses)
                        {
                            var add = lens.ToString().ToDouble();
                            focals.Add((float)add);
                        }
                    }
                    else
                    {
                        focals.Add((float)focal);
                    }

                    //FOV = 2 arctan (x / (2 f)), where x is the diagonal of the film.
                    var unit = FormsControl.CameraDevice;

                    unit.Id = name;
                    unit.SensorCropFactor = focal35mm / focal;
                    unit.FocalLengths = focals;
                    unit.PixelXDimension = exif["PixelXDimension"].ToString().ToFloat();
                    unit.PixelYDimension = exif["PixelYDimension"].ToString().ToFloat();
                    unit.FocalLength = focal;

                    var info = _deviceInput.Device.ActiveFormat;
                    var pixelsZoom = info.VideoZoomFactorUpscaleThreshold;
                    float aspectH = unit.PixelXDimension / unit.PixelYDimension;
                    float aspectV = 1.0f;
                    float fovH = info.VideoFieldOfView;
                    float fovV = fovH / aspectH;

                    var sensorWidth = (float)(2 * unit.FocalLength * Math.Tan(fovH * Math.PI / 2.0f * 180));
                    var sensorHeight = (float)(2 * unit.FocalLength * Math.Tan(fovV * Math.PI / 2.0f * 180));

                    unit.SensorHeight = sensorHeight;
                    unit.SensorWidth = sensorWidth;
                    unit.FieldOfView = fovH;

                }

                FormsControl.CameraDevice.Meta.FocalLength = focal;
                FormsControl.CameraDevice.Meta.ISO = (int)iso;
                FormsControl.CameraDevice.Meta.Aperture = aperture;
                FormsControl.CameraDevice.Meta.Shutter = shutterSpeed;

                var width = (int)pixelBuffer.Width;
                var height = (int)pixelBuffer.Height;
                var bytesPerRow = (int)pixelBuffer.BytesPerRow;
                var baseAddress = pixelBuffer.BaseAddress;

                var dataSize = height * bytesPerRow;
                var data = SKData.Create(baseAddress, dataSize);

                // Store raw frame data instead of creating SKImage immediately
                var rawFrame = new RawFrameData
                {
                    BaseAddress = baseAddress,
                    Width = width,
                    Height = height,
                    BytesPerRow = bytesPerRow,
                    Time = DateTime.UtcNow,
                    CurrentRotation = CurrentRotation,
                    Facing = FormsControl.Facing,
                    Orientation = (int)CurrentRotation,
                    Data = data
                };

                SetRawFrame(rawFrame);
                FormsControl.UpdatePreview();
            }
            finally
            {
                pixelBuffer.Unlock(CVPixelBufferLock.ReadOnly);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[NativeCameraiOS] Frame processing error: {e}");
        }
        finally
        {
            // IMPORTANT: Always reset processing flag
            _isProcessingFrame = false;
        }
    }

    CapturedImage _kill;

    void SetRawFrame(RawFrameData rawFrame)
    {
        lock (_lockRawFrame)
        {
            // Dispose old raw frame data
            _oldRawFrame?.Dispose();
            _oldRawFrame = _latestRawFrame;
            _latestRawFrame = rawFrame;
        }
    }

    void SetCapture(CapturedImage capturedImage)
    {
        lock (_lockPreview)
        {
            // Apple's recommended pattern: Keep only the latest frame
            // Dispose the old preview immediately if we have a new one
            if (_preview != null && capturedImage != null)
            {
                _preview.Dispose();
                _preview = null;
            }

            // Dispose any queued frame
            _kill?.Dispose();
            _kill = _preview;
            _preview = capturedImage;
        }
    }

    #endregion



    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Orientation Handling

    public SKBitmap HandleOrientation(SKBitmap bitmap, double sensor)
    {
        SKBitmap rotated;
        switch (sensor)
        {
            case 180:
                using (var surface = new SKCanvas(bitmap))
                {
                    surface.RotateDegrees(180, bitmap.Width / 2.0f, bitmap.Height / 2.0f);
                    surface.DrawBitmap(bitmap.Copy(), 0, 0);
                }
                return bitmap;

            case 270: //iphone on the right side
                rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                using (var surface = new SKCanvas(rotated))
                {
                    surface.Translate(0, rotated.Height);
                    surface.RotateDegrees(270);
                    surface.DrawBitmap(bitmap, 0, 0);
                }
                return rotated;

            case 90: // iphone on the left side
                rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                using (var surface = new SKCanvas(rotated))
                {
                    surface.Translate(rotated.Width, 0);
                    surface.RotateDegrees(90);
                    surface.DrawBitmap(bitmap, 0, 0);
                }
                return rotated;

            default:
                return bitmap;
        }
    }

    public void UpdateOrientationFromMainThread()
    {
        _uiOrientation = UIApplication.SharedApplication.StatusBarOrientation;
        _deviceOrientation = UIDevice.CurrentDevice.Orientation;
        UpdateDetectOrientation();
    }

    public void UpdateDetectOrientation()
    {
        if (_videoDataOutput?.Connections?.Any() == true)
        {
            // Get current video orientation from connection
            var videoConnection = _videoDataOutput.ConnectionFromMediaType(AVMediaTypes.Video.GetConstant());
            if (videoConnection != null && videoConnection.SupportsVideoOrientation)
            {
                _videoOrientation = videoConnection.VideoOrientation;
            }
            
            CurrentRotation = GetRotation(
                _uiOrientation,
                _videoOrientation,
                _deviceInput?.Device?.Position ?? AVCaptureDevicePosition.Back);

            switch (_uiOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                    _imageOrientation = UIImageOrientation.Right;
                    break;
                case UIInterfaceOrientation.PortraitUpsideDown:
                    _imageOrientation = UIImageOrientation.Left;
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                    _imageOrientation = UIImageOrientation.Up;
                    break;
                case UIInterfaceOrientation.LandscapeRight:
                    _imageOrientation = UIImageOrientation.Down;
                    break;
                default:
                    _imageOrientation = UIImageOrientation.Up;
                    break;
            }

            System.Diagnostics.Debug.WriteLine($"[UpdateDetectOrientation]: rotation: {CurrentRotation}, orientation: {_imageOrientation}, device: {_deviceInput?.Device?.Position}, video: {_videoOrientation}, ui:{_uiOrientation}");
        }
    }

    public Rotation GetRotation(
        UIInterfaceOrientation interfaceOrientation,
        AVCaptureVideoOrientation videoOrientation,
        AVCaptureDevicePosition cameraPosition)
    {
        /*
         Calculate the rotation between the videoOrientation and the interfaceOrientation.
         The direction of the rotation depends upon the camera position.
         */

        switch (videoOrientation)
        {
            case AVCaptureVideoOrientation.Portrait:
                switch (interfaceOrientation)
                {
                    case UIInterfaceOrientation.LandscapeRight:
                        if (cameraPosition == AVCaptureDevicePosition.Front)
                        {
                            return Rotation.rotate90Degrees;
                        }
                        else
                        {
                            return Rotation.rotate270Degrees;
                        }

                    case UIInterfaceOrientation.LandscapeLeft:
                        if (cameraPosition == AVCaptureDevicePosition.Front)
                        {
                            return Rotation.rotate270Degrees;
                        }
                        else
                        {
                            return Rotation.rotate90Degrees;
                        }

                    case UIInterfaceOrientation.Portrait:
                        return Rotation.rotate0Degrees;

                    case UIInterfaceOrientation.PortraitUpsideDown:
                        return Rotation.rotate180Degrees;

                    default:
                        return Rotation.rotate0Degrees;
                }

            case AVCaptureVideoOrientation.PortraitUpsideDown:
                switch (interfaceOrientation)
                {
                    case UIInterfaceOrientation.LandscapeRight:
                        if (cameraPosition == AVCaptureDevicePosition.Front)
                        {
                            return Rotation.rotate270Degrees;
                        }
                        else
                        {
                            return Rotation.rotate90Degrees;
                        }

                    case UIInterfaceOrientation.LandscapeLeft:
                        if (cameraPosition == AVCaptureDevicePosition.Front)
                        {
                            return Rotation.rotate90Degrees;
                        }
                        else
                        {
                            return Rotation.rotate270Degrees;
                        }

                    case UIInterfaceOrientation.Portrait:
                        return Rotation.rotate180Degrees;

                    case UIInterfaceOrientation.PortraitUpsideDown:
                        return Rotation.rotate0Degrees;

                    default:
                        return Rotation.rotate180Degrees;
                }

            case AVCaptureVideoOrientation.LandscapeRight:
                switch (interfaceOrientation)
                {
                    case UIInterfaceOrientation.LandscapeRight:
                        return Rotation.rotate0Degrees;

                    case UIInterfaceOrientation.LandscapeLeft:
                        return Rotation.rotate180Degrees;

                    case UIInterfaceOrientation.Portrait:
                        if (cameraPosition == AVCaptureDevicePosition.Front)
                        {
                            return Rotation.rotate270Degrees;
                        }
                        else
                        {
                            return Rotation.rotate90Degrees;
                        }

                    case UIInterfaceOrientation.PortraitUpsideDown:
                        if (cameraPosition == AVCaptureDevicePosition.Front)
                        {
                            return Rotation.rotate90Degrees;
                        }
                        else
                        {
                            return Rotation.rotate270Degrees;
                        }

                    default:
                        return Rotation.rotate0Degrees;
                }

            case AVCaptureVideoOrientation.LandscapeLeft:
                switch (interfaceOrientation)
                {
                    case UIInterfaceOrientation.LandscapeLeft:
                        return Rotation.rotate0Degrees;

                    case UIInterfaceOrientation.LandscapeRight:
                        return Rotation.rotate180Degrees;

                    case UIInterfaceOrientation.Portrait:
                        if (cameraPosition == AVCaptureDevicePosition.Front)
                        {
                            return Rotation.rotate90Degrees;
                        }
                        else
                        {
                            return Rotation.rotate270Degrees;
                        }

                    case UIInterfaceOrientation.PortraitUpsideDown:
                        if (cameraPosition == AVCaptureDevicePosition.Front)
                        {
                            return Rotation.rotate270Degrees;
                        }
                        else
                        {
                            return Rotation.rotate90Degrees;
                        }

                    default:
                        return Rotation.rotate0Degrees;
                }

            default:
                return Rotation.rotate0Degrees;
        }
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Stop();
            
            _session?.Dispose();
            _videoDataOutput?.Dispose();
            _stillImageOutput?.Dispose();
            _deviceInput?.Dispose();
            _videoDataOutputQueue?.Dispose();

            SetCapture(null);
            _kill?.Dispose();

            // Clean up raw frame data
            lock (_lockRawFrame)
            {
                _latestRawFrame?.Dispose();
                _oldRawFrame?.Dispose();
                _latestRawFrame = null;
                _oldRawFrame = null;
            }

            // Clean up orientation observer
            if (_orientationObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_orientationObserver);
                _orientationObserver?.Dispose();
                _orientationObserver = null;
            }
        }

        base.Dispose(disposing);
    }

    #endregion
}
#endif
