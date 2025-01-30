
/*
All the XAMARIN-related base SkiaControl implementation. 
Normally other partial code definitions should be framework independent.
*/

//using Microsoft.Maui.HotReload;

using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using DrawnUi.Maui.Draw;
using SkiaSharp.Views.Forms;
using Color = Xamarin.Forms.Color;

namespace DrawnUi.Maui.Draw
{
    public partial class SkiaControl : VisualElement, IDrawnBase
    //,
    //IVisualTreeElement,
    //IReloadHandler,
    //IHotReloadableView
    {

        public static Color TransparentColor = Color.Transparent;
        public static Color WhiteColor = Color.White;
        public static Color BlackColor = Color.Black;
        public static Color RedColor = Color.Red;

        public static readonly BindableProperty ClearColorProperty = BindableProperty.Create(nameof(ClearColor), typeof(Color), typeof(SkiaControl),
            Color.Transparent,
            propertyChanged: NeedDraw);
        public Color ClearColor
        {
            get { return (Color)GetValue(ClearColorProperty); }
            set { SetValue(ClearColorProperty, value); }
        }

        public static readonly BindableProperty ZIndexProperty = BindableProperty.Create(nameof(ZIndex),
            typeof(int), typeof(SkiaControl),
            0);

        public int ZIndex
        {
            get { return (int)GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        public static readonly BindableProperty MinimumHeightRequestProperty = BindableProperty.Create(nameof(MinimumHeightRequest),
            typeof(double), typeof(SkiaControl),
            -1.0);
        public double MinimumHeightRequest
        {
            get { return (double)GetValue(MinimumHeightRequestProperty); }
            set { SetValue(MinimumHeightRequestProperty, value); }
        }

        public static readonly BindableProperty MaximumHeightRequestProperty = BindableProperty.Create(nameof(MaximumHeightRequest),
            typeof(double), typeof(SkiaControl),
            -1.0);
        public double MaximumHeightRequest
        {
            get { return (double)GetValue(MaximumHeightRequestProperty); }
            set { SetValue(MaximumHeightRequestProperty, value); }
        }

        public static readonly BindableProperty MinimumWidthRequestProperty = BindableProperty.Create(nameof(MinimumWidthRequest),
            typeof(double), typeof(SkiaControl),
            -1.0);
        public double MinimumWidthRequest
        {
            get { return (double)GetValue(MinimumWidthRequestProperty); }
            set { SetValue(MinimumWidthRequestProperty, value); }
        }

        public static readonly BindableProperty MaximumWidthRequestProperty = BindableProperty.Create(nameof(MaximumWidthRequest),
            typeof(double), typeof(SkiaControl),
            -1.0);
        public double MaximumWidthRequest
        {
            get { return (double)GetValue(MaximumWidthRequestProperty); }
            set { SetValue(MaximumWidthRequestProperty, value); }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            //if (!isApplyingStyle && !string.IsNullOrEmpty(propertyName))
            //{
            //    ExplicitPropertiesSet[propertyName] = true;
            //}

            #region intercept properties coming from VisualElement..

            //some VisualElement props will not call this method so we would override them as new

            if (propertyName.IsEither(nameof(ZIndex)))
            {
                Parent?.InvalidateViewsList();
                Repaint();
            }
            else
            if (propertyName.IsEither(
                    nameof(Opacity),
                    nameof(TranslationX), nameof(TranslationY),
                    nameof(Rotation),
                    nameof(ScaleX), nameof(ScaleY)
                ))
            {
                Repaint();
            }
            else
            if (propertyName.IsEither(nameof(BackgroundColor),
                    nameof(Background),
                    nameof(IsClippedToBounds)
                ))
            {
                Update();
            }
            else
            if (propertyName.IsEither(
                    nameof(Padding),
                    nameof(HorizontalOptions), nameof(VerticalOptions),
                    nameof(HeightRequest), nameof(WidthRequest),
                    nameof(MaximumWidthRequest), nameof(MinimumWidthRequest),
                    nameof(MaximumHeightRequest), nameof(MinimumHeightRequest)
                ))
            {
                InvalidateMeasure();
            }
            else
            if (propertyName.IsEither(nameof(IsVisible)))
            {
                OnVisibilityChanged(IsVisible);

                InvalidateMeasure();
            }
            else
            if (propertyName.IsEither(
                    nameof(AnchorX), nameof(AnchorY),
                    nameof(RotationX), nameof(RotationY)))
            {
                //todo add option not to throw?..
                throw new NotImplementedException("DrawnUi is not using this Maui VisualElement property.");
            }

            #endregion
        }


        public virtual void AddSubView(SkiaControl control)
        {
            if (control == null)
                return;
            control.SetParent(this);

            OnChildAdded(control);

            //if (Debugger.IsAttached)
            //	Superview?.PostponeExecutionAfterDraw(() =>
            //	{
            //		ReportHotreloadChildAdded(control);
            //	});
        }

        public virtual void RemoveSubView(SkiaControl control)
        {
            if (control == null)
                return;

            //if (Debugger.IsAttached)
            //	Superview?.PostponeExecutionAfterDraw(() =>
            //	{
            //		ReportHotreloadChildRemoved(control);
            //	});

            try
            {
                control.SetParent(null);
                OnChildRemoved(control);
            }
            catch (Exception e)
            {
                Super.Log(e);
            }
        }

        public SKShader CreateGradient(SKRect destination, SkiaGradient gradient)
        {
            if (gradient != null && gradient.Type != GradientType.None)
            {
                var colors = new List<SKColor>();
                foreach (var color in gradient.Colors.ToList())
                {
                    var usingColor = color;
                    if (gradient.Light < 1.0)
                    {
                        usingColor = usingColor.MakeDarker(100 - gradient.Light * 100);
                    }
                    else if (gradient.Light > 1.0)
                    {
                        usingColor = usingColor.MakeLighter(gradient.Light * 100 - 100);
                    }

                    var newAlpha = usingColor.A * gradient.Opacity;
                    usingColor = usingColor.WithAlpha(newAlpha);
                    colors.Add(usingColor.ToSKColor());
                }

                float[] colorPositions = null;
                if (gradient.ColorPositions?.Count == colors.Count)
                {
                    colorPositions = gradient.ColorPositions.Select(x => (float)x).ToArray();
                }

                switch (gradient.Type)
                {
                    case GradientType.Sweep:

                        //float sweep = (float)Value3;//((float)this.Variable1 % (float)this.Variable2 / 100F) * 360.0F;

                        return SKShader.CreateSweepGradient(
                             new SKPoint(destination.Left + destination.Width / 2.0f,
                                destination.Top + destination.Height / 2.0f),
                            colors.ToArray(),
                            colorPositions,
                            gradient.TileMode, (float)Value1, (float)(Value1 + Value2));

                    case GradientType.Circular:
                    case GradientType.Oval:
                        var halfX = gradient.StartXRatio * destination.Width;
                        var halfY = gradient.StartYRatio * destination.Height;
                        if (gradient.Type == GradientType.Circular)
                            return SKShader.CreateRadialGradient(
                                new SKPoint(destination.Left + halfX, destination.Top + halfY),
                                Math.Min(destination.Width / 2f, destination.Height / 2f),
                                colors.ToArray(),
                                colorPositions,
                                gradient.TileMode
                            );
                        var shader = SKShader.CreateRadialGradient(
                            new SKPoint(destination.Left + halfX, destination.Top + halfY),
                            Math.Max(destination.Width / 2f, destination.Height / 2f),
                            colors.ToArray(),
                            colorPositions,
                            gradient.TileMode
                        );
                        // Create a scaling matrix centered around the gradient's origin point
                        float scaleX = destination.Width >= destination.Height ? 1f : destination.Width / destination.Height;
                        float scaleY = destination.Height >= destination.Width ? 1f : destination.Height / destination.Width;
                        var transform = SKMatrix.CreateScale(scaleX, scaleY, destination.Left + halfX, destination.Top + halfY);
                        return shader.WithLocalMatrix(transform);

                    case GradientType.Linear:
                    default:
                        return SKShader.CreateLinearGradient(
                            new SKPoint(destination.Left + destination.Width * gradient.StartXRatio,
                                destination.Top + destination.Height * gradient.StartYRatio),
                            new SKPoint(destination.Left + destination.Width * gradient.EndXRatio,
                                destination.Top + destination.Height * gradient.EndYRatio),
                            colors.ToArray(),
                            colorPositions,
                            gradient.TileMode);
                        break;
                }

            }

            return null;
        }

        public static SKImageFilter CreateShadow(SkiaShadow shadow, float scale)
        {
            var colorShadow = shadow.Color;
            if (colorShadow.A == 1.0)
            {
                colorShadow = shadow.Color.WithAlpha((float)shadow.Opacity);
            }
            if (shadow.ShadowOnly)
            {
                return SKImageFilter.CreateDropShadowOnly(
                    (float)(shadow.X * scale), (float)(shadow.Y * scale),
                    (float)(shadow.Blur * scale), (float)(shadow.Blur * scale),
                    colorShadow.ToSKColor());
            }
            else
            {
                return SKImageFilter.CreateDropShadow(
                    (float)(shadow.X * scale), (float)(shadow.Y * scale),
                    (float)(shadow.Blur * scale), (float)(shadow.Blur * scale),
                    colorShadow.ToSKColor());
            }
        }

        public static float GetDensity()
        {
            return (float)Screen.DisplayInfo.Density;
        }

        protected virtual void OnLayoutChanged()
        {
            LayoutReady = this.Height > 0 && this.Width > 0;
            if (LayoutReady)
            {
                if (!CompareSize(DrawingRect.Size, _lastSize, 1))
                {
                    _lastSize = DrawingRect.Size;
                }
            }
        }

    }

    public static class XamarinExtensions
    {
        public static bool IsFinite(this double value)
        {
            return !double.IsInfinity(value);
        }

        public static bool IsFinite(this float value)
        {
            return !float.IsInfinity(value);
        }

        public static bool IsNormal(this float value)
        {
            return !float.IsInfinity(value) && !float.IsNaN(value);
        }

        public static Xamarin.Forms.Rect ToMauiRectangle(this SKRect rect)
        {
            return new(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public static SKPoint ToSKPoint(this PointF value)
        {
            return new(value.X, value.Y);
        }

    }


}
