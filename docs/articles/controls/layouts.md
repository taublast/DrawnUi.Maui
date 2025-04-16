# Layout Controls

DrawnUi.Maui provides a powerful and flexible layout system for arranging controls with high performance. The system is similar to MAUI's native layout system but optimized for direct rendering with SkiaSharp.

## Core Layout Types

DrawnUi.Maui offers several core layout types:

### SkiaLayout

The base layout control for measurement, arrangement, and rendering of child elements. It supports different layout strategies via the `LayoutType` property:

- Managing child controls
- Performance optimizations (see below)

```xml
<DrawUi:SkiaLayout
    LayoutType="Absolute"
    WidthRequest="400"
    HeightRequest="300">
    <!-- Child controls here -->
</DrawUi:SkiaLayout>
```

### ContentLayout

A specialized layout for hosting a single content element with optional overlays.

```xml
<DrawUi:ContentLayout>
    <DrawUi:SkiaImage Source="background.png" />
</DrawUi:ContentLayout>
```

## Layout Types

The `LayoutType` property on `SkiaLayout` supports:

- **Absolute**: Free positioning using explicit coordinates
- **Grid**: Row and column-based layout
- **Column**: Vertical stacking (like VStack)
- **Row**: Horizontal stacking (like HStack)
- **Wrap**: Items wrap to new lines when space runs out

```xml
<DrawUi:SkiaLayout LayoutType="Wrap" Spacing="5,5">
    <DrawUi:SkiaLabel Text="Item 1" />
    <DrawUi:SkiaLabel Text="Item 2" />
    <!-- More items -->
</DrawUi:SkiaLayout>
```

## Specialized Layout Controls

### InfiniteLayout

Provides infinite scrolling in horizontal, vertical, or both directions.

```xml
<DrawUi:InfiniteLayout ScrollOrientation="Both">
    <DrawUi:SkiaImage Source="large_map.png" />
</DrawUi:InfiniteLayout>
```

### SnappingLayout

Supports snap points for controlled scrolling, ideal for carousels or paginated interfaces.

```xml
<DrawUi:SnappingLayout Orientation="Horizontal">
    <DrawUi:SkiaLabel Text="Page 1" />
    <DrawUi:SkiaLabel Text="Page 2" />
    <DrawUi:SkiaLabel Text="Page 3" />
</DrawUi:SnappingLayout>
```

## Performance Optimization

The layout system is designed for performance:

### Measurement Strategies

Control how and when items are measured using the `MeasureItemsStrategy` property:

- **MeasureFirst**: Measures all items before rendering (good for static content)
- **MeasureAll**: Continuously measures all items (for dynamic content)
- **MeasureVisible**: Only measures visible items (most efficient for large collections)

```xml
<DrawUi:SkiaLayout
    MeasureItemsStrategy="MeasureVisible">
    <!-- Child controls -->
</DrawUi:SkiaLayout>
```

> **Note:** Data templating and collection binding (e.g., `ItemsSource`, `ItemTemplate`) are not currently available as direct properties in SkiaLayout. If you need dynamic content, you must add/remove child controls programmatically.

### Virtualization

Virtualization is handled internally by the measurement strategy. When using `MeasureVisible`, only visible items are measured and rendered, improving performance for large collections.

## Example: Creating a Grid Layout

```xml
<DrawUi:SkiaLayout LayoutType="Grid" 
    ColumnDefinitions="Auto,*,100" 
    RowDefinitions="Auto,*,50">
    <!-- Header spanning all columns -->
    <DrawUi:SkiaLabel 
        Text="Grid Header" 
        Column="0" 
        ColumnSpan="3"
        Row="0" 
        HorizontalOptions="Center" />
    <!-- Sidebar -->
    <DrawUi:SkiaLayout 
        LayoutType="Column" 
        Column="0" 
        Row="1" 
        RowSpan="2"
        BackgroundColor="LightGray"
        Padding="10">
        <DrawUi:SkiaLabel Text="Menu Item 1" />
        <DrawUi:SkiaLabel Text="Menu Item 2" />
        <DrawUi:SkiaLabel Text="Menu Item 3" />
    </DrawUi:SkiaLayout>
    <!-- Main content -->
    <DrawUi:ContentLayout 
        Column="1" 
        Row="1">
        <DrawUi:SkiaLabel 
            Text="Main Content Area" 
            HorizontalOptions="Center" 
            VerticalOptions="Center" />
    </DrawUi:ContentLayout>
    <!-- Right panel -->
    <DrawUi:SkiaLayout 
        LayoutType="Column" 
        Column="2" 
        Row="1"
        BackgroundColor="LightBlue"
        Padding="5">
        <DrawUi:SkiaLabel Text="Panel Info" />
    </DrawUi:SkiaLayout>
    <!-- Footer spanning columns 1-2 -->
    <DrawUi:SkiaLabel 
        Text="Footer" 
        Column="1" 
        ColumnSpan="2"
        Row="2" 
        HorizontalOptions="Center"
        VerticalOptions="Center" />
</DrawUi:SkiaLayout>
```

All grid functionality is handled by SkiaLayout with `LayoutType="Grid"`.

## Under the Hood

The layout system is built on top of the `SkiaControl` base class. Layout controls extend this for child management, measurement, and arrangement. Internally, structures like `LayoutStructure` and `GridStructure` efficiently track and manage layout information.

### Caching

Caching options help balance CPU and memory usage:

- **None**: No caching, recalculated every frame
- **Operations**: Caches drawing commands
- **Image**: Caches as bitmap (more memory, less CPU)
- **GPU**: Uses hardware acceleration where available