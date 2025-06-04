using Android.Hardware.Camera2;
using Android.Media;
using Java.Lang;
using Exception = System.Exception;
using Image = Android.Media.Image;

namespace DrawnUi.Camera
{
    public partial class NativeCamera
    {
        public class CameraCaptureListener : CameraCaptureSession.CaptureCallback,
            Android.Media.ImageReader.IOnImageAvailableListener
        {
            private readonly NativeCamera owner;
            private TaskCompletionSource<BrightnessResult> _brightnessResultHandler;
            private bool _isMeasuringBrightness;

            public void SetBrightnessMeasurementHandler(TaskCompletionSource<BrightnessResult> handler)
            {
                _brightnessResultHandler = handler;
                _isMeasuringBrightness = true;
            }

            public CameraCaptureListener(NativeCamera owner)
            {
                if (owner == null)
                    throw new System.ArgumentNullException("owner");
                this.owner = owner;
            }

            private TaskCompletionSource<ExposureResult> _meteringTcs;
            private double _meteringAperture;

            public void SetMeteringResultHandler(TaskCompletionSource<ExposureResult> tcs, double aperture)
            {
                _meteringTcs = tcs;
                _meteringAperture = aperture;
            }

            public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
            {
                Process(result);
            }

            public override void OnCaptureProgressed(CameraCaptureSession session, CaptureRequest request, CaptureResult partialResult)
            {
                Process(partialResult);
            }

            public void OnImageAvailable(ImageReader reader)
            {

                Image image = null;
                try
                {
                    System.Diagnostics.Debug.WriteLine("[CAMERA] AcquireLatestImage..");

                    image = reader.AcquireNextImage();
                    if (image != null)
                    {
                        System.Diagnostics.Debug.WriteLine("[CAMERA] Processing still image..");
                        owner.OnCapturedImage(image);
                    }
                }
                catch (Exception e)
                {
                    Super.Log(e);
                    owner.OnCaptureError(e);
                }
                finally
                {
                    if (image != null)
                    {
                        image.Close();
                    }
                    owner.CapturingStill = false;
                    System.Diagnostics.Debug.WriteLine("[CAMERA] Still capture finished");
                }
            }

            private void Process(CaptureResult result)
            {
                // Check if we're waiting for a brightness measurement result
                if (_isMeasuringBrightness && _brightnessResultHandler != null && !_brightnessResultHandler.Task.IsCompleted)
                {
                    try
                    {
                        // Get the camera's chosen exposure settings for "proper" exposure
                        var measuredExposureTime = (long)result.Get(CaptureResult.SensorExposureTime);
                        var measuredSensitivity = (int)result.Get(CaptureResult.SensorSensitivity);
                        var measuredAperture = (float)result.Get(CaptureResult.LensAperture);

                        // Convert to standard units
                        double shutterSpeed = measuredExposureTime / 1_000_000_000.0; // nanoseconds to seconds
                        double iso = measuredSensitivity;
                        double aperture = measuredAperture;

                        // Calculate the EV that the camera chose for "proper" exposure
                        var chosenEV = MathF.Log2((float)((aperture * aperture) / shutterSpeed)) + MathF.Log2((float)(iso / 100.0));

                        // Convert EV to scene brightness (lux)
                        // Formula: Lux = K * 2^EV / (ISO/100)
                        // K ≈ 12.5 for reflected light (camera's built-in meter measures reflected light)
                        const double K = 12.5;
                        var sceneBrightness = K * MathF.Pow(2, chosenEV) / (iso / 100.0);

                        System.Diagnostics.Debug.WriteLine($"[ANDROID CAMERA] Measured: f/{aperture:F1}, 1/{(1 / shutterSpeed):F0}, ISO{iso:F0}");
                        System.Diagnostics.Debug.WriteLine($"[ANDROID CAMERA] Calculated EV: {chosenEV:F1}, Scene brightness: {sceneBrightness:F0} lux");

                        var brightnessResult = new BrightnessResult
                        {
                            Success = true,
                            Brightness = sceneBrightness
                        };

                        _brightnessResultHandler.TrySetResult(brightnessResult);
                        _brightnessResultHandler = null;
                        _isMeasuringBrightness = false;
                    }
                    catch (Exception e)
                    {
                        _brightnessResultHandler?.TrySetException(e);
                        _brightnessResultHandler = null;
                        _isMeasuringBrightness = false;
                    }
                    return; // Don't process further when measuring brightness
                }

                // Check if we're waiting for a metering result
                if (_meteringTcs != null && !_meteringTcs.Task.IsCompleted)
                {
                    try
                    {
                        var actualExposureTime = (long)result.Get(CaptureResult.SensorExposureTime);
                        var actualSensitivity = (int)result.Get(CaptureResult.SensorSensitivity);
                        var actualAperture = (float)result.Get(CaptureResult.LensAperture);

                        double shutterSpeed = actualExposureTime / 1_000_000_000.0;
                        double iso = actualSensitivity;
                        double aperture = actualAperture;

                        double ev = MathF.Log2((float)((aperture * aperture) / shutterSpeed)) - MathF.Log2((float)(iso / 100.0f));

                        // Get exposure compensation
                        var exposureCompensation = (int)result.Get(CaptureResult.ControlAeExposureCompensation);

                        var exposureResult = new ExposureResult
                        {
                            Success = true,
                            ExposureValue = ev,
                            Brightness = exposureCompensation,
                            SuggestedShutterSpeed = shutterSpeed,
                            SuggestedIso = iso,
                            SuggestedAperture = aperture
                        };

                        _meteringTcs.TrySetResult(exposureResult);
                        _meteringTcs = null; 
                    }
                    catch (Exception e)
                    {
                        _meteringTcs?.TrySetException(e);
                        _meteringTcs = null;
                    }
                }

                owner.FormsControl.CameraDevice.Meta.ISO = (int)result.Get(CaptureResult.SensorSensitivity);
                owner.FormsControl.CameraDevice.Meta.FocalLength = (float)result.Get(CaptureResult.LensFocalLength);

                switch (owner.mState)
                {
                    case NativeCamera.STATE_WAITING_LOCK:
                    {
                        Integer afState = (Integer)result.Get(CaptureResult.ControlAfState);
                        if (afState == null)
                        {
                            owner.mState = NativeCamera.STATE_PICTURE_TAKEN; // avoids multiple picture callbacks
                            owner.StartCapturingStill();
                        }
                        else
                        if (
                            afState == null ||
                            ((int)ControlAFState.FocusedLocked) == afState.IntValue() ||
                            ((int)ControlAFState.NotFocusedLocked) == afState.IntValue() ||
                            ((int)ControlAFState.Inactive) == afState.IntValue())
                        {
                            // ControlAeState can be null on some devices
                            // also they can not be supporting auto-focus

                            Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                            if (aeState == null ||
                                aeState.IntValue() == ((int)ControlAEState.Converged))
                            {
                                owner.mState = NativeCamera.STATE_PICTURE_TAKEN;
                                owner.StartCapturingStill();
                            }
                            else
                            {
                                owner.RunPrecaptureSequence();
                            }
                        }

                        break;
                    }

                    case NativeCamera.STATE_WAITING_PRECAPTURE:
                    {
                        // ControlAeState can be null on some devices
                        Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                        if (aeState == null ||
                            aeState.IntValue() == ((int)ControlAEState.Precapture) ||
                            aeState.IntValue() == ((int)ControlAEState.FlashRequired))
                        {
                            owner.mState = NativeCamera.STATE_WAITING_NON_PRECAPTURE;
                        }
                        break;
                    }

                    case NativeCamera.STATE_WAITING_NON_PRECAPTURE:
                    {
                        // ControlAeState can be null on some devices
                        Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                        if (aeState == null || aeState.IntValue() != ((int)ControlAEState.Precapture))
                        {
                            owner.mState = NativeCamera.STATE_PICTURE_TAKEN;

                            owner.StartCapturingStill();
                        }
                        break;
                    }
                }
            }
        }
    }
}
