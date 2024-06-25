using Microsoft.Maui.Controls.Internals;
using System.Runtime.CompilerServices;
using Size = Microsoft.Maui.Graphics.Size;

namespace DrawnUi.Maui.Views;

/// <summary>
/// Optimized DrawnView having only one child inside Content property. Can autosize to to children size.
/// For all drawn app put this directly inside the ContentPage as root view.
/// If you put this inside some Maui control like Grid whatever expect more GC collections during animations making them somewhat less fluid.
/// </summary>
[ContentProperty("Content")]

public class Canvas : DrawnView, IGestureListener
{

    public override void SetChildren(IEnumerable<SkiaControl> views)
    {
        //do not use subviews as we are using Content property for this control
        // so we just override not calling base
    }

    protected override void OnChildAdded(SkiaControl child)
    {
        if (Views.Count > 1)
        {
            throw new Exception(
                "Canvas cannot have more than one subview due to rendering optimizations. Please use Content property and use SkiaLayout if you need to have many controls on canvas.");
        }

        base.OnChildAdded(child);
    }

    /// <summary>
    /// Use Content property for direct access
    /// </summary>
    protected virtual void SetContent(SkiaControl view)
    {
        var oldContent = Views.FirstOrDefault();
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


    #region LAYOUT & AUTOSIZE

    private double _widthConstraint;
    private double _heightConstraint;
    private Size _measuredContent;

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();

        InvalidateChildren();
    }

    protected override void InvalidateMeasure()
    {
        NeedMeasure = true;

        base.InvalidateMeasure();
    }

    bool _wasMeasured;

    protected Size AdaptSizeToContentIfNeeded(double widthConstraint, double heightConstraint, bool force = false)
    {

        if (force || _widthConstraint != widthConstraint && _heightConstraint != heightConstraint)
        {

            var measured = Measure((float)widthConstraint, (float)heightConstraint);

            var request = measured.Units;

            _widthConstraint = widthConstraint;
            _heightConstraint = heightConstraint;
            _measuredContent = new Size(request.Width + Margin.Left + Margin.Right, request.Height + Margin.Top + Margin.Bottom);
            NeedMeasure = false;

            if (_measuredContent != Size.Zero)
            {
                _wasMeasured = true;
            }
        }

        DesiredSize = _measuredContent;
        return DesiredSize;
    }

