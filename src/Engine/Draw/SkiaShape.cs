using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Infrastructure.Xaml;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Color = Microsoft.Maui.Graphics.Color;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace DrawnUi.Maui.Draw
{
    /// <summary>
    /// Implements ISkiaGestureListener to pass gestures to children
    /// </summary>
    public class SkiaShape : ContentLayout, ISkiaGestureListener
    {
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
            typeof(CornerRadius),
            typeof(SkiaShape),
            default(CornerRadius),
            propertyChanged: NeedInvalidateMeasure);

        [System.ComponentModel.TypeConverter(typeof(Microsoft.Maui.Converters.CornerRadiusTypeConverter))]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
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

            var willStroke = StrokeColor != Colors.Transparent && StrokeWidth > 0;
            float pixelsStrokeWidth = 0;
            float halfStroke = 0;

            if (willStroke)
            {
                pixelsStrokeWidth = (float)Math.Round(StrokeWidth * scale);
                halfStroke = (float)(pixelsStrokeWidth / 2.0f);

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

        protected SKRect MeasuredStrokeAwareChildrenSize { get; set; }

        protected SKRect MeasuredStrokeAwareSize { get; set; }

        protected SKPaint RenderingPaint { get; set; }

        /// <summary>
        /// Parsed PathData
        /// </summary>
        protected SKPath DrawPath { get; set; } = new();

        protected SKPath DrawPathShape { get; set; } = new();

        protected SKRoundRect DrawRoundedRect { get; set; }

        public override void OnDisposing()
        {

            RenderingPaint?.Dispose();
            RenderingPaint = null;

            DrawPath?.Dispose();
            DrawPathResized?.Dispose();
            DrawPathAligned?.Dispose();
            DrawRoundedRect?.Dispose();
            DrawPathShape?.Dispose();

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
            if (CornerRadius != default)
            {
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
                                new SKPoint(scaledRadiusLeftTop,scaledRadiusLeftTop),
                                new SKPoint(scaledRadiusRightTop,scaledRadiusRightTop),
                                new SKPoint(scaledRadiusRightBottom,scaledRadiusRightBottom),
                                new SKPoint(scaledRadiusLeftBottom,scaledRadiusLeftBottom),
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
                if (StrokeWidth == 0 || StrokeColor == Colors.Transparent)
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
            DrawPathShape.Reset();
            DrawPathShape.AddOval(outRect);
            ctx.Canvas.DrawPath(DrawPathShape, paint);

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

            CornerRadius scaledRadius = new(
                (CornerRadius.TopLeft * scale),
                (CornerRadius.TopRight * scale),
                (CornerRadius.BottomLeft * scale),
                (CornerRadius.BottomRight * scale));

            var radii = new SKPoint[]
            {
                new SKPoint((float)scaledRadius.TopLeft,(float)scaledRadius.TopLeft), //LeftTop
                new SKPoint((float)scaledRadius.TopRight,(float)scaledRadius.TopRight), //RightTop
                new SKPoint((float)scaledRadius.BottomLeft,(float)scaledRadius.BottomLeft), //LeftBottom
                new SKPoint((float)scaledRadius.BottomRight,(float)scaledRadius.BottomRight), //RightBottom
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
                }

            }

            void PaintWithShadows(Action render)
            {
                if (Shadows != null && Shadows.Count > 0)
                {
                    for (int index = 0; index < Shadows.Count(); index++)
                    {
                        AddShadowFilter(RenderingPaint, Shadows[index], RenderingScale);

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

                    PaintBackground(ctx, outRect, radii, minSize, RenderingPaint);
                }

                var hasGradient = SetupGradient(RenderingPaint, FillGradient, outRect);
                PaintBackground(ctx, outRect, radii, minSize, RenderingPaint);
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
                var created = new ObservableCollection<SkiaShadow>();
                ShadowsPropertyChanged(instance, null, created);
                return created;
            },
            validateValue: (bo, v) => v is IList<SkiaShadow>,
            propertyChanged: NeedDraw,
            coerceValue: CoerceShadows);

        private static int instanceCount = 0;

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


    }
}
