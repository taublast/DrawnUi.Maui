using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using DrawnUi.Maui.Infrastructure.Xaml;


namespace DrawnUi.Maui.Draw
{
    /// <summary>
    /// Implements ISkiaGestureListener to pass gestures to children
    /// </summary>
    /// <summary>
/// A versatile visual element that can render various shapes such as rectangles, circles, ellipses, 
/// polygons, paths, and lines. Supports customizable fills, strokes, shadows, and gradients.
/// Can also serve as a container for child elements with content clipping.
/// </summary>
/// <remarks>
/// SkiaShape can be used to create various UI elements like buttons, cards, dividers, and custom controls.
/// It supports advanced features like corner radius, stroke customization, path data for complex shapes,
/// and shadow effects.
/// </remarks>
public partial class SkiaShape : ContentLayout
    {
        /// <summary>
        /// Propagates the binding context to shadow elements and then to child elements.
        /// </summary>
        public override void ApplyBindingContext()
        {
            foreach (var shade in Shadows)
            {
                shade.BindingContext = BindingContext;
            }

            base.ApplyBindingContext();
        }

        #region PROPERTIES

        public static readonly BindableProperty PathDataProperty = BindableProperty.Create(nameof(PathData),
            typeof(string), typeof(SkiaShape),
            null,
            propertyChanged: NeedSetType);

        private static void NeedSetType(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaShape control)
            {
                control.SetupType();
            }
        }

        /// <summary>
        /// Defines a path using SVG path data syntax when the Type property is set to ShapeType.Path.
        /// </summary>
        /// <remarks>
        /// Example syntax: "M0,0L15.825011,8.0009766 31.650999,15.997986 15.825011,23.998993 0,32 0,15.997986z"
        /// - M: MoveTo command
        /// - L: LineTo command
        /// - Z: Close path
        /// - H: Horizontal line
        /// - V: Vertical line
        /// - C: Cubic Bézier curve
        /// - S: Smooth cubic Bézier
        /// - Q: Quadratic Bézier curve
        /// - T: Smooth quadratic Bézier
        /// - A: Arc
        /// </remarks>
        public string PathData
        {
            get { return (string)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        public static new readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type),
            typeof(ShapeType), typeof(SkiaShape),
            ShapeType.Rectangle,
            propertyChanged: NeedSetType);

        /// <summary>
        /// Gets or sets the type of shape to be rendered.
        /// </summary>
        /// <remarks>
        /// Available shape types:
        /// - Rectangle: A rectangle with optional corner radius
        /// - Circle: A perfect circle
        /// - Ellipse: An oval shape that fits the bounding box
        /// - Path: A custom path defined by PathData property
        /// - Polygon: A closed shape defined by Points property
        /// - Line: A line connecting Points
        /// - Arc: A curved segment of a circle
        /// </remarks>
        public new ShapeType Type
        {
            get { return (ShapeType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }


        #region StrokeGradient

        private const string nameStrokeGradient = "StrokeGradient";

        public static readonly BindableProperty StrokeGradientProperty = BindableProperty.Create(
            nameStrokeGradient,
            typeof(SkiaGradient),
            typeof(SkiaShape),
            null,
            propertyChanged: StrokeGradientPropertyChanged);

        public SkiaGradient StrokeGradient
        {
            get { return (SkiaGradient)GetValue(StrokeGradientProperty); }
            set { SetValue(StrokeGradientProperty, value); }
        }


        private static void StrokeGradientPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaControl skiaControl)
            {
                if (oldvalue is SkiaGradient skiaGradientOld)
                {
                    skiaGradientOld.Parent = null;
                    skiaGradientOld.BindingContext = null;
                }

                if (newvalue is SkiaGradient skiaGradient)
                {
                    skiaGradient.Parent = skiaControl;
                    skiaGradient.BindingContext = skiaControl.BindingContext;
                }

                skiaControl.Update();
            }

        }


        #endregion

        public static readonly BindableProperty ClipBackgroundColorProperty = BindableProperty.Create(
            nameof(ClipBackgroundColor),
            typeof(bool),
            typeof(SkiaShape),
            false, propertyChanged: NeedDraw);

        /// <summary>
        /// When set to true, the background color will be clipped out, allowing for effects like a transparent shape with only a shadow.
        /// </summary>
        /// <remarks>
        /// This is useful when you want to display a shadow but keep the background transparent to see through the shape.
        /// For example, creating a floating effect with just an outline and shadow.
        /// </remarks>
        public bool ClipBackgroundColor
        {
            get { return (bool)GetValue(ClipBackgroundColorProperty); }
            set { SetValue(ClipBackgroundColorProperty, value); }
        }

        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(
            nameof(StrokeWidth),
            typeof(double),
            typeof(SkiaShape),
            0.0,
            propertyChanged: NeedInvalidateMeasure);

        /// <summary>
        /// Gets or sets the width of the stroke (outline) of the shape in device-independent units.
        /// </summary>
        /// <remarks>
        /// - A value of 0 means no stroke will be drawn
        /// - Stroke is rendered on top of the fill and any child elements
        /// - Can be combined with StrokeColor, StrokeCap, and StrokePath for customized outlines
        /// </remarks>
        public double StrokeWidth
        {
            get { return (double)GetValue(StrokeWidthProperty); }
            set { SetValue(StrokeWidthProperty, value); }
        }

        public static readonly BindableProperty StrokeCapProperty = BindableProperty.Create(
            nameof(StrokeCap),
            typeof(SKStrokeCap),
            typeof(SkiaShape),
            SKStrokeCap.Round,
            propertyChanged: NeedDraw);

