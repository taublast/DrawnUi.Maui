
using System;
using System.Windows.Input;

namespace AppoMobi.Touch
{

    public interface IWithTouch
    {
        bool InputTransparent { get; }

        /// <summary>
        /// Gets or sets the command which is executed when the element is tapped twice. This is a bindable property.
        /// </summary>
        ICommand DoubleTappedCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the DoubleTappedCommand. This is a bindable property.
        /// </summary>
        object DoubleTappedCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when a finger comes down. This is a bindable property.
        /// </summary>
        ICommand DownCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the DownCommand. This is a bindable property.
        /// </summary>
        object DownCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// The object which handles all the gestures.
        /// </summary>
        GestureHandler GestureHandler
        {
            get;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the LongPressed event is raised. This is a bindable property.
        /// </summary>
        ICommand LongPressedCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the LongPressedCommand. This is a bindable property.
        /// </summary>
        object LongPressedCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is pressed long. This is a bindable property.
        /// </summary>
        ICommand LongPressingCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the LongPressingCommand. This is a bindable property.
        /// </summary>
        object LongPressingCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element was panned. This is a bindable property.
        /// </summary>
        ICommand PannedCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the PannedCommand. This is a bindable property.
        /// </summary>
        object PannedCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is panned. This is a bindable property.
        /// </summary>
        ICommand PanningCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the PanningCommand. This is a bindable property.
        /// </summary>
        object PanningCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the Pinched event is raised. This is a bindable property.
        /// </summary>
        ICommand PinchedCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the PinchedCommand. This is a bindable property.
        /// </summary>
        object PinchedCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is pinched. This is a bindable property.
        /// </summary>
        ICommand PinchingCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the PinchingCommand. This is a bindable property.
        /// </summary>
        object PinchingCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element was rotated. This is a bindable property.
        /// </summary>
        ICommand RotatedCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the RotatedCommand. This is a bindable property.
        /// </summary>
        object RotatedCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is rotated. This is a bindable property.
        /// </summary>
        ICommand RotatingCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the RotatingCommand. This is a bindable property.
        /// </summary>
        object RotatingCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is swiped. This is a bindable property.
        /// </summary>
        ICommand SwipedCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the SwipedCommand. This is a bindable property.
        /// </summary>
        object SwipedCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is tapped. This is a bindable property.
        /// </summary>
        ICommand TappedCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the TappedCommand. This is a bindable property.
        /// </summary>
        object TappedCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is tapped and another tap may follow. This is a bindable property.
        /// </summary>
        ICommand TappingCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the TappingCommand. This is a bindable property.
        /// </summary>
        object TappingCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command which is executed when a finger is liftet from the touch screen. This is a bindable property.
        /// </summary>
        ICommand UpCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the UpCommand. This is a bindable property.
        /// </summary>
        object UpCommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// The event which is raised when two Tapping events came within ~250ms.
        /// </summary>
        event EventHandler<TapEventArgs> DoubleTapped;

        /// <summary>
        /// The event which is raised when a finger comes down. The finger(s) is/are still on the touch screen.
        /// </summary>
        event EventHandler<DownUpEventArgs> Down;

        /// <summary>
        /// The event which is raised when a finger finally comes up after a LongPressing event.
        /// </summary>
        event EventHandler<LongPressEventArgs> LongPressed;

        /// <summary>
        /// The event which is raised when a finger comes down and stays there.
        /// </summary>
        event EventHandler<LongPressEventArgs> LongPressing;

        /// <summary>
        /// The event which is raised when a finger comes up after a Panning event.
        /// </summary>
        event EventHandler<PanEventArgs> Panned;

        /// <summary>
        /// The event which is raised when a finger comes down and then moves in any direction.
        /// </summary>
        event EventHandler<PanEventArgs> Panning;

        /// <summary>
        /// The event which is raised when at least one finger is released after a Pinching event.
        /// </summary>
        event EventHandler<PinchEventArgs> Pinched;

        /// <summary>
        /// The event which is raised when two fingers are moved together or away from each other.
        /// </summary>
        event EventHandler<PinchEventArgs> Pinching;

        /// <summary>
        /// The event which is raised when at least one finger is released after a Rotating event.
        /// </summary>
        event EventHandler<RotateEventArgs> Rotated;

        /// <summary>
        /// The event which is raised when two fingers come down and their angle is changed.
        /// </summary>
        event EventHandler<RotateEventArgs> Rotating;

        /// <summary>
        /// The event which is raised when a finger comes down, is moved and still moves when it is raised again.
        /// </summary>
        event EventHandler<SwipeEventArgs> Swiped;

        /// <summary>
        /// The event which is raised when no other touch gesture followed Tapping within 250ms.
        /// </summary>
        event EventHandler<TapEventArgs> Tapped;

        /// <summary>
        /// The event which is raised when a finger comes down and up again.
        /// </summary>
        event EventHandler<TapEventArgs> Tapping;

        /// <summary>
        /// The event which is raised when a finger is lifted from the touch screen. There may be other fingers still on it.
        /// </summary>
        event EventHandler<DownUpEventArgs> Up;
    }

    public interface IGestureListener
    {
        bool OnDoubleTapped(TapEventArgs args);

        bool OnDown(DownUpEventArgs args);

        bool OnLongPressed(LongPressEventArgs args);

        bool OnLongPressing(LongPressEventArgs args);

        bool OnPanned(PanEventArgs args);

        bool OnPanning(PanEventArgs args);

        bool OnPinched(PinchEventArgs args);

        bool OnPinching(PinchEventArgs args);

        bool OnRotated(RotateEventArgs args);

        bool OnRotating(RotateEventArgs args);

        bool OnSwiped(SwipeEventArgs args);

        bool OnTapped(TapEventArgs args);

        bool OnTapping(TapEventArgs args);

        bool OnUp(DownUpEventArgs args);
    }
}