using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{


    public class MainPageTestImage : BasePage, IDisposable
    {
        Canvas Canvas;

        public void Dispose()
        {
            this.Content = null;
            Canvas?.Dispose();
        }


        public MainPageTestImage()
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
                                  VerticalOptions = LayoutOptions.Center,
                                  HorizontalOptions = LayoutOptions.Center,
                                  WidthRequest = 150,
                                  HeightRequest = 150,
                                  Type = ShapeType.Polygon,
                                  StrokeColor = Colors.Black,
                                  StrokeWidth = 3,
                                  BackgroundColor = Colors.Red,
                                  Points = new List<SkiaPoint>()
                                  {
                                      new (0.1f, 0.1f),
                                      new (0.9f, 0.1f),
                                      new (0.1f, 0.5f),
                                  },
                                  Content = new SkiaImage()
                                  {
                                      Source = "cap.png",
                                      Aspect = TransformAspect.AspectFit,
                                      VerticalOptions = LayoutOptions.Center,
                                      HorizontalOptions = LayoutOptions.Center
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