        /// <summary>
        /// Gets or sets the style of stroke line caps and joins. Default is SKStrokeCap.Round.
        /// </summary>
        /// <remarks>
        /// Available stroke cap styles:
        /// - Round: Rounded ends and joins (default)
        /// - Butt: Flat, squared-off ends
        /// - Square: Squared-off ends that extend beyond the end point by half the stroke width
        /// 
        /// This affects how line segments connect at corners and how they end at termination points.
        /// </remarks>
        public SKStrokeCap StrokeCap
        {
            get { return (SKStrokeCap)GetValue(StrokeCapProperty); }
            set { SetValue(StrokeCapProperty, value); }
        }


        public static readonly BindableProperty LayoutChildrenProperty = BindableProperty.Create(
            nameof(LayoutChildren),
            typeof(LayoutType),
            typeof(SkiaShape),
            LayoutType.Absolute,
            propertyChanged: NeedDraw);

        /// <summary>
        /// Gets or sets the layout type to use for arranging child elements within the shape.
        /// </summary>
        /// <remarks>
        /// Available layout types:
        /// - Absolute: Child elements are positioned using explicit coordinates (default)
        /// - Column: Child elements are arranged vertically
        /// - Row: Child elements are arranged horizontally
        /// - Grid: Child elements are arranged in a grid pattern
        /// 
        /// This property allows the shape to act as a container with specific layout behavior.
        /// </remarks>
        public LayoutType LayoutChildren
        {
            get { return (LayoutType)GetValue(LayoutChildrenProperty); }
            set { SetValue(LayoutChildrenProperty, value); }
        }

        public static readonly BindableProperty StrokeBlendModeProperty = BindableProperty.Create(nameof(StrokeBlendMode),
            typeof(SKBlendMode), typeof(SkiaShape),
            SKBlendMode.SrcOver,
            propertyChanged: NeedDraw);

        /// <summary>
        /// Gets or sets the blend mode used when rendering the stroke outline.
        /// Default is SKBlendMode.SrcOver (normal alpha blending).
        /// </summary>
        /// <remarks>
        /// Blend modes control how the stroke color combines with the underlying content.
        /// Common blend modes include:
        /// - SrcOver: Normal alpha blending (default)
        /// - Multiply: Darkens the underlying content
        /// - Screen: Lightens the underlying content
        /// - Plus: Adds the colors together
        /// - Clear: Makes the underlying content transparent
        /// 
        /// This can be used for creative effects like glows, neon effects, or inverted outlines.
        /// </remarks>
        public SKBlendMode StrokeBlendMode
        {
            get { return (SKBlendMode)GetValue(StrokeBlendModeProperty); }
            set { SetValue(StrokeBlendModeProperty, value); }
        }


        #endregion

        #region SHAPES


        protected static float[] GetDashArray(double[] input, float scale)
        {
            if (input == null || input.Length == 0)
            {
                return new float[0];
            }

            float[] array = new float[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                array[i] = Convert.ToSingle(Math.Round(input[i] * scale));
            }

            return array;
        }

        public struct ShapePaintArguments
        {
            public SKRect StrokeAwareSize { get; set; }

            public SKRect StrokeAwareChildrenSize { get; set; }
        }


        #endregion

        #region RENDERiNG

        /// <summary>
        /// Configures the shape based on its Type property, particularly for Path type shapes.
        /// </summary>
        /// <remarks>
        /// This method is automatically called when the Type or PathData properties change.
        /// For Path types, it parses the SVG path data string into a SkiaSharp path object.
        /// The method ensures proper resource management by scheduling the disposal of any
        /// previously created path objects to prevent memory leaks.
        /// </remarks>
        public virtual void SetupType()
        {
            if (Type == ShapeType.Path)
            {
                var kill = DrawPath;
                if (!string.IsNullOrEmpty(PathData))
                {
                    DrawPath = SKPath.ParseSvgPathData(this.PathData);
                }
                else
                {
                    DrawPath = null;
                }

                if (kill != null)
                {
                    Tasks.StartDelayed(TimeSpan.FromSeconds(3), () =>
                    {
                        kill.Dispose();
                    });
                }
            }

            Update();
        }

        public override void Arrange(SKRect destination, float widthRequest, float heightRequest, float scale)
        {
            base.Arrange(destination, widthRequest, heightRequest, scale);

            // need to do it everytime we arrange
            CalculateSizeForStroke(DrawingRect, scale);
        }

