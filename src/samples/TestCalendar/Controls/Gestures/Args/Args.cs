using System;
 
using AppoMobi.Maui.Gestures;
 
using  Microsoft.Maui.Graphics;


namespace AppoMobi.Touch
{

    /// <summary>
    /// The arguments for the <code>Down</code> and <code>Up</code> events.
    /// </summary>
    public class DownUpEventArgs : BaseGestureEventArgs
    {
        /// <summary>
        /// The touches which triggered the Down or Up event.
        /// </summary>
        public virtual int[] TriggeringTouches
        {
            get;
            protected set;
        }

        public DownUpEventArgs()
        {
        }

        public DownUpEventArgs(TouchActionEventArgs args)
        {
            this.Cancelled = !args.IsInContact;
            this.Touches = new Point[]
            {
                new Point(args.Location.X, args.Location.Y)
            };
        }
    }

    /// <summary>
    /// The arguments for the <code>Tapping</code>, <code>Tapped</code> and <code>DoubleTapped</code> events.
    /// </summary>
    public class TapEventArgs : BaseGestureEventArgs
    {
        /// <summary>
        /// The number of taps in a short period of time (~250ms).
        /// </summary>
        public virtual int NumberOfTaps
        {
            get;
            protected set;
        }

        public TapEventArgs()
        {
        }

        public TapEventArgs(TouchActionEventArgs args)
        {
            this.Cancelled = !args.IsInContact;
            this.Touches = new Point[]
            {
                new Point(args.Location.X, args.Location.Y)
            };
            this.NumberOfTaps = 1;
        }
    }

    /// <summary>
    /// The arguments for the <code>Rotating</code> and <code>Rotated</code> events.
    /// </summary>
    public class RotateEventArgs : BaseGestureEventArgs
    {
        /// <summary>
        /// The angle a line from the first to the second finger currently has on the screen.
        /// </summary>
        public virtual double Angle
        {
            get;
            protected set;
        }

        /// <summary>
        /// The angle the fingers were rotated on the screen compared to the last time this event was raised.
        /// </summary>
        public virtual double DeltaAngle
        {
            get;
            protected set;
        }

        /// <summary>
        /// The angle the fingers were rotated on the screen compared to the start of the gesture.
        /// </summary>
        public virtual double TotalAngle
        {
            get;
            protected set;
        }

        public RotateEventArgs()
        {
        }

        protected void CalculateAngles(RotateEventArgs previous)
        {
            if ((int)this.Touches.Length > 1)
            {
                this.Angle = this.GetAngle();
            }
            else if (previous == null)
            {
                this.Angle = 0;
            }
            else
            {
                this.Angle = previous.Angle;
            }
            if (previous == null)
            {
                this.DeltaAngle = 0;
                this.TotalAngle = 0;
                return;
            }
            if ((int)this.Touches.Length != (int)previous.Touches.Length)
            {
                this.DeltaAngle = 0;
                this.TotalAngle = previous.TotalAngle;
                return;
            }
            this.DeltaAngle = this.Angle - previous.Angle;
            this.TotalAngle = previous.TotalAngle + this.DeltaAngle;
        }

        internal RotateEventArgs Diff(RotateEventArgs lastArgs)
        {
            return new RotateEventArgs()
            {
                Cancelled = this.Cancelled,
                Handled = base.Handled,
                ViewPosition = this.ViewPosition,
                Touches = this.Touches,
                Sender = this.Sender,
                Angle = this.Angle,
                TotalAngle = this.TotalAngle,
                DeltaAngle = this.TotalAngle - lastArgs.TotalAngle
            };
        }

        double GetAngle()
        {
            double x = this.Touches[1].X - this.Touches[0].X;
            return Math.Atan2(this.Touches[1].Y - this.Touches[0].Y, x) * 180 / 3.14159265358979;
        }
    }

    /// <summary>
    /// The arguments for the <code>Panning</code> and <code>Panned</code> events.
    /// </summary>
    public class PanEventArgs : BaseGestureEventArgs
    {
        /// <summary>
        /// The distance in X/Y that the finger was moved since this event was raised the last time.
        /// </summary>
        public virtual Point DeltaDistance
        {
            get;
            protected set;
        }

        /// <summary>
        /// The distance in X/Y that the finger was moved since the pan gesture began.
        /// </summary>
        public virtual Point TotalDistance
        {
            get;
            protected set;
        }

        /// <summary>
        /// The velocity of the finger in X/Y.
        /// </summary>
        public virtual Point Velocity
        {
            get;
            protected set;
        }

