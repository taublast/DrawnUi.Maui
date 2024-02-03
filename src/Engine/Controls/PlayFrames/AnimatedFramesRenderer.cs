namespace DrawnUi.Maui.Controls;

/// <summary>
///     Base class for playing frames. Subclass to play spritesheets, gifs, custom animations etc.
/// </summary>
public class AnimatedFramesRenderer : SkiaControl
{
    protected RangeAnimator Animator;

    protected bool PlayWhenAvailable;

    private bool wasLayout;

    protected bool WasPlayingBeforeVisibilityChanged;

    protected bool WasStarted;

    protected bool CanPlay => wasLayout && CheckCanStartAnimator();


    public bool IsPlaying
    {
        get
        {
            if (Animator != null) return Animator.IsRunning;
            return false;
        }
    }

    public override void OnVisibilityChanged(bool newvalue)
    {
        if (newvalue)
        {
            if (WasPlayingBeforeVisibilityChanged || PlayWhenAvailable) Start(); //resume
        }
        else
        {
            if (IsPlaying)
            {
                WasPlayingBeforeVisibilityChanged = true;
                Stop(); //be quiet
            }
        }

        base.OnVisibilityChanged(newvalue);
    }

    protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
    {
        base.Paint(ctx, destination, scale, arguments);

        RenderFrame(ctx, destination, scale, arguments);

        if (IsPlaying)
            Update();
    }

    protected virtual void RenderFrame(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
    { }

    protected override void OnLayoutChanged()
    {
        base.OnLayoutChanged();

        wasLayout = true;

        InitializeAnimator();

        if (PlayWhenAvailable)
        {
            PlayWhenAvailable = false;
            if (Animator != null)
                if (!Animator.IsRunning)
                    Start();
        }
    }

    //protected override void OnParentVisibilityChanged(bool newvalue)
    //{
    //    base.OnParentVisibilityChanged(newvalue);

    //    if (!newvalue)
    //    {
    //        Animator?.Pause();
    //    }
    //    else
    //    {
    //        Animator?.Resume();
    //    }
    //}

    public void InitializeAnimator()
    {
        if (Animator == null)
            Animator = new RangeAnimator(this)
            {
                OnUpdated = OnAnimatorUpdated,
                OnStart = () =>
                {
                    WasStarted = true;
                    OnPropertyChanged(nameof(IsPlaying));
                    OnStarted();
                },
                OnStop = () =>
                {
                    OnPropertyChanged(nameof(IsPlaying));
                    if (WasStarted)
                        OnFinished();
                    WasStarted = false;
                }
            };

        Animator.Repeat = Repeat;

        OnAnimatorInitializing();

        if (AutoPlay && CheckCanStartAnimator()) Start();
    }

    protected virtual void OnAnimatorInitializing()
    { }

    /// <summary>
    ///     Override this to react on animator running.
    /// </summary>
    /// <param name="value"></param>
    protected virtual void OnAnimatorUpdated(double value)
    { }


    protected virtual bool CheckCanStartAnimator()
    {
        return true;
    }

    protected virtual void OnAnimatorStarting()
    { }

    protected virtual void OnAnimatorSeeking(double frame)
    { }

    public void Seek(double frame)
    {
        OnAnimatorSeeking(frame);
        Update();
    }

    public void Start(int delayMs = 0)
    {
        if (CanPlay)
        {
            OnAnimatorStarting();
            Animator.Start(delayMs);
        }
        else
        {
            PlayWhenAvailable = true;
        }
    }

    public void Stop()
    {
        Animator.Stop();
    }


    /// <summary>
    ///     ratio 0-1
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public double GetFrameAt(float ratio)
    {
        return Animator.Distance * ratio;
    }

    protected virtual void OnFinished()
    {
        Finished?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnStarted()
    {
        Started?.Invoke(this, EventArgs.Empty);
    }


    public override void OnDisposing()
    {
        if (Animator != null)
        {
            Stop();

            Animator.Dispose();
        }

        base.OnDisposing();
    }

    #region EVENTS

    /// <summary>
    ///     All repeats completed
    /// </summary>
    public event EventHandler Finished;

    public event EventHandler Started;

    #endregion


    #region PROPERTIES

    public static readonly BindableProperty RepeatProperty = BindableProperty.Create(nameof(Repeat),
        typeof(int),
        typeof(AnimatedFramesRenderer),
        0, propertyChanged: ApplyRepeatProperty
    );

    private static void ApplyRepeatProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is AnimatedFramesRenderer control)
            if (control.Animator != null)
                control.Animator.Repeat = control.Repeat;
    }

    /// <summary>
    ///     >0 how many times should repeat, if less than 0 will loop forever
    /// </summary>
    public int Repeat
    {
        get => (int)GetValue(RepeatProperty);
        set => SetValue(RepeatProperty, value);
    }


    public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create(nameof(AutoPlay),
        typeof(bool),
        typeof(AnimatedFramesRenderer),
        true);

    public bool AutoPlay
    {
        get => (bool)GetValue(AutoPlayProperty);
        set => SetValue(AutoPlayProperty, value);
    }

    #endregion
}