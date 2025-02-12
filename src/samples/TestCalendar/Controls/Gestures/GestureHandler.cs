using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace AppoMobi.Touch
{

    public class GestureHandler : IGestureListener
    {
        public readonly static string[] AllProperties;

        public readonly static BindableProperty DownCommandProperty;

        public readonly static BindableProperty DownCommandParameterProperty;

        public readonly static BindableProperty UpCommandProperty;

        public readonly static BindableProperty UpCommandParameterProperty;

        public readonly static BindableProperty TappingCommandProperty;

        public readonly static BindableProperty TappingCommandParameterProperty;

        public readonly static BindableProperty TappedCommandProperty;

        public readonly static BindableProperty TappedCommandParameterProperty;

        public readonly static BindableProperty DoubleTappedCommandProperty;

        public readonly static BindableProperty DoubleTappedCommandParameterProperty;

        public readonly static BindableProperty LongPressingCommandProperty;

        public readonly static BindableProperty LongPressingCommandParameterProperty;

        public readonly static BindableProperty LongPressedCommandProperty;

        public readonly static BindableProperty LongPressedCommandParameterProperty;

        public readonly static BindableProperty PinchingCommandProperty;

        public readonly static BindableProperty PinchingCommandParameterProperty;

        public readonly static BindableProperty PinchedCommandProperty;

        public readonly static BindableProperty PinchedCommandParameterProperty;

        public readonly static BindableProperty PanningCommandProperty;

        public readonly static BindableProperty PanningCommandParameterProperty;

        public readonly static BindableProperty PannedCommandProperty;

        public readonly static BindableProperty PannedCommandParameterProperty;

        public readonly static BindableProperty SwipedCommandProperty;

        public readonly static BindableProperty SwipedCommandParameterProperty;

        public readonly static BindableProperty RotatingCommandProperty;

        public readonly static BindableProperty RotatingCommandParameterProperty;

        public readonly static BindableProperty RotatedCommandProperty;

        public readonly static BindableProperty RotatedCommandParameterProperty;
        IWithTouch Element;

        public bool HandlesDoubleTapped
        {
            get
            {
                if (this.DoubleTapped != null)
                {
                    return true;
                }
                return this.Element.DoubleTappedCommand != null;
            }
        }

        public bool HandlesDown
        {
            get
            {
                if (this.Down != null)
                {
                    return true;
                }
                return this.Element.DownCommand != null;
            }
        }

        public bool HandlesLongPressed
        {
            get
            {
                if (this.LongPressed != null)
                {
                    return true;
                }
                return this.Element.LongPressedCommand != null;
            }
        }

        public bool HandlesLongPressing
        {
            get
            {
                if (this.LongPressing != null)
                {
                    return true;
                }
                return this.Element.LongPressingCommand != null;
            }
        }

        public bool HandlesPanned
        {
            get
            {
                if (this.Panned != null)
                {
                    return true;
                }
                return this.Element.PannedCommand != null;
            }
        }

        public bool HandlesPanning
        {
            get
            {
                if (this.Panning != null)
                {
                    return true;
                }
                return this.Element.PanningCommand != null;
            }
        }

        public bool HandlesPinched
        {
            get
            {
                if (this.Pinched != null)
                {
                    return true;
                }
                return this.Element.PinchedCommand != null;
            }
        }

        public bool HandlesPinching
        {
            get
            {
                if (this.Pinching != null)
                {
                    return true;
                }
                return this.Element.PinchingCommand != null;
            }
        }

        public bool HandlesRotated
        {
            get
            {
                if (this.Rotated != null)
                {
                    return true;
                }
                return this.Element.RotatedCommand != null;
            }
        }

        public bool HandlesRotating
        {
            get
            {
                if (this.Rotating != null)
                {
                    return true;
                }
                return this.Element.RotatingCommand != null;
            }
        }

        public bool HandlesSwiped
        {
            get
            {
                if (this.Swiped != null)
                {
                    return true;
                }
                return this.Element.SwipedCommand != null;
            }
        }

        public bool HandlesTapped
        {
            get
            {
                if (this.Tapped != null)
                {
                    return true;
                }
                return this.Element.TappedCommand != null;
            }
        }

        public bool HandlesTapping
        {
            get
            {
                if (this.Tapping != null)
                {
                    return true;
                }
                return this.Element.TappingCommand != null;
            }
        }

        public bool HandlesUp
        {
            get
            {
                if (this.Up != null)
                {
                    return true;
                }
                return this.Element.UpCommand != null;
            }
        }

        static GestureHandler()
        {
            GestureHandler.AllProperties = new String[] { "Down", "DownCommand", "DownCommandParameter", "Up", "UpCommand", "UpCommandParameter", "Tapping", "TappingCommand", "TappingCommandParameter", "Tapped", "TappedCommand", "TappedCommandParameter", "DoubleTapped", "DoubleTappedCommand", "DoubleTappedCommandParameter", "LongPressing", "LongPressingCommand", "LongPressingCommandParameter", "LongPressed", "LongPressedCommand", "LongPressedCommandParameter", "Pinching", "PinchingCommand", "PinchingCommandParameter", "Pinched", "PinchedCommand", "PinchedCommandParameter", "Panning", "PanningCommand", "PanningCommandParameter", "Panned", "PannedCommand", "PannedCommandParameter", "Swiped", "SwipedCommand", "SwipedCommandParameter", "Rotating", "RotatingCommand", "RotatingCommandParameter", "Rotated", "RotatedCommand", "RotatedCommandParameter" };
            GestureHandler.DownCommandProperty = BindableProperty.Create("DownCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.DownCommandParameterProperty = BindableProperty.Create("DownCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.UpCommandProperty = BindableProperty.Create("UpCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.UpCommandParameterProperty = BindableProperty.Create("UpCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.TappingCommandProperty = BindableProperty.Create("TappingCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.TappingCommandParameterProperty = BindableProperty.Create("TappingCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.TappedCommandProperty = BindableProperty.Create("TappedCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.TappedCommandParameterProperty = BindableProperty.Create("TappedCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.DoubleTappedCommandProperty = BindableProperty.Create("DoubleTappedCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.DoubleTappedCommandParameterProperty = BindableProperty.Create("DoubleTappedCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.LongPressingCommandProperty = BindableProperty.Create("LongPressingCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.LongPressingCommandParameterProperty = BindableProperty.Create("LongPressingCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.LongPressedCommandProperty = BindableProperty.Create("LongPressedCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.LongPressedCommandParameterProperty = BindableProperty.Create("LongPressedCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.PinchingCommandProperty = BindableProperty.Create("PinchingCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.PinchingCommandParameterProperty = BindableProperty.Create("PinchingCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.PinchedCommandProperty = BindableProperty.Create("PinchedCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.PinchedCommandParameterProperty = BindableProperty.Create("PinchedCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.PanningCommandProperty = BindableProperty.Create("PanningCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.PanningCommandParameterProperty = BindableProperty.Create("PanningCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.PannedCommandProperty = BindableProperty.Create("PannedCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.PannedCommandParameterProperty = BindableProperty.Create("PannedCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.SwipedCommandProperty = BindableProperty.Create("SwipedCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.SwipedCommandParameterProperty = BindableProperty.Create("SwipedCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.RotatingCommandProperty = BindableProperty.Create("RotatingCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.RotatingCommandParameterProperty = BindableProperty.Create("RotatingCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.RotatedCommandProperty = BindableProperty.Create("RotatedCommand", typeof(ICommand), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
            GestureHandler.RotatedCommandParameterProperty = BindableProperty.Create("RotatedCommandParameter", typeof(Object), typeof(GestureHandler), null, BindingMode.OneWay, null, null, null, null, null);
        }

        public GestureHandler(IWithTouch element)
        {
            this.Element = element;
        }

        void ExecuteCommand(ICommand command, object parameter, BaseGestureEventArgs args)
        {
            object obj = parameter;
            if (command != null)
            {
                args.Sender = this.Element;
                object obj1 = obj;
                if (obj1 == null)
                {
                    obj1 = args;
                }
                obj = obj1;
                if (command.CanExecute(obj))
                {
                    MainThread.BeginInvokeOnMainThread(() => command.Execute(obj));
                }
            }
        }

        public bool OnDoubleTapped(TapEventArgs args)
        {
            bool handled = false;
            if (this.HandlesDoubleTapped)
            {

                this.RaiseEvent<TapEventArgs>(this.DoubleTapped, args);
                ICommand doubleTappedCommand = null;
                object doubleTappedCommandParameter = null;
                try
                {
                    doubleTappedCommand = this.Element.DoubleTappedCommand;
                    doubleTappedCommandParameter = this.Element.DoubleTappedCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(doubleTappedCommand, doubleTappedCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public virtual bool OnDown(DownUpEventArgs args)
        {
            bool handled = false;
            if (this.HandlesDown)
            {
                this.RaiseEvent<DownUpEventArgs>(this.Down, args);
                ICommand downCommand = null;
                object downCommandParameter = null;
                try
                {
                    downCommand = this.Element.DownCommand;
                    downCommandParameter = this.Element.DownCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(downCommand, downCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public bool OnLongPressed(LongPressEventArgs args)
        {
            bool handled = false;
            if (this.HandlesLongPressed)
            {

                this.RaiseEvent<LongPressEventArgs>(this.LongPressed, args);
                ICommand longPressedCommand = null;
                object longPressedCommandParameter = null;
                try
                {
                    longPressedCommand = this.Element.LongPressedCommand;
                    longPressedCommandParameter = this.Element.LongPressedCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(longPressedCommand, longPressedCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public bool OnLongPressing(LongPressEventArgs args)
        {
            bool handled = false;
            if (this.HandlesLongPressing)
            {

                this.RaiseEvent<LongPressEventArgs>(this.LongPressing, args);
                ICommand longPressingCommand = null;
                object longPressingCommandParameter = null;
                try
                {
                    longPressingCommand = this.Element.LongPressingCommand;
                    longPressingCommandParameter = this.Element.LongPressingCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(longPressingCommand, longPressingCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public bool OnPanned(PanEventArgs args)
        {
            bool handled = false;
            if (this.HandlesPanned)
            {

                this.RaiseEvent<PanEventArgs>(this.Panned, args);
                ICommand pannedCommand = null;
                object pannedCommandParameter = null;
                try
                {
                    pannedCommand = this.Element.PannedCommand;
                    pannedCommandParameter = this.Element.PannedCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(pannedCommand, pannedCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public bool OnPanning(PanEventArgs args)
        {
            bool handled = false;
            if (this.HandlesPanning)
            {

                this.RaiseEvent<PanEventArgs>(this.Panning, args);
                ICommand panningCommand = null;
                object panningCommandParameter = null;
                try
                {
                    panningCommand = this.Element.PanningCommand;
                    panningCommandParameter = this.Element.PanningCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(panningCommand, panningCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public bool OnPinched(PinchEventArgs args)
        {
            bool handled = false;
            if (this.HandlesPinched)
            {

                this.RaiseEvent<PinchEventArgs>(this.Pinched, args);
                ICommand pinchedCommand = null;
                object pinchedCommandParameter = null;
                try
                {
                    pinchedCommand = this.Element.PinchedCommand;
                    pinchedCommandParameter = this.Element.PinchedCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(pinchedCommand, pinchedCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public bool OnPinching(PinchEventArgs args)
        {
            bool handled = false;
            if (this.HandlesPinching)
            {

                this.RaiseEvent<PinchEventArgs>(this.Pinching, args);
                ICommand pinchingCommand = null;
                object pinchingCommandParameter = null;
                try
                {
                    pinchingCommand = this.Element.PinchingCommand;
                    pinchingCommandParameter = this.Element.PinchingCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(pinchingCommand, pinchingCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public bool OnRotated(RotateEventArgs args)
        {
            bool handled = false;
            if (this.HandlesRotated)
            {

                this.RaiseEvent<RotateEventArgs>(this.Rotated, args);
                ICommand rotatedCommand = null;
                object rotatedCommandParameter = null;
                try
                {
                    rotatedCommand = this.Element.RotatedCommand;
                    rotatedCommandParameter = this.Element.RotatedCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(rotatedCommand, rotatedCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public bool OnRotating(RotateEventArgs args)
        {
            bool handled = false;
            if (this.HandlesRotating)
            {

                this.RaiseEvent<RotateEventArgs>(this.Rotating, args);
                ICommand rotatingCommand = null;
                object rotatingCommandParameter = null;
                try
                {
                    rotatingCommand = this.Element.RotatingCommand;
                    rotatingCommandParameter = this.Element.RotatingCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(rotatingCommand, rotatingCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public bool OnSwiped(SwipeEventArgs args)
        {
            bool handled = false;
            if (this.HandlesSwiped)
            {

                this.RaiseEvent<SwipeEventArgs>(this.Swiped, args);
                ICommand swipedCommand = null;
                object swipedCommandParameter = null;
                try
                {
                    swipedCommand = this.Element.SwipedCommand;
                    swipedCommandParameter = this.Element.SwipedCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(swipedCommand, swipedCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public virtual bool OnTapped(TapEventArgs args)
        {
            bool handled = false;
            if (this.HandlesTapped)
            {

                this.RaiseEvent<TapEventArgs>(this.Tapped, args);
                ICommand tappedCommand = null;
                object tappedCommandParameter = null;
                try
                {
                    tappedCommand = this.Element.TappedCommand;
                    tappedCommandParameter = this.Element.TappedCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(tappedCommand, tappedCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public bool OnTapping(TapEventArgs args)
        {
            bool handled = false;
            if (this.HandlesTapping)
            {

                this.RaiseEvent<TapEventArgs>(this.Tapping, args);
                ICommand tappingCommand = null;
                object tappingCommandParameter = null;
                try
                {
                    tappingCommand = this.Element.TappingCommand;
                    tappingCommandParameter = this.Element.TappingCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(tappingCommand, tappingCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        public virtual bool OnUp(DownUpEventArgs args)
        {
            bool handled = false;
            if (this.HandlesUp)
            {

                this.RaiseEvent<DownUpEventArgs>(this.Up, args);
                ICommand upCommand = null;
                object upCommandParameter = null;
                try
                {
                    upCommand = this.Element.UpCommand;
                    upCommandParameter = this.Element.UpCommandParameter;
                }
                catch (Exception exception)
                {
                }
                this.ExecuteCommand(upCommand, upCommandParameter, args);
                handled = args.Handled;
            }
            return handled;
        }

        void RaiseEvent<T>(EventHandler<T> handler, T args)
        where T : BaseGestureEventArgs
        {
            args.Sender = this.Element;
            if (handler != null)
            {
                MainThread.BeginInvokeOnMainThread(() => handler(this.Element, args));
            }
        }

        public event EventHandler<TapEventArgs> DoubleTapped;

        public event EventHandler<DownUpEventArgs> Down;

        public event EventHandler<LongPressEventArgs> LongPressed;

        public event EventHandler<LongPressEventArgs> LongPressing;

        public event EventHandler<PanEventArgs> Panned;

        public event EventHandler<PanEventArgs> Panning;

        public event EventHandler<PinchEventArgs> Pinched;

        public event EventHandler<PinchEventArgs> Pinching;

        public event EventHandler<RotateEventArgs> Rotated;

        public event EventHandler<RotateEventArgs> Rotating;

        public event EventHandler<SwipeEventArgs> Swiped;

        public event EventHandler<TapEventArgs> Tapped;

        public event EventHandler<TapEventArgs> Tapping;

        public event EventHandler<DownUpEventArgs> Up;

    }

    /// <summary>
    /// Only raises the events, if the finger(s) were moved more than Settings.MinimumDeltaDistance/MinimumDeltaAngle.
    /// </summary>
    public class GestureThrottler : IGestureListener
    {
	    IGestureListener listener;
	    PanEventArgs lastPanArgs;
	    PinchEventArgs lastPinchArgs;
	    RotateEventArgs lastRotateArgs;

        public GestureThrottler(IGestureListener nextListener)
        {
            this.listener = nextListener;
        }

        public bool OnDoubleTapped(TapEventArgs args)
        {
            return this.listener.OnDoubleTapped(args);
        }

        public bool OnDown(DownUpEventArgs args)
        {
            return this.listener.OnDown(args);
        }

        public bool OnLongPressed(LongPressEventArgs args)
        {
            return this.listener.OnLongPressed(args);
        }

        public bool OnLongPressing(LongPressEventArgs args)
        {
            return this.listener.OnLongPressing(args);
        }

        public bool OnPanned(PanEventArgs args)
        {
            if (this.lastPanArgs != null)
            {
                args = args.Diff(this.lastPanArgs);
                this.lastPanArgs = null;
            }
            return this.listener.OnPanned(args);
        }

        public bool OnPanning(PanEventArgs args)
        {
            bool flag = false;
            if (this.lastPanArgs != null)
            {
                PanEventArgs panEventArg = args.Diff(this.lastPanArgs);
                if (Math.Abs(panEventArg.DeltaDistance.X) > Settings.MinimumDeltaDistance || Math.Abs(panEventArg.DeltaDistance.Y) > Settings.MinimumDeltaDistance)
                {
                    args = panEventArg;
                }
                else
                {
                    args = null;
                }
            }
            if (args != null)
            {
                flag = this.listener.OnPanning(args);
                this.lastPanArgs = args;
            }
            return flag;
        }

        public bool OnPinched(PinchEventArgs args)
        {
            if (this.lastPinchArgs != null)
            {
                args = args.Diff(this.lastPinchArgs);
                this.lastPinchArgs = null;
            }
            return this.listener.OnPinched(args);
        }

        public bool OnPinching(PinchEventArgs args)
        {
            bool flag = false;
            if (this.lastPinchArgs != null)
            {
                PinchEventArgs pinchEventArg = args.Diff(this.lastPinchArgs);
                if (Math.Abs(pinchEventArg.DeltaScale - 1) <= Settings.MinimumDeltaScale)
                {
                    args = null;
                }
                else
                {
                    args = pinchEventArg;
                }
            }
            if (args != null)
            {
                flag = this.listener.OnPinching(args);
                this.lastPinchArgs = args;
            }
            return flag;
        }

        public bool OnRotated(RotateEventArgs args)
        {
            if (this.lastRotateArgs != null)
            {
                args = args.Diff(this.lastRotateArgs);
                this.lastRotateArgs = null;
            }
            return this.listener.OnRotated(args);
        }

        public bool OnRotating(RotateEventArgs args)
        {
            bool flag = false;
            if (this.lastRotateArgs != null)
            {
                RotateEventArgs rotateEventArg = args.Diff(this.lastRotateArgs);
                if (Math.Abs(rotateEventArg.DeltaAngle) <= Settings.MinimumDeltaAngle)
                {
                    args = null;
                }
                else
                {
                    args = rotateEventArg;
                }
            }
            if (args != null)
            {
                flag = this.listener.OnRotating(args);
                this.lastRotateArgs = args;
            }
            return flag;
        }

        public bool OnSwiped(SwipeEventArgs args)
        {
            this.lastPanArgs = null;
            return this.listener.OnSwiped(args);
        }

        public bool OnTapped(TapEventArgs args)
        {
            return this.listener.OnTapped(args);
        }

        public bool OnTapping(TapEventArgs args)
        {
            return this.listener.OnTapping(args);
        }

        public bool OnUp(DownUpEventArgs args)
        {
            return this.listener.OnUp(args);
        }
    }


}