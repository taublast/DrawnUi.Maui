namespace Sandbox.Views.Controls;

public partial class SliderColor : SkiaSlider
{
    public SliderColor()
    {
        InitializeComponent();
    }

    public override void OnEndChanged()
    {
        SelectedColor = GetColorForValue(End);

        base.OnEndChanged();
    }

    public Color GetColorForValue(double sliderValue)
    {
        var colors = this.ColorPanel.PrimaryColors;
        if (colors.Count == 0)
        {
            colors = SkiaColorsPanel.DefaultPrimaryColors;
        }
        int numberOfColors = colors.Count;

        if (numberOfColors > 1)
        {
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

            Color startColor = colors[segmentIndex];
            Color endColor = colors[segmentIndex + 1];

            return InterpolateColor(startColor, endColor, fractionIntoSegment);
        }

        return Colors.Transparent;

    }

    private static Color InterpolateColor(Color start, Color end, double fraction)
    {
        float r = (float)(start.Red + (end.Red - start.Red) * fraction);
        float g = (float)(start.Green + (end.Green - start.Green) * fraction);
        float b = (float)(start.Blue + (end.Blue - start.Blue) * fraction);
        return new Color(r, g, b);
    }

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
        nameof(SelectedColor),
        typeof(Color),
        typeof(SliderColor),
        Colors.White,
        propertyChanged: SelectedColorChanged,
        defaultBindingMode: BindingMode.OneWayToSource);

    private static void SelectedColorChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SliderColor control)
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

}