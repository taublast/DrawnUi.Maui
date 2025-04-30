
using AppoMobi.Maui.Gestures;
using System.Windows.Input;

namespace AppoMobi.Framework.Forms.UI.Touch
{


    public class PanAwareHotspot : Hotspot, IDisposable
    {
        public double TotalDistance;
        public double PanningStartX;
        public bool IsPanning;

        public PanAwareHotspot()
        {
            TouchMode = TouchHandlingStyle.Default;
        }

        public override void OnUp(TouchActionEventArgs e)
        {
            base.OnUp(e);

            IsPanning = false;

            Debug.WriteLine($"Up");

            PanningFinished?.Execute(TotalDistance);

            TotalDistance = 0;
            DistanceX = 0;
            DistanceY = 0;
        }



        public override void OnPanning(object sender, TouchActionEventArgs e)
        {
            base.OnPanning(sender, e);

            if (!IsPanning)
            {
                TotalDistance = 0;
                PanningStartX = e.Location.X;
                IsPanning = true;
            }


            TotalDistance += e.Location.X - PanningStartX;

            Debug.WriteLine($"Panning {e.Location.X} => {TotalDistance}");

            DistanceX = TotalDistance;// / Display.DisplayDensity;
            DistanceY = e.Distance.Total.Y;// / Display.DisplayDensity;
        }



        //-------------------------------------------------------------
        // PanningFinished
        //-------------------------------------------------------------
        private const string namePanningFinished = "PanningFinished";
        public static readonly BindableProperty PanningFinishedProperty = BindableProperty.Create(namePanningFinished,
            typeof(ICommand),
            typeof(PanAwareHotspot), null); //, BindingMode.TwoWay
        public ICommand PanningFinished
        {
            get { return (ICommand)GetValue(PanningFinishedProperty); }
            set { SetValue(PanningFinishedProperty, value); }
        }

        //-------------------------------------------------------------
        // DistanceX
        //-------------------------------------------------------------
        private const string nameDistanceX = "DistanceX";
        public static readonly BindableProperty DistanceXProperty = BindableProperty.Create(nameDistanceX, typeof(double), typeof(PanAwareHotspot), 0.0); //, BindingMode.TwoWay
        public double DistanceX
        {
            get { return (double)GetValue(DistanceXProperty); }
            set { SetValue(DistanceXProperty, value); }
        }

        //-------------------------------------------------------------
        // DistanceY
        //-------------------------------------------------------------
        private const string nameDistanceY = "DistanceY";
        public static readonly BindableProperty DistanceYProperty = BindableProperty.Create(nameDistanceY, typeof(double), typeof(PanAwareHotspot), 0.0); //, BindingMode.TwoWay
        public double DistanceY
        {
            get { return (double)GetValue(DistanceYProperty); }
            set { SetValue(DistanceYProperty, value); }
        }

    }
}
