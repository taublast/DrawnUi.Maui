# Interactive Card Gallery Tutorial

Ready for something **impressive**? Let's build an animated, interactive card gallery that showcases the true power of DrawnUI! You'll create smooth 60fps animations, beautiful visual effects, and responsive touch interactions - all with better performance than native controls.

> 💡 **Prerequisites**: Complete [Your First DrawnUI App](first-app.md) before starting this tutorial.

## What We're Building

An interactive card gallery featuring:
- 🎨 **Beautiful gradient cards** with shadows and glows
- ✨ **Smooth animations** (scale, rotation, color transitions)
- 👆 **Gesture interactions** (tap, pan, pinch)
- 🚀 **60fps performance** thanks to hardware acceleration
- 💫 **Visual effects** that would be complex in native MAUI

**This demonstrates DrawnUI's core advantages: pixel-perfect control, smooth animations, and superior performance.**

---

## Prerequisites

- **.NET 9** or later
- **MAUI 9.0.70+**
- **Visual Studio 2022** or **VS Code** with MAUI extension

## Step 1: Create & Setup Project

### Create New MAUI Project
```bash
dotnet new maui -n InteractiveCardGallery
cd InteractiveCardGallery
```

### Add DrawnUI Package
```bash
dotnet add package DrawnUi.Maui
```

### Initialize DrawnUI in MauiProgram.cs

Replace your `MauiProgram.cs` with this enhanced setup:

```csharp
using DrawnUi.Maui.Infrastructure;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseDrawnUi(new DrawnUiStartupSettings
            {
                UseDesktopKeyboard = true,
                DesktopWindow = new()
                {
                    Width = 400,
                    Height = 700
                }
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "FontText");
                fonts.AddFont("OpenSans-Semibold.ttf", "FontSemibold");
            });

        return builder.Build();
    }
}
```

> 💡 **Why this setup?** We're enabling desktop keyboard support and setting an optimal window size for testing. The DrawnUi startup settings give us better control over rendering performance.

---

## Step 2: Create the Interactive Card

