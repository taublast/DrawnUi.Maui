# Getting Started with DrawnUi

This guide will help you get started with DrawnUi in your .NET MAUI application.

## Installation

### 1. Add the NuGet Package

Install the DrawnUi NuGet package in your .NET MAUI project:

```bash
dotnet add package AppoMobi.Maui.DrawnUi
```

or fork the DrawnUi repo and referece the main project directly.

> **Note**: There are some additional packages supporting optional features like games, camera, maps etc, they must be refereced separately.

To make everything compile from first attempt You might also need at least the following MAUI setup inside your csproj:

```
	<PropertyGroup>
        <WindowsPackageType>MSIX</WindowsPackageType>
		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">15.0</SupportedOSPlatformVersion>
		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">15.2</SupportedOSPlatformVersion>
		<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>
        <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.19041.0</SupportedOSPlatformVersion>
        <TargetPlatformMinVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.19041.0</TargetPlatformMinVersion>
	</PropertyGroup>
    
    <ItemGroup>
        <PackageReference Include="Microsoft.Maui.Controls" Version="9.0.30" />
        <PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="9.0.30" />
    </ItemGroup>

```

For Windows to overcome an existing restriction in SKiaSharp you would need to enable MSIX packaging for your Windows project. This limitation will be resolved.


### 2. Initialize in Your MAUI App

Update your `MauiProgram.cs` file to initialize draw:

```csharp
using DrawnUi.Draw;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseDrawnUi() // <---- Add this line
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
```

### Add Namespace to XAML

Add the DrawnUi namespace to your XAML files:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:draw="http://schemas.appomobi.com/drawnUi/2023/draw"
             x:Class="YourNamespace.YourPage">
    <!-- Page content -->
</ContentPage>
```

### Using DrawnUi Controls

Now you can add DrawnUi controls to your page. You have two main options:

#### Option 1: Use Canvas inside a regular ContentPage

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:draw="http://schemas.appomobi.com/drawnUi/2023/draw"
             x:Class="YourNamespace.YourPage">

    <draw:Canvas HorizontalOptions="Fill" VerticalOptions="Fill">
        <draw:SkiaLayout Type="Column" Spacing="16" Padding="32">
            <draw:SkiaLabel
                Text="Hello DrawnUi!"
                FontSize="24"
                HorizontalOptions="Center"
                VerticalOptions="Center" />

            <draw:SkiaButton
                Text="Click Me"
                WidthRequest="120"
                HeightRequest="40"
                CornerRadius="8"
                BackgroundColor="Blue"
                TextColor="White"
                VerticalOptions="Center"
                HorizontalOptions="Center"
                Clicked="OnButtonClicked" />

        </draw:SkiaLayout>
    </draw:Canvas>
</ContentPage>
```

#### Option 2: Use DrawnUiBasePage (for keyboard support)

```xml
<draw:DrawnUiBasePage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                      xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                      xmlns:draw="http://schemas.appomobi.com/drawnUi/2023/draw"
                      x:Class="YourNamespace.YourPage">

    <draw:Canvas 
        RenderingMode="Accelerated"
        Gestures="Lock"
        HorizontalOptions="Fill" VerticalOptions="Fill">
        <draw:SkiaLayout Type="Column" Spacing="16" Padding="32">
            <draw:SkiaLabel
                Text="Hello DrawnUi!"
                FontSize="24"
                HorizontalOptions="Center"
                VerticalOptions="Center" />

            <draw:SkiaButton
                Text="Click Me"
                WidthRequest="120"
                HeightRequest="40"
                CornerRadius="8"
                BackgroundColor="Blue"
                TextColor="White"
                VerticalOptions="Center"
                HorizontalOptions="Center"
                Clicked="OnButtonClicked" />
        </draw:SkiaLayout>
    </draw:Canvas>
</draw:DrawnUiBasePage>
```

### Setup Canvas

If you indend to process gestures inside your canvas setup the `Gestures` property accordingly
If you would have animated content or use shaders set `RenderingMode` to `Accelerated`. Otherwise leave it as it is to use the default lightweight `Software` mode, it is still perfect for rendering static content.

### Handling Events

Handle control events in your code-behind:

```csharp
private void OnButtonClicked(SkiaButton sender, SkiaGesturesParameters e)
{
    // Handle button click
    DisplayAlert("DrawnUi", "Button clicked!", "OK");
}
```

> **Important**: DrawnUi button events use `Action<SkiaButton, SkiaGesturesParameters>` instead of the standard EventHandler pattern. The first parameter is the specific control type (SkiaButton), and the second contains gesture information.

## Using Platform-Specific Styles

DrawnUi controls support platform-specific styling:

```xml
<draw:SkiaButton
    Text="Platform Style"
    ControlStyle="Platform"
    WidthRequest="150"
    HeightRequest="40" />
    
<draw:SkiaSwitch
    ControlStyle="Platform"
    IsToggled="true"
    Margin="0,20,0,0" />
```

## Next Steps

- Explore the [Controls documentation](controls/index.md) to learn about available controls
- See [Platform-Specific Styling](advanced/platform-styling.md) for more styling options
- Check out the [Sample Applications](samples.md) for complete examples