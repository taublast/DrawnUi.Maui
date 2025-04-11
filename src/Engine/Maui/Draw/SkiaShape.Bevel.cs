using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DrawnUi.Draw
{
    /// <summary>
    /// Extension of SkiaShape that adds bevel and emboss functionality
    /// </summary>
    public partial class SkiaShape
    {
        public static readonly BindableProperty BevelTypeProperty = BindableProperty.Create(
            nameof(BevelType), 
            typeof(BevelType), 
            typeof(SkiaShape),
            BevelType.None, 
            propertyChanged: NeedDraw);

        /// <summary>
        /// Gets or sets the type of bevel effect to apply to the shape.
        /// </summary>
        /// <remarks>
        /// - None: No bevel effect is applied
        /// - Bevel: Creates a raised effect with light on top/left and shadow on bottom/right
        /// - Emboss: Creates an inset effect with shadow on top/left and light on bottom/right
        /// 
        /// This property must be used with the Bevel property for the effect to be visible.
        /// </remarks>
        public BevelType BevelType
        {
            get { return (BevelType)GetValue(BevelTypeProperty); }
            set { SetValue(BevelTypeProperty, value); }
        }

        public static readonly BindableProperty BevelProperty = BindableProperty.Create(
            nameof(Bevel),
            typeof(SkiaBevel),
            typeof(SkiaShape),
            null,
            propertyChanged: BevelPropertyChanged);

        /// <summary>
        /// Gets or sets the bevel configuration for the shape.
        /// </summary>
        /// <remarks>
        /// This property allows configuring the appearance of the bevel or emboss effect,
        /// including depth, light color, shadow color, and opacity.
        /// 
        /// Example XAML usage:
        /// <code>
        /// &lt;draw:SkiaShape BevelType="Bevel"&gt;
        ///   &lt;draw:SkiaShape.Bevel&gt;
        ///     &lt;draw:SkiaBevel Depth="3" LightColor="White" ShadowColor="Gray" Opacity="0.7" /&gt;
        ///   &lt;/draw:SkiaShape.Bevel&gt;
        /// &lt;/draw:SkiaShape&gt;
        /// </code>
        /// 
        /// Must be used with the BevelType property set to Bevel or Emboss.
        /// </remarks>
        public SkiaBevel Bevel
        {
            get { return (SkiaBevel)GetValue(BevelProperty); }
            set { SetValue(BevelProperty, value); }
        }

        private static void BevelPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaControl skiaControl)
            {
                if (oldvalue is SkiaBevel oldBevel)
                {
                    oldBevel.Dettach();
                }

                if (newvalue is SkiaBevel newBevel)
                {
                    newBevel.Attach(skiaControl);
                }

                skiaControl.Update();
            }
        }


        /// <summary>
        /// Paints the bevel or emboss effect for the shape
        /// </summary>
        protected virtual void PaintBevelEffect(SkiaDrawingContext ctx, SKRect outRect, SKPoint[] radii, float depth)
        {
            if (BevelType == BevelType.None || Bevel == null)
                return;

            var paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = depth
            };

            // Create paths for the edges
            using SKPath topLeftPath = new();
            using SKPath bottomRightPath = new();

            // Depending on the shape type, create appropriate beveled edge paths
            switch (Type)
            {
                case ShapeType.Rectangle:
                    if (CornerRadius != default)
                    {
                        // Create beveled paths for rounded rectangles
                        CreateBevelPathsForRoundRect(outRect, radii, topLeftPath, bottomRightPath, depth);
                    }
                    else
                    {
                        // Simple rectangle bevel
                        CreateBevelPathsForRect(outRect, topLeftPath, bottomRightPath, depth);
                    }
                    break;

                case ShapeType.Circle:
                case ShapeType.Ellipse:
                    // Create beveled paths for circular shapes 
                    CreateBevelPathsForEllipse(outRect, topLeftPath, bottomRightPath, depth);
                    break;

                case ShapeType.Path:
                    if (DrawPathResized != null)
                    {
                        CreateBevelPathsForCustomPath(DrawPathResized, topLeftPath, bottomRightPath, depth);
                    }
                    break;

                case ShapeType.Polygon:
                    if (Points != null && Points.Count > 0)
                    {
                        CreateBevelPathsForPolygon(outRect, Points, topLeftPath, bottomRightPath, depth);
                    }
                    break;
            }

            // Set colors based on bevel or emboss effect
            if (BevelType == BevelType.Bevel)
            {
                // Bevel: light on top/left, shadow on bottom/right
                paint.Color = Bevel.LightColor.WithAlpha((float)Bevel.Opacity).ToSKColor();
                ctx.Canvas.DrawPath(topLeftPath, paint);
                
                paint.Color = Bevel.ShadowColor.WithAlpha((float)Bevel.Opacity).ToSKColor();
                ctx.Canvas.DrawPath(bottomRightPath, paint);
            }
            else // Emboss
            {
                // Emboss: shadow on top/left, light on bottom/right
                paint.Color = Bevel.ShadowColor.WithAlpha((float)Bevel.Opacity).ToSKColor();
                ctx.Canvas.DrawPath(topLeftPath, paint);
                
                paint.Color = Bevel.LightColor.WithAlpha((float)Bevel.Opacity).ToSKColor();
                ctx.Canvas.DrawPath(bottomRightPath, paint);
            }
        }

        /// <summary>
        /// Creates bevel effect paths for a rectangle
        /// </summary>
        private void CreateBevelPathsForRect(SKRect rect, SKPath topLeftPath, SKPath bottomRightPath, float depth)
        {
            float halfDepth = depth / 2;
            
            // Top path
            topLeftPath.MoveTo(rect.Left, rect.Top + halfDepth);
            topLeftPath.LineTo(rect.Right, rect.Top + halfDepth);
            
            // Left path
            topLeftPath.MoveTo(rect.Left + halfDepth, rect.Top);
            topLeftPath.LineTo(rect.Left + halfDepth, rect.Bottom);
            
            // Bottom path
            bottomRightPath.MoveTo(rect.Left, rect.Bottom - halfDepth);
            bottomRightPath.LineTo(rect.Right, rect.Bottom - halfDepth);
            
            // Right path
            bottomRightPath.MoveTo(rect.Right - halfDepth, rect.Top);
            bottomRightPath.LineTo(rect.Right - halfDepth, rect.Bottom);
        }

        /// <summary>
        /// Creates bevel effect paths for a rounded rectangle
        /// </summary>
        private void CreateBevelPathsForRoundRect(SKRect rect, SKPoint[] radii, SKPath topLeftPath, SKPath bottomRightPath, float depth)
        {
            // Get corner radii
            float topLeftRadius = radii[0].X;
            float topRightRadius = radii[1].X;
            float bottomRightRadius = radii[2].X;
            float bottomLeftRadius = radii[3].X;

            // Adjust rect for stroke width
            float halfDepth = depth / 2;

            // Top edge (excluding corners)
            if (topLeftRadius + topRightRadius < rect.Width)
            {
                topLeftPath.MoveTo(rect.Left + topLeftRadius, rect.Top + halfDepth);
                topLeftPath.LineTo(rect.Right - topRightRadius, rect.Top + halfDepth);
            }

            // Left edge (excluding corners)
            if (topLeftRadius + bottomLeftRadius < rect.Height)
            {
                topLeftPath.MoveTo(rect.Left + halfDepth, rect.Top + topLeftRadius);
                topLeftPath.LineTo(rect.Left + halfDepth, rect.Bottom - bottomLeftRadius);
            }

            // Draw top-left corner arc if radius is large enough
            if (topLeftRadius > halfDepth)
            {
                // Calculate the center and radius of the corner arc
                SKRect arcRect = new SKRect(
                    rect.Left + halfDepth,
                    rect.Top + halfDepth,
                    rect.Left + topLeftRadius * 2 - halfDepth,
                    rect.Top + topLeftRadius * 2 - halfDepth
                );
                topLeftPath.AddArc(arcRect, 180, 90);
            }

            // Bottom edge (excluding corners)
            if (bottomLeftRadius + bottomRightRadius < rect.Width)
            {
                bottomRightPath.MoveTo(rect.Left + bottomLeftRadius, rect.Bottom - halfDepth);
                bottomRightPath.LineTo(rect.Right - bottomRightRadius, rect.Bottom - halfDepth);
            }

            // Right edge (excluding corners)
            if (topRightRadius + bottomRightRadius < rect.Height)
            {
                bottomRightPath.MoveTo(rect.Right - halfDepth, rect.Top + topRightRadius);
                bottomRightPath.LineTo(rect.Right - halfDepth, rect.Bottom - bottomRightRadius);
            }

            // Draw top-right corner arc if radius is large enough
            if (topRightRadius > halfDepth)
            {
                SKRect arcRect = new SKRect(
                    rect.Right - topRightRadius * 2 + halfDepth,
                    rect.Top + halfDepth,
                    rect.Right - halfDepth,
                    rect.Top + topRightRadius * 2 - halfDepth
                );
                bottomRightPath.AddArc(arcRect, 270, 90);
            }

            // Draw bottom-right corner arc if radius is large enough
            if (bottomRightRadius > halfDepth)
            {
                SKRect arcRect = new SKRect(
                    rect.Right - bottomRightRadius * 2 + halfDepth,
                    rect.Bottom - bottomRightRadius * 2 + halfDepth,
                    rect.Right - halfDepth,
                    rect.Bottom - halfDepth
                );
                bottomRightPath.AddArc(arcRect, 0, 90);
            }

            // Draw bottom-left corner arc if radius is large enough
            if (bottomLeftRadius > halfDepth)
            {
                SKRect arcRect = new SKRect(
                    rect.Left + halfDepth,
                    rect.Bottom - bottomLeftRadius * 2 + halfDepth,
                    rect.Left + bottomLeftRadius * 2 - halfDepth,
                    rect.Bottom - halfDepth
                );
                topLeftPath.AddArc(arcRect, 90, 90);
            }
        }

        /// <summary>
        /// Creates bevel effect paths for an ellipse or circle
        /// </summary>
        private void CreateBevelPathsForEllipse(SKRect rect, SKPath topLeftPath, SKPath bottomRightPath, float depth)
        {
            // For ellipse/circle, we'll create arcs for the top-left and bottom-right quadrants
            float halfDepth = depth / 2;
            
            // Top-left quadrant (180 to 0 degrees)
            using (SKPath arcPath = new SKPath())
            {
                // We need to adjust the rectangle to account for the stroke width
                SKRect adjustedRect = new SKRect(
                    rect.Left + halfDepth,
                    rect.Top + halfDepth,
                    rect.Right - halfDepth,
                    rect.Bottom - halfDepth
                );
                arcPath.AddArc(adjustedRect, 180, 180);
                topLeftPath.AddPath(arcPath);
            }
            
            // Bottom-right quadrant (0 to 180 degrees)
            using (SKPath arcPath = new SKPath())
            {
                // We need to adjust the rectangle to account for the stroke width
                SKRect adjustedRect = new SKRect(
                    rect.Left + halfDepth,
                    rect.Top + halfDepth,
                    rect.Right - halfDepth,
                    rect.Bottom - halfDepth
                );
                arcPath.AddArc(adjustedRect, 0, 180);
                bottomRightPath.AddPath(arcPath);
            }
        }

        /// <summary>
        /// Creates bevel effect paths for a custom path
        /// </summary>
        private void CreateBevelPathsForCustomPath(SKPath originalPath, SKPath topLeftPath, SKPath bottomRightPath, float depth)
        {
            // This is a simplified implementation that might not work for all path types
            // A more robust implementation would analyze the path segments and determine
            // which ones face up/left vs down/right
            
            using (var pathMeasure = new SKPathMeasure(originalPath))
            {
                float pathLength = pathMeasure.Length;
                float halfPathLength = pathLength / 2;
                
                // First half of the path goes to topLeftPath
                pathMeasure.GetSegment(0, halfPathLength, topLeftPath, true);
                
                // Second half of the path goes to bottomRightPath
                pathMeasure.GetSegment(halfPathLength, pathLength, bottomRightPath, true);
            }
        }

        /// <summary>
        /// Creates bevel effect paths for a polygon
        /// </summary>
        private void CreateBevelPathsForPolygon(SKRect rect, IList<SkiaPoint> points, SKPath topLeftPath, SKPath bottomRightPath, float depth)
        {
            // This is a simplified implementation that distributes edges based on their orientation
            if (points.Count < 3)
                return;

            // Calculate center of polygon
            float centerX = rect.MidX;
            float centerY = rect.MidY;

            for (int i = 0; i < points.Count; i++)
            {
                var current = ScalePoint(points[i], rect);
                var next = ScalePoint(points[(i + 1) % points.Count], rect);

                // Calculate midpoint of the edge
                float midX = (current.X + next.X) / 2;
                float midY = (current.Y + next.Y) / 2;

                // Determine if this edge is in the top-left or bottom-right half
                // This is a simple heuristic that can be improved
                if ((midX <= centerX && midY <= centerY) || (midX <= centerX && midY >= centerY) || (midX >= centerX && midY <= centerY))
                {
                    // Top or left edges
                    topLeftPath.MoveTo(current);
                    topLeftPath.LineTo(next);
                }
                else
                {
                    // Bottom or right edges
                    bottomRightPath.MoveTo(current);
                    bottomRightPath.LineTo(next);
                }
            }
        }
    }
}
