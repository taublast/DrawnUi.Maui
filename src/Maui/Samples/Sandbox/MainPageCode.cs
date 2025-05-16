using Sandbox.Views;
using Canvas = DrawnUi.Views.Canvas;

namespace Sandbox
{
    public class MainPageCode : BasePageCodeBehind, IDisposable
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
                Gestures = GesturesMode.Enabled,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.White,
                //Content = new SkiaLottie()
                //{
                //    DefaultFrame = -1,
                //    Source = @"Lottie\ok.json",
                //    WidthRequest = 100,
                //    HeightRequest = 100,
                //    HorizontalOptions = LayoutOptions.Fill,
                //}
                Content = new SkiaLayout()
                {
                    BackgroundColor = Colors.Red,
                    Tag = "Container",
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Children = new List<SkiaControl>()
                    {


                        new SkiaEditor()
                        {
                            HeightRequest = 100,
                            WidthRequest = 300,
                            BackgroundColor = Colors.White,
                            TextColor = Colors.Black,
                            FontSize = 14
                        }

                    }
                }.Fill()
            };


            this.Content = Canvas;
        }
    }
}
