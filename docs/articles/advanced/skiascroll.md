# Advanced Scrolling with SkiaScroll in DrawnUi.Maui

DrawnUi.Maui’s SkiaScroll control provides high-performance, flexible scrolling for custom UIs, games, dashboards, and data-heavy apps. This article covers advanced usage, virtualization, customization, and best practices for SkiaScroll and related controls.

## Why SkiaScroll?
- **Smooth, pixel-perfect scrolling** on all platforms
- **Supports both vertical, horizontal, and bidirectional scrolling**
- **Virtualization** for large data sets
- **Customizable headers, footers, and overlays**
- **Pinch-to-zoom and gesture support**
- **Works with any DrawnUi content: layouts, images, shapes, etc.**

## Basic Usage

```xml
<DrawUi:SkiaScroll Orientation="Vertical" WidthRequest="400" HeightRequest="600">
    <DrawUi:SkiaLayout LayoutType="Column" Spacing="10">
        <DrawUi:SkiaLabel Text="Item 1" />
        <DrawUi:SkiaLabel Text="Item 2" />
        <!-- More items -->
    </DrawUi:SkiaLayout>
</DrawUi:SkiaScroll>
```

## Multi-Directional and Zoomable Scrolling

```xml
<DrawUi:SkiaScroll Orientation="Both" ZoomLocked="False" ZoomMin="1" ZoomMax="3">
    <DrawUi:SkiaLayout>
        <DrawUi:SkiaImage Source="large_map.jpg" />
    </DrawUi:SkiaLayout>
</DrawUi:SkiaScroll>
```

## Virtualization for Large Data Sets

Enable virtualization for smooth performance with thousands of items:

```xml
<DrawUi:SkiaScroll UseVirtual="True" Orientation="Vertical">
    <DrawUi:SkiaLayout 
        LayoutType="Column" 
        ItemsSource="{Binding LargeItemCollection}"
        VirtualizationMode="Enabled">
        <DrawUi:SkiaLayout.ItemTemplate>
            <DataTemplate>
                <DrawUi:SkiaLabel Text="{Binding Title}" />
            </DataTemplate>
        </DrawUi:SkiaLayout.ItemTemplate>
    </DrawUi:SkiaLayout>
</DrawUi:SkiaScroll>
```

- `UseVirtual` on SkiaScroll enables virtualization.
- `VirtualizationMode` on SkiaLayout controls the strategy (Enabled, Smart, Managed).
- Combine with `RecyclingTemplate` for template reuse.

## Custom Headers, Footers, and Overlays

```xml
<DrawUi:SkiaScroll HeaderSticky="True" HeaderBehind="False">
    <DrawUi:SkiaScroll.Header>
        <DrawUi:SkiaShape Type="Rectangle" BackgroundColor="#3498DB" HeightRequest="80">
            <DrawUi:SkiaLabel Text="Sticky Header" TextColor="White" FontSize="18" />
        </DrawUi:SkiaShape>
    </DrawUi:SkiaScroll.Header>
    <DrawUi:SkiaLayout LayoutType="Column">
        <!-- Content -->
    </DrawUi:SkiaLayout>
    <DrawUi:SkiaScroll.Footer>
        <DrawUi:SkiaShape Type="Rectangle" BackgroundColor="#2C3E50" HeightRequest="60">
            <DrawUi:SkiaLabel Text="Footer" TextColor="White" />
        </DrawUi:SkiaShape>
    </DrawUi:SkiaScroll.Footer>
</DrawUi:SkiaScroll>
```

## Pull-to-Refresh

```xml
<DrawUi:SkiaScroll x:Name="MyScrollView" Refreshing="OnRefreshing">
    <DrawUi:SkiaScroll.RefreshIndicator>
        <DrawUi:RefreshIndicator />
    </DrawUi:SkiaScroll.RefreshIndicator>
    <DrawUi:SkiaLayout LayoutType="Column">
        <!-- Content items -->
    </DrawUi:SkiaLayout>
</DrawUi:SkiaScroll>
```

In code-behind:
```csharp
private async void OnRefreshing(object sender, EventArgs e)
{
    // Perform refresh operation
    await LoadDataAsync();
    ((SkiaScroll)sender).EndRefresh();
}
```

## Infinite and Looped Scrolling

Use SkiaScrollLooped for banners, carousels, or infinite galleries:

```xml
<DrawUi:SkiaScrollLooped Orientation="Horizontal" IsBanner="True" CycleSpace="100">
    <DrawUi:SkiaLayout LayoutType="Row">
        <DrawUi:SkiaImage Source="image1.jpg" />
        <DrawUi:SkiaImage Source="image2.jpg" />
        <!-- More images -->
    </DrawUi:SkiaLayout>
</DrawUi:SkiaScrollLooped>
```

## Programmatic Scrolling and Position Tracking

```csharp
// Scroll to a specific position
myScroll.ScrollToPosition(0, 500, true); // Animated scroll to Y=500

// Scroll to a child element
myScroll.ScrollToView(targetElement, true);

// Track scroll position
float y = myScroll.ViewportOffsetY;
```

## Performance Tips
- Enable virtualization for large lists
- Use `Cache="Operations"` for static or rarely-changing content
- Avoid nesting too many scrolls; prefer flat layouts
- Use SkiaLabelFps to monitor performance
- For custom drawing, override OnDraw in your content controls

## Advanced: Custom Scroll Effects and Gestures
- Implement parallax, sticky headers, or custom scroll physics by extending SkiaScroll
- Use gesture listeners for advanced input (drag, swipe, pinch)
- Combine with SkiaDrawer for overlay panels

## Summary
SkiaScroll and related controls provide a robust, high-performance foundation for any scrolling UI in DrawnUi.Maui. With support for virtualization, zoom, custom overlays, and advanced gestures, you can build everything from chat apps to dashboards and games with smooth, responsive scrolling.
