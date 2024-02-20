using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Draw;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;

namespace DrawnUi.Maui.Controls;

public class SkiaCarousel : SnappingLayout
{

    public SkiaCarousel()
    {
        //some defaults
        Spacing = 0;

        //init
        ChildrenFactory.UpdateViews();
    }

    public override bool IsClippedToBounds => true;

    public override void ScrollToNearestAnchor(Vector2 location, Vector2 velocity)
    {
        var theshold = 100f;
        if (Math.Abs(velocity.X) < theshold)
        {
            velocity.X = 0;
        }
        if (Math.Abs(velocity.Y) < theshold)
        {
            velocity.Y = 0;
        }

        base.ScrollToNearestAnchor(location, velocity);
    }

    #region EVENTS

    public event EventHandler<int> SelectedIndexChanged;

    public event EventHandler<int> ItemAppearing;

    public event EventHandler<int> ItemDisappearing;

    public event EventHandler Scrolled;

    public event EventHandler Scrolling;

    protected Dictionary<int, bool> ItemsVisibility { get; } = new();

    void SendVisibility(int index, bool state)
    {
        var last = ItemsVisibility[index];
        if (last != state)
        {
            ItemsVisibility[index] = state;
            if (state)
            {
                //Debug.WriteLine($"ItemAppearing: {index}");
                ItemAppearing?.Invoke(this, index);
            }
            else
            {
                ItemDisappearing?.Invoke(this, index);
                //Debug.WriteLine($"ItemDisappearing: {index}");
            }
        }
    }

    void InitializeItemsVisibility(int count, bool force)
    {
        if (force || ItemsVisibility.Count != count)
        {
            for (int i = 0; i < count; i++)
            {
                ItemsVisibility[i] = false;
            }
        }
    }


    #endregion

    #region METHODS

    public virtual void ApplyIndex(bool instant = false)
    {
        if (SelectedIndex >= 0 && SelectedIndex < SnapPoints.Count)
        {
            var snapPoint = SnapPoints[SelectedIndex];

            ScrollToOffset(snapPoint, Vector2.Zero, !instant && CanAnimate && Animated);
        }
    }

    public override void UpdateReportedPosition()
    {
        if (SnapPoints == null || SnapPoints.Count == 0)
            return;

        var snapPoint = SnapPoints.FirstOrDefault(x => AreVectorsEqual(x, CurrentSnap, 1));

        var index = SnapPoints.IndexOf(snapPoint);
        if (index >= 0 && index < ChildrenFactory.GetChildrenCount())
        {
            SelectedIndex = index;
        }
    }

    #endregion

    #region TEMPLATE

    /// <summary>
    /// This might be called from background thread if we set InitializeTemplatesInBackgroundDelay true
    /// </summary>
    public override void OnTemplatesAvailable()
    {
        ApplyOptions();

        base.OnTemplatesAvailable();
    }

    protected virtual bool CanRender
    {
        get
        {
            if (!IsTemplated)
                return SnapPoints.Count > 0;

            return ChildrenFactory.TemplatesAvailable && SnapPoints.Count > 0;
        }
    }

    protected virtual bool ChildrenReady
    {
        get
        {
            if (!IsTemplated)
                return Views.Count > 0;

            return ChildrenFactory.TemplatesAvailable;
        }
    }

