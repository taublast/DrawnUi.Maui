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
                        BackgroundColor = Colors.Black,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        Children =
                        {
                            new SkiaLayout()
                            {
                                WidthRequest = 200,
                                VerticalOptions = LayoutOptions.Fill,
                                Children = new List<SkiaControl>()
                                {
                                    new SkiaStack()
                                    {
                                        Spacing = 0,
                                        BackgroundColor = Colors.LightPink,
                                        VerticalOptions = LayoutOptions.Fill,
                                        Children =
                                        {
                                            new SkiaLayer()
                                            {
                                                Tag = "1",
                                                HeightRequest = 100,
                                                BackgroundColor = Colors.Red,
                                            },
                                            new SkiaLayer()
                                            {
                                                Tag = "2 - Fill",
                                                VerticalOptions = LayoutOptions.Fill,
                                                BackgroundColor = Colors.Green,
                                            },
                                            new SkiaButton()
                                            {
                                                Tag = "3",
                                                CornerRadius = 11,
                                                HeightRequest = 38,
                                                WidthRequest = 100,
                                                Text = "XXXXXX",
                                                BackgroundColor = Colors.White,
                                                TextColor = Colors.Black,
                                                VerticalOptions = LayoutOptions.Start,
                                                //UseCache = SkiaCacheType.Image,
                                                HorizontalOptions = LayoutOptions.Center,
                                                Margin = 16
                                            },
                                        }
                                    }
                                }
                            }
                        }
                    }

      
                }
            };


            this.Content = Canvas;
        }
    }
}
