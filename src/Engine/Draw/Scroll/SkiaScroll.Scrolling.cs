using DrawnUi.Maui.Draw;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw;

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
        var displacement = offsetFrom - offsetTo;

        //Debug.WriteLine($"[BOUNCE] {offsetFrom} - {offsetTo} with {velocity}");

        if (displacement != Vector2.Zero)
        {
            var spring = new Spring((float)(1 * (1 + RubberDamping)), 200, (float)(0.5f * (1 + RubberDamping)));
            _animatorBounce.Initialize(offsetTo, displacement, velocity, spring);
            _animatorBounce.Start();
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


    public bool StartToFlingFrom(Vector2 from, Vector2 velocity)
    {
        var contentOffset = from;// new Vector2((float)ViewportOffsetX, (float)ViewportOffsetY);

        _animatorFling.Initialize(contentOffset, velocity, 1f - DecelerationRatio);

        if (PrepareToFlingAfterInitialized())
        {
            _animatorFling.RunAsync(null).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    async Task<bool> FlingFrom(Vector2 from, Vector2 velocity)
    {
        //todo - add cancellation support

        //	Trace.WriteLine($"[FLING] velocity {velocity}");

        var contentOffset = from;// new Vector2((float)ViewportOffsetX, (float)ViewportOffsetY);

        _animatorFling.Initialize(contentOffset, velocity, 1f - DecelerationRatio);

        return await FlingAfterInitialized();
    }

    async Task<bool> FlingToAuto(Vector2 from, Vector2 to, float changeSpeedSecs = 0)
    {
        var velocity = _animatorFling.Parameters.VelocityToZero(from, to, changeSpeedSecs);

        _animatorFling.Initialize(from, velocity, 1f - DecelerationRatio);

        if (changeSpeedSecs > 0)
            _animatorFling.Speed = changeSpeedSecs;

        return await FlingAfterInitialized();
    }

    async Task<bool> FlingTo(Vector2 from, Vector2 to, float timeSeconds)
    {
        Vector2 velocity = _animatorFling.Parameters.VelocityTo(from, to, timeSeconds);

        _animatorFling.Initialize(from, velocity, 1f - DecelerationRatio);

        _animatorFling.Speed = timeSeconds;

        return await FlingAfterInitialized();
    }

    protected bool PrepareToFlingAfterInitialized()
    {
        var destination = _animatorFling.Parameters.Destination;

        var destinationPoint = new SKPoint(destination.X, destination.Y);

        _isSnapping = false;
        _changeSpeed = null;

        if (!OffsetOk(destination)) //we detected that scroll will end past the bounds
        {
            var contentRect = new SKRect(0, 0, ptsContentWidth, ptsContentHeight);
            var closestPoint = GetClosestSidePoint(destinationPoint, contentRect, Viewport.Units.Size);
            _axis = new(closestPoint.X, closestPoint.Y);

            //Trace.WriteLine($"[INTERSECTION] GetClosestSidePoint point {destination} content {contentRect} viewport {Viewport.Units.Size} => {closestPoint}");
            _changeSpeed = _animatorFling.Parameters.DurationToValue(new Vector2(closestPoint.X, closestPoint.Y));
            if (_changeSpeed != null)
            {
                _animatorFling.Speed = _changeSpeed.Value;
                //Trace.WriteLine($"[Fling] going to SIDE {closestPoint}");
            }
        }

        return _animatorFling.Speed > 0;
    }

    protected async Task<bool> FlingAfterInitialized()
    {
        if (PrepareToFlingAfterInitialized())
        {
            await _animatorFling.RunAsync(null);
            return true;
        }

        return false;
    }

    Vector2 _axis;
    double? _changeSpeed = null;
}
