using System;

namespace DrawnUi.Maui.Draw;

public class Looper : IDisposable
{
    public Looper()
    {

    }

    public Looper(Action onFrame)
    {
        OnFrame = onFrame;
    }

    public CancellationTokenSource Cancel { get; protected set; }

    public void Start(int targetFps)
    {
        var existingCanel = Cancel;
        existingCanel?.Cancel(); ;
        Cancel = new();
        Tasks.StartDelayed(TimeSpan.FromMilliseconds(1), async () =>
        {
            SetTargetFps(targetFps);
            await StartLooperAsync(Cancel.Token);
        });
        existingCanel?.Dispose();
    }

    double targetIntervalMs;

    public void SetTargetFps(int targetFps)
    {
        targetIntervalMs = 1000.0 / (double)targetFps;
    }

    public void Stop()
    {
        Cancel?.Cancel();
        Cancel?.Dispose();
    }

    public void Dispose()
    {
        Stop();
    }

    protected async Task StartLooperAsync(CancellationToken cancellationToken)
    {
        loopStopwatch.Restart();
        long lastFrameEnd = loopStopwatch.ElapsedMilliseconds;

        while (!cancellationToken.IsCancellationRequested)
        {
            frameStopwatch.Restart();

            OnFrame?.Invoke();

            frameStopwatch.Stop();

            var frameExecutionTimeMs = frameStopwatch.Elapsed.TotalMilliseconds;
            var elapsedTimeSinceLastFrame = loopStopwatch.ElapsedMilliseconds - lastFrameEnd;
            var timeToWait = targetIntervalMs - elapsedTimeSinceLastFrame - frameExecutionTimeMs;

            if (timeToWait < 1)
            {
                timeToWait = 1;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(timeToWait));
            //Thread.Sleep(TimeSpan.FromMilliseconds(timeToWait));

            lastFrameEnd = loopStopwatch.ElapsedMilliseconds;
        }
    }

    public Action OnFrame { get; set; }

    static Stopwatch frameStopwatch = new();
    static Stopwatch loopStopwatch = new();
}