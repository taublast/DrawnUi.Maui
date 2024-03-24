using AppoMobi.Maui.Gestures;

namespace Sandbox.Views
{
    public partial class MainPageScroll : ContentPage
    {

        public MainPageScroll()
        {
            try
            {
                InitializeComponent();

                //avoid setting context BEFORE InitializeComponent, can bug 
                //having parent BindingContext still null when constructing from xaml
                BindingContext = new MainPageViewModel();
            }
            catch (Exception e)
            {
                Super.DisplayException(this, e);
                Console.WriteLine(e);
            }
        }


    }
}