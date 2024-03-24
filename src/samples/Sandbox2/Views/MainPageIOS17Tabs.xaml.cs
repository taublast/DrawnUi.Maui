namespace Sandbox.Views
{


    public partial class MainPageIOS17Tabs : ContentPage
    {


        public MainPageIOS17Tabs()
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