Replace `MainPage.xaml` with our card gallery:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage x:Class="InteractiveCardGallery.MainPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:draw="http://schemas.appomobi.com/drawnUi/2023/draw"
             Title="Interactive Cards">

    <draw:Canvas
        RenderingMode="Accelerated"
        Gestures="Enabled"
        BackgroundColor="#f0f0f5"
        HorizontalOptions="Fill"
        VerticalOptions="Fill">

        <!-- Main Container -->
        <draw:SkiaStack
            Spacing="0"
            VerticalOptions="Fill">

            <!-- Title Section -->
            <draw:SkiaLayout Type="Column"
                             HorizontalOptions="Center"
                             Margin="16"
                             UseCache="Operations"
                             Spacing="8">

                <draw:SkiaLabel
                    Text="Interactive Cards"
                    FontSize="32"
                    FontAttributes="Bold"
                    TextColor="#2c3e50"
                    HorizontalOptions="Center" />

                <draw:SkiaLabel
                    Text="Tap and drag to interact!"
                    FontSize="16"
                    TextColor="#7f8c8d"
                    HorizontalOptions="Center" />
            </draw:SkiaLayout>

            <!-- Card Gallery -->
            <draw:SkiaScroll
                IgnoreWrongDirection="True"
                VerticalOptions="Fill"
                Orientation="Vertical"
                Spacing="20">

                <draw:SkiaStack Type="Column"
                                Tag="Cells"
                                Padding="0,5"
                                Spacing="0">

                    <!-- Card 1: Gradient Glow Card -->
                    <draw:SkiaLayer
                        Padding="20,8"
                        UseCache="Image">
                        <draw:SkiaShape
                            x:Name="Card1"
                            Type="Rectangle"
                            CornerRadius="20"
                            WidthRequest="300"
                            HeightRequest="180"
                            HorizontalOptions="Center"

                            ConsumeGestures="OnCardGestures">

                            <!-- Gradient Background -->
                            <draw:SkiaControl.FillGradient>
                                <draw:SkiaGradient
                                    Type="Linear"
                                    Angle="45">
                                    <draw:SkiaGradient.Colors>
                                        <Color>#667eea</Color>
                                        <Color>#764ba2</Color>
                                    </draw:SkiaGradient.Colors>
                                </draw:SkiaGradient>
                            </draw:SkiaControl.FillGradient>

                            <!-- Glow Effect -->
                            <draw:SkiaShape.VisualEffects>
                                <draw:DropShadowEffect
                                    Color="#667eea"
                                    Blur="3"
                                    X="1"
                                    Y="1" />
                            </draw:SkiaShape.VisualEffects>

                            <!-- Card Content -->
                            <draw:SkiaLayout Type="Column" Margin="24" Spacing="12">
                                <draw:SkiaRichLabel
                                    Text="🎨 Gradient Card"
                                    FontSize="20"
                                    FontAttributes="Bold"
                                    TextColor="White" />
                                <draw:SkiaLabel
                                    Text="Beautiful gradients with glow effects"
                                    FontSize="14"
                                    TextColor="#e8e8ff" />
                                <draw:SkiaLabel
                                    Text="Tap to animate!"
                                    FontSize="12"
                                    TextColor="#ccccff"
                                    Margin="0,8,0,0" />
                            </draw:SkiaLayout>
                        </draw:SkiaShape>
                    </draw:SkiaLayer>

                    <!-- Card 2: Interactive Gaming Card -->
                    <draw:SkiaLayer
                        Padding="20,8"
                        ZIndex="10"
                        x:Name="Pannable"
                        ConsumeGestures="OnCardGestures"
                        UseCache="Image">

                        <draw:SkiaShape
                            x:Name="Card2"
                            Type="Rectangle"
                            CornerRadius="20"
                            WidthRequest="300"
                            HeightRequest="180"
                            HorizontalOptions="Center">

                            <!-- Gaming Theme Background -->
                            <draw:SkiaShape.FillGradient>
                                <draw:SkiaGradient
                                    StartXRatio="0.85"
                                    StartYRatio="0.25"
                                    Type="Circular">
                                    <draw:SkiaGradient.Colors>
                                        <Color>#ff6b6b</Color>
                                        <Color>#c44569</Color>
                                    </draw:SkiaGradient.Colors>
                                </draw:SkiaGradient>
                            </draw:SkiaShape.FillGradient>

                            <!-- Gaming Glow -->
                            <draw:SkiaShape.VisualEffects>
                                <draw:DropShadowEffect
                                    Color="#ff6b6b"
                                    Blur="5"
                                    X="0"
                                    Y="0" />
                            </draw:SkiaShape.VisualEffects>

                            <!-- Gaming Content -->
                            <draw:SkiaLayout Type="Column" Margin="24" Spacing="12">
                                <draw:SkiaRichLabel
                                    Text="🎮 Gaming Card"
                                    FontSize="20"
                                    FontAttributes="Bold"
                                    TextColor="White" />
                                <draw:SkiaLabel
                                    Text="Drag me around! Smooth movement"
                                    FontSize="14"
                                    TextColor="#ffe8e8" />
                                <draw:SkiaLabel
                                    Text="Pan gesture enabled"
                                    FontSize="12"
                                    TextColor="#ffcccc"
                                    Margin="0,8,0,0" />
                            </draw:SkiaLayout>
                        </draw:SkiaShape>
                    </draw:SkiaLayer>

                    <!-- Card 3: Data Visualization Card -->
                    <draw:SkiaLayer
                        Padding="20,8"
                        UseCache="Image">

                        <draw:SkiaShape
                        Type="Rectangle"
                        CornerRadius="20"
                        WidthRequest="300"
                        HeightRequest="200"
                        HorizontalOptions="Center"
                        ConsumeGestures="OnCardGestures">

                        <!-- Tech Background -->
                        <draw:SkiaShape.FillGradient>
                            <draw:SkiaGradient
                                Type="Linear"
                                Angle="135">
                                <draw:SkiaGradient.Colors>
                                    <Color>#004400</Color>
                                    <Color>#009900</Color>
                                </draw:SkiaGradient.Colors>
                            </draw:SkiaGradient>
                        </draw:SkiaShape.FillGradient>

                        <!-- Tech Glow -->
                        <draw:SkiaShape.VisualEffects>
                            <draw:DropShadowEffect
                                Color="#99ff0000"
                                Blur="5"
                                X="0"
                                Y="0" />
                        </draw:SkiaShape.VisualEffects>

                        <!-- Progress Bars -->
                        <draw:SkiaLayout Type="Column" Margin="24" Spacing="16">
                                <draw:SkiaRichLabel
                                Text="📊 Data Card"
                                FontSize="20"
                                FontAttributes="Bold"
                                TextColor="White" />

                            <!-- Progress Bars -->
                            <draw:SkiaLayout Type="Column" Spacing="8">
                                <draw:SkiaLabel Text="Performance: 87%" FontSize="12" TextColor="#e8f4ff" />
                                <draw:SkiaShape
                                    Type="Rectangle"
                                    CornerRadius="4"
                                    WidthRequest="200"
                                    HeightRequest="6"
                                    BackgroundColor="#50ffffff">
                                    <draw:SkiaShape Type="Rectangle"
                                                    CornerRadius="4"
                                                    WidthRequest="174"
                                                    HeightRequest="6"
                                                    BackgroundColor="White"
                                                    HorizontalOptions="Start" />
                                </draw:SkiaShape>

                                <draw:SkiaLabel Text="Memory: 64%" FontSize="12" TextColor="#e8f4ff" />
                                <draw:SkiaShape
                                    Type="Rectangle"
                                    CornerRadius="4"
                                    WidthRequest="200"
                                    HeightRequest="6"
                                    BackgroundColor="#50ffffff">
                                    <draw:SkiaShape
                                        Type="Rectangle"
                                        CornerRadius="4"
                                        WidthRequest="128"
                                        HeightRequest="6"
                                        BackgroundColor="White"
                                        HorizontalOptions="Start" />
                                </draw:SkiaShape>
                            </draw:SkiaLayout>
                        </draw:SkiaLayout>
                    </draw:SkiaShape>
                    </draw:SkiaLayer>

                </draw:SkiaStack>
            </draw:SkiaScroll>

        </draw:SkiaStack>
    </draw:Canvas>    
    
