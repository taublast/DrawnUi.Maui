
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Brush = Microsoft.UI.Xaml.Media.Brush;
using TextChangedEventArgs = Microsoft.UI.Xaml.Controls.TextChangedEventArgs;

namespace DrawnUi.Controls;


public partial class MauiEntryHandler : EntryHandler
{
    static MauiEntryHandler()
    {
       
    }

    TextBox _control;

    protected override void ConnectHandler(TextBox platformView)
    {
        base.ConnectHandler(platformView);

        _control = platformView;

        //var transparentBrush = Colors.Transparent.ToPlatform();

        //var backgroundKeys = new[]
        //{
        //    "TextControlBackground",
        //    "TextControlBackgroundPointerOver",
        //    "TextControlBackgroundFocused",
        //    "TextControlBackgroundDisabled",
        //};

        //foreach (var key in backgroundKeys)
        //{
        //    platformView.Resources[key] = transparentBrush;
        //}

        //RefreshThemeResources(platformView);

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

    

    internal static void RefreshThemeResources(FrameworkElement nativeView)
    {
        var previous = nativeView.RequestedTheme;

        // Workaround for https://github.com/dotnet/maui/issues/7820
        nativeView.RequestedTheme = nativeView.ActualTheme switch
        {
            ElementTheme.Dark => ElementTheme.Light,
            _ => ElementTheme.Dark
        };

        nativeView.RequestedTheme = previous;
    }

    static void SetValueForAllKey(Microsoft.UI.Xaml.ResourceDictionary resources, IEnumerable<string> keys, object? value)
    {
        foreach (string key in keys)
        {
            resources[key] = value;
        }
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

