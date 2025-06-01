using System.Collections.Immutable;
using HarfBuzzSharp;

namespace DrawnUi.Draw
{
    public class VisualLayerDraft
    {
        /// <summary>
        /// Identifier for debugging and lookup operations
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Child of a cached object
        /// </summary>
        public bool IsFrozen { get; set; }

        /// <summary>
        /// The SkiaControl this layer represents
        /// </summary>
        public SkiaControl Control { get; set; }

        /// <summary>
        /// Child layers contained within this layer
        /// </summary>
        public List<VisualLayerDraft> Children { get; set; }

        /// <summary>
        /// Cached rendering object, null means render control directly
        /// </summary>
        public CachedObject Cache { get; set; }

        /// <summary>
        /// Type of caching applied to this layer
        /// </summary>
        public SkiaCacheType Cached { get; set; }

        /// <summary>
        /// Layout bounds in local coordinates
        /// </summary>
        public SKRect Destination { get; set; }

        /// <summary>
        /// Layout bounds in local coordinates at time of creation
        /// </summary>
        public SKRect Origin { get; }

        /// <summary>
        /// Local transformation matrix applied to this layer
        /// </summary>
        public SKMatrix Transforms { get; protected set; }

        /// <summary>
        /// Combined transformation matrix including all parent transforms
        /// </summary>
        public SKMatrix TransformsTotal { get; protected set; }

        /// <summary>
        /// Combined opacity including all parent opacities
        /// </summary>
        public double OpacityTotal { get; set; }

        /// <summary>
        /// Total rotation in degrees extracted from transform matrix
        /// </summary>
        public double RotationTotal { get; set; }

        /// <summary>
        /// Total scale factors extracted from transform matrix
        /// </summary>
        public SKPoint ScaleTotal { get; set; }

        /// <summary>
        /// Total translation extracted from transform matrix
        /// </summary>
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

        /// <summary>
        /// Attaches cached children and relocates them to account for new parent position
        /// </summary>
        /// <param name="cache">Cached object containing children to attach</param>
        public void AttachFromCache(CachedObject cache)
        {
            if (cache.Children == null || cache.Children.Count == 0)
                return;

            Children = cache.Children;

            // Use cache.Bounds as the original parent destination
            // Calculate position delta: where we are now vs where cache was created
            var deltaX = this.Destination.Left - cache.Bounds.Left;
            var deltaY = this.Destination.Top - cache.Bounds.Top;

            // Single recursive pass: relocate positions AND update transforms
            UpdateParentChildRelationshipsWithRelocation(this, null, true, deltaX, deltaY); // MISSING METHOD
        }

