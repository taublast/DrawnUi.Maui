#if IOS || MACCATALYST
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using DrawnUi.Controls;
using Foundation;
using ImageIO;
using Photos;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UIKit;

namespace DrawnUi.Camera;

public partial class NativeCamera : NSObject, IDisposable, INativeCamera, INotifyPropertyChanged, IAVCaptureVideoDataOutputSampleBufferDelegate
{
    private readonly SkiaCamera _formsControl;
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
        _formsControl = formsControl;
        _session = new AVCaptureSession();
        _videoDataOutput = new AVCaptureVideoDataOutput();
        _videoDataOutputQueue = new DispatchQueue("VideoDataOutput", false);
        
        Setup();
    }

    /// <summary>
    /// Measures actual scene brightness using iOS camera's auto exposure system
    /// </summary>
    public async Task<BrightnessResult> MeasureSceneBrightness(MeteringMode meteringMode)
    {
        try
        {
            if (CaptureDevice == null)
                return new BrightnessResult { Success = false, ErrorMessage = "Camera not initialized" };

            // Lock for configuration
            NSError error;
            if (!CaptureDevice.LockForConfiguration(out error))
                return new BrightnessResult { Success = false, ErrorMessage = error?.LocalizedDescription };

            // Set metering mode first
            switch (meteringMode)
            {
                case MeteringMode.Spot:
                    // Set focus/exposure point to center for spot metering
                    var centerPoint = new CGPoint(0.5, 0.5);
                    if (CaptureDevice.ExposurePointOfInterestSupported)
                    {
                        CaptureDevice.ExposurePointOfInterest = centerPoint;
                    }
                    break;

                case MeteringMode.CenterWeighted:
                    // Reset to default (center-weighted is usually the default)
                    if (CaptureDevice.ExposurePointOfInterestSupported)
                    {
                        CaptureDevice.ExposurePointOfInterest = new CGPoint(0.5, 0.5);
                    }
                    break;
            }

            // Set to AUTO exposure mode to let camera measure the scene
            if (CaptureDevice.IsExposureModeSupported(AVCaptureExposureMode.AutoExpose))
            {
                CaptureDevice.ExposureMode = AVCaptureExposureMode.AutoExpose;
            }
            else if (CaptureDevice.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                CaptureDevice.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
            }
            else
            {
                CaptureDevice.UnlockForConfiguration();
                return new BrightnessResult { Success = false, ErrorMessage = "Auto exposure not supported" };
            }

            CaptureDevice.UnlockForConfiguration();

            // Wait for camera to measure and adjust (important!)
            await Task.Delay(1000);

            // Now read what the camera decided
            var measuredDuration = CaptureDevice.ExposureDuration.Seconds;
            var measuredISO = CaptureDevice.ISO;
            var measuredAperture = CaptureDevice.LensAperture;

            // Calculate the EV that the camera chose for "proper" exposure
            var chosenEV = Math.Log2((measuredAperture * measuredAperture) / measuredDuration) + Math.Log2(measuredISO / 100.0);

            // Convert EV to scene brightness (lux)
            // Formula: Lux = K * 2^EV / (ISO/100)
            // K ≈ 12.5 for reflected light (camera's built-in meter measures reflected light)
            const double K = 12.5;
            var sceneBrightness = K * Math.Pow(2, chosenEV) / (measuredISO / 100.0);

            System.Diagnostics.Debug.WriteLine($"[iOS CAMERA] Measured: f/{measuredAperture:F1}, 1/{(1 / measuredDuration):F0}, ISO{measuredISO:F0}");
            System.Diagnostics.Debug.WriteLine($"[iOS CAMERA] Calculated EV: {chosenEV:F1}, Scene brightness: {sceneBrightness:F0} lux");

            return new BrightnessResult
            {
                Success = true,
                Brightness = sceneBrightness
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[iOS CAMERA ERROR] {ex.Message}");
            return new BrightnessResult { Success = false, ErrorMessage = ex.Message };
        }
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

    private void SetupHardware()
    {
        _session.BeginConfiguration();

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
        var cameraPosition = _formsControl.Facing == CameraPosition.Selfie 
            ? AVCaptureDevicePosition.Front 
            : AVCaptureDevicePosition.Back;

        // Get video device
        AVCaptureDevice videoDevice = null;
        
        if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0) && _formsControl.Type == CameraType.Max)
        {
            videoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInTripleCamera, AVMediaTypes.Video, cameraPosition);
        }

        if (videoDevice == null)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 2) && _formsControl.Type == CameraType.Max)
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

        // Get best format
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

        // Configure device
        NSError error;
        if (videoDevice.LockForConfiguration(out error))
        {
            if (videoDevice.SmoothAutoFocusSupported)
                videoDevice.SmoothAutoFocusEnabled = true;
                
            videoDevice.ActiveFormat = format;
            videoDevice.UnlockForConfiguration();
        }

        // Clear existing inputs
        while (_session.Inputs.Any())
        {
            _session.RemoveInput(_session.Inputs[0]);
        }

        // Add video input
        _deviceInput = new AVCaptureDeviceInput(videoDevice, out error);
        if (error != null)
        {
            Console.WriteLine($"Could not create video device input: {error.LocalizedDescription}");
            _session.CommitConfiguration();
            State = CameraProcessorState.Error;
            return;
        }

        _session.AddInput(_deviceInput);

        // Setup still image output
        var dictionary = new NSMutableDictionary();
        dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
        _stillImageOutput = new AVCaptureStillImageOutput()
        {
            OutputSettings = new NSDictionary()
        };
        _stillImageOutput.HighResolutionStillImageOutputEnabled = true;
        _session.AddOutput(_stillImageOutput);

        // Setup video data output for preview
        if (_session.CanAddOutput(_videoDataOutput))
        {
            _session.AddOutput(_videoDataOutput);
            _videoDataOutput.AlwaysDiscardsLateVideoFrames = true;
            _videoDataOutput.WeakVideoSettings = new NSDictionary(CVPixelBuffer.PixelFormatTypeKey, 
                CVPixelFormatType.CV32BGRA);
            _videoDataOutput.SetSampleBufferDelegate(this, _videoDataOutputQueue);
        }
        else
        {
            Console.WriteLine("Could not add video data output to the session");
            _session.CommitConfiguration();
            State = CameraProcessorState.Error;
            return;
        }

        // Check flash support
        _flashSupported = videoDevice.FlashAvailable;

        // Get camera info
        var focalLengths = new List<float>();
        
        // Default focal length for iOS devices - this should ideally be read from device characteristics
        var physicalFocalLength = 4.15f; // Typical iPhone focal length
        
        focalLengths.Add(physicalFocalLength);

        var cameraUnit = new CameraUnit
        {
            Id = videoDevice.UniqueID,
            Facing = _formsControl.Facing,
            FocalLengths = focalLengths,
            FocalLength = physicalFocalLength,
            FieldOfView = videoDevice.ActiveFormat.VideoFieldOfView,
            SensorWidth = 4.8f, // Default iPhone sensor width in mm
            SensorHeight = 3.6f, // Default iPhone sensor height in mm
            Meta = _formsControl.CreateMetadata()
        };

        _formsControl.CameraDevice = cameraUnit;

        var formatDescription = videoDevice.ActiveFormat.FormatDescription as CMVideoFormatDescription;
        if (formatDescription != null)
        {
            var dimensions = formatDescription.Dimensions;
            _formsControl.SetRotatedContentSize(new SKSize(dimensions.Width, dimensions.Height), 0);
        }

        _session.CommitConfiguration();
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

    public void Stop()
    {
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
        // iOS handles this automatically through AVCaptureConnection
        // The orientation is applied when creating the final image
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

                // Check photo library permissions
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

                // Convert to UIImage then to SKImage
                using var uiImage = UIImage.LoadFromData(jpegData);
                var skImage = uiImage.ToSKImage();

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
        });
    }

    public CapturedImage GetPreviewImage()
    {
        lock (_lockPreview)
        {
            var get = _preview;
            _preview = null;
            return get;
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

            // Wait for completion
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
        if (_formsControl == null || _isCapturingStill)
            return;

        try
        {
            using var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer;
            if (pixelBuffer == null)
                return;

            pixelBuffer.Lock(CVPixelBufferLock.ReadOnly);

            try
            {
                var width = (int)pixelBuffer.Width;
                var height = (int)pixelBuffer.Height;
                var bytesPerRow = (int)pixelBuffer.BytesPerRow;
                var baseAddress = pixelBuffer.BaseAddress;

                var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);

                // Create SKImage from pixel buffer data
                var dataSize = height * bytesPerRow;
                using var data = SKData.Create(baseAddress, dataSize);
                var image = SKImage.FromPixels(info, data, bytesPerRow);
                
                var capturedImage = new CapturedImage()
                {
                    Facing = _formsControl.Facing,
                    Time = DateTime.UtcNow,
                    Image = image,
                    Orientation = _formsControl.DeviceRotation
                };

                lock (_lockPreview)
                {
                    _preview?.Dispose();
                    _preview = capturedImage;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    PreviewCaptureSuccess?.Invoke(capturedImage);
                    _formsControl.UpdatePreview();
                });
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
    }

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            
            lock (_lockPreview)
            {
                _preview?.Dispose();
                _preview = null;
            }
        }

        base.Dispose(disposing);
    }

    #endregion
}
#endif
