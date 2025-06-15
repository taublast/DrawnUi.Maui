using Android.Media;
using SkiaSharp.Views.Android;
using Exception = System.Exception;
using Trace = System.Diagnostics.Trace;

namespace DrawnUi.Camera;

public partial class NativeCamera : Java.Lang.Object, ImageReader.IOnImageAvailableListener, INativeCamera
{
    /// <summary>
    /// IOnImageAvailableListener
    /// </summary>
    /// <param name="reader"></param>
    public void OnImageAvailable(ImageReader reader)
    {
        lock (lockProcessingPreviewFrame)
        {
            if (lockProcessing || FormsControl.Height <= 0 || FormsControl.Width <= 0 || CapturingStill)
                return;

            FramesReader = reader;

            if (Output != null)
            {
                var allocated = Output;

                lockProcessing = true;

                Android.Media.Image image = null;
                try
                {
                    // ImageReader
                    image = reader.AcquireLatestImage();
                    if (image != null)
                    {
                        if (allocated.Allocation != null && allocated.Bitmap is { Width: > 0, Height: > 0 })
                        {
                            ProcessImage(image, allocated.Allocation);
                            allocated.Update();
                            var sk = allocated.Bitmap.ToSKImage();
                            if (sk != null)
                            {
                                var outImage = new CapturedImage()
                                {
                                    Facing = FormsControl.Facing,
                                    Time = DateTime.UtcNow, // todo use image.Timestamp ?
                                    Image = sk,
                                    Orientation = FormsControl.DeviceRotation
                                };
                                Preview = outImage;
                                OnPreviewCaptureSuccess(outImage);
                                FormsControl.UpdatePreview();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
                finally
                {
                    if (image != null)
                    {
                        //lockAllocation = false;
                        image.Close();
                    }

                    lockProcessing = false;
                }
            }
        }
    }
}
