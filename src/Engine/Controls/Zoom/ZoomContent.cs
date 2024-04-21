using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Draw;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw;

/// <summary>
/// Wrapper to zoom and pan content by changing the rendering scale so not affecting quality, this is not a transform.TODO add animated movement
/// </summary>
public class ZoomContent : ContentLayout, ISkiaGestureListener
{
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        Reset();
    }

    #region PROPERTIES

    public static readonly BindableProperty PanningModeProperty = BindableProperty.Create(
        nameof(PanningMode),
        typeof(PanningModeType),
        typeof(ZoomContent),
        PanningModeType.OneFinger);

    public PanningModeType PanningMode
    {
        get { return (PanningModeType)GetValue(PanningModeProperty); }
        set { SetValue(PanningModeProperty, value); }
    }

    public static readonly BindableProperty ZoomMinProperty = BindableProperty.Create(nameof(ZoomMin),
        typeof(double),
        typeof(ZoomContent),
        0.1);
    public double ZoomMin
    {
        get { return (double)GetValue(ZoomMinProperty); }
        set { SetValue(ZoomMinProperty, value); }
    }

    public static readonly BindableProperty ZoomMaxProperty = BindableProperty.Create(nameof(ZoomMax),
        typeof(double),
        typeof(ZoomContent),
        10.0);
    public double ZoomMax
    {
        get { return (double)GetValue(ZoomMaxProperty); }
        set { SetValue(ZoomMaxProperty, value); }
    }

    public static readonly BindableProperty ViewportZoomProperty = BindableProperty.Create(nameof(ViewportZoom),
        typeof(double), typeof(ZoomContent),
        1.0,
        propertyChanged: ApplyZoom);

    public double ViewportZoom
    {
        get { return (double)GetValue(ViewportZoomProperty); }
        set { SetValue(ViewportZoomProperty, value); }
    }

    public static readonly BindableProperty ZoomSpeedProperty = BindableProperty.Create(nameof(ZoomSpeed),
        typeof(double), typeof(ZoomContent),
        0.9);

    /// <summary>
    /// How much of finger movement will afect zoom change
    /// </summary>
    public double ZoomSpeed
    {
        get { return (double)GetValue(ZoomSpeedProperty); }
        set { SetValue(ZoomSpeedProperty, value); }
    }

    public static readonly BindableProperty ZoomLockedProperty = BindableProperty.Create(nameof(ZoomLocked),
        typeof(bool),
        typeof(ZoomContent),
        false);
    public bool ZoomLocked
    {
        get { return (bool)GetValue(ZoomLockedProperty); }
        set { SetValue(ZoomLockedProperty, value); }
    }

    public static readonly BindableProperty PanSpeedProperty = BindableProperty.Create(
        nameof(PanSpeed),
        typeof(double),
        typeof(ZoomContent),
        1.75);

    public double PanSpeed
    {
        get { return (double)GetValue(PanSpeedProperty); }
        set { SetValue(PanSpeedProperty, value); }
    }

    #endregion

    private static void ApplyZoom(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is ZoomContent control)
        {
            control.Content?.InvalidateWithChildren();
            control.Update();
        }
    }

    /*
    public (SKRect ScaledDestination, float Scale) ComputeContentScale(SKRect destination, SKPoint offsetCenter, float scale)
    {

        //todo apply offsetCenter but clamp, not letting beyond possible boundaries

        var useScale = scale / (float)ViewportZoom;
        var scaleDifference = scale - useScale;

        // Calculate the original center in pixels
        float centerX = destination.Left + destination.Width / 2.0f;
        float centerY = destination.Top + destination.Height / 2.0f;

        // Calculate the dimensions of the scaled rectangle
        float newWidth = destination.Width * (1 + scaleDifference);
        float newHeight = destination.Height * (1 + scaleDifference);

        // Calculate the top-left corner of the scaled rectangle, ensuring it scales around the provided center point
        float newLeft = centerX - (newWidth / 2);
        float newTop = centerY - (newHeight / 2);

        // Create the scaled rectangle
        var scaledDestination = new SKRect(newLeft, newTop, newLeft + newWidth, newTop + newHeight);

        return (scaledDestination, useScale);
    }
    */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected float ScalePixels(float value)
    {
        var useScale = RenderingScale / (float)ViewportZoom;
        var scaleDifference = RenderingScale - useScale;

        return value * (1 + scaleDifference);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected float ScalePoints(double value)
    {
        var useScale = RenderingScale / (float)ViewportZoom;

        return (float)(value * useScale);
    }

    public (SKRect ScaledDestination, float Scale) ComputeContentScale(SKRect destination, SKPoint offsetCenter, float scale)
    {
        var useScale = scale / (float)ViewportZoom;
        var scaleDifference = scale - useScale;

        var scaledOffset = new SKPoint(offsetCenter.X * (1 + scaleDifference), offsetCenter.Y * (1 + scaleDifference));

        float scaledContentWidth = Content.MeasuredSize.Units.Width * useScale;
        float scaledContentHeight = Content.MeasuredSize.Units.Height * useScale;

        // Calculate boundaries for clamping
        float maxOffsetX = Math.Max(0, (scaledContentWidth - destination.Width) / 2);
        float minOffsetX = -maxOffsetX;
        float maxOffsetY = Math.Max(0, (scaledContentHeight - destination.Height) / 2);
        float minOffsetY = -maxOffsetY;

        // Clamp offsetCenter to these boundaries
        float clampedOffsetX = Math.Clamp(scaledOffset.X, minOffsetX, maxOffsetX);
        float clampedOffsetY = Math.Clamp(scaledOffset.Y, minOffsetY, maxOffsetY);

        // Remaining scaling and positioning logic...
        float centerX = destination.Left + destination.Width / 2.0f;
        float centerY = destination.Top + destination.Height / 2.0f;

        float newWidth = destination.Width * (1 + scaleDifference);
        float newHeight = destination.Height * (1 + scaleDifference);

        float newLeft = centerX - (newWidth / 2) + clampedOffsetX;
        float newTop = centerY - (newHeight / 2) + clampedOffsetY;

        var scaledDestination = new SKRect(
            (float)Math.Round(newLeft),
            (float)Math.Round(newTop),
            (float)Math.Round(newLeft + newWidth),
            (float)Math.Round(newTop + newHeight)
            );

        return (scaledDestination, useScale);
    }



    public (SKRect ScaledDestination, float Scale) ComputeContentScale(SKRect destination, float scale, SKPoint offsetCenter)
    {
        // First, get the scaled rectangle without considering the pinch center
        var (scaledDestination, useScale) = ComputeContentScale(destination, offsetCenter, scale);

        return (scaledDestination, useScale);
    }


    protected override int DrawViews(SkiaDrawingContext context, SKRect destination, float scale, bool debug = false)
    {
        var use = ComputeContentScale(destination, scale, OffsetImage);

        var useScale = use.Scale;
        if (use.Scale < 1)
        {
            Content.Scale = 1 + ViewportZoom - scale;
            useScale = 1;
        }
        else
        {
            Content.Scale = 1;
        }

        return base.DrawViews(context, use.ScaledDestination, useScale, debug);
    }

    protected override ScaledSize MeasureContent(IEnumerable<SkiaControl> children, SKRect destination, float scale)
    {
        var use = ComputeContentScale(destination, scale, OffsetImage);

        return base.MeasureContent(children, use.ScaledDestination, use.Scale);
    }

    double _lastPinch = 0;
    double _zoom = 1;

    PointF _pinchCenter;

    bool _wasPinching;
    bool _wasPanning;

    protected SKPoint OffsetImage;

    protected PointF _panStarted;

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

                    if (PanningMode == PanningModeType.TwoFingers || PanningMode == PanningModeType.Enabled)
                    {
                        var moved = args.Pinch.Center - _pinchCenter;

                        OffsetImage = new(
                            (float)Math.Round(OffsetImage.X - Math.Round(ScalePoints(PanSpeed) * moved.Width / RenderingScale)),
                            (float)Math.Round(OffsetImage.Y - Math.Round(ScalePoints(PanSpeed) * moved.Height / RenderingScale)));
                    }

                    _lastPinch = args.Pinch.Scale;
                    _zoom += delta;

                    //Debug.WriteLine($"[ZOOM] got {args.Pinch.Scale:0.000}, delta {delta:0.00} -> {_zoom:0.00}");

                    _pinchCenter = args.Pinch.Center;

                    SetZoom(_zoom, false); //todo

                    _zoom = ViewportZoom;
                }
                else
                {
                    //attach
                    _lastPinch = args.Pinch.Scale;

                    if (!_wasPanning)
                        _pinchCenter = args.Pinch.Center;
                    else
                    {
                        var inverseOffsetX = (float)Math.Round(OffsetImage.X / ScalePoints(PanSpeed)) * RenderingScale;
                        var inverseOffsetY = (float)Math.Round(OffsetImage.Y / ScalePoints(PanSpeed)) * RenderingScale;

                        _pinchCenter = new(args.Pinch.Center.X - inverseOffsetX, args.Pinch.Center.Y - inverseOffsetY);

                    }

                    LastValue = -1;
                }
                return this;
            }
        }
        else
        if (touchAction == TouchActionResult.Panning)
        {
            if (_wasPinching && args.NumberOfTouches < 2)
            {
                _wasPinching = false;
                _wasPanning = false;
            }

            if (CompareDoubles(ViewportZoom, 1, 0.001))
            {
                return null; //let us be panned by parent control
            }

            if (PanningMode == PanningModeType.OneFinger && args.NumberOfTouches < 2 || PanningMode == PanningModeType.Enabled)
            {
                if (!_wasPanning)
                {
                    _panStarted = args.Location;
                }

                _wasPanning = true;

                var deltatX = args.Location.X - _panStarted.X;
                var deltaY = args.Location.Y - _panStarted.Y;

                _panStarted = args.Location;

                OffsetImage = new((float)Math.Round(OffsetImage.X - ScalePoints(PanSpeed) * deltatX / RenderingScale),
                    (float)Math.Round(OffsetImage.Y - ScalePoints(PanSpeed) * deltaY / RenderingScale));

                Update();

                return this;  //absorb
            }

        }
        else
        if (touchAction == TouchActionResult.Up)
        {
            if (args.NumberOfTouches < 2 && ViewportZoom == 1)
            {
                OffsetImage = SKPoint.Empty;
            }
            _wasPanning = false;
        }

        if (!_wasPinching)
        {
            return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
        }

        if (_wasPinching && touchAction == TouchActionResult.Up && args.NumberOfTouches < 2)
        {
            _lastPinch = 0;
            _wasPinching = false;

            if (ViewportZoom == 1.0)
                _wasPanning = false;
        }

        return this; //absorb
    }


    private void ClampOffsetImage(SKRect destination, float scale)
    {
        var useScale = scale / (float)ViewportZoom;

        float scaledContentWidth = Content.MeasuredSize.Units.Width * useScale;
        float scaledContentHeight = Content.MeasuredSize.Units.Height * useScale;

        // Calculate boundaries for clamping
        float maxOffsetX = Math.Max(0, (scaledContentWidth - destination.Width) / 2);
        float minOffsetX = -maxOffsetX;
        float maxOffsetY = Math.Max(0, (scaledContentHeight - destination.Height) / 2);
        float minOffsetY = -maxOffsetY;

        OffsetImage = new SKPoint(
            Math.Clamp(OffsetImage.X, minOffsetX, maxOffsetX),
            Math.Clamp(OffsetImage.Y, minOffsetY, maxOffsetY)
        );
    }

    private float GetMaxOffsetX(float scale)
    {
        float scaledContentWidth = Content.MeasuredSize.Units.Width * scale;
        return Math.Max(0, (scaledContentWidth - Content.Destination.Width) / 2);
    }

    private float GetMinOffsetX(float scale)
    {
        return -GetMaxOffsetX(scale);
    }

    private float GetMaxOffsetY(float scale)
    {
        float scaledContentHeight = Content.MeasuredSize.Units.Height * scale;
        return Math.Max(0, (scaledContentHeight - Content.Destination.Height) / 2);
    }

    private float GetMinOffsetY(float scale)
    {
        return -GetMaxOffsetY(scale);
    }


    public void Reset()
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
            if (animate)
            {
                InitializeAnimator();

                var start = LastValue;
                var end = Value;

                if (_animatorValue.IsRunning)
                {
                    _animatorValue
                        .SetSpeed(50)
                        .SetValue(end);
                }
                else
                {
                    _animatorValue.Start(
                        (value) =>
                        {
                            ViewportZoom = value;
                        },
                        start, end, 150, Easing.Linear);
                }

            }
            else
            {
                ViewportZoom = zoom;
            }
        }

        LastValue = Value;

        //Debug.WriteLine($"[ZOOM] {ViewportZoom:0.000}");
    }

    protected RangeAnimator _animatorValue;

    /// <summary>
    /// Last ViewportZoom value we are animating from
    /// </summary>
    protected double LastValue = 0;

    protected double Value = 0;

    protected void InitializeAnimator()
    {
        if (_animatorValue == null)
        {
            _animatorValue = new(this)
            {
                //OnStop = () =>
                //{
                //    //SetValue(Value);
                //}
            };
        }
    }
}