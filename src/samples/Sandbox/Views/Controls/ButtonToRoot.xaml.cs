using AppoMobi.Maui.Gestures;
using AppoMobi.Specials;
using MauiNet8;

namespace Sandbox.Views.Controls;

public partial class ButtonToRoot
{
    public ButtonToRoot()
    {
        InitializeComponent();
    }

    private void GoToRoot(object sender, TouchActionEventArgs e)
    {
        if (TouchEffect.CheckLockAndSet())
            return;

        App.Instance.SetMainPage(new MainPage());
    }
}