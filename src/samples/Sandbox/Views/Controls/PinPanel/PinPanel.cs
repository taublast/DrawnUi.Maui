using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AppoMobi.Maui.Gestures;
using AppoMobi.Specials;
using SkiaLabel = DrawnUi.Maui.Draw.SkiaLabel;

namespace Sandbox
{
    public class PinPanel : SkiaLayout
    {
        protected SkiaMauiEntry EntryHidden;
        protected SkiaLayout CellsLayout;
        public Color ColorUnderline;
        public Color ColorFocused;
        public Color ColorBackground;
        public string FontCell;

        public PinPanel()
        {
            ColorUnderline = Colors.GreenYellow;
            ColorBackground = Colors.DarkGray;
            ColorFocused = Colors.White;
            FontCell = "FontText";

            Children = new List<SkiaControl>()
            {
                new SkiaMauiEntry()
                {
                    BackgroundColor = Colors.Black, HeightRequest = 8, WidthRequest = 8, TranslationX = -3000,
                }.Adapt((c) =>
                {
                    EntryHidden = c;
                    c.TextChanged += EntryHiddenTextChanged;
                    c.FocusChanged += (sender, focused) =>
                    {
                        Debug.WriteLine($"ENTRY Focus changed to {focused}!");
                        //if (focused)
                        //{
                        //    FocusedElement.Instance.VisualElement = (VisualElement)sender;
                        //}
                        //else
                        //{
                        //    if (FocusedElement.Instance.VisualElement == (VisualElement)sender)
                        //        FocusedElement.Instance.VisualElement = null;
                        //}
                    };
                }),
                new SkiaLayout()
                    {
                        Type = LayoutType.Row,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        ItemTemplate = new DataTemplate(() =>
                        {
                            var cell = new SkiaLayout() // DIGIT CELL
                                {
                                    BackgroundColor = ColorBackground,
                                    HeightRequest = 47,
                                    WidthRequest = 32,
                                    Children = new List<SkiaControl>()
                                    {
                                        new SkiaShape()
                                        {
                                            Type = ShapeType.Line,
                                            StrokeColor = ColorUnderline,
                                            StrokeWidth = 2,
                                            HorizontalOptions = LayoutOptions.Fill,
                                            VerticalOptions = LayoutOptions.End
                                        },
                                        new SkiaLabel()
                                        {
                                            FontFamily = FontCell,
                                            FontSize = 18,
                                            HorizontalOptions = LayoutOptions.Fill,
                                            VerticalOptions = LayoutOptions.Fill,
                                            HorizontalTextAlignment = DrawTextAlignment.Center,
                                            VerticalTextAlignment = TextAlignment.Center,
                                        }.Adapt((label) =>
                                        {
                                            label.SetBinding(SkiaLabel.TextProperty, "Display"); //todo compiled
                                        })
                                    }
                                }
                                .Subscribe(EntryHidden, (me, prop) =>
                                {
                                    //programmatic triggers
                                    if (prop == nameof(EntryHidden.IsFocused))
                                    {
                                        if (EntryHidden.IsFocused)
                                        {
                                            me.BackgroundColor = ColorFocused;
                                        }
                                        else
                                        {
                                            me.BackgroundColor = ColorBackground;
                                        }
                                    }
                                });
                            return cell;
                        })
                    }
                    .Assign(out CellsLayout)
            };

            OnGestures = (parameters, info) =>
            {
                if (parameters.Type == TouchActionResult.Tapped)
                {
                    if (EntryHidden.IsFocused)
                        Unfocus();
                    else
                        Focus();

                    return this;
                }

                return null;
            };

            Reset();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName.IsEither(nameof(Limit), nameof(ChangeToReset)))
            {
                Reset();
            }
            else if (propertyName == nameof(ToggleKeyboard))
            {
                if (ToggleKeyboard)
                    Focus();
                else
                    Unfocus();
            }
        }

        #region Command

        public static readonly BindableProperty FinishedCommandProperty = BindableProperty.Create(
            nameof(FinishedCommand),
            typeof(ICommand),
            typeof(PinPanel),
            null);

        public ICommand FinishedCommand
        {
            get { return (ICommand)GetValue(FinishedCommandProperty); }
            set { SetValue(FinishedCommandProperty, value); }
        }

        public static readonly BindableProperty ChangedCommandProperty = BindableProperty.Create(
            nameof(ChangedCommand),
            typeof(ICommand),
            typeof(PinPanel),
            null);

