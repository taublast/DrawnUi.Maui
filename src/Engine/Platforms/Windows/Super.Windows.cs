using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DrawnUi.Maui.Draw
{
    public partial class Super
    {

        protected static void SetupFrameLooper()
        {
            Tasks.StartDelayed(TimeSpan.FromMilliseconds(1), async () =>
            {
                await StartFrameLooperAsync(CancellationToken.None);
            });
        }

        protected static async Task StartFrameLooperAsync(CancellationToken cancellationToken)
        {
            var frameStopwatch = new Stopwatch();
            var loopStopwatch = Stopwatch.StartNew(); 
            long lastFrameEnd = loopStopwatch.ElapsedMilliseconds;
            var targetIntervalMs = 1000.0 / 120.0; // Targeting 60 FPS (* 2 with double buffering)

            while (!cancellationToken.IsCancellationRequested)
            {
                frameStopwatch.Restart();

                // Render DrawnView
                OnFrame?.Invoke(0);

                frameStopwatch.Stop();  

                var frameExecutionTimeMs = frameStopwatch.Elapsed.TotalMilliseconds;
                var elapsedTimeSinceLastFrame = loopStopwatch.ElapsedMilliseconds - lastFrameEnd;
                var timeToWait = targetIntervalMs - elapsedTimeSinceLastFrame - frameExecutionTimeMs;

                if (timeToWait > 0)
                    Thread.Sleep(TimeSpan.FromMilliseconds(timeToWait));

                lastFrameEnd = loopStopwatch.ElapsedMilliseconds;
            }
        }

        public static void Init()
        {
            if (Initialized)
                return;

            Initialized = true;

            if (Super.NavBarHeight < 0)

                Super.NavBarHeight = 50; //manual

            Super.StatusBarHeight = 0;

            VisualDiagnostics.VisualTreeChanged += OnVisualTreeChanged;

        }

    }
}

