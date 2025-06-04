using Android.Hardware.Camera2;
using CameraError = Android.Hardware.Camera2.CameraError;
using Exception = System.Exception;

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
}