        /// <summary>
        /// Combined method: relocates positions AND updates transforms in single recursive pass
        /// </summary>
        /// <param name="node">The node to update</param>
        /// <param name="parent">Parent node for transform calculations</param>
        /// <param name="frozen">Whether children should be marked as frozen</param>
        /// <param name="deltaX">X offset to apply for relocation</param>
        /// <param name="deltaY">Y offset to apply for relocation</param>
        private void UpdateParentChildRelationshipsWithRelocation(VisualLayerDraft node, VisualLayerDraft parent, bool frozen, float deltaX, float deltaY)
        {
      
            // Step 1: Update destination from Origin + delta (like CloneAndUpdateTree logic)
            var newDestination = node.Origin;
            newDestination.Offset(deltaX, deltaY);
            node.Destination = newDestination;

            // Step 2: Update hit box from Origin + delta (same pattern as CloneAndUpdateTree)
            node.HitBox = ScaledRect.FromPixels(newDestination, node.HitBox.Scale);

            // Step 3: Update transforms (same as your existing logic)
            if (parent != null)
            {
                node.TransformsTotal = parent.TransformsTotal.PostConcat(node.Transforms);
                node.HitBoxWithTransforms = ScaledRect.FromPixels(
                    TransformRect(node.HitBox.Pixels, node.TransformsTotal),
                    node.HitBox.Scale);
                node.OpacityTotal = parent.OpacityTotal * (node.Control?.Opacity ?? 1.0);
            }
            else
            {
                node.TransformsTotal = node.Transforms;
                node.HitBoxWithTransforms = ScaledRect.FromPixels(
                    TransformRect(node.HitBox.Pixels, node.Transforms),
                    node.HitBox.Scale);
                node.OpacityTotal = node.Control?.Opacity ?? 1.0;
            }

            // Step 4: Recursively process children (same delta, like CloneAndUpdateTree)
            foreach (var child in node.Children)
            {
                child.IsFrozen = frozen;
                UpdateParentChildRelationshipsWithRelocation(child, node, frozen, deltaX, deltaY);
            }
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
        public VisualLayerDraft(SkiaControl control, VisualLayerDraft parent, SKRect destination, float scale, string tag)
        {
            Tag = tag;
            Control = control;
            Children = new();
            Transforms = control.RenderTransformMatrix;
            TransformsTotal = Transforms;
            Destination = destination;
            Origin = destination;

            if (Control is SkiaControl skiaControl)
            {
                Cached = control.UsingCacheType;
                HitBox = ScaledRect.FromPixels(destination, scale);

                // Don't add Left/Top offset if destination already includes it
                var rectToTransform = destination; // Use destination directly

                if (parent != null)
                {
                    TransformsTotal = parent.TransformsTotal.PostConcat(Transforms);
                    HitBoxWithTransforms =
                        ScaledRect.FromPixels(TransformRect(rectToTransform, TransformsTotal), scale);
                    OpacityTotal = (parent?.OpacityTotal ?? 1f) * (Control?.Opacity ?? 1f);
                }
                else
                {
                    OpacityTotal = (Control?.Opacity ?? 1f);
                    HitBoxWithTransforms = ScaledRect.FromPixels(TransformRect(rectToTransform, Transforms), scale);
                }

                DecomposeMatrix(TransformsTotal, out var combinedScale, out var rotation, out var translation);
                ScaleTotal = combinedScale;
                RotationTotal = rotation;
                TranslationTotal = translation;
            }
        }

        protected VisualLayerDraft()
        {
            Children = new();
        }

        public static VisualLayerDraft CreateEmpty()
        {
            return new VisualLayerDraft();
        }

        /*

        /// <summary>
        /// Creates a deep clone of this rendered node tree and updates its positions based on new destination
        /// </summary>
        /// <param name="newDestination">The new destination rectangle</param>
        /// <param name="scale">The scale factor</param>
        /// <param name="parentNode">Optional parent rendered node for transform calculations</param>
        /// <returns>A new VisualNode with updated positions</returns>
        public VisualLayerDraft CloneAndUpdateTree(SKRect newDestination, float scale, VisualLayerDraft parentNode = null)
        {
            // Clone the node
            VisualLayerDraft clone = new VisualLayerDraft(
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
            else if (Control?.Parent is SkiaControl parent && parent.VisualLayerPreparing != null)
            {
                // Use parent's rendered node if available
                clone.TransformsTotal = clone.Transforms;
                clone.TransformsTotal = parent.VisualLayerPreparing.TransformsTotal.PostConcat(clone.TransformsTotal);

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
            clone.Children = new List<VisualLayerDraft>();
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

        */

        /// <summary>
        /// Helper to transform a rectangle using a matrix
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static SKRect TransformRect(SKRect rect, SKMatrix matrix)
        {
            // Transform all four corners of the rectangle
            SKPoint[] corners = new SKPoint[4]
            {
                new SKPoint(rect.Left, rect.Top), new SKPoint(rect.Right, rect.Top),
                new SKPoint(rect.Right, rect.Bottom), new SKPoint(rect.Left, rect.Bottom)
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


    /// <summary>
    /// Immutable snapshot of visual data.
    /// This What was just drawn on the canvas, not what is present on the canvas,
    /// because a cached layer will contain children inside its cache and represent only one drawn layer.
    /// </summary>
    /// <param name="Tag">Identifier for debugging and lookup operations</param>
    /// <param name="ControlType">Type name of the control this layer represents</param>
    /// <param name="Cached">Type of caching applied to this layer</param>
    /// <param name="Destination">Layout bounds in local coordinates</param>
    /// <param name="Transforms">Local transformation matrix applied to this layer</param>
    /// <param name="TransformsTotal">Combined transformation matrix including all parent transforms</param>
    /// <param name="OpacityTotal">Combined opacity including all parent opacities</param>
    /// <param name="RotationTotal">Total rotation in degrees extracted from transform matrix</param>
    /// <param name="ScaleTotal">Total scale factors extracted from transform matrix</param>
    /// <param name="TranslationTotal">Total translation extracted from transform matrix</param>
    /// <param name="HitBox">Local hitbox bounds for internal use</param>
    /// <param name="HitBoxWithTransforms">Exact position on canvas with all transforms applied</param>
    /// <param name="IsFrozen">Whether this layer is a child of a cached object</param>
    /// <param name="Children">Immutable collection of child layers</param>
    public sealed record VisualLayer(
        string Tag,
        string ControlType,
        SkiaCacheType Cached,
        SKRect Destination,
        SKMatrix Transforms,
        SKMatrix TransformsTotal,
        double OpacityTotal,
        double RotationTotal,
        SKPoint ScaleTotal,
        SKPoint TranslationTotal,
        ScaledRect HitBox,
        ScaledRect HitBoxWithTransforms,
        bool IsFrozen,
        ImmutableArray<VisualLayer> Children)
    {
        /// <summary>
        /// Empty snapshot for default/null cases
        /// </summary>
        public static VisualLayer Empty { get; } = new(
            Tag: string.Empty,
            ControlType: "Empty",
            Cached: SkiaCacheType.None,
            Destination: SKRect.Empty,
            Transforms: SKMatrix.Identity,
            TransformsTotal: SKMatrix.Identity,
            OpacityTotal: 1.0,
            RotationTotal: 0.0,
            ScaleTotal: new SKPoint(1, 1),
            TranslationTotal: SKPoint.Empty,
            HitBox: default,
            HitBoxWithTransforms: default,
            IsFrozen: false,
            Children: ImmutableArray<VisualLayer>.Empty
        );

        /// <summary>
        /// Creates a snapshot from a VisualNodeRenderer
        /// </summary>
        public static VisualLayer FromRenderer(VisualLayerDraft renderer)
        {
            var childSnapshots = renderer.Children?.Select(FromRenderer).ToImmutableArray()
                ?? ImmutableArray<VisualLayer>.Empty;

            return new VisualLayer(
                Tag: renderer.Tag,
                ControlType: renderer.Control?.GetType().Name ?? "Unknown",
                Cached: renderer.Cached,
                Destination: renderer.Destination,
                Transforms: renderer.Transforms,
                TransformsTotal: renderer.TransformsTotal,
                OpacityTotal: renderer.OpacityTotal,
                RotationTotal: renderer.RotationTotal,
                ScaleTotal: renderer.ScaleTotal,
                TranslationTotal: renderer.TranslationTotal,
                HitBox: renderer.HitBox,
                HitBoxWithTransforms: renderer.HitBoxWithTransforms,
                IsFrozen: renderer.IsFrozen,
                Children: childSnapshots
            );
        }

        /// <summary>
        /// Creates snapshots for multiple VisualNodeRenderers
        /// </summary>
        public static ImmutableArray<VisualLayer> FromRenderers(IEnumerable<VisualLayerDraft> renderers)
        {
            return renderers.Select(FromRenderer).ToImmutableArray();
        }

        /// <summary>
        /// Finds a snapshot by tag in the tree
        /// </summary>
        public static VisualLayer? FindByTag(VisualLayer snapshot, string tag)
        {
            if (snapshot.Tag == tag)
                return snapshot;

            foreach (var child in snapshot.Children)
            {
                var found = FindByTag(child, tag);
                if (found != null)
                    return found;
            }

            return null;
        }

        /// <summary>
        /// Gets all snapshots at a specific depth level
        /// </summary>
        public static IEnumerable<VisualLayer> GetAtDepth(VisualLayer snapshot, int depth)
        {
            if (depth == 0)
            {
                yield return snapshot;
            }
            else if (depth > 0)
            {
                foreach (var child in snapshot.Children)
                {
                    foreach (var descendant in GetAtDepth(child, depth - 1))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        /// <summary>
        /// Finds a snapshot by tag using instance method
        /// </summary>
        public VisualLayer? FindByTag(string tag)
        {
            return FindByTag(this, tag);
        }

        /// <summary>
        /// Gets all snapshots at a specific depth level using instance method
        /// </summary>
        public IEnumerable<VisualLayer> GetAtDepth(int depth)
        {
            return GetAtDepth(this, depth);
        }

        /// <summary>
        /// Gets all child snapshots recursively
        /// </summary>
        public IEnumerable<VisualLayer> GetAllChildren()
        {
            foreach (var child in Children)
            {
                yield return child;
                foreach (var grandChild in child.GetAllChildren())
                {
                    yield return grandChild;
                }
            }
        }

        /// <summary>
        /// Counts total nodes in the tree
        /// </summary>
        public int CountNodes()
        {
            return 1 + Children.Sum(child => child.CountNodes());
        }
    }


}
