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

Apple:

Add this this your Infi.plist:

```xml

```

## Basic Usage Example

Create SkiaControl and call an embedded command:

 

It's important to understand the difference between setting the `IsOn` property to `true` and invoking the `Start` method directly.

In some rare scenarios your app could have several camera controls and `IsOn` is like a radiobutton value, meaning "this camera is `On` right now", others must have this value set to `false`.
When app goes to background all cameras stop and when app returns from background state the control would need to know whether to automatically resume (start again basically) a specific camera instance, and it would if `IsOn` was set to `true`. 

Also when your control appears for the first time and you didn't get user camera permissions yet the native platform camera control should not be created yet, and it wouldn't if `SkiaControl` `IsOn` is still false. So the main workflow here is to create `SkiaCamera`, execute your needed preliminary logic and then safely turn the camera on by setting `IsOn` to `true`. For example, you can invoke `public ICommand CommandCheckPermissionsAndStart` it does something similar, like one could guess by it's name.

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

## References:

iOS: 
* [Manual Camera Controls in Xamarin.iOS](https://github.com/MicrosoftDocs/xamarin-docs/blob/0506e3bf14b520776fc7d33781f89069bbc57138/docs/ios/user-interface/controls/intro-to-manual-camera-controls.md) by David Britch

