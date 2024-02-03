namespace DrawnUi.Maui.Controls;

/// <summary>
///     In heavy TODO state
/// </summary>
public class SkiaGif : SkiaImage
{
    protected GifAnimator Animator { get; } = new();

    public class GifAnimation : IDisposable
    {
        protected object LockFrames = new();

        protected SKBitmap[] Frames { get; set; }

        protected int Width { get; set; }

        protected int Height { get; set; }

        protected int DurationMs { get; set; }

        protected int[] FramesPositionsMs { get; set; }

        public bool IsDisposed { get; set; }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                lock (LockFrames)
                {
                    if (Frames != null)
                        foreach (var frame in Frames)
                            frame.Dispose();
                    Frames = null;
                }
            }
        }

        public void Render(SKCanvas canvas, SKRect destination, int frame)
        {
            // Get the bitmap and center it
            var bitmap = Frames[frame];
            canvas.DrawBitmap(bitmap, destination); //, BitmapStretch.Uniform);
        }

        /// <summary>
        ///     Position 0.0 -> 0.1
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="destination"></param>
        /// <param name="position"></param>
        public void Render(SKCanvas canvas, SKRect destination, float position)
        {
            //todo calculate frame number upon position:
            var frame = 0;

            Render(canvas, destination, frame);
        }

        public void LoadFromStream(Stream stream)
        {
            SKBitmap[] bitmaps;
            int height, width, frameCount, totalDuration = 0;
            int[] durations;
            int[] accumulatedDurations;

            using var skStream = new SKManagedStream(stream);
            using var codec = SKCodec.Create(skStream);
            width = codec.Info.Width;
            height = codec.Info.Height;

            // Get info and allocate source bitmaps
            frameCount = codec.FrameCount;
            bitmaps = new SKBitmap[frameCount];
            durations = new int[frameCount];
            accumulatedDurations = new int[frameCount];

            // Note: There's also a RepetitionCount property of SKCodec not used here

            // Loop through the frames
            for (var frame = 0; frame < frameCount; frame++)
            {
                // From the FrameInfo collection, get the duration of each frame
                durations[frame] = codec.FrameInfo[frame].Duration;

                // Create a full-color bitmap for each frame
                var imageInfo = new SKImageInfo(width, height);
                bitmaps[frame] = new SKBitmap(imageInfo);

                // Get the address of the pixels in that bitmap
                var pointer = bitmaps[frame].GetPixels();

                // Create an SKCodecOptions value to specify the frame
                SKCodecOptions codecOptions = new(frame); // SKCodecOptions(frame, false);

                // Copy pixels from the frame into the bitmap
                codec.GetPixels(imageInfo, pointer, codecOptions);
            }

            // Calculate total duration
            for (var frame = 0; frame < durations.Length; frame++) totalDuration += durations[frame];

            // Calculate the accumulated durations 
            for (var frame = 0; frame < durations.Length; frame++)
                accumulatedDurations[frame] = durations[frame] +
                                              (frame == 0 ? 0 : accumulatedDurations[frame - 1]);

            lock (LockFrames)
            {
                if (!IsDisposed)
                {
                    var kill = Frames;
                    Frames = bitmaps;
                    Width = width;
                    Height = height;
                    DurationMs = totalDuration;
                    FramesPositionsMs = accumulatedDurations;

                    if (kill != Frames && kill != null)
                        Tasks.StartDelayed(TimeSpan.FromSeconds(2), () =>
                        {
                            foreach (var frame in kill) frame.Dispose();
                        });
                }
            }
        }
    }


    public class GifAnimator : AnimatedFramesRenderer
    { }
}