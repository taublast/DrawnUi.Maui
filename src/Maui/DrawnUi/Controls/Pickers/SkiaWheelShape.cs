using System.Numerics;

namespace DrawnUi.Controls;

/// <summary>
/// Custom SkiaShape that positions children in a circular arrangement around the wheel circumference.
/// Handles rotation and positioning calculations for the spinner wheel.
/// </summary>
public class SkiaWheelShape : SkiaShape, ISkiaGestureListener
{
    public SkiaWheelShape()
    {
        Type = ShapeType.Circle;
        UseCache = SkiaCacheType.Operations;
        RecyclingTemplate = RecyclingTemplate.Disabled;
    }

    #region BINDABLE PROPERTIES

    public static readonly BindableProperty WheelRotationProperty = BindableProperty.Create(
        nameof(WheelRotation),
        typeof(double),
        typeof(SkiaWheelShape),
        0.0,
        propertyChanged: NeedDraw);

    public double WheelRotation
    {
        get => (double)GetValue(WheelRotationProperty);
        set => SetValue(WheelRotationProperty, value);
    }

    public static readonly BindableProperty WheelRadiusProperty = BindableProperty.Create(
        nameof(WheelRadius),
        typeof(double),
        typeof(SkiaWheelShape),
        100.0,
        propertyChanged: NeedDraw);

    public double WheelRadius
    {
        get => (double)GetValue(WheelRadiusProperty);
        set => SetValue(WheelRadiusProperty, value);
    }

    public static readonly BindableProperty InverseVisualRotationProperty = BindableProperty.Create(
        nameof(InverseVisualRotation),
        typeof(bool),
        typeof(SkiaWheelShape),
        false,
        propertyChanged: NeedDraw);

    /// <summary>
    /// Controls the visual orientation direction. False = normal, True = inverted
    /// </summary>
    public bool InverseVisualRotation
    {
        get => (bool)GetValue(InverseVisualRotationProperty);
        set => SetValue(InverseVisualRotationProperty, value);
    }

    #endregion

    #region GESTURE HANDLING

    ScrollFlingAnimator _flingAnimator;
    bool _isUserPanning;
    double _lastPanAngle;
    double _panStartRotation;

    public override ISkiaGestureListener ProcessGestures(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
    {
        if (args.Type == TouchActionResult.Down)
        {
            _flingAnimator?.Stop();
            _isUserPanning = true;
            _panStartRotation = WheelRotation;
            _lastPanAngle = GetAngleFromPoint(args.Event.Location);
            return this;
        }

        if (args.Type == TouchActionResult.Panning && _isUserPanning)
        {
            var currentAngle = GetAngleFromPoint(args.Event.Location);
            var deltaAngle = currentAngle - _lastPanAngle;

            // Handle angle wrapping
            if (deltaAngle > 180) deltaAngle -= 360;
            if (deltaAngle < -180) deltaAngle += 360;

            WheelRotation += deltaAngle;
            _lastPanAngle = currentAngle;

            return this;
        }

        if (args.Type == TouchActionResult.Up && _isUserPanning)
        {
            _isUserPanning = false;

            // Start fling animation if there's sufficient velocity
            var velocity = args.Event.Distance.Velocity;
            var angularVelocity = GetAngularVelocity(velocity);

            if (Math.Abs(angularVelocity) > 50) // Minimum velocity threshold
            {
                StartFlingAnimation(angularVelocity);
            }
            else
            {
                SnapToNearestItem();
            }

            return null;
        }

        return base.ProcessGestures(args, apply);
    }

    double GetAngleFromPoint(PointF point)
    {
        var center = new SKPoint(DrawingRect.MidX, DrawingRect.MidY);
        var dx = point.X - center.X;
        var dy = point.Y - center.Y;
        return Math.Atan2(dy, dx) * 180.0 / Math.PI;
    }

    double GetAngularVelocity(PointF velocity)
    {
        var center = new SKPoint(DrawingRect.MidX, DrawingRect.MidY);
        var radius = Math.Min(DrawingRect.Width, DrawingRect.Height) / 2.0;
        var linearVelocity = Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y);
        return linearVelocity / radius * 180.0 / Math.PI; // Convert to degrees per second
    }

    void StartFlingAnimation(double angularVelocity)
    {
        if (_flingAnimator == null)
        {
            _flingAnimator = new ScrollFlingAnimator(this);
        }

        var deceleration = 0.95f; // Adjust for desired friction
        var threshold = 1.0f; // Stop when velocity is below this threshold

        _flingAnimator.OnUpdated = (value) =>
        {
            WheelRotation = value;
        };

        _flingAnimator.OnStop = () =>
        {
            SnapToNearestItem();
        };

        _flingAnimator.InitializeWithVelocity((float)WheelRotation, (float)angularVelocity, deceleration, threshold);
        _flingAnimator.Start();
    }

