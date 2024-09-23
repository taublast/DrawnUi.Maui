namespace Sandbox.Views
{
    public partial class MainPageCarousels
    {

        public MainPageCarousels()
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

        private void SkiaButton_Tapped(object sender, SkiaGesturesParameters skiaGesturesParameters)
        {
            //MainCarousel.ChildrenFactory.PrintDebugVisible();
        }
    }
}