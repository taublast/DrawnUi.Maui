using AppoMobi.Specials;
using DrawnUi.Maui.Draw;

namespace ShadersCarouselDemo
{


    public class Cell : SkiaLayout
    {
        //protected override void OnCacheCreated()
        //{
        //    base.OnCacheCreated();

        //    Debug.WriteLine("Cache created");
        //}
    }

    public partial class MainPage : ContentPage
    {


        private readonly MockDataProvider _mock;

        public ObservableRangeCollection<SimpleItemViewModel> Items { get; } = new();

        private int _SelectedIndex;
        public int SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                if (_SelectedIndex != value)
                {
                    _SelectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _SpeedMs = 1.0;
        public double SpeedMs
        {
            get
            {
                return _SpeedMs;
            }
            set
            {
                if (_SpeedMs != value)
                {
                    _SpeedMs = value;
                    OnPropertyChanged();
                    MainCarousel.SpeedRatio = SpeedMs;
                }
            }
        }


        private bool _AutoPlay;
        public bool AutoPlay
        {
            get
            {
                return _AutoPlay;
            }
            set
            {
                if (_AutoPlay != value)
                {
                    _AutoPlay = value;
                    OnPropertyChanged();
                    if (value && !MainCarousel.InTransition)
                    {
                        MainCarousel.PlayOne();
                    }
                }
            }
        }

        public MainPage()
        {
            try
            {
                InitializeComponent();

                BindingContext = this;

                _mock = new MockDataProvider();

                var data = _mock.GetRandomItems(10);

                Items.AddRange(data);

            }
            catch (Exception e)
            {
                Super.DisplayException(this, e);
            }
        }

        private async void OnTransitionChanged(object sender, bool b)
        {
            if (AutoPlay && !b && !MainCarousel.IsUserFocused)
            {
                //kick next
                await Task.Delay(1000);
                MainCarousel.PlayOne();
            }
        }
    }

}