        protected void CalculateSizeForStroke(SKRect destination, float scale)
        {
            var x = (int)Math.Round(destination.Left);
            var y = (int)Math.Round(destination.Top);

            var strokeAwareSize = new SKRect(x, y,
                (float)(x + Math.Round(destination.Width)),
                (float)(y + Math.Round(destination.Height)));

            var strokeAwareChildrenSize = strokeAwareSize;
            ContractPixelsRect(strokeAwareChildrenSize, scale, Padding);

            var willStroke = StrokeColor != TransparentColor && StrokeWidth > 0;
            float pixelsStrokeWidth = 0;
            float halfStroke = 0;

            if (willStroke)
            {
                pixelsStrokeWidth = (float)Math.Round(StrokeWidth * scale);
                halfStroke = (float)(pixelsStrokeWidth / 2.0f);

                strokeAwareSize =
                    SKRect.Inflate(strokeAwareSize, -halfStroke, -halfStroke);

                strokeAwareChildrenSize = strokeAwareSize;
            }

            MeasuredStrokeAwareSize = strokeAwareSize;
            MeasuredStrokeAwareChildrenSize = strokeAwareChildrenSize;

            //rescale the path to match container
            if (Type == ShapeType.Path)
            {
                DrawPath.GetTightBounds(out var bounds);
                using SKPath stretched = new();
                stretched.AddPath(DrawPath);

                float scaleX = strokeAwareSize.Width / (bounds.Width + halfStroke);
                float scaleY = strokeAwareSize.Height / (bounds.Height + halfStroke);
                float translateX = (strokeAwareSize.Width - (bounds.Width + halfStroke) * scaleX) / 2 -
                                   bounds.Left * scaleX;
                float translateY = (strokeAwareSize.Height - (bounds.Height + halfStroke) * scaleY) / 2 -
                                   bounds.Top * scaleY;
                SKMatrix matrix = SKMatrix.CreateIdentity();
#if SKIA3
                matrix.PreConcat(SKMatrix.CreateScale(scaleX, scaleY));
                matrix.PreConcat(SKMatrix.CreateTranslation(translateX, translateY));
#else
                SKMatrix.PreConcat(ref matrix, SKMatrix.CreateScale(scaleX, scaleY));
                SKMatrix.PreConcat(ref matrix, SKMatrix.CreateTranslation(translateX, translateY));
#endif
                stretched.Transform(matrix);
                stretched.Offset(halfStroke, halfStroke);

                DrawPathResized.Reset();
                DrawPathResized.AddPath(stretched);
            }
        }

        public SKPath DrawPathResized { get; } = new();

        public SKPath DrawPathAligned { get; } = new();

        public SKRect MeasuredStrokeAwareChildrenSize { get; protected set; }

        public SKRect MeasuredStrokeAwareSize { get; protected set; }

        public double BorderWithPixels
        {
            get
            {
                return (DrawingRect.Width - MeasuredStrokeAwareChildrenSize.Width);
            }
        }

        protected SKPaint RenderingPaint { get; set; }

        /// <summary>
        /// Parsed PathData
        /// </summary>
        protected SKPath DrawPath { get; set; } = new();

        protected SKPath DrawPathShape { get; set; } = new();

        protected SKRoundRect DrawRoundedRect { get; set; }

        SKPath ClipContentPath { get; set; } = new();

        public override void OnDisposing()
        {

            RenderingPaint?.Dispose();
            RenderingPaint = null;

            DrawPath?.Dispose();
            DrawPathResized?.Dispose();
            DrawPathAligned?.Dispose();
            DrawRoundedRect?.Dispose();
            DrawPathShape?.Dispose();
            ClipContentPath?.Dispose();

            base.OnDisposing();
        }

        public override object CreatePaintArguments()
        {
            return new ShapePaintArguments()
            {
                StrokeAwareSize = MeasuredStrokeAwareSize,
                StrokeAwareChildrenSize = MeasuredStrokeAwareChildrenSize
            };
        }

