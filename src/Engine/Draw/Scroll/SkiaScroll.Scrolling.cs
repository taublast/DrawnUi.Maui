using DrawnUi.Maui.Draw;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw;

public class VelocityAccumulator
{
    private List<(Vector2 velocity, DateTime time)> velocities = new List<(Vector2 velocity, DateTime time)>();
    private const double Threshold = 10.0; // Minimum significant movement
    private const int MaxSampleSize = 5; // Number of samples for weighted average
    private const int ConsiderationTimeframeMs = 150; // Timeframe in ms for velocity consideration

    public void Clear()
    {
        velocities.Clear();
    }

    public void CaptureVelocity(Vector2 velocity)
    {
        var now = DateTime.UtcNow;
        if (velocities.Count == MaxSampleSize) velocities.RemoveAt(0);
        velocities.Add((velocity, now));
    }

    public Vector2 CalculateFinalVelocity(float clampAbsolute = 0)
    {
        var now = DateTime.UtcNow;
        var relevantVelocities = velocities.Where(v => (now - v.time).TotalMilliseconds <= ConsiderationTimeframeMs).ToList();
        if (!relevantVelocities.Any()) return Vector2.Zero;

        // Calculate weighted average for both X and Y components
        float weightedSumX = relevantVelocities.Select((v, i) => v.velocity.X * (i + 1)).Sum();
        float weightedSumY = relevantVelocities.Select((v, i) => v.velocity.Y * (i + 1)).Sum();
        var weightSum = Enumerable.Range(1, relevantVelocities.Count).Sum();

        if (clampAbsolute != 0)
        {
            return new Vector2(Math.Clamp(weightedSumX / weightSum, -clampAbsolute, clampAbsolute),
                Math.Clamp(weightedSumY / weightSum, -clampAbsolute, clampAbsolute));
        }

        return new Vector2(weightedSumX / weightSum, weightedSumY / weightSum);
    }
}

public struct ScrollToIndexOrder
{
    public static ScrollToIndexOrder Default => new()
    {
        Index = -1
    };
    public bool IsSet
    {
        get
        {
            return Index >= 0;
        }
    }
    public bool Animated { get; set; }
    public float MaxTimeSecs { get; set; }
    public RelativePositionType RelativePosition { get; set; }
    public int Index { get; set; }
}

public struct ScrollToPointOrder
{
    public bool IsValid
    {
        get
        {
            return !float.IsNaN(Location.X) && !float.IsNaN(Location.Y); ;
        }
    }

    public static ScrollToPointOrder NotValid => new()
    {
        Location = new SKPoint(float.NaN, float.NaN)
    };


    public static ScrollToPointOrder ToPoint(SKPoint point, bool animated)
    {
        return new()
        {
            Location = point,
            Animated = animated
        };
    }

    public static ScrollToPointOrder ToCoords(float x, float y, bool animated)
    {
        return new()
        {
            Location = new SKPoint(x, y),
            Animated = animated
        };
    }

    public static ScrollToPointOrder ToCoords(float x, float y, float maxTimeSecs)
    {
        return new()
        {
            Location = new SKPoint(x, y),
            MaxTimeSecs = maxTimeSecs
        };
    }

    public bool Animated { get; set; }
    public SKPoint Location { get; set; }
    public float MaxTimeSecs { get; set; }

}


public partial class SkiaScroll
{

    public float ViewportOffsetY
    {
        get
        {
            return _orderedOffsetY;
        }

        set
        {
            if (_orderedOffsetY != value)
            {
                _orderedOffsetY = value;
                //Debug.WriteLine($"[ViewportOffsetY] {value}");
                Update();
                OnPropertyChanged();
            }
        }
    }

    protected float _orderedOffsetY;


    public float ViewportOffsetX
    {
        get
        {
            return _viewportOffsetX;
        }

        set
        {
            if (_viewportOffsetX != value)
            {
                _viewportOffsetX = value;
                Update();
                OnPropertyChanged();
            }
        }
    }
    float _viewportOffsetX;

