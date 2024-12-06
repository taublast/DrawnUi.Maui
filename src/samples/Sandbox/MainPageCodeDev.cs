using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;
using TextTransform = DrawnUi.Maui.Draw.TextTransform;

namespace Sandbox
{


    public class MainPageCodeDev : BasePage, IDisposable
    {
        Canvas Canvas;

        public void Dispose()
        {
            this.Content = null;
            Canvas?.Dispose();
        }




        public MainPageCodeDev()
        {
#if DEBUG
            HotReloadService.UpdateApplicationEvent += ReloadUI;
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
                HardwareAcceleration = HardwareAccelerationMode.Disabled,

                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.LightGray,

                Content = new SkiaLayout()
                {
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                    Children = new List<SkiaControl>()
                    {
                        new SkiaLayout()
                        {
                            Children = new List<SkiaControl>()
                            {
                                new SkiaShape()
                                {
                                    HorizontalOptions = LayoutOptions.Fill,
                                    HeightRequest = 128,
                                    StrokeColor = Colors.Black,
                                    StrokeWidth = 1,
                                    Margin = 8,
                                    CornerRadius = 10,
                                    Padding = 10,
                                    BackgroundColor = Colors.White,
                                    Content = new SkiaMarkdownLabel()
                                    {
                                        Text = "hello ppl how are you\r\nNew line comes here\r\nAnd another one too",
                                        TextTransform = TextTransform.Titlecase,
                                        MaxLines = 2
                                    }

                                }

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
