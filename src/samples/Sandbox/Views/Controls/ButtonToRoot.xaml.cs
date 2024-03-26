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

        Super.EnableRendering = false; //globally disable the rendering
        Tasks.StartDelayed(TimeSpan.FromSeconds(1), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                App.Current.MainPage = new MainPage();
                Super.EnableRendering = true;//enable back again and kick to update
            });
        });
    }
}