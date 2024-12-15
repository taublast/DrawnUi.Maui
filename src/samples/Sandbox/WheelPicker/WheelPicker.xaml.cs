
using DrawnUi.Maui;
using System.Collections;

namespace AppoMobi.Maui.DrawnUi.Demo.Views.Controls
{
    public partial class WheelPicker
    {
        public WheelPicker()
        {
            InitializeComponent();

            //InitializeAnimator();

            SetItemSource();

            Scroller.IndexChanged += OnIndexChanged;
        }

        private void OnScrolled(object sender, double y)
        {
            OnPropertyChanged(nameof(DebugInfo));
        }

        private void OnWheelMeasured(object sender, ScaledSize e)
        {
            OnPropertyChanged(nameof(DebugInfo));
        }

        private void OnIndexChanged(object sender, int index)
        {
            UpdateIndex(index);
        }

        public void UpdateIndex(int index)
        {
            SelectedIndex = index;
        }

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex),
        typeof(int),
        typeof(WheelPicker),
        -1,
        BindingMode.TwoWay,
        propertyChanged: OnUpdateState);


        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        //public static readonly BindableProperty StartupIndexProperty = BindableProperty.Create(nameof(StartupIndex),
        //    typeof(int),
        //    typeof(WheelPicker),
        //    -1,
        //    BindingMode.Default,
        //    propertyChanged: OnUpdateStartupState);

        ///// <summary>
        ///// Read-only
        ///// </summary>
        //public int StartupIndex
        //{
        //    get { return (int)GetValue(StartupIndexProperty); }
        //    set { SetValue(StartupIndexProperty, value); }
        //}

        public static readonly BindableProperty DataSourceProperty = BindableProperty.Create(
            nameof(DataSource),
            typeof(IList),
            typeof(WheelPicker),
            null,
            propertyChanged: DataSourcePropertyChanged);

        public IList DataSource
        {
            get => (IList)GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }
        private static void DataSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is WheelPicker control)
            {
                control.SetItemSource();
            }
        }

        public void UpdateDebug()
        {
            OnPropertyChanged(nameof(DebugInfo));
        }

        public void SetItemSource()
        {
            Wheel.ItemsSource = DataSource;

            SetStartup();

            OnPropertyChanged(nameof(DebugInfo));
        }

        public string DebugInfo
        {
            get
            {
                if (this.DataSource == null || this.DataSource.Count < 1)
                    return "empty";

                //var sub = Wheel.Views[0];
                //var label = sub.Views[0] as SkiaLabel;
                //         return $"y {Scroller.ViewportOffsetY:0.0} items {Wheel.Views.Count}, [{sub.BindingContext}] [{label.BindingContext}] => '{label.Text}' as {label.TextColor.ToArgbHex()}";

                return $"VelocityY: {Scroller.VelocityY:0}";
            }
        }

        public void ApplyStartupIndex()
        {
            if (LayoutReady && SelectedIndex >= 0 && Scroller.CurrentIndex != SelectedIndex)
            {
                Scroller.ScrollToIndex(SelectedIndex, false, RelativePositionType.Center);
            }
        }

        private static void OnUpdateState(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is WheelPicker control)
            {
                //todo 
            }
        }

        private static void OnUpdateStartupState(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is WheelPicker control)
            {
                control.SetStartup();
            }
        }

        protected void SetStartup()
        {
            ApplyStartupIndex();
        }

        protected override void OnLayoutReady()
        {
            base.OnLayoutReady();

            SetStartup();
        }

        protected RangeAnimator _animatorValue;

        protected void InitializeAnimator()
        {
            if (_animatorValue == null)
            {

                Container.ScaleX = 0.0f;
                Container.ScaleY = 0.0f;

                _animatorValue = new(Container)
                {
                    OnStop = () =>
                    {
                        //SetValue(Value);
                    }
                };
            }
        }
    }
}