    /// <summary>
    /// We need this mainly to autosize inside grid cells
    /// This is also called when parent visibilty changes
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Rect bounds)
    {
        NeedCheckParentVisibility = true;

        if (NeedAutoSize)
        {
            AdaptSizeToContentIfNeeded(bounds.Width, bounds.Height, true);
        }

        return base.ArrangeOverride(bounds);
    }

    private Size _lastMeasureResult;
    private Size _lastMeasureConstraints;

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        if (_lastMeasureConstraints.Width == widthConstraint && _lastMeasureConstraints.Height == heightConstraint)
        {
            return _lastMeasureResult;
        }

        //we are going to receive the size NOT reduced by Maui margins
        Size ret;
        NeedCheckParentVisibility = true;

        if (NeedAutoSize)
        {
            ret = AdaptSizeToContentIfNeeded(widthConstraint, heightConstraint, NeedMeasure);
        }
        else
        {
            ret = base.MeasureOverride(widthConstraint, heightConstraint);
            NeedMeasure = false;
            Update();
        }

        _lastMeasureConstraints = new(widthConstraint, heightConstraint);
        _lastMeasureResult = ret;

        return ret;
    }


    public override void Invalidate()
    {

        if (NeedAutoSize)
        {
            //will trigger parent calling our MeasureOverride
            //this can be called from main thread only !!!
            MainThread.BeginInvokeOnMainThread(() =>
            {
                InvalidateMeasureNonVirtual(InvalidationTrigger.HorizontalOptionsChanged);
            });

        }

        base.Invalidate();
    }



    public float AdaptWidthContraintToRequest(double widthConstraint)
    {
        if (widthConstraint >= 0)//&& width < widthConstraint)
            widthConstraint -= Margin.HorizontalThickness;

        return (float)widthConstraint;
    }
    public float AdaptHeightContraintToRequest(double heightConstraint)
    {
        if (heightConstraint >= 0)// && widthPixels < heightConstraint)
            heightConstraint -= Margin.VerticalThickness;

        return (float)heightConstraint;
    }

    public override ScaledSize Measure(float widthConstraintPts, float heightConstraintPts)
    {
        if (!IsVisible)
        {
            return SetMeasured(0, 0, (float)RenderingScale);
        }

        if (widthConstraintPts < 0 || heightConstraintPts < 0)
        {
            //not setting NeedMeasure=false;
            return ScaledSize.Default;
        }

        widthConstraintPts = AdaptWidthContraintToRequest(widthConstraintPts);
        heightConstraintPts = AdaptHeightContraintToRequest(heightConstraintPts);

        widthConstraintPts += (float)(ReserveSpaceAround.Left + ReserveSpaceAround.Right);
        heightConstraintPts += (float)(ReserveSpaceAround.Top + ReserveSpaceAround.Bottom);

        if (IsVisible && Views.Any())
        {
            var children = GetOrderedSubviews();
            if (children.Count > 0)
            {
                SKRect rectForChild = GetMeasuringRectForChildren(widthConstraintPts, heightConstraintPts, (float)RenderingScale);

                var maxHeight = 0.0f;
                var maxWidth = 0.0f;
                foreach (var child in children)
                {
                    child.OnBeforeMeasure(); //could set IsVisible or whatever inside
                    var willDraw = MeasureChild(child, rectForChild.Width, rectForChild.Height, RenderingScale);
                    if (!willDraw.IsEmpty)
                    {
                        if (willDraw.Pixels.Width > maxWidth)
                            maxWidth = willDraw.Pixels.Width;
                        if (willDraw.Pixels.Height > maxHeight)
                            maxHeight = willDraw.Pixels.Height;
                    }
                }

                ContentSize = ScaledSize.FromPixels(maxWidth, maxHeight, (float)RenderingScale);

                widthConstraintPts = AdaptWidthContraintToContentRequest(widthConstraintPts, ContentSize, Padding.Left + Padding.Right);
                heightConstraintPts = AdaptHeightContraintToContentRequest(heightConstraintPts, ContentSize, Padding.Top + Padding.Bottom);
            }
        }

        return SetMeasured(widthConstraintPts, heightConstraintPts, (float)RenderingScale);
        //return base.Measure(widthConstraintPts, heightConstraintPts);
    }

    /// <summary>
    /// All in in UNITS, OUT in PIXELS
    /// </summary>
    /// <param name="widthConstraint"></param>
    /// <param name="heightConstraint"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public SKRect GetMeasuringRectForChildren(float widthConstraint, float heightConstraint, float scale)
    {
        var constraintLeft = Padding.Left * scale;
        var constraintRight = Padding.Right * scale;
        var constraintTop = Padding.Top * scale;
        var constraintBottom = Padding.Bottom * scale;

        SKRect rectForChild = new SKRect(0 + (float)constraintLeft,
            0 + (float)constraintTop,
            widthConstraint * scale - (float)constraintRight,
            heightConstraint * scale - (float)constraintBottom);

        return rectForChild;
    }

    public float AdaptWidthContraintToContentRequest(float widthConstraintUnits, ScaledSize measuredContent, double sideConstraintsUnits)
    {
        return SkiaControl.AdaptConstraintToContentRequest(
            widthConstraintUnits,
            measuredContent.Units.Width,
            sideConstraintsUnits,
            NeedAutoWidth,
            MinimumWidthRequest,
            MaximumWidthRequest,
            1, false);
    }

    public float AdaptHeightContraintToContentRequest(float heightConstraintUnits, ScaledSize measuredContent, double sideConstraintsUnits)
    {
        return SkiaControl.AdaptConstraintToContentRequest(
            heightConstraintUnits,
            measuredContent.Units.Height,
            sideConstraintsUnits,
            NeedAutoHeight,
            MinimumHeightRequest,
            MaximumHeightRequest,
            1, false);
    }


    /// <summary>
    /// In UNITS
    /// </summary>
    /// <param name="widthRequest"></param>
    /// <param name="heightRequest"></param>
    /// <returns></returns>
    protected Size AdaptSizeRequestToContent(double widthRequest, double heightRequest)
    {
        if (NeedAutoWidth && ContentSize.Units.Width > 0)
        {
            widthRequest = ContentSize.Units.Width + Padding.Left + Padding.Right;
        }
        if (NeedAutoHeight && ContentSize.Units.Height > 0)
        {
            heightRequest = ContentSize.Units.Height + Padding.Top + Padding.Bottom;
        }

        return new Size(widthRequest, heightRequest); ;
    }

    private ScaledSize _contentSize = new();
    public ScaledSize ContentSize
    {
        get
        {
            return _contentSize;
        }
        protected set
        {
            if (_contentSize != value)
            {
                _contentSize = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// PIXELS
    /// </summary>
    /// <param name="child"></param>
    /// <param name="availableWidth"></param>
    /// <param name="availableHeight"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    protected ScaledSize MeasureChild(SkiaControl child, double availableWidth, double availableHeight, double scale)
    {
        child.OnBeforeMeasure();
        if (!child.CanDraw)
            return ScaledSize.Default; //child set himself invisible

        return child.Measure((float)availableWidth, (float)availableHeight, (float)scale);
    }

    #endregion

    #region GESTURES

    protected virtual void OnGesturesAttachChanged()
    {
        if (this.Gestures == GesturesMode.Disabled)
        {
            TouchEffect.SetForceAttach(this, false);
        }
        else
        {
            TouchEffect.SetForceAttach(this, true);

            if (this.Gestures == GesturesMode.Enabled)
                TouchEffect.SetShareTouch(this, TouchHandlingStyle.Default);
            else
            if (this.Gestures == GesturesMode.Lock)
                TouchEffect.SetShareTouch(this, TouchHandlingStyle.Lock);
            else
            if (this.Gestures == GesturesMode.Share)
                TouchEffect.SetShareTouch(this, TouchHandlingStyle.Share);
        }
    }

    public static Color DebugGesturesColor { get; set; } = Colors.Transparent;// Color.Parse("#ff0000");

    /// <summary>
    /// To filter micro-gestures on super sensitive screens, start passing panning only when threshold is once overpassed
    /// </summary>
    public static float FirstPanThreshold = 5;

    bool _isPanning;

    protected virtual void ProcessGestures(SkiaGesturesParameters args)
    {

        lock (LockIterateListeners)
        {
            ISkiaGestureListener consumed = null;
            ISkiaGestureListener wasConsumed = null;

            IsHiddenInViewTree = false; //if we get a gesture, we are visible by design
            bool manageChildFocus = false;

            //Super.Log($"[Touch] Canvas got {args.Type}");
            if (DebugGesturesColor != Colors.Transparent && args.Type == TouchActionResult.Down)
            {
                PostponeExecutionAfterDraw(() =>
                {
                    using (SKPaint paint = new SKPaint
                    {
                        Style = SKPaintStyle.StrokeAndFill,
                        Color = DebugGesturesColor.ToSKColor()
                    })
                    {
                        this.CanvasView.Surface.Canvas.DrawCircle((float)(args.Event.Location.X), (float)(args.Event.Location.Y), (float)(20 * RenderingScale), paint);
                    }
                });
            }

            //var listeners = CollectionsMarshal.AsSpan(GestureListeners.GetListeners());
            foreach (var listener in GestureListeners.GetListeners())
            {
                if (!listener.CanDraw || listener.InputTransparent)
                {
                    continue;
                }

                if (listener == FocusedChild)
                    manageChildFocus = true;

                var forChild = true;
                if (args.Type != TouchActionResult.Up)
                    forChild = ((SkiaControl)listener).HitIsInside(args.Event.StartingLocation.X, args.Event.StartingLocation.Y) || listener == FocusedChild;

                if (forChild)
                {
                    //Debug.WriteLine($"[Passed] to {listener}");
                    if (manageChildFocus && listener == FocusedChild)
                    {
                        manageChildFocus = false;
                    }

                    var adjust = new GestureEventProcessingInfo()
                    {
                        alreadyConsumed = wasConsumed
                    };

                    consumed = listener.OnSkiaGestureEvent(args, adjust);

                    if (consumed != null)
                    {
                        // TODO implement same code as skiacontrol?
                        break;
                    }
                }
            }

            if (args.Type == TouchActionResult.Down)
            {
                Debug.WriteLine($"DOWN {args.Event.Location.Y}");
            }

            if (args.Type == TouchActionResult.Tapped)
            {
                Debug.WriteLine($"TAPPED {args.Event.Location.Y}");
            }

            //if (TouchEffect.LogEnabled)
            {
                if (consumed == null)
                {
                    Super.Log($"[Touch] {args.Type} ({args.Event.NumberOfTouches}) not consumed");
                }
                else
                {
                    Super.Log($"[Touch] {args.Type} ({args.Event.NumberOfTouches}) consumed by {consumed} {consumed.Tag}");
                }
            }

            if (args.Type == TouchActionResult.Up)
                if (manageChildFocus || FocusedChild != null && consumed != FocusedChild)
                {
                    FocusedChild = consumed;
                }
        }

    }

    private SKPoint _panningOffset;

    /// <summary>
    /// IGestureListener implementation
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args1"></param>
    /// <param name="args1"></param>
    /// <param name=""></param>
    public virtual void OnGestureEvent(TouchActionType type, TouchActionEventArgs args1, TouchActionResult touchAction)
    {
        var args = SkiaGesturesParameters.Create(touchAction, args1);

        if (args.Type == TouchActionResult.Panning)
        {
            //filter micro-gestures
            if ((Math.Abs(args.Event.Distance.Delta.X) < 1 && Math.Abs(args.Event.Distance.Delta.Y) < 1)
                || (Math.Abs(args.Event.Distance.Velocity.X / RenderingScale) < 1 && Math.Abs(args.Event.Distance.Velocity.Y / RenderingScale) < 1))
            {
                return;
            }

            var threshold = FirstPanThreshold * RenderingScale;

            if (!_isPanning)
            {
                //filter first panning movement on super sensitive screens
                if (Math.Abs(args.Event.Distance.Total.X) < threshold && Math.Abs(args.Event.Distance.Total.Y) < threshold)
                {
                    _panningOffset = SKPoint.Empty;
                    return;
                }

                if (_panningOffset == SKPoint.Empty)
                {
                    _panningOffset = args.Event.Distance.Total.ToSKPoint();
                }

                //args.PanningOffset = _panningOffset;

                _isPanning = true;
            }
        }

        if (args.Type == TouchActionResult.Down)
        {
            _isPanning = false;
        }

        //this is intended to not lose gestures when fps drops and avoid crashes in double-buffering
        PostponeExecutionBeforeDraw(() =>
               {
                   try
                   {
                       ProcessGestures(args);
                   }
                   catch (Exception e)
                   {
                       Trace.WriteLine(e);
                   }
               });

        Repaint();
    }

    #endregion

    #region HELPER METHODS



    public void BreakLine()
    {
        LineBreaks.Add(Views.Count);
    }

    protected List<int> LineBreaks = new List<int>();

    public Canvas() : base()
    {

    }

    public void Clear()
    {
        ClearChildren();
    }

    public void PlayRippleAnimation(Color color, double x, double y, bool removePrevious = true)
    {
        var animation = new RippleAnimator(this)
        {
            X = x,
            Y = y
        };
        animation.Start();
    }

    public void PlayShimmerAnimation(Color color, float shimmerWidth, float shimmerAngle, int speedMs = 1000, bool removePrevious = false)
    {
        var animation = new ShimmerAnimator(this)
        {
            Color = color.ToSKColor(),
            ShimmerWidth = shimmerWidth,
            ShimmerAngle = shimmerAngle,
            Speed = speedMs
        };
        animation.Start();
    }


    public static long TimeLastGC { get; set; }

    public static long DelayNanosBetweenGC { get; set; } = 160_000_000; //160ms

    public static void CollectGarbage(long timeNanos)
    {
        if (TimeLastGC == 0) //first time
        {
            TimeLastGC = timeNanos;
            return;
        }

        if (timeNanos - TimeLastGC > DelayNanosBetweenGC)
        {
            TimeLastGC = timeNanos;
            GC.Collect(0);
        }
    }


    #endregion

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        //Reacting to Maui view properties changing
        if (propertyName.IsEither(nameof(WidthRequest), nameof(HeightRequest), nameof(BackgroundColor)))
        {
            Update();
        }
    }

    #region PROPERTIES

    public static readonly BindableProperty ReserveSpaceAroundProperty = BindableProperty.Create(
        nameof(ReserveSpaceAround),
        typeof(Thickness),
        typeof(Canvas),
        Thickness.Zero);

    public Thickness ReserveSpaceAround
    {
        get { return (Thickness)GetValue(ReserveSpaceAroundProperty); }
        set { SetValue(ReserveSpaceAroundProperty, value); }
    }


    private static void GesturesChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is Canvas control)
        {
            control.OnGesturesAttachChanged();
        }
    }

    public static readonly BindableProperty GesturesProperty = BindableProperty.Create(
        nameof(GesturesMode),
        typeof(GesturesMode),
        typeof(Canvas),
        GesturesMode.Disabled, propertyChanged: GesturesChanged);

    public GesturesMode Gestures
    {
        get { return (GesturesMode)GetValue(GesturesProperty); }
        set { SetValue(GesturesProperty, value); }
    }