    protected virtual void InitializeViewport(float scale)
    {
        _loadMoreTriggeredAt = 0;

        ContentOffsetBounds = GetContentOffsetBounds();

        HasContentToScroll = ptsContentHeight > Viewport.Units.Height || ptsContentWidth > Viewport.Units.Width;

        _scrollMinX = ContentOffsetBounds.Left;
        if (_scrollMinX >= 0)
        {
            ViewportOffsetX = 0;
        }
        _scrollMaxX = 0;

        _scrollMinY = ContentOffsetBounds.Top;
        if (_scrollMinY >= 0)
        {
            ViewportOffsetY = 0;
        }
        _scrollMaxY = 0;

        ViewportReady = true;
        onceAfterInitializeViewport = true;
    }

    bool onceAfterInitializeViewport;

    public bool ViewportReady { get; protected set; }

    protected virtual void InitializeScroller(float scale)
    {
        if (_animatorBounce == null)
        {
            _animatorBounce = new(this)
            {
                OnStart = () =>
                {

                },
                OnStop = () =>
                {
                    UpdateLoadingLock(false);
                    IsSnapping = false;
                },
                OnVectorUpdated = (value) =>
                {
                    if (Orientation == ScrollOrientation.Vertical)
                    {
                        ViewportOffsetY = value.Y; //not clamped
                    }
                    else
                    if (Orientation == ScrollOrientation.Horizontal)
                    {
                        ViewportOffsetX = value.X;
                    }
                    else
                    {
                        ViewportOffsetX = value.X;
                        ViewportOffsetY = value.Y;
                    }
                }
            };

            _animatorFling = new(this)
            {
                //					Friction = FrictionScrolled,
                //					Scale = scale,
                OnStart = () =>
                {
                    //_isSnapping = false;
                    OnScrollerStarted();
                },
                OnStop = () =>
                {
                    //_isSnapping = false;
                    OnScrollerStopped();
                },
                OnVectorUpdated = (value) =>
                {
                    var clamped = ClampOffset(value.X, value.Y);
                    ViewportOffsetX = clamped.X;
                    ViewportOffsetY = clamped.Y;

                    OnScrollerUpdated();
                }
            };

            _scrollerX = new(this)
            {
                OnStop = () =>
                {
                    IsSnapping = false;
                    //SkiaImageLoadingManager.Instance.IsLoadingLocked = false;
                }
            };

            _scrollerY = new(this)
            {
                OnStop = () =>
                {
                    IsSnapping = false;
                    //SkiaImageLoadingManager.Instance.IsLoadingLocked = false;
                }
            };
        }

        if (_animatorBounce.IsRunning)
        {
            _animatorBounce.Stop();
        }

        SetDetectIndexChildPoint(TrackIndexPosition);

        this.UpdateVisibleIndex();

        ExecuteDelayedScrollOrders();

        if (CheckNeedToSnap())
            Snap(0);
    }

    /// <summary>
    /// Use Range scroller, offset in Units
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="animate"></param>
    public void ScrollToX(float offset, bool animate)
    {

        if (animate)
        {
            _scrollerX.Start(
                (value) =>
                {
                    ViewportOffsetX = (float)value;
                },
                InternalViewportOffset.Units.X, offset, (uint)ScrollingSpeedMs, ScrollingEasing);
        }
        else
        {
            ViewportOffsetX = offset;
            IsSnapping = false;
        }
    }



    /// <summary>
    /// Use Range scroller, offset in Units
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="animate"></param>
    protected void ScrollToY(float offset, bool animate)
    {

        if (animate)
        {
            _scrollerY.Start(
                (value) =>
                {
                    ViewportOffsetY = (float)value;
                },
                InternalViewportOffset.Units.Y, offset, (uint)ScrollingSpeedMs, ScrollingEasing);
        }
        else
        {
            ViewportOffsetY = offset;
            IsSnapping = false;
        }
    }

