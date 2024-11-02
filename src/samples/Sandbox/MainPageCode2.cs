using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{


    public class MainPageCode2 : BasePage, IDisposable
    {
        Canvas Canvas;

        public void Dispose()
        {
            this.Content = null;
            Canvas?.Dispose();
        }

        public MainPageCode2()
        {
#if DEBUG
            HotReloadService.UpdateApplicationEvent += ReloadUI;
#endif
            Build();
        }

        private int _reloads;

        internal class CustomShape : SkiaShape
        {
            public CustomShape()
            {
                Type = ShapeType.Circle;
            }

            public override ISkiaGestureListener ProcessGestures(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
            {
                if (args.Type == AppoMobi.Maui.Gestures.TouchActionResult.Tapped)
                {
                    this.BackgroundColor = Colors.Blue;
                    return this;
                }

                return base.ProcessGestures(args, apply);
            }
        }

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
                BackgroundColor = Colors.Gray,

                Content = new SkiaLayout()
                {
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                    BackgroundColor = Colors.Bisque,
                    Children = new List<SkiaControl>()
                {
                    new SkiaLayout()
                    {
                        HeightRequest = 200,
                        BackgroundColor = Colors.Green,
                        Children = new List<SkiaControl>()
                        {
                            new SkiaLayout()
                            {
                                ColumnSpacing = 0,
                                BackgroundColor = Colors.Green,
                                Margin=new Thickness(0),
                                Type = LayoutType.Grid,
                                RowDefinitions = new RowDefinitionCollection()
                                {
                                    new RowDefinition(new GridLength(1,GridUnitType.Star)),
                                    new RowDefinition(new GridLength(100,GridUnitType.Absolute)),
                                    new RowDefinition(new GridLength(1,GridUnitType.Star)),
                                },
                                Children = new List<SkiaControl>()
                                {
                                    new CustomShape()
                                    {
                                        Tag="1",
                                        HeightRequest = 50,
                                        WidthRequest = 50,
                                        HorizontalOptions = LayoutOptions.Center,
                                        BackgroundColor = Colors.Red,
                                    }.WithRow(0),
                                    new SkiaShape()
                                    {
                                        Tag="2",
                                        HorizontalOptions = LayoutOptions.Fill,
                                        BackgroundColor = Colors.Yellow,
                                    }.WithRow(1),
                                    new CustomShape()
                                    {
                                        Tag="3",
                                        HeightRequest = 50,
                                        WidthRequest = 50,
                                        HorizontalOptions = LayoutOptions.Center,
                                        BackgroundColor = Colors.Red,
                                    }.WithRow(2),
                                }
                            },


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
