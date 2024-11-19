
/*
All the MAUI-related base SkiaControl implementation. 
Normally other partial code definitions should be framework independent.
*/

using System.Runtime.CompilerServices;
using Microsoft.Maui.HotReload;

namespace DrawnUi.Maui.Draw
{
    public partial class SkiaControl : VisualElement,
        IVisualTreeElement,
        IReloadHandler,
        IHotReloadableView
    {
        public static Color TransparentColor = Colors.Transparent;
        public static Color WhiteColor = Colors.White;
        public static Color BlackColor = Colors.Black;
        public static Color RedColor = Colors.Red;

        public static readonly BindableProperty ClearColorProperty = BindableProperty.Create(nameof(ClearColor), typeof(Color), typeof(SkiaControl),
            Colors.Transparent,
            propertyChanged: NeedDraw);
        public Color ClearColor
        {
            get { return (Color)GetValue(ClearColorProperty); }
            set { SetValue(ClearColorProperty, value); }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

            try
            {
                base.OnPropertyChanged(propertyName);
            }
            catch (Exception e)
            {
                //we are avoiding MAUI crashes due concurrent access to properties from different threads
                Super.Log(e);
            }

            //if (!isApplyingStyle && !string.IsNullOrEmpty(propertyName))
            //{
            //    ExplicitPropertiesSet[propertyName] = true;
            //}

            #region intercept properties coming from VisualElement..

            //some VisualElement props will not call this method so we would override them as new

            if (propertyName == nameof(ZIndex))
            {
                Parent?.InvalidateViewsList();
                Repaint();
            }
            else
            if (propertyName.IsEither(
                    nameof(Opacity),
                    nameof(TranslationX), nameof(TranslationY),
                    nameof(Rotation),
                    nameof(AnchorX), nameof(AnchorY),
                    nameof(RotationX), nameof(RotationY),
                    nameof(ScaleX), nameof(ScaleY)
                ))
            {
                Repaint();
            }
            else
            if (propertyName.IsEither(nameof(BackgroundColor),
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

            #endregion
        }

        #region HotReload

        IView IReplaceableView.ReplacedView =>
            MauiHotReloadHelper.GetReplacedView(this) ?? this;

        IReloadHandler IHotReloadableView.ReloadHandler { get; set; }

        void IHotReloadableView.TransferState(IView newView)
        {
            //reload the the ViewModel
            if (newView is SkiaControl v)
                v.BindingContext = BindingContext;
        }

        void IHotReloadableView.Reload()
        {
            InvalidateMeasure();
        }

        #endregion

        #region IVisualTreeElement

        public virtual IReadOnlyList<IVisualTreeElement> GetVisualChildren() //working fine
        {
            return Views.Cast<IVisualTreeElement>().ToList().AsReadOnly();

            //return Views.Select(x => x as IVisualTreeElement).ToList().AsReadOnly();;
        }

        public virtual IVisualTreeElement GetVisualParent()  //working fine
        {
            return Parent as IVisualTreeElement;
        }

        #endregion

        #region HOTRELOAD

        /// <summary>
        /// HOTRELOAD IReloadHandler
        /// </summary>
        public virtual void Reload()
        {
            InvalidateMeasure();
        }

        public virtual void ReportHotreloadChildAdded(SkiaControl child)
        {
            if (child == null)
                return;

            //this.OnChildAdded(child);

            var children = GetVisualChildren();
            var index = children.FindIndex(child);

            if (index >= 0)
                VisualDiagnostics.OnChildAdded(this, child, index);
        }

        public virtual void ReportHotreloadChildRemoved(SkiaControl control)
        {
            if (control == null)
                return;


            var children = GetVisualChildren();
            var index = children.FindIndex(control);

            if (index >= 0)
                VisualDiagnostics.OnChildRemoved(this, control, index);
            //            this.OnChildRemoved(control, index);
        }



        #endregion


        public virtual void AddSubView(SkiaControl control)
        {
            if (control == null)
                return;
            control.SetParent(this);

            OnChildAdded(control);

            if (Debugger.IsAttached)
                Superview?.PostponeExecutionAfterDraw(() =>
                {
                    ReportHotreloadChildAdded(control);
                });
        }

        public virtual void RemoveSubView(SkiaControl control)
        {
            if (control == null)
                return;

            if (Debugger.IsAttached)
                Superview?.PostponeExecutionAfterDraw(() =>
                {
                    ReportHotreloadChildRemoved(control);
                });

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

        protected virtual void OnLayoutChanged()
        {
            var ready = this.Height > 0 && this.Width > 0;
            if (ready)
            {
                if (!CompareSize(DrawingRect.Size, _lastSize, 1))
                {
                    _lastSize = DrawingRect.Size;
                    Frame = new Rect(DrawingRect.Left, DrawingRect.Top, DrawingRect.Width, DrawingRect.Height);
                }
            }
            LayoutReady = ready;
        }

        /// <summary>
        /// Creates Shader for gradient and sets it to passed SKPaint along with BlendMode
        /// </summary>
        /// <param name="paint"></param>
        /// <param name="gradient"></param>
        /// <param name="destination"></param>
        public bool SetupGradient(SKPaint paint, SkiaGradient gradient, SKRect destination)
        {
            if (paint != null)
            {
                if (gradient != null)
                {
                    if (paint.Color.Alpha == 0)
                    {
                        paint.Color = SKColor.FromHsl(0, 0, 0);
                    }

                    paint.Color = SKColors.White;
                    paint.BlendMode = gradient.BlendMode;

                    var kill = paint.Shader;
                    paint.Shader = CreateGradient(destination, gradient);
                    kill?.Dispose();

                    return true;
                }
                else
                {
                    var kill = paint.Shader;
                    paint.Shader = null;
                    kill?.Dispose();
                }
            }

            return false;
        }

        public static SKImageFilter CreateShadow(SkiaShadow shadow, float scale)
        {
            var colorShadow = shadow.Color;
            if (colorShadow.Alpha == 1.0)
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
            return (float)Super.Screen.Density;
        }
    }
}
