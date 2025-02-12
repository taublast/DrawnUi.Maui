using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using AppoMobi.Touch;
using AppoMobi.Models;
using AppoMobi.Xam;
using System.Threading.Tasks;
using TestCalendar.Views;

namespace AppoMobi
{
    public partial class CellDay
    {

        public CellDay()
        {
            InitializeComponent();
            InitGesturesForCell(SelectionBox);

            _front = txtText.TextColor;

            var item = BindingContext as AppoDay;
            if (item == null) return;
            SetupCell();
        }
 
        protected override void OnBindingContextChanged()
 
        {
            SetupCell();
            base.OnBindingContextChanged();
        }

        Color _back { get; set; } = MonthView.ColorBackground;
        Color _front { get; set; }


        public override void Dispose()
        {
            if (!(BindingContext is AppoDay item)) return;
            try { item.PropertyChanged -= OnItemPropertyChanged; }
            catch (Exception e) { }

            base.Dispose();
        }


        public void SetupCell()
        {
            if (!(BindingContext is AppoDay item)) return;

            try { item.PropertyChanged -= OnItemPropertyChanged; }
            catch (Exception e) { }
            item.PropertyChanged += OnItemPropertyChanged;
            var ee = new PropertyChangedEventArgs("Selected");
            OnItemPropertyChanged(item, ee);

            if (item.Disabled)
            {
                _back = Colors.WhiteSmoke;
                cControl.BackgroundColor = _back;
                _front = Colors.DarkGray;
                txtText.TextColor = _front;
            }

            txtText.Text = item.DayDesc;

            var dateAndTime = DateTime.Now;
            var today = dateAndTime.Date;

            if (item.Date == today)
            {
                _frame = new Frame();
                _frame.BorderColor = Colors.Red;
                _frame.Padding = new Thickness(0);
               // _frame.CornerRadius = 0;
                _frame.HasShadow = false;
                _frame.BackgroundColor=Colors.Transparent;
                _frame.HorizontalOptions= LayoutOptions.FillAndExpand;
                _frame.VerticalOptions=LayoutOptions.FillAndExpand;

                cControl.Children.Add(_frame);

                //txtText.BackgroundColor = Color.DarkSeaGreen;
            }
        }

        Frame _frame { get; set; }

        public event EventHandler<TapEventArgs> ItemTapped;
        public async void CallItemTapped(object item, TapEventArgs e)
        {
            //cControl.Animate(AnimationType.Scale);
            //await Task.Delay(250);
            ItemTapped?.Invoke(item, e);
        }

        void OnTapped_Cell(object sender, TapEventArgs e)
        {
            CallItemTapped(this, e); //сагат
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                var item = (AppoDay)sender;
                if (item.Selected)
                {
                    cControl.BackgroundColor= Colors.Red;
                    txtText.TextColor = MonthView.ColorBackground;
                }
                else
                {
                    cControl.BackgroundColor = _back;
                    txtText.TextColor = _front;
                }
            }
        }

 
    }
}