    protected virtual void OnScrollerStarted()
    {
        UpdateLoadingLock(_animatorFling.Parameters.InitialVelocity);
    }

    protected virtual void OnScrollerStopped()
    {
        //Super.Log("OnScrollerStopped..");

        UpdateLoadingLock(false);

        if (CheckNeedToSnap())
        {
            Snap(SystemAnimationTimeSecs);
        }
        else
        {
            //scroll ended prematurely by our intent because it would end past the bounds
            if (Bounces)
            {
                if (_animatorFling.SelfFinished && _changeSpeed != null)
                {
                    var remainingVelocity = _animatorFling.Parameters.VelocityAt(_animatorFling.Speed);
                    var velocityX = remainingVelocity.X;
                    if (Math.Abs(remainingVelocity.X) > MaxBounceVelocity)
                    {
                        velocityX = Math.Sign(remainingVelocity.X) * MaxBounceVelocity;
                    }

                    var velocityY = remainingVelocity.Y;
                    if (Math.Abs(remainingVelocity.Y) > MaxBounceVelocity)
                    {
                        velocityY = Math.Sign(remainingVelocity.Y) * MaxBounceVelocity;
                    }

                    //Super.Log("OnScrollerStopped Bouncing..");
                    Bounce(new Vector2((float)ViewportOffsetX, (float)ViewportOffsetY), _axis, new Vector2(velocityX, velocityY));


                }
                else
                {
                    var whut = 1;
                }
            }
        }
    }

    protected virtual void OnScrollerUpdated()
    {
        UpdateLoadingLock(_animatorFling.CurrentVelocity);
    }


    public virtual void ExecuteDelayedScrollOrders()
    {
        if (OrderedScrollToIndex.IsSet)
        {
            ExecuteScrollToIndexOrder();
        }
        else
        {
            ExecuteScrollToOrder();
        }
    }


    /*
    
    basic concept:

    when finger goes up we check where the scrolling would end with current velocity.
    if it is outside of the bounds we adjust the scroling duration so it ends near the bounds,
    otherwise we start scrolling animator as usual.

    when scrolling animator stops natually
    we check if we are outside of the bounds then start bouncing animator if needed

    when animator passes offsets to props they get clamped, see below

    if the finger goes down we stop animators unnaturally

    when the finger is down we can pan: we apply rubber clamp to offsets if bounce prop is true,
    otherwise we apply simple clamp

     */

    //deceleration slow 0.999
    // deceleration normal 0.998
    // deceleration fast 0.99


    protected enum GesturesLogicState
    {
        None,
        Began,
        Changed,
        Ended,
        Canceled,
    }


    void Bounce(Vector2 offsetFrom, Vector2 offsetTo, Vector2 velocity)
    {
        //Super.Log($"[SCROLL] {this.Tag} *BOUNCE* to {offsetTo.Y} v {velocity.Y}..");

        var displacement = offsetFrom - offsetTo;

        //Debug.WriteLine($"[BOUNCE] {offsetFrom} - {offsetTo} with {velocity}");

        if (displacement != Vector2.Zero)
        {
            var spring = new Spring((float)(1 * (1 + RubberDamping)), 200, (float)(0.5f * (1 + RubberDamping)));
            _animatorBounce.Initialize(offsetTo, displacement, velocity, spring);
            _animatorBounce.Start();
        }
        else
        {
            IsSnapping = false;
        }
    }

