using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{

    public class MainPageCode : BasePage, IDisposable
    {
        Canvas Canvas;

        public override void Dispose()
        {
            base.Dispose();

            this.Content = null;
            Canvas?.Dispose();
        }

        public MainPageCode()
        {
#if DEBUG
            Super.HotReload += ReloadUI;
#endif
            Build();
        }

        private int _reloads;

        void Build()
        {
            Canvas?.Dispose();

            Canvas = new Canvas()
            {
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 100,
                HeightRequest = 100,
                BackgroundColor = Colors.Red,
                Content = new SkiaLottie()
                {
                    DefaultFrame = -1,
                    Source = @"Lottie\ok.json",
                    WidthRequest = 100,
                    HeightRequest = 100,
                    HorizontalOptions = LayoutOptions.Fill,
                }
            };

            _reloads++;

            this.Content = Canvas;
        }

        private void ReloadUI(Type[] obj)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Build();
            });
        }

    }
}
