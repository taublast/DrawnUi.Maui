using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;

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
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 100,
                HeightRequest = 100,
                BackgroundColor = Colors.Red,
                Content = new SkiaLottie()
                {
                    DefaultFrame = -1,
                    Source = @"Lottie\ok.json",
                    WidthRequest = 100,
                    HeightRequest = 100,
                    HorizontalOptions = LayoutOptions.Fill,
                }
            };

 

            this.Content = Canvas;
        }

 

    }
}
