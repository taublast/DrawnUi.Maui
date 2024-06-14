using AppoMobi.Maui.Gestures;

namespace Sandbox.Views.Controls;

public class TestPressEffect : SkiaShader, IStateEffect, ISkiaGestureProcessor
{
    public double Progress { get; set; }

    private RangeAnimator _animator;

    bool _initialized;
    private PointF _mouse;

    public void UpdateState()
    {
        if (Parent != null && !_initialized && Parent.IsLayoutReady)
        {
            _initialized = true;
            if (_animator == null)
            {
                _animator = new(Parent);
            }
        }

        base.Update();
    }

    public override void Attach(SkiaControl parent)
    {
        base.Attach(parent);

        UpdateState();
    }

    protected override SKRuntimeEffectUniforms CreateUniforms(SKRect destination)
    {
        var uniforms = base.CreateUniforms(destination);

        uniforms["progress"] = (float)Progress;
        uniforms["iMouse"] = new[] { _mouse.X, _mouse.Y, 0f, 0f };

        return uniforms;
    }


    public virtual ISkiaGestureListener ProcessGestures(
        SkiaGesturesParameters args,
        GestureEventProcessingInfo apply)
    {
        _mouse = args.Event.Location;

        if (args.Type == TouchActionResult.Down && _animator != null)
        {
            _animator.Start((v) =>
            {
                this.Progress = v;
                Update();
            }, 0, 1, 4500);

        }

        return null;
    }
}