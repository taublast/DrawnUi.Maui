using System.Diagnostics;

namespace DrawnUi.Maui.Draw;

public class SkiaValueAnimator : AnimatorBase
{

    public override void Dispose()
    {
        this.tcs?.TrySetCanceled();
        cancellationTokenRegistration.Dispose();
        base.Dispose();
    }

    public override void Stop()
    {
        base.Stop();

        mStartValueIsSet = false;

        if (cancellationTokenRegistration.Token.IsCancellationRequested)
        {
            this.tcs?.TrySetCanceled();
        }
        cancellationTokenRegistration.Dispose();

    }

    public virtual async Task RunAsync(Action initialize, CancellationToken cancellationToken = default)
    {
        if (this.IsRunning)
        {
            this.Stop(); //without this you'd get artifacts
        }

        this.tcs = new TaskCompletionSource<bool>();

        this.cancellationTokenRegistration = cancellationToken.Register(() =>
        {
            if (this.IsRunning)
            {
                this.Stop();
            }
        });

        initialize?.Invoke();
        Start();

        await this.tcs.Task;
    }


    private TaskCompletionSource<bool> tcs;
    private CancellationTokenRegistration cancellationTokenRegistration;


    protected override void OnRunningStateChanged(bool isRunning)
    {
        if (!isRunning)
        {
            tcs?.SetResult(true);
        }

        base.OnRunningStateChanged(isRunning);
    }


    private bool lockCheck;

    public void Seek(float msTime)
    {
        //todo

    }

    public override bool TickFrame(long frameTime)
    {
        if (lockCheck)
        {
            return true;
        }
        lockCheck = true;

        try
        {
            if (!IsRunning)
                return true;

            if (mLastFrameTime == 0)
            {
                //  First frame.
                mLastFrameTime = frameTime;
                mStartFrameTime = frameTime;
            }

            long deltaFromStart = (frameTime - mStartFrameTime);
            long deltaT = (frameTime - mLastFrameTime);

            mLastFrameTime = frameTime;
            bool finished = UpdateValue(deltaT, deltaFromStart);

            var currentValue = TransformReportedValue(deltaT);
            OnUpdated?.Invoke(currentValue);

            if (finished)
            {
                if (Repeat < 0) //forever
                {
                    mValue = mMinValue;
                    mLastFrameTime = 0;
                    mStartFrameTime = 0;
                    finished = false;
                }
                else if (Repeat > 0)
                {
                    Repeat--;
                    mValue = mMinValue;
                    mLastFrameTime = 0;
                    mStartFrameTime = 0;
                    finished = false;
                }
                else
                {
                    Stop();
                }
            }

            //#if DEBUG
            //            if (finished)
            //            {
            //                System.Diagnostics.Debug.WriteLine($"[SkiaValueAnimator] Stopped at {mValue:0.000}");
            //            }
            //            else
            //            {
            //                System.Diagnostics.Debug.WriteLine($"[SkiaValueAnimator] {mValue:0.000}");
            //            }
            //#endif

            return finished;
        }
        finally
        {
            lockCheck = false;
        }

    }

    /// <summary>
    /// -1 means forever..
    /// </summary>
    public int Repeat
    {
        get
        {
            return _repeat;
        }
        set
        {
            _repeat = value;
        }
    }
    int _repeat;

    private double _mValue;
    public double mValue
    {
        get
        {
            return _mValue;
        }
        set
        {
            if (_mValue != value)
            {
                _mValue = value;
            }
            mStartValueIsSet = true;
        }
    }
    public bool mStartValueIsSet { get; protected set; }
    public double mMaxValue { get; set; } = double.MaxValue;
    public double mMinValue { get; set; } = double.MinValue;
    public Easing Easing { get; set; } = Easing.Linear;
    public double Speed { get; set; } = 0;

    /// <summary>
    /// 	/// Passed over mValue, you can change the reported passed value here
    /// </summary>
    /// <param name="deltaT"></param>
    /// <returns>modified mValue for callback consumer</returns>
    protected virtual double TransformReportedValue(long deltaT)
    {
        return mValue;
    }

    public static bool Debug = true;


    public static long GetNanoseconds()
    {
        double timestamp = Stopwatch.GetTimestamp();
        double nanoseconds = 1_000_000_000.0 * timestamp / Stopwatch.Frequency;

        return (long)nanoseconds;
    }

    /// <summary>
    /// Update mValue using time distance between rendered frames.
    /// Return true if anims is finished.
    /// </summary>
    /// <param name="deltaT"></param>
    /// <returns></returns>
    protected virtual bool UpdateValue(long deltaT, long deltaFromStart)
    {
        var elapsedMs = deltaFromStart / 1000_000.0;
        var progress = elapsedMs / Speed;
        var deltaValue = mMaxValue - mMinValue;

        var eased = Easing.Ease(progress);

        ElapsedMs = elapsedMs;
        Progress = eased;

        var value = deltaValue * progress + mMinValue;

        // When the animation hits the max/min value, consider animation done.
        bool ret = false;
        if (value < mMinValue)
        {
            mValue = mMinValue;
            ret = true;
        }
        if (value >= mMaxValue)
        {
            mValue = mMaxValue;
            ret = true;
        }
        else
        {
            mValue = value;
        }

        //OnValueUpdated?.Invoke(mValue);

        return ret;
    }

    public double ElapsedMs { get; protected set; }

    /// <summary>
    /// We are using this internally to apply easing. Can be above 1 when finishing. If you need progress 0-1 use ProgressAnimator.
    /// </summary>
    protected double Progress { get; set; }

    public Action<double> OnUpdated { get; set; }

    protected virtual void ClampOnStart()
    {
        if (mValue < mMinValue)
        {
            mValue = mMinValue;
            //throw new Exception("Starting value need to be in between min value and max value");
        }
        else
        if (mValue > mMaxValue)
        {
            mValue = mMaxValue;
            //throw new Exception("Starting value need to be in between min value and max value");
        }
    }

    public override void Start(double delayMs = 0)
    {
        if (!IsRunning)
        {
            ClampOnStart();
        }

        //System.Diagnostics.Debug.WriteLine($"[ANIM] START min {mMinValue:0.0} max {mMaxValue:0.0} cur {mValue:0.0}");


        base.Start();
    }

    #region EXTENSIONS
    public SkiaValueAnimator SetValue(double value)
    {
        mValue = value;
        return this;
    }

    public SkiaValueAnimator SetSpeed(double value)
    {
        Speed = value;
        return this;
    }
    #endregion


    public SkiaValueAnimator(IDrawnBase parent) : base(parent)
    {
    }
}