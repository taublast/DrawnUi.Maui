using System.Text;

using DrawnUi.Maui.Extensions;
using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{


    public class MainPageCodeTabs : BasePageCodeBehind, IDisposable
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

        void Init()
        {

            colorBar = Colors.Teal;
            colorUnselectedText = Colors.White;
            colorUnselected = Colors.White;
            colorSelected = Colors.White;
            selectedIconSize = 40; //pts
            unselectedIconSize = 26; //pts
            unselectedTextSize = 12; //pts
            unselectedSpacing = 3; //pts
        }
 

        private Color colorBar;
        private Color colorUnselectedText;
        private Color colorUnselected;
        private Color colorSelected;
        private float selectedIconSize;
        private float unselectedIconSize;
        private float unselectedTextSize;
        private float unselectedSpacing;

        public override void Build()
        {
            Canvas?.Dispose();

            Init();

            Canvas = new Canvas()
            {
                Gestures = GesturesMode.Lock,
                HardwareAcceleration = HardwareAccelerationMode.Enabled,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.Black,

                Content =

                new SkiaLayout()
                {
                    Padding = new(20),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Background = new LinearGradientBrush()
                    {
                        StartPoint = new(0, 0),
                        EndPoint = new(0, 1),
                        GradientStops = new GradientStopCollection()
                        {
                            new (Colors.LemonChiffon, 0),
                            new (Colors.White, 1),
                        }
                    }

                }.WithChildren(

                new SkiaMarkdownLabel()
                {
                    Text="__Floating Button TabBar__",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                },

                new CircleTabs()
                {
                    UseCache = SkiaCacheType.GPU,
                    AnimationSpeedMs = 300,
                    HeightRequest = 92,
                    BarColor = colorBar,
                    ButtonColor = colorBar,
                    ButtonPadding = 9,
                    ButtonSize = 70,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.End,
                    IndicatorsUnselected = [
                        new SkiaLayout()
                        {
                            UseCache = SkiaCacheType.Image,
                            Type = LayoutType.Column,
                            Spacing = unselectedSpacing,
                            Children = new List<SkiaControl>()
                            {
                                new SkiaSvg()
                                {
                                    SvgString = App.Current.Resources.Get<string>("SvgHome"),
                                    TintColor = colorUnselected,
                                    WidthRequest = unselectedIconSize,
                                    LockRatio = 1,
                                    HorizontalOptions = LayoutOptions.Center,
                                },
                                new SkiaLabel()
                                {
                                    TextColor = colorUnselectedText,
                                    Text = "Home", FontSize = unselectedTextSize,
                                    HorizontalOptions = LayoutOptions.Center,
                                }
                            }
                        },
                        new SkiaLayout()
                        {
                            UseCache = SkiaCacheType.Image,
                            Type = LayoutType.Column,
                            Spacing = unselectedSpacing,
                            Children = new List<SkiaControl>()
                            {
                                new SkiaSvg()
                                {
                                    SvgString = App.Current.Resources.Get<string>("SvgChat"),
                                    TintColor = colorUnselected,
                                    WidthRequest = unselectedIconSize,
                                    LockRatio = 1,
                                    HorizontalOptions = LayoutOptions.Center,
                                },
                                new SkiaLabel()
                                {
                                    TextColor = colorUnselectedText,
                                    Text = "Chat", FontSize = unselectedTextSize, HorizontalOptions = LayoutOptions.Center,
                                }
                            }
                        },
                        new SkiaLayout()
                        {
                            UseCache = SkiaCacheType.Image,
                            Type = LayoutType.Column,
                            Spacing = unselectedSpacing,
                            Children = new List<SkiaControl>()
                            {
                                new SkiaSvg()
                                {
                                    SvgString = App.Current.Resources.Get<string>("SvgSettings"),
                                    TintColor = colorUnselected,
                                    WidthRequest = unselectedIconSize,
                                    LockRatio = 1,
                                    HorizontalOptions = LayoutOptions.Center,
                                },
                                new SkiaLabel()
                                {
                                    TextColor = colorUnselectedText,
                                    Text = "Settings", FontSize = unselectedTextSize, HorizontalOptions = LayoutOptions.Center,
                                }
                            }
                        }
                    ],
                    IndicatorsSelected = [
                        new SkiaSvg()
                        {
                            Tag="Home",
                            UseCache = SkiaCacheType.Image,
                            SvgString = App.Current.Resources.Get<string>("SvgHome"),
                            VerticalOffset = -2,
                            TintColor = colorSelected,
                            VerticalAlignment = DrawImageAlignment.Start,
                            WidthRequest = selectedIconSize,
                            LockRatio = 1
                        },
                        new SkiaSvg()
                        {
                            Tag="Chat",
                            UseCache = SkiaCacheType.Image,
                            SvgString = App.Current.Resources.Get<string>("SvgChat"),
                            TintColor = colorSelected,
                            WidthRequest = selectedIconSize,
                            LockRatio = 1
                        },
                        new SkiaSvg()
                        {
                            Tag="Cogs",
                            UseCache = SkiaCacheType.Image,
                            SvgString = App.Current.Resources.Get<string>("SvgSettings"),
                            TintColor = colorSelected,
                            WidthRequest = selectedIconSize,
                            LockRatio = 1
                        },
                    ],
                }

                )
            };

 

            this.Content = Canvas;
        }

 

    }
}
