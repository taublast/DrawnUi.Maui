using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DrawnUi.Maui.Infrastructure.Helpers;


namespace DrawnUi.Maui.Draw
{
    [ContentProperty("Content")]
    public partial class SkiaScroll : SkiaControl, ISkiaGestureListener, IDefinesViewport
    {
        /// <summary>
        /// Min velocity in points/sec to flee/swipe when finger is up
        /// </summary>
        public static float ThesholdSwipeOnUp = 40f;

        /// <summary>
        /// To filter micro-gestures while manually panning
        /// </summary>
        public static float ScrollVelocityThreshold = 20;

        /// <summary>
        /// Time for the snapping animations as well as the scroll to top etc animations..
        /// </summary>
        public static float SystemAnimationTimeSecs = 0.2f;

        /// <summary>
        /// TODO impement this
        /// </summary>
        public enum ScrollingInteractionState
        {
            None,
            Dragging,
            Scrolling,
            Zooming
        }

        public override void OnWillDisposeWithChildren()
        {
            base.OnWillDisposeWithChildren();

            Content?.Dispose();
            Header?.Dispose();
            Footer?.Dispose();
        }

        private ScrollingInteractionState _intercationState;
        public ScrollingInteractionState InteractionState
        {
            get
            {
                return _intercationState;
            }
            set
            {
                if (_intercationState != value)
                {
                    _intercationState = value;
                    OnPropertyChanged();
                }
            }
        }


        public virtual void UpdateVisibleIndex()
        {
            if (LayoutReady && TrackIndexPosition != RelativePositionType.None)
            {
                CurrentIndexHit = CalculateVisibleIndex(TrackIndexPosition);
                CurrentIndex = CurrentIndexHit.Index;
            }
        }

        #region Scrollers


