# Layout Controls

DrawnUi.Maui provides a powerful and flexible layout system for arranging controls with high performance. The system is similar to MAUI's native layout system but optimized for direct rendering with SkiaSharp.

## Core Layout Types

DrawnUi.Maui offers several core layout types:

### SkiaLayout

The base layout control for measurement, arrangement, and rendering of child elements. It supports different layout strategies via the `Type` property:

- Managing child controls
- Performance optimizations (see below)

```xml
<draw:SkiaLayout
    Type="Absolute"
    WidthRequest="400"
    HeightRequest="300">
    <!-- Child controls here -->
</draw:SkiaLayout>
```

## Layout Types

The `Type` property on `SkiaLayout` supports:

- **Absolute**: Free positioning, default. Think of it like a MAUI Grid with a single column and a single row.
- **Grid**: Row and column-based layout, classic MAUI Grid.
- **Column**: Vertical stacking (like MAUI VerticalStackLayout)
- **Row**: Horizontal stacking (like MAUI HorizontalStackLayout)
- **Wrap**: Items wrap to new lines when space runs out (similar to WPF Stackpanel)

```xml
<draw:SkiaLayout Type="Wrap" Spacing="5">
    <draw:SkiaLabel Text="Item 1" />
    <draw:SkiaLabel Text="Item 2" />
    <!-- More items -->
</draw:SkiaLayout>
```

> **Note**: The `Spacing` property takes a single double value that applies to both horizontal and vertical spacing between items.

## Specialized Layout Controls

### ContentLayout

A specialized layout for hosting a single content element is `ContentLayout`, `SkiaShape` is subclassing it to be able to contain a single child inside a `Content` property, instead of using `Children`.

### SnappingLayout

Supports snap points for controlled scrolling, ideal for carousels or paginated interfaces. `SkiaDrawer`, `SkiaCarousel` are deriving from it.

## Example: Creating a Grid Layout

```xml
<draw:SkiaLayout Type="Grid"
    ColumnDefinitions="Auto,*,100"
    RowDefinitions="Auto,*,50">
    <!-- Header spanning all columns -->
    <draw:SkiaLabel 
        Text="Grid Header" 
        Column="0" 
        ColumnSpan="3"
        Row="0" 
        HorizontalOptions="Center" />
    <!-- Sidebar -->
    <draw:SkiaLayout
        Type="Column"
        Column="0"
        Row="1"
        RowSpan="2"
        BackgroundColor="LightGray"
        Padding="10">
        <draw:SkiaLabel Text="Menu Item 1" />
        <draw:SkiaLabel Text="Menu Item 2" />
        <draw:SkiaLabel Text="Menu Item 3" />
    </draw:SkiaLayout>
    <!-- Main content -->
    <draw:ContentLayout 
        Column="1" 
        Row="1">
        <draw:SkiaLabel 
            Text="Main Content Area" 
            HorizontalOptions="Center" 
            VerticalOptions="Center" />
    </draw:ContentLayout>
    <!-- Right panel -->
    <draw:SkiaLayout
        Type="Column"
        Column="2"
        Row="1"
        BackgroundColor="LightBlue"
        Padding="5">
        <draw:SkiaLabel Text="Panel Info" />
    </draw:SkiaLayout>
    <!-- Footer spanning columns 1-2 -->
    <draw:SkiaLabel 
        Text="Footer" 
        Column="1" 
        ColumnSpan="2"
        Row="2" 
        HorizontalOptions="Center"
        VerticalOptions="Center" />
</draw:SkiaLayout>
```

All grid functionality is handled by SkiaLayout with `Type="Grid"`.

## Under the Hood

The layout system is built on top of the `SkiaControl` base class. Layout controls extend this for child management, measurement, and arrangement. Internally, structures like `LayoutStructure` and `GridStructure` efficiently track and manage layout information.

### Caching

Caching options help balance CPU and memory usage:

- **None**: No caching, recalculated every frame
- **Operations**: Caches drawing commands
- **Image**: Caches as bitmap (more memory, less CPU)
- **GPU**: Uses hardware acceleration where available