        public PanEventArgs()
        {
        }

        protected void CalculateDistances(BaseGestureEventArgs previous)
        {
            PanEventArgs panEventArg = previous as PanEventArgs;
            if (previous == null)
            {
                this.DeltaDistance = Point.Zero;
                this.TotalDistance = Point.Zero;
                return;
            }
            if ((int)this.Touches.Length != (int)previous.Touches.Length)
            {
                this.DeltaDistance = Point.Zero;
                this.TotalDistance = (panEventArg != null ? panEventArg.TotalDistance : Point.Zero);
                return;
            }
            this.DeltaDistance = this.Center.Subtract(previous.Center);
            this.TotalDistance = (panEventArg != null ? panEventArg.TotalDistance.Add(this.DeltaDistance) : this.DeltaDistance);
        }

        internal PanEventArgs Diff(PanEventArgs lastArgs)
        {
            return new PanEventArgs()
            {
                Cancelled = this.Cancelled,
                Handled = base.Handled,
                ViewPosition = this.ViewPosition,
                Touches = this.Touches,
                Sender = this.Sender,
                Velocity = this.Velocity,
                TotalDistance = this.TotalDistance,
                DeltaDistance = this.TotalDistance.Subtract(lastArgs.TotalDistance)
            };
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            PanEventArgs panEventArg = obj as PanEventArgs;
            if (panEventArg == null)
            {
                return false;
            }
            return this.Equals(panEventArg);
        }

        public bool Equals(PanEventArgs other)
        {
            if (other == null)
            {
                return false;
            }
            if (!this.DeltaDistance.Equals(other.DeltaDistance))
            {
                return false;
            }
            if (!this.TotalDistance.Equals(other.TotalDistance))
            {
                return false;
            }
            if (!this.Velocity.Equals(other.Velocity))
            {
                return false;
            }
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();
            Point deltaDistance = this.DeltaDistance;
            int num = hashCode ^ deltaDistance.GetHashCode();
            deltaDistance = this.TotalDistance;
            return num ^ deltaDistance.GetHashCode() ^ this.Velocity.GetHashCode();
        }
    }

    /// <summary>
    /// The arguments for the <code>LongPressing</code> and <code>LongPressed</code> events.
    /// </summary>
    public class LongPressEventArgs : AppoMobi.Touch.TapEventArgs
    {
        /// <summary>
        /// Duration of long press in milliseconds.
        /// </summary>
        public virtual long Duration
        {
            get;
            protected set;
        }

        public LongPressEventArgs()
        {
        }
    }

    /// <summary>
    /// A simple filter so that events are only raised if Element.InputTransparent == false.
    /// </summary>
    public  class GestureFilter : IGestureListener
    {
	    VisualElement element;
	    IGestureListener listener;

	    bool RaiseEvent
        {
            get
            {
                if (this.element == null)
                {
                    return true;
                }
                return !this.element.InputTransparent;
            }
        }

        public GestureFilter(IWithTouch element, IGestureListener nextListener)
        {
            this.element = element as VisualElement;
            this.listener = nextListener;
        }

        public bool OnDoubleTapped(TapEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnDoubleTapped(args);
        }

        public bool OnDown(DownUpEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnDown(args);
        }

        public bool OnLongPressed(LongPressEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnLongPressed(args);
        }

        public bool OnLongPressing(LongPressEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnLongPressing(args);
        }

        public bool OnPanned(PanEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnPanned(args);
        }

        public bool OnPanning(PanEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnPanning(args);
        }

        public bool OnPinched(PinchEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnPinched(args);
        }

        public bool OnPinching(PinchEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnPinching(args);
        }

        public bool OnRotated(RotateEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnRotated(args);
        }

        public bool OnRotating(RotateEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnRotating(args);
        }

        public bool OnSwiped(SwipeEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnSwiped(args);
        }

        public bool OnTapped(TapEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnTapped(args);
        }

        public bool OnTapping(TapEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnTapping(args);
        }

        public bool OnUp(DownUpEventArgs args)
        {
            if (!this.RaiseEvent)
            {
                return false;
            }
            return this.listener.OnUp(args);
        }
    }

