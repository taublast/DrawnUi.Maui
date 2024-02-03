using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Infrastructure.Xaml;
using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;
using Color = Microsoft.Maui.Graphics.Color;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace DrawnUi.Maui.Draw
{
    /// <summary>
    /// Implements ISkiaGestureListener to pass gestures to children
    /// </summary>
    public class SkiaShape : SkiaControl, ISkiaGestureListener
    {

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
        /// For Type = Path, use the path markup syntax
        /// </summary>
        public string PathData
        {
            get { return (string)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type),
            typeof(ShapeType), typeof(SkiaShape),
            ShapeType.Rectangle,
            propertyChanged: NeedSetType);
        public ShapeType Type
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
        /// This is for the tricky case when you want to drop shadow but keep background transparent to see through, set to True in that case.
        /// </summary>
        public bool ClipBackgroundColor
        {
            get { return (bool)GetValue(ClipBackgroundColorProperty); }
            set { SetValue(ClipBackgroundColorProperty, value); }
        }


        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            nameof(CornerRadius),
            typeof(Thickness),
            typeof(SkiaShape),
            Thickness.Zero,
            propertyChanged: NeedInvalidateMeasure);

        public Thickness CornerRadius
        {
            get { return (Thickness)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(
            nameof(StrokeWidth),
            typeof(double),
            typeof(SkiaShape),
            0.0,
            propertyChanged: NeedInvalidateMeasure);

        public double StrokeWidth
        {
            get { return (double)GetValue(StrokeWidthProperty); }
            set { SetValue(StrokeWidthProperty, value); }
        }

        public static readonly BindableProperty StrokePathProperty = BindableProperty.Create(
            nameof(StrokePath),
            typeof(double[]),
            typeof(SkiaShape),
            null);

        [TypeConverter(typeof(StringToDoubleArrayTypeConverter))]
        public double[] StrokePath
        {
            get { return (double[])GetValue(StrokePathProperty); }
            set { SetValue(StrokePathProperty, value); }
        }


        public static readonly BindableProperty StrokeCapProperty = BindableProperty.Create(
            nameof(StrokeCap),
            typeof(SKStrokeCap),
            typeof(SkiaShape),
            SKStrokeCap.Round,
            propertyChanged: NeedDraw);

        public SKStrokeCap StrokeCap
        {
            get { return (SKStrokeCap)GetValue(StrokeCapProperty); }
            set { SetValue(StrokeCapProperty, value); }
        }

        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
            nameof(StrokeColor),
            typeof(Color),
            typeof(SkiaShape),
            Colors.Transparent,
            propertyChanged: NeedDraw);

        public Color StrokeColor
        {
            get { return (Color)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }



        public static readonly BindableProperty LayoutChildrenProperty = BindableProperty.Create(
            nameof(LayoutChildren),
            typeof(LayoutType),
            typeof(SkiaShape),
            LayoutType.Absolute,
            propertyChanged: NeedDraw);

        public LayoutType LayoutChildren
        {
            get { return (LayoutType)GetValue(LayoutChildrenProperty); }
            set { SetValue(LayoutChildrenProperty, value); }
        }


        public static readonly BindableProperty StrokeBlendModeProperty = BindableProperty.Create(nameof(StrokeBlendMode),
           typeof(SKBlendMode), typeof(SkiaShape),
           SKBlendMode.SrcOver,
           propertyChanged: NeedDraw);
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

        public virtual void SetupType()
        {
            if (!string.IsNullOrEmpty(PathData) && Type == ShapeType.Path)
            {
                var kill = DrawPath;
                DrawPath = SKPath.ParseSvgPathData(this.PathData);
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

            var willStroke = StrokeColor != Colors.Transparent && StrokeWidth > 0;
            float pixelsStrokeWidth = 0;
            float halfStroke = 0;

            if (willStroke)
            {
                pixelsStrokeWidth = (float)Math.Round(StrokeWidth * scale);
                halfStroke = (float)Math.Round(pixelsStrokeWidth / 2.0f);

                strokeAwareSize =
                    SKRect.Inflate(strokeAwareSize, -halfStroke, -halfStroke);

                strokeAwareChildrenSize =
                    SKRect.Inflate(strokeAwareSize, -halfStroke, -halfStroke);
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
                float translateX = (strokeAwareSize.Width - (bounds.Width + halfStroke) * scaleX) / 2 - bounds.Left * scaleX;
                float translateY = (strokeAwareSize.Height - (bounds.Height + halfStroke) * scaleY) / 2 - bounds.Top * scaleY;
                SKMatrix matrix = SKMatrix.CreateIdentity();
                SKMatrix.PreConcat(ref matrix, SKMatrix.CreateScale(scaleX, scaleY));
                SKMatrix.PreConcat(ref matrix, SKMatrix.CreateTranslation(translateX, translateY));
                stretched.Transform(matrix);
                stretched.Offset(halfStroke, halfStroke);

                DrawPathResized.Reset();
                DrawPathResized.AddPath(stretched);
            }
        }

        public SKPath DrawPathResized { get; } = new();

        public SKPath DrawPathAligned { get; } = new();

        protected SKRect MeasuredStrokeAwareChildrenSize { get; set; }

        protected SKRect MeasuredStrokeAwareSize { get; set; }

        protected SKPaint RenderingPaint { get; set; }

        /// <summary>
        /// Parsed PathData
        /// </summary>
        protected SKPath DrawPath { get; set; } = new();

        public override void OnDisposing()
        {

            RenderingPaint?.Dispose();
            RenderingPaint = null;

            DrawPath?.Dispose();
            DrawPathResized?.Dispose();
            DrawPathAligned?.Dispose();

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

        public override SKPath CreateClip(object arguments, bool usePosition)
        {
            var strokeAwareSize = MeasuredStrokeAwareSize;
            var strokeAwareChildrenSize = MeasuredStrokeAwareChildrenSize;

            if (arguments is ShapePaintArguments args)
            {
                strokeAwareSize = args.StrokeAwareSize;
                strokeAwareChildrenSize = args.StrokeAwareChildrenSize;
            }

            if (!usePosition)
            {
                var offsetToZero = new SKPoint(strokeAwareSize.Left - strokeAwareChildrenSize.Left, strokeAwareSize.Top - strokeAwareChildrenSize.Top);
                strokeAwareChildrenSize = new(offsetToZero.X, offsetToZero.Y, strokeAwareChildrenSize.Width + offsetToZero.X, strokeAwareChildrenSize.Height + offsetToZero.Y);
            }
            var path = new SKPath();

            switch (Type)
            {
            case ShapeType.Path:
            path.AddPath(DrawPathResized);
            break;

            case ShapeType.Circle:
            path.AddCircle(
               (float)Math.Round(strokeAwareChildrenSize.Left + strokeAwareChildrenSize.Width / 2.0f),
               (float)Math.Round(strokeAwareChildrenSize.Top + strokeAwareChildrenSize.Height / 2.0f),
               Math.Min(strokeAwareChildrenSize.Width, strokeAwareChildrenSize.Height) /
               2.0f);
            break;

            case ShapeType.Ellipse:
            path.AddOval(strokeAwareChildrenSize);
            break;

            case ShapeType.Rectangle:
            default:
            if (CornerRadius != Thickness.Zero)
            {
                var scaledRadiusLeftTop = (float)(CornerRadius.Left * RenderingScale);
                var scaledRadiusRightTop = (float)(CornerRadius.Right * RenderingScale);
                var scaledRadiusLeftBottom = (float)(CornerRadius.Top * RenderingScale);
                var scaledRadiusRightBottom = (float)(CornerRadius.Bottom * RenderingScale);
                var rrect = new SKRoundRect(strokeAwareChildrenSize);

                // Step 3: Calculate the inner rounded rectangle corner radii
                float strokeWidth = (float)Math.Floor(Math.Round(StrokeWidth * RenderingScale));

                float cornerRadiusDifference = (float)strokeWidth / 2.0f;

                scaledRadiusLeftTop = (float)Math.Round(Math.Max(scaledRadiusLeftTop - cornerRadiusDifference, 0));
                scaledRadiusRightTop = (float)Math.Round(Math.Max(scaledRadiusRightTop - cornerRadiusDifference, 0));
                scaledRadiusLeftBottom = (float)Math.Round(Math.Max(scaledRadiusLeftBottom - cornerRadiusDifference, 0));
                scaledRadiusRightBottom = (float)Math.Round(Math.Max(scaledRadiusRightBottom - cornerRadiusDifference, 0));

                rrect.SetRectRadii(strokeAwareChildrenSize, new[]
                {
                            new SKPoint(scaledRadiusLeftTop,scaledRadiusLeftTop),
                            new SKPoint(scaledRadiusRightTop,scaledRadiusRightTop),
                            new SKPoint(scaledRadiusLeftBottom,scaledRadiusLeftBottom),
                            new SKPoint(scaledRadiusRightBottom,scaledRadiusRightBottom),
                        });
                path.AddRoundRect(rrect);
                //path.AddRoundRect(strokeAwareChildrenSize, innerCornerRadius, innerCornerRadius);
            }
            else
                path.AddRect(strokeAwareChildrenSize);
            break;
            }

            return path;
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
            var willStroke = StrokeColor != Colors.Transparent && StrokeWidth > 0;
            float pixelsStrokeWidth = (float)Math.Round(StrokeWidth * scale);

            RenderingPaint ??= new SKPaint()
            {
                //IsAntialias = true,
            };

            //if (IsDistorted)
            //{
            //    RenderingPaint.FilterQuality = SKFilterQuality.Medium;
            //}
            //else
            //{
            //    RenderingPaint.FilterQuality = SKFilterQuality.None;
            //}

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

            var scaledRadiusLeftTop = (float)Math.Round(CornerRadius.Left * scale);
            var scaledRadiusRightTop = (float)Math.Round(CornerRadius.Right * scale);
            var scaledRadiusLeftBottom = (float)Math.Round(CornerRadius.Top * scale);
            var scaledRadiusRightBottom = (float)Math.Round(CornerRadius.Bottom * scale);

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
                paint.StrokeCap = this.StrokeCap; //todo full stroke object

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

                using SKPath path1 = new SKPath();

                switch (Type)
                {
                case ShapeType.Rectangle:
                if (CornerRadius != Thickness.Zero)
                {
                    var rrect = new SKRoundRect();
                    rrect.SetRectRadii(outRect, new[]
                    {
                                    new SKPoint(scaledRadiusLeftTop,scaledRadiusLeftTop),
                                    new SKPoint(scaledRadiusRightTop,scaledRadiusRightTop),
                                    new SKPoint(scaledRadiusLeftBottom,scaledRadiusLeftBottom),
                                    new SKPoint(scaledRadiusRightBottom,scaledRadiusRightBottom),
                                });
                    ctx.Canvas.DrawRoundRect(rrect, paint);
                }
                //ctx.Canvas.DrawRoundRect(outRect, scaledRadius, scaledRadius, paint);
                else
                    ctx.Canvas.DrawRect(outRect, paint);
                break;

                case ShapeType.Circle:
                ctx.Canvas.DrawCircle(outRect.MidX, outRect.MidY, minSize / 2.0f, paint);
                break;

                case ShapeType.Ellipse:
                path1.AddOval(outRect);
                ctx.Canvas.DrawPath(path1, paint);
                break;

                case ShapeType.Arc:
                // Start & End Angle for Radial Gauge
                var startAngle = (float)Value1;
                var sweepAngle = (float)Value2;
                path1.AddArc(outRect, startAngle, sweepAngle);
                ctx.Canvas.DrawPath(path1, paint);
                break;

                case ShapeType.Path:
                path1.AddPath(DrawPathAligned);
                ctx.Canvas.DrawPath(path1, paint);

                break;
                }

            }

            using var pathB = new SKPath();

            void PaintBackground(SKPaint paint)
            {
                paint.BlendMode = this.FillBlendMode;

                switch (Type)
                {
                case ShapeType.Rectangle:
                if (CornerRadius != Thickness.Zero)
                {
                    if (StrokeWidth == 0 || StrokeColor == Colors.Transparent)
                    {
                        paint.IsAntialias = true;
                    }

                    var rrect = new SKRoundRect();
                    rrect.SetRectRadii(outRect, new[]
                    {
                                    new SKPoint(scaledRadiusLeftTop,scaledRadiusLeftTop),
                                    new SKPoint(scaledRadiusRightTop,scaledRadiusRightTop),
                                    new SKPoint(scaledRadiusLeftBottom,scaledRadiusLeftBottom),
                                    new SKPoint(scaledRadiusRightBottom,scaledRadiusRightBottom),
                                });

                    ctx.Canvas.DrawRoundRect(rrect, RenderingPaint);
                }
                else
                    ctx.Canvas.DrawRect(outRect, RenderingPaint);
                break;

                case ShapeType.Circle:
                if (StrokeWidth == 0 || StrokeColor == Colors.Transparent)
                {
                    paint.IsAntialias = true;
                }
                ctx.Canvas.DrawCircle(outRect.MidX, outRect.MidY, minSize / 2.0f, RenderingPaint);
                break;

                case ShapeType.Ellipse:
                if (StrokeWidth == 0 || StrokeColor == Colors.Transparent)
                {
                    paint.IsAntialias = true;
                }
                pathB.AddOval(outRect);
                ctx.Canvas.DrawPath(pathB, paint);
                break;

                case ShapeType.Path:
                if (StrokeWidth == 0 || StrokeColor == Colors.Transparent)
                {
                    paint.IsAntialias = true;
                }


                ctx.Canvas.DrawPath(DrawPathAligned, paint);

                break;

                //case ShapeType.Arc: - has no background
                }
            }

            void PaintWithShadows(Action render)
            {
                if (Shadows != null && Shadows.Count > 0)
                {
                    for (int index = 0; index < Shadows.Count(); index++)
                    {
                        AddShadowFilter(RenderingPaint, Shadows[index]);

                        if (ClipBackgroundColor)
                        {
                            using var clip = new SKPath();
                            using var clipContent = CreateClip(arguments, true);
                            clip.AddPath(clipContent);
                            ctx.Canvas.Save();
                            ctx.Canvas.ClipPath(clip, SKClipOperation.Difference, true);

                            render();

                            ctx.Canvas.Restore();
                        }
                        else
                        {
                            render();
                        }
                    }
                }
                else
                {
                    render();
                }
            }

            //background with shadows pass, no stroke
            PaintWithShadows(() =>
            {
                //add gradient
                //if gradient opacity is not 1, then we need to fill with background color first
                //then on top draw semi-transparent gradient
                if (FillGradient?.Opacity != 1
                    && BackgroundColor != null && BackgroundColor != Colors.Transparent)
                {
                    RenderingPaint.Color = BackgroundColor.ToSKColor();
                    RenderingPaint.Shader = null;
                    RenderingPaint.BlendMode = this.FillBlendMode;

                    PaintBackground(RenderingPaint);
                }

                var hasGradient = SetupGradient(RenderingPaint, FillGradient, outRect);
                PaintBackground(RenderingPaint);
            });

            //draw children views clipped with shape
            using var clip = new SKPath();
            using var clipContent = CreateClip(arguments, true);
            clip.AddPath(clipContent);
            ctx.Canvas.Save();
            ctx.Canvas.ClipPath(clip, SKClipOperation.Intersect, true);

            var rectForChildren = ContractPixelsRect(strokeAwareChildrenSize, scale, Padding);

            DrawViews(ctx, rectForChildren, scale);

            ctx.Canvas.Restore();

            //last pass for stroke over background or children
            if (willStroke)
            {
                PaintStroke(RenderingPaint);
            }

        }


        #endregion

        public void OnFocusChanged(bool focus)
        { }
    }
}
