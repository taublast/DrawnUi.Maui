namespace Sandbox.Views.Controls;

public partial class ColorPicker
{
    public ColorPicker()
    {
        InitializeComponent();
    }

    public virtual void UpdateColor()
    {
        ClearColor = SelectedColor;
    }

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
        nameof(SelectedColor),
        typeof(Color),
        typeof(SkiaShape),
        Colors.White,
        propertyChanged: SelectedColorChanged);

    private static void SelectedColorChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is ColorPicker control)
        {

            control.Update();
        }
    }

    public Color SelectedColor
    {
        get { return (Color)GetValue(SelectedColorProperty); }
        set { SetValue(SelectedColorProperty, value); }
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