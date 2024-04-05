using DrawnUi.Maui.Draw;
using System.Windows.Input;

namespace DrawnUi.Maui.Draw;

public class SkiaHotspotZoom : SkiaHotspot
{
    double _lastPinch = 0;
    double _zoom = 1;
    PointF _pinchCenter;
    bool _wasPinching = false;
    /// <summary>
    /// Last ViewportZoom value we are animating from
    /// </summary>
    protected double LastValue = 0;
    protected double Value = 0;

    public virtual void ReportZoom()
    {
        var args = new ZoomEventArgs(center: _pinchCenter, value: ViewportZoom);
        Zoomed?.Invoke(this, args);
        CommandZoomed?.Execute(args);
    }

    public virtual void Reset()
    {
        _lastPinch = 0;
        _zoom = 1;
        _wasPinching = false;

        ViewportZoom = 1;
    }

    public void SetZoom(double zoom, bool animate)
    {
        if (zoom < ZoomMin)
            zoom = ZoomMin;
        else
        if (zoom > ZoomMax)
            zoom = ZoomMax;

        Value = zoom;

        if (LastValue != Value)
        {
            ViewportZoom = zoom;
        }

        LastValue = Value;
    }


    public override ISkiaGestureListener ProcessGestures(TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
        SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
    {

        if (touchAction == TouchActionResult.Pinched)
        {
            _wasPinching = true;

            if (!ZoomLocked)
            {
                if (_lastPinch != 0 || args.Pinch.Delta != 0)
                {
                    double delta = 0;
                    if (args.Pinch.Delta != 0)
                    {
                        delta = args.Pinch.Delta * ZoomSpeed;
                    }
                    else
                        delta = (args.Pinch.Scale - _lastPinch) * ZoomSpeed;

                    _lastPinch = args.Pinch.Scale;
                    _zoom += delta;
                    _pinchCenter = args.Pinch.Center;

                    //Debug.WriteLine($"[ZOOM] got {args.Pinch.Scale:0.000}, delta {delta:0.00} -> {_zoom:0.00}");

                    SetZoom(_zoom, false); //todo

                    _zoom = ViewportZoom;
                }
                else
                {
                    //attach
                    _lastPinch = args.Pinch.Scale;
                    LastValue = -1;
                    //Debug.WriteLine($"[ZOOM] attached to {_lastPinch:0.00}");

                }
                return this;
            }
        }

        if (!_wasPinching)
        {
            return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
        }

        if (_wasPinching && touchAction == TouchActionResult.Up && args.NumberOfTouches < 2)
        {
            _lastPinch = 0;
            _wasPinching = false;
        }

        return this; //absorb
    }

    public static readonly BindableProperty ZoomSpeedProperty = BindableProperty.Create(nameof(ZoomSpeed),
        typeof(double), typeof(SkiaHotspotZoom),
        0.9);

    /// <summary>
    /// How much of finger movement will afect zoom change
    /// </summary>
    public double ZoomSpeed
    {
        get { return (double)GetValue(ZoomSpeedProperty); }
        set { SetValue(ZoomSpeedProperty, value); }
    }

    public event EventHandler<ZoomEventArgs> Zoomed;

    public static readonly BindableProperty CommandZoomedProperty = BindableProperty.Create(nameof(CommandZoomed), typeof(ICommand),
        typeof(SkiaHotspotZoom),
        null);
    public ICommand CommandZoomed
    {
        get { return (ICommand)GetValue(CommandZoomedProperty); }
        set { SetValue(CommandZoomedProperty, value); }
    }

    private static void ApplyZoom(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaHotspotZoom control)
        {
            control.ReportZoom();
        }
    }

    public static readonly BindableProperty ZoomMinProperty = BindableProperty.Create(nameof(ZoomMin),
        typeof(double),
        typeof(SkiaHotspotZoom),
        0.1);
    public double ZoomMin
    {
        get { return (double)GetValue(ZoomMinProperty); }
        set { SetValue(ZoomMinProperty, value); }
    }

    public static readonly BindableProperty ZoomMaxProperty = BindableProperty.Create(nameof(ZoomMax),
        typeof(double),
        typeof(SkiaHotspotZoom),
        10.0);
    public double ZoomMax
    {
        get { return (double)GetValue(ZoomMaxProperty); }
        set { SetValue(ZoomMaxProperty, value); }
    }

    public static readonly BindableProperty ViewportZoomProperty = BindableProperty.Create(nameof(ViewportZoom),
        typeof(double), typeof(SkiaHotspotZoom),
        1.0,
        propertyChanged: ApplyZoom);

    public double ViewportZoom
    {
        get { return (double)GetValue(ViewportZoomProperty); }
        set { SetValue(ViewportZoomProperty, value); }
    }

    public static readonly BindableProperty ZoomLockedProperty = BindableProperty.Create(nameof(ZoomLocked),
        typeof(bool),
        typeof(SkiaHotspotZoom),
        false);
    public bool ZoomLocked
    {
        get { return (bool)GetValue(ZoomLockedProperty); }
        set { SetValue(ZoomLockedProperty, value); }
    }

}