        public bool HasContentToScroll
        {
            get
            {
                return _hasContentToScroll;
            }

            set
            {
                if (_hasContentToScroll != value)
                {
                    _hasContentToScroll = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _hasContentToScroll;


        public static readonly BindableProperty HeaderStickyProperty = BindableProperty.Create(
         nameof(HeaderSticky),
         typeof(bool),
         typeof(SkiaScroll),
         false, propertyChanged: NeedInvalidateMeasure);

        /// <summary>
        /// Should the header stay in place when content is scrolling
        /// </summary>
        public bool HeaderSticky
        {
            get { return (bool)GetValue(HeaderStickyProperty); }
            set { SetValue(HeaderStickyProperty, value); }
        }


        public static readonly BindableProperty ParallaxOverscrollEnabledProperty = BindableProperty.Create(
            nameof(ParallaxOverscrollEnabled),
            typeof(bool),
            typeof(SkiaScroll),
            true, propertyChanged: NeedInvalidateMeasure);

        public bool ParallaxOverscrollEnabled
        {
            get { return (bool)GetValue(ParallaxOverscrollEnabledProperty); }
            set { SetValue(ParallaxOverscrollEnabledProperty, value); }
        }

        public static readonly BindableProperty HeaderBehindProperty = BindableProperty.Create(
            nameof(HeaderBehind),
            typeof(bool),
            typeof(SkiaScroll),
            false, propertyChanged: NeedInvalidateMeasure);

        public bool HeaderBehind
        {
            get { return (bool)GetValue(HeaderBehindProperty); }
            set { SetValue(HeaderBehindProperty, value); }
        }

        public static readonly BindableProperty ContentOffsetProperty = BindableProperty.Create(
            nameof(ContentOffset),
            typeof(double),
            typeof(SkiaScroll),
            0.0, propertyChanged: NeedDraw);

        public double ContentOffset
        {
            get { return (double)GetValue(ContentOffsetProperty); }
            set { SetValue(ContentOffsetProperty, value); }
        }

        public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
        nameof(Header),
        typeof(SkiaControl),
        typeof(SkiaScroll),
       null, propertyChanged: (b, o, n) =>
       {
           if (b is SkiaScroll control)
           {
               control.SetHeader((SkiaControl)n);
           }
       });

        public SkiaControl Header
        {
            get { return (SkiaControl)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly BindableProperty HeaderParallaxRatioProperty = BindableProperty.Create(
            nameof(HeaderParallaxRatio),
            typeof(double),
            typeof(SkiaScroll),
            1.0, propertyChanged: NeedDraw);

        public double HeaderParallaxRatio
        {
            get { return (double)GetValue(HeaderParallaxRatioProperty); }
            set { SetValue(HeaderParallaxRatioProperty, value); }
        }


        public static readonly BindableProperty FooterProperty = BindableProperty.Create(
        nameof(Footer),
        typeof(SkiaControl),
        typeof(SkiaScroll),
        null, propertyChanged: (b, o, n) =>
        {
            if (b is SkiaScroll control)
            {
                control.SetFooter((SkiaControl)n);
            }
        });

        public SkiaControl Footer
        {
            get { return (SkiaControl)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }


        public static readonly BindableProperty RefreshIndicatorProperty = BindableProperty.Create(nameof(RefreshIndicator),
            typeof(IRefreshIndicator),
            typeof(SkiaScroll),
            null,
            propertyChanged: OnNeedSetRefreshIndicator);

        public IRefreshIndicator RefreshIndicator
        {
            get { return (IRefreshIndicator)GetValue(RefreshIndicatorProperty); }
            set { SetValue(RefreshIndicatorProperty, value); }
        }

        private static void OnNeedSetRefreshIndicator(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaScroll control)
            {
                control.SetRefreshIndicator(newvalue as IRefreshIndicator);
            }
        }

        protected IRefreshIndicator InternalRefreshIndicator { get; set; }

        private void SetRefreshIndicator(IRefreshIndicator indicator)
        {
            //delete existing from Views
            //and dispose
            if (InternalRefreshIndicator is SkiaControl control)
            {
                control.SetParent(null);
                control.Dispose();
            }

            //set props for the new one and and it to views
            if (indicator is SkiaControl newControl)
            {
                InternalRefreshIndicator = indicator;

                if (Orientation == ScrollOrientation.Vertical)
                {
                    newControl.HeightRequest = RefreshDistanceLimit;
                }
                else
                if (Orientation == ScrollOrientation.Horizontal)
                {
                    newControl.WidthRequest = RefreshDistanceLimit;
                }
                newControl.ZIndex = 1000;
                AddSubView(newControl);
            }
        }

        private static void NeedToScroll(BindableObject bindable, object oldvalue, object newvalue)
        {
            if ((int)newvalue >= 0 && bindable is SkiaScroll scroll)
            {
                scroll.ScrollToIndex(index: (int)newvalue,
                    scroll.OrderedScrollIsAnimated,
                    scroll.TrackIndexPosition);
                scroll.OrderedScroll = -1;
            }
        }

        public static readonly BindableProperty OrderedScrollProperty = BindableProperty.Create(nameof(OrderedScroll),
            typeof(int),
            typeof(SkiaScroll), -1, BindingMode.TwoWay, propertyChanged: NeedToScroll);

        public int OrderedScroll
        {
            get { return (int)GetValue(OrderedScrollProperty); }
            set { SetValue(OrderedScrollProperty, value); }
        }

        public static readonly BindableProperty OrderedScrollIsAnimatedProperty = BindableProperty.Create(nameof(OrderedScrollIsAnimated),
            typeof(bool),
            typeof(SkiaScroll), false);

        public bool OrderedScrollIsAnimated
        {
            get { return (bool)GetValue(OrderedScrollIsAnimatedProperty); }
            set { SetValue(OrderedScrollIsAnimatedProperty, value); }
        }

        public static readonly BindableProperty RefreshEnabledProperty = BindableProperty.Create(nameof(RefreshEnabled),
            typeof(bool),
            typeof(SkiaScroll),
            false);
        public bool RefreshEnabled
        {
            get { return (bool)GetValue(RefreshEnabledProperty); }
            set { SetValue(RefreshEnabledProperty, value); }
        }

        public static readonly BindableProperty IsRefreshingProperty = BindableProperty.Create(nameof(IsRefreshing),
        typeof(bool),
        typeof(SkiaScroll),
        false,
        propertyChanged: (bindable, old, changed) =>
        {
            if (bindable is SkiaScroll scroll)
            {
                try
                {
                    scroll.SetIsRefreshing((bool)changed);
                }
                catch (Exception e)
                {
                    Super.Log(e);
                }
            }
        });
        public bool IsRefreshing
        {
            get { return (bool)GetValue(IsRefreshingProperty); }
            set { SetValue(IsRefreshingProperty, value); }
        }


        public static readonly BindableProperty RefreshCommandProperty = BindableProperty.Create(nameof(RefreshCommand),
        typeof(ICommand),
        typeof(SkiaScroll),
        null);
        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }


        public static readonly BindableProperty RefreshDistanceLimitProperty = BindableProperty.Create(nameof(RefreshDistanceLimit),
        typeof(float),
        typeof(SkiaScroll),
        150f);
        /// <summary>
        /// Applyed to RefreshView
        /// </summary>
        public float RefreshDistanceLimit
        {
            get { return (float)GetValue(RefreshDistanceLimitProperty); }
            set { SetValue(RefreshDistanceLimitProperty, value); }
        }

        public static readonly BindableProperty RefreshShowDistanceProperty = BindableProperty.Create(
            nameof(RefreshShowDistance),
            typeof(float),
            typeof(SkiaScroll),
            50f);

        /// <summary>
        /// Applyed to RefreshView
        /// </summary>
        public float RefreshShowDistance
        {
            get { return (float)GetValue(RefreshShowDistanceProperty); }
            set { SetValue(RefreshShowDistanceProperty, value); }
        }


        public Easing ScrollingEasing = Easing.SpringOut;

        private readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(10);
        private float filterFactor = 0.99f; //   (0 to 1)

        protected float _velocitySwipe = 200; //pts
        protected float _velocitySwipeRatio = 1.0f;

        protected Vector2 _panningLastDelta;
        protected Vector2 _panningCurrentOffsetPts;
        private Vector2 _panningStartOffsetPts;

        public async void PlayEdgeGlowAnimation(Color color, double x, double y, bool removePrevious = true)
        {
            if (removePrevious)
            {
                UnregisterAllAnimatorsByType(typeof(EdgeGlowAnimator));
            }
            var animation = new EdgeGlowAnimator(this)
            {
                GlowPosition = GlowPosition.Top,
                Color = color.ToSKColor(),
                X = x,
                Y = y,
            };
            animation.Start();
        }

        /// <summary>
        /// Units
        /// </summary>
        public Vector2 OverscrollDistance
        {
            get
            {
                return _overscrollDistance;
            }

            set
            {
                if (_overscrollDistance != value)
                {
                    //if (_rubberBandDistanceY == 0 && value != 0)
                    //{
                    //	//show effect
                    //	PlayEdgeGlowAnimation(Colors.White, 100, 100);
                    //}
                    _overscrollDistance = value;
                    OnPropertyChanged();
                }
            }
        }
        Vector2 _overscrollDistance;



        public bool ScrollLocked
        {
            get
            {
                return _scrollLocked;
            }

            set
            {
                if (_scrollLocked != value)
                {
                    _scrollLocked = value;
                    OnPropertyChanged();
                    //Debug.WriteLine($"[SCROLL] ScrollLocked = {value}");
                }
            }
        }
        bool _scrollLocked;

        //private const float PanTimeThreshold = 10;
        //private DateTimeOffset lastPanTime = DateTimeOffset.Now;

        protected VelocityTracker VelocityTrackerPan = new();

        protected VelocityTracker VelocityTrackerScale = new();

        DateTime lastPanTime;


        /// <summary>
        /// There are the bounds the scroll offset can go to.. This is NOT the bounds for the whole content.
        /// </summary>
        protected SKRect ContentOffsetBounds { get; set; }


        /// <summary>
        /// Used to clamp while panning while finger is down
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //protected virtual Vector2 ClampOffsetWithRubberBand(float x, float y)
        //{
        //    var clampedElastic = RubberBandUtils.ClampOnTrack(new Vector2(x, y), ContentOffsetBounds, (float)RubberEffect);

        //    if (Orientation == ScrollOrientation.Vertical)
        //    {
        //        var clampedX = Math.Max(ContentOffsetBounds.Left, Math.Min(ContentOffsetBounds.Right, x));
        //        return clampedElastic with { X = clampedX };
        //    }
        //    else
        //    if (Orientation == ScrollOrientation.Horizontal)
        //    {
        //        var clampedY = Math.Max(ContentOffsetBounds.Top, Math.Min(ContentOffsetBounds.Bottom, y));
        //        return clampedElastic with { Y = clampedY };
        //    }


        //    return clampedElastic;
        //}
        protected virtual Vector2 ClampOffsetWithRubberBand(float x, float y)
        {
            Vector2 clampedElastic = Vector2.Zero;
            var add = Elastic * RenderingScale;

            bool clamped = false;
            if (RefreshEnabled)
            {
                add += RefreshDistanceLimit * RenderingScale * 10;

                if (Orientation == ScrollOrientation.Vertical && y > 0) //pulling down
                {
                    clamped = true;
                    float adjusted = (float)(RefreshIndicator.Height * RenderingScale + add);
                    var customDims = new Vector2(ContentOffsetBounds.Width, adjusted);
                    clampedElastic = RubberBandUtils.ClampOnTrack(
                        new Vector2(x, y),
                        ContentOffsetBounds,
                        (float)RubberEffect,
                        customDims
                    );
                }
                else
                if (Orientation == ScrollOrientation.Horizontal && x > 0)//pulling right
                {
                    clamped = true;
                    float adjusted = (float)(RefreshIndicator.Width * RenderingScale + add);
                    var customDims = new Vector2(adjusted, ContentOffsetBounds.Height);

                    clampedElastic = RubberBandUtils.ClampOnTrack(
                        new Vector2(x, y),
                        ContentOffsetBounds,
                        (float)RubberEffect,
                        customDims
                    );
                }

            }

            if (!clamped)
            {
                clampedElastic = RubberBandUtils.ClampOnTrack(
                    new Vector2(x, y),
                    ContentOffsetBounds,
                    (float)RubberEffect,
                    new Vector2(add, add)
                );
            }

            // Preserve the clamping in the non-scrolling direction
            if (Orientation == ScrollOrientation.Vertical)
            {
                var clampedX = Math.Max(ContentOffsetBounds.Left, Math.Min(ContentOffsetBounds.Right, x));
                return clampedElastic with { X = clampedX };
            }
            if (Orientation == ScrollOrientation.Horizontal)
            {
                var clampedY = Math.Max(ContentOffsetBounds.Top, Math.Min(ContentOffsetBounds.Bottom, y));
                return clampedElastic with { Y = clampedY };
            }

            return clampedElastic;
        }

        public static int Elastic = 100;

        public virtual Vector2 ClampOffset(float x, float y, bool strict = false)
        {
            if (!Bounces || strict)
            {
                var clampedX = Math.Max(ContentOffsetBounds.Left, Math.Min(ContentOffsetBounds.Right, x));
                var clampedY = Math.Max(ContentOffsetBounds.Top, Math.Min(ContentOffsetBounds.Bottom, y));

                return new Vector2(clampedX, clampedY);
            }

            return ClampOffsetWithRubberBand(x, y);
        }

        public static readonly BindableProperty RespondsToGesturesProperty = BindableProperty.Create(
            nameof(RespondsToGestures),
            typeof(bool),
            typeof(SkiaScroll),
            true);

        /// <summary>
        /// If disabled will not scroll using gestures. Scrolling will still be possible by code.
        /// </summary>
        public bool RespondsToGestures
        {
            get { return (bool)GetValue(RespondsToGesturesProperty); }
            set { SetValue(RespondsToGesturesProperty, value); }
        }


        public static readonly BindableProperty CanScrollUsingHeaderProperty = BindableProperty.Create(
            nameof(CanScrollUsingHeader),
            typeof(bool),
            typeof(SkiaScroll),
            true);

        /// <summary>
        /// If disabled will not scroll using gestures. Scrolling will still be possible by code.
        /// </summary>
        public bool CanScrollUsingHeader
        {
            get { return (bool)GetValue(CanScrollUsingHeaderProperty); }
            set { SetValue(CanScrollUsingHeaderProperty, value); }
        }

        protected bool ContentGesturesHit;

        public override bool IsGestureForChild(ISkiaGestureListener listener, float x, float y)
        {
            if (ContentGesturesHit
                && HeaderBehind && listener == Header)
            {
                return false; //do not pass gestures to header
            }

            return base.IsGestureForChild(listener, x, y);
        }

        protected bool ChildWasPanning { get; set; }

        protected bool ChildWasTapped { get; set; }


        protected bool IsContentActive
        {
            get
            {
                return Content != null && Content.IsVisible;
            }
        }


        protected VelocityAccumulator VelocityAccumulator { get; } = new();

        int lastNumberOfTouches;

        protected virtual void ResetPan()
        {
            //Trace.WriteLine("[SCROLL] Pan reset!");
            ChildWasTapped = false;
            WasSwiping = false;
            IsUserFocused = true;
            IsUserPanning = false;
            ChildWasPanning = false;
            ChildWasTapped = false;

            StopScrolling();

            VelocityAccumulator.Clear();

            _panningLastDelta = Vector2.Zero;
            _panningStartOffsetPts = new(ViewportOffsetX, ViewportOffsetY);
            _panningCurrentOffsetPts = _panningStartOffsetPts;
        }


        private bool lockHeader;

        public override bool UsesRenderingTree => false;

        public override bool IsGestureForChild(SkiaControlWithRect child, SKPoint point)
        {
            if (lockHeader && child.Control != Header)
            {
                return false;
            }

            var forChild = base.IsGestureForChild(child, point);
            if (!HeaderBehind && Header != null)
            {
                //block gestures for other children if from header got them
                if (child.Control == this.Header && forChild)
                {
                    lockHeader = true;
                }
            }
            return forChild;
        }

        public override bool IsGestureForChild(ISkiaGestureListener listener, SKPoint point)
        {
            if (lockHeader && listener != Header)
            {
                return false;
            }

            var forChild = base.IsGestureForChild(listener, point);
            if (!HeaderBehind && Header != null)
            {
                //block gestures for other children if from header got them
                if (listener == this.Header && forChild)
                {
                    lockHeader = true;
                }
            }
            return forChild;
        }

        private bool inContact;

        protected bool LockGesturesUntilDown;

        public override ISkiaGestureListener ProcessGestures(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
        {
            if (LockGesturesUntilDown)
            {
                if (args.Type != TouchActionResult.Down)
                    return null;

                LockGesturesUntilDown = false;
            }

            if (args.Type == TouchActionResult.Down)
            {
                lockHeader = false;
                inContact = true;
            }
            else
            if (args.Type == TouchActionResult.Up)
            {
                lockHeader = false;
                inContact = false;
            }

            var preciseStop = false;
            ContentGesturesHit = false;
            var thisOffset = TranslateInputCoords(apply.childOffset);
            if (Content != null && Header != null)
            {
                var x = args.Event.Location.X + thisOffset.X;
                var y = args.Event.Location.Y + thisOffset.Y;

                ContentGesturesHit = Content.HitIsInside(x, y);
            }

            if (TouchEffect.LogEnabled)
            {
                Super.Log($"[SCROLL] {this.Tag} Got {args.Type} touches {args.Event.NumberOfTouches} {VelocityY}..");
            }

            if (args.Type == TouchActionResult.Down && RespondsToGestures)
            {
                ResetPan();
            }

            bool passedToChildren = false;
            ISkiaGestureListener PassToChildren()
            {
                passedToChildren = true;
                return base.ProcessGestures(args, apply);
            }


            bool wrongDirection = false;

            if (args.Type == TouchActionResult.Panning)
            {
                if (args.Event.Manipulation != null && !ZoomLocked) //todo not only panning?
                {
                    IsUserFocused = true;
                    var scale = args.Event.Manipulation.Scale + this.ViewportZoom;
                    Debug.WriteLine($"Scale: {scale}");
                    var zoomed = SetZoom(scale);
                }


                if (IgnoreWrongDirection)
                {
                    var panDirection = DirectionType.Vertical;
                    if (Math.Abs(args.Event.Distance.Delta.X) > Math.Abs(args.Event.Distance.Delta.Y))
                    {
                        panDirection = DirectionType.Horizontal;
                    }
                    if (Orientation == ScrollOrientation.Vertical && panDirection != DirectionType.Vertical)
                    {
                        wrongDirection = true;
                    }
                    if (Orientation == ScrollOrientation.Horizontal && panDirection != DirectionType.Horizontal)
                    {
                        wrongDirection = true;
                    }
                }
            }


            if (!IsUserPanning || wrongDirection || args.Type == TouchActionResult.Up || args.Type == TouchActionResult.Tapped || !RespondsToGestures)
            {
                var childConsumed = PassToChildren();
                if (childConsumed != null)
                {
                    if (args.Type == TouchActionResult.Panning)
                    {
                        ChildWasPanning = true;
                    }
                    else if (args.Type == TouchActionResult.Tapped)
                    {
                        ChildWasTapped = true;
                    }
                    if (args.Type != TouchActionResult.Up)
                        return childConsumed;
                }
                else
                {
                    ChildWasPanning = false;
                }
            }

            ISkiaGestureListener consumed = null;
            if (Orientation == ScrollOrientation.Vertical || Orientation == ScrollOrientation.Both)
            {
                VelocityY = (float)(args.Event.Distance.Velocity.Y / RenderingScale);
            }
            if (Orientation == ScrollOrientation.Horizontal || Orientation == ScrollOrientation.Both)
            {
                VelocityX = (float)(args.Event.Distance.Velocity.X / RenderingScale);
            }

            var hadNumberOfTouches = lastNumberOfTouches;
            lastNumberOfTouches = args.Event.NumberOfTouches;

            if (args.Type == TouchActionResult.Wheel && !ZoomLocked)
            {
                IsUserFocused = true;
                Debug.WriteLine($"Wheel: {args.Event.Wheel.Scale}");
                var zoomed = SetZoom(args.Event.Wheel.Scale);
                consumed = this;
            }
            else if (args.Event.NumberOfTouches < 2 && hadNumberOfTouches < 2)
            {
                switch (args.Type)
                {
                    case TouchActionResult.Tapped:
                    case TouchActionResult.LongPressing:
                        if (!passedToChildren)
                        {
                            _panningStartOffsetPts = new(InternalViewportOffset.Units.X, InternalViewportOffset.Units.Y);
                            consumed = PassToChildren();
                        }
                        break;

                    case TouchActionResult.Panning when RespondsToGestures:
                        bool canPan = !ScrollLocked;
                        if (Orientation == ScrollOrientation.Vertical)
                        {
                            canPan &= Math.Abs(VelocityY) > ScrollVelocityThreshold;
                        }
                        else if (Orientation == ScrollOrientation.Horizontal)
                        {
                            canPan &= Math.Abs(VelocityX) > ScrollVelocityThreshold;
                        }
                        else if (Orientation == ScrollOrientation.Both)
                        {
                            canPan &= Math.Abs(VelocityX) > ScrollVelocityThreshold || Math.Abs(VelocityY) > ScrollVelocityThreshold;
                        }

                        if (lockHeader && !CanScrollUsingHeader)
                        {
                            canPan = false;
                        }

                        if (canPan)
                        {
                            bool checkOverscroll = true;

                            if (!IsUserFocused)
                            {
                                ResetPan();
                                checkOverscroll = false;
                            }

                            IsUserPanning = true;

                            var movedPtsY = (args.Event.Distance.Delta.Y / RenderingScale) * ChangeDIstancePanned;
                            var movedPtsX = (args.Event.Distance.Delta.X / RenderingScale) * ChangeDIstancePanned;

                            var interpolatedMoveToX = _panningLastDelta.X + (movedPtsX - _panningLastDelta.X) * 0.22f;
                            var interpolatedMoveToY = _panningLastDelta.Y + (movedPtsY - _panningLastDelta.Y) * 0.22f;

                            _panningLastDelta = new Vector2(interpolatedMoveToX, interpolatedMoveToY);

                            var moveTo = new Vector2(
                                (float)Math.Round(_panningCurrentOffsetPts.X + interpolatedMoveToX),
                                (float)Math.Round(_panningCurrentOffsetPts.Y + interpolatedMoveToY));

                            _panningCurrentOffsetPts = moveTo;

                            if (IgnoreWrongDirection && wrongDirection)
                            {
                                IsUserPanning = false;
                                IsUserFocused = false;
                                return null;
                            }

                            VelocityAccumulator.CaptureVelocity(new(VelocityX, VelocityY));

                            var clamped = ClampOffset(moveTo.X, moveTo.Y);

                            if (!Bounces && checkOverscroll)
                            {
                                if (!AreEqual(clamped.X, moveTo.X, 1) && !AreEqual(clamped.Y, moveTo.Y, 1))
                                {
                                    return null;
                                }
                            }

                            ViewportOffsetX = clamped.X;
                            ViewportOffsetY = clamped.Y;

                            IsUserPanning = true;
                            _lastVelocity = new Vector2(VelocityX, VelocityY);

                            consumed = this;
                        }
                        break;

                    case TouchActionResult.Up when RespondsToGestures:
                        if ((!ChildWasTapped || OverScrolled) && (!ChildWasPanning || IsUserPanning))
                        {
                            if (apply.alreadyConsumed != null)
                            {
                                if (CheckNeedToSnap())
                                    Snap(SystemAnimationTimeSecs);
                                return null;
                            }

                            bool canSwipe = true;
                            if (lockHeader && !CanScrollUsingHeader)
                            {
                                canSwipe = false;
                            }

                            if (!ScrollLocked && canSwipe)
                            {
                                var finalVelocity = VelocityAccumulator.CalculateFinalVelocity(this.MaxVelocity);

                                bool fling = false;
                                bool swipe = false;

                                if (!OverScrolled || Orientation == ScrollOrientation.Both)
                                {
                                    var mainDirection = GetDirectionType(new Vector2(finalVelocity.X, finalVelocity.Y), DirectionType.None, 0.9f);

                                    if (Orientation != ScrollOrientation.Both && !IsUserPanning)
                                    {
                                        if (IgnoreWrongDirection)
                                        {
                                            if (Orientation == ScrollOrientation.Vertical && mainDirection != DirectionType.Vertical)
                                            {
                                                return null;
                                            }
                                            if (Orientation == ScrollOrientation.Horizontal && mainDirection != DirectionType.Horizontal)
                                            {
                                                return null;
                                            }
                                        }
                                    }

                                    var swipeThreshold = ThesholdSwipeOnUp * RenderingScale;
                                    if ((Orientation == ScrollOrientation.Both
                                        || (Orientation == ScrollOrientation.Vertical && mainDirection == DirectionType.Vertical)
                                        || (Orientation == ScrollOrientation.Horizontal && mainDirection == DirectionType.Horizontal))
                                        && (Math.Abs(finalVelocity.X) > swipeThreshold || Math.Abs(finalVelocity.Y) > swipeThreshold))
                                    {
                                        swipe = true;
                                    }
                                }

                                if (OverScrolled || swipe)
                                {
                                    IsUserPanning = false;

                                    bool bounceX = false, bounceY = false;
                                    if (OverScrolled)
                                    {
                                        var contentRect = new SKRect(0, 0, ptsContentWidth, ptsContentHeight);
                                        var closestPoint = GetClosestSidePoint(new SKPoint((float)InternalViewportOffset.Units.X, (float)InternalViewportOffset.Units.Y), contentRect, Viewport.Units.Size);

                                        var axis = new Vector2(closestPoint.X, closestPoint.Y);

                                        var velocityY = finalVelocity.Y * ChangeVelocityScrolled;
                                        var velocityX = finalVelocity.X * ChangeVelocityScrolled;

                                        if (Math.Abs(velocityX) > MaxBounceVelocity)
                                        {
                                            velocityX = Math.Sign(velocityX) * MaxBounceVelocity;
                                        }

                                        if (Math.Abs(velocityY) > MaxBounceVelocity)
                                        {
                                            velocityY = Math.Sign(velocityY) * MaxBounceVelocity;
                                        }

                                        if (OverscrollDistance.Y != 0)
                                        {
                                            bounceY = true;
                                            BounceY(InternalViewportOffset.Units.Y,
                                                axis.Y, velocityY);
                                        }

                                        if (OverscrollDistance.X != 0)
                                        {
                                            bounceX = true;
                                            BounceX(InternalViewportOffset.Units.X,
                                                axis.X, velocityX);
                                        }

                                        fling = true;
                                    }

                                    if (Orientation != ScrollOrientation.Neither)
                                    {
                                        if (!Bounces)
                                        {
                                            if (Orientation == ScrollOrientation.Vertical && !bounceY)
                                            {
                                                if ((AreEqual(InternalViewportOffset.Pixels.Y, ContentOffsetBounds.Bottom, 1) && finalVelocity.Y > 0) ||
                                                (AreEqual(InternalViewportOffset.Pixels.Y, ContentOffsetBounds.Top, 1) && finalVelocity.Y < 0))
                                                    return null;
                                            }
                                            if (Orientation == ScrollOrientation.Horizontal && !bounceX)
                                            {
                                                if ((AreEqual(InternalViewportOffset.Pixels.X, ContentOffsetBounds.Right, 1) && finalVelocity.X > 0) ||
                                                 (AreEqual(InternalViewportOffset.Pixels.X, ContentOffsetBounds.Left, 1) && finalVelocity.X < 0))
                                                    return null;
                                            }
                                        }

                                        var velocityY = finalVelocity.Y * ChangeVelocityScrolled;
                                        var velocityX = finalVelocity.X * ChangeVelocityScrolled;

                                        if (Math.Abs(velocityX) > _minVelocity && !bounceX)
                                        {
                                            IsUserFocused = false;
                                            _vectorAnimatorBounceX.Stop();
                                            fling = StartToFlingFrom(_animatorFlingX, ViewportOffsetX, velocityX);
                                        }

                                        if (Math.Abs(velocityY) > _minVelocity && !bounceY)
                                        {
                                            IsUserFocused = false;
                                            _vectorAnimatorBounceY.Stop();
                                            fling = StartToFlingFrom(_animatorFlingY, ViewportOffsetY, velocityY);
                                        }
                                    }

                                    if (fling)
                                    {
                                        WasSwiping = true;
                                        consumed = this;
                                        passedToChildren = true;
                                    }
                                }

                                IsUserFocused = false;
                                IsUserPanning = false;

                                if (!fling)
                                {
                                    if (CheckNeedToSnap())
                                        Snap(SystemAnimationTimeSecs);
                                    else
                                    {
                                        _destination = SKRect.Empty;
                                    }
                                }
                            }
                            break;
                        }
                        break;
                }
            }

            if (consumed != null || IsUserPanning)
            {
                return consumed ?? this;
            }

            if (!passedToChildren)
                return PassToChildren();

            return null;
        }


        public virtual bool OnFocusChanged(bool focus)
        {
            return false;
        }


        SpringWithVelocityAnimator _vectorAnimatorBounceX;

        SpringWithVelocityAnimator _vectorAnimatorBounceY;

        /// <summary>
        /// Fling with deceleration
        /// </summary>
        protected ScrollFlingAnimator _animatorFlingX;

        /// <summary>
        /// Fling with deceleration
        /// </summary>
        protected ScrollFlingAnimator _animatorFlingY;

        /// <summary>
        /// Direct scroller for ordered scroll, snap etc
        /// </summary>
        protected RangeAnimator _scrollerX;

        /// <summary>
        /// Direct scroller for ordered scroll, snap etc
        /// </summary>
        protected RangeAnimator _scrollerY;

        /// <summary>
        /// Units
        /// </summary>
        protected float _scrollMinX;

        /// <summary>
        /// Units
        /// </summary>
        protected float _scrollMinY;


        protected float _scrollMaxX;
        protected float _scrollMaxY;

        public void StopScrolling()
        {
            _scrollerX?.Stop();
            _scrollerY?.Stop();

            _animatorFlingX?.Stop();
            _animatorFlingY?.Stop();

            _vectorAnimatorBounceX?.Stop();
            _vectorAnimatorBounceY?.Stop();

            VelocityTrackerPan.Clear();
            VelocityTrackerScale.Clear();

            //ViewportOffsetY = InternalViewportOffset.Units.Y;
            //ViewportOffsetX = InternalViewportOffset.Units.X;
        }

        void UpdateLoadingLock(bool state)
        {
            SkiaImageManager.Instance.IsLoadingLocked = state;
        }

        void UpdateLoadingLock(Vector2 velocity)
        {
            bool shouldLock;

            switch (Orientation)
            {
                case ScrollOrientation.Vertical:
                    shouldLock = Math.Abs(velocity.Y) >= VelocityImageLoaderLock;
                    break;
                case ScrollOrientation.Horizontal:
                    shouldLock = Math.Abs(velocity.X) >= VelocityImageLoaderLock;
                    break;
                default:
                    shouldLock = Math.Abs(velocity.Y) >= VelocityImageLoaderLock || Math.Abs(velocity.X) >= VelocityImageLoaderLock;
                    break;
            }

            UpdateLoadingLock(shouldLock);
        }

        float _minVelocitySnap = 15f;


        /// <summary>
        /// POINTS per sec
        /// </summary>
        private float snapMinimumVelocity = 3f;


        //public virtual bool ScrollStoppedForSnap()
        //{
        //	//if (_velocityScrollerY.IsRunning)
        //	//{
        //	//    return _velocityScrollerY.mVelocity <= snapMinimumVelocity;
        //	//}

        //	if (_animatorFling.IsRunning)
        //	{
        //		return _animatorFling.CurrentVelocity.Y <= snapMinimumVelocity && _animatorFling.CurrentVelocity.X <= snapMinimumVelocity;
        //	}

        //	return !_animatorBounce.IsRunning && !_scrollerX.IsRunning && !_scrollerY.IsRunning;
        //}

        //protected bool CanSnap()
        //{
        //	return (!IsUserFocused
        //		&& SnapToChildren != SnapToChildrenType.Disabled
        //		&& Content is SkiaLayout layout
        //		&& ScrollStoppedForSnap());
        //}



        /// <summary>
        /// ToDo adapt this to same logic as ScrollLooped has !
        /// </summary>
        /// <param name="force"></param>
        //protected virtual void SnapIfNeeded(bool force = false)
        //{
        //	return; //todo

        //	if (force ||
        //		!IsUserFocused
        //		&& SnapToChildren != SnapToChildrenType.Disabled
        //		&& ScrollStoppedForSnap())
        //	{
        //		if (Content is SkiaLayout layout)
        //		{
        //			var hit = CurrentIndexHit;
        //			if (hit?.Index > -1 && layout.Views.Count > hit?.Index)
        //			{
        //				float needOffsetX = (float)Math.Truncate(InternalViewportOffset.Pixels.X);
        //				var initialOffset = needOffsetX;

        //				var calcOffset = NotValidPoint();
        //				if (SnapToChildren == SnapToChildrenType.Center)
        //				{
        //					calcOffset = CalculateScrollOffsetForIndex(hit.Index, RelativePositionType.Center);
        //				}

        //				if (SnapToChildren == SnapToChildrenType.Side)
        //				{
        //					if (TrackIndexPosition == RelativePositionType.Start)
        //					{
        //						calcOffset = CalculateScrollOffsetForIndex(hit.Index, RelativePositionType.Start);
        //					}
        //					else if (TrackIndexPosition == RelativePositionType.End)
        //					{
        //						calcOffset = CalculateScrollOffsetForIndex(hit.Index, RelativePositionType.End);
        //					}
        //				}

        //				if (PointIsValid(calcOffset))
        //				{
        //					if (initialOffset != calcOffset.X)
        //					{
        //						System.Diagnostics.Debug.WriteLine($"[SNAP] ------------ {CurrentIndex}");
        //						ScrollToX(calcOffset.X, true);
        //					}
        //				}


        //			}
        //		}
        //	}
        //}


        public static readonly BindableProperty BouncesProperty = BindableProperty.Create(nameof(Bounces),
        typeof(bool),
        typeof(SkiaScroll),
        true);
        /// <summary>
        /// Should the scroll bounce at edges. Set to false if you want this scroll to let the parent SkiaDrawer respond to scroll when the child scroll reached bounds.
        /// </summary>
        public bool Bounces
        {
            get { return (bool)GetValue(BouncesProperty); }
            set { SetValue(BouncesProperty, value); }
        }

        public static readonly BindableProperty RubberDampingProperty = BindableProperty.Create(
            nameof(RubberDamping),
            typeof(double),
            typeof(SkiaScroll),
            0.55);

        /// <summary>
        /// If Bounce is enabled this basically controls how less the scroll will bounce when displaced from limit by finger or inertia. Default is 0.55.
        /// </summary>
        public double RubberDamping
        {
            get { return (double)GetValue(RubberDampingProperty); }
            set { SetValue(RubberDampingProperty, value); }
        }


        public static readonly BindableProperty RubberEffectProperty = BindableProperty.Create(
            nameof(RubberEffect),
            typeof(double),
            typeof(SkiaScroll),
            0.55);

        /// <summary>
        /// If Bounce is enabled this basically controls how far from the limit can the scroll be elastically offset by finger or inertia. Default is 0.55.
        /// </summary>
        public double RubberEffect
        {
            get { return (double)GetValue(RubberEffectProperty); }
            set { SetValue(RubberEffectProperty, value); }
        }

        public float SnapBouncingIfVelocityLessThan
        {
            get { return (float)GetValue(SnapBouncingIfVelocityLessThanProperty); }
            set { SetValue(SnapBouncingIfVelocityLessThanProperty, value); }
        }
        public static readonly BindableProperty SnapBouncingIfVelocityLessThanProperty = BindableProperty.Create(nameof(SnapBouncingIfVelocityLessThan),
            typeof(float),
            typeof(SkiaScroll),
            750.0f);


        public static readonly BindableProperty AutoScrollingSpeedMsProperty = BindableProperty.Create(nameof(AutoScrollingSpeedMs),
            typeof(int),
            typeof(SkiaScroll),
            600);

        /// <summary>
        /// For snap and ordered scrolling
        /// </summary>
        public int AutoScrollingSpeedMs
        {
            get { return (int)GetValue(AutoScrollingSpeedMsProperty); }
            set { SetValue(AutoScrollingSpeedMsProperty, value); }
        }


        /// <summary>
        /// Use this to control how fast the scroll will decelerate. Values 0.1 - 0.9 are the best, default is 0.3. Usually you would set higher friction for ScrollView-like scrolls and much lower for CollectionView-like scrolls (0.1 or 0.2).
        /// </summary>
        public float FrictionScrolled
        {
            get { return (float)GetValue(FrictionScrolledProperty); }
            set { SetValue(FrictionScrolledProperty, value); }
        }

        public static readonly BindableProperty FrictionScrolledProperty = BindableProperty.Create(nameof(FrictionScrolled),
        typeof(float),
        typeof(SkiaScroll),
        .3f,
        propertyChanged: FrictionValueChanged);


        public static readonly BindableProperty IgnoreWrongDirectionProperty = BindableProperty.Create(
            nameof(IgnoreWrongDirection),
            typeof(bool),
            typeof(SkiaScroll),
            false);

        /// <summary>
        /// Will ignore gestures of the wrong direction, like if this Orientation is Horizontal will ignore gestures with vertical direction velocity. Default is False.
        /// </summary>
        public bool IgnoreWrongDirection
        {
            get { return (bool)GetValue(IgnoreWrongDirectionProperty); }
            set { SetValue(IgnoreWrongDirectionProperty, value); }
        }

        /*
        public static readonly BindableProperty IgnoreWrongDirectionLockProperty = BindableProperty.Create(
            nameof(IgnoreWrongDirectionLock),
            typeof(bool),
            typeof(SkiaScroll),
            false);

        /// <summary>
        /// In case if will ignore gestures of the wrong direction, should we lock this direction or multi-directional scrolling (True) is still allowed (False). Default is False.
        /// </summary>
        public bool IgnoreWrongDirectionLock
        {
            get { return (bool)GetValue(IgnoreWrongDirectionLockProperty); }
            set { SetValue(IgnoreWrongDirectionLockProperty, value); }
        }
        */

        public static readonly BindableProperty ResetScrollPositionOnContentSizeChangedProperty = BindableProperty.Create(
            nameof(ResetScrollPositionOnContentSizeChanged),
            typeof(bool),
            typeof(SkiaScroll),
            false);

        public bool ResetScrollPositionOnContentSizeChanged
        {
            get { return (bool)GetValue(ResetScrollPositionOnContentSizeChangedProperty); }
            set { SetValue(ResetScrollPositionOnContentSizeChangedProperty, value); }
        }


        /// <summary>
        /// For when the finger is up and swipe is detected
        /// </summary>
        public float ChangeVelocityScrolled
        {
            get { return (float)GetValue(ChangeVelocityScrolledProperty); }
            set { SetValue(ChangeVelocityScrolledProperty, value); }
        }
        public static readonly BindableProperty ChangeVelocityScrolledProperty = BindableProperty.Create(nameof(ChangeVelocityScrolled),
            typeof(float),
            typeof(SkiaScroll),
            1.33f);

        public static readonly BindableProperty MaxVelocityProperty = BindableProperty.Create(
            nameof(MaxVelocity),
            typeof(float),
            typeof(SkiaScroll),
            3000f);

        /// <summary>
        /// Limit user input velocity
        /// </summary>
        public float MaxVelocity
        {
            get { return (float)GetValue(MaxVelocityProperty); }
            set { SetValue(MaxVelocityProperty, value); }
        }

        public static readonly BindableProperty MaxBounceVelocityProperty = BindableProperty.Create(
            nameof(MaxBounceVelocity),
            typeof(float),
            typeof(SkiaScroll),
            500f);

        /// <summary>
        /// Limit bounce velocity
        /// </summary>
        public float MaxBounceVelocity
        {
            get { return (float)GetValue(MaxBounceVelocityProperty); }
            set { SetValue(MaxBounceVelocityProperty, value); }
        }

        /// <summary>
        /// For when the finger is down and panning
        /// </summary>
        public float ChangeDIstancePanned
        {
            get { return (float)GetValue(ChangeDIstancePannedProperty); }
            set { SetValue(ChangeDIstancePannedProperty, value); }
        }


        public static readonly BindableProperty ChangeDIstancePannedProperty = BindableProperty.Create(nameof(ChangeDIstancePanned),
            typeof(float),
            typeof(SkiaScroll),
            0.975f);



        private static void FrictionValueChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaScroll control)
            {
                control.UpdateFriction();
            }
        }

        int _currentIndex = -1;
        public int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }
            protected set
            {
                if (_currentIndex != value)
                {
                    _currentIndex = value;
                    OnPropertyChanged();
                    IndexChanged?.Invoke(this, value);
                    //Debug.WriteLine($"Scroll {Tag} CurrentIndex {value}");
                }
            }
        }

