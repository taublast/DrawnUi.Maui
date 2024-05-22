
using Sandbox.Views;

namespace MauiNet8;

public partial class MainPageDev : BasePage
{


    public MainPageDev()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception e)
        {
            Super.DisplayException(this, e);
        }
    }


    private void OnLinkTapped(object sender, string link)
    {
        Super.Log(link);
    }
}