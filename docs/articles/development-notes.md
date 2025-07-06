# Development Notes

This section contains important information for developers working with DrawnUI.Maui.

## Requirements

* **.NET 9 only** - Maui.Controls 9.0.70 minimum
* **SkiaSharp v3** - By default the main branch targets NET 9 and uses SkiaSharp v3
* **Legacy Support** - For NET 8 and SkiaSharp v2 versions use 1.2.x (no longer updated)

## Platform-Specific Requirements

### Windows
To use hardware accelerated Windows canvas with NET 9, you need to [pack your Windows project as MSIX](https://learn.microsoft.com/en-us/dotnet/maui/windows/setup?view=net-maui-9.0).

> **Visual Studio Note**: This might create a situation where you need to hit Build every time you change your code and only then hit Run, otherwise VS will run the previous package.

### All Platforms
All files to be consumed (images etc) must be placed inside the MAUI app `Resources/Raw` folder, subfolders allowed. If you need to load from the native app folder use prefix "file://".

## Build Configuration

The `<UseSkiaSharp3>true</UseSkiaSharp3>` inside `Directory.Build.props` file controls whether we build for:
- **v3 net9** - SkiaSharp v3 with .NET 9
- **v2 net8** - SkiaSharp v2 with .NET 8 (legacy)

## Package Information

### Current Package Strategy
* **Main Package**: `DrawnUi.Maui` (replaces old `AppoMobi.Maui.DrawnUi`)
* **Backward Compatibility**: Old package ID kept for compatibility for some time
* **Version Requirements**:
  - NET 9: versions 1.3.x and higher
  - NET 8 (legacy): versions 1.2.x (no longer updated)

### Additional Packages
There are additional packages supporting optional features:
- **DrawnUi.Maui.Camera** - Camera implementations for iOS, MacCatalyst, Windows, Android
- **DrawnUi.Maui.Game** - Gaming helpers and frame time interpolators
- **DrawnUi.Maui.Maps** - Map integration features

## Roadmap

### Planned Features
* **Accessibility Support** - Compatible and on the roadmap
* **Masonry Layout** - Todo item for SkiaLayout
* **Additional Platform Support** - Expanding Rive support beyond Windows

### Recent Updates
* Performance optimizations for layouts and SkiaLabel
* Safety optimizations for accelerated rendering handlers on all platforms
* Windows accelerated handler now synced with display when refresh rate is >=120Hz
* Frame time interpolator adjustments for DrawnUi.Maui.Game

## Development Best Practices

### Performance Tips
1. **Use Caching Appropriately**:
   - `UseCache = SkiaCacheType.Operations` for labels and SVG
   - `UseCache = SkiaCacheType.Image` for complex layouts, buttons
   - `UseCache = SkiaCacheType.GPU` for small static overlays

2. **Layout Optimization**:
   - Only visible elements are rendered
   - Recycling templates for better performance
   - Virtualization for large lists

3. **Image Handling**:
   - Images are cached on per-app run basis
   - Automatic reload when going online
   - Velocity-based loading control in SkiaScroll

### Common Patterns
* **Custom Controls**: Inherit from `SkiaControl` or `SkiaLayout`
* **Gesture Handling**: Implement `ISkiaGestureListener` or override `ProcessGestures`
* **Effects**: Use `VisualEffects` property for filters, shaders, and animations

## Migration Notes

### From Legacy Versions
* `ViewsAdapter` properties renamed:
  - `GetViewForIndex` → `GetExistingViewAtIndex`
  - `GetViewAtIndex` → `GetOrCreateViewForIndex`
* `Canvas` property `RenderingMode` replaced `HardwareAcceleration`
* Stack and absolute layouts now correctly apply one-directional `Fill` of children

### Breaking Changes
* `Margins` and `Padding` now work properly everywhere (might affect legacy UIs)
* `OnMeasuring` can be overridden, while `Measure` is not virtual anymore
* Performance optimizations may affect timing-sensitive code

## Troubleshooting Development Issues

For common development issues and solutions, see the [FAQ section](faq.md#troubleshooting).

## Contributing

When contributing to DrawnUi.Maui:
1. Target .NET 9 and SkiaSharp v3
2. Follow existing code patterns and naming conventions
3. Add appropriate caching for performance-critical controls
4. Include XML documentation for public APIs
5. Test on all supported platforms when possible

## Resources

- **Source Code**: [GitHub Repository](https://github.com/taublast/DrawnUi.Maui)
- **Issues**: [GitHub Issues](https://github.com/taublast/DrawnUi.Maui/issues)
- **Discussions**: [GitHub Discussions](https://github.com/taublast/DrawnUi/discussions)
- **Background Article**: [Why DrawnUi was created](https://taublast.github.io/posts/MauiJuly/)
