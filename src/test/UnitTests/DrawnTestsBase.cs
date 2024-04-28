using DrawnUi.Maui.Draw;
using SkiaSharp;

namespace UnitTests;

public class DrawnTestsBase : TestsBase
{
    public static SKPicture RenderWithOperationsContext(SKRect cacheRecordingArea, Action<SkiaDrawingContext> draw)
    {
        using (var recorder = new SKPictureRecorder())
        {
            var canvas = recorder.BeginRecording(cacheRecordingArea);

            var recordingContext = new SkiaDrawingContext()
            {
                Superview = null,
                FrameTimeNanos = Super.GetCurrentTimeNanos(),
                Canvas = canvas,
                Width = canvas.DeviceClipBounds.Width,
                Height = canvas.DeviceClipBounds.Height
            };


            draw(recordingContext);

            var skPicture = recorder.EndRecording();

            //var renderObject = new(SkiaCacheType.Operations, skPicture, recordArea);

            return skPicture;
        }
    }
}