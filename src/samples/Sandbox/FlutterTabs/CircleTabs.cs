using System;
using System.Xml;
using AppoMobi.Maui.Gestures;

namespace Sandbox
{


    public class TabCircle : SkiaShape
    {
        public readonly SkiaLayout Container;

        public TabCircle()
        {
            Type = ShapeType.Circle;
            
            Container = new SkiaLayout()
            {
                HorizontalOptions =LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
            };
            Content = Container;
        }

        public void AddIcon(SkiaControl icon)
        {
            icon.VerticalOptions = LayoutOptions.Center;
            icon.HorizontalOptions = LayoutOptions.Center;
            if (icon.Parent != Container)
            {
                Container.AddSubView(icon);
            }
        }
    }

    public class CircleTabs : CustomTabsSelector
    {
        private float StartAnimationRange = 0.33f;
        private float EndAnimationRange = 0.6f;
        private Easing easingAnimation = Easing.BounceIn;

        private bool IsAnimated = true;

        private readonly TabCircle _button;
        private readonly CircleTabsShape _track;
        private readonly SkiaLayout _grid;
        private bool _wasBuilt;
        private float _lastButtonMiddle;

        protected override void OnLayoutReady()
        {
            base.OnLayoutReady();

            SetupTabs();

            ApplyColors();

            ApplyButtonSize();

            PaintSelectedIndex();
        }

        void ApplyButtonSize()
        {
            var topMargin = ButtonSize / 2 - ButtonPadding ;

            _track.Margin = new Thickness(0, topMargin, 0, 0); 
            _track.Setup(ButtonSize, ButtonPadding);

            _grid.Margin = new Thickness(CircleTabsShape.SidePadding, topMargin, CircleTabsShape.SidePadding, 0);

            _button.HeightRequest = ButtonSize;
            _button.WidthRequest = ButtonSize;

            //_button.IsVisible = false;
            //_grid.IsVisible = false;
        }

        void ApplyColors()
        {
            _button.BackgroundColor = ButtonColor;
            _track.BackgroundColor = BarColor;
        }

        public CircleTabs()
        {

            _button = new TabCircle()
            {
                UseCache = SkiaCacheType.Operations
            };

            _track = new CircleTabsShape(ButtonSize, ButtonPadding)
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                UseCache = SkiaCacheType.Operations
            };

            _grid = new SkiaLayout()
            {
                UseCache = SkiaCacheType.Operations,
                Margin = new (CircleTabsShape.SidePadding, 0),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center
            };

            ApplyColors();

            ApplyButtonSize();

            Children = new List<SkiaControl>()
            {
                _track, _grid, _button
            };
        }


        public int TabsCount
        {
            get
            {
                if (IndicatorsUnselected == null)
                    return 0;
                return IndicatorsUnselected.Count;
            }
        }

