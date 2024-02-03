using DrawnUi.Maui;
using AppoMobi.Maui.Gestures;

namespace DrawnUi.Maui.Demo.Views.Controls;



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