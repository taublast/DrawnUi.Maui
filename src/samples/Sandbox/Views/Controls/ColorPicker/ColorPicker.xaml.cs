namespace Sandbox.Views.Controls;

public partial class ColorPicker
{
    public ColorPicker()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
        nameof(SelectedColor),
        typeof(Color),
        typeof(SkiaShape),
        Colors.White,
        propertyChanged: SelectedColorChanged,
        defaultBindingMode: BindingMode.OneWayToSource);

    private static void SelectedColorChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is ColorPicker control)
        {

            control.Update();
        }
    }

    /// <summary>
    /// Resulting color, read-only
    /// </summary>
    public Color SelectedColor
    {
        get { return (Color)GetValue(SelectedColorProperty); }
        set { SetValue(SelectedColorProperty, value); }
    }

    private static void DefaultColorChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is ColorPicker control)
        {
            control.SetSliderValueForColor((Color)newvalue);
        }
    }

    /// <summary>
    /// TODO Under consctruction
    /// </summary>
    /// <param name="color"></param>
    public void SetSliderValueForColor(Color color)
    {
        if (Slider != null)
        {
            var hue = color.GetHue();

            //set panel indicator position
            //todo

            //set slider
            ClearColor = Color.FromHsv(hue, 1, 1);
        }
    }

    public static readonly BindableProperty DefaultColorProperty = BindableProperty.Create(
        nameof(DefaultColor),
        typeof(Color),
        typeof(SkiaShape),
        Colors.White,
        propertyChanged: DefaultColorChanged,
        defaultBindingMode: BindingMode.OneWayToSource);

    /// <summary>
    /// Default color, set this at startup at will
    /// </summary>
    public Color DefaultColor
    {
        get { return (Color)GetValue(DefaultColorProperty); }
        set { SetValue(DefaultColorProperty, value); }
    }

    private Color _clearColor;
    public Color ClearColor
    {
        get
        {
            return _clearColor;
        }
        set
        {
            if (_clearColor != value)
            {
                _clearColor = value;
                OnPropertyChanged();

                ColorsPanel.SelectionColors = new List<Color>()
                {
                    Colors.White,
                    value
                };
            }
        }
    }

    public void OnSliderValueChanged(object sender, double e)
    {
        ClearColor = Slider.SelectedColor;
    }

    private void ColorsPanelSelectionChanged(object sender, Color value)
    {
        SelectedColor = value;
    }

}