    /// <summary>
    /// This uses whole viewport size, do not use this for snapping
    /// </summary>
    /// <param name="overscrollPoint"></param>
    /// <param name="contentRect"></param>
    /// <param name="viewportSize"></param>
    /// <returns></returns>
    public static SKPoint GetClosestSidePoint(SKPoint overscrollPoint, SKRect contentRect, SKSize viewportSize)
    {
        SKPoint closestPoint = new SKPoint();

        // The overscrollPoint represents the negative of the content offset, so we need to reverse it for calculation
        SKPoint contentOffset = new SKPoint(-overscrollPoint.X, -overscrollPoint.Y);

        var width = contentRect.Width - viewportSize.Width;
        if (width < 0)
            width = 0;

        if (contentOffset.X < 0) //scrolling to  right
            closestPoint.X = contentRect.Left;
        else
        if (contentOffset.X > 0) //scrolling to left
            closestPoint.X = width;
        else
            closestPoint.X = contentOffset.X;

        var height = contentRect.Height - viewportSize.Height;
        if (height < 0)
            height = 0;

        if (contentOffset.Y < 0) //scrolling to bottom
            closestPoint.Y = contentRect.Top;
        else
        if (contentOffset.Y > 0) //scrolling to top
            closestPoint.Y = height;
        else
            closestPoint.Y = contentOffset.Y;

        // Reverse the offset back to the overscroll representation for the result
        closestPoint.X = -closestPoint.X;
        closestPoint.Y = -closestPoint.Y;

        return closestPoint;
    }


    public static SKPoint ClosestPoint(SKRect rect, SKPoint point)
    {
        SKPoint result = point;

        if (!rect.ContainsInclusive(point))
        {
            if (point.X < rect.Left)
                result.X = rect.Left;
            else if (point.X > rect.Right)
                result.X = rect.Right;

            if (point.Y < rect.Top)
                result.Y = rect.Top;
            else if (point.Y > rect.Bottom)
                result.Y = rect.Bottom;
        }

        return result;
    }

    protected virtual bool OffsetOk(Vector2 offset)
    {
        if (offset.Y >= ContentOffsetBounds.Top && offset.Y <= ContentOffsetBounds.Bottom
        && offset.X >= ContentOffsetBounds.Left && offset.X <= ContentOffsetBounds.Right)
            return true;

        return false;
    }


