using System;
using System.Windows.Input;
using AppoMobi.Maui.Gestures;
using DrawnUi.Maui.Draw;

namespace AppoMobi.Touch
{
    public class LegacyGesturesStackLayout : Microsoft.Maui.Controls.StackLayout, IWithTouch, AppoMobi.Maui.Gestures.IGestureListener
    {
        #region MODERN

        static void GesturesChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is LegacyGesturesStackLayout control)
            {
                control.OnGesturesAttachChanged();
            }
        }

        public static readonly BindableProperty GesturesProperty = BindableProperty.Create(
            nameof(GesturesMode),
            typeof(GesturesMode),
            typeof(LegacyGesturesStackLayout),
            GesturesMode.Enabled, propertyChanged: GesturesChanged);

        public GesturesMode Gestures
        {
            get { return (GesturesMode)GetValue(GesturesProperty); }
            set { SetValue(GesturesProperty, value); }
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            OnGesturesAttachChanged();
        }

        protected virtual void OnGesturesAttachChanged()
        {
            if (Handler == null)
            {
                TouchEffect.SetForceAttach(this, false);
                return;
            }

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
            }
        }

        public void OnGestureEvent(TouchActionType type, TouchActionEventArgs args, TouchActionResult action)
        {
            GesturesProcessor.OnGestureEvent(this, type, args, action);
        }

        #endregion

        GestureHandler gestureHandler;

        /// <summary>
        /// The BindableProperty for the DownCommand.
        /// </summary>
        public readonly static BindableProperty DownCommandProperty;

        /// <summary>
        /// The BindableProperty for the DownCommandParameter.
        /// </summary>
        public readonly static BindableProperty DownCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the UpCommand.
        /// </summary>
        public readonly static BindableProperty UpCommandProperty;

        /// <summary>
        /// The BindableProperty for the UpCommandParameter.
        /// </summary>
        public readonly static BindableProperty UpCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the TappingCommand.
        /// </summary>
        public readonly static BindableProperty TappingCommandProperty;

        /// <summary>
        /// The BindableProperty for the TappingCommandParameter.
        /// </summary>
        public readonly static BindableProperty TappingCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the TappedCommand.
        /// </summary>
        public readonly static BindableProperty TappedCommandProperty;

        /// <summary>
        /// The BindableProperty for the TappedCommandParameter.
        /// </summary>
        public readonly static BindableProperty TappedCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the DoubleTappedCommand.
        /// </summary>
        public readonly static BindableProperty DoubleTappedCommandProperty;

        /// <summary>
        /// The BindableProperty for the DoubleTappedCommandParameter.
        /// </summary>
        public readonly static BindableProperty DoubleTappedCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the LongPressingCommand.
        /// </summary>
        public readonly static BindableProperty LongPressingCommandProperty;

        /// <summary>
        /// The BindableProperty for the LongPressingCommandParameter.
        /// </summary>
        public readonly static BindableProperty LongPressingCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the LongPressedCommand.
        /// </summary>
        public readonly static BindableProperty LongPressedCommandProperty;

        /// <summary>
        /// The BindableProperty for the LongPressedCommandParameter.
        /// </summary>
        public readonly static BindableProperty LongPressedCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the PinchingCommand.
        /// </summary>
        public readonly static BindableProperty PinchingCommandProperty;

        /// <summary>
        /// The BindableProperty for the PinchingCommandParameter.
        /// </summary>
        public readonly static BindableProperty PinchingCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the PinchedCommand.
        /// </summary>
        public readonly static BindableProperty PinchedCommandProperty;

        /// <summary>
        /// The BindableProperty for the PinchedCommandParameter.
        /// </summary>
        public readonly static BindableProperty PinchedCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the PanningCommand.
        /// </summary>
        public readonly static BindableProperty PanningCommandProperty;

        /// <summary>
        /// The BindableProperty for the PanningCommandParameter.
        /// </summary>
        public readonly static BindableProperty PanningCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the PannedCommand.
        /// </summary>
        public readonly static BindableProperty PannedCommandProperty;

        /// <summary>
        /// The BindableProperty for the PannedCommandParameter.
        /// </summary>
        public readonly static BindableProperty PannedCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the SwipedCommand.
        /// </summary>
        public readonly static BindableProperty SwipedCommandProperty;

        /// <summary>
        /// The BindableProperty for the SwipedCommandParameter.
        /// </summary>
        public readonly static BindableProperty SwipedCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the RotatingCommand.
        /// </summary>
        public readonly static BindableProperty RotatingCommandProperty;

        /// <summary>
        /// The BindableProperty for the RotatingCommandParameter.
        /// </summary>
        public readonly static BindableProperty RotatingCommandParameterProperty;

        /// <summary>
        /// The BindableProperty for the RotatedCommand.
        /// </summary>
        public readonly static BindableProperty RotatedCommandProperty;

        /// <summary>
        /// The BindableProperty for the RotatedCommandParameter.
        /// </summary>
        public readonly static BindableProperty RotatedCommandParameterProperty;

        /// <summary>
        /// Gets or sets the command which is executed when the element is tapped twice. This is a bindable property.
        /// </summary>
        public ICommand DoubleTappedCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.DoubleTappedCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.DoubleTappedCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the DoubleTappedCommand. This is a bindable property.
        /// </summary>
        public object DoubleTappedCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.DoubleTappedCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.DoubleTappedCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when a finger comes down. This is a bindable property.
        /// </summary>
        public ICommand DownCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.DownCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.DownCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the DownCommand. This is a bindable property.
        /// </summary>
        public object DownCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.DownCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.DownCommandParameterProperty, value);
            }
        }

        public GestureHandler GestureHandler
        {
            get
            {
                if (this.gestureHandler == null)
                {
                    this.gestureHandler = new GestureHandler(this);
                }
                return this.gestureHandler;
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the LongPressed event is raised. This is a bindable property.
        /// </summary>
        public ICommand LongPressedCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.LongPressedCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.LongPressedCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the LongPressedCommand. This is a bindable property.
        /// </summary>
        public object LongPressedCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.LongPressedCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.LongPressedCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is pressed long. This is a bindable property.
        /// </summary>
        public ICommand LongPressingCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.LongPressingCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.LongPressingCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the LongPressingCommand. This is a bindable property.
        /// </summary>
        public object LongPressingCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.LongPressingCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.LongPressingCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element was panned. This is a bindable property.
        /// </summary>
        public ICommand PannedCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.PannedCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.PannedCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the PannedCommand. This is a bindable property.
        /// </summary>
        public object PannedCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.PannedCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.PannedCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is panned. This is a bindable property.
        /// </summary>
        public ICommand PanningCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.PanningCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.PanningCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the PanningCommand. This is a bindable property.
        /// </summary>
        public object PanningCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.PanningCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.PanningCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the Pinched event is raised. This is a bindable property.
        /// </summary>
        public ICommand PinchedCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.PinchedCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.PinchedCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the PinchedCommand. This is a bindable property.
        /// </summary>
        public object PinchedCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.PinchedCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.PinchedCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is pinched. This is a bindable property.
        /// </summary>
        public ICommand PinchingCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.PinchingCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.PinchingCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the PinchingCommand. This is a bindable property.
        /// </summary>
        public object PinchingCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.PinchingCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.PinchingCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element was rotated. This is a bindable property.
        /// </summary>
        public ICommand RotatedCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.RotatedCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.RotatedCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the RotatedCommand. This is a bindable property.
        /// </summary>
        public object RotatedCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.RotatedCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.RotatedCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is rotated. This is a bindable property.
        /// </summary>
        public ICommand RotatingCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.RotatingCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.RotatingCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the RotatingCommand. This is a bindable property.
        /// </summary>
        public object RotatingCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.RotatingCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.RotatingCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is swiped. This is a bindable property.
        /// </summary>
        public ICommand SwipedCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.SwipedCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.SwipedCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the SwipedCommand. This is a bindable property.
        /// </summary>
        public object SwipedCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.SwipedCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.SwipedCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is tapped. This is a bindable property.
        /// </summary>
        public ICommand TappedCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.TappedCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.TappedCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the TappedCommand. This is a bindable property.
        /// </summary>
        public object TappedCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.TappedCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.TappedCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when the element is tapped and another tap may follow. This is a bindable property.
        /// </summary>
        public ICommand TappingCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.TappingCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.TappingCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the TappingCommand. This is a bindable property.
        /// </summary>
        public object TappingCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.TappingCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.TappingCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command which is executed when a finger is lifted from the touch screen. This is a bindable property.
        /// </summary>
        public ICommand UpCommand
        {
            get
            {
                return (ICommand)base.GetValue(GestureHandler.UpCommandProperty);
            }
            set
            {
                base.SetValue(GestureHandler.UpCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the UpCommand. This is a bindable property.
        /// </summary>
        public object UpCommandParameter
        {
            get
            {
                return base.GetValue(GestureHandler.UpCommandParameterProperty);
            }
            set
            {
                base.SetValue(GestureHandler.UpCommandParameterProperty, value);
            }
        }

        static LegacyGesturesStackLayout()
        {
            LegacyGesturesStackLayout.DownCommandProperty = GestureHandler.DownCommandProperty;
            LegacyGesturesStackLayout.DownCommandParameterProperty = GestureHandler.DownCommandParameterProperty;
            LegacyGesturesStackLayout.UpCommandProperty = GestureHandler.UpCommandProperty;
            LegacyGesturesStackLayout.UpCommandParameterProperty = GestureHandler.UpCommandParameterProperty;
            LegacyGesturesStackLayout.TappingCommandProperty = GestureHandler.TappingCommandProperty;
            LegacyGesturesStackLayout.TappingCommandParameterProperty = GestureHandler.TappingCommandParameterProperty;
            LegacyGesturesStackLayout.TappedCommandProperty = GestureHandler.TappedCommandProperty;
            LegacyGesturesStackLayout.TappedCommandParameterProperty = GestureHandler.TappedCommandParameterProperty;
            LegacyGesturesStackLayout.DoubleTappedCommandProperty = GestureHandler.DoubleTappedCommandProperty;
            LegacyGesturesStackLayout.DoubleTappedCommandParameterProperty = GestureHandler.DoubleTappedCommandParameterProperty;
            LegacyGesturesStackLayout.LongPressingCommandProperty = GestureHandler.LongPressingCommandProperty;
            LegacyGesturesStackLayout.LongPressingCommandParameterProperty = GestureHandler.LongPressingCommandParameterProperty;
            LegacyGesturesStackLayout.LongPressedCommandProperty = GestureHandler.LongPressedCommandProperty;
            LegacyGesturesStackLayout.LongPressedCommandParameterProperty = GestureHandler.LongPressedCommandParameterProperty;
            LegacyGesturesStackLayout.PinchingCommandProperty = GestureHandler.PinchingCommandProperty;
            LegacyGesturesStackLayout.PinchingCommandParameterProperty = GestureHandler.PinchingCommandParameterProperty;
            LegacyGesturesStackLayout.PinchedCommandProperty = GestureHandler.PinchedCommandProperty;
            LegacyGesturesStackLayout.PinchedCommandParameterProperty = GestureHandler.PinchedCommandParameterProperty;
            LegacyGesturesStackLayout.PanningCommandProperty = GestureHandler.PanningCommandProperty;
            LegacyGesturesStackLayout.PanningCommandParameterProperty = GestureHandler.PanningCommandParameterProperty;
            LegacyGesturesStackLayout.PannedCommandProperty = GestureHandler.PannedCommandProperty;
            LegacyGesturesStackLayout.PannedCommandParameterProperty = GestureHandler.PannedCommandParameterProperty;
            LegacyGesturesStackLayout.SwipedCommandProperty = GestureHandler.SwipedCommandProperty;
            LegacyGesturesStackLayout.SwipedCommandParameterProperty = GestureHandler.SwipedCommandParameterProperty;
            LegacyGesturesStackLayout.RotatingCommandProperty = GestureHandler.RotatingCommandProperty;
            LegacyGesturesStackLayout.RotatingCommandParameterProperty = GestureHandler.RotatingCommandParameterProperty;
            LegacyGesturesStackLayout.RotatedCommandProperty = GestureHandler.RotatedCommandProperty;
            LegacyGesturesStackLayout.RotatedCommandParameterProperty = GestureHandler.RotatedCommandParameterProperty;
        }

     

        /// <summary>
        /// The event which is raised when two Tapping events came within 250ms.
        /// </summary>
        public event EventHandler<TapEventArgs> DoubleTapped
        {
            add
            {
                this.GestureHandler.DoubleTapped += value;
                this.OnPropertyChanged("DoubleTapped");
            }
            remove
            {
                this.GestureHandler.DoubleTapped -= value;
                this.OnPropertyChanged("DoubleTapped");
            }
        }

        /// <summary>
        /// The event which is raised when a finger comes down. The finger(s) is/are still on the touch screen.
        /// </summary>
        public event EventHandler<DownUpEventArgs> Down
        {
            add
            {
                this.GestureHandler.Down += value;
                this.OnPropertyChanged("Down");
            }
            remove
            {
                this.GestureHandler.Down -= value;
                this.OnPropertyChanged("Down");
            }
        }

        /// <summary>
        /// The event which is raised when a finger finally comes up after a LongPressing event.
        /// </summary>
        public event EventHandler<LongPressEventArgs> LongPressed
        {
            add
            {
                this.GestureHandler.LongPressed += value;
                this.OnPropertyChanged("LongPressed");
            }
            remove
            {
                this.GestureHandler.LongPressed -= value;
                this.OnPropertyChanged("LongPressed");
            }
        }

        /// <summary>
        /// The event which is raised when a finger comes down and stays there.
        /// </summary>
        public event EventHandler<LongPressEventArgs> LongPressing
        {
            add
            {
                this.GestureHandler.LongPressing += value;
                this.OnPropertyChanged("LongPressing");
            }
            remove
            {
                this.GestureHandler.LongPressing -= value;
                this.OnPropertyChanged("LongPressing");
            }
        }

        /// <summary>
        /// The event which is raised when a finger comes up after a Panning event.
        /// </summary>
        public event EventHandler<PanEventArgs> Panned
        {
            add
            {
                this.GestureHandler.Panned += value;
                this.OnPropertyChanged("Panned");
            }
            remove
            {
                this.GestureHandler.Panned -= value;
                this.OnPropertyChanged("Panned");
            }
        }

        /// <summary>
        /// The event which is raised when a finger comes down and then moves in any direction.
        /// </summary>
        public event EventHandler<PanEventArgs> Panning
        {
            add
            {
                this.GestureHandler.Panning += value;
                this.OnPropertyChanged("Panning");
            }
            remove
            {
                this.GestureHandler.Panning -= value;
                this.OnPropertyChanged("Panning");
            }
        }

        /// <summary>
        /// The event which is raised when at least one finger is released after a Pinching event.
        /// </summary>
        public event EventHandler<PinchEventArgs> Pinched
        {
            add
            {
                this.GestureHandler.Pinched += value;
                this.OnPropertyChanged("Pinched");
            }
            remove
            {
                this.GestureHandler.Pinched -= value;
                this.OnPropertyChanged("Pinched");
            }
        }

        /// <summary>
        /// The event which is raised when two fingers are moved together or away from each other.
        /// </summary>
        public event EventHandler<PinchEventArgs> Pinching
        {
            add
            {
                this.GestureHandler.Pinching += value;
                this.OnPropertyChanged("Pinching");
            }
            remove
            {
                this.GestureHandler.Pinching -= value;
                this.OnPropertyChanged("Pinching");
            }
        }

        /// <summary>
        /// The event which is raised when at least one finger is lifted after a Rotating event.
        /// </summary>
        public event EventHandler<RotateEventArgs> Rotated
        {
            add
            {
                this.GestureHandler.Rotated += value;
                this.OnPropertyChanged("Rotated");
            }
            remove
            {
                this.GestureHandler.Rotated -= value;
                this.OnPropertyChanged("Rotated");
            }
        }

        /// <summary>
        /// The event which is raised when two fingers come down and their angle is changed.
        /// </summary>
        public event EventHandler<RotateEventArgs> Rotating
        {
            add
            {
                this.GestureHandler.Rotating += value;
                this.OnPropertyChanged("Rotating");
            }
            remove
            {
                this.GestureHandler.Rotating -= value;
                this.OnPropertyChanged("Rotating");
            }
        }

        /// <summary>
        /// The event which is raised when a finger comes down, is moved and still moves when it is raised again.
        /// </summary>
        public event EventHandler<SwipeEventArgs> Swiped
        {
            add
            {
                this.GestureHandler.Swiped += value;
                this.OnPropertyChanged("Swiped");
            }
            remove
            {
                this.GestureHandler.Swiped -= value;
                this.OnPropertyChanged("Swiped");
            }
        }

        /// <summary>
        /// The event which is raised when no other touch gesture followed Tapping within 250ms.
        /// </summary>
        public event EventHandler<TapEventArgs> Tapped
        {
            add
            {
                this.GestureHandler.Tapped += value;
                this.OnPropertyChanged("Tapped");
            }
            remove
            {
                this.GestureHandler.Tapped -= value;
                this.OnPropertyChanged("Tapped");
            }
        }

        /// <summary>
        /// The event which is raised when a finger comes down and up again.
        /// </summary>
        public event EventHandler<TapEventArgs> Tapping
        {
            add
            {
                this.GestureHandler.Tapping += value;
                this.OnPropertyChanged("Tapping");
            }
            remove
            {
                this.GestureHandler.Tapping -= value;
                this.OnPropertyChanged("Tapping");
            }
        }

        /// <summary>
        /// The event which is raised when a finger is lifted from the touch screen. There may be other fingers still on it.
        /// </summary>
        public event EventHandler<DownUpEventArgs> Up
        {
            add
            {
                this.GestureHandler.Up += value;
                this.OnPropertyChanged("Up");
            }
            remove
            {
                this.GestureHandler.Up -= value;
                this.OnPropertyChanged("Up");
            }
        }
    }
}