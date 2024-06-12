using AppoMobi.Maui.Gestures;
using DrawnUi.Maui.Infrastructure;

namespace Sandbox.Views
{
    public partial class MainPageShader_Transition
    {

        public MainPageShader_Transition()
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