        /// <summary>
        /// Creates a clipping path based on the shape's type and properties.
        /// </summary>
        /// <param name="arguments">Optional arguments that can affect the clip path creation</param>
        /// <param name="usePosition">If true, the clip path includes the position offset</param>
        /// <param name="path">Optional existing path to modify, or null to create a new path</param>
        /// <returns>A SkiaSharp path that can be used for clipping operations</returns>
        /// <remarks>
        /// This method is used internally to:
        /// - Clip child elements to the shape's boundaries
        /// - Create masks for shadow effects
        /// - Define the boundaries for hit-testing and gesture handling
        /// 
        /// The created path accounts for stroke width and corner radius when applicable.
        /// </remarks>
        public override SKPath CreateClip(object arguments, bool usePosition, SKPath path = null)
        {
            path ??= new SKPath();

            var strokeAwareSize = MeasuredStrokeAwareSize;
            var strokeAwareChildrenSize = MeasuredStrokeAwareChildrenSize;

            if (arguments is ShapePaintArguments args)
            {
                strokeAwareSize = args.StrokeAwareSize;
                strokeAwareChildrenSize = args.StrokeAwareChildrenSize;
            }

            if (!usePosition)
            {
                var offsetToZero = new SKPoint(strokeAwareChildrenSize.Left - strokeAwareSize.Left,
                    strokeAwareChildrenSize.Top - strokeAwareSize.Top);
                strokeAwareChildrenSize = new(offsetToZero.X, offsetToZero.Y,
                    strokeAwareChildrenSize.Width + offsetToZero.X, strokeAwareChildrenSize.Height + offsetToZero.Y);
            }

            switch (Type)
            {

                case ShapeType.Path:
                    ShouldClipAntialiased = true;
                    path.AddPath(DrawPathResized);
                    break;

                case ShapeType.Circle:
                    ShouldClipAntialiased = true;
                    path.AddCircle(
                        (float)(strokeAwareChildrenSize.Left + strokeAwareChildrenSize.Width / 2.0f),
                        (float)(strokeAwareChildrenSize.Top + strokeAwareChildrenSize.Height / 2.0f),
                        (float)Math.Floor(Math.Min(strokeAwareChildrenSize.Width, strokeAwareChildrenSize.Height) /
                                   2.0f) + 0);
                    break;

                case ShapeType.Ellipse:
                    ShouldClipAntialiased = true;

                    path.AddOval(strokeAwareChildrenSize);
                    break;

                case ShapeType.Polygon:
                    if (Points != null && Points.Count > 0)
                    {
                        path.Reset();

                        var scaleX = MeasuredStrokeAwareSize.Width;
                        var scaleY = MeasuredStrokeAwareSize.Height;
                        var offsetX = usePosition ? MeasuredStrokeAwareSize.Left : 0;
                        var offsetY = usePosition ? MeasuredStrokeAwareSize.Top : 0;

                        bool first = true;
                        foreach (var skiaPoint in Points)
                        {
                            var point = new SKPoint(
                                (float)Math.Round(offsetX + skiaPoint.X * scaleX),
                                (float)Math.Round(offsetY + skiaPoint.Y * scaleY));

                            if (first)
                            {
                                path.MoveTo(point);
                                first = false;
                            }
                            else
                            {
                                path.LineTo(point);
                            }
                        }
                        path.Close();
                    }
                    break;

                case ShapeType.Rectangle:
                default:
                    if (CornerRadius != default)
                    {
                        ShouldClipAntialiased = true;

                        //path.AddRect(strokeAwareChildrenSize);

                        var scaledRadiusLeftTop = (float)(CornerRadius.TopLeft * RenderingScale);
                        var scaledRadiusRightTop = (float)(CornerRadius.TopRight * RenderingScale);
                        var scaledRadiusLeftBottom = (float)(CornerRadius.BottomLeft * RenderingScale);
                        var scaledRadiusRightBottom = (float)(CornerRadius.BottomRight * RenderingScale);
                        var rrect = new SKRoundRect(strokeAwareChildrenSize);

                        // Step 3: Calculate the inner rounded rectangle corner radii
                        float strokeWidth = (float)Math.Floor(Math.Round(StrokeWidth * RenderingScale));

                        float cornerRadiusDifference = (float)strokeWidth / 2.0f;

                        scaledRadiusLeftTop = (float)(Math.Max(scaledRadiusLeftTop - cornerRadiusDifference, 0));
                        scaledRadiusRightTop = (float)(Math.Max(scaledRadiusRightTop - cornerRadiusDifference, 0));
                        scaledRadiusLeftBottom = (float)(Math.Max(scaledRadiusLeftBottom - cornerRadiusDifference, 0));
                        scaledRadiusRightBottom = (float)(Math.Max(scaledRadiusRightBottom - cornerRadiusDifference, 0));

                        rrect.SetRectRadii(strokeAwareChildrenSize, new[]
                        {
                        new SKPoint(scaledRadiusLeftTop, scaledRadiusLeftTop),
                        new SKPoint(scaledRadiusRightTop, scaledRadiusRightTop),
                        new SKPoint(scaledRadiusRightBottom, scaledRadiusRightBottom),
                        new SKPoint(scaledRadiusLeftBottom, scaledRadiusLeftBottom),
                    });
                        path.AddRoundRect(rrect);
                        //path.AddRoundRect(strokeAwareChildrenSize, innerCornerRadius, innerCornerRadius);

                    }
                    else
                    {
                        ShouldClipAntialiased = false;
                        path.AddRect(strokeAwareChildrenSize);

                    }

                    break;
            }

            return path;
        }


        protected virtual void PaintBackground(SkiaDrawingContext ctx,
           SKRect outRect,
           SKPoint[] radii,
           float minSize,
           SKPaint paint)
        {
            paint.BlendMode = this.FillBlendMode;

            switch (Type)
            {
                case ShapeType.Rectangle:
                    if (CornerRadius != default)
                    {
                        if (StrokeWidth == 0 || StrokeColor == TransparentColor)
                        {
                            paint.IsAntialias = true;
                        }

                        if (DrawRoundedRect == null)
                            DrawRoundedRect = new();
                        DrawRoundedRect.SetRectRadii(outRect, radii);
                        ctx.Canvas.DrawRoundRect(DrawRoundedRect, RenderingPaint);
                    }
                    else
                        ctx.Canvas.DrawRect(outRect, RenderingPaint);

                    break;

                case ShapeType.Polygon:

                    if (Points != null && Points.Count > 1)
                    {
                        DrawPathShape.Reset();

                        paint.StrokeJoin = MapStrokeCapToStrokeJoin(this.StrokeCap);

                        if (SmoothPoints > 0)
                        {
                            AddSmoothPath(DrawPathShape, Points, MeasuredStrokeAwareSize, SmoothPoints, true);
                        }
                        else
                        {
                            AddStraightPath(DrawPathShape, Points, MeasuredStrokeAwareSize, true);
                        }

                        ctx.Canvas.DrawPath(DrawPathShape, paint);
                    }
                    break;

                case ShapeType.Circle:
                    if (StrokeWidth == 0 || StrokeColor == TransparentColor)
                    {
                        paint.IsAntialias = true;
                    }

                    ctx.Canvas.DrawCircle(outRect.MidX, outRect.MidY, minSize / 2.0f, RenderingPaint);
                    break;

                case ShapeType.Ellipse:
                    if (StrokeWidth == 0 || StrokeColor == TransparentColor)
                    {
                        paint.IsAntialias = true;
                    }

                    DrawPathShape.Reset();
                    DrawPathShape.AddOval(outRect);
                    ctx.Canvas.DrawPath(DrawPathShape, paint);

                    break;

                case ShapeType.Path:
                    if (StrokeWidth == 0 || StrokeColor == TransparentColor)
                    {
                        paint.IsAntialias = true;
                    }


                    ctx.Canvas.DrawPath(DrawPathAligned, paint);

                    break;

                    //case ShapeType.Arc: - has no background
            }
        }

