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
