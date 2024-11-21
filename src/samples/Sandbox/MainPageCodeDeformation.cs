using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{


    public class MainPageCodeDeformation : BasePage, IDisposable
    {
        Canvas Canvas;

        public void Dispose()
        {
            this.Content = null;
            Canvas?.Dispose();
        }


        public static (SkiaShape Shape, Point Point) CreateSkiaLine(Color color, double x1, double y1, double x2, double y2, int width = 5)
        {
            var distX = x1 - x2;
            var distY = y1 - y2;
            var lineDist = Math.Sqrt(distX * distX + distY * distY);

            var lineCenterX = x2 + (distX / 2);
            var lineCenterY = y2 + (distY / 2);
            var lineFinalX = lineCenterX - (lineDist / 2);

            var lineRotateAngle = Math.Acos(distX / lineDist) * (180 / Math.PI);

            var line = new SkiaShape()
            {
                Type = ShapeType.Rectangle,
                BackgroundColor = color,
                WidthRequest = lineDist,
                HeightRequest = width,
                Rotation = (distY < 0) ? 360 - lineRotateAngle : lineRotateAngle
            };

            return (line, new Point(lineFinalX, lineCenterY));
        }

        public MainPageCodeDeformation()
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
                                CreateSkiaLine(Colors.Red, 50, 50, 300, 102, 5).Shape

                            }
                        }.With((c) =>
                        {
                            foreach (Point node in new Point[]
                                     {
                                         new(150, 150),
                                         new(250,200),
                                         new(250,150),
                                         new(250,300),
                                         new(250,250),
                                         new(350,150),
                                         new(450,150),
                                         new(300,250),
                                         new(300,300),
                                         new(300,360),
                                         new(150,350)
                                     })
                            {
                                c.AddSubView(CreateSkiaLine(Colors.Blue, 75, 175, node.X, node.Y).Shape);
                            }
                        })
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