    /// <summary>
    /// The direction or <value>NotClear</value> if the direction is not without any doubt.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// The swipe gesture was not in a clear direction.
        /// </summary>
        NotClear,
        Left,
        Right,
        Up,
        Down
    }

    /// <summary>
    /// The base class for all gesture event args.
    /// </summary>
    public class BaseGestureEventArgs : EventArgs
    {
	    Point center;

        /// <summary>
        /// Android and iOS sometimes cancel a touch gesture. In this case we raise a *ed event with Cancelled set to <value>true</value>.
        /// </summary>
        public virtual bool Cancelled
        {
            get;
            protected set;
        }

        /// <summary>
        /// The center of the fingers on the screen.
        /// </summary>
        public virtual Point Center
        {
            get
            {
                if (this.center.IsEmpty)
                {
                    this.center = this.Touches.Center();
                }
                return this.center;
            }
            protected set
            {
                this.center = value;
            }
        }

        /// <summary>
        /// This flag could be set to false to run other eventhandlers of the same or surrounding elements.
        /// It is not used on every platform! Default value is <value>true</value>.
        /// </summary>
        public bool Handled { get; set; } = true;

        /// <summary>
        /// The number of fingers on the screen.
        /// </summary>
        public virtual int NumberOfTouches
        {
            get
            {
                Point[] touches = this.Touches;
                if (touches == null)
                {
                    return 0;
                }
                return (int)touches.Length;
            }
        }

        /// <summary>
        /// The element which raised this event.
        /// </summary>
        public virtual IWithTouch Sender
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the position of the fingers on the screen.
        /// </summary>
        public virtual Point[] Touches
        {
            get;
            protected set;
        }

        /// <summary>
        /// The position of the <see cref="P:AppoMobi.Touch.BaseGestureEventArgs.Sender" /> on the screen.
        /// </summary>
        public virtual Rect ViewPosition
        {
            get;
            protected set;
        }

        public BaseGestureEventArgs()
        {
        }

        public override bool Equals(object obj)
        {
            BaseGestureEventArgs baseGestureEventArg = obj as BaseGestureEventArgs;
            BaseGestureEventArgs baseGestureEventArg1 = baseGestureEventArg;
            if (baseGestureEventArg == null)
            {
                return false;
            }
            return this.Equals(baseGestureEventArg1);
        }

        public bool Equals(BaseGestureEventArgs other)
        {
            if (other == null)
            {
                return false;
            }
            if (this.Touches == null && other.Touches == null)
            {
                return true;
            }
            if ((int)this.Touches.Length != (int)other.Touches.Length)
            {
                return false;
            }
            for (int i = 0; i < (int)this.Touches.Length; i++)
            {
                if (!this.Touches[i].Equals(other.Touches[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.Touches.GetHashCode();
        }
    }


    /// <summary>
    /// The arguments for the <code>Pinching</code> and <code>Pinched</code> events.
    /// </summary>
    public class PinchEventArgs : BaseGestureEventArgs
    {
        /// <summary>
        /// The relative distance between the two fingers on the screen compared to the last time this event was raised.
        /// </summary>
        public virtual double DeltaScale
        {
            get;
            protected set;
        }

        /// <summary>
        /// The relative X distance between the two fingers on the screen compared to the last time this event was raised.
        /// </summary>
        public virtual double DeltaScaleX
        {
            get;
            protected set;
        }

        /// <summary>
        /// The relative Y distance between the two fingers on the screen compared to the last time this event was raised.
        /// </summary>
        public virtual double DeltaScaleY
        {
            get;
            protected set;
        }

        /// <summary>
        /// The distance between the first two fingers.
        /// </summary>
        public virtual double Distance
        {
            get;
            protected set;
        }

        /// <summary>
        /// The X distance between the first two fingers.
        /// </summary>
        public virtual double DistanceX
        {
            get;
            protected set;
        }

        /// <summary>
        /// The Y distance between the first two fingers.
        /// </summary>
        public virtual double DistanceY
        {
            get;
            protected set;
        }

        /// <summary>
        /// The relative distance between the two fingers on the screen compared to when the gesture started.
        /// </summary>
        public virtual double TotalScale
        {
            get;
            protected set;
        }

        /// <summary>
        /// The relative X distance between the two fingers on the screen compared to when the gesture started.
        /// </summary>
        public virtual double TotalScaleX
        {
            get;
            protected set;
        }

        /// <summary>
        /// The relative Y distance between the two fingers on the screen compared to when the gesture started.
        /// </summary>
        public virtual double TotalScaleY
        {
            get;
            protected set;
        }

        public PinchEventArgs()
        {
        }

        //[return: TupleElementNames(new string[] { "delta", "total" })]
        ValueTuple<double , double> CalculateScale(double distance, double previousDistance, double previousTotalScale)
        {
            if (distance < 0)
            {
                distance = -distance;
            }
            if (previousDistance < 0)
            {
                previousDistance = -previousDistance;
            }
            double num = (distance < Settings.MinimumDistanceForScale || previousDistance < Settings.MinimumDistanceForScale ? 1 : distance / previousDistance);
            return new ValueTuple<double, double>(num, previousTotalScale * num);
        }

        protected void CalculateScales(PinchEventArgs previous)
        {
            double num;
            double num1;
            if ((int)this.Touches.Length > 1)
            {
                this.DistanceX = this.Touches[1].X - this.Touches[0].X;
                this.DistanceY = this.Touches[1].Y - this.Touches[0].Y;
                this.Distance = Math.Sqrt(this.DistanceX * this.DistanceX + this.DistanceY * this.DistanceY);
            }
            else if (previous == null)
            {
                double num2 = 0;
                num1 = num2;
                this.DistanceY = num2;
                double num3 = num1;
                num = num3;
                this.DistanceX = num3;
                this.Distance = num;
            }
            else
            {
                this.Distance = previous.Distance;
                this.DistanceX = previous.DistanceX;
                this.DistanceY = previous.DistanceY;
            }
            if (previous == null)
            {
                double num4 = 1;
                num1 = num4;
                this.DeltaScaleY = num4;
                double num5 = num1;
                num = num5;
                this.DeltaScaleX = num5;
                this.DeltaScale = num;
                double num6 = 1;
                num1 = num6;
                this.TotalScaleY = num6;
                double num7 = num1;
                num = num7;
                this.TotalScaleX = num7;
                this.TotalScale = num;
                return;
            }
            if ((int)this.Touches.Length != (int)previous.Touches.Length)
            {
                double num8 = 1;
                num1 = num8;
                this.DeltaScaleY = num8;
                double num9 = num1;
                num = num9;
                this.DeltaScaleX = num9;
                this.DeltaScale = num;
                this.TotalScale = previous.TotalScale;
                this.TotalScaleX = previous.TotalScaleX;
                this.TotalScaleY = previous.TotalScaleY;
                return;
            }
            ValueTuple<double, double> valueTuple = this.CalculateScale(this.Distance, previous.Distance, previous.TotalScale);
            
 
            this.DeltaScale = valueTuple.Item1;
 
            this.TotalScale = valueTuple.Item2;
            valueTuple = this.CalculateScale(this.DistanceX, previous.DistanceX, previous.TotalScaleX);
 
            this.DeltaScaleX = valueTuple.Item1;
    
            this.TotalScaleX = valueTuple.Item2;
            valueTuple = this.CalculateScale(this.DistanceY, previous.DistanceY, previous.TotalScaleY);
    
            this.DeltaScaleY = valueTuple.Item1;
  
            this.TotalScaleY = valueTuple.Item2;
        }

        internal PinchEventArgs Diff(PinchEventArgs lastArgs)
        {
            return new PinchEventArgs()
            {
                Cancelled = this.Cancelled,
                Handled = base.Handled,
                ViewPosition = this.ViewPosition,
                Touches = this.Touches,
                Sender = this.Sender,
                Distance = this.Distance,
                DistanceX = this.DistanceX,
                DistanceY = this.DistanceY,
                TotalScale = this.TotalScale,
                TotalScaleX = this.TotalScaleX,
                TotalScaleY = this.TotalScaleY,
                DeltaScale = this.TotalScale / lastArgs.TotalScale,
                DeltaScaleX = this.TotalScaleX / lastArgs.TotalScaleX,
                DeltaScaleY = this.TotalScaleY / lastArgs.TotalScaleY
            };
        }
    }

    /// <summary>
    /// The argument for the <code>Swiped</code> event.
    /// </summary>
    public class SwipeEventArgs : PanEventArgs
    {
        /// <summary>
        /// The direction in which the finger moved when it was lifted from the screen.
        /// </summary>
        public Direction Direction
        {
            get;
            protected set;
        }

        public SwipeEventArgs()
        {
        }

        public SwipeEventArgs(PanEventArgs panArgs, Direction direction)
        {
            this.Cancelled = panArgs.Cancelled;
            this.ViewPosition = panArgs.ViewPosition;
            this.Touches = panArgs.Touches;
            this.DeltaDistance = panArgs.DeltaDistance;
            this.TotalDistance = panArgs.TotalDistance;
            this.Velocity = panArgs.Velocity;
            this.Direction = direction;
        }
    }

}