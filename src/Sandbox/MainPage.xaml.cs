using AppoMobi.Maui.Gestures;
using AppoMobi.Specials;
using DrawnUi.Maui.Infrastructure;
using DrawnUi.Maui.Views;
using Sandbox;

namespace MauiNet8;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        try
        {
            InitializeComponent();

            BindingContext = new MainPageViewModel();

            Buttons.ItemsSource = App.MainPages;

        }
        catch (Exception e)
        {
            Super.DisplayException(this, e);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
#if ANDROID

        App.TestLink();
        
#endif
    }

    private void TappedSelectPage(object sender, TouchActionEventArgs e)
    {
        if (sender is SkiaButton btn)
        {
            var page = App.MainPages.FirstOrDefault(x => x.Name == btn.Text);
            if (page != null)
            {
                Super.EnableRendering = false; //globally disable the rendering
                //to avoid crash accessing memory when the old mainpage is destroyed
                //but double buffered hw-accelerated view might try to render to it

                var instance = (ContentPage)Activator.CreateInstance(page.Type);
                Tasks.StartDelayed(TimeSpan.FromSeconds(1), () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        App.Current.MainPage = instance;
                        Super.EnableRendering = true;//enable back again and kick to update
                    });
                });

            }
        }
    }
    
    

    
}