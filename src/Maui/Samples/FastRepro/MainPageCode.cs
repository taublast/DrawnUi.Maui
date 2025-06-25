using AppoMobi.Maui.DrawnUi.Demo.Views.Controls;
using DrawnUi.Views;
using Canvas = DrawnUi.Views.Canvas;

namespace Sandbox
{
    public class MainPageCode : BasePageReloadable, IDisposable
    {
        Canvas Canvas;

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.Content = null;
                Canvas?.Dispose();
            }

            base.Dispose(isDisposing);
        }


        public override void Build()
        {
            Canvas?.Dispose();

            Canvas = new Canvas()
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.White,
                Gestures = GesturesMode.Enabled,
                Children =
                {
                    new SkiaLayout()
                    {
                        //Type = LayoutType.Column,
                        WidthRequest = 200,
                        VerticalOptions = LayoutOptions.Fill,
                        Children = new List<SkiaControl>()
                        {
                            new SkiaShape() //VLine
                            {
                                UseCache = SkiaCacheType.Image,
                                Margin = new Thickness(10, 0, 0, 0),
                                Tag="BuggyShape",
                                Type = ShapeType.Circle,
                                WidthRequest = 30,
                                LockRatio = 1,
                                StrokeColor = Colors.Black,
                                StrokeWidth = 2,
                                Padding = 2,
                                BackgroundColor = Colors.IndianRed,
                                Children = new List<SkiaControl>()
                                {
                                    new SkiaShape()
                                        {
                                            Tag="ChildShape",
                                            BackgroundColor = Colors.DarkGray, Type = ShapeType.Circle
                                        }
                                        .Fill()
                                }
                            },
                        }
                    }
                }
            };


            this.Content = Canvas;
        }
    }
}
