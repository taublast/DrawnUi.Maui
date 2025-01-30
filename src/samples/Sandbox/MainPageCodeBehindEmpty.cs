using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{
    public class MainPageCodeBehindEmpty : BasePage, IDisposable
    {
        Canvas Canvas;

        public void Dispose()
        {
            this.Content = null;
            Canvas?.Dispose();
        }

        public MainPageCodeBehindEmpty()
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
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.LightGray
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