    void SnapToNearestItem()
    {
        if (Children.Count == 0)
        {
            return;
        }

        var anglePerItem = 360.0 / Children.Count;

        // Get the selection position from parent spinner
        var positionOffset = 90.0; // Default to Right
        bool inverseVisualRotation = false;
        if (Parent is SkiaSpinner spinner)
        {
            positionOffset = spinner.GetSelectionPositionOffset();
            inverseVisualRotation = spinner.InverseVisualRotation;
        }

        var adjustedRotation = (-WheelRotation + positionOffset) % 360;
        if (adjustedRotation < 0) adjustedRotation += 360;

        // When visual rotation is inverted, we need to flip the rotation calculation
        if (inverseVisualRotation)
        {
            adjustedRotation = (-adjustedRotation + positionOffset * 2) % 360;
            if (adjustedRotation < 0) adjustedRotation += 360;
        }

        var nearestIndex = Math.Round(adjustedRotation / anglePerItem);
        var targetRotation = -(nearestIndex * anglePerItem + positionOffset);

        // When visual rotation is inverted, flip the target rotation calculation
        if (inverseVisualRotation)
        {
            targetRotation = -targetRotation - positionOffset * 2;
        }

        // Animate to the nearest position
        var animator = new RangeAnimator(this);
        animator.Start(value => { WheelRotation = value; }, WheelRotation, targetRotation, 300, Easing.CubicOut);
    }

    #endregion

    #region LAYOUT AND DRAWING

    /// <summary>
    /// Renders children positioned around the wheel circumference with proper rotation
    /// </summary>
    protected override int RenderViewsList(DrawingContext context, IEnumerable<SkiaControl> skiaControls)
    {
        if (skiaControls == null)
            return 0;


        var total = ItemsSource.Count;

        List<SkiaControlWithRect> tree = new();
        var childrenList = skiaControls.ToList();

        for (int index = 0; index < total; index++)
        {
            var child = ChildrenFactory.GetViewForIndex(index);

            child.OptionalOnBeforeDrawing();
            if (child.CanDraw)
            {
                var radius = Math.Min(DrawingRect.Width, DrawingRect.Height) / 2.0;
                var textRadius = radius * 0.5;
                var anglePerItem = 360.0 / total;
                var itemAngle = index * anglePerItem - 90;
                //var angleInRadians = itemAngle * Math.PI / 180.0;
                if (InverseVisualRotation)
                {
                    itemAngle = -itemAngle;
                }

                var angleInRadians = itemAngle * Math.PI / 180.0;

                // Calculate relative to center (Translation might be center-based)
                var offsetXPixels = Math.Cos(angleInRadians) * textRadius;
                var offsetYPixels = Math.Sin(angleInRadians) * textRadius;

                // Convert pixels to units using the scale factor
                var scale = child.MeasuredSize.Scale;
                var offsetXUnits = offsetXPixels / scale;
                var offsetYUnits = offsetYPixels / scale;

                child.TranslationX = offsetXUnits;
                child.TranslationY = offsetYUnits;

                var textRotation = itemAngle;
                if (InverseVisualRotation)
                {
                    // When direction is reversed, we need to flip the text orientation
                    // so it's readable on the opposite side
                    textRotation += 180;
                }
                child.Rotation = textRotation;

                child.Clipping = (path, childRect) =>
                {
                    child.ShouldClipAntialiased = true;
                    path.Reset();
                    var halfAngle = anglePerItem / 2.0;   
                    var tanHalfAngle = Math.Tan(halfAngle * Math.PI / 180.0);
                    var centerY = childRect.MidY;
                    var halfWidth = childRect.Width * 0.5f;

                    var distanceToRightEdge = childRect.Right - halfWidth;
                    var cutDepth = (float)(childRect.Height * 0.5f - distanceToRightEdge * tanHalfAngle);

                    // Create trapezoid from center outward
                    path.MoveTo(halfWidth, centerY);
                    path.LineTo(childRect.Right, childRect.Top + cutDepth);
                    path.LineTo(childRect.Right, childRect.Bottom - cutDepth);
                    path.LineTo(halfWidth, centerY);
                    path.Close();

                    if (InverseVisualRotation)
                    {
                        // Flip the path horizontally around its center
                        var centerX = childRect.MidX;
                        var flipMatrix = SKMatrix.CreateScale(-1, 1, centerX, centerY);
                        path.Transform(flipMatrix);
                    }
                };

                child.Render(context);

                tree.Add(new SkiaControlWithRect(child,
                    context.Destination,
                    child.LastDrawnAt,
                    index));
            }
        }

        SetRenderingTree(tree);

        return total;
    }

    #endregion
}