#pragma warning disable NU1605, CS0108

    public static readonly BindableProperty ContentProperty = BindableProperty.Create(
        nameof(Content),
        typeof(ISkiaControl), typeof(Canvas),
        null,
        propertyChanged: OnReplaceContent);

#pragma warning restore NU1605, CS0108

    private static void OnReplaceContent(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is Canvas control)
        {
            control.SetContent(newvalue as SkiaControl);
        }
    }
    public new ISkiaControl Content
    {
        get { return (ISkiaControl)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    #endregion

    #region RENDERiNG


    protected override void OnParentChanged()
    {
        base.OnParentChanged();

        NeedCheckParentVisibility = true;
    }


    /// <summary>
    /// Enable canvas rendering itsself
    /// </summary>
    public virtual void EnableUpdates()
    {
        UpdateLocked = false;
        NeedCheckParentVisibility = true;
        Update();
    }

    /// <summary>
    /// Disable invalidating and drawing on the canvas
    /// </summary>
    public virtual void DisableUpdates()
    {
        UpdateLocked = true;
    }


    protected override void Draw(SkiaDrawingContext context, SKRect destination, float scale)
    {

        if (BackgroundColor != null)
        {
            context.Canvas.Clear(BackgroundColor.ToSKColor());
        }
        else
            context.Canvas.Clear();

        double widthRequest = WidthRequest;
        double heightRequest = HeightRequest;

        Arrange(destination, widthRequest, heightRequest, scale);

        if (!IsGhost)
        {

            PaintTintBackground(context.Canvas);

            base.Draw(context, Destination, scale);
        }

        //Storyboard.CollectGarbage(frameTimeNanos);

    }


    #endregion



}