    public bool OverScrolled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return OverscrollDistance != Vector2.Zero;
        }
    }


    protected float ptsContentWidth;
    protected float ptsContentHeight;

    /// <summary>
    /// There are the bounds the scroll offset can go to.. This is NOT the bounds for the whole content.
    /// </summary>
    public SKRect GetContentOffsetBounds()
    {
        ptsContentWidth = ContentSize.Units.Width;
        ptsContentHeight = ContentSize.Units.Height;

        if (Orientation == ScrollOrientation.Vertical)
        {
            ptsContentHeight += HeaderSize.Units.Height + FooterSize.Units.Height + (float)ContentOffset;
        }

        if (Orientation == ScrollOrientation.Horizontal)
        {
            ptsContentWidth += HeaderSize.Units.Width + FooterSize.Units.Width + (float)ContentOffset;
        }

        var width = ptsContentWidth - Viewport.Units.Width;
        var height = ptsContentHeight - Viewport.Units.Height;

        if (height < 0)
            height = 0;

        if (width < 0)
            width = 0;

        var rect = new SKRect(-width, -height, 0, 0);

        return rect;
    }

    public Vector2 CalculateOverscrollDistance(float x, float y)
    {
        float overscrollX = 0f;
        float overscrollY = 0f;

        if (x > _scrollMaxX)
        {
            overscrollX = x - _scrollMaxX;
        }
        else if (x < _scrollMinX)
        {
            overscrollX = -(_scrollMinX - x);
        }

        if (y > _scrollMaxY)
        {
            overscrollY = y - _scrollMaxY;
        }
        else if (y < _scrollMinY)
        {
            overscrollY = -(_scrollMinY - y);
        }

        //Debug.WriteLine($"[OVERSCROLL] {overscrollY}");

        return new Vector2(overscrollX, overscrollY);
    }

    protected double _minVelocity = 1.5;

    private float _DecelerationRatio = 0.002f;
    public float DecelerationRatio
    {
        get
        {
            return _DecelerationRatio;
        }
        set
        {
            if (_DecelerationRatio != value)
            {
                _DecelerationRatio = value;
                OnPropertyChanged();
            }
        }
    }

    public void UpdateFriction()
    {
        var friction = FrictionScrolled;
        if (friction < 0.1)
        {
            //silent clamp
            friction = 0.1f;
        }

        DecelerationRatio = (float)friction / 100f; // 0.2 => 0.002
    }


    public virtual bool StartToFlingFrom(Vector2 from, Vector2 velocity)
    {
        var contentOffset = from;

        _animatorFling.Initialize(contentOffset, velocity, 1f - DecelerationRatio);

        if (PrepareToFlingAfterInitialized())
        {
            _animatorFling.RunAsync(null).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    protected virtual async Task<bool> FlingFrom(Vector2 from, Vector2 velocity)
    {
        //todo - add cancellation support

        //	Trace.WriteLine($"[FLING] velocity {velocity}");

        var contentOffset = from;// new Vector2((float)ViewportOffsetX, (float)ViewportOffsetY);

        _animatorFling.Initialize(contentOffset, velocity, 1f - DecelerationRatio);

        return await FlingAfterInitialized();
    }

    protected virtual async Task<bool> FlingToAuto(Vector2 from, Vector2 to, float changeSpeedSecs = 0)
    {
        var velocity = _animatorFling.Parameters.VelocityToZero(from, to, changeSpeedSecs);

        _animatorFling.Initialize(from, velocity, 1f - DecelerationRatio);

        if (changeSpeedSecs > 0)
            _animatorFling.Speed = changeSpeedSecs;

        return await FlingAfterInitialized();
    }

    protected virtual async Task<bool> FlingTo(Vector2 from, Vector2 to, float timeSeconds)
    {
        Vector2 velocity = _animatorFling.Parameters.VelocityTo(from, to, timeSeconds);

        _animatorFling.Initialize(from, velocity, 1f - DecelerationRatio);

        _animatorFling.Speed = timeSeconds;

        return await FlingAfterInitialized();
    }

    protected virtual bool PrepareToFlingAfterInitialized()
    {
        var destination = _animatorFling.Parameters.Destination;

        var destinationPoint = new SKPoint(destination.X, destination.Y);

        _changeSpeed = null;

        if (!OffsetOk(destination)) //detected that scroll will end past the bounds
        {
            var contentRect = new SKRect(0, 0, ptsContentWidth, ptsContentHeight);
            var closestPoint = GetClosestSidePoint(destinationPoint, contentRect, Viewport.Units.Size);
            _axis = new(closestPoint.X, closestPoint.Y);

            _changeSpeed = _animatorFling.Parameters.DurationToValue(new Vector2(closestPoint.X, closestPoint.Y));
            _animatorFling.Speed = _changeSpeed.Value;
        }

        return _animatorFling.Speed > 0;
    }

    protected async Task<bool> FlingAfterInitialized()
    {

        if (PrepareToFlingAfterInitialized())
        {
            await _animatorFling.RunAsync(null);

            IsSnapping = false;

            return true;
        }

        return false;
    }



    /// <summary>
    /// We might order a scroll before the control was drawn, so it's a kind of startup position
    /// saved every time one calls ScrollTo
    /// </summary>
    protected ScrollToPointOrder OrderedScrollTo = ScrollToPointOrder.NotValid;

    /// <summary>
    /// We might order a scroll before the control was drawn, so it's a kind of startup position
    /// saved every time one calls ScrollToIndex
    /// </summary>
    protected ScrollToIndexOrder OrderedScrollToIndex;

    /// <summary>
    /// In Units
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="animate"></param>
    protected void ScrollToOffset(Vector2 targetOffset, float maxTimeSecs)
    {
        if (maxTimeSecs > 0 && Height > 0)
        {
            //_animatorFling.Stop();
            //var from = new Vector2((float)ViewportOffsetX, (float)ViewportOffsetY);
            //FlingToAuto(from, targetOffset, maxTimeSecs).ConfigureAwait(false);

            StopScrolling();
            ScrollToX(targetOffset.X, true);
            ScrollToY(targetOffset.Y, true);
        }
        else
        {
            ViewportOffsetX = targetOffset.X;
            ViewportOffsetY = targetOffset.Y;
            IsSnapping = false;

            this.UpdateVisibleIndex();
        }
    }


    public virtual void MoveToY(float value)
    {
        if (!ScrollLocked)
        {
            ViewportOffsetY = value;
        }
    }

    public virtual void MoveToX(float value)
    {
        if (!ScrollLocked)
        {
            ViewportOffsetX = value;

        }
    }

    public void ScrollToIndex(int index, bool animate, RelativePositionType option = RelativePositionType.Start)
    {
        //saving to use upon creating control if this was called before its internal structure was really created
        OrderedScrollToIndex = new()
        {
            Animated = animate,
            RelativePosition = option,
            Index = index
        };

        ExecuteScrollToIndexOrder();
    }

    public bool ExecuteScrollToOrder()
    {
        if (OrderedScrollTo.IsValid)
        {
            ScrollToOffset(new Vector2(OrderedScrollTo.Location.X,
                    OrderedScrollTo.Location.Y),
                OrderedScrollTo.MaxTimeSecs);
            OrderedScrollTo = ScrollToPointOrder.NotValid;
            return true;
        }

        return false;
    }

    public bool ExecuteScrollToIndexOrder()
    {
        if (OrderedScrollToIndex.IsSet)
        {
            //saving to use upon creating control if this was called before its internal structure was really created
            var offset = CalculateScrollOffsetForIndex(OrderedScrollToIndex.Index,
                OrderedScrollToIndex.RelativePosition);

            if (PointIsValid(offset))
            {
                var time = 0f;
                if (OrderedScrollToIndex.Animated)
                    time = SystemAnimationTimeSecs;

                ScrollTo(offset.X, offset.Y, time);
                OrderedScrollToIndex = ScrollToIndexOrder.Default;
                return true;
            }
        }
        return false;
    }

    public void ScrollTo(float x, float y, float maxSpeedSecs)
    {
        StopScrolling();

        OrderedScrollTo = ScrollToPointOrder.ToCoords(x, y, maxSpeedSecs);

        if (!ExecuteScrollToOrder())
        {
            this.UpdateVisibleIndex();
        }
    }

    public void ScrollToTop(float maxTimeSecs)
    {
        if (Orientation == ScrollOrientation.Vertical)
        {
            ScrollTo(InternalViewportOffset.Units.X, 0, maxTimeSecs);
        }
        else
        if (Orientation == ScrollOrientation.Horizontal)
        {
            ScrollTo(0, InternalViewportOffset.Units.Y, maxTimeSecs);
        }
        else
        {
            ScrollTo(0, 0, maxTimeSecs);
        }
    }

    public void ScrollToBottom(float maxTimeSecs)
    {
        if (Orientation == ScrollOrientation.Vertical)
        {
            ScrollTo(InternalViewportOffset.Units.X, _scrollMinY, maxTimeSecs);
        }
        else
        if (Orientation == ScrollOrientation.Horizontal)
        {
            ScrollTo(_scrollMinX, InternalViewportOffset.Units.Y, maxTimeSecs);
        }
        else
        {
            ScrollTo(_scrollMinX, _scrollMinY, maxTimeSecs);
        }
    }

    private bool _Snapped;
    public bool Snapped
    {
        get
        {
            return _Snapped;
        }
        set
        {
            if (_Snapped != value)
            {
                _Snapped = value;
                OnPropertyChanged();
            }
        }
    }


    private bool _IsSnapping;
    public bool IsSnapping
    {
        get
        {
            return _IsSnapping;
        }
        set
        {
            if (_IsSnapping != value)
            {
                _IsSnapping = value;
                OnPropertyChanged();
            }
        }
    }

    Vector2 _axis;
    double? _changeSpeed = null;
    private Vector2 _lastVelocity;
}