        public event EventHandler<int> IndexChanged;

        public ContainsPointResult CurrentIndexHit
        {
            get
            {
                return _CurrentIndexHit;
            }
            set
            {
                if (value != _CurrentIndexHit)
                {
                    _CurrentIndexHit = value;
                    OnPropertyChanged();
                }
            }
        }
        private ContainsPointResult _CurrentIndexHit;

        void WatchState()
        {

        }

        protected SKPoint DetectIndexChildIndexAt;

        void SetDetectIndexChildPoint(RelativePositionType option = RelativePositionType.Start)
        {
            //todo this will need to change for multiple columns?

            if (!IsContentActive || Content.MeasuredSize == null || TrackIndexPosition == RelativePositionType.None)
                return;

            var point = new SKPoint();


            if (this.Orientation == ScrollOrientation.Vertical)
            {
                var endY = this.Viewport.Pixels.Height;
                if (this.Content.MeasuredSize.Pixels.Height < endY)
                    endY = this.Content.MeasuredSize.Pixels.Height;

                if (option == RelativePositionType.End)
                {
                    point.Y += (endY - TrackIndexPositionOffset);
                }
                else
                if (option == RelativePositionType.Center)
                {
                    point.Y += endY / 2f;
                }

                point.X = this.Viewport.Pixels.MidX;
            }
            else
            if (this.Orientation == ScrollOrientation.Horizontal)
            {
                var endX = this.Viewport.Pixels.Width;
                if (this.Content.MeasuredSize.Pixels.Width < endX)
                    endX = this.Content.MeasuredSize.Pixels.Width;

                if (option == RelativePositionType.End)
                {
                    point.X += endX - TrackIndexPositionOffset;
                }
                else
                if (option == RelativePositionType.Center)
                {
                    point.X += endX / 2f;
                }

                point.Y = this.Viewport.Pixels.MidY;
            }

            //Debug.WriteLine($"[POINT] V {Viewport.Pixels.Bottom} P {point.Y}");

            DetectIndexChildIndexAt = point;
        }

