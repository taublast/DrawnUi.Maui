namespace Sandbox.Views
{
    public partial class MainPageShaderRipples
    {

        public MainPageShaderRipples()
        {
            try
            {
                InitializeComponent();

                BindingContext = new MainPageViewModel();
            }
            catch (Exception e)
            {
                Super.DisplayException(this, e);
            }
        }


    }
}