        protected virtual SKStrokeJoin MapStrokeCapToStrokeJoin(SKStrokeCap strokeCap)
        {
            switch (strokeCap)
            {
                case SKStrokeCap.Round:
                    return SKStrokeJoin.Round;
                case SKStrokeCap.Square:
                    return SKStrokeJoin.Bevel;
                case SKStrokeCap.Butt:
                default:
                    return SKStrokeJoin.Miter;
            }
        }

        protected override void PaintWithShadows(SkiaDrawingContext ctx, Action render)
        {
            render(); //we will handle shadows by ourselves
        }

        protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
        {

            var strokeAwareSize = MeasuredStrokeAwareSize;
            var strokeAwareChildrenSize = MeasuredStrokeAwareChildrenSize;
            if (arguments is ShapePaintArguments args)
            {
                strokeAwareSize = args.StrokeAwareSize;
                strokeAwareChildrenSize = args.StrokeAwareChildrenSize;
            }

            //base.Paint(ctx, destination, scale, arguments); //for debug

            //we gonna set stroke On only when drawing the last pass
            //otherwise stroke antialiasing will not work
            var willStroke = StrokeColor != TransparentColor && StrokeWidth > 0;
            float pixelsStrokeWidth = (float)Math.Round(StrokeWidth * scale);

            RenderingPaint ??= new SKPaint()
            {
                IsAntialias = true,
            };

            RenderingPaint.IsDither = IsDistorted;

            if (BackgroundColor != null)
            {
                RenderingPaint.Color = BackgroundColor.ToSKColor();
            }
            else
            {
                RenderingPaint.Color = SKColors.Transparent;
            }

            RenderingPaint.Style = SKPaintStyle.Fill;

            var minSize = Math.Min(strokeAwareSize.Height, strokeAwareSize.Width);

            var outRect = strokeAwareSize;

            if (Type == ShapeType.Path)
            {
                DrawPathAligned.Reset();
                DrawPathAligned.AddPath(DrawPathResized);
                DrawPathAligned.Offset(outRect.Left, outRect.Top);
            }

            CornerRadius scaledRadius = new(
                (CornerRadius.TopLeft * scale),
                (CornerRadius.TopRight * scale),
                (CornerRadius.BottomLeft * scale),
                (CornerRadius.BottomRight * scale));

            var radii = new SKPoint[]
            {
            new SKPoint((float)scaledRadius.TopLeft, (float)scaledRadius.TopLeft), //LeftTop
            new SKPoint((float)scaledRadius.TopRight, (float)scaledRadius.TopRight), //RightTop
            new SKPoint((float)scaledRadius.BottomLeft, (float)scaledRadius.BottomLeft), //LeftBottom
            new SKPoint((float)scaledRadius.BottomRight, (float)scaledRadius.BottomRight), //RightBottom
            };


            void PaintStroke(SKPaint paint)
            {
                paint.BlendMode = this.StrokeBlendMode;

                SetupGradient(paint, StrokeGradient, outRect);

                //todo add shadow = GLOW to stroke!
                paint.ImageFilter = null; // kill background shadow

                if (StrokeGradient?.Opacity != 1)
                {
                    paint.Shader = null; //kill background gradient
                }

                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeCap = this.StrokeCap;
                paint.StrokeJoin = MapStrokeCapToStrokeJoin(this.StrokeCap);

                if (this.StrokePath != null && StrokePath.Length > 0)
                {
                    var array = GetDashArray(StrokePath, scale);
                    paint.PathEffect = SKPathEffect.CreateDash(array, 0);
                }
                else
                {
                    paint.PathEffect = null;
                }

                paint.StrokeWidth = pixelsStrokeWidth;
                paint.Color = StrokeColor.ToSKColor();
                paint.IsAntialias = true;

                switch (Type)
                {
                    case ShapeType.Rectangle:
                        if (CornerRadius != default)
                        {
                            if (DrawRoundedRect == null)
                                DrawRoundedRect = new();
                            DrawRoundedRect.SetRectRadii(outRect, radii);
                            ctx.Canvas.DrawRoundRect(DrawRoundedRect, paint);
                        }
                        else
                            ctx.Canvas.DrawRect(outRect, paint);
                        break;

                    case ShapeType.Circle:
                        ctx.Canvas.DrawCircle(outRect.MidX, outRect.MidY, minSize / 2.0f, paint);
                        break;

                    case ShapeType.Line:
                        if (Points != null && Points.Count > 1)
                        {
                            DrawPathShape.Reset();
                            if (SmoothPoints > 0)
                            {
                                AddSmoothPath(DrawPathShape, Points, strokeAwareSize, SmoothPoints, false);
                            }
                            else
                            {
                                AddStraightPath(DrawPathShape, Points, strokeAwareSize, false);
                            }
                            ctx.Canvas.DrawPath(DrawPathShape, paint);
                        }
                        break;

                    case ShapeType.Ellipse:
                        DrawPathShape.Reset();
                        DrawPathShape.AddOval(outRect);
                        ctx.Canvas.DrawPath(DrawPathShape, paint);
                        break;

                    case ShapeType.Arc:
                        DrawPathShape.Reset();
                        // Start & End Angle for Radial Gauge
                        var startAngle = (float)Value1;
                        var sweepAngle = (float)Value2;
                        DrawPathShape.AddArc(outRect, startAngle, sweepAngle);
                        ctx.Canvas.DrawPath(DrawPathShape, paint);
                        break;

                    case ShapeType.Path:
                        DrawPathShape.Reset();
                        DrawPathShape.AddPath(DrawPathAligned);
                        ctx.Canvas.DrawPath(DrawPathShape, paint);

                        break;

                    case ShapeType.Polygon:
                        if (Points != null && Points.Count > 1)
                        {
                            DrawPathShape.Reset();

                            var path = DrawPathShape;

                            if (SmoothPoints > 0)
                            {
                                AddSmoothPath(path, Points, strokeAwareSize, SmoothPoints, true);
                            }
                            else
                            {
                                AddStraightPath(path, Points, strokeAwareSize, true);
                            }

                            ctx.Canvas.DrawPath(path, RenderingPaint);
                        }
                        break;
                }

            }

            void PaintWithShadowsInternal(Action render)
            {

                void RenderShadow(SkiaShadow shadow)
                {
                    SetupShadow(RenderingPaint, shadow, RenderingScale);

                    if (ClipBackgroundColor)
                    {
                        ClipContentPath ??= new();
                        ClipContentPath.Reset();
                        CreateClip(arguments, true, ClipContentPath);

                        var saved = ctx.Canvas.Save();

                        ClipSmart(ctx.Canvas, ClipContentPath, SKClipOperation.Difference);
                        render();

                        ctx.Canvas.RestoreToCount(saved);
                    }
                    else
                    {
                        render();
                    }
                }

                if (PlatformShadow != null)
                {
                    RenderShadow(PlatformShadow);
                }
                else
                if (Shadows != null && Shadows.Count > 0)
                {
                    for (int index = 0; index < Shadows.Count(); index++)
                    {
                        RenderShadow(Shadows[index]);
                    }
                }
                else
                {
                    render();
                }
            }

            //background with shadows pass, no stroke
            PaintWithShadowsInternal(() =>
            {
                if (SetupBackgroundPaint(RenderingPaint, outRect))
                {
                    PaintBackground(ctx, outRect, radii, minSize, RenderingPaint);
                }
            });

            //draw children views clipped with shape
            ClipContentPath ??= new();
            ClipContentPath.Reset();

            CreateClip(arguments, true, ClipContentPath);

            var saved = ctx.Canvas.Save();

            ClipSmart(ctx.Canvas, ClipContentPath);

            var rectForChildren = ContractPixelsRect(strokeAwareChildrenSize, scale, Padding);

            DrawViews(ctx, rectForChildren, scale);

            ctx.Canvas.RestoreToCount(saved);

            //last pass for stroke over background or children
            if (willStroke)
            {
                PaintStroke(RenderingPaint);
            }

        }

