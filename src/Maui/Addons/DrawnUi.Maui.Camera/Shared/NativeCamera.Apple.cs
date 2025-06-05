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
using Photos;
using SkiaSharp;
using SkiaSharp.Views.iOS;
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
    bool _cameraUnitInitialized;

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
    /// Measures scene brightness using camera auto exposure system
    /// </summary>
    public async Task<BrightnessResult> MeasureSceneBrightness(MeteringMode meteringMode)
    {
        try
        {
            if (CaptureDevice == null)
                return new BrightnessResult { Success = false, ErrorMessage = "Camera not initialized" };

            NSError error;
            if (!CaptureDevice.LockForConfiguration(out error))
                return new BrightnessResult { Success = false, ErrorMessage = error?.LocalizedDescription };

            switch (meteringMode)
            {
                case MeteringMode.Spot:
                    var centerPoint = new CGPoint(0.5, 0.5);
                    if (CaptureDevice.ExposurePointOfInterestSupported)
                    {
                        CaptureDevice.ExposurePointOfInterest = centerPoint;
                    }
                    break;

                case MeteringMode.CenterWeighted:
                    if (CaptureDevice.ExposurePointOfInterestSupported)
                    {
                        CaptureDevice.ExposurePointOfInterest = new CGPoint(0.5, 0.5);
                    }
                    break;
            }

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

            await Task.Delay(1000);

            var measuredDuration = CaptureDevice.ExposureDuration.Seconds;
            var measuredISO = CaptureDevice.ISO;
            var measuredAperture = CaptureDevice.LensAperture;

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
        var cameraPosition = _formsControl.Facing == CameraPosition.Selfie 
            ? AVCaptureDevicePosition.Front 
            : AVCaptureDevicePosition.Back;

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
            Facing = _formsControl.Facing,
            FocalLengths = focalLengths,
            //FocalLength = physicalFocalLength,
            FieldOfView = videoDevice.ActiveFormat.VideoFieldOfView,
            //SensorWidth = 4.8f,
            //SensorHeight = 3.6f,
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

    public void ApplyCameraUnit(CameraUnit cameraUnit)
    {

        if (cameraUnit == null && _formsControl.CameraDevice != null)
        {
            var info = _deviceInput.Device.ActiveFormat;
            var focal = cameraUnit.FocalLength;

            var pixelsZoom = info.VideoZoomFactorUpscaleThreshold;
            float aspectH = cameraUnit.PixelXDimension / cameraUnit.PixelYDimension;
            float aspectV = 1.0f;
            float fovH = info.VideoFieldOfView;
            float fovV = fovH / aspectH;

            var sensorWidth = (float)(2 * cameraUnit.FocalLength * Math.Tan(fovH * Math.PI / 2.0f * 180));
            var sensorHeight = (float)(2 * cameraUnit.FocalLength * Math.Tan(fovV * Math.PI / 2.0f * 180));

            cameraUnit.SensorHeight = sensorHeight;
            cameraUnit.SensorWidth = sensorWidth;
            cameraUnit.FieldOfView = fovH;
            cameraUnit.Meta = _formsControl.CreateMetadata();
            cameraUnit.Facing = _formsControl.Facing;

            _formsControl.CameraDevice = cameraUnit;
        }

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
        // iOS handles orientation automatically
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
        if (_formsControl == null || _isCapturingStill || State != CameraProcessorState.Enabled)
            return;

        try
        {
            using var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer;
            if (pixelBuffer == null)
                return;

            pixelBuffer.Lock(CVPixelBufferLock.ReadOnly);

            try
            {
                if (!_cameraUnitInitialized)
                {
                    _cameraUnitInitialized = true;

                    var attachments = sampleBuffer.GetAttachments(CMAttachmentMode.ShouldPropagate);
                    var focals = new List<float>();
                    var exif = attachments["{Exif}"] as NSDictionary;

                    var focal35mm = exif["FocalLenIn35mmFilm"].ToString().ToFloat();
                    var focal = exif["FocalLength"].ToString().ToFloat();
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
                    var unit = new CameraUnit
                    {
                        Id = name,
                        SensorCropFactor = focal35mm / focal,
                        FocalLengths = focals,
                        PixelXDimension = exif["PixelXDimension"].ToString().ToFloat(),
                        PixelYDimension = exif["PixelYDimension"].ToString().ToFloat(),
                        FocalLength = focal
                    };

                    ApplyCameraUnit(unit);
                }


                var width = (int)pixelBuffer.Width;
                var height = (int)pixelBuffer.Height;
                var bytesPerRow = (int)pixelBuffer.BytesPerRow;
                var baseAddress = pixelBuffer.BaseAddress;

                var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);

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

                SetCapture(capturedImage);

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

    CapturedImage _kill;

    void SetCapture(CapturedImage capturedImage)
    {
        lock (_lockPreview)
        {
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
        }

        base.Dispose(disposing);
    }

    #endregion
}
#endif
