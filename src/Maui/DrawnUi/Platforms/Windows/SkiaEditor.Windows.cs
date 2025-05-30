using DrawnUi.Draw;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Text;
using System.Diagnostics;
using TextChangedEventArgs = Microsoft.UI.Xaml.Controls.TextChangedEventArgs;
using Visibility = Microsoft.UI.Xaml.Visibility;

namespace DrawnUi.Draw
{
    public partial class SkiaEditor : SkiaLayout, ISkiaGestureListener
    {
        private TextBox _hiddenTextBox;
        private bool _updatingText;

        public int NativeSelectionStart
        {
            get
            {
                if (_hiddenTextBox != null)
                {
                    return _hiddenTextBox.SelectionStart;
                }
                return 0;
            }
        }

        public void SetCursorPositionNative(int position, int stop = -1)
        {
            if (_hiddenTextBox != null)
            {
                try
                {
                    _hiddenTextBox.SelectionStart = Math.Min(position, _hiddenTextBox.Text?.Length ?? 0);
                    if (stop >= 0)
                    {
                        _hiddenTextBox.SelectionLength = Math.Max(0, Math.Min(stop, _hiddenTextBox.Text?.Length ?? 0) - _hiddenTextBox.SelectionStart);
                    }
                    else
                    {
                        _hiddenTextBox.SelectionLength = 0;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"[SetCursorPositionNative] {e}");
                }
            }
        }

        public void DisposePlatform()
        {
            try
            {
                if (_hiddenTextBox != null)
                {
                    _hiddenTextBox.TextChanged -= HiddenTextBox_TextChanged;
                    _hiddenTextBox.SelectionChanged -= HiddenTextBox_SelectionChanged;
                    
                    var layout = (Panel)Superview?.Handler?.PlatformView;
                    if (layout != null)
                    {
                        layout.Children.Remove(_hiddenTextBox);
                    }
                    _hiddenTextBox = null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[DisposePlatform] {e}");
            }
        }

        public void UpdateNativePosition()
        {
            try
            {
                if (_hiddenTextBox != null)
                {
                    var layout = (Panel)Superview?.Handler?.PlatformView;
                    if (layout != null)
                    {
                        // Update size
                        _hiddenTextBox.Width = DrawingRect.Width;
                        _hiddenTextBox.Height = DrawingRect.Height;

                        // Measure with the new size
                        _hiddenTextBox.Measure(new Windows.Foundation.Size(DrawingRect.Width, DrawingRect.Height));

                        // Arrange at the correct position
                        _hiddenTextBox.Arrange(new Windows.Foundation.Rect(
                            DrawingRect.Left,
                            DrawingRect.Top,
                            DrawingRect.Width,
                            DrawingRect.Height));

                        // Force layout update
                        _hiddenTextBox.UpdateLayout();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[UpdateNativePosition] {e}");
            }
        }

        public void SetFocusNative(bool focus)
        {
            try
            {
                var layout = (Panel)Superview.Handler?.PlatformView;

                if (focus)
                {
                    if (_hiddenTextBox == null)
                    {
                        // Create the hidden TextBox if it does not exist
                        _hiddenTextBox = new TextBox
                        {
                            IsReadOnly = false,
                            AcceptsReturn = true,
                            TextWrapping = TextWrapping.Wrap,
                            Width = 0,
                            Height = 0,
                            Visibility = Visibility.Visible,
                            Name = "HiddenTextBox" + GenerateUniqueId(),
                            Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(1, 0, 0, 0))
                        };

                        _hiddenTextBox.TextChanged += HiddenTextBox_TextChanged;
                        _hiddenTextBox.SelectionChanged += HiddenTextBox_SelectionChanged;

                        // Add the TextBox to your layout
                        layout.Children.Add(_hiddenTextBox);
                        
                        // Update position right away
                        UpdateNativePosition();
                    }

                    // Request focus and show the keyboard
                    _hiddenTextBox.Focus(FocusState.Programmatic);
                }
                else
                {
                    if (_hiddenTextBox != null)
                    {
                        // Remove focus and hide the keyboard
                        _hiddenTextBox.IsTabStop = false;

                        // Remove event handlers
                        _hiddenTextBox.TextChanged -= HiddenTextBox_TextChanged;
                        _hiddenTextBox.SelectionChanged -= HiddenTextBox_SelectionChanged;

                        // Remove the hidden TextBox from the layout
                        layout.Children.Remove(_hiddenTextBox);
                        _hiddenTextBox = null;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[SetFocusNative] {e}");
            }
        }

        private void HiddenTextBox_TextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if (!_updatingText)
            {
                _updatingText = true;
                Text = _hiddenTextBox.Text;
                _updatingText = false;
            }
        }

        private void HiddenTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // This allows parent control to track selection changes
            SetCursorPositionWithDelay(50, _hiddenTextBox.SelectionStart);
        }

        public int GenerateUniqueId()
        {
            long currentTime = DateTime.Now.Ticks;
            int uniqueId = unchecked((int)currentTime);
            return uniqueId;
        }
    }
}
