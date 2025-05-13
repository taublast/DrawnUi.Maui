using Android.Hardware.Camera2;
using Android.Media;
using Java.Lang;
using CameraError = Android.Hardware.Camera2.CameraError;
using Exception = System.Exception;
using Image = Android.Media.Image;

namespace DrawnUi.Camera;

public partial class NativeCamera
{


    public class CameraCaptureStillPictureSessionCallback : CameraCaptureSession.CaptureCallback
    {
        private static readonly string TAG = "CameraCaptureStillPictureSessionCallback";

        private readonly NativeCamera owner;

        public CameraCaptureStillPictureSessionCallback(NativeCamera owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");

            this.owner = owner;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {

            var SensorExposureTime = result.Get(CaptureResult.SensorExposureTime);
            var ControlAfMode = result.Get(CaptureResult.ControlAfMode);

            var meta = owner.FormsControl.CreateMetadata();

            meta.Vendor = $"{Android.OS.Build.Manufacturer}";
            meta.Model = $"{Android.OS.Build.Model}";
            meta.Orientation = (int)result.Get(CaptureResult.JpegOrientation);
            meta.ISO = (int)result.Get(CaptureResult.SensorSensitivity);
            meta.FocalLength = (float)result.Get(CaptureResult.LensFocalLength);

            owner.FormsControl.CameraDevice.Meta = meta;
        }
    }

    public class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        private readonly NativeCamera owner;

        public CameraCaptureSessionCallback(NativeCamera owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            //owner.ShowToast(ResStrings.Error);
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            // The camera is already closed
            if (null == owner.mCameraDevice)
            {
                return;
            }

            // When the session is ready, we start displaying the preview.
            owner.CaptureSession = session;
            try
            {
                // Auto focus should be continuous for camera preview.
                owner.mPreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.ContinuousPicture);

                owner.mPreviewRequestBuilder.AddTarget(owner.mImageReaderPreview.Surface);

                // Flash is automatically enabled when necessary.
                owner.SetCapturingStillOptions(owner.mPreviewRequestBuilder);

                // Finally, we start displaying the camera preview.
                owner.mPreviewRequest = owner.mPreviewRequestBuilder.Build();

                owner.CaptureSession.SetRepeatingRequest(
                    owner.mPreviewRequest,
                    owner.mCaptureCallback,
                    owner.mBackgroundHandler);
            }
            catch (Exception e)
            {
                Super.Log(e);
                //owner.ShowToast(ResStrings.Error);
            }
        }
    }

    public class CameraStateListener : CameraDevice.StateCallback
    {
        private readonly NativeCamera owner;

        public CameraStateListener(NativeCamera owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnClosed(CameraDevice camera)
        {
            //base.OnClosed(camera);

            // owner.StopBackgroundThread();
        }

        public override void OnOpened(CameraDevice cameraDevice)
        {
            try
            {
                // This method is called when the camera is opened.  We start camera preview here.
                owner.mCameraOpenCloseLock.Release();
                owner.mCameraDevice = cameraDevice;
                owner.CreateCameraPreviewSession();
            }
            catch (Exception e)
            {
                Super.Log(e);
            }
        }

        public override void OnDisconnected(CameraDevice cameraDevice)
        {
            try
            {
                cameraDevice.Close();

                owner.mCameraDevice = null;

                owner.mCameraOpenCloseLock.Release();
            }
            catch (Exception e)
            {
                Super.Log(e);
            }

        }

        public override void OnError(CameraDevice cameraDevice, CameraError error)
        {

            if (owner == null)
                return;

            try
            {
                owner.mCameraOpenCloseLock.Release();
                cameraDevice.Close();
                owner.mCameraDevice = null;
            }
            catch (Exception e)
            {
                Super.Log(e);
            }

        }

    }

    public class CameraCaptureListener : CameraCaptureSession.CaptureCallback,
        Android.Media.ImageReader.IOnImageAvailableListener
    {
        private readonly NativeCamera owner;

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


