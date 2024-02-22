using AppoMobi.Maui.Gestures;
using AppoMobi.Specials;
using DrawnUi.Maui.Infrastructure;
using DrawnUi.Maui.Views;
using Sandbox;

namespace MauiNet8;

public partial class MainPage : ContentPage
{
    int count = 0;

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

    protected override void OnAppearing()
    {
        base.OnAppearing();
    
        DoTest();
    }
    
    void DoTest()
    {

        // using Skia's SkSL
        string shaderCode = @"

            uniform float iTime;  // Manually updated time variable
            uniform float2 iResolution; // Screen iResolution

            float f(vec3 p) {
            p.z -= iTime * 10.;
            float a = p.z * .1;
            p.xy *= mat2(cos(a), sin(a), -sin(a), cos(a));
            return .1 - length(cos(p.xy) + sin(p.yz));
            }

            half4 main(vec2 fragcoord) { 
            vec3 d = .5 - fragcoord.xy1 / iResolution.y;
            vec3 p=vec3(0);
            for (int i = 0; i < 32; i++) {
              p += f(p) * d;
            }
            return ((sin(p) + vec3(2, 5, 9)) / length(p)).xyz1;
            }

                ";


        var effect = SkSl.Compile(shaderCode);

    }

    
    private void TappedSelectPage(object sender, TouchActionEventArgs e)
    {
        if (sender is SkiaButton btn)
        {
            var page = App.MainPages.FirstOrDefault(x => x.Name == btn.Text);
            if (page != null)
            {
                Super.EnableRendering = false; //globally disable the rendering
                //to avoid crash accessing memory when the old mainpage is destroyed
                //but double buffered hw-accelerated view might try to render to it

                var instance = (ContentPage)Activator.CreateInstance(page.Type);
                Tasks.StartDelayed(TimeSpan.FromSeconds(1), () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        App.Current.MainPage = instance;
                        Super.EnableRendering = true;//enable back again and kick to update
                    });
                });

            }
        }
    }
    
    

    
}