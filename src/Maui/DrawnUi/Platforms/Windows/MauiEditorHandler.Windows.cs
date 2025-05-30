
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using Brush = Microsoft.UI.Xaml.Media.Brush;
using TextChangedEventArgs = Microsoft.UI.Xaml.Controls.TextChangedEventArgs;

namespace DrawnUi.Controls;


public partial class MauiEditorHandler : EditorHandler
{

    TextBox _control;

    protected override void ConnectHandler(TextBox platformView)
    {
        base.ConnectHandler(platformView);

        _control = platformView;

        platformView.Text = this.VirtualView.Text;

        //platformView.EditorAction += OnEditorAction;
        platformView.TextChanged += OnTextChanged;

        ApplySettings();
    }

    void ApplySettings()
    {
        if (_control != null)
        {
            _control.Background = null; // new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Transparent.ToWindowsColor());
            _control.BorderBrush = null;
            _control.Padding = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 0);
        }
    }

    public override void PlatformArrange(Rect frame)
    {
        base.PlatformArrange(frame);

        ApplySettings();
    }


    protected override TextBox CreatePlatformView()
    {
        var view = base.CreatePlatformView();

        _control = view;

        //_control.GotFocus += (sender, args) =>
        //{
        //    (VirtualView as MauiEntry).Background = Microsoft.Maui.Controls.Brush.Transparent;
        //};

        return _control;
    }

    protected override void DisconnectHandler(TextBox platformView)
    {
        base.DisconnectHandler(platformView);

        _control = null;

        //platformView.EditorAction -= OnEditorAction;
        platformView.TextChanged -= OnTextChanged;
    }

    public int CountLines { get; set; }

    private void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
    {
        var textBox = (TextBox)sender;

        //int lineCount = textBox.LineCount; // (UWP/WinUI 3 only - not available on all platforms)
        if (textBox.Text == null)
        {
            this.CountLines = 1;
        }
        else
        {
            CountLines = textBox.Text.Split('\r').Length;
        }

        Debug.WriteLine($"[Editor] lines {CountLines}");

        var lineCount = CountLines;

        // Fallback: Count newlines, but this won't account for word wrap
        // int lineCount = textBox.Text.Split('\n').Length;

        // Calculate required height (estimate line height * lines)
        double lineHeight = textBox.FontSize * 1.4; // 1.4 fudge for line spacing/padding
        int maxLines = 5;
        int minLines = 1;

        // Clamp the line count
        lineCount = Math.Max(minLines, Math.Min(lineCount, maxLines));

        if (VirtualView is ISmartNative smart)
        {
            smart.NeededHeight = lineHeight * lineCount;
        } 
    }

 


}

