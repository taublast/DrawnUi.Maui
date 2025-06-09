using Android.Hardware.Camera2;
using CameraError = Android.Hardware.Camera2.CameraError;
using Exception = System.Exception;

namespace DrawnUi.Camera;

public partial class NativeCamera
{

    public static void FillMetadata(Metadata meta, CaptureResult result)
    {
        // Get the camera's chosen exposure settings for "proper" exposure
        var measuredExposureTime = (long)result.Get(CaptureResult.SensorExposureTime);
        var measuredSensitivity = (int)result.Get(CaptureResult.SensorSensitivity);
        var measuredAperture = (float)result.Get(CaptureResult.LensAperture);
        var usedLens = (float)result.Get(CaptureResult.LensFocalLength);

        // Convert to standard units
        double shutterSpeed = measuredExposureTime / 1_000_000_000.0; // nanoseconds to seconds
        double iso = measuredSensitivity;
        double aperture = measuredAperture;

        meta.FocalLength = usedLens;
        meta.ISO = (int)iso;
        meta.Aperture = aperture;
        meta.Shutter = shutterSpeed;

        meta.Orientation = (int)result.Get(CaptureResult.JpegOrientation);
    }

    public class StillPhotoCaptureCallback : CameraCaptureSession.CaptureCallback
    {
        private readonly NativeCamera _camera;

        public StillPhotoCaptureCallback(NativeCamera camera)
        {
            if (camera == null)
                throw new System.ArgumentNullException("camera");

            this._camera = camera;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            //var ControlAfMode = result.Get(CaptureResult.ControlAfMode);

            var meta = _camera.FormsControl.CreateMetadata();
            meta.Vendor = $"{Android.OS.Build.Manufacturer}";
            meta.Model = $"{Android.OS.Build.Model}";

            NativeCamera.FillMetadata(meta, result);

            _camera.FormsControl.CameraDevice.Meta = meta;
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
}


