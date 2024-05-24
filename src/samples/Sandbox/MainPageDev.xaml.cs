
using AppoMobi.Specials;
using DrawnUi.Maui.Infrastructure;
using Sandbox;
using Sandbox.Resources.Strings;
using Sandbox.Views;
using Sandbox.Views.Xaml2Pdf;
using System.Text;


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


}