using DrawnUi.Maui.Draw;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Visibility = Microsoft.UI.Xaml.Visibility;

namespace DrawnUi.Maui.Draw
{
    public partial class SkiaEditor : SkiaLayout, ISkiaGestureListener
    {
        public int NativeSelectionStart
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void SetCursorPositionNative(int position, int stop = -1)
        {
            //todo
        }


        public void DisposePlatform()
        {
            //todo

        }

        public void UpdateNativePosition()
        {
            //todo
        }

        private TextBox _hiddenTextBox;

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
                            Width = 0,
                            Height = 0,
                            Visibility = Visibility.Collapsed,
                            Name = "HiddenTextBox" + GenerateUniqueId()
                        };

                        // Add the TextBox to your layout
                        layout.Children.Add(_hiddenTextBox);
                    }

                    // Request focus and show the keyboard
                    _hiddenTextBox.Focus(FocusState.Programmatic);
                }
                else
                {
                    if (_hiddenTextBox != null)
                    {
                        // Remove focus and hide the keyboard
                        _hiddenTextBox.Visibility = Visibility.Collapsed;
                        _hiddenTextBox.IsTabStop = false;

                        // Remove the hidden TextBox from the layout
                        layout.Children.Remove(_hiddenTextBox);
                        _hiddenTextBox = null;
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public int GenerateUniqueId()
        {
            long currentTime = DateTime.Now.Ticks;
            int uniqueId = unchecked((int)currentTime);
            return uniqueId;
        }
    }

}
