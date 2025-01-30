using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{


    public class MainPageDevShape : BasePage, IDisposable
    {
        Canvas Canvas;

        public void Dispose()
        {
            this.Content = null;
            Canvas?.Dispose();
        }

        public MainPageDevShape()
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
                Gestures = GesturesMode.Enabled,
                HardwareAcceleration = HardwareAccelerationMode.Enabled,

                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Fill,
                WidthRequest = 300,
                HeightRequest = 300,
                BackgroundColor = Colors.White,

                Content = new SkiaLayout()
                {
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                    Children = new List<SkiaControl>()
                    {

                        new SkiaImage()
                        {
                            Source="car.png",
                            WidthRequest = 150,
                            HeightRequest = 150,
                            VerticalOptions = LayoutOptions.End,
                            Margin = 24,
                            Shadow = new Shadow()
                            {
                                Radius = 8,
                                Brush= Colors.Purple,
                                Offset = new (5,5)
                            }
                        }
                    }
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
