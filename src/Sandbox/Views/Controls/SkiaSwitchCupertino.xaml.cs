using AppoMobi.Maui.Gestures;

namespace Sandbox.Views.Controls;



public partial class SkiaSwitchCupertino : SkiaSwitch
{
    public SkiaSwitchCupertino()
    {
        InitializeComponent();
    }

    protected void OnTapped(object sender, TouchActionEventArgs e)
    {
        IsToggled = !IsToggled;
    }

}