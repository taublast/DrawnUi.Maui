using AppoMobi.Maui.Gestures;
using System.Collections;

namespace Sandbox.Views
{
    public partial class MainPage
    {

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

        private void TappedSelectPage(object sender, TouchActionEventArgs e)
        {
            if (sender is SkiaButton btn)
            {
                var page = App.MainPages.FirstOrDefault(x => x.Name == btn.Text);
                if (page != null)
                {
                    var instance = (ContentPage)Activator.CreateInstance(page.Type);
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        App.Current.MainPage = instance;
                    });
                }
            }
        }
    }
}