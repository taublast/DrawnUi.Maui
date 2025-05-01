# Getting Started with DrawnUi

This guide will help you get started with DrawnUi in your .NET MAUI application.

## Installation

### 1. Add the NuGet Package

Install the DrawnUi NuGet package in your .NET MAUI project:

```bash
dotnet add package AppoMobi.Maui.DrawnUi
```

You might also need at least the following MAUI setup inside your csproj:

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


### 2. Initialize in Your MAUI App

Update your `MauiProgram.cs` file to initialize DrawnUi:

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

### using DrawnUi Controls

Now you can add DrawnUi controls to your page:

```xml
<draw:DrawnUiBasePage>
    <draw:SkiaLayout>
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
            Margin="0,50,0,0"
            Clicked="OnButtonClicked" />
    </draw:SkiaLayout>
</draw:DrawnUiBasePage>
```

### Handling Events

Handle control events in your code-behind:

```csharp
private void OnButtonClicked(object sender, SkiaGesturesParameters e)
{
    // Handle button click
}
```

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