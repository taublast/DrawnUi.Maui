namespace DrawnUi.Maui.Draw
{

    /// <summary>
    /// Used inside RenderingTree. Rect is real drawing position
    /// </summary>
    /// <param name="Control"></param>
    /// <param name="Rect"></param>
    /// <param name="Index"></param>
    public record SkiaControlWithRect(SkiaControl Control,
        SKRect Rect,
        SKRect HitRect,
        int Index);


    public interface IRenderObject
    {
        bool UseRenderingObject(RenderDrawingContext context,
            SKRect destination,
            float scale);
    }

    public class ElementRenderer
    {
        public ElementRenderer(SkiaControl control)
        {
            Element = control;
            PaintWithOpacity = new SKPaint();
        }

        public void Dispose()
        {
            PaintWithOpacity?.Dispose();
        }

        /// <summary>
        /// Must not be used while rendering, only for createing render object
        /// </summary>
        protected readonly SkiaControl Element;

        /// <summary>
        /// Can be reused for drawing, single threaded only
        /// </summary>
        public SKPaint PaintWithOpacity { get; protected set; }

        public virtual RenderObject CreateRenderObject(
            RenderDrawingContext ctx,
            SKRect
            destination,
            float scale)
        {
            var ret = new RenderObject()
            {
                IsDistorted = Element.IsDistorted,
                WillClipBounds = Element.WillClipBounds,
                EffectPostRenderer = Element.EffectPostRenderer,
                ShouldClipAntialiased = Element.ShouldClipAntialiased
            };

            //todo add clipping

            if (Element.UsingCacheType != SkiaCacheType.None)
            {
                Element.DrawUsingRenderObject(ctx,
                    Element.SizeRequest.Width, Element.SizeRequest.Height,
                    destination, scale);

                ret.Cache = Element.RenderObject;
            }

            return ret;
        }

        public virtual bool UseRenderingObject(
            RenderObject renderMe,
            RenderDrawingContext ctx,
            SKRect destination,
            float scale)
        {
            if (renderMe.Cache != null)
            {
                if (renderMe.DelegateDrawCache != null)
                {
                    renderMe.DelegateDrawCache(renderMe.Cache, ctx, destination);
                }
                else
                {
                    DrawWithClipAndTransforms(renderMe, ctx, destination, destination, true, true,
                        (ctx) =>
                    {

                        if (renderMe.EffectPostRenderer != null)
                        {
                            renderMe.EffectPostRenderer.Render(ctx, destination);
                        }
                        else
                        {
                            ctx.PaintWithOpacity.Color = SKColors.White;
                            ctx.PaintWithOpacity.IsAntialias = true;
                            ctx.PaintWithOpacity.IsDither = renderMe.IsDistorted;
                            ctx.PaintWithOpacity.FilterQuality = SKFilterQuality.Medium;

                            renderMe.Cache.Draw(ctx.Canvas, destination, ctx.PaintWithOpacity);
                        }
                    });
                }
            }
            return false;
        }

        public void DrawWithClipAndTransforms(
            RenderObject renderMe,
            RenderDrawingContext ctx,
            SKRect destination,
            SKRect transformsArea,
            bool useOpacity,
            bool useClipping,
            Action<RenderDrawingContext> draw)
        {





        }


    }

    public class RenderTreeRenderer
    {
        public RenderTreeRenderer()
        {

        }



#if SKIA3
        public Sk3dView Helper3d;
#else
        public SK3dView Helper3d;
#endif


    }

    public class RenderDrawingContext : SkiaDrawingContext
    {
        public SKPaint PaintWithOpacity { get; protected set; }
    }

    public class RenderObject
    {

        public void Dispose()
        {
            //todo dispose cache etc?
        }

        public bool ShouldClipAntialiased { get; set; }
        public SKPath ClippingPath { get; set; }
        public CachedObject Cache { get; set; }
        public bool IsDistorted { get; set; }
        public bool WillClipBounds { get; set; }
        public IPostRendererEffect EffectPostRenderer { get; set; }

        public Action<CachedObject, SkiaDrawingContext, SKRect> DelegateDrawCache { get; set; }



        public virtual void DrawRenderObject(CachedObject cache,
            SkiaDrawingContext ctx,
            SKRect destination)
        {

        }






    }

    public class RenderLabel : RenderObject
    {
        public string Text { get; set; }
        public SKRect Rect { get; set; }
        public SKPaint Paint { get; set; }

        public void Draw()
        {
            throw new NotImplementedException();
        }
    }

}
