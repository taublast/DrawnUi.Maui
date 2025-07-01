using DrawnUi.Views;
using DrawnUi.Controls;
using Canvas = DrawnUi.Views.Canvas;
using System.Collections.ObjectModel;
using AppoMobi.Specials;

namespace Sandbox
{
    public class MainPageCode : BasePageReloadable, IDisposable
    {
        Canvas Canvas;
        SkiaSpinner _spinner;
        SkiaLabel _selectedLabel;
        ObservableCollection<string> _spinnerItems;

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.Content = null;
                Canvas?.Dispose();
            }

            base.Dispose(isDisposing);
        }

        /// <summary>
        /// This will be called by HotReload
        /// </summary>
        public override void Build()
        {
            Canvas?.Dispose();

            // Initialize spinner items
            _spinnerItems = new ObservableCollection<string>
            {
                "Alice",
                "Bob",
                "Charlie",
                "Diana",
                "Edward",
                "Fiona",
                "George",
                "Hannah",
                "Ivan",
                "Julia",
                "Kevin",
                "Luna"
            };

            // Create the spinner
            _spinner = new SkiaSpinner
            {
                ItemsSource = _spinnerItems,
                InverseVisualRotation = true,
                SidePosition = SidePosition.Left,
                WheelRadius = 150,
                WidthRequest = 320,
                HeightRequest = 320,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            // Create selected item label
            _selectedLabel = new SkiaLabel
            {
                FontSize = 18,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20, 0, 0)
            }.Observe(_spinner, (me, prop) =>
            {
                if (prop.IsEither(nameof(BindingContext), nameof(SkiaSpinner.SelectedIndex)))
                {
                    var itemName = "None";
                    if (_spinner.SelectedIndex >= 0)
                    {
                        itemName = $"{_spinnerItems[_spinner.SelectedIndex]} [{_spinner.SelectedIndex}]";
                    }

                    me.Text = $"Selected: {itemName}";
                }
            });

            // Subscribe to selection changes
            _spinner.SelectedIndexChanged += OnSpinnerSelectionChanged;

            Canvas = new Canvas()
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.DarkSlateBlue,
                Gestures = GesturesMode.Enabled,
                Children =
                {
                    new SkiaLayer()
                    {
                        VerticalOptions = LayoutOptions.Fill,
                        Children =
                        {
                            new SkiaLayout()
                            {
                                Type = LayoutType.Column,
                                HorizontalOptions = LayoutOptions.Fill,
                                VerticalOptions = LayoutOptions.Fill,
                                Spacing = 20,
                                Padding = new Thickness(20),
                                Children =
                                {
                                    // Title
                                    new SkiaLabel()
                                    {
                                        Text = "SkiaSpinner Demo",
                                        FontSize = 24,
                                        FontWeight = FontWeights.Bold,
                                        TextColor = Colors.White,
                                        HorizontalOptions = LayoutOptions.Center,
                                        Margin = new Thickness(0, 20, 0, 0)
                                    },

                                    // Instructions
                                    new SkiaLabel()
                                    {
                                        Text = "Pan to spin the wheel, or tap the buttons below",
                                        FontSize = 14,
                                        TextColor = Colors.LightGray,
                                        HorizontalOptions = LayoutOptions.Center,
                                        HorizontalTextAlignment = DrawTextAlignment.Center
                                    },

                                    // Spinner container
                                    new SkiaLayout()
                                    {
                                        HorizontalOptions = LayoutOptions.Fill,
                                        VerticalOptions = LayoutOptions.Fill,
                                        Children = { _spinner }
                                    },

                                    // Selected item display
                                    _selectedLabel,

                                    // Control buttons
                                    new SkiaLayout()
                                    {
                                        Type = LayoutType.Row,
                                        HorizontalOptions = LayoutOptions.Center,
                                        Spacing = 15,
                                        Children =
                                        {
                                            new SkiaButton()
                                            {
                                                Text = "Spin Random",
                                                BackgroundColor = Colors.Orange,
                                                TextColor = Colors.White,
                                                CornerRadius = 8,
                                            }.OnTapped(me => { OnButtonTapped("spin"); }),
                                            new SkiaButton()
                                            {
                                                Text = "Add Item",
                                                BackgroundColor = Colors.Green,
                                                TextColor = Colors.White,
                                                CornerRadius = 8,
                                            }.OnTapped(me => { OnButtonTapped("add"); }),
                                            new SkiaButton()
                                            {
                                                Text = "Remove Item",
                                                BackgroundColor = Colors.Red,
                                                TextColor = Colors.White,
                                                CornerRadius = 8,
                                            }.OnTapped(me => { OnButtonTapped("remove"); }),
                                        }
                                    }
                                }
                            },
#if DEBUG
                            new SkiaLabelFps()
                            {
                                Margin = new(0, 0, 4, 24),
                                VerticalOptions = LayoutOptions.End,
                                HorizontalOptions = LayoutOptions.End,
                                Rotation = -45,
                                BackgroundColor = Colors.DarkRed,
                                TextColor = Colors.White,
                                ZIndex = 110,
                            }
#endif
                        }
                    }
                }
            };

            this.Content = Canvas;
        }

        void OnSpinnerSelectionChanged(object sender, int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < _spinnerItems.Count)
            {
                _selectedLabel.Text = $"Selected: {_spinnerItems[selectedIndex]}";
            }
            else
            {
                _selectedLabel.Text = "Selected: None";
            }
        }

        void OnButtonTapped(object parameter)
        {
            var action = parameter?.ToString();

            switch (action)
            {
                case "spin":
                    _spinner.SpinToRandom();
                    break;

                case "add":
                    if (_spinnerItems.Count < 150)
                    {
                        var newName = $"Person {_spinnerItems.Count + 1}";
                        _spinnerItems.Add(newName);
                    }

                    break;

                case "remove":
                    if (_spinnerItems.Count > 2)
                    {
                        _spinnerItems.RemoveAt(_spinnerItems.Count - 1);
                    }

                    break;
            }
        }
    }
}