        /// <summary>
        /// Calculates CurrentIndex
        /// </summary>
        public virtual ContainsPointResult CalculateVisibleIndex(RelativePositionType option)
        {
            if (Content is SkiaLayout layout)
            {

                var pixelsOffsetX = InternalViewportOffset.Pixels.X;// (float)(ViewportOffsetX * layout.RenderingScale);
                var pixelsOffsetY = InternalViewportOffset.Pixels.Y;// (float)(ViewportOffsetY * layout.RenderingScale);

                return GetItemIndex(layout, pixelsOffsetX, pixelsOffsetY, option);
            }
            else
            if (Content is ILayoutInsideViewport inside)
            {

                var point = new SKPoint(
                    DetectIndexChildIndexAt.X + InternalViewportOffset.Pixels.X + DrawingRect.Left,
                    DetectIndexChildIndexAt.Y + InternalViewportOffset.Pixels.Y + DrawingRect.Top);

                var found = inside.GetVisibleChildIndexAt(point);

                if (found.Index != -1)
                {



                    //todo translate found
                    var area = found.Area;
                    area.Offset(-DrawingRect.Left, -DrawingRect.Top);
                    point.Offset(-DrawingRect.Left, -DrawingRect.Top);
                    return new ContainsPointResult()
                    {
                        Index = found.Index,
                        Area = area,
                        Point = point,
                        Unmodified = new(InternalViewportOffset.Pixels.X, InternalViewportOffset.Pixels.Y)
                    };
                }

                return found;
            }

            return ContainsPointResult.NotFound();
        }

        public virtual ContainsPointResult GetItemIndex(SkiaLayout layout, float pixelsOffsetX, float pixelsOffsetY, RelativePositionType option)
        {
            if (layout.LatestStackStructure == null)
                return ContainsPointResult.NotFound();

            bool trace = false;

            if (this.Orientation == ScrollOrientation.Vertical)
            {
                var initialValue = pixelsOffsetY;

                // ----------- proper to infinite start 

                if (option == RelativePositionType.Center)
                {
                    pixelsOffsetY -= Viewport.Pixels.Height / 2f;
                }
                else
                if (option == RelativePositionType.End)
                {
                    pixelsOffsetY -= Viewport.Pixels.Height;
                }

                if (pixelsOffsetY > 0)
                {
                    //inverted scroll
                    pixelsOffsetY -= Content.MeasuredSize.Pixels.Height;

                }
                else
                {
                    //normal scroll
                    if (-pixelsOffsetY > Content.MeasuredSize.Pixels.Height)
                    {
                        pixelsOffsetY += Content.MeasuredSize.Pixels.Height;
                    }
                }

                // ----------- proper to infinite end

                var point = new SKPoint(
                (float)Math.Abs(pixelsOffsetX),
                (float)Math.Abs(pixelsOffsetY)
                );

                if (layout.Type == LayoutType.Column || layout.Type == LayoutType.Wrap && layout.Split > 0) //todo grid
                {
                    var stackStructure = layout.LatestStackStructure;
                    int index = -1;
                    int row;
                    int col;

                    if (trace)
                        Trace.WriteLine($"offset: {point.Y}");

                    foreach (var childInfo in stackStructure.GetChildren())
                    {
                        index++;
                        if (childInfo.Destination.ContainsInclusive(point))
                        {
                            return new ContainsPointResult()
                            {
                                Index = index,
                                Area = childInfo.Destination,
                                Point = point,
                                Unmodified = new SKPoint(0, initialValue)
                            };
                        }
                    }

                }
            }
            else
            if (this.Orientation == ScrollOrientation.Horizontal)
            {
                var initialValue = pixelsOffsetX;

                // ----------- proper to infinite start 

                if (option == RelativePositionType.Center)
                {
                    pixelsOffsetX -= Viewport.Pixels.Width / 2f;
                }
                else
                if (option == RelativePositionType.End)
                {
                    pixelsOffsetX -= Viewport.Pixels.Width;
                }

                if (pixelsOffsetX > 0)
                {
                    //inverted scroll
                    //var bak = pixelsOffsetX;
                    pixelsOffsetX -= Content.MeasuredSize.Pixels.Width;
                    //Trace.WriteLine($"[INVERT ] {bak:0.0} --> {pixelsOffsetX:0.0}");
                }
                else
                {
                    //normal scroll
                    if (-pixelsOffsetX > Content.MeasuredSize.Pixels.Width)
                    {
                        pixelsOffsetX += Content.MeasuredSize.Pixels.Width;
                    }
                }

                //Trace.WriteLine($"[CALC] for {pixelsOffsetX:0.0}");
                // ----------- proper to infinite end


                var point = new SKPoint(
                (float)Math.Abs(pixelsOffsetX),
                (float)Math.Abs(pixelsOffsetY)
                );


                if (layout.Type == LayoutType.Row || layout.Type == LayoutType.Wrap && layout.Split == 0) //todo grid
                {
                    var stackStructure = layout.StackStructure;
                    int index = -1;
                    int row;
                    int col;

                    foreach (var childInfo in stackStructure.GetChildren())
                    {
                        index++;
                        var childRect = childInfo.Destination.Clone();
                        //childRect.Offset(point.X, point.Y);

                        if (childRect.ContainsInclusive(point))
                        {
                            return new ContainsPointResult()
                            {
                                Index = index,
                                Area = childRect,
                                Point = point,
                                Unmodified = new SKPoint(initialValue, 0)
                            };
                        }
                    }

                }
            }

            return ContainsPointResult.NotFound();
        }


