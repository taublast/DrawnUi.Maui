# SkiaShape

SkiaShape is a versatile control for rendering various geometric shapes in DrawnUi.Maui. Unlike traditional shape controls, SkiaShape offers high-performance rendering through SkiaSharp while supporting advanced features like custom paths, shadows, gradients, and content hosting.

## Basic Usage

SkiaShape supports various shape types through its `Type` property:

```xml
<DrawUi:SkiaShape 
    Type="Rectangle" 
    WidthRequest="200" 
    HeightRequest="100"
    BackgroundColor="Blue" 
    StrokeColor="White" 
    StrokeWidth="2" 
    CornerRadius="10" />
```

## Shape Types

SkiaShape supports the following shape types:

- **Rectangle**: A basic rectangle, optionally with rounded corners
- **Circle**: A perfect circle that maintains 1:1 aspect ratio
- **Ellipse**: An oval shape that can have different width and height
- **Path**: A custom shape defined by SVG path data
- **Polygon**: A shape defined by a collection of points
- **Line**: A series of connected line segments
- **Arc**: A circular arc segment

## Common Properties

### Visual Properties

| Property | Type | Description |
|----------|------|-------------|
| `BackgroundColor` | Color | Fill color of the shape |
| `StrokeColor` | Color | Outline color of the shape |
| `StrokeWidth` | float | Width of the outline stroke |
| `CornerRadius` | float | Rounded corner radius for rectangles |
| `StrokeCap` | StrokeCap | End cap style for lines (Round, Butt, Square) |
| `StrokePath` | string | Dash pattern for creating dashed lines |
| `StrokeBlendMode` | BlendMode | Controls how strokes blend with underlying content |
| `ClipBackgroundColor` | bool | If true, creates a "hollow" shape with just shadows and strokes |

### Shape-Specific Properties

| Property | Type | Description |
|----------|------|-------------|
| `PathData` | string | SVG path data for Path type shapes |
| `Points` | Collection\<SkiaPoint\> | Collection of points for Polygon or Line shapes |
| `SmoothPoints` | float | Level of smoothing for Polygon/Line shapes (0.0-1.0) |
| `StartAngle` | float | Starting angle for Arc shapes |
| `SweepAngle` | float | Sweep angle for Arc shapes |

## Advanced Features

### Shadow Effects

SkiaShape supports multiple shadows through the `Shadows` collection property:

```xml
<DrawUi:SkiaShape 
    Type="Rectangle" 
    BackgroundColor="White" 
    CornerRadius="20">
    <DrawUi:SkiaShape.Shadows>
        <DrawUi:SkiaShadow 
            Color="#80000000" 
            BlurRadius="10" 
            Offset="0,4" />
    </DrawUi:SkiaShape.Shadows>
</DrawUi:SkiaShape>
```

### Gradients

SkiaShape supports gradient fills via the `BackgroundGradient` and `StrokeGradient` properties:

```xml
<DrawUi:SkiaShape Type="Rectangle">
    <DrawUi:SkiaShape.BackgroundGradient>
        <DrawUi:SkiaGradient 
            Type="Linear" 
            StartColor="Red" 
            EndColor="Blue" 
            StartPoint="0,0" 
            EndPoint="1,1" />
    </DrawUi:SkiaShape.BackgroundGradient>
</DrawUi:SkiaShape>
```

### Custom Paths

For complex shapes, you can use SVG path data:

```xml
<DrawUi:SkiaShape 
    Type="Path" 
    PathData="M0,0L15.825,8.0 31.65,15.99 15.82,23.99 0,32 0,15.99z" 
    BackgroundColor="Red" />
```

The PathData property follows standard SVG path notation:
- M: Move to (absolute)
- m: Move to (relative)
- L: Line to (absolute)
- l: Line to (relative)
- H/h: Horizontal line
- V/v: Vertical line
- C/c: Cubic bezier curve
- S/s: Smooth cubic bezier
- Q/q: Quadratic bezier curve
- T/t: Smooth quadratic bezier
- A/a: Elliptical arc
- Z/z: Close path

### As a Content Container

SkiaShape can function as a container, clipping child elements to its shape boundaries:

```xml
<DrawUi:SkiaShape 
    Type="Circle" 
    BackgroundColor="Green" 
    WidthRequest="200" 
    HeightRequest="200">
    <DrawUi:SkiaImage 
        Source="background.jpg" 
        VerticalOptions="Fill" 
        HorizontalOptions="Fill" />
    <DrawUi:SkiaLabel 
        Text="Circular Content" 
        TextColor="White" 
        HorizontalOptions="Center" 
        VerticalOptions="Center" />
</DrawUi:SkiaShape>
```

The `LayoutChildren` property controls how children are arranged (Absolute, Column, Row, Grid).

## Creating Polygons

For polygon shapes, you can define points in various ways:

### Using SkiaPoint Collection

```xml
<DrawUi:SkiaShape 
    Type="Polygon" 
    BackgroundColor="Purple">
    <DrawUi:SkiaShape.Points>
        <DrawUi:SkiaPoint X="0" Y="0" />
        <DrawUi:SkiaPoint X="100" Y="0" />
        <DrawUi:SkiaPoint X="100" Y="100" />
        <DrawUi:SkiaPoint X="0" Y="100" />
    </DrawUi:SkiaShape.Points>
</DrawUi:SkiaShape>
```

