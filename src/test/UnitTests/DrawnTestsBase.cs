using DrawnUi.Draw;
using SkiaSharp;

namespace UnitTests;

public class DrawnTestsBase : TestsBase
{
    public static SKPicture RenderWithOperationsContext(SKRect cacheRecordingArea, Action<DrawingContext> draw)
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

            var ctx = new DrawingContext(recordingContext, cacheRecordingArea, 1);
            
            draw(ctx);

            var skPicture = recorder.EndRecording();

            //var renderObject = new(SkiaCacheType.Operations, skPicture, recordArea);

            return skPicture;
        }
    }

}