        protected virtual SKPoint ClampedOrderedScrollOffset(SKPoint scrollTo)
        {
            var scrollSpaceY = ptsContentHeight - Viewport.Units.Height;

            var offsetViewport = Math.Abs(scrollTo.Y) - Viewport.Units.Height;

            if (scrollSpaceY < 0 || offsetViewport < 0)
            {
                return NotValidPoint();
            }

            return scrollTo;
        }

        /// <summary>
        /// ToDo this actually work only for Stack and Row
        /// </summary>
        /// <param name="index"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public virtual SKPoint CalculateScrollOffsetForIndex(int index, RelativePositionType option)
        {
            //Debug.WriteLine($"CalculateScrollOffsetForIndex ? {index}");

            if (Content is SkiaLayout layout)
            {
                var childrenCount = layout.ChildrenFactory.GetChildrenCount();
                if (
                    ptsContentHeight <= 0 || ptsContentWidth <= 0 ||
                    childrenCount == 0 || index < 0 || index >= childrenCount)
                {
                    return NotValidPoint(); //can throw too
                }

                var structure = layout.LatestStackStructure;
                if (structure != null && structure.GetCount() > 0)// && layout.StackStructure.Count == childrenCount)
                {
                    float offset = 0;

                    //in case index falls out of array bounds due to multiple threads..
                    try
                    {
                        ControlInStack childInfo = null;

                        bool isValid = false;
                        if (Orientation == ScrollOrientation.Horizontal)
                        {
                            if (index < structure.MaxColumns)
                            {
                                isValid = true;
                                childInfo = structure.Get(index, 0);
                            }
                        }
                        else
                        {
                            if (index < structure.MaxRows)
                            {
                                isValid = true;
                                childInfo = structure.Get(0, index);
                            }
                        }

                        if (isValid && childInfo.Measured != null)
                        {

                            if (Orientation == ScrollOrientation.Horizontal)
                            {
                                //todo rework
                                var childOffset = childInfo.Destination.Left / (float)layout.RenderingScale;

                                if (option == RelativePositionType.End)
                                {
                                    offset = childOffset - (this.Viewport.Units.Width - childInfo.Measured.Units.Width);
                                }
                                else if (option == RelativePositionType.Center)
                                {
                                    offset = childOffset -
                                             (this.Viewport.Units.Width - childInfo.Measured.Units.Width) / 2f;
                                }
                                else
                                {
                                    offset = childOffset;
                                }

                                return ClampedOrderedScrollOffset(new SKPoint(-offset, 0));



                            }
                            else
                            if (Orientation == ScrollOrientation.Vertical)
                            {
                                var scrollSpaceY = ptsContentHeight - Viewport.Units.Height;

                                if (scrollSpaceY > 0)
                                {
                                    //todo rework
                                    var childOffset = childInfo.Destination.Top / (float)layout.RenderingScale;

                                    if (option == RelativePositionType.End)
                                    {
                                        offset = childOffset - (this.Viewport.Units.Height - childInfo.Measured.Units.Height);
                                    }
                                    else if (option == RelativePositionType.Center)
                                    {

                                        offset = childOffset -
                                                 (this.Viewport.Units.Height - childInfo.Measured.Units.Height) / 2f;
                                    }
                                    else
                                    {
                                        offset = childOffset;
                                    }

                                    //Debug.WriteLine($"CalculateScrollOffsetForIndex OK {index} {offset:0.0}");

                                    return new SKPoint(0, -offset);
                                }

                                //return ClampedOrderedScrollOffset(new SKPoint(0, -offset));




                            }

                        }

                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e);
                    }
                }
            }

            return NotValidPoint();
        }




        protected virtual bool CheckNeedToSnap()
        {
            bool ret = !(IsSnapping || Snapped
                         || IsUserFocused
                         || OrderedScrollTo.IsValid //already scrolling somewhere
                         || this.SnapToChildren == SnapToChildrenType.Disabled
                         || _vectorAnimatorBounceY.IsRunning || _vectorAnimatorBounceX.IsRunning
                         || _animatorFlingX.IsRunning && (Math.Abs(_animatorFlingX.CurrentVelocity) > _minVelocitySnap
                        || _animatorFlingY.IsRunning && (Math.Abs(_animatorFlingY.CurrentVelocity) > _minVelocitySnap
                        || Math.Abs(_animatorFlingY.CurrentVelocity) > _minVelocitySnap)
                        || Math.Abs(_animatorFlingX.CurrentVelocity) > _minVelocitySnap)
                        );

            //Trace.WriteLine($"CheckNeedToSnap {ret}");

            return ret;
        }

        public virtual void Snap(float maxTimeSecs)
        {
            if (OrderedScrollTo.IsValid || IsSnapping)
            {
                return;
            }

            IsSnapping = true;

            if (Content is SkiaLayout layout)
            {
                var hit = CurrentIndexHit;
                if (hit?.Index > -1 && layout.ChildrenFactory.GetChildrenCount() > hit?.Index)
                {
                    //if (hit.Unmodified == SKPoint.Empty)
                    //{
                    //	_isSnapping = false;
                    //	return;
                    //}

                    var needMove = 0f;
                    if (Orientation == ScrollOrientation.Vertical)
                    {
                        //float needOffsetY = (float)Math.Truncate(ViewportOffsetY);
                        float needOffsetY = (float)Math.Truncate(InternalViewportOffset.Pixels.Y);
                        var initialOffset = needOffsetY;
                        if (SnapToChildren == SnapToChildrenType.Center)
                        {
                            var center = hit.Area.Height / 2f;
                            var pointY = hit.Area.Bottom - hit.Point.Y;
                            needMove = -(pointY - center);
                        }
                        else if (SnapToChildren == SnapToChildrenType.Side)
                        {

                            if (TrackIndexPosition == RelativePositionType.Start)
                            {
                                needMove = hit.Point.Y - hit.Area.Bottom;
                            }
                            else if (TrackIndexPosition == RelativePositionType.End)
                            {
                                needMove = -(hit.Area.Bottom - hit.Point.Y);
                            }
                        }

                        var threshold = RenderingScale * 2;

                        needOffsetY = hit.Unmodified.Y + needMove;
                        if (needMove != 0f && Math.Abs(initialOffset - needOffsetY) > threshold)
                        {
                            //Snapped = true;
                            //ScrollTo(InternalViewportOffset.Units.X, needOffsetY / layout.RenderingScale, maxTimeSecs);

                            Snapped = true;

                            _animatorFlingX.Stop();
                            _animatorFlingY.Stop();

                            ScrollTo(ViewportOffsetX, needOffsetY / layout.RenderingScale, AutoScrollingSpeedMs);

                            return;
                        }

                        //Trace.WriteLine($"Snap low threshold");
                    }
                    else if (Orientation == ScrollOrientation.Horizontal)
                    {
                        float needOffsetX = (float)Math.Truncate(InternalViewportOffset.Units.X);
                        var initialOffset = needOffsetX;
                        if (SnapToChildren == SnapToChildrenType.Center)
                        {
                            var center = hit.Area.Width / 2f;
                            var pointX = hit.Area.Right - hit.Point.X;
                            needMove = -(pointX - center);
                        }
                        else if (SnapToChildren == SnapToChildrenType.Side)
                        {

                            if (TrackIndexPosition == RelativePositionType.Start)
                            {
                                needMove = hit.Area.Width - (hit.Area.Right - hit.Point.X);
                                //needOffsetX += needMove;
                            }
                            else if (TrackIndexPosition == RelativePositionType.End)
                            {
                                needMove = -(hit.Area.Right - hit.Point.X);
                                //needOffsetX += needMove;
                            }
                        }

                        needOffsetX = hit.Unmodified.X + needMove;
                        if (needMove != 0f && initialOffset != needOffsetX)
                        {
                            Snapped = true;

                            _animatorFlingX.Stop();
                            _animatorFlingY.Stop();

                            ScrollTo(needOffsetX / layout.RenderingScale, ViewportOffsetY, AutoScrollingSpeedMs);

                            return;
                        }

                    }
                }

            }

            IsSnapping = false;
        }



        public static readonly BindableProperty SnapToChildrenProperty
        = BindableProperty.Create(nameof(SnapToChildren),
        typeof(SnapToChildrenType), typeof(SkiaScroll),
        SnapToChildrenType.Disabled, propertyChanged: NeedDraw);
        /// <summary>
        /// Whether should snap to children after scrolling stopped
        /// </summary>
        public SnapToChildrenType SnapToChildren
        {
            get
            {
                return (SnapToChildrenType)GetValue(SnapToChildrenProperty);
            }
            set
            {
                SetValue(SnapToChildrenProperty, value);
            }
        }


        public static readonly BindableProperty TrackIndexPositionProperty
        = BindableProperty.Create(nameof(TrackIndexPosition),
        typeof(RelativePositionType), typeof(SkiaScroll),
        RelativePositionType.None, propertyChanged: OnTrackingChanged);