        #endregion

        #region SHADOWS

        private static void ShadowsPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaShape control)
            {

                var enumerableShadows = (IEnumerable<SkiaShadow>)newvalue;

                if (oldvalue != null)
                {
                    if (oldvalue is INotifyCollectionChanged oldCollection)
                    {
                        oldCollection.CollectionChanged -= control.OnShadowCollectionChanged;
                    }

                    if (oldvalue is IEnumerable<SkiaShadow> oldList)
                    {
                        foreach (var shade in oldList)
                        {
                            shade.Dettach();
                        }
                    }
                }

                foreach (var shade in enumerableShadows)
                {
                    shade.Attach(control);
                }

                if (newvalue is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged -= control.OnShadowCollectionChanged;
                    newCollection.CollectionChanged += control.OnShadowCollectionChanged;
                }

                control.Update();
            }

        }

        private void OnShadowCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (SkiaShadow newSkiaPropertyShadow in e.NewItems)
                    {
                        newSkiaPropertyShadow.Attach(this);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Remove:
                    foreach (SkiaShadow oldSkiaPropertyShadow in e.OldItems ?? new SkiaShadow[0])
                    {
                        oldSkiaPropertyShadow.Dettach();
                    }

                    break;
            }

            Update();
        }

        public static readonly BindableProperty ShadowsProperty = BindableProperty.Create(
            nameof(Shadows),
            typeof(IList<SkiaShadow>),
            typeof(SkiaShape),
            defaultValueCreator: (instance) =>
            {
                var created = new SkiaShadowsCollection();
                ShadowsPropertyChanged(instance, null, created);
                return created;
            },
            validateValue: (bo, v) => v is IList<SkiaShadow>,
            propertyChanged: NeedDraw,
            coerceValue: CoerceShadows);

        private static int instanceCount = 0;

        /// <summary>
        /// Gets or sets a collection of shadow effects to be applied to the shape.
        /// </summary>
        /// <remarks>
        /// - Multiple shadows can be applied to create complex effects
        /// - Each shadow can have different color, offset, blur, and opacity settings
        /// - Shadows are rendered beneath the shape and its content
        /// - For performance reasons, use shadows sparingly, especially on complex shapes
        /// - Example: Adding a drop shadow in XAML:
        ///   &lt;draw:SkiaShape.Shadows&gt;
        ///     &lt;draw:SkiaShadow X="3" Y="3" Blur="5" Color="Gray" Opacity="0.5" /&gt;
        ///   &lt;/draw:SkiaShape.Shadows&gt;
        /// </remarks>
        public IList<SkiaShadow> Shadows
        {
            get => (IList<SkiaShadow>)GetValue(ShadowsProperty);
            set => SetValue(ShadowsProperty, value);
        }

        private static object CoerceShadows(BindableObject bindable, object value)
        {
            if (!(value is ReadOnlyCollection<SkiaShadow> readonlyCollection))
            {
                return value;
            }

            return new ReadOnlyCollection<SkiaShadow>(
                readonlyCollection.ToList());
        }


        #endregion

        #region POINTS

        private static object CoercePoints(BindableObject bindable, object value)
        {
            if (value is ReadOnlyCollection<SkiaPoint> readonlyCollection)
            {
                return new ReadOnlyCollection<SkiaPoint>(readonlyCollection.ToList());
            }

            return value;
        }


        public static readonly BindableProperty PointsProperty = BindableProperty.Create(
            nameof(Points),
            typeof(IList<SkiaPoint>),
            typeof(SkiaShape),
            defaultValueCreator: (instance) =>
            {
                var created = new ObservableCollection<SkiaPoint>();
                PointsPropertyChanged(instance, null, created);
                return created;
            },
            validateValue: (bo, v) => v is IList<SkiaPoint>,
            propertyChanged: NeedDraw,
            coerceValue: CoercePoints);

        /// <summary>
        /// Gets or sets the collection of points used to define the shape when Type is Polygon or Line.
        /// </summary>
        /// <remarks>
        /// - For Polygon, points define the vertices of a closed shape
        /// - For Line, points define the vertices of an open path
        /// - Points use relative coordinates from 0.0 to 1.0, representing position within the shape's bounds
        /// - Can be defined in XAML using a comma-separated list of x,y pairs
        /// - Example: "0.5,0 1,0.5 0.5,1 0,0.5" defines a diamond shape
        /// </remarks>
        [TypeConverter(typeof(SkiaPointCollectionConverter))]
        public IList<SkiaPoint> Points
        {
            get => (IList<SkiaPoint>)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        public static List<SkiaPoint> PolygonStar
        {
            get
            {
                return CreateStarPoints(5);
            }
        }

        private static void PointsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SkiaShape control)
            {
                if (oldValue is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= control.OnPointsCollectionChanged;
                }

                if (oldValue is IEnumerable<SkiaPoint> oldPoints)
                {
                    foreach (var point in oldPoints)
                    {
                        point.ParentShape = null;
                    }
                }

                if (newValue is IEnumerable<SkiaPoint> newPoints)
                {
                    foreach (var point in newPoints)
                    {
                        point.ParentShape = control;
                    }
                }

                if (newValue is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged -= control.OnPointsCollectionChanged;
                    newCollection.CollectionChanged += control.OnPointsCollectionChanged;
                }

                control.Update();
            }
        }

        private void OnPointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (SkiaPoint oldPoint in e.OldItems)
                {
                    oldPoint.ParentShape = null;
                }
            }

            if (e.NewItems != null)
            {
                foreach (SkiaPoint newPoint in e.NewItems)
                {
                    newPoint.ParentShape = this;
                }
            }

            Update();
        }



        #endregion

        #region SMOOTH

        public static readonly BindableProperty SmoothPointsProperty = BindableProperty.Create(
            nameof(SmoothPoints),
            typeof(float),
            typeof(SkiaShape),
            0f, // Default value is 0 (no smoothing)
            propertyChanged: NeedDraw,
            coerceValue: (bindable, value) =>
            {
                float val = (float)value;
                return Math.Max(0f, Math.Min(1f, val)); // Clamp between 0 and 1
            }
        );

        /// <summary>
        /// Controls the automatic smoothness between points of Line and Polygon. Ranges from 0.0 (no smoothing) to 1.0.
        /// Works for sequential points only.
        /// </summary>
        public float SmoothPoints
        {
            get => (float)GetValue(SmoothPointsProperty);
            set => SetValue(SmoothPointsProperty, value);
        }

        private void AddSmoothPath(SKPath path, IList<SkiaPoint> points, SKRect rect, float smoothness, bool isClosed)
        {
            if (points == null || points.Count < 2)
            {
                return;
            }

            var scaledPoints = points.Select(p => ScalePoint(p, rect)).ToList();

            if (isClosed)
            {
                var firstPoint = scaledPoints[0];
                var closingPoint = new SKPoint(
                    (firstPoint.X + scaledPoints[1].X) / 2,
                    (firstPoint.Y + scaledPoints[1].Y) / 2);

                scaledPoints.RemoveAt(0);
                scaledPoints.Insert(0, closingPoint);
                scaledPoints.Add(firstPoint);
            }

            path.MoveTo(scaledPoints[0]);

            int pointCount = scaledPoints.Count;

            if (isClosed)
            {
                for (int i = 0; i < pointCount; i++)
                {
                    SmoothPoint(path, scaledPoints, i, smoothness, isClosed);
                }
                path.Close();
            }
            else
            {
                for (int i = 0; i < pointCount; i++)
                {
                    if (i == 0 || i == pointCount - 1)
                    {
                        path.LineTo(scaledPoints[i]);
                    }
                    else
                    {
                        SmoothPoint(path, scaledPoints, i, smoothness, isClosed);
                    }
                }
            }
        }

        private void SmoothPoint(SKPath path, IList<SKPoint> scaledPoints, int i, float smoothness, bool isClosed)
        {
            int pointCount = scaledPoints.Count;

            var prev = scaledPoints[(i - 1 + pointCount) % pointCount];
            var current = scaledPoints[i];
            var next = scaledPoints[(i + 1) % pointCount];

            if (!isClosed)
            {
                if (i == 0)
                {
                    path.LineTo(current);
                    return;
                }
                if (i == pointCount - 1)
                {
                    path.LineTo(current);
                    return;
                }
            }

            var v1 = new SKPoint(current.X - prev.X, current.Y - prev.Y);
            var v2 = new SKPoint(next.X - current.X, next.Y - current.Y);

            float lengthV1 = (float)Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
            float lengthV2 = (float)Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);

            if (lengthV1 == 0 || lengthV2 == 0)
            {
                path.LineTo(current);
                return;
            }

            v1 = new SKPoint(v1.X / lengthV1, v1.Y / lengthV1);
            v2 = new SKPoint(v2.X / lengthV2, v2.Y / lengthV2);

            float smoothingRadius = lengthV1 * smoothness * 0.3f;
            smoothingRadius = Math.Min(smoothingRadius, lengthV1 * 0.5f);
            smoothingRadius = Math.Min(smoothingRadius, lengthV2 * 0.5f);

            if (smoothingRadius < 0.001f)
            {
                path.LineTo(current); // No significant smoothing is needed
                return;
            }

            var p1 = new SKPoint(
                current.X - v1.X * smoothingRadius,
                current.Y - v1.Y * smoothingRadius);

            var p2 = new SKPoint(
                current.X + v2.X * smoothingRadius,
                current.Y + v2.Y * smoothingRadius);

            path.LineTo(p1);

            // quadratic Bezier curve to smooth  angle
            path.QuadTo(current, p2);
        }

        private void AddStraightPath(SKPath path, IList<SkiaPoint> points, SKRect rect, bool isClosed)
        {
            if (points == null || points.Count < 2)
            {
                return;
            }

            path.MoveTo(ScalePoint(points[0], rect));

            for (int i = 1; i < points.Count; i++)
            {
                path.LineTo(ScalePoint(points[i], rect));
            }

            if (isClosed)
            {
                path.Close();
            }
        }

        private SKPoint ScalePoint(SkiaPoint point, SKRect rect)
        {
            return new SKPoint(
                (float)Math.Round(rect.Left + point.X * rect.Width),
                (float)Math.Round(rect.Top + point.Y * rect.Height));
        }

        #endregion

        public static List<SkiaPoint> CreateStarPoints(int numberOfPoints, double innerRadiusRatio = 0.5)
        {
            if (numberOfPoints < 2)
                throw new ArgumentException("Number of points must be at least 2.", nameof(numberOfPoints));
            if (innerRadiusRatio <= 0 || innerRadiusRatio >= 1)
                throw new ArgumentException("Inner radius ratio must be between 0 and 1.", nameof(innerRadiusRatio));

            List<SkiaPoint> points = new List<SkiaPoint>();
            double angleStep = Math.PI / numberOfPoints;
            double outerRadius = 1.0;
            double innerRadius = outerRadius * innerRadiusRatio;

            for (int i = 0; i < numberOfPoints * 2; i++)
            {
                double angle = i * angleStep - Math.PI / 2;

                double radius = (i % 2 == 0) ? outerRadius : innerRadius;
                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);

                points.Add(new SkiaPoint((float)x, (float)y));
            }

            // Find bounding box
            var minX = points.Min(p => p.X);
            var maxX = points.Max(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxY = points.Max(p => p.Y);

            var scaleX = 1.0f / (maxX - minX);
            var scaleY = 1.0f / (maxY - minY);

            var scale = Math.Min(scaleX, scaleY);

            var offsetX = (1.0f - (maxX - minX) * scale) / 2.0f - minX * scale;
            var offsetY = (1.0f - (maxY - minY) * scale) / 2.0f - minY * scale;

            for (int i = 0; i < points.Count; i++)
            {
                var x = points[i].X * scale + offsetX;
                var y = points[i].Y * scale + offsetY;

                points[i] = new SkiaPoint(x, y);
            }

            return points;
        }

        public static List<SkiaPoint> CreateStarPointsCrossed(int numberOfPoints)
        {
            if (numberOfPoints < 5 || numberOfPoints % 2 == 0)
                throw new ArgumentException("Number of points must be an odd number greater than or equal to 5.", nameof(numberOfPoints));

            List<SkiaPoint> points = new List<SkiaPoint>();
            double angleStep = 2 * Math.PI / numberOfPoints;
            double radius = 1.0;

            List<SkiaPoint> outerPoints = new List<SkiaPoint>();
            for (int i = 0; i < numberOfPoints; i++)
            {
                double angle = i * angleStep - Math.PI / 2;
                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);
                outerPoints.Add(new SkiaPoint((float)x, (float)y));
            }

            int skip = (numberOfPoints - 1) / 2;
            for (int i = 0; i < numberOfPoints; i++)
            {
                points.Add(outerPoints[(i * skip) % numberOfPoints]);
            }

            points.Add(points[0]);

            // Find bounding box
            var minX = points.Min(p => p.X);
            var maxX = points.Max(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxY = points.Max(p => p.Y);

            var scaleX = 1.0f / (maxX - minX);
            var scaleY = 1.0f / (maxY - minY);

            var scale = Math.Min(scaleX, scaleY);

            var offsetX = (1.0f - (maxX - minX) * scale) / 2.0f - minX * scale;
            var offsetY = (1.0f - (maxY - minY) * scale) / 2.0f - minY * scale;

            for (int i = 0; i < points.Count; i++)
            {
                var x = points[i].X * scale + offsetX;
                var y = points[i].Y * scale + offsetY;

                points[i] = new SkiaPoint(x, y);
            }

            return points;
        }


    }
}
