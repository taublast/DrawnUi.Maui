 
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
                        //COLUMN
                        Type = LayoutType.Column,
                        Padding = 16,
                        Spacing = 0,
                        WidthRequest = 200,
                        VerticalOptions = LayoutOptions.Fill,
                        Children = new List<SkiaControl>()
                        {

                            new SkiaLayout()
                            {
                                HeightRequest = 100,
                                HorizontalOptions = LayoutOptions.Fill,
                                BackgroundColor = Colors.Red,
                            },

                            //ATTACHED MESSAGE
                            new SkiaLayout()
                            {
                                //WidthRequest = 200,
                                HorizontalOptions = LayoutOptions.Fill,
                                BackgroundColor = Color.FromHex("#11000000"),
                                Spacing = 0,
                                Children = new List<SkiaControl>()
                                {
                                    new SkiaShape()
                                    {
                                        Margin = new Thickness(8, 8, 0, 8),
                                        CornerRadius = 0,
                                        BackgroundColor = Colors.YellowGreen,
                                        HorizontalOptions = LayoutOptions.Start,
                                        WidthRequest = 2,
                                        VerticalOptions = LayoutOptions.Fill,
                                    },

                                    //new SkiaLabel()
                                    //{
                                    //    LineBreakMode = LineBreakMode.TailTruncation,
                                    //    MaxLines = 1,
                                    //    Text = $"Dev",
                                    //    FontSize = 15,
                                    //    HorizontalOptions = LayoutOptions.Fill,
                                    //    Margin = new Thickness(16, 8, 8, 0),
                                    //    TextColor = Colors.Black,
                                    //},

                                    new SkiaLabel()
                                    {
                                        LineBreakMode = LineBreakMode.TailTruncation,
                                        MaxLines = 1,
                                        Text = $"Что чего и как оно, как искать теперь его?",
                                        FontSize = 15,
                                        Margin = new Thickness(16, 28, 8, 8), //TODO BUG 2
                                        TextColor = Colors.Black,
                                    }
                                }
                            },

/*
           <draw:SkiaLabel
       Padding="10,0"
       AutoSize="FitHorizontal"
       FontSize="55"
       HeightRequest="64"
       HorizontalOptions="Fill"
       HorizontalTextAlignment="Center"
       MaxLines="1"
       Text="{Binding ResultDesc}"
       TextColor="{x:Static xam:TextColors.Result}"
       UseCache="Operations"
       VerticalOptions="Start"
       VerticalTextAlignment="Center" />
 *
 */

                            //AUTOSIZED ABSOLUTE
                            new SkiaLayout()
                            {
                                //WidthRequest = MessageMaxWidth,
                                BackgroundColor = Colors.Green,
                                Spacing = 0,
                                WidthRequest = 200,
                                VerticalOptions = LayoutOptions.Fill,
                                /*
                                Children = new List<SkiaControl>()
                                {
                                    new SkiaShape() //VLine
                                    {
                                        Margin = new Thickness(8, 8, 0, 8),
                                        CornerRadius = 0,
                                        BackgroundColor = Colors.IndianRed,
                                        HorizontalOptions = LayoutOptions.Start,
                                        WidthRequest = 2,
                                        VerticalOptions = LayoutOptions.Fill,
                                    },
                                      
                                    new SkiaLabel()
                                    {
                                        LineBreakMode = LineBreakMode.TailTruncation,
                                        MaxLines = 1,
                                        Text = $"...",
                                        FontSize = 15,
                                        WidthRequest = 200,
                                        Margin = new Thickness(16, 28, 8, 8), //TODO BUG 2 - looks like absolute bug
                                        TextColor = Colors.Black,
                                    }
                                }
                                */
                            },

                            new SkiaLayout()
                            {
                                HeightRequest = 100,
                                HorizontalOptions = LayoutOptions.Fill,
                                BackgroundColor = Colors.Blue,
                            }

           
                        }
                    }
                }
            };


            this.Content = Canvas;
        }
    }
}
