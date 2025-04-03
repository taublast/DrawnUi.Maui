using DrawnUi;
using DrawnUi;
using DrawnUi;

namespace DrawnUi.Demo.Views.Controls;

public class ScrollingBanner : SkiaScrollLooped
{
    public ScrollingBanner()
    {
        ScrollType = ViewportScrollType.None;
    }

    private SkiaValueAnimator _animator;

    protected override void OnParentVisibilityChanged(bool newvalue)
    {
        if (!newvalue)
        {
            StopAnimators();
        }

        base.OnParentVisibilityChanged(newvalue);
    }

    public override void OnDisposing()
    {
        StopAnimators();

        _animator?.Dispose();

        base.OnDisposing();
    }

    private void StopAnimators()
    {
        _animator?.Stop();
    }

    public static SkiaLabel DebugLabel = new()
    {
        IsOverlay = true,
        TextColor = Colors.White,
        BackgroundColor = Colors.Black,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center
    };

    protected override void OnMeasured()
    {
        base.OnMeasured();

        //calc speed pts/sec
        var scrollAmount = this.ContentSize.Units.Width + Viewport.Units.Width;
        var k = 60 / 1000.0; //pts/sec,
        var speed = scrollAmount / k; //ms

        if (_animator == null)
        {
            //_overlay = DebugLabel;

            _animator = new SkiaValueAnimator(this)
            {
                Speed = speed,
                Easing = Easing.Linear,
                Repeat = -1
            };

            var lastV = 0.0;

            _animator.OnUpdated += (double value) =>
            {
                ViewportOffsetX = (float)(this.Viewport.Units.Width - value);
            };
        }

        _animator.mValue = 0;
        _animator.mMinValue = 0;
        _animator.mMaxValue = scrollAmount;

        _animator.Start(5000);



    }

}