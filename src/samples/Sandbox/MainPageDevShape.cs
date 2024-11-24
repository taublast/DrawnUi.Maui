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
                        new SkiaShape()
                        {
                            SmoothPoints = 0.9f,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            WidthRequest = 150,
                            HeightRequest = 150,
                            Type = ShapeType.Polygon,
                            StrokeColor = Colors.Black,
                            StrokeWidth = 3,
                            StrokeBlendMode = SKBlendMode.SrcIn,
                            StrokeCap = SKStrokeCap.Round,
                            Rotation = 0,
                            BackgroundColor = Colors.Yellow,
                            //Points = SkiaShape.CreateStarPoints(5, 0.5),
                            Shadows = new List<SkiaShadow>()
                            {

                            },
                            Points = new List<SkiaPoint>()
                            {
                                new (0.1f, 0.1f),
                                new (0.9f, 0.1f),
                                new (0.8f, 0.9f),
                                new (0.1f, 0.9f),
                            },
                        },
                        new SkiaShape()
                        {
                            IsVisible = false,
                            Type = ShapeType.Line,
                            StrokeColor = Colors.Red,
                            StrokeWidth = 2,
                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalOptions = LayoutOptions.Fill,
                            Points = new List<SkiaPoint>()
                            {
                                new (0.2f, 0.8f),
                                new (0.8f, 0.2f),
                                new (1.0f, 0.2f),
                            },
                        },
                        new SkiaShape()
                        {
                            IsVisible = false,
                            SmoothPoints = 0.3f,
                            Type = ShapeType.Line,
                            StrokeColor = Colors.Green,
                            StrokeWidth = 2,
                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalOptions = LayoutOptions.Fill,
                            Points = new List<SkiaPoint>()
                            {
                                new (0.2f, 0.8f),
                                new (0.8f, 0.2f),
                                new (1.0f, 0.2f),
                            },
                        },
                        new SkiaImage()
                        {
                            Source="car.png",
                            WidthRequest = 100,
                            HeightRequest = 100,
                            VerticalOptions = LayoutOptions.End,
                            Margin = 10,
                            Shadow = new Shadow()
                            {
                                Radius = 3,
                                Brush= Colors.Black,
                                Offset = new (2,2)
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