</ContentPage>
```

> 🎯 **Key Features Demonstrated:**
> - **Gradients**: Linear, radial, and angled gradients
> - **Visual Effects**: Drop shadows with custom colors and blur
> - **Caching**: Different strategies for optimal performance  
> - **Layouts**: Nested layouts with proper spacing
> - **Gestures**: Tap and pan gesture handling

---

## Step 3: Add Interactive Behaviors

Replace `MainPage.xaml.cs` with the interaction logic:

```csharp
using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Infrastructure;

namespace InteractiveCardGallery;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception e)
        {
            Super.DisplayException(this, e);
        }
    }

        private void OnCardGestures(object sender, SkiaGesturesInfo e)
    {
        if (sender is SkiaControl control)
        {
            if (e.Args.Type == TouchActionResult.Tapped)
            {
                e.Consumed = true; //could consume

                Task.Run(async () =>
                {
                    // Color pulse effect
                    if (control is SkiaShape shape && shape.FillGradient is SkiaGradient gradient)
                    {
                        var originalStart = gradient.Colors[0];
                        var originalEnd = gradient.Colors[1];
                        var lighter = 1.5;

                        // Brighten colors
                        var gradientStartColor = Color.FromRgba(
                            Math.Min(255, originalStart.Red * lighter),
                            Math.Min(255, originalStart.Green * lighter),
                            Math.Min(255, originalStart.Blue * lighter),
                            originalStart.Alpha);

                        var gradientEndColor = Color.FromRgba(
                            Math.Min(255, originalEnd.Red * lighter),
                            Math.Min(255, originalEnd.Green * lighter),
                            Math.Min(255, originalEnd.Blue * lighter),
                            originalEnd.Alpha);

                        gradient.Colors = new List<Color>() { gradientStartColor, gradientEndColor };

                        // Restore original colors
                        await Task.Delay(200);
                        gradient.Colors = new List<Color>() { originalStart, originalEnd };
                    }
                });

                Task.Run(async () =>
                {
                    // Smooth scale animation with bounce effect
                    control.ScaleToAsync(1.1, 1.1, 150, Easing.CubicOut);
                    await Task.Delay(100);
                    control.ScaleToAsync(1.0, 1.0, 200, Easing.BounceOut);

                    // Rotate animation for fun
                    control.RotateToAsync(control.Rotation + 5, 200, Easing.SpringOut);
                    await Task.Delay(150);
                    control.RotateToAsync(0, 300, Easing.SpringOut);
                });

            }

            if (sender == Pannable)
            {
                // Smooth drag following with momentum
                if (e.Args.Type == TouchActionResult.Panning)
                {
                    e.Consumed = true;

                    control.TranslationX += e.Args.Event.Distance.Delta.X / control.RenderingScale;
                    control.TranslationY += e.Args.Event.Distance.Delta.Y / control.RenderingScale;

                    // Add subtle rotation based on pan direction
                    var deltaX = e.Args.Event.Distance.Total.X / control.RenderingScale;
                    var rotationAmount = deltaX * 0.1;
                    control.Rotation = Math.Max(-15, Math.Min(15, rotationAmount));
                }
                else if (e.Args.Type == TouchActionResult.Up)
                {
                    // Snap back to original position
                    control.TranslateToAsync(0, 0, 100, Easing.SpringOut);
                    control.RotateToAsync(0, 75, Easing.SpringOut);
                }
            }
        }
    }
}
```

> ⚡ **Animation Highlights:**
> - **Scale bounce**: Professional elastic scaling effect
> - **Color pulsing**: Dynamic color changes during interaction
> - **Drag with momentum**: Smooth real-time position updates
> - **Auto snap-back**: Spring animations return to rest position

---

## Step 4: Run Your Impressive App!

Build and run the app:

```bash
dotnet build
dotnet run
```

### What You'll Experience:

1. **🎨 Beautiful Cards**: Gradient backgrounds with glowing shadows
2. **✨ Smooth Animations**: 60fps interactions that feel native
3. **👆 Responsive Touch**: Immediate feedback to every gesture
4. **🚀 Superior Performance**: Faster than equivalent native MAUI controls

---


## Troubleshooting

### Common Issues:

**App won't start:**
- Ensure you called `.UseDrawnUi()` in MauiProgram.cs
- Verify .NET 9 is installed
- Check that MAUI workload is installed: `dotnet workload install maui`

**Cards not animating:**
- Verify gestures are enabled on the Canvas
- Check that controls have proper `x:Name` attributes
- Ensure animations are awaited properly

**Performance issues:**
- Use appropriate `UseCache` for your content
- Avoid nested animations during heavy interactions
- Profile with platform tools to identify bottlenecks

**Visual effects not showing:**
- Check that hardware acceleration is enabled
- Verify effect properties (blur, color) are valid
- Test on different devices for hardware limitations

---

## 🎉 Congratulations!

You've built a **genuinely impressive** first DrawnUI app that demonstrates:
- ✅ **Beautiful, pixel-perfect UI**
- ✅ **Smooth 60fps animations** 
- ✅ **Professional visual effects**
- ✅ **Superior performance**
- ✅ **Cross-platform consistency**

**This isn't just "Hello World" - this is what DrawnUI enables you to build!**

Ready to create your next amazing app? The DrawnUI community is excited to see what you build! 🚀