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
    /// Under consctruction
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
        var color = GetColorForValue(e);

        ClearColor = color;
    }

    private void ColorsPanelSelectionChanged(object sender, Color value)
    {
        SelectedColor = value;
    }

    public static Color GetColorForValue(double sliderValue)
    {
        int numberOfColors = SkiaColorsPanel.DefaultPrimaryColors.Count;
        sliderValue = Math.Clamp(sliderValue, 0.0, 1.0);

        double segmentSize = 1.0 / (numberOfColors - 1);
        int segmentIndex = (int)(sliderValue / segmentSize);

        // Handle edge case where sliderValue is 1.0
        if (segmentIndex >= numberOfColors - 1)
        {
            segmentIndex = numberOfColors - 2;
        }

        double segmentStartValue = segmentSize * segmentIndex;
        double fractionIntoSegment = (sliderValue - segmentStartValue) / segmentSize;

        Color startColor = SkiaColorsPanel.DefaultPrimaryColors[segmentIndex];
        Color endColor = SkiaColorsPanel.DefaultPrimaryColors[segmentIndex + 1];

        return InterpolateColor(startColor, endColor, fractionIntoSegment);
    }

    private static Color InterpolateColor(Color start, Color end, double fraction)
    {
        float r = (float)(start.Red + (end.Red - start.Red) * fraction);
        float g = (float)(start.Green + (end.Green - start.Green) * fraction);
        float b = (float)(start.Blue + (end.Blue - start.Blue) * fraction);
        return new Color(r, g, b);
    }
}