namespace DrawnUi.Maui.Models;

public class RestartingTimer<T> //where T:class 
{
    public bool IsRunning { get; protected set; }

    public T Context { get; protected set; }
    public void Kick(T param)
    {
        if (IsRunning)
        {
            Restart(param);
        }
        else
        {
            Start(param);
        }
    }

    private readonly TimeSpan timespan;

    private readonly Action<T> callback;

    private CancellationTokenSource cancellation;

    public RestartingTimer(uint ms, Action<T> callback)
    {
        this.timespan = TimeSpan.FromMilliseconds(ms);
        this.callback = callback;
        this.cancellation = new CancellationTokenSource();
    }
    public RestartingTimer(TimeSpan timespan, Action<T> callback)
    {
        this.timespan = timespan;
        this.callback = callback;
        this.cancellation = new CancellationTokenSource();
    }



    public void Restart(T param)
    {
        Stop();
        Start(param);
    }

    public void Start(T param)
    {
        IsRunning = true;
        Context = param;

        CancellationTokenSource cts = this.cancellation; // safe copy
        Tasks.StartDelayed(this.timespan, cts.Token, async () =>
            {
                if (cts.IsCancellationRequested) return;
                this.callback.Invoke(param);
                Stop();
                cts.Cancel();
                IsRunning = false;
            });
    }

    public void Stop()
    {
        Interlocked.Exchange(ref this.cancellation, new CancellationTokenSource()).Cancel();
    }

    protected bool disposed;
    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;


    }

}