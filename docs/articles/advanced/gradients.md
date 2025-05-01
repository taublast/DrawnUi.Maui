# Using Gradients in DrawnUi.Maui

DrawnUi.Maui provides powerful gradient support for shapes, text, and images, enabling visually rich and modern UI designs. This article covers the types of gradients available, how to apply them, and practical examples for common scenarios.

## Gradient Types

DrawnUi.Maui supports several gradient types:

- **Linear Gradient**: Colors transition along a straight line.
- **Radial Gradient**: Colors radiate outward from a center point.
- **Sweep Gradient**: Colors sweep around a center point in a circular fashion.

## Applying Gradients to Shapes

You can apply gradients to the background or stroke of any `SkiaShape` using the `BackgroundGradient` and `StrokeGradient` properties.

### Linear Gradient Example

```xml
<DrawUi:SkiaShape Type="Rectangle" CornerRadius="16" WidthRequest="200" HeightRequest="100">
    <DrawUi:SkiaShape.BackgroundGradient>
        <DrawUi:SkiaGradient 
            Type="Linear" 
            StartColor="#FF6A00" 
            EndColor="#FFD800" 
            StartPoint="0,0" 
            EndPoint="1,1" />
    </DrawUi:SkiaShape.BackgroundGradient>
</DrawUi:SkiaShape>
```

### Radial Gradient Example

```xml
<DrawUi:SkiaShape Type="Circle" WidthRequest="120" HeightRequest="120">
    <DrawUi:SkiaShape.BackgroundGradient>
        <DrawUi:SkiaGradient 
            Type="Radial" 
            StartColor="#00C3FF" 
            EndColor="#FFFF1C" 
            Center="0.5,0.5" 
            Radius="0.5" />
    </DrawUi:SkiaShape.BackgroundGradient>
</DrawUi:SkiaShape>
```

### Sweep Gradient Example

```xml
<DrawUi:SkiaShape Type="Ellipse" WidthRequest="180" HeightRequest="100">
    <DrawUi:SkiaShape.BackgroundGradient>
        <DrawUi:SkiaGradient 
            Type="Sweep" 
            StartColor="#FF0080" 
            EndColor="#7928CA" 
            Center="0.5,0.5" />
    </DrawUi:SkiaShape.BackgroundGradient>
</DrawUi:SkiaShape>
```

### Multi-Stop Gradients

You can define gradients with multiple color stops:

```xml
<DrawUi:SkiaShape Type="Rectangle" WidthRequest="220" HeightRequest="60">
    <DrawUi:SkiaShape.BackgroundGradient>
        <DrawUi:SkiaGradient Type="Linear" StartPoint="0,0" EndPoint="1,0">
            <DrawUi:SkiaGradient.Stops>
                <DrawUi:GradientStop Color="#FF6A00" Offset="0.0" />
                <DrawUi:GradientStop Color="#FFD800" Offset="0.5" />
                <DrawUi:GradientStop Color="#00FFB4" Offset="1.0" />
            </DrawUi:SkiaGradient.Stops>
        </DrawUi:SkiaGradient>
    </DrawUi:SkiaShape.BackgroundGradient>
</DrawUi:SkiaShape>
```

## Applying Gradients to Text

You can apply gradients to text using the `FillGradient` property on `SkiaLabel`:

```xml
<DrawUi:SkiaLabel 
    Text="Gradient Text" 
    FontSize="32" 
    FillGradient="{StaticResource MyGradient}" />
```

Or define inline:

```xml
<DrawUi:SkiaLabel Text="Sunset" FontSize="40">
    <DrawUi:SkiaLabel.FillGradient>
        <DrawUi:SkiaGradient Type="Linear" StartColor="#FF6A00" EndColor="#FFD800" StartPoint="0,0" EndPoint="1,0" />
    </DrawUi:SkiaLabel.FillGradient>
</DrawUi:SkiaLabel>
```

## Applying Gradients to Images

You can overlay gradients on images using the `UseGradient`, `StartColor`, and `EndColor` properties on `SkiaImage`:

```xml
<DrawUi:SkiaImage 
    Source="photo.jpg" 
    UseGradient="True" 
    StartColor="#00000000" 
    EndColor="#CC000000" />
```

This creates a fade effect from transparent to black over the image.

## Defining Gradients as Resources

For reuse, define gradients as resources:

```xml
<ContentPage.Resources>
    <DrawUi:SkiaGradient x:Key="MyGradient" Type="Linear" StartColor="#FF6A00" EndColor="#FFD800" StartPoint="0,0" EndPoint="1,1" />
</ContentPage.Resources>
```

Then reference with:

```xml
<DrawUi:SkiaLabel Text="Reusable Gradient" FillGradient="{StaticResource MyGradient}" />
```

## C# Example: Creating a Gradient in Code

```csharp
var gradient = new SkiaGradient
{
    Type = SkiaGradientType.Linear,
    StartColor = Colors.Red,
    EndColor = Colors.Yellow,
    StartPoint = new Point(0, 0),
    EndPoint = new Point(1, 1)
};

myShape.BackgroundGradient = gradient;
```

## Tips and Best Practices

- Use gradients to add depth and visual interest to your UI.
- For performance, prefer simple gradients or reuse gradient resources.
- Gradients can be animated by changing their properties in code.
- Combine gradients with shadows for modern card and button designs.

## Summary

Gradients in DrawnUi.Maui are flexible and easy to use across shapes, text, and images. Use the provided properties and examples to create visually appealing, modern interfaces.
