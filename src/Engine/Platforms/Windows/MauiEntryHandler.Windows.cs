using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using Brush = Microsoft.UI.Xaml.Media.Brush;
using TextChangedEventArgs = Microsoft.UI.Xaml.Controls.TextChangedEventArgs;

namespace DrawnUi.Maui.Controls;


public partial class MauiEntryHandler : EntryHandler
{

    TextBox _control;

    protected override void ConnectHandler(TextBox platformView)
    {
        base.ConnectHandler(platformView);

        _control = platformView;

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

    protected override void DisconnectHandler(TextBox platformView)
    {
        base.DisconnectHandler(platformView);

        _control = null;

        //platformView.EditorAction -= OnEditorAction;
        platformView.TextChanged -= OnTextChanged;
    }



    private void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
    {

    }



}