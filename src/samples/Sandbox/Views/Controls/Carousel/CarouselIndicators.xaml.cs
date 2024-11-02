namespace Sandbox.Views.Controls.Carousel;

public partial class CarouselIndicators : SkiaLayout
{

    public CarouselIndicators()
    {
        InitializeComponent();

        //UpdateSource();
    }

    public void UpdateValues()
    {
        if (ItemsSource != null)
        {
            var index = 0;
            foreach (var item in ItemsSource)
            {
                if (item is SelectedLabel label)
                {
                    label.IsSelected = index == SelectedIndex;
                }
                index++;
            }
        }
    }

    public virtual void UpdateSource()
    {
        var values = new List<SelectedLabel>();
        for (int i = 0; i < TotalEnabled; i++)
        {
            values.Add(new()
            {
                Text = $"{i + 1}",
                IsSelected = i == SelectedIndex
            });
        }

        Values = values;

        ItemsSource = Values;
    }

    private static void OnDotsChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is CarouselIndicators control)
        {
            control.UpdateSource();
        }
    }

    private static void OnIndexChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is CarouselIndicators control)
        {
            control.UpdateValues();
        }
    }

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex),
        typeof(int),
        typeof(CarouselIndicators),
        -1,
        BindingMode.OneWay,
        propertyChanged: OnIndexChanged);

    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }


    public static readonly BindableProperty TotalEnabledProperty = BindableProperty.Create(
        nameof(TotalEnabled),
        typeof(int),
        typeof(CarouselIndicators),
        0, propertyChanged: OnDotsChanged);



    public int TotalEnabled
    {
        get { return (int)GetValue(TotalEnabledProperty); }
        set { SetValue(TotalEnabledProperty, value); }
    }

    private List<SelectedLabel> _values;
    public List<SelectedLabel> Values
    {
        get
        {
            return _values;
        }
        set
        {
            if (_values != value)
            {
                _values = value;
                OnPropertyChanged();
            }
        }
    }


}