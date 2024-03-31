using AppoMobi.Maui.Gestures;

namespace Sandbox.Views.Controls;

public partial class SkiaSwitchFun : SkiaSwitch
{
    public SkiaSwitchFun()
    {
        InitializeComponent();
    }

    protected void OnTapped(object sender, TouchActionEventArgs e)
    {
        IsToggled = !IsToggled;
    }

}