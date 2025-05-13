using HarfBuzzSharp;

namespace DrawnUi.Draw
{
    public class RenderedNode
    {

        /// <summary>
        /// Create this ONLY when DrawingRect and RenderTransformMatrix are ready
        /// </summary>
        /// <param name="control"></param>
        public RenderedNode(SkiaControl control, RenderedNode parent, SKRect destination, float scale)
        {
            Control = control;
            Children = new();
            Transforms = control.RenderTransformMatrix;
            TransformsTotal = Transforms;
            Destination = destination;

            if (Control is SkiaControl skiaControl)
            {
                Cached = control.UsingCacheType;
                HitBox = ScaledRect.FromPixels(destination, scale);

                var mapped = HitBox.Pixels;
                mapped.Offset((float)(skiaControl.Left * scale), (float)(skiaControl.Top * scale));

                // Calculate the mapped hit box based on combined transformations
                if (parent != null)
                {
                    SKMatrix combinedMatrix = TransformsTotal;
                    combinedMatrix = parent.TransformsTotal.PostConcat(combinedMatrix);
                    TransformsTotal = combinedMatrix;
                    HitBoxWithTransforms = ScaledRect.FromPixels(TransformRect(mapped, combinedMatrix), scale);
                }
                else
                {
                    // Root node - just use local transformation
                    HitBoxWithTransforms = ScaledRect.FromPixels(TransformRect(mapped, skiaControl.RenderTransformMatrix), scale);
                }
            }
        }

        /// <summary>
        /// Creates a deep clone of this rendered node tree and updates its positions based on new destination
        /// </summary>
        /// <param name="newDestination">The new destination rectangle</param>
        /// <param name="scale">The scale factor</param>
        /// <param name="parentNode">Optional parent rendered node for transform calculations</param>
        /// <returns>A new RenderedNode with updated positions</returns>
        public RenderedNode CloneAndUpdateTree(SKRect newDestination, float scale, RenderedNode parentNode = null)
        {
            // Clone the node
            RenderedNode clone = new RenderedNode(
                Control,
                null, // Parent will be set through parentNode parameter
                newDestination,
                scale
            );

            // Copy essential properties
            clone.Transforms = Transforms; // Local transforms stay the same

            // Calculate the delta between original and new destination
            float deltaX = newDestination.Left - Destination.Left;
            float deltaY = newDestination.Top - Destination.Top;

            // Update hit boxes
            var updatedHitBox = HitBox.Pixels;
            updatedHitBox.Offset(deltaX, deltaY);
            clone.HitBox = ScaledRect.FromPixels(updatedHitBox, scale);

            // Recalculate transforms total and hit box with transforms
            if (parentNode != null)
            {
                clone.TransformsTotal = clone.Transforms;
                clone.TransformsTotal = parentNode.TransformsTotal.PostConcat(clone.TransformsTotal);

                var mappedHitBox = clone.HitBox.Pixels;
                clone.HitBoxWithTransforms = ScaledRect.FromPixels(
                    TransformRect(mappedHitBox, clone.TransformsTotal),
                    scale);
            }
            else if (Control?.Parent is SkiaControl parent && parent.RenderedNode != null)
            {
                // Use parent's rendered node if available
                clone.TransformsTotal = clone.Transforms;
                clone.TransformsTotal = parent.RenderedNode.TransformsTotal.PostConcat(clone.TransformsTotal);

                var mappedHitBox = clone.HitBox.Pixels;
                clone.HitBoxWithTransforms = ScaledRect.FromPixels(
                    TransformRect(mappedHitBox, clone.TransformsTotal),
                    scale);
            }
            else
            {
                // Root node case
                clone.HitBoxWithTransforms = ScaledRect.FromPixels(
                    TransformRect(clone.HitBox.Pixels, clone.Transforms),
                    scale);
            }

            // Recursively process children
            clone.Children = new List<RenderedNode>();
            foreach (var child in Children)
            {
                // Calculate new child destination by applying the same delta
                var childDestination = child.Destination;
                childDestination.Offset(deltaX, deltaY);

                var childClone = child.CloneAndUpdateTree(childDestination, scale, clone);
                clone.Children.Add(childClone);
            }

            return clone;
        }

        public SkiaCacheType Cached { get; set; }
        public SKRect Destination { get; set; }
        public SKMatrix Transforms { get; protected set; }
        public SKMatrix TransformsTotal { get; protected set; }

        public SkiaControl Control { get; set; }
        public List<RenderedNode> Children { get; set; }

        /// <summary>
        /// A hitbox rather for internal use, because it is same as LastDrawnAt. For exact position on canvas use HitBoxWithTransforms.
        /// </summary>
        public ScaledRect HitBox { get; set; }

        /// <summary>
        /// Exact position on canvas use HitBoxWithTransforms, all matrix transforms and Left, Top offset applied.
        /// </summary>
        public ScaledRect HitBoxWithTransforms { get; set; }

        /// <summary>
        /// Helper to transform a rectangle using a matrix
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static SKRect TransformRect(SKRect rect, SKMatrix matrix)
        {
            // Transform all four corners of the rectangle
            SKPoint[] corners = new SKPoint[4] {
            new SKPoint(rect.Left, rect.Top),
            new SKPoint(rect.Right, rect.Top),
            new SKPoint(rect.Right, rect.Bottom),
            new SKPoint(rect.Left, rect.Bottom)
        };

            for (int i = 0; i < 4; i++)
            {
                corners[i] = matrix.MapPoint(corners[i]);
            }

            return new SKRect(
                Math.Min(Math.Min(corners[0].X, corners[1].X), Math.Min(corners[2].X, corners[3].X)),
                Math.Min(Math.Min(corners[0].Y, corners[1].Y), Math.Min(corners[2].Y, corners[3].Y)),
                Math.Max(Math.Max(corners[0].X, corners[1].X), Math.Max(corners[2].X, corners[3].X)),
                Math.Max(Math.Max(corners[0].Y, corners[1].Y), Math.Max(corners[2].Y, corners[3].Y))
            );
        }
    }
}