        public override ISkiaGestureListener ProcessGestures(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
        {
            if (args.Type == TouchActionResult.Tapped && TabsCount > 0)
            {
                var thisOffset = TranslateInputCoords(apply.childOffset);
                var x = args.Event.Location.X + thisOffset.X;
                var y = args.Event.Location.Y + thisOffset.Y;
                var relativeX = x - LastDrawnAt.Left; //inside this control
                var relativeY = y - LastDrawnAt.Top; //inside this control

                //define which tab area was tapped
                var tabArea = (int)(relativeX / (this.DrawingRect.Width / TabsCount));

                SelectedIndex = tabArea;
                
                return this; 
            }

            return base.ProcessGestures(args, apply);
        }

        public override async Task OnTabSelectionChanged(bool tabsChanged, int index)
        {
            await base.OnTabSelectionChanged(tabsChanged, index);

            if (IsAnimated && IsLayoutReady)
            {
                AnimateToSelectedIndex();
            }
            else
            {
                PaintSelectedIndex();
            }
        }

        #region ANIMATION

        CancellationTokenSource _cts;
        private SkiaControl _currentButtonIcon;
        private SkiaControl _nextButtonIcon;

        public static readonly BindableProperty AnimationSpeedMsProperty = BindableProperty.Create(
            nameof(AnimationSpeedMs),
            typeof(int),
            typeof(CircleTabs),
            250);

        public int AnimationSpeedMs
        {
            get { return (int)GetValue(AnimationSpeedMsProperty); }
            set { SetValue(AnimationSpeedMsProperty, value); }
        }

        private bool IsAnimating { get; set; }

        void AnimateToSelectedIndex()
        {
            if (SelectedIndex < 0 || TabsCount < 1)
            {
                return;
            }

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var speed = AnimationSpeedMs;

            var area = _grid.DrawingRect.Width / TabsCount;
            var selectedArea = area * SelectedIndex;
            var middle = area / 2;
            var targetButtonMiddlePixels = selectedArea + middle + CircleTabsShape.SidePadding*RenderingScale;

            var scale = RenderingScale;
            var fromx = _lastButtonMiddle;
            var toX = targetButtonMiddlePixels;

            _nextButtonIcon = IndicatorsSelected[SelectedIndex];
            if (PreviousSelectedIndex >= 0 && SelectedIndex != PreviousSelectedIndex)
            {
                _previousUnselectedIndicator = IndicatorsUnselected[PreviousSelectedIndex];
                _currentButtonIcon = IndicatorsSelected[PreviousSelectedIndex];
            }
            else
            {
                _currentButtonIcon = IndicatorsSelected[SelectedIndex];
                _previousUnselectedIndicator = null;
            }

            var cancel = _cts;
            _ = Task.Run(async () =>
            {
                IsAnimating = true;
                bool apply = true;
                await AnimateAsync((range) =>
                    {
                        //animate
                        //range going 0.0 - 1.0
                        var animatedX = fromx + (toX - fromx) * range;
                        this.AllowCaching = false;

                        PaintSelectedPosition((float)animatedX, scale);

                        // Animate icon transitions
                        AnimateIcons(range);
                    },
                    () =>
                    {
                        apply = false;
                        var index = 0;
                        foreach (SkiaControl icon in IndicatorsSelected)
                        {
                            icon.IsVisible = false;
                            index++;
                        }
                    },
                    speed, easingAnimation, cancel);

                if (apply)
                    PaintSelectedIndex();

                IsAnimating = false;
            });

        }

        void AnimateIcons(double range)
        {
            if (_currentButtonIcon == null || _nextButtonIcon == null) return;

            // Animation parameters
            float startY = 0; // Starting Y position (center of FAB or track, TranslationY = 0)
            float endY = ButtonSize / 2; // Half the button height for movement (positive for down, negative for up)
            float opacityStart = 1.0f; // Fully visible
            float opacityEnd = 0.0f; // Fully transparent

            // Adapt ranges for flexible StartAnimationRange and EndAnimationRange breakpoints
            double startRange, endRange;

            // Starting phase: 0.0 to StartAnimationRange (outgoing animations)
            if (range <= StartAnimationRange)
            {
                startRange = range / StartAnimationRange; // Scale 0.0–StartAnimationRange to 0.0–1.0
                endRange = 0; // No end animation yet
            }
            // Middle phase: StartAnimationRange to EndAnimationRange (pause or transition, no animation changes)
            else if (range <= EndAnimationRange)
            {
                startRange = 1; // Starting animations complete
                endRange = 0; // End animations not started yet
            }
            // Ending phase: EndAnimationRange to 1.0 (incoming animations)
            else
            {
                startRange = 1; // Starting animations complete
                endRange = (range - EndAnimationRange) / (1.0 - EndAnimationRange); // Scale EndAnimationRange–1.0 to 0.0–1.0
            }

            // Cap ranges at 1.0 to prevent overshoot
            startRange = Math.Min(startRange, 1.0);
            endRange = Math.Min(endRange, 1.0);

            //TRACK
            // For the now unselected icon in the track (fading in, moving from top to bottom)
            if (_previousUnselectedIndicator != null)
            {
                var unselectedIcon = _previousUnselectedIndicator;
                if (unselectedIcon != null)
                {
                    var unselectedIconY = -(endY - (endY - startY) * startRange); // Move up from top to bottom (negative to 0)
                    var unselectedIconOpacity = (opacityStart - opacityEnd) * startRange; // Fade in from 0 to 1

                    // Apply transformations
                    unselectedIcon.TranslationY = unselectedIconY; // Negative values for upward movement, then to 0
                    unselectedIcon.Opacity = unselectedIconOpacity;
                }
            }

            //TRACK
            // For the selected icon in the track (fading out, moving from bottom to top)
            var selectedIcon = IndicatorsUnselected[SelectedIndex];
            if (selectedIcon != null)
            {
                selectedIcon.IsVisible = true;
                // Move from bottom (0) to top (-ButtonWidth / 2), fading out
                var selectedIconY = -(startY + (endY - startY) * endRange); 
                var selectedIconOpacity = opacityStart - (opacityStart - opacityEnd) * endRange; // Fade out from 1 to 0

                // Apply transformations
                selectedIcon.TranslationY = selectedIconY; // Negative values for upward movement
                selectedIcon.Opacity = selectedIconOpacity;
            }

            //BTN

            // For the current icon (fading out, moving down)
            var currentIconY = startY + (endY - startY) * startRange; // Move down (positive TranslationY)
            var currentIconOpacity = opacityStart - (opacityStart - opacityEnd) * startRange; // Fade out
            _currentButtonIcon.TranslationY = currentIconY; // Positive for downward movement
            _currentButtonIcon.Opacity = currentIconOpacity;

            //BTN
            // For the new icon (fading in, moving up from bottom) – prepare for the end
            if (_nextButtonIcon != null)
            {
                _nextButtonIcon.IsVisible = true;
                var nextIconY = (endY - (endY - startY) * endRange); // Move up from -ButtonWidth / 2 to 0 (negative to 0)
                if (nextIconY < 0)
                {
                    nextIconY = 0; // Ensure TranslationY doesn’t go below 0
                }
                var nextIconOpacity = (opacityStart - opacityEnd) * endRange; // Fade in
                _nextButtonIcon.TranslationY = nextIconY; // Negative for upward movement
                _nextButtonIcon.Opacity = nextIconOpacity;
            }
        }

        #endregion

        /// <summary>
        /// After tabs are changed by developer we will set em up
        /// </summary>
        void SetupTabs()
        {

            if (TabsCount >= 0)
            {
                if (SelectedIndex < 0)
                    SelectedIndex = 0;
            }

            int index = 0;
            _grid.ClearChildren();
            foreach (SkiaControl unselectedTab in IndicatorsUnselected)
            {
                unselectedTab.HorizontalOptions = LayoutOptions.Center;
                unselectedTab.VerticalOptions = LayoutOptions.Center;
                _grid.AddSubView(unselectedTab.WithColumn(index));
                index++;
            }

            index = 0;
            _button.Container.ClearChildren();
            foreach (SkiaControl selectedTab in IndicatorsSelected)
            {
                selectedTab.IsVisible = index == SelectedIndex;
                _button.AddIcon(selectedTab);
                index++;
            }
        }

        /// <summary>
        /// This is what paints everything according to (animated) horizontal offset etc
        /// </summary>
        void PaintSelectedIndex()
        {
            if (SelectedIndex < 0 || TabsCount < 1)
            {
                return;
            }

            this.AllowCaching = true;

            var area = (DrawingRect.Width - CircleTabsShape.SidePadding * RenderingScale * 2) / TabsCount;
            var selectedArea = area * SelectedIndex;
            var middle = area / 2;
            var buttonMiddle = CircleTabsShape.SidePadding*RenderingScale + selectedArea + middle;

            SetupUnselected();
            SetupSelected();

            PaintSelectedPosition(buttonMiddle, RenderingScale);

            PreviousSelectedIndex = SelectedIndex;
        }

        protected int PreviousSelectedIndex = -1;
        private SkiaControl _previousUnselectedIndicator;

        /// <summary>
        /// Executes after the animation or instead of it if not animated
        /// </summary>
        void SetupUnselected()
        {
            var index = 0;
            foreach (SkiaControl unselectedTab in IndicatorsUnselected)
            {
                unselectedTab.HorizontalOptions = LayoutOptions.Fill;
                unselectedTab.Opacity = SelectedIndex != index ? 1 : 0;
                unselectedTab.TranslationY = 0;
                unselectedTab.HorizontalFillRatio = 1 / (double)TabsCount;
                unselectedTab.HorizontalPositionOffsetRatio = index;
                index++;
            }
        }

        void SetupSelected()
        {
            var index = 0;
            foreach (SkiaControl icon in IndicatorsSelected)
            {
                if (index == SelectedIndex)
                {
                    icon.Opacity = 1;
                    icon.TranslationY = 0;
                    icon.IsVisible = true;
                }
                else
                {
                    icon.IsVisible = false;
                }
                index++;
            }
        }

        void PaintSelectedPosition(float buttonMiddlePixels, float scale)
        {
            var buttonLeftPoints = buttonMiddlePixels / scale - ButtonSize / 2;

            _button.TranslationX = buttonLeftPoints;
            _track.SetCutoutMiddleX(buttonMiddlePixels);

            _lastButtonMiddle = buttonMiddlePixels;
        }


        #region PROPERTIES

        public static readonly BindableProperty ButtonSizeProperty = BindableProperty.Create(
            nameof(ButtonSize),
            typeof(float),
            typeof(CircleTabs),
            defaultValue: 90.0f);

        public static readonly BindableProperty ButtonPaddingProperty = BindableProperty.Create(
            nameof(ButtonPadding),
            typeof(float),
            typeof(CircleTabs),
            defaultValue: 10.0f);

        public static readonly BindableProperty ButtonColorProperty = BindableProperty.Create(
                nameof(ButtonColor),
                typeof(Color),
                typeof(CircleTabs),
                Colors.Red);

        public static readonly BindableProperty BarColorProperty = BindableProperty.Create(
                nameof(BarColor),
                typeof(Color),
                typeof(CircleTabs),
                Colors.Red );

        public static readonly BindableProperty UnselectedColorProperty = BindableProperty.Create(
                nameof(UnselectedColor),
                typeof(Color),
                typeof(CircleTabs),
                Colors.White);

        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
                nameof(SelectedColor),
                typeof(Color),
                typeof(CircleTabs),
                Colors.White);

        /// <summary>
        /// Diameter of the moving button
        /// </summary>
        public float ButtonSize
        {
            get => (float)GetValue(ButtonSizeProperty);
            set => SetValue(ButtonSizeProperty, value);
        }

        /// <summary>
        /// Padding around the moving button
        /// </summary>
        public float ButtonPadding
        {
            get => (float)GetValue(ButtonPaddingProperty);
            set => SetValue(ButtonPaddingProperty, value);
        }
        
        /// <summary>
        /// Color of the track bar
        /// </summary>
        public Color BarColor
        {
            get => (Color)GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }

        /// <summary>
        /// Color of the moving button
        /// </summary>
        public Color ButtonColor
        {
            get => (Color)GetValue(ButtonColorProperty);
            set => SetValue(ButtonColorProperty, value);
        }

        /// <summary>
        /// Color ot the unselected tabs icons and text
        /// </summary>
        public Color UnselectedColor
        {
            get => (Color)GetValue(UnselectedColorProperty);
            set => SetValue(UnselectedColorProperty, value);
        }

        /// <summary>
        /// Color of selected tabs icons and text 
        /// </summary>
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        #endregion

    }
}
