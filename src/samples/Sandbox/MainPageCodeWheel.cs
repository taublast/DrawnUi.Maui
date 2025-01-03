using AppoMobi.Maui.DrawnUi.Demo.Views.Controls;
using Sandbox.Views;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{


    public class MainPageCodeWheel : BasePage, IDisposable
    {
        Canvas Canvas;

        public void Dispose()
        {
            this.Content = null;
            Canvas?.Dispose();
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

        public MainPageCodeWheel()
        {
#if DEBUG
            HotReloadService.UpdateApplicationEvent += ReloadUI;
#endif
            Build();
        }

        private int _reloads;

        void Build()
        {
            var itemsSource = Enumerable.Range(1, 12).Select(x => $"{x * 10} m").ToList();

            var selector = new MyDataTemplateSelector();

            Canvas?.Dispose();

            Canvas = new Canvas()
            {
                Gestures = GesturesMode.Lock,
                HardwareAcceleration = HardwareAccelerationMode.Enabled,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
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


                    new SkiaLayout()
                    {
                        Tag = "1",
                        Type = LayoutType.Column,
                        BackgroundColor = Colors.Blue,
                        HorizontalOptions = LayoutOptions.Fill,
                        Children = new List<SkiaControl>()
                        {
                            new SkiaMarkdownLabel()
                            {
                                Margin=16,
                                FontFamily="OpenSansRegular",
                                FontSize=15,
                                Text="`CODE` xx",
                                TextColor=Colors.White,
                            }.CenterX(),
                            new WheelPicker()
                            {
                                Margin = 100,
                                Tag = "Picker",
                                DataSource = itemsSource,
                                //ItemTemplate = new DataTemplate(() =>
                                //{
                                //    return new SkiaLabel()
                                //    {
                                //        Text = "???",
                                //        BackgroundColor = Colors.Yellow
                                //    };

                                //})
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