    protected override int RenderViewsList(IEnumerable<SkiaControl> skiaControls, SkiaDrawingContext context, SKRect destination, float scale,
        bool debug = false)
    {
        var drawn = 0;

        if (CanRender)
        {
            double progressMax = 0;
            double progress = 0;
            if (this.IsVertical)
            {
                progressMax = SnapPoints.Last().Y;
                progress = CurrentPosition.Y;
            }
            else
            {
                progressMax = SnapPoints.Last().X;
                progress = CurrentPosition.X;
            }

            this.ScrollProgress = progress / progressMax;
            //Trace.WriteLine($"[C] Progress {ScrollProgress:0.0}");

            var childrenCount = ChildrenFactory.GetChildrenCount();

            //PASS 1
            List<ControlInStack> visibleElements = new();
            List<SkiaControlWithRect> tree = new();

            for (int index = 0; index < childrenCount; index++)
            {
                bool wasUsed = false;
                var position = CalculateChildPosition(CurrentPosition, index);
                if (position.OnScreen || position.NextToScreen)
                {
                    var cell = new ControlInStack()
                    {
                        ControlIndex = index,
                        IsVisible = position.OnScreen,
                        Offset = position.Offset,
                    };
                    visibleElements.Add(cell);
                    wasUsed = true;
                }

                if (!wasUsed)
                    ChildrenFactory.MarkViewAsHidden(index); //recycle template

                SendVisibility(index, position.OnScreen);
            }

            //PASS 2 - draw only visible and thoses at sides that would be selected
            var track = DrawingRect.Width - SidesOffset;
            foreach (var cell in visibleElements)
            {
                var view = ChildrenFactory.GetChildAt(cell.ControlIndex);

                //Debug.Write($"[Carousel] {Tag} obtained cell for index {cell.ControlIndex}, visible {cell.IsVisible}");

                if (view == null)
                {
                    break; //looks like itemssource changed?..
                }

                if (cell.ControlIndex == SelectedIndex)
                {
                    //todo calculate ScrollAmount from 0 to 1
                    var pixels = cell.Offset.X * RenderingScale;
                    ScrollAmount = pixels / track;
                }

                if (cell.IsVisible || this.PreloadNeighboors)
                {
                    view.OnBeforeDraw(); //draw even hidden neighboors to be able to preload stuff
                    if (view.CanDraw)
                    {
                        view.LockUpdate(true);
                        view.TranslationX = cell.Offset.X;
                        view.TranslationY = cell.Offset.Y;
                        view.LockUpdate(false);
                        view.Render(context, destination, scale);
                    }

                }

                if (cell.IsVisible) //but handle gestures only for visible views
                {
                    drawn++;

                    //used by gestures etc..
                    cell.Drawn.Set(view.DrawingRect.Left, view.DrawingRect.Top, view.DrawingRect.Right, view.DrawingRect.Bottom);
                    cell.LastDrawnSize = new(cell.Drawn.Width, cell.Drawn.Height);

                    var destinationRect = new SKRect(cell.Drawn.Left, cell.Drawn.Top, cell.Drawn.Right, cell.Drawn.Bottom);
                    tree.Add(new SkiaControlWithRect(view, destinationRect, cell.ControlIndex));
                }

            }

            //Trace.WriteLine($"[CAROUSEL] {Tag}: {ChildrenFactory.GetDebugInfo()}");

            RenderTree = tree.ToImmutableArray();
            _builtRenderTreeStamp = _measuredStamp;

            return drawn;
        }

        return drawn;
    }

    private double _ScrollAmount;
    /// <summary>
    /// Scroll amount from 0 to 1 of the current (SelectedIndex) slide. Another similar but different property would be ScrollProgress.
    /// </summary>
    public double ScrollAmount
    {
        get
        {
            return _ScrollAmount;
        }
        set
        {
            if (_ScrollAmount != value)
            {
                _ScrollAmount = value;
                OnPropertyChanged();
            }
        }
    }

    private double _ScrollProgress;
    /// <summary>
    /// Scroll progress from 0 to (numberOfSlides-1).
    /// This is not dependent of the SelectedIndex, just reflects visible progress. Useful to create custom controls attached to carousel.
    /// Calculated as (for horizontal): CurrentPosition.X / SnapPoints.Last().X 
    /// </summary>
    public double ScrollProgress
    {
        get
        {
            return _ScrollProgress;
        }
        set
        {
            if (_ScrollProgress != value)
            {
                _ScrollProgress = value;
                OnPropertyChanged();
            }
        }
    }

    protected void AdaptTemplate(SkiaControl skiaControl)
    {
        var margin = SidesOffset;

        if (this.HeightRequest < 0 && VerticalOptions.Alignment == LayoutAlignment.Start)
        {
            skiaControl.VerticalOptions = LayoutOptions.Start;
        }
        else
        {
            skiaControl.VerticalOptions = LayoutOptions.Fill;
        }

        if (this.WidthRequest < 0 && HorizontalOptions.Alignment == LayoutAlignment.Start)
        {
            skiaControl.HorizontalOptions = LayoutOptions.Start;
        }
        else
        {
            skiaControl.HorizontalOptions = LayoutOptions.Fill;
        }

        if (IsVertical)
        {
            skiaControl.Margin = new Thickness(0, margin, 0, margin);
        }
        else
        {
            skiaControl.Margin = new Thickness(margin, 0, margin, 0);
        }
    }



