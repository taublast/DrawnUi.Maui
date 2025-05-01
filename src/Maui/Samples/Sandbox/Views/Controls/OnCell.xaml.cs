namespace Sandbox.Views.Controls
{
    public partial class OnCell
    {

        public OnCell()
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

        private void SkiaButton_Tapped(object sender, SkiaControl.ControlTappedEventArgs controlTappedEventArgs)
        {
            //MainCarousel.ChildrenFactory.PrintDebugVisible();
        }
    }
}
