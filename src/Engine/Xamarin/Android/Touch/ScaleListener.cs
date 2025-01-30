using Android.Content;
using Android.Views;
using AppoMobi.Maui.Gestures;
using System;

namespace AppoMobi.Maui.Gestures;

public class ScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
{

    public ScaleListener(Context context, PlatformTouchEffect.TouchListener parent)
    {
        _parent = parent;

    }

    public float ScaleLimitMin { get; set; } = 0.1f;

    public float ScaleLimitMax { get; set; } = 1000.0f;

    public float ScaleFactor { get; set; } = 1.0f;


    public override bool OnScaleBegin(ScaleGestureDetector detector)
    {
        _parent.IsPinching = true;
        return base.OnScaleBegin(detector);
    }

    public override void OnScaleEnd(ScaleGestureDetector detector)
    {
        _parent.IsPinching = false;
        base.OnScaleEnd(detector);
    }

    private readonly PlatformTouchEffect.TouchListener _parent;

    public override bool OnScale(ScaleGestureDetector scaleGestureDetector)
    {
        if (!_parent.PinchEnabled)
            return base.OnScale(scaleGestureDetector);

        //used code

        var scale = ScaleFactor * scaleGestureDetector.ScaleFactor;

        ScaleFactor = Math.Max(ScaleLimitMin, Math.Min(scale, ScaleLimitMax));

        float focusX = scaleGestureDetector.FocusX;
        float focusY = scaleGestureDetector.FocusY;

        _parent.OnScaleChanged(this, new ScaleEventArgs
        {
            Scale = ScaleFactor,
            Center = new(focusX, focusY) //pixels
        });

        return true;
    }
}