### Using Relative Coordinates

You can define points using relative coordinates (0.0-1.0) that automatically scale to the shape's dimensions:

```xml
<DrawUi:SkiaShape 
    Type="Polygon" 
    BackgroundColor="CornflowerBlue">
    <DrawUi:SkiaShape.Points>
        <DrawUi:SkiaPoint X="0.0" Y="0.8" />
        <DrawUi:SkiaPoint X="0.0" Y="0.7" />
        <DrawUi:SkiaPoint X="1.0" Y="0.2" />
        <DrawUi:SkiaPoint X="1.0" Y="0.3" />
    </DrawUi:SkiaShape.Points>
</DrawUi:SkiaShape>
```

### Using String Definition

You can also use a converter for inline point definitions:

```xml
<DrawUi:SkiaShape 
    Type="Polygon" 
    BackgroundColor="Purple"
    Points="0,0 100,0 100,100 0,100" />
```

### Predefined Shapes

SkiaShape provides predefined point collections for common shapes:

```xml
<DrawUi:SkiaShape 
    Type="Polygon" 
    BackgroundColor="Yellow"
    Points="{x:Static DrawUi:SkiaShape.PolygonStar}" />
```

### Smooth Curves

For smoother, curved polygons, adjust the `SmoothPoints` property (0.0-1.0):

```xml
<DrawUi:SkiaShape 
    Type="Polygon" 
    BackgroundColor="#220000FF" 
    SmoothPoints="0.9"
    Points="0.0,0.8 0.0,0.7 1.0,0.2 1.0,0.3" />
```

A value of 0 creates sharp corners, while a value of 1.0 creates maximally smooth curves.

## Creating Lines

Lines can be created using the same point collection approach:

```xml
<DrawUi:SkiaShape 
    Type="Line" 
    StrokeColor="Black" 
    StrokeWidth="2"
    Points="0,0 50,50 100,0 150,50" />
```

Customize line appearance with:
- `StrokeCap`: Controls how line ends appear
- `StrokePath`: Define dash patterns ("5,5" creates 5px dashes with 5px gaps)

## Practical Examples

### Card with Shadow

```xml
<DrawUi:SkiaShape 
    Type="Rectangle" 
    BackgroundColor="White" 
    CornerRadius="12" 
    Padding="16"
    WidthRequest="300" 
    HeightRequest="150"
    LayoutChildren="Column">
    
    <DrawUi:SkiaShape.Shadows>
        <DrawUi:SkiaShadow 
            Color="#22000000" 
            BlurRadius="20" 
            Offset="0,4" />
    </DrawUi:SkiaShape.Shadows>
    
    <DrawUi:SkiaLabel 
        Text="Card Title" 
        FontSize="18" 
        FontWeight="Bold" />
    
    <DrawUi:SkiaLabel 
        Text="This is a card with rounded corners and a shadow effect. SkiaShape makes it easy to create modern UI components." 
        TextColor="#666666" 
        Margin="0,10,0,0" />
</DrawUi:SkiaShape>
```

### Progress Indicator

```xml
<DrawUi:SkiaShape 
    Type="Arc" 
    StrokeColor="#EEEEEE" 
    StrokeWidth="10" 
    BackgroundColor="Transparent"
    StartAngle="0" 
    SweepAngle="360" 
    WidthRequest="100" 
    HeightRequest="100">
    
    <DrawUi:SkiaShape 
        Type="Arc" 
        StrokeColor="Blue" 
        StrokeWidth="10" 
        BackgroundColor="Transparent"
        StartAngle="0" 
        SweepAngle="{Binding Progress}" 
        WidthRequest="100" 
        HeightRequest="100" />
    
    <DrawUi:SkiaLabel 
        Text="{Binding ProgressText}" 
        HorizontalOptions="Center" 
        VerticalOptions="Center" />
</DrawUi:SkiaShape>
```

### Custom Button

```xml
<DrawUi:SkiaShape 
    Type="Path" 
    PathData="M10,0 L90,0 C95,0 100,5 100,10 L100,40 C100,45 95,50 90,50 L10,50 C5,50 0,45 0,40 L0,10 C0,5 5,0 10,0 Z" 
    BackgroundColor="Blue" 
    WidthRequest="100" 
    HeightRequest="50">
    
    <DrawUi:SkiaShape.GestureRecognizers>
        <TapGestureRecognizer Command="{Binding ButtonCommand}" />
    </DrawUi:SkiaShape.GestureRecognizers>
    
    <DrawUi:SkiaLabel 
        Text="SUBMIT" 
        TextColor="White" 
        FontWeight="Bold"
        HorizontalOptions="Center" 
        VerticalOptions="Center" />
</DrawUi:SkiaShape>
```

## Performance Considerations

- For static shapes, set `Cache="Image"` to render once and cache as bitmap
- For frequently animated shapes, use `Cache="Operations"` for best performance
- Avoid excessive shadows or complex paths in performance-critical UI
- For very complex paths, pre-process SVG data when possible rather than computing at runtime

## Platform Specific Notes

SkiaShape renders consistently across all platforms supported by MAUI, ensuring that your UI maintains the same appearance on Android, iOS, Windows, and macOS.