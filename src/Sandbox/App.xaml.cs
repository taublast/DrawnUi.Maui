using DrawnUi.Maui;
using Microsoft.Maui.Platform;
using System.Globalization;

namespace Sandbox
{
    public partial class App : Application
    {
        public App()
        {
            Super.SetLocale("en");

            InitializeComponent();

            MainPage = new MainPage();
            //MainPage = new MainPageBackdrop();
            //MainPage = new MainPageDynamicHeightCells();
            //MainPage = new MainPageIOS17Tabs();
            //MainPage = new MainPageLabels();
            //MainPage = new MainGC();
            //MainPage = new MainPageShader(); //needs HW accel, NO WINDOWS!!!!!, wait for skia sharp 3.0
        }



    }
}