        public ICommand ChangedCommand
        {
            get { return (ICommand)GetValue(ChangedCommandProperty); }
            set { SetValue(ChangedCommandProperty, value); }
        }

        public static readonly BindableProperty EmptyCommandProperty = BindableProperty.Create(
            nameof(EmptyCommand),
            typeof(ICommand),
            typeof(PinPanel),
            null);

        public ICommand EmptyCommand
        {
            get { return (ICommand)GetValue(EmptyCommandProperty); }
            set { SetValue(EmptyCommandProperty, value); }
        }

        #endregion

        public void Reset()
        {
            if (EntryHidden != null)
                EntryHidden.Text = "";

            var source = new List<CDigitCell>();
            for (int i = 0; i < Limit; i++)
            {
                source.Add(new CDigitCell { Value = -1, Display = "" });
            }

            Digits = source;
            CellsLayout.ItemsSource = Digits;
        }

        public static readonly BindableProperty ToggleKeyboardProperty = BindableProperty.Create(nameof(ToggleKeyboard),
            typeof(bool), typeof(PinPanel), false); //, BindingMode.TwoWay

        public bool ToggleKeyboard
        {
            get { return (bool)GetValue(ToggleKeyboardProperty); }
            set { SetValue(ToggleKeyboardProperty, value); }
        }

        public List<CDigitCell> Digits { get; protected set; }

        private string DebuggerDisplay()
        {
            if (Digits == null)
                return "NULL";

            var ret = "";
            foreach (var digit in Digits)
            {
                ret += $"[{digit.Display}] ";
            }

            return ret;
        }

        public static readonly BindableProperty LimitProperty =
            BindableProperty.Create(nameof(Limit), typeof(int), typeof(PinPanel), 4); //, BindingMode.TwoWay

        public int Limit
        {
            get { return (int)GetValue(LimitProperty); }
            set { SetValue(LimitProperty, value); }
        }

        public static readonly BindableProperty ResultProperty = BindableProperty.Create(nameof(Result), typeof(string),
            typeof(PinPanel), string.Empty);

        public string Result
        {
            get { return (string)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly BindableProperty ChangeToResetProperty =
            BindableProperty.Create(nameof(ChangeToReset), typeof(int), typeof(PinPanel), 0);

        public int ChangeToReset
        {
            get { return (int)GetValue(ChangeToResetProperty); }
            set { SetValue(ChangeToResetProperty, value); }
        }

        public new void Focus()
        {
            EntryHidden?.SetFocus(true);
            Debug.WriteLine("Trying to focus");
        }

        public new void Unfocus()
        {
            EntryHidden?.SetFocus(false);
            Debug.WriteLine("Trying to unfocus");
        }

        private void EntryHiddenTextChanged(object sender, string text)
        {
            Debug.WriteLine($"ENTRY EntryHiddenTextChanged {text}");


            if (Digits == null || text == null)
                return;

            if (text.Length > Digits.Count)
            {
                Debug.WriteLine($"limit exceeded");
                EntryHidden.Text = EntryHidden.OldText;
                return;
            }

            bool validated = text.All(Char.IsDigit);
            var length = 0;
            if (validated)
            {
                length = text.Length;
                if (length > Digits.Count)
                    validated = false; //thou shell not pass
                else
                {
                    var pos = -1;
                    if (length > 0) //not empty
                    {
                        foreach (var digit in Digits)
                        {
                            if (true) //?
                            {
                                pos++;
                                if (pos < length)
                                {
                                    digit.Display = text[pos].ToString();
                                    digit.Value = text[pos].ToString().ToInteger();
                                }
                                else
                                {
                                    //delete to the right just in case
                                    digit.Display = "";
                                    digit.Value = -1;
                                }
                            }
                        }
                    }
                }
            }

            if (!validated)
            {
                //INVALID
                EntryHidden.Text = EntryHidden.OldText;
                return;
            }

            Result = text.ToUpperInvariant();

            if (ChangedCommand != null)
            {
                if (ChangedCommand.CanExecute(null))
                {
                    ChangedCommand?.Execute(Result);
                }
            }

            if (length == Digits.Count && FinishedCommand != null)
            {
                if (FinishedCommand.CanExecute(null))
                {
                    FinishedCommand?.Execute(Result);
                }
            }

            if (length == 0 && EmptyCommand != null)
            {
                if (EmptyCommand.CanExecute(null))
                {
                    EmptyCommand?.Execute(Result);
                }
            }
        }
    }
}
