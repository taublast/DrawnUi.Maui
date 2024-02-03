using AppoMobi.Specials;
using DrawnUi.Maui.Draw;

namespace DrawnUi.Maui.Draw;

public class AnimatorBase : ISkiaAnimator
{
    private bool _IsPostAnimator;
    public bool IsPostAnimator
    {
        get
        {
            return _IsPostAnimator;
        }
        set
        {
            if (_IsPostAnimator != value)
            {
                _IsPostAnimator = value;
            }
        }
    }


    public AnimatorBase(IDrawnBase parent)
    {
        Parent = parent;
    }

    public static double Radians(double degrees)
    {
        return degrees * 0.0174533;
    }

    protected double runDelayMs;


    protected bool Register()
    {
        IsPaused = false;

        if (Parent != null && this.IsPostAnimator)
        {
            if (this is IOverlayEffect effect)
            {
                if (!Parent.PostAnimators.Contains(effect))
                {
                    Parent.PostAnimators.Add(effect);
                    //System.Diagnostics.Debug.WriteLine($"[AnimatorBase] Added PostAnimator {Uid}");
                }
            }
            else
                throw new Exception("Post animator must implement IOverlayEffect");
        }

        var ret = Parent?.RegisterAnimator(this);

        Parent?.Update();

        return ret.GetValueOrDefault();
    }

    protected void Unregister()
    {
        if (Parent != null && this.IsPostAnimator)
        {
            if (this is IOverlayEffect effect)
            {
                if (Parent.PostAnimators.Contains(effect))
                {
                    Parent.PostAnimators.Remove(effect);
                    //System.Diagnostics.Debug.WriteLine($"[AnimatorBase] Removed PostAnimator {Uid}");
                }
            }
            else
                throw new Exception("Post animator must implement IOverlayEffect");
        }

        Parent?.UnregisterAnimator(Uid);
    }




    private bool isStarting;

    public virtual void Start(double delayMs = 0)
    {
        if (isStarting)
            return;

        isStarting = true;

        if (delayMs > 0)
        {
            runDelayMs = delayMs;
            Tasks.StartDelayed(TimeSpan.FromMilliseconds(delayMs), () =>
            {
                Start();
            });

            isStarting = false;
            return;
        }

        if (Register())
        {
            WasStarted = true;
            if (!IsRunning)
            {
                mLastFrameTime = 0;
                mStartFrameTime = 0;
                IsRunning = true;
            }
        }

        isStarting = false;
    }

    public virtual bool TickFrame(long frameTime)
    {
        if (!IsRunning)
            return true;

        mLastFrameTime = frameTime;
        return false;
    }

    public virtual void Pause()
    {
        IsPaused = true; 
    }

    public virtual void Resume()
    {
        IsPaused = false;
    }

    public bool IsPaused { get; set; }

    public virtual void Stop()
    {
        Unregister();

        mLastFrameTime = 0;
        mStartFrameTime = 0;
        IsRunning = false;

        WasStarted = false;
    }

    public Action OnStop { get; set; }
    public Action OnStart { get; set; }

    public IDrawnBase Parent { get; protected set; }

    public bool IsDeactivated
    {
        get => _isDeactivated;
        set
        {
            _isDeactivated = value;
        }
    }

    public long mLastFrameTime { get; set; }

    public long mStartFrameTime
    {
        get;
        set;
    }

    public Guid Uid { get; set; } = Guid.NewGuid();

    public virtual void Dispose()
    {
        Stop();
        Parent = null;
    }

    protected virtual void OnRunningStateChanged(bool isRunning)
    {
        if (isRunning)
        {
            OnStart?.Invoke();
        }
        else
        {
            OnStop?.Invoke();
        }
    }

    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }

        set
        {
            if (_isRunning != value)
            {
                _isRunning = value;
                OnRunningStateChanged(value);
            }
        }
    }
    bool _isRunning;
    private bool _isDeactivated;


    public bool WasStarted { get; protected set; }
}