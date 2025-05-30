using HarfBuzzSharp;

namespace DrawnUi.Draw
{
    public class VisualNode
    {
        public string Tag { get; set; }
        public SkiaControl Control { get; set; }
        public List<VisualNode> Children { get; set; }
        public CachedObject Cache { get; set; }
        public SkiaCacheType Cached { get; set; }
        public SKRect Destination { get; set; }
        public SKMatrix Transforms { get; protected set; }
        public SKMatrix TransformsTotal { get; protected set; }
        public double OpacityTotal { get; set; }
        public double RotationTotal { get; set; }
        public SKPoint ScaleTotal { get; set; }
        public SKPoint TranslationTotal { get; set; }

        /// <summary>
        /// A hitbox rather for internal use, because it is same as LastDrawnAt. For exact position on canvas use HitBoxWithTransforms.
        /// </summary>
        public ScaledRect HitBox { get; set; }

        /// <summary>
        /// Exact position on canvas use HitBoxWithTransforms, all matrix transforms and Left, Top offset applied.
        /// </summary>
        public ScaledRect HitBoxWithTransforms { get; set; }

        public void Render(DrawingContext context)
        {
            //todo draw cache or direct
            if (Control == null)
            {
                throw new ApplicationException("VisualNode SkiaControl is not set");
            }

            if (Cache != null)
            {
                Control.DrawRenderObjectInternal(context.WithDestination(Cache.RecordingArea), Cache);
            }
            else
            {
                Control.DrawDirectInternal(context, Destination);
            }
        }

        #region VisualTransforms

        public static void DecomposeMatrix(SKMatrix m, out SKPoint scale, out float rotation, out SKPoint translation)
        {
            // Extract translation
            translation = new SKPoint(m.TransX, m.TransY);

            // Extract scale
            float scaleX = (float)Math.Sqrt(m.ScaleX * m.ScaleX + m.SkewY * m.SkewY);
            float scaleY = (float)Math.Sqrt(m.ScaleY * m.ScaleY + m.SkewX * m.SkewX);
            scale = new SKPoint(scaleX, scaleY);

            // Extract rotation (in radians)
            float rotationRad = (float)Math.Atan2(m.SkewY, m.ScaleX);
            rotation = rotationRad * (180f / (float)Math.PI); 
        }

        #endregion

        /// <summary>
        /// Create this ONLY when DrawingRect and RenderTransformMatrix are ready
        /// </summary>
        /// <param name="control"></param>
        /// <param name="parent"></param>
        /// <param name="destination"></param>
        /// <param name="scale"></param>
        /// <param name="tag"></param>
        public VisualNode(SkiaControl control, VisualNode parent, SKRect destination, float scale, string tag)
        {
            Tag = tag;
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

                    DecomposeMatrix(TransformsTotal, out var combinedScale, out var rotation, out var translation);
                    ScaleTotal = combinedScale;
                    RotationTotal = rotation;
                    TranslationTotal = translation;
                    OpacityTotal = (parent?.OpacityTotal ?? 1f) * (Control?.Opacity ?? 1f);
                }
                else
                {
                    // Root node - just use local transformation
                    OpacityTotal = (Control?.Opacity ?? 1f);
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
        /// <returns>A new VisualNode with updated positions</returns>
        public VisualNode CloneAndUpdateTree(SKRect newDestination, float scale, VisualNode parentNode = null)
        {
            // Clone the node
            VisualNode clone = new VisualNode(
                Control,
                null, // Parent will be set through parentNode parameter
                newDestination,
                scale, this.Tag
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
            else if (Control?.Parent is SkiaControl parent && parent.VisualNode != null)
            {
                // Use parent's rendered node if available
                clone.TransformsTotal = clone.Transforms;
                clone.TransformsTotal = parent.VisualNode.TransformsTotal.PostConcat(clone.TransformsTotal);

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
            clone.Children = new List<VisualNode>();
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
