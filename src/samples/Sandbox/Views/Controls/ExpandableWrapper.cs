using AppoMobi.Maui.Gestures;
using DrawnUi.Maui.Extensions;
using System.Diagnostics;

namespace Sandbox.Views.Controls
{
    public class ExpandableVerticalStack : SkiaLayout, ISkiaGestureListener
    {
        public ExpandableVerticalStack()
        {
            this.Type = LayoutType.Stack;
        }

        public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
        {
            heightConstraint = float.PositiveInfinity;

            return base.Measure(widthConstraint, heightConstraint, scale);
        }
    }

    public class ExpandableWrapper : ContentLayout, ISkiaGestureListener
    {

        public ExpandableWrapper()
        {
            ApplyProperties();
        }

        public override void OnDisposing()
        {
            if (_overlay != null && !_overlay.IsDisposed)
            {
                _overlay.Dispose();
            }

            base.OnDisposing();
        }


        public static readonly BindableProperty MaxHeightProperty = BindableProperty.Create(
            nameof(MaxHeight),
            typeof(double),
            typeof(ExpandableWrapper),
            250.0,
             propertyChanged: NeedRecalculate);

        public double MaxHeight
        {
            get { return (double)GetValue(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
            nameof(IsOpen),
            typeof(bool),
            typeof(ExpandableWrapper),
            false, propertyChanged: NeedToggle,
            defaultBindingMode: BindingMode.TwoWay);

        private static void NeedToggle(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is ExpandableWrapper control)
            {
                control.ApplyProperties();
                control.Redraw();
            }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
        }

        public override ISkiaGestureListener ProcessGestures(
            TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
            SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
        {

            if (touchAction != TouchActionResult.Tapped)
            {
                return null;
            }

            //var thisOffset = TranslateInputCoords(childOffset);

            ISkiaGestureListener PassToChildren()
            {
                //if (touchAction == TouchActionResult.Tapped && RenderObject != null)
                //    Trace.WriteLine($"[PassToChildren] {DrawingRect}:{RenderObject.Bounds} -> {childOffset}");

                return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
            }

            if (NeedWrap && !IsOpen)
            {
                if (touchAction == TouchActionResult.Tapped)
                {
                    if (TouchEffect.CheckLockAndSet(Uid.ToString()))
                        return this;

                    IsOpen = !IsOpen;

                    OnToggled?.Invoke(this, IsOpen);

                    return this;
                }
                return null;
            }

            var consumed = PassToChildren();

            if (consumed != null)
            {
                return consumed;
            }

            if (NeedWrap)
            {
                if (touchAction == TouchActionResult.Tapped)
                {
                    IsOpen = !IsOpen;
                    return this;
                }
            }

            return null;
        }

        public event EventHandler<bool> OnToggled;

        SkiaControl _overlay;
        SkiaControl _background;
        private bool _needWrap = true;

        protected override void SetContent(SkiaControl view)
        {
            if (view != null)
            {
                view.ZIndex = 0;
            }

            base.SetContent(view);

            ApplyProperties();
        }

        public bool NeedWrap
        {
            get => _needWrap;
            protected set => _needWrap = value;
        }

        private static void NeedRecalculate(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is ExpandableWrapper control)
            {
                Debug.WriteLine($"[InvalidateMeasure] ------------------------------");

                control.InvalidateMeasure();
            }
        }

        bool lastOpen;

        protected override int DrawViews(SkiaDrawingContext context, SKRect destination, float scale, bool debug = false)
        {
            if (context.Superview == null || destination.Width <= 0 || destination.Height <= 0)
            {
                return 0;
            }

            var drawViews = GetOrderedSubviews();

            if (!NeedWrap || IsOpen)
            {
                drawViews = drawViews.Where(x => x != _overlay).ToList();
            }

            return RenderViewsList(drawViews, context, destination, scale);
        }

        protected override ScaledSize SetMeasured(float width, float height, float scale)
        {

            return base.SetMeasured(width, height, scale);
        }

        bool isMeasuring;

        public void Redraw()
        {
            NeedMeasure = true;
            Update();
        }

        public virtual void ApplyProperties()
        {

            if (_background == null)
            {
                _background = new SkiaShape()
                {
                    Tag = "_background",
                    InputTransparent = true,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    ZIndex = -1,
                    Margin = new Thickness(16, 0),
                    CornerRadius = 4,
                    BackgroundColor = App.Current.Resources.Get<Color>("ColorPaper"),
                };
            }

            if (_overlay == null)
            {
                _overlay = new SkiaLayout()
                {
                    Tag = "_overlay",
                    InputTransparent = true,
                    UseCache = SkiaCacheType.Image,
                    HorizontalOptions = LayoutOptions.Fill,
                    HeightRequest = this.MaxHeight,
                    ZIndex = 1,
                    CreateChildren = () => new()
                    {
                        new SkiaLabel
                        {
                            MarginBottom = 32,
                            TextColor = App.Current.Resources.Get<Color>("ColorPrimary"),
                            FontFamily = "FontText",
                            FontSize = 15,
                            FontWeight = 600,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.End,
                            Text = "SeeMore"
                        },
                        new SkiaShape
                        {
                            ZIndex = -1,
                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = new Thickness(32, 0),
                            FillGradient = new SkiaGradient
                            {
                                EndXRatio = 0,
                                EndYRatio = 1,
                                StartXRatio = 0,
                                StartYRatio = 0,
                                Type = GradientType.Linear,
                                Colors = new List<Color>
                                {
                                    Color.FromArgb("#00FFFFFF"),
                                    Color.FromArgb("#66FFFFFF"),
                                    Color.FromArgb("#EEFFFFFF"),
                                    Color.FromArgb("#FFFFFFFF")
                                },
                                ColorPositions = new List<double>
                                {
                                    0.25,
                                    0.5,
                                    0.8,
                                    1.0
                                }
                            }
                        }
                    }
                };
            }

            if (Views.All(x => x != _background))
            {
                AddSubView(_background);
                AddSubView(_overlay);
            }

            if (!IsOpen)
            {
                ViewportHeightLimit = MaxHeight; //this could invalidate parent

                if (Content != null)
                {
                    if (_overlay != null)
                    {
                        _overlay.NeedMeasure = true;
                    }

                    Content.InputTransparent = _needWrap;

                    Update();
                }

            }
            else
            {
                if (_background != null)
                {
                    _background.NeedMeasure = true;
                }

                ViewportHeightLimit = -1;//this could invalidate parent

                if (Content != null)
                {
                    Content.InputTransparent = false;
                }
            }

        }

        object lockMeasure = new();

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            NeedMeasure = true;
        }


