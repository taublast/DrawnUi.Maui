using DrawnUi.Maui.Draw;

namespace ShadersCarouselDemo.Controls;

public partial class DrawnSwitch : SkiaSwitch
{
    public DrawnSwitch()
    {
        InitializeComponent();
    }

    protected void OnTapped(object sender, SkiaGesturesParameters skiaGesturesParameters)
    {
        IsToggled = !IsToggled;
    }

}