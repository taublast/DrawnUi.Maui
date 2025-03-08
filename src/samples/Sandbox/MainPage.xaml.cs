using Sandbox;
using Sandbox.Views;

namespace MauiNet8;

public partial class MainPage : BasePageCodeBehind
{
    int count = 0;

    public List<MainPageVariant> ButtonsList { get; }

    public MainPage()
    {
        try
        {
            ButtonsList = App.MainPages;

            InitializeComponent();

            BindingContext = new MainPageViewModel();
        }
        catch (Exception e)
        {
            Super.DisplayException(this, e);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID

        //App.TestLink();

#endif
    }

    private void TappedSelectPage(object sender, SkiaGesturesParameters skiaGesturesParameters)
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

                App.Instance.SetMainPage(instance);

            }
        }
    }




}
