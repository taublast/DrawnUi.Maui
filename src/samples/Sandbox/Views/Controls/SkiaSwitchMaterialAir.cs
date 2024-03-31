using Sandbox.Views.Controls;

namespace AppoMobi.Maui.DrawnUi.Demo.Views.Controls;

public class SkiaSwitchMaterialAir : SkiaSwitchFun
{
    public override void ApplyOff()
    {
        base.ApplyOff();

        Track.Opacity = 0.75;
        Track.Margin = new(8, 6);
        Track.BackgroundColor = Colors.Transparent;
        Track.StrokeWidth = 2;
        Track.StrokeColor = this.ColorFrameOff;
    }

    public override void ApplyOn()
    {
        base.ApplyOn();

        Track.Opacity = 1;
        Track.Margin = new(8, 6);
        Track.BackgroundColor = Colors.Transparent;
        Track.StrokeWidth = 2;
        Track.StrokeColor = this.ColorFrameOn;
    }
}