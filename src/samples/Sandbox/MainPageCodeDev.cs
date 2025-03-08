using System.Diagnostics;
using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{


    public class MainPageCodeDev : BasePageCodeBehind, IDisposable
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


        public class MyDataTemplateSelector : DataTemplateSelector
        {


            protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
            {
                return new DataTemplate(() =>
                {
                    return new SkiaLabel()
                    {
                        HeightRequest = 32,
                        BackgroundColor = Colors.Red,
                        HorizontalOptions = LayoutOptions.Fill
                    };
                });
            }
        }

        public override void Build()
        {
            Canvas?.Dispose();

            Canvas = new Canvas()
            {
                WidthRequest = 210,
                HorizontalOptions = LayoutOptions.Center,
                Gestures = GesturesMode.Lock,
                HardwareAcceleration = HardwareAccelerationMode.Enabled,
                VerticalOptions = LayoutOptions.Fill,
                HeightRequest = 250,
                BackgroundColor = Colors.Gray,

                //Content = new SkiaLayout()
                //{
                //    Type = LayoutType.Column,
                //    Spacing = 8,
                //    BackgroundColor = Colors.White,
                //    HorizontalOptions = LayoutOptions.Fill,
                //    ItemsSource = itemsSource,
                //    ItemTemplate = selector
                //}

                Content =

                    new SkiaLayout() //wrapper
                    {
                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalOptions = LayoutOptions.Fill,
                    }.WithChildren(

                           new SkiaLayout()
                           {
                               Tag = "1",
                               Type = LayoutType.Column,
                               BackgroundColor = Colors.Gray,
                               VerticalOptions = LayoutOptions.Fill,
                               HorizontalOptions = LayoutOptions.Fill,
                               Children = new List<SkiaControl>()
                    {
                        //new SkiaMarkdownLabel()
                        //{
                        //    Margin=16,
                        //    FontFamily="OpenSansRegular",
                        //    FontSize=15,
                        //    Text="`CODE` xx",
                        //    TextColor=Colors.White,
                        //}.CenterX(),

                 
                    }
                           },
                           //new SkiaLabel()
                           //{
                           //    Margin = new(8),
                           //    VerticalOptions = LayoutOptions.Center,
                           //    ZIndex = 100,
                           //    FontSize = 8,
                           //    TextColor = Colors.White,
                           //    BackgroundColor = Colors.Black,
                           //}.Adjust((c) =>
                           //{
                           //    c.SetBinding(SkiaLabel.TextProperty, static (WheelScroll x) => x.DebugWheel, source: wheel);
                           //}),
                           new SkiaLabelFps()
                           {
                               Margin = new(0,0,4,24),
                               VerticalOptions = LayoutOptions.End,
                               HorizontalOptions = LayoutOptions.End,
                               Rotation = -45,
                                BackgroundColor = Colors.DarkRed,
                               TextColor = Colors.White,
                               ZIndex = 110,
                           }

                    )

         

            };

            this.Content = Canvas;
        }


    }
}