    public override object CreateContentFromTemplate()
    {
        var created = base.CreateContentFromTemplate();
        SkiaControl control;
        if (created is ISkiaAttachable element)
        {
            control = element.AttachControl;
        }
        else
        {
            control = created as SkiaControl;
        }

        AdaptTemplate(control);
        return control;
    }


    SemaphoreSlim semaphoreItemSouce = new(1);

    protected async Task ProcessItemsSource()
    {
        await semaphoreItemSouce.WaitAsync();
        try
        {
            AdaptItemsSource();
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
        }
        finally
        {
            semaphoreItemSouce.Release();
        }

    }

    protected virtual void AdaptItemsSource()
    {

    }

    public override void OnItemSourceChanged()
    {
        base.OnItemSourceChanged();

        AdaptChildren();

        if (ChildrenFactory.TemplatesAvailable)
        {
            ApplyOptions();
        }

    }

    #endregion

    #region ENGINE

    protected (Vector2 Offset, bool OnScreen, bool NextToScreen) CalculateChildPosition(Vector2 currentPosition, int index)
    {
        var childPos = SnapPoints[index];
        float newX = 0;
        float newY = 0;
        bool isVisible = true;
        bool nextToScreen = false;

        float nextToScreenOffset = 10; // you can adjust this value

        if (IsVertical)
        {
            newY = (currentPosition.Y + Math.Abs(childPos.Y));

            if (newY + SidesOffset * 2 > Height - 1 || newY + (Height - SidesOffset * 2) + SidesOffset * 2 < 1)
            {
                isVisible = false;
                if (Math.Abs(newY) - Width - SidesOffset - Spacing <= nextToScreenOffset)
                {
                    nextToScreen = true;
                }
            }
        }
        else
        {
            newX = (currentPosition.X + Math.Abs(childPos.X));
            if (newX + SidesOffset * 1 > Width - 1 || newX + (Width - SidesOffset * 1) + SidesOffset * 1 < 1)
            {
                isVisible = false;
                if (Math.Abs(newX) - Width - SidesOffset - Spacing <= nextToScreenOffset)
                {
                    nextToScreen = true;
                }
            }
        }

        return (new Vector2(newX, newY), isVisible, nextToScreen);
    }




