using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw;

public class SkiaLabelFps : SkiaLabel, ISkiaAnimator
{
    public SkiaLabelFps()
    {
        IsParentIndependent = true;
        WillNotUpdateParent = true;

        Tag = "FPS";
        MaxLines = 1;
        BackgroundColor = Colors.Black;
        TextColor = Colors.LimeGreen;
        InputTransparent = true;
        ZIndex = int.MaxValue;
        UpdateForceRefresh();
    }

    public bool IsDeactivated { get; set; }

    public bool IsPaused { get; set; }
    public bool IsHiddenInViewTree { get; set; }

    public virtual void Pause()
    {
        UnregisterAnimator(this.Uid);
    }

    public virtual void Resume()
    {
        RegisterAnimator(this);
    }

    public override void OnParentChanged(IDrawnBase newvalue, IDrawnBase oldvalue)
    {
        base.OnParentChanged(newvalue, oldvalue);

        UpdateForceRefresh();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        UpdateForceRefresh();
    }


    void UpdateForceRefresh()
    {
        if (ForceRefresh)
        {
            Start();
        }
        else
        {
            if (IsRunning)
                Stop();
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == ForceRefreshProperty.PropertyName)
        {
            UpdateForceRefresh();
        }
    }

    public static readonly BindableProperty ForceRefreshProperty = BindableProperty.Create(nameof(ForceRefresh),
    typeof(bool),
    typeof(SkiaLabelFps),
    false);
    public bool ForceRefresh
    {
        get { return (bool)GetValue(ForceRefreshProperty); }
        set { SetValue(ForceRefreshProperty, value); }
    }


    protected override void Draw(SkiaDrawingContext context, SKRect destination, float scale)
    {
        base.Draw(context, destination, scale);

        if (Superview == null)
            return;

        Text = $"FPS: {Superview.FPS:00.0}";

        UpdateForceRefresh();
        //if we had zero updates after 2secs just display a zero
        //if (_topParent.FPS > 1.0)
        //{
        //	_restartingTimer ??= new RestartingTimer<object>(
        //		2000,
        //		(context) =>
        //		{
        //			Task.Run(Update).ConfigureAwait(false);
        //		});
        //	_restartingTimer.Kick(null);
        //}
        FinalizeDrawingWithRenderObject(context, scale);

    }


    public bool TickFrame(long frameTime)
    {
        return false;
    }

    public bool IsRunning { get; set; }
    public bool WasStarted { get; }


    public void Stop()
    {
        IsRunning = false;
        UnregisterAnimator(this.Uid);
    }

    public void Start(double delayMs = 0)
    {
        if (!IsRunning)
        {
            IsRunning = RegisterAnimator(this);
        }
    }
}
