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
        // Initial timestamp
        long lastFrameTimestamp = Stopwatch.GetTimestamp();

        while (!cancellationToken.IsCancellationRequested)
        {
            var startFrameTimestamp = Stopwatch.GetTimestamp();

            OnFrame?.Invoke();

            var endFrameTimestamp = Stopwatch.GetTimestamp();
            var frameElapsedTicks = endFrameTimestamp - startFrameTimestamp;
            var frameElapsedMs = (frameElapsedTicks / (double)Stopwatch.Frequency) * 1000;
            var totalElapsedMs = ((startFrameTimestamp - lastFrameTimestamp) / (double)Stopwatch.Frequency) * 1000;
            var timeToWaitMs = targetIntervalMs - totalElapsedMs - frameElapsedMs;

            if (timeToWaitMs < 1)
            {
                timeToWaitMs = 1;
            }

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(timeToWaitMs), cancellationToken);
            }
            catch
            {
                break;
            }

            lastFrameTimestamp = Stopwatch.GetTimestamp();
        }
    }


    public Action OnFrame { get; set; }

    static Stopwatch frameStopwatch = new();
    static Stopwatch loopStopwatch = new();
}