        private static void OnTrackingChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaScroll control)
            {
                control.SetDetectIndexChildPoint(control.TrackIndexPosition);
                NeedDraw(bindable, oldvalue, newvalue);
            }
        }

        /// <summary>
        /// The position in viewport you want to track for content layout child index
        /// </summary>
        public RelativePositionType TrackIndexPosition
        {
            get
            {
                return (RelativePositionType)GetValue(TrackIndexPositionProperty);
            }
            set
            {
                SetValue(TrackIndexPositionProperty, value);
            }
        }


        public static readonly BindableProperty TrackIndexPositionOffsetProperty = BindableProperty.Create(nameof(TrackIndexPositionOffset),
        typeof(float),
        typeof(SkiaScroll),
        8.0f, propertyChanged: OnTrackingChanged);
        public float TrackIndexPositionOffset
        {
            get { return (float)GetValue(TrackIndexPositionOffsetProperty); }
            set { SetValue(TrackIndexPositionOffsetProperty, value); }
        }

        public static readonly BindableProperty LoadMoreCommandProperty = BindableProperty.Create(nameof(LoadMoreCommand),
            typeof(ICommand),
            typeof(SkiaScroll),
            null);
        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        public static readonly BindableProperty LoadMoreOffsetProperty = BindableProperty.Create(nameof(LoadMoreOffset),
            typeof(float),
            typeof(SkiaScroll),
            0.0f, propertyChanged: OnTrackingChanged);
        public float LoadMoreOffset
        {
            get { return (float)GetValue(LoadMoreOffsetProperty); }
            set { SetValue(LoadMoreOffsetProperty, value); }
        }



        #endregion


        protected SKSize LastContentSizePixels = new SKSize(-1, -1);
        protected SKSize LastMeasuredSizePixels = new SKSize(-1, -1);


        protected override void OnMeasured()
        {
            base.OnMeasured();

            if (ContentSize.Pixels != LastContentSizePixels || MeasuredSize.Pixels != LastMeasuredSizePixels)
            {
                LastContentSizePixels = ContentSize.Pixels;
                LastMeasuredSizePixels = MeasuredSize.Pixels;

                InitializeViewport((float)RenderingScale);

                InitializeScroller((float)RenderingScale);
            }
        }

        private PointF lastVelocity;
        private double prevV;
        private long c1;


        protected virtual ISkiaGestureListener PassGestureToChildren(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
        {
            if (IsContentActive)
            {
                return Content.OnSkiaGestureEvent(args, apply);
            }

            return null;
        }

        /// <summary>
        /// Had no panning just down+up with velocity more than threshold
        /// </summary>
        protected bool WasSwiping { get; set; }

        public bool IsUserFocused { get; protected set; }
        public bool IsUserPanning { get; protected set; }

        public float VelocityY
        {
            get
            {
                return _velocityY;
            }

            set
            {
                if (Math.Abs(value) > MaxVelocity)
                {
                    value = MaxVelocity * Math.Sign(value);
                }
                if (_velocityY != value)
                {
                    _velocityY = value;
                    OnPropertyChanged();
                }
            }
        }
        float _velocityY;

        public float VelocityX
        {
            get
            {
                return _velocityX;
            }

            set
            {
                if (Math.Abs(value) > MaxVelocity)
                {
                    value = MaxVelocity * Math.Sign(value);
                }
                if (_velocityX != value)
                {
                    _velocityX = value;
                    OnPropertyChanged();
                }
            }
        }
        float _velocityX;


        private DateTime lastInputTime;

        bool SameSign(double a, double b)
        {
            return Math.Sign(a) == Math.Sign(b);
        }



        public bool SetZoom(double zoom)
        {
            if (ZoomLocked)
                return false;

            //Debug.WriteLine($"[ZOOM] {zoom:0.000}");

            if (zoom < ZoomMin)
                zoom = ZoomMin;
            else
            if (zoom > ZoomMax)
                zoom = ZoomMax;

            ZoomScaleInternal = zoom;

            ViewportZoom = zoom;
            return true;
        }

        /*
        public bool SetZoom(double zoom)
        {
            if (ZoomLocked)
                return false;

            Debug.WriteLine($"[ZOOM] {zoom:0.000}");

            if (zoom < ZoomMin)
                zoom = ZoomMin;
            else if (zoom > ZoomMax)
                zoom = ZoomMax;

            // Calculate viewport center in screen coordinates
            var viewportCenterScreen = new SKPoint((float)(Width / 2), (float)(Height / 2));

            // Current content scale
            var scale = RenderingScale; // Assuming RenderingScale is your base scale factor
            var currentContentScale = (float)(scale * ViewportZoom);

            // Current content offset in pixels
            var contentOffsetPixels = new SKPoint(
                ViewportOffsetX * currentContentScale,
                ViewportOffsetY * currentContentScale);

            // Content coordinates of the center before zooming
            var contentCenterBeforeZoom = new SKPoint(
                (viewportCenterScreen.X - contentOffsetPixels.X) / currentContentScale,
                (viewportCenterScreen.Y - contentOffsetPixels.Y) / currentContentScale);

            // Update the zoom level
            ZoomScaleInternal = zoom;
            ViewportZoom = zoom;

            // New content scale
            var newContentScale = (float)(scale * ViewportZoom);

            // Adjust offsets to keep the content centered
            ViewportOffsetX = ((viewportCenterScreen.X - (contentCenterBeforeZoom.X * newContentScale)) / newContentScale);
            ViewportOffsetY = ((viewportCenterScreen.Y - (contentCenterBeforeZoom.Y * newContentScale)) / newContentScale);

            return true;
        }
        */

        /// <summary>
        /// We might have difference between pinch scale and manually set zoom. 
        /// </summary>
        protected double ZoomScaleInternal { get; set; }


        protected ScaledSize HeaderSize;
        protected ScaledSize FooterSize;

        //private Stopwatch measureWatch = new();

        public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
        {

            if (IsMeasuring || !CanDraw || (widthConstraint < 0 || heightConstraint < 0))
            {
                return MeasuredSize;
            }

            try
            {

                //measureWatch.Restart();

                IsMeasuring = true;

                var request = CreateMeasureRequest(widthConstraint, heightConstraint, scale);
                if (request.IsSame)
                {
                    return MeasuredSize;
                }

                var constraints = GetMeasuringConstraints(request);

                if (Content != null && Content.IsVisible)
                {
                    var viewport = GetContentAvailableRect(constraints.Content);

                    Viewport = ScaledRect.FromPixels(constraints.Content, request.Scale);

                    var zoomedScale = (float)(request.Scale * ViewportZoom);

                    heightConstraint = viewport.Height;

                    var measuredContent = Content.Measure(viewport.Width, heightConstraint, zoomedScale);

                    if (ResetScrollPositionOnContentSizeChanged && (ContentSize.Pixels.Height != measuredContent.Pixels.Height || ContentSize.Pixels.Width != measuredContent.Pixels.Width))
                    {
                        if (ViewportOffsetX != 0 || ViewportOffsetY != 0)
                            ScrollTo(0, 0, 0);
                    }

                    ContentSize = ScaledSize.FromPixels(measuredContent.Pixels.Width, measuredContent.Pixels.Height, scale);
                }
                else
                {
                    ContentSize = ScaledSize.Default;
                }

                if (Header != null)
                    HeaderSize = Header.Measure(request.WidthRequest, request.HeightRequest, request.Scale);
                else
                    HeaderSize = ScaledSize.Default;

                if (Footer != null)
                    FooterSize = Footer.Measure(request.WidthRequest, request.HeightRequest, request.Scale);
                else
                    FooterSize = ScaledSize.Default;

                return SetMeasuredAdaptToContentSize(constraints, scale);
            }
            finally
            {
                IsMeasuring = false;
                //measureWatch.Stop();

            }

        }


        public ScaledRect Viewport { get; protected set; } = new();

        protected override ScaledSize SetMeasured(float width, float height, bool widthCut, bool heightCut, float scale)
        {
            if (Content != null)
            {
                _lastContentSize = this.Content.MeasuredSize;
            }
            else
                _lastContentSize = ScaledSize.Default;

            return base.SetMeasured(width, height, widthCut, heightCut, scale);
        }


        /// <summary>
        /// In PIXELS
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        protected SKRect GetContentAvailableRect(SKRect destination)
        {
            var childRect = new SKRect(destination.Left, destination.Top, destination.Right, destination.Bottom);

            if (Orientation == ScrollOrientation.Both)
            {
                childRect.Right = float.PositiveInfinity;
                childRect.Bottom = float.PositiveInfinity;
            }
            else
            if (Orientation == ScrollOrientation.Vertical)
            {
                childRect.Right = destination.Right;
                childRect.Bottom = float.PositiveInfinity;
            }
            if (Orientation == ScrollOrientation.Horizontal)
            {
                childRect.Right = float.PositiveInfinity;
                childRect.Bottom = destination.Bottom;
            }

            return childRect;
        }



        /// <summary>
        /// This is where the view port is actually is after being scrolled. We used this value to offset viewport on drawing the last frame
        /// </summary>
        protected ScaledPoint InternalViewportOffset { get; set; } = ScaledPoint.FromPixels(0, 0, 1);

        protected ScaledRect ContentViewport { get; set; } = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void AdjustHeaderParallax()
        {
            if (HeaderParallaxRatio == 1)
            {
                ParallaxComputedValue = 0;
            }
            else
            {
                if (this.Orientation == ScrollOrientation.Vertical)
                {
                    var m = InternalViewportOffset.Units.Y * this.HeaderParallaxRatio;
                    ParallaxComputedValue = m;
                }
                else
                if (this.Orientation == ScrollOrientation.Horizontal)
                {
                    var m = InternalViewportOffset.Units.X * this.HeaderParallaxRatio;
                    ParallaxComputedValue = m;
                }
            }

        }




        /// <summary>
        /// Input offset parameters in PIXELS. We render the scroll Content using pixal snapping but the prepared content will be scrolled (offset) using subpixels for a smooth look.
        /// Creates a valid ViewportRect inside.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="offsetPtsX"></param>
        /// <param name="offsetPtsY"></param>
        /// <param name="viewportScale"></param>
        /// <param name="scale"></param>
        protected virtual void PositionViewport(SKRect destination, SKPoint offsetPixels, float viewportScale, float scale)
        {
            if (!IsContentActive || Content == null)
                return;

            if (!IsSnapping)
                Snapped = false;

            var isScroling = _animatorFlingY.IsRunning || _animatorFlingX.IsRunning ||
                             _vectorAnimatorBounceY.IsRunning || _vectorAnimatorBounceX.IsRunning
                                                      || _scrollerX.IsRunning || _scrollerY.IsRunning || IsUserPanning;

            ContentAvailableSpace = GetContentAvailableRect(destination);

            //we scroll at subpixels but stop only at pixel-snapped
            //if (IsScrolling && !isScroling && !IsUserPanning || onceAfterInitializeViewport)
            //{
            //    var roundY = (float)Math.Round(offsetPixels.Y) - offsetPixels.Y;
            //    var roundX = (float)Math.Round(offsetPixels.X) - offsetPixels.X;
            //    offsetPixels.Offset(roundX, roundY);
            //}

            InternalViewportOffset = ScaledPoint.FromPixels(offsetPixels.X, offsetPixels.Y, scale); //removed pixel rounding

            var childRect = ContentAvailableSpace;
            childRect.Offset(InternalViewportOffset.Pixels.X, InternalViewportOffset.Pixels.Y);

            ContentRectWithOffset = ScaledRect.FromPixels(childRect, scale);

            AdjustHeaderParallax();

            //content size changed?.. maybe need to set offsets to a valid position then
            if (onceAfterInitializeViewport)
            {
                onceAfterInitializeViewport = false;
                var clamped = ClampOffset(InternalViewportOffset.Units.X, InternalViewportOffset.Units.Y, true);
                //AdjustHeaderParallax(ScaledPoint.FromUnits(clamped.X, clamped.Y, scale));

                ViewportOffsetX = clamped.X;
                ViewportOffsetY = clamped.Y;

                if (ViewportOffsetX == 0 && ViewportOffsetY == 0)
                {
                    HideRefreshIndicator();
                    ScrollTo(0, 0, 0);
                }

            }

            OverscrollDistance = CalculateOverscrollDistance(InternalViewportOffset.Units.X, InternalViewportOffset.Units.Y);

            if (Content is IInsideViewport viewport)
            {
                SKRect absoluteViewPort = DrawingRect;
                //var absoluteViewPort = Viewport.Pixels;
                //absoluteViewPort.Offset(this.DrawingRect.Left, DrawingRect.Top);

                if (Header != null)
                {
                    if (this.Orientation == ScrollOrientation.Vertical)
                    {
                        absoluteViewPort = new SKRect(
                            absoluteViewPort.Left,
                            absoluteViewPort.Top - Header.MeasuredSize.Pixels.Height,
                            absoluteViewPort.Right,
                            absoluteViewPort.Bottom - Header.MeasuredSize.Pixels.Height
                            );
                        absoluteViewPort.Offset(0, (float)Math.Round(-ContentOffset * scale));
                    }
                    else
                    if (this.Orientation == ScrollOrientation.Horizontal)
                    {
                        absoluteViewPort = new SKRect(absoluteViewPort.Left - Header.MeasuredSize.Pixels.Width, absoluteViewPort.Top, absoluteViewPort.Right - Header.MeasuredSize.Pixels.Width, absoluteViewPort.Bottom);
                        absoluteViewPort.Offset((float)Math.Round(-ContentOffset * scale), 0);
                    }
                }

                ContentViewport = ScaledRect.FromPixels(absoluteViewPort, scale);

                viewport.OnViewportWasChanged(ContentViewport);
            }

            CheckNeedRefresh();

            if (LoadMoreCommand != null)
            {

                if (_loadMoreTriggeredAt != 0
                    && Math.Abs(InternalViewportOffset.Units.Y - _loadMoreTriggeredAt) > (LoadMoreOffset + 100) * scale
                    && (DateTime.Now - _loadMoreTriggeredTime).TotalSeconds > 3)
                //we have scrolled out of the triggered loadMore by 100pts
                {
                    _loadMoreTriggeredAt = 0; //so can track loadMore again
                }

                if (HasContentToScroll && _loadMoreTriggeredAt == 0)
                {
                    var threshold = LoadMoreOffset * scale;

                    if ((Orientation == ScrollOrientation.Vertical && InternalViewportOffset.Units.Y <= _scrollMinY + threshold)
                        || (Orientation == ScrollOrientation.Horizontal && InternalViewportOffset.Units.X <= _scrollMinX + threshold))
                    {
                        _loadMoreTriggeredTime = DateTime.Now;
                        _loadMoreTriggeredAt = InternalViewportOffset.Units.Y;
                        Debug.WriteLine("LoadMoreCommand");
                        LoadMoreCommand?.Execute(this);
                    }
                }

            }

            //POST EVENTS


            Scrolled?.Invoke(this, InternalViewportOffset);

            OnScrolled();

            if (isScroling)
            {
                IsScrolling = true;
            }
            else
            {
                if (IsScrolling)
                {
                    ScrollingEnded?.Invoke(this, InternalViewportOffset);
                    OnScrollingEnded();
                }
                IsScrolling = false;
            }

            //Super.Log($"[SCROLL] {InternalViewportOffset.Pixels.Y}");
            //ExecuteDelayedScrollOrders(); //todo move toonscrolled?
        }

        private bool _IsScrolling;
        public bool IsScrolling
        {
            get
            {
                return _IsScrolling;
            }
            set
            {
                if (_IsScrolling != value)
                {
                    _IsScrolling = value;
                    OnPropertyChanged();
                }
            }
        }

        public override ScaledRect GetOnScreenVisibleArea(float inflateByPixels = 0)
        {
            if (Virtualisation != VirtualisationType.Disabled) //true by default
            {
                //passing visible area to be rendered
                //when scrolling we will pass changed area to be rendered
                //most suitable for large content
                var inflated = ContentViewport.Pixels;
                inflated.Inflate(inflateByPixels, inflateByPixels);
                return ScaledRect.FromPixels(inflated, RenderingScale);
            }
            else
            {
                //passing the whole area to be rendered.
                //when scrolling we will just translate it
                //most suitable for small content
                return ContentRectWithOffset;

                //absoluteViewPort = new SKRect(Viewport.Pixels.Left, Viewport.Pixels.Top,
                //    Viewport.Pixels.Left + ContentSize.Pixels.Width, Viewport.Pixels.Top + ContentSize.Pixels.Height);
            }
        }

        float _loadMoreTriggeredAt;


        protected virtual void HideRefreshIndicator()
        {
            RefreshIndicator?.SetDragRatio(0);
            ScrollLocked = false;
            wasRefreshing = false;
        }

        /// <summary>
        /// Notify current scroll offset to some dependent views.
        /// </summary>
        public virtual void OnScrolled()
        {
            //if (RefreshIndicator is { IsVisible: true } && OverScrolled)
            //{
            //    ApplyScrollPositionToRefreshViewUnsafe();
            //}
        }

        public virtual void OnScrollingEnded()
        {

        }

        public event EventHandler<ScaledPoint> ScrollingEnded;

        public event EventHandler<ScaledPoint> Scrolled;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void ApplyScrollPositionToRefreshViewUnsafe()
        {
            var ratio = 0.0f;
            if (Orientation == ScrollOrientation.Vertical)
            {
                ratio = (OverscrollDistance.Y - RefreshShowDistance) / (RefreshDistanceLimit - RefreshShowDistance);
                if (ratio >= 0)
                    RefreshIndicator.SetDragRatio(ratio);
            }
            else
            if (Orientation == ScrollOrientation.Horizontal)
            {
                ratio = (OverscrollDistance.X - RefreshShowDistance) / (RefreshDistanceLimit - RefreshShowDistance);
                if (ratio >= 0)
                    RefreshIndicator.SetDragRatio(ratio);
            }


            if (IsUserPanning)
            {
                if (!IsRefreshing && RefreshCommand != null && ratio >= 1 && !wasRefreshing && !ScrollLocked)
                {
                    //SetIsRefreshing(true);
                    //RefreshCommand.Execute(this);
                    IsRefreshing = true;
                }
            }
            else
            {
                HideRefreshIndicator();
            }
        }

        public virtual void CheckNeedRefresh()
        {
            if (IsRefreshing)
            {
                return;
            }

            if (RefreshEnabled && RefreshIndicator != null)
            {
                if (OverScrolled)
                {
                    ApplyScrollPositionToRefreshViewUnsafe();
                }
                else
                if (RefreshIndicator.IsVisible)
                {
                    HideRefreshIndicator();
                }
            }
        }

        bool wasRefreshing;

        public void SetIsRefreshing(bool state)
        {
            //lock scrolling at top
            if (state)
            {
                LockGesturesUntilDown = true;
                wasRefreshing = true;
                IsRefreshing = true;
                ScrollLocked = true;
                RefreshCommand?.Execute(this);
            }
            else
            {
                if (ViewportOffsetX == 0 && ViewportOffsetY == 0)
                {
                    HideRefreshIndicator();
                }
                else
                {
                    ScrollToTop(SystemAnimationTimeSecs);
                }
                ScrollLocked = false;
            }

        }

        //public void CheckSnap(bool force = false)
        //{
        //	if (this.SnapToChildren != SnapToChildrenType.Disabled && !_isSnapping)
        //		Task.Run(() =>
        //		{
        //			if (!_isSnapping)
        //				SnapIfNeeded(force);
        //		}).ConfigureAwait(false);
        //}

        protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
        {
            if (destination.Width == 0 || destination.Height == 0)
                return;

            base.Paint(ctx, destination, scale, arguments);

            DrawViews(ctx, ContentRectWithOffset.Pixels, _zoomedScale);

            //if (Virtualisation) //true by default
            //{
            //    DrawViews(ctx, ContentRectWithOffset.Pixels, _zoomedScale);
            //}
            //else
            //{
            //    DrawViews(ctx, ContentAvailableSpace, _zoomedScale);
            //}
        }

        protected override void Draw(SkiaDrawingContext context, SKRect destination,
            float scale)
        {


            isDrawing = true;
            if (IsContentActive)
            {
                //content size changed, we need to initialize scroller again at least
                if (_lastContentSize != this.Content.MeasuredSize)
                {
                    NeedMeasure = true;
                }
            }

            Arrange(destination, SizeRequest.Width, SizeRequest.Height, scale);

            _zoomedScale = (float)(scale * ViewportZoom);

            if (!CheckIsGhost())
            {
                var posX = (float)(ViewportOffsetX * _zoomedScale);
                var posY = (float)(ViewportOffsetY * _zoomedScale);

                //var posX = (float)(ViewportOffsetX * _zoomedScale);
                //var posY = (float)(ViewportOffsetY * _zoomedScale);

                var needReposition =
                                     _updatedViewportForPixY != posY
                                 || _updatedViewportForPixX != posX
                                 || _destination != destination;

                //reposition viewport (scroll)
                if (needReposition)
                {

                    if (Orientation == ScrollOrientation.Vertical)
                    {
                        if (posY < _updatedViewportForPixY)
                        {
                            IsScrollingDirection = LinearDirectionType.Forward;
                        }
                        else
                        if (posY > _updatedViewportForPixY)
                        {
                            IsScrollingDirection = LinearDirectionType.Backward;
                        }
                        else
                        {
                            IsScrollingDirection = LinearDirectionType.None;
                        }
                    }
                    else
                    if (Orientation == ScrollOrientation.Horizontal)
                    {
                        if (posX < _updatedViewportForPixX)
                        {
                            IsScrollingDirection = LinearDirectionType.Forward;
                        }
                        else
                        if (posX > _updatedViewportForPixX)
                        {
                            IsScrollingDirection = LinearDirectionType.Backward;
                        }
                        else
                        {
                            IsScrollingDirection = LinearDirectionType.None;
                        }
                    }
                    else
                    {
                        IsScrollingDirection = LinearDirectionType.None;
                    }


                    _updatedViewportForPixX = posX;
                    _updatedViewportForPixY = posY;
                    _destination = destination;

                    PositionViewport(DrawingRect, new(posX, posY), _zoomedScale, (float)scale);

                    InvalidateCache();
                }

                DrawWithClipAndTransforms(context, DrawingRect, DrawingRect, true,
                    true, (ctx) =>
                    {
                        PaintWithEffects(ctx, DrawingRect, scale, CreatePaintArguments());
                    });

                //Paint(context, DrawingRect, scale, CreatePaintArguments());
            }

            FinalizeDrawingWithRenderObject(context, scale);

            OnDrawn(context, DrawingRect, _zoomedScale, scale);

            isDrawing = false;
        }

        public double ParallaxComputedValue
        {
            get => _parallaxComputedValue;
            set
            {
                if (value.Equals(_parallaxComputedValue)) return;
                _parallaxComputedValue = value;
                OnPropertyChanged();
            }
        }

        protected override int DrawViews(SkiaDrawingContext context, SKRect destination, float scale,
            bool debug = false)
        {
            if (destination.Width <= 0 || destination.Height <= 0)
            {
                return 0;
            }

            int Render(SkiaDrawingContext ctx)
            {
                var drawViews = new List<SkiaControl>(5) { Content };
                var offsetFooter = 0f;
                var translateContent = 0.0;

                if (Header != null)
                {
                    bool drawHeaderBefore = false;

                    if (this.Orientation == ScrollOrientation.Vertical)
                    {
                        translateContent = Header.MeasuredSize.Units.Height;

                        if (!ParallaxOverscrollEnabled)
                        {
                            if (OverscrollDistance.Y <= 0)
                            {
                                Header.AddTranslationY = ParallaxComputedValue;
                            }
                            else
                            {
                                Header.AddTranslationY = 0;
                            }
                        }
                        else
                        {
                            Header.AddTranslationY = ParallaxComputedValue;
                        }

                        // Adjust the header hitbox for parallax
                        var headerTop = destination.Top - Header.UseTranslationY;
                        var headerBottom = headerTop + Header.MeasuredSize.Pixels.Height;
                        var hitboxHeader = new SKRect(destination.Left, (float)headerTop, destination.Right, (float)headerBottom);

                        if (!HeaderBehind && !HeaderSticky)
                        {
                            //draw only if onscreen
                            if (hitboxHeader.IntersectsWith(this.Viewport.Pixels))
                                drawHeaderBefore = true;
                        }
                        else
                        {
                            //will not draw header as one of the views, but as overlay, like refreshview below
                            translateContent += ContentOffset;
                        }

                        if (Content != null)
                            Content.AddTranslationY = translateContent;

                        offsetFooter += Header.MeasuredSize.Units.Height + (float)ContentOffset;

                        if (drawHeaderBefore)
                        {
                            drawViews.Add(Header);
                        }
                        else
                        if (HeaderBehind)
                        {
                            if (hitboxHeader.IntersectsWith(this.Viewport.Pixels))
                                Header.Render(context, DrawingRect, scale);
                        }

                    }
                    else
                    if (this.Orientation == ScrollOrientation.Horizontal)
                    {
                        translateContent = Header.MeasuredSize.Units.Width;

                        if (!ParallaxOverscrollEnabled)
                        {
                            if (OverscrollDistance.X <= 0)
                            {
                                Header.AddTranslationX = ParallaxComputedValue;
                            }
                            else
                            {
                                Header.AddTranslationX = 0;
                            }
                        }
                        else
                        {
                            Header.AddTranslationX = ParallaxComputedValue;
                        }

                        // Adjust the header hitbox for parallax in horizontal orientation
                        var headerLeft = destination.Left + Header.UseTranslationX;
                        var headerRight = headerLeft + Header.MeasuredSize.Pixels.Width;
                        var hitboxHeader = new SKRect((float)headerLeft, destination.Top, (float)headerRight, destination.Bottom);

                        if (!HeaderBehind && !HeaderSticky)
                        {
                            // Draw only if onscreen
                            if (hitboxHeader.IntersectsWith(this.Viewport.Pixels))
                                drawHeaderBefore = true;
                        }
                        else
                        {
                            // Will not draw header as one of the views, but as overlay
                            translateContent += ContentOffset;
                        }


                        if (Content != null)
                            Content.AddTranslationY = translateContent;

                        offsetFooter += Header.MeasuredSize.Units.Width + (float)ContentOffset; ;

                        if (drawHeaderBefore)
                        {
                            drawViews.Add(Header);
                        }
                        else
                        if (HeaderBehind)
                        {
                            if (hitboxHeader.IntersectsWith(this.Viewport.Pixels))
                                Header.Render(context, DrawingRect, scale);
                        }

                    }

                }

                if (Footer != null)
                {
                    if (this.Orientation == ScrollOrientation.Vertical)
                    {
                        if (IsContentActive)
                        {
                            offsetFooter += Content.DrawingRect.Height;
                        }
                        Footer.AddTranslationY = offsetFooter / scale;

                        //draw only if onscreen
                        var hitbox = new SKRect(destination.Left, destination.Top + offsetFooter, destination.Right, destination.Top + offsetFooter + Footer.MeasuredSize.Pixels.Height);
                        if (hitbox.IntersectsWith(this.Viewport.Pixels))
                            drawViews.Add(Footer);
                    }
                    else
                    if (this.Orientation == ScrollOrientation.Horizontal)
                    {
                        if (IsContentActive)
                        {
                            offsetFooter += Content.DrawingRect.Width;
                        }
                        Footer.AddTranslationX = offsetFooter / scale;

                        //draw only if onscreen
                        var hitbox = new SKRect(destination.Left + offsetFooter, destination.Top, destination.Left + offsetFooter + Footer.MeasuredSize.Pixels.Width, destination.Bottom);
                        if (hitbox.IntersectsWith(this.Viewport.Pixels))
                            drawViews.Add(Footer);
                    }


                }

                return RenderViewsList(drawViews, ctx, destination, scale);
            }

            var drawn = Render(context);

            if (Header != null && HeaderSticky && !HeaderBehind)
            {
                Header.Render(context, DrawingRect, scale);
                drawn++;
            }

            if (RefreshEnabled && RefreshIndicator != null && OverScrolled)
            {
                if (InternalRefreshIndicator is SkiaControl refreshIndicator)
                {
                    if (refreshIndicator.CanDraw)
                    {
                        refreshIndicator.Render(context, DrawingRect, scale);
                        drawn++;
                    }
                }
            }

            return drawn;
        }


        float _updatedViewportForPixX;
        float _updatedViewportForPixY;
        //float _lastPosViewportScale;

        public SKRect ContentAvailableSpace { get; protected set; }

        /// <summary>
        /// The viewport for content
        /// </summary>
        public ScaledRect ContentRectWithOffset { get; protected set; }

        public SkiaScroll() : base()
        {
            Init();
        }

        protected void Init()
        {
            UpdateFriction();
            SetRefreshIndicator(RefreshIndicator);
        }

        public override void SetChildren(IEnumerable<SkiaControl> views)
        {
            //do not use subviews as we are using Content property for this control

            return;
        }

        public override void ApplyBindingContext()
        {
            base.ApplyBindingContext();

            if (Content?.BindingContext == null)
                Content?.SetInheritedBindingContext(BindingContext);
        }

        /// <summary>
        /// Use Content property for direct access
        /// </summary>
        /// <param name="view"></param>
        protected virtual void SetContent(SkiaControl view)
        {
            var oldContent = Views.Except(new[] { Footer, Header }).FirstOrDefault(x => x is not IRefreshIndicator);
            if (view != oldContent)
            {
                if (oldContent != null)
                {
                    RemoveSubView(oldContent);
                }
                if (view != null)
                {
                    AddSubView(view);
                }
            }
        }

        public void SetHeader(SkiaControl view)
        {
            var oldContent = Views.Except(new[] { Footer, Content }).FirstOrDefault(x => x is not IRefreshIndicator);
            if (view != oldContent)
            {
                if (oldContent != null)
                {
                    RemoveSubView(oldContent);
                }
                if (view != null)
                {
                    view.ZIndex = 1;
                    AddSubView(view);
                }
            }
        }

        public void SetFooter(SkiaControl view)
        {
            var oldContent = Views.Except(new[] { Header, Content }).FirstOrDefault(x => x is not IRefreshIndicator);
            if (view != oldContent)
            {
                if (oldContent != null)
                {
                    RemoveSubView(oldContent);
                }
                if (view != null)
                {
                    AddSubView(view);
                }
            }
        }

        #region PROPERTIES

        public static readonly BindableProperty ScrollingSpeedMsProperty = BindableProperty.Create(nameof(ScrollingSpeedMs),
            typeof(int),
            typeof(SkiaScroll),
            400);

        /// <summary>
        /// Used by range scroller (ScrollToX, ScrollToY)
        /// </summary>
        public int ScrollingSpeedMs
        {
            get { return (int)GetValue(ScrollingSpeedMsProperty); }
            set { SetValue(ScrollingSpeedMsProperty, value); }
        }

        public static readonly BindableProperty ZoomLockedProperty = BindableProperty.Create(nameof(ZoomLocked),
        typeof(bool),
        typeof(SkiaScroll),
        true);
        public bool ZoomLocked
        {
            get { return (bool)GetValue(ZoomLockedProperty); }
            set { SetValue(ZoomLockedProperty, value); }
        }


        public static readonly BindableProperty ZoomMinProperty = BindableProperty.Create(nameof(ZoomMin),
        typeof(double),
        typeof(SkiaScroll),
        0.1);
        public double ZoomMin
        {
            get { return (double)GetValue(ZoomMinProperty); }
            set { SetValue(ZoomMinProperty, value); }
        }

        public static readonly BindableProperty ZoomMaxProperty = BindableProperty.Create(nameof(ZoomMax),
            typeof(double),
            typeof(SkiaScroll),
            10.0);
        public double ZoomMax
        {
            get { return (double)GetValue(ZoomMaxProperty); }
            set { SetValue(ZoomMaxProperty, value); }
        }

        public static readonly BindableProperty ViewportZoomProperty = BindableProperty.Create(nameof(ViewportZoom),
            typeof(double), typeof(SkiaScroll),
            1.0,
            propertyChanged: NeedDraw);
        public double ViewportZoom
        {
            get { return (double)GetValue(ViewportZoomProperty); }
            set { SetValue(ViewportZoomProperty, value); }
        }

        public static readonly BindableProperty VelocityImageLoaderLockProperty = BindableProperty.Create(
            nameof(VelocityImageLoaderLock),
            typeof(double),
            typeof(SkiaScroll),
            2500.0);

        /// <summary>
        /// Range at which the image loader will stop or resume loading images while scrolling
        /// </summary>
        public double VelocityImageLoaderLock
        {
            get { return (double)GetValue(VelocityImageLoaderLockProperty); }
            set { SetValue(VelocityImageLoaderLockProperty, value); }
        }


        /*
        public static readonly BindableProperty ViewportOffsetYProperty = BindableProperty.Create(nameof(ViewportOffsetY),
            typeof(double), typeof(SkiaScroll),
            0.0,
            propertyChanged: NeedDraw);
        public double ViewportOffsetY
        {
            get
            {
                return (double)GetValue(ViewportOffsetYProperty);
            }
            set
            {
                SetValue(ViewportOffsetYProperty, value);

            }
        }



        public static readonly BindableProperty ViewportOffsetXProperty
            = BindableProperty.Create(nameof(ViewportOffsetX),
            typeof(double), typeof(SkiaScroll),
            0.0,
            propertyChanged: NeedDraw);
        public double ViewportOffsetX
        {
            get
            {
                return (double)GetValue(ViewportOffsetXProperty);
            }
            set
            {
                SetValue(ViewportOffsetXProperty, value);

            }
        }
        */
        public static readonly BindableProperty ContentProperty = BindableProperty.Create(
            nameof(Content),
            typeof(SkiaControl), typeof(SkiaScroll),
            null,
            propertyChanged: OnReplaceContent);

        private static void OnReplaceContent(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaScroll control)
            {
                control.SetContent(newvalue as SkiaControl);
            }
        }
        public SkiaControl Content
        {
            get { return (SkiaControl)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(ScrollOrientation), typeof(SkiaScroll),
            ScrollOrientation.Vertical,
            propertyChanged: NeedDraw);
        /// <summary>
        /// <summary>Gets or sets the scrolling direction of the ScrollView. This is a bindable property.</summary>
        /// </summary>
        public ScrollOrientation Orientation
        {
            get { return (ScrollOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }


        public static readonly BindableProperty ScrollTypeProperty = BindableProperty.Create(nameof(ViewportScrollType), typeof(ViewportScrollType), typeof(SkiaScroll),
            ViewportScrollType.Scrollable,
            propertyChanged: NeedDraw);
        /// <summary>
        /// <summary>Gets or sets the scrolling direction of the ScrollView. This is a bindable property.</summary>
        /// </summary>
        public ViewportScrollType ScrollType
        {
            get { return (ViewportScrollType)GetValue(ScrollTypeProperty); }
            set { SetValue(ScrollTypeProperty, value); }
        }

        public static readonly BindableProperty VirtualisationProperty = BindableProperty.Create(
        nameof(Virtualisation),
        typeof(VirtualisationType),
        typeof(SkiaScroll),
        VirtualisationType.Enabled,
        propertyChanged: NeedInvalidateMeasure);

        /// <summary>
        /// Default is true, children get the visible viewport area for rendering and can virtualize.
        /// If set to false children get the full content area for rendering and draw all at once.
        /// </summary>
        public VirtualisationType Virtualisation
        {
            get { return (VirtualisationType)GetValue(VirtualisationProperty); }
            set { SetValue(VirtualisationProperty, value); }
        }


        //todo ZOOM


        #endregion

        #region ScrollViewKeyboardAwareBehavior

        public static readonly BindableProperty AdaptToKeyboardForProperty = BindableProperty.Create(
            nameof(AdaptToKeyboardFor),
            typeof(SkiaControl),
            typeof(SkiaScroll),
            null, propertyChanged: OnNeedAdaptToKeyboard);

        public SkiaControl AdaptToKeyboardFor
        {
            get { return (SkiaControl)GetValue(AdaptToKeyboardForProperty); }
            set { SetValue(AdaptToKeyboardForProperty, value); }
        }

        public static readonly BindableProperty AdaptToKeyboardSizeProperty = BindableProperty.Create(
            nameof(AdaptToKeyboardSize),
            typeof(double),
            typeof(SkiaScroll),
            0.0, propertyChanged: OnNeedAdaptToKeyboard);

        public double AdaptToKeyboardSize
        {
            get { return (double)GetValue(AdaptToKeyboardSizeProperty); }
            set { SetValue(AdaptToKeyboardSizeProperty, value); }
        }

        private static void OnNeedAdaptToKeyboard(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaScroll control)
            {
                control.AdaptToKeyboard();
            }
        }

        double AddPadding = 0;
        private double _scrollTo;

        public void CalculateNeededScrollForKeyboard()
        {
            _scrollTo = -1;

            try
            {
                if (AdaptToKeyboardFor == null || AdaptToKeyboardSize == 0 || !this.LayoutReady)
                    return;

                var myPos = AdaptToKeyboardFor.GetPositionOnCanvasInPoints();
                var scrollPos = this.GetPositionOnCanvasInPoints();

                var scrollRect = new SKRect(0, scrollPos.Y, 10, (float)this.Height + scrollPos.Y);
                var parentHeight = Superview.Height;
                var screenRect = new SKRect(0, 0, 10, (float)(parentHeight - AdaptToKeyboardSize));
                var viewportRect = scrollRect.IntersectWith(screenRect);
                var elementRect = new SKRect(0, myPos.Y, 10, (float)AdaptToKeyboardFor.Height + myPos.Y);

                var needScrollMore = elementRect.Bottom - viewportRect.Bottom + AddPadding;

                if (needScrollMore > 0)
                    _scrollTo = this.ViewportOffsetY - needScrollMore;

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }

        }

        public virtual void AdaptToKeyboard()
        {
            Tasks.StartDelayed(TimeSpan.FromMilliseconds(150), () =>
            {
                CalculateNeededScrollForKeyboard();

                //scroll to show on screen
                if (LayoutReady && _scrollTo < 0)
                {
                    //Debug.WriteLine($"[SCROLLING] to {_scrollTo} actual offset {this.OffsetY}, last {lastOffsetY}");
                    ViewportOffsetY = (float)_scrollTo;
                }
            });
        }

        #endregion


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(ViewportZoom)
                || propertyName == nameof(Orientation))
            {
                Invalidate();
            }
        }


        public override void InvalidateViewport()
        {
            //owns viewport
            Repaint();
        }


        #region RENDERiNG


        public override bool WillClipBounds => true;



        bool isDrawing;
        private SKRect _destination;
        private ScaledSize _lastContentSize;
        private float _velocityKY;
        private float _velocityKX;
        private float _zoomedScale = 1;
        private double _LastPanDistanceY;
        private double _LastPanDistanceX;
        private DateTime _loadMoreTriggeredTime;
        private double _parallaxComputedValue;
        private float _offsetMoved;
        private long _offsetMovedTime;


        protected virtual void OnDrawn(SkiaDrawingContext context, SKRect destination,
            float zoomedScale,
            double scale = 1.0)
        {

        }


        //public Action<ISkiaControl> Measured { get; set; }

        #endregion


    }
}
