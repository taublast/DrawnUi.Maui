using System;
using System.Drawing;
using Point = Microsoft.Maui.Graphics.Point;


namespace AppoMobi.Xam
{

    public class GesturesContentView : ContentView
    {
        //-------------------------------------------------------------
        // GesturesEventParameter
        //-------------------------------------------------------------
        const string nameGesturesEventParameter = "GesturesEventParameter";
        public static readonly BindableProperty GesturesEventParameterProperty = BindableProperty.Create(nameGesturesEventParameter, typeof(object), typeof(GesturesContentView), null); //, BindingMode.TwoWay
        public object GesturesEventParameter
        {
            get { return (object)GetValue(GesturesEventParameterProperty); }
            set { SetValue(GesturesEventParameterProperty, value); }
        }


        public event EventHandler<XamTapEventArgs> Tapped;

        //todo
        //public event EventHandler<XamTapEventArgs> Down;
        //public event EventHandler<XamTapEventArgs> Up;

        public GesturesContentView()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 1;
            tapGestureRecognizer.Tapped += (s, e) =>
            {

                var test = e;
                // handle the tap
                var ee = new XamTapEventArgs();
                ee.NumberOfTaps = 1;
                ee.Sender = this;

                ee.ViewPosition = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);

                Tapped?.Invoke(this, ee);
            };
            GestureRecognizers.Add(tapGestureRecognizer);

        }


    }

    //*********************************************************
    public class GesturesStackLayout : StackLayout
    //*********************************************************
    {
        
        //-------------------------------------------------------------
        // GesturesEventParameter
        //-------------------------------------------------------------
        const string nameGesturesEventParameter = "GesturesEventParameter";
        public static readonly BindableProperty GesturesEventParameterProperty = BindableProperty.Create(nameGesturesEventParameter, typeof(object), typeof(GesturesStackLayout), null); //, BindingMode.TwoWay
        public object GesturesEventParameter
        {
            get { return (object)GetValue(GesturesEventParameterProperty); }
            set { SetValue(GesturesEventParameterProperty, value); }
        }	



        public event EventHandler<XamTapEventArgs> Tapped;

        //todo
        //public event EventHandler<XamTapEventArgs> Down;
        //public event EventHandler<XamTapEventArgs> Up;

        public GesturesStackLayout()
        {
            GestureRecognizers.Clear();
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 1;
            tapGestureRecognizer.Tapped += (s, e) =>
            {

                var test = e;
                // handle the tap
                var ee = new XamTapEventArgs();
                ee.NumberOfTaps = 1;
                ee.ViewPosition = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
                ee.Sender = this;

                Tapped?.Invoke(this, ee);
            };
            GestureRecognizers.Add(tapGestureRecognizer);
        }


    }


    public class GesturesFrame : Border
    {
        public event EventHandler<XamTapEventArgs> Tapped;

        //-------------------------------------------------------------
        // GesturesEventParameter
        //-------------------------------------------------------------
        const string nameGesturesEventParameter = "GesturesEventParameter";
        public static readonly BindableProperty GesturesEventParameterProperty = BindableProperty.Create(nameGesturesEventParameter, typeof(object), typeof(GesturesStackLayout), null); //, BindingMode.TwoWay
        public object GesturesEventParameter
        {
            get { return (object)GetValue(GesturesEventParameterProperty); }
            set { SetValue(GesturesEventParameterProperty, value); }
        }


        //todo
        //public event EventHandler<XamTapEventArgs> Down;
        //public event EventHandler<XamTapEventArgs> Up;

        public GesturesFrame()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 1;
            tapGestureRecognizer.Tapped += (s, e) =>
            {

                var test = e;
                // handle the tap
                var ee = new XamTapEventArgs();
                ee.NumberOfTaps = 1;
                ee.Sender = this;                

                ee.ViewPosition = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);

                Tapped?.Invoke(this, ee);
            };
            GestureRecognizers.Add(tapGestureRecognizer);

        }


    }

    /// <summary>
    /// The arguments for the <code>Down</code> and <code>Up</code> events.
    /// </summary>
    public class XamDownUpEventArgs : BaseXamGestureEventArgs
    {
       

    }

    /// <summary>
    /// The arguments for the <code>Tapping</code>, <code>Tapped</code> and <code>DoubleTapped</code> events.
    /// </summary>
    public class XamTapEventArgs : BaseXamGestureEventArgs
    {
        /// <summary>
        /// The number of taps in a short period of time (~250ms).
        /// </summary>
        public virtual int NumberOfTaps
        {
            get;
            set;
        }
    }

    /// <summary>
	/// The base class for all gesture event args.
	/// </summary>
	public class BaseXamGestureEventArgs : EventArgs
    {
	    Point center;

        /// <summary>
        /// The element which raised this event.
        /// </summary>
        public virtual object Sender
        {
            get;
            set;
        }

        /// <summary>
        /// Android and iOS sometimes cancel a touch gesture. In this case we raise a *ed event with Cancelled set to <value>true</value>.
        /// </summary>
        public virtual bool Cancelled
        {
            get;
            set;
        }

        /// <summary>
        /// The position of the <see cref="P:AppoMobi.Touch.BaseGestureEventArgs.Sender" /> on the screen.
        /// </summary>
        public virtual Rectangle ViewPosition
        {
            get;
            set;
        }

     
    }


    public class XamGestureHandler
    {
 

        public static readonly BindableProperty TappedCommandParameterProperty = BindableProperty.Create("TappedCommandParameter", typeof(object), typeof(XamGestureHandler), null, BindingMode.OneWay, null, null, null, null, null);

 
    }
}
