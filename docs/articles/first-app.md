# Your First DrawnUi App

This quickstart guide will help you create your first DrawnUi.Maui application from scratch.

## Prerequisites
- .NET 8 or later
- Visual Studio 2022+ (with MAUI workload) or VS Code

## 1. Create a New MAUI Project

```bash
dotnet new maui -n MyDrawnUiApp
cd MyDrawnUiApp
```

## 2. Add DrawnUi to Your Project

```bash
dotnet add package DrawnUi
```

## 3. Add a DrawnUi Canvas to MainPage

Open `MainPage.xaml` and replace the content with:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:draw="clr-namespace:DrawnUi.Maui;assembly=DrawnUi.Maui"
             x:Class="MyDrawnUiApp.MainPage">
    <draw:Canvas HorizontalOptions="Fill" VerticalOptions="Fill">
        <draw:SkiaLayout LayoutType="Column" Padding="32" Spacing="24">
            <draw:SkiaLabel Text="Hello, DrawnUi!" FontSize="32" TextColor="Blue" />
            <draw:SkiaButton Text="Click Me" Tapped="OnButtonTapped" />
        </draw:SkiaLayout>
    </draw:Canvas>
</ContentPage>
```

## 4. Handle Button Tap in Code

In `MainPage.xaml.cs`:

```csharp
private void OnButtonTapped(object sender, EventArgs e)
{
    // Show a message or update UI
    DisplayAlert("DrawnUi", "Button tapped!", "OK");
}
```

## 5. Run Your App

Build and run your app on Windows, Android, iOS, or Mac.

## Next Steps
- Explore the [Controls documentation](controls/index.md)
- Try out [Samples](samples.md)
- Read about [Advanced features](advanced/index.md)

Welcome to the DrawnUi community!
