using System.Reflection;

namespace Sandbox
{
    public partial class App : Application
    {
        public App()
        {
            Super.SetLocale("en");

            InitializeComponent();

            MainPage = new AppShell();

            var mask = "MainPage";

            var xamlResources = this.GetType().Assembly
                .GetCustomAttributes<XamlResourceIdAttribute>();

            MainPages = xamlResources
                .Where(x => x.Type.Name.Contains(mask)
                && !x.Type.Name.ToLower().Contains("dev")
#if !SKIA3 && (WINDOWS || MACCATALYST)
                && !x.Type.Name.ToLower().Contains("shader") //skia2 incompatible
#endif
                && x.Type.Name != mask)
                .Select(s => new MainPageVariant()
                {
                    Name = s.Type.Name.Replace(mask, string.Empty),
                    Type = s.Type
                }).ToList();
        }

        public static List<MainPageVariant> MainPages { get; protected set; }

        public void SetMainPage(Page page)
        {
            Super.EnableRendering = false; //globally disable the rendering
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(500);
                var kill = this.MainPage;
                MainPage = page;
                await Task.Delay(250);
                Super.EnableRendering = true;//enable back again and kick to update
                if (kill is IDisposable disposable)
                {
                    disposable?.Dispose();
                }
            });
        }

        public static App Instance => App.Current as App;

    }



    public record MainPageVariant()
    {
        public Type Type { get; set; }
        public string Name { get; set; }
    }



}
