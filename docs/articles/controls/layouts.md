# Layout Controls

DrawnUi.Maui provides a powerful and flexible layout system that allows you to arrange controls in various ways while maintaining high performance. The layout system is designed to be similar to MAUI's native layout system but with optimizations specifically for direct rendering with SkiaSharp.

## Core Layout Types

DrawnUi.Maui offers several core layout types to handle different arrangement needs:

### SkiaLayout

The base layout control that handles the measurement, arrangement, and rendering of child elements with different layout strategies through its LayoutType property. It provides fundamental layout capabilities:

- Managing child controls
- Template recycling for improved performance
- Virtualization support for efficiently rendering large collections
- DataTemplate support for data-driven UIs

```xml
<DrawUi:SkiaLayout
    LayoutType="Absolute"
    WidthRequest="400"
    HeightRequest="300">
    <!-- Child controls here -->
</DrawUi:SkiaLayout>
```

### ContentLayout

A specialized layout type designed to host a single content element with optional attached elements. Ideal for scenarios where you need to position a main content with overlaid controls.

```xml
<DrawUi:ContentLayout>
    <DrawUi:SkiaImage Source="background.png" />
</DrawUi:ContentLayout>
```

## Layout Types

The `LayoutType` property on `SkiaLayout` supports several layout strategies:

- **Absolute**: Free positioning of elements using explicit coordinates
- **Grid**: Row and column-based grid layout
- **Column**: Vertical stacking of elements (similar to VStack)
- **Row**: Horizontal stacking of elements (similar to HStack)
- **Wrap**: Arranges elements in rows with automatic wrapping when space runs out

```xml
<DrawUi:SkiaLayout LayoutType="Wrap" Spacing="5,5">
    <!-- Items will wrap to new lines when they exceed available width -->
    <DrawUi:SkiaLabel Text="Item 1" />
    <DrawUi:SkiaLabel Text="Item 2" />
    <!-- More items -->
</DrawUi:SkiaLayout>
```

## Specialized Layout Controls

### InfiniteLayout

Extends ContentLayout to provide infinite scrolling capabilities in horizontal, vertical, or both directions.

```xml
<DrawUi:InfiniteLayout ScrollOrientation="Both">
    <DrawUi:SkiaImage Source="large_map.png" />
</DrawUi:InfiniteLayout>
```

### SnappingLayout

A specialized layout supporting snap points for controlled scrolling experience, ideal for carousel-like interfaces or paginated scrolling.

```xml
<DrawUi:SnappingLayout Orientation="Horizontal">
    <DrawUi:SkiaLabel Text="Page 1" />
    <DrawUi:SkiaLabel Text="Page 2" />
    <DrawUi:SkiaLabel Text="Page 3" />
</DrawUi:SnappingLayout>
```

## Performance Optimization

The layout system in DrawnUi.Maui is designed with performance in mind:

### Template Recycling

When displaying collections using ItemsSource and ItemTemplate, the layout system can recycle template instances to reduce object creation and improve performance:

```xml
<DrawUi:SkiaLayout
    LayoutType="Column"
    ItemsSource="{Binding Items}"
    RecyclingTemplate="Enabled">
    <DrawUi:SkiaLayout.ItemTemplate>
        <DataTemplate>
            <DrawUi:SkiaLabel Text="{Binding Name}" />
        </DataTemplate>
    </DrawUi:SkiaLayout.ItemTemplate>
</DrawUi:SkiaLayout>
```

### Virtualization

The layout system supports virtualization to only render items that are currently visible:

```xml
<DrawUi:SkiaLayout
    LayoutType="Column"
    ItemsSource="{Binding LargeCollection}"
    VirtualizationMode="Enabled">
    <!-- Template definition -->
</DrawUi:SkiaLayout>
```

### Measurement Strategies

Different measurement strategies control how and when items are measured:

- **MeasureFirst**: Measures all items before rendering (good for static content)
- **MeasureAll**: Continuously measures all items (useful for dynamic content)
- **MeasureVisible**: Only measures visible items (most efficient for large collections)

```xml
<DrawUi:SkiaLayout
    MeasuringStrategy="MeasureVisible"
    ItemsSource="{Binding LargeCollection}">
    <!-- Template definition -->
</DrawUi:SkiaLayout>
```

## Example: Creating a Grid Layout

Here's an example of creating a grid layout with defined rows and columns:

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

Note that all grid functionality is handled by the SkiaLayout with LayoutType="Grid", not through a separate GridLayout class.

## Under the Hood

The layout system is built on top of the `SkiaControl` base class, which provides core rendering capabilities. The layout controls extend this to add child management, measurement, and arrangement functionality. Internally, the system uses specialized structures like `LayoutStructure` and `GridStructure` to efficiently track and manage layout information.

Understanding the cache system is crucial for optimal performance. The caching options allow you to balance between CPU usage and memory consumption based on your specific needs:

- **None**: No caching, recalculated every frame
- **Operations**: Caches drawing commands
- **Image**: Caches as bitmap (more memory, less CPU)
- **GPU**: Uses hardware acceleration where available