        protected override ScaledSize MeasureContent(IEnumerable<SkiaControl> children, SKRect rectForChildrenPixels, float scale)
        {
            var fakeRect = new SKRect(rectForChildrenPixels.Left, rectForChildrenPixels.Top, rectForChildrenPixels.Right, float.PositiveInfinity);

            return base.MeasureContent(children, fakeRect, scale);
        }

        public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
        {
            //if (IsMeasuring || !CanDraw || (widthConstraint < 0 || heightConstraint < 0))
            //{
            //    return MeasuredSize;
            //}

            lock (lockMeasure)
            {

                if (_background != null)
                {
                    _background.NeedMeasure = true;
                }

                if (_overlay != null)
                {
                    _overlay.NeedMeasure = true;
                }

                float ProvideHeight(bool wrap)
                {
                    if (wrap && !IsOpen)
                    {
                        return (float)Math.Round((MaxHeight - Margin.Top - Margin.Bottom) * RenderingScale);
                    }

                    return float.PositiveInfinity;
                }

                //self wil be measured with max available height OR with limited, choosen upon props
                var provideHeight = ProvideHeight(true);

                var measured = base.Measure(widthConstraint, provideHeight, scale);

                _needWrap = measured.Units.Height >= (MaxHeight - Margin.Top - Margin.Bottom) - 44;

                if (!_needWrap && Content != null)
                {
                    Content.InputTransparent = false;
                }

                lastOpen = IsOpen;

                return measured;
            }
        }
    }


}
