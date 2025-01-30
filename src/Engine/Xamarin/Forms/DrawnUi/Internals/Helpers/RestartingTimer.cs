namespace DrawnUi.Maui.Models;

public class RestartingTimer<T> //where T:class 
{
    private readonly TimeSpan timespan;
    private readonly Action<T> callback;

    private CancellationTokenSource cancellation;

    public RestartingTimer(TimeSpan timespan, Action<T> callback)
    {
        this.timespan = timespan;
        this.callback = callback;
        this.cancellation = new CancellationTokenSource();
    }

    public void Start(T param)
    {
        CancellationTokenSource cts = this.cancellation; // safe copy
        Device.StartTimer(this.timespan,
            () =>
            {
                if (cts.IsCancellationRequested) return false;
                this.callback.Invoke(param);
                Stop();
                cts.Cancel();
                return false; // or true for periodic behavior
            });
    }

    public void Restart(T param)
    {
        Stop();
        Start(param);
    }

    public void Stop()
    {
        Interlocked.Exchange(ref this.cancellation, new CancellationTokenSource()).Cancel();
    }

    public void Dispose()
    {

    }
}