    /// <summary>
    /// Will translate child child and raise appearing/disappearing events
    /// </summary>
    /// <param name="currentPosition"></param>
    public override void ApplyPosition(Vector2 currentPosition)
    {
        CurrentPosition = currentPosition;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            InTransition = !CheckTransitionEnded();
        });

        Update();
    }



    protected override bool ScrollToOffset(Vector2 targetOffset, Vector2 velocity, bool animate)
    {
        if (ScrollLocked || targetOffset == CurrentSnap)
        {
            //Debug.WriteLine("[CAROUSEL] ScrollToOffset blocked");
            return false;
        }

        if (animate && Height > 0)
        {

            //_animatorSpring?.Stop();

            var start = CurrentSnap;
            var end = new Vector2((float)targetOffset.X, (float)targetOffset.Y);

            var atSnapPoint = SnapPoints.Any(x => x.X == CurrentSnap.X && x.Y == CurrentSnap.Y);

            var displacement = start - end;

            if (velocity == Vector2.Zero)
                velocity = GetAutoVelocity(displacement);

            if (displacement != Vector2.Zero)
            {
                if (atSnapPoint && !ThresholdOk(displacement))
                {
                    //Debug.WriteLine("[CAROUSEL] threshold low");
                    return false;
                }

                if (Bounces)
                {
                    var spring = new Spring((float)(1 * (1 + RubberDamping)), 200, (float)(0.5f * (1 + RubberDamping)));
                    _animatorSpring.Initialize(end, displacement, velocity, spring, 0.5f);
                    _animatorSpring.Start();
                }
                else
                {
                    var maxSpeed = 0.25; //secs

                    var direction = GetDirectionType(start, end, 0.8f);
                    var seconds = displacement / velocity;
                    var speed = maxSpeed;
                    if (direction == DirectionType.Vertical)
                    {
                        speed = Math.Abs(seconds.Y);
                        speed *= (Math.Abs(end.Y - start.Y) / Height);
                    }
                    else
                    if (direction == DirectionType.Horizontal)
                    {
                        speed = Math.Abs(seconds.X);
                        speed *= (Math.Abs(end.X - start.X) / Height);
                    }

                    if (speed > maxSpeed)
                        speed = maxSpeed;

                    _animatorRange.Initialize(start, end, (float)speed, Easing.Linear);
                    _animatorRange.Start();
                }
            }
            else
            {
                //Debug.WriteLine($"[CAROUSEL] displacement zero!");
                return false;
            }

            //if (displacement != Vector2.Zero)
            //{
            //    if (atSnapPoint && !ThresholdOk(displacement))
            //    {
            //        //Debug.WriteLine("[CAROUSEL] threshold low");
            //        return false;
            //    }

            //    var spring = new Spring((float)(1 * (1 + RubberDamping)), 200, (float)(0.5f * (1 + RubberDamping)));
            //    _animatorSpring.Initialize(end, displacement, velocity, spring);

            //    _animatorSpring.Start();
            //}

        }
        else
        if (CanDraw)
        {
            //Debug.WriteLine("[CAROUSEL] setting offset");
            ApplyPosition(targetOffset);
        }

        CurrentSnap = targetOffset;
        UpdateReportedPosition();
        return true;
    }

    bool ThresholdOk(Vector2 displacement)
    {
        var threshold = 10;

        if (IsVertical)
            return threshold < Math.Abs(displacement.Y);

        return threshold < Math.Abs(displacement.X);
    }

    public override void ApplyOptions()
    {
        if (Parent == null)
            return;

        Viewport = Parent.DrawingRect;

        InitializeChildren();

        base.ApplyOptions();
    }



    void Init()
    {
        //CheckConstraints();

        if (Parent != null)
        {
            ApplyOptions();

            if (_animatorSpring == null)
            {
                _animatorSpring = new(this)
                {
                    OnStart = () =>
                    {

                    },
                    OnStop = () =>
                    {
                        Scrolled?.Invoke(this, null);
                    },
                    OnVectorUpdated = (value) =>
                    {
                        ApplyPosition(value);
                        Scrolling?.Invoke(this, null);
                    }
                };
                _animatorRange = new(this)
                {
                    OnVectorUpdated = (value) =>
                    {
                        ApplyPosition(value);
                        Scrolling?.Invoke(this, null);
                    },
                    OnStop = () =>
                    {
                        Scrolled?.Invoke(this, null);
                    }
                };
            }

            ViewportReady = this.Height > 0 && this.Width > 0;

            //if (IsTemplated)
            //{
            //    ResizeChildrenViewport();
            //}
        }
    }

    /// <summary>
    /// There are the bounds the scroll offset can go to.. This are NOT the bounds of the whole content.
    /// </summary>
    public override SKRect GetContentOffsetBounds()
    {
        if (SnapPoints.Count > 0)
        {
            var last = SnapPoints.Count - 1;

            if (!IsLooped)
            {
                //DEFAULT
                if (IsVertical)
                {
                    return new SKRect(
                        SnapPoints[0].X,
                        SnapPoints[last].Y,
                        SnapPoints[last].X,
                        SnapPoints[0].Y
                    );
                }
                else
                {
                    return new SKRect(
                        SnapPoints[last].X,
                        SnapPoints[0].Y,
                        SnapPoints[0].X,
                        SnapPoints[last].Y
                    );
                }
            }
            else
            {
                //LOOPED
                if (IsVertical)
                {
                    return new SKRect(
                        SnapPoints[0].X,
                        float.MinValue,
                        SnapPoints[last].X,
                        float.MaxValue
                    );
                }
                else
                {
                    return new SKRect(
                        float.MinValue,
                        SnapPoints[0].Y,
                        float.MaxValue,
                        SnapPoints[last].Y
                    );
                }
            }



        }

        return SKRect.Empty;
    }


    protected virtual void CheckConstraints()
    {
        if (this.HeightRequest < 0 && VerticalOptions.Alignment == LayoutAlignment.Start
            || this.WidthRequest < 0 && HorizontalOptions.Alignment == LayoutAlignment.Start)
        {
            throw new Exception("Carousel must have a fixed size or use Fill");
        }
    }


    protected override void OnChildAdded(SkiaControl child)
    {
        base.OnChildAdded(child);

        AdaptChildren();
    }

    protected bool ChildrenInitialized;

    /// <summary>
    /// We expect this to be called after this alyout is invalidated
    /// </summary>
    public virtual void InitializeChildren()
    {
        if (Viewport == SKRect.Empty || !ChildrenReady)
        {
            return;
        }

        ChildrenInitialized = true;

        var childrenCount = ChildrenFactory.GetChildrenCount();
        InitializeItemsVisibility(childrenCount, true);

        var snapPoints = new List<Vector2>();
        float currentPosition = 0;

        var cellSize = new SKSize((float)Width, (float)Height);

        for (int index = 0; index < childrenCount; index++)
        {

            var offset = (float)(index * (-SidesOffset * 2 + Spacing));

            var position = IsVertical
                ? new SKPoint(0, currentPosition + offset)
                : new SKPoint(currentPosition + offset, 0);

            snapPoints.Add(new Vector2(-position.X, -position.Y));

            currentPosition += (IsVertical ? cellSize.Height : cellSize.Width);

        }

        SnapPoints = snapPoints;

        ContentOffsetBounds = GetContentOffsetBounds();

        CurrentSnap = new(-1, -1);

        if (SnapPoints.Any() && (SelectedIndex < 0 || SelectedIndex > snapPoints.Count - 1))
        {
            SelectedIndex = 0;
        }
        else
        {
            ApplyIndex(true);
        }
    }


    /// <summary>
    /// Set children layout options according to our settings. Not used for template case.
    /// </summary>
    protected virtual void AdaptChildren()
    {
        if (!IsTemplated)
        {

            var index = 0;

            ChildrenFactory.UpdateViews();

            using var cells = ChildrenFactory.GetViewsIterator();

            foreach (var skiaControl in cells)
            {
                ItemsVisibility[index] = false;

                AdaptTemplate(skiaControl);

                index++;
            }

            ChildrenCount = ChildrenFactory.GetChildrenCount();
        }

    }

    private int _ChildrenCount;
    public int ChildrenCount
    {
        get
        {
            return _ChildrenCount;
        }
        set
        {
            if (_ChildrenCount != value)
            {
                _ChildrenCount = value;
                OnPropertyChanged();
            }
        }
    }


    protected override int GetTemplatesPoolLimit()
    {
        var poolSize = 3;
        if (this.RecyclingTemplate == RecyclingTemplate.Disabled)
            poolSize = ItemsSource.Count;

        return poolSize;
    }

    protected override int GetTemplatesPoolPrefill()
    {
        return GetTemplatesPoolLimit();
    }


    //would need if layout was column or row inside scroll
    void ResizeChildrenViewport()
    {
        var childrenCount = ChildrenFactory.GetChildrenCount();
        SKRect rect = DrawingRect;
        if (IsVertical)
        {
            var totalheight = this.Height * childrenCount + Spacing * childrenCount - 1;
            rect = new SKRect(DrawingRect.Left, DrawingRect.Top, DrawingRect.Right, (float)(DrawingRect.Top + totalheight * RenderingScale));
        }
        else
        {
            var totalWidth = this.Width * childrenCount + Spacing * childrenCount - 1;
            rect = new SKRect(DrawingRect.Left, DrawingRect.Top, (float)(DrawingRect.Left + totalWidth * RenderingScale), DrawingRect.Bottom);
        }
        Viewport = rect;
    }

    protected override Vector2 FindNearestAnchorInternal(Vector2 current, Vector2 velocity)
    {
        if (velocity == Vector2.Zero)
        {
            return base.FindNearestAnchorInternal(current, velocity);
        }

        //hack to slide only 1 item at a time
        return base.FindNearestAnchorInternal(_panningStartOffset, velocity);
    }

    #endregion

    #region PROPERTIES

    //public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
    //{
    //    var measured = base.Measure(widthConstraint, heightConstraint, scale);

    //    //todo use DynamicSize


    //    return measured;
    //}

    public static readonly BindableProperty PreloadNeighboorsProperty = BindableProperty.Create(
        nameof(PreloadNeighboors),
        typeof(bool),
        typeof(SkiaCarousel),
        true);

    /// <summary>
    /// Whether should preload neighboors sides cells even when they are hidden, to preload images etc.. Default is true. Beware set this to False if you have complex layouts otherwise rendering might be slow.
    /// </summary>
    public bool PreloadNeighboors
    {
        get { return (bool)GetValue(PreloadNeighboorsProperty); }
        set { SetValue(PreloadNeighboorsProperty, value); }
    }

    public static readonly BindableProperty DynamicSizeProperty = BindableProperty.Create(
        nameof(DynamicSize),
        typeof(bool),
        typeof(SkiaCarousel),
        false, propertyChanged: NeedInvalidateMeasure);

    /// <summary>
    /// When specific dimension is adapting to children size, will use max child dimension if False,
    /// otherwise will change size when children with different dimension size are selected. Default is false.
    /// If true, requires MeasureAllItems to be set to all items.
    /// </summary>
    public bool DynamicSize
    {
        get { return (bool)GetValue(DynamicSizeProperty); }
        set { SetValue(DynamicSizeProperty, value); }
    }


    public static readonly BindableProperty IsLoopedProperty = BindableProperty.Create(
        nameof(IsLooped),
        typeof(bool),
        typeof(SkiaCarousel),
        false, propertyChanged: NeedApplyOptions);

    public bool IsLooped
    {
        get { return (bool)GetValue(IsLoopedProperty); }
        set { SetValue(IsLoopedProperty, value); }
    }


    public static readonly BindableProperty IsVerticalProperty = BindableProperty.Create(
        nameof(IsVertical),
        typeof(bool),
        typeof(SkiaCarousel),
        false, propertyChanged: NeedApplyOptions);

    /// <summary>
    /// Orientation of the carousel
    /// </summary>
    public bool IsVertical
    {
        get { return (bool)GetValue(IsVerticalProperty); }
        set { SetValue(IsVerticalProperty, value); }
    }



    bool viewportSet;

    protected override void OnLayoutChanged()
    {
        base.OnLayoutChanged();

        if (Parent != null)
        {
            Viewport = Parent.DrawingRect;

            if (!viewportSet)// !CompareRects(Viewport, _lastViewport, 1))
            {
                viewportSet = true;
                _lastViewport = Viewport;
                Init();
            }

            if (DynamicSize && SelectedIndex >= 0)
            {
                if (!ChildrenInitialized)
                {
                    InitializeChildren();
                }
                ApplyDynamicSize(SelectedIndex);
            }
            else
            {
                InitializeChildren();
            }
        }
    }


    protected virtual void ApplyDynamicSize(int index)
    {
        if (ChildrenFactory.TemplatesAvailable)
        {
            var child = ChildrenFactory.GetChildAt(index);
            if (child != null && !child.NeedMeasure)
            {
                if (this.NeedAutoHeight)
                {
                    {
                        var height = child.MeasuredSize.Units.Height;
                        if (height >= 0)
                        {
                            //Trace.WriteLine($"[DH] {height:0.0}");
                            this.ViewportHeightLimit = height;
                        }
                    }
                }
                if (this.NeedAutoWidth)
                {
                    {
                        var width = child.MeasuredSize.Units.Width;
                        if (width >= 0)
                        {
                            //this.ViewportWidthLimit = width;
                        }
                    }
                }
            }
        }
    }



    protected virtual void OnSelectedIndexChanged(int index)
    {
        SelectedIndexChanged?.Invoke(this, index);

        if (!LayoutReady)
            return;

        ApplyIndex();

        if (!ChildrenInitialized)
        {
            InitializeChildren();
        }

        if (DynamicSize && SelectedIndex >= 0)
        {
            ApplyDynamicSize(SelectedIndex);
        }

    }

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex),
        typeof(int),
        typeof(SkiaCarousel),
        -1, BindingMode.TwoWay,
        propertyChanged: (b, o, n) =>
        {
            if (b is SkiaCarousel control)
            {
                control.OnSelectedIndexChanged((int)n);
            }
        });

    /// <summary>
    /// Zero-based index of the currently selected slide
    /// </summary>
    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly BindableProperty IsRightToLeftProperty = BindableProperty.Create(
        nameof(IsRightToLeft),
        typeof(bool),
        typeof(SkiaCarousel),
        false, propertyChanged: NeedApplyOptions);

    /// <summary>
    /// TODO
    /// </summary>
    public bool IsRightToLeft
    {
        get { return (bool)GetValue(IsRightToLeftProperty); }
        set { SetValue(IsRightToLeftProperty, value); }
    }

    public static readonly BindableProperty SidesOffsetProperty = BindableProperty.Create(
        nameof(SidesOffset),
        typeof(double),
        typeof(SkiaCarousel),
        0.0, propertyChanged: NeedAdaptChildren);

    /// <summary>
    /// Basically size margins of every slide, offset from the side of the carousel. Another similar but different property to use would be Spacing between slides.
    /// </summary>
    public double SidesOffset
    {
        get { return (double)GetValue(SidesOffsetProperty); }
        set { SetValue(SidesOffsetProperty, value); }
    }


    private static void NeedAdaptChildren(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaCarousel control)
        {
            control.AdaptChildren();
        }
    }


    #endregion

    #region GESTURES

    protected bool IsUserFocused { get; set; }
    protected bool IsUserPanning { get; set; }

    protected Vector2 _panningOffset;
    protected Vector2 _panningStartOffset;
    private SKRect _lastViewport;

    public override ISkiaGestureListener ProcessGestures(TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
    SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
    {
        bool passedToChildren = false;

        //Debug.WriteLine($"[CAROUSEL] {this.Tag} Got {touchAction}..");

        //var thisOffset = TranslateInputCoords(childOffset);

        ISkiaGestureListener PassToChildren()
        {
            passedToChildren = true;

            if (!IsTemplated || RecyclingTemplate == RecyclingTemplate.Disabled)
            {
                return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed); //can pass to them all
            }

            //todo templated visible only...

            return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
        }

        ISkiaGestureListener consumed = null;

        if (!IsUserPanning || !RespondsToGestures)
        {
            consumed = PassToChildren();
            if (consumed != null)
            {
                return consumed;
            }
        }

        if (!RespondsToGestures)
            return null;


        void ResetPan()
        {
            IsUserFocused = true;
            IsUserPanning = false;

            _animatorSpring?.Stop();

            _panningOffset = CurrentPosition;
            _panningStartOffset = CurrentPosition;

            //Debug.WriteLine($"ResetPan: {_panningOffset}");
        }

        switch (touchAction)
        {
        case TouchActionResult.Down:

        //        if (!IsUserFocused) //first finger down
        if (args.NumberOfTouches == 1) //first finger down
        {
            ResetPan();
        }

        consumed = this;

        break;

        case TouchActionResult.Panning when args.NumberOfTouches == 1:

        if (!IsUserFocused)
        {
            ResetPan();
        }

        IsUserPanning = true;

        var x = _panningOffset.X + args.Distance.Delta.X / RenderingScale;
        var y = _panningOffset.Y + args.Distance.Delta.Y / RenderingScale;

        //saving non clamped
        _panningOffset.X = x;
        _panningOffset.Y = y;


        var clamped = ClampOffset((float)x, (float)y, Bounces);

        //Debug.WriteLine($"[CAROUSEL] Panning: {_panningOffset:0} / {clamped:0}");
        ApplyPosition(clamped);

        consumed = this;
        break;

        case TouchActionResult.Up:

        if (IsUserFocused)
        {
            Vector2 Velocity;
            float velocity = 0;
            if (!IsVertical)
            {
                velocity = (float)(args.Distance.Velocity.X / RenderingScale);
                Velocity = new(velocity, 0);
            }
            else
            {
                velocity = (float)(args.Distance.Velocity.Y / RenderingScale);
                Velocity = new(0, velocity);
            }

            if (IsUserPanning) //|| Math.Abs(velocity) > 30)
            {
                consumed = this;

                //animate
                CurrentSnap = CurrentPosition;
                ScrollToNearestAnchor(CurrentSnap, Velocity);
            }

            IsUserPanning = false;
            IsUserFocused = false;

        }

        break;
        }

        if (consumed != null || IsUserPanning)
        {
            return consumed ?? this;
        }

        if (!passedToChildren)
            return PassToChildren();

        return null;
    }

    #endregion
}