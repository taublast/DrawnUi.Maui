# SkiaCamera

A drawn camera control for .NET MAUI.
Supports Android, iOS, MacCatalyst, Windows.


* Provides preview frames and still capture results for further processing.  
Easily pass images images to AI/ML!

* Renders live preview on a SkiaShap canvas with hardware acceleration.
Apply shaders, adjustments and transforms to camera preview in realtime and draw anything over!

## Platform Support

| Platform | Status | Implementation |
|----------|--------|----------------|
| Android | ✅ Complete | Camera2 API with CameraX |
| iOS | ✅ Complete | AVFoundation (shared with macCatalyst) |
| macCatalyst | ✅ Complete | AVFoundation (shared with iOS) |
| Windows | ✅ Complete | MediaCapture with WinRT APIs |

## Installation

```
//todo
```

## Basic Usage Example

```
//todo
```

## Testing

A test page is provided in the Sandbox project:
- `CameraTestPage.xaml` - UI layout with camera preview and controls
- `CameraTestPage.xaml.cs` - Event handling and camera operations

## Common patterns

### Image effects and filters

### Consume preview for AI/ML processing

## Permissions

```
//todo
```

## Troubleshooting

1. **Camera not starting**: Check permissions and camera availability
2. **Black preview**: Verify camera device enumeration
3. **Capture failures**: Check storage permissions and available space
4. **Performance issues**: Ensure logs not running for every frame, cache controls that are drawn over

## Future Enhancements

- [ ] Video recording support
- [ ] Advanced camera controls (focus, exposure, ISO)
- [ ] Custom resolution selection



