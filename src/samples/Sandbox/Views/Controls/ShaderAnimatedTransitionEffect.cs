using AppoMobi.Maui.Gestures;

namespace Sandbox.Views.Controls;

/// <summary>
/// Will animate from Parent control to Secondary then call TransitionEnded event that could make Parent invisible, dispose, whatever.
/// </summary>
public class ShaderAnimatedTransitionEffect : ShaderTransitionEffect
{

    public static readonly BindableProperty DurationMsProperty = BindableProperty.Create(nameof(DurationMsProperty),
        typeof(double),
        typeof(ShaderAnimatedTransitionEffect),
        350.0,
        propertyChanged: NeedChangeSource);
    public double DurationMs
    {
        get { return (double)GetValue(DurationMsProperty); }
        set { SetValue(DurationMsProperty, value); }
    }

    bool _initialized;

    public override void UpdateState()
    {
        if (Parent != null && !_initialized && Parent.IsLayoutReady)
        {
            _initialized = true;
            if (_animator == null)
            {
                _animator = new(Parent);

                _animator.Start((v) =>
                {
                    this.Progress = v;
                    Update();
                }, 0, 1, (uint)DurationMs);
            }
        }

        base.Update();
    }

    private PingPongAnimator _animator;
}