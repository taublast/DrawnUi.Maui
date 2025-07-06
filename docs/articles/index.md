# DrawnUi.Maui

**A small library for .NET MAUI built on top of SkiaSharp, to layout and draw your UI on a Skia canvas.**

* Provides infrastructure to create and render drawn controls with gestures and animations, comes with some pre-built controls.  
* Hardware-accelerated rendering on iOS • MacCatalyst • Android • Windows  
* Free to use under the MIT license, a nuget package is available.
* To consume inside a usual MAUI app, wrap drawn controls inside `Canvas` views.
* To create a totally drawn apps with just one `Canvas` as root view. `SkiaShell`, `SkiaViewSwitcher` provided for navigation with modals, popups, toasts etc.
* Drawn controls are virtual: no native views/handlers are created, UI-thread is not required to accessed and modify them.
 

## Live Examples

- **[Engine Demo](https://github.com/taublast/AppoMobi.Maui.DrawnUi.Demo)** - A totally drawn app demo with recycled cells, camera etc
- **[Space Shooter Game](https://github.com/taublast/AppoMobi.Maui.DrawnUi.SpaceShooter)** - Arcade game etude built with DrawnUi
- **[Shaders Carousel](https://github.com/taublast/ShadersCarousel/)** - A totally drawn app with making use of SkiaSharp v3 shaders
- **[Sandbox project](https://github.com/taublast/DrawnUi.Maui/tree/main/src/Maui/Samples/Sandbox)** - Experiment with pre-built drawn controls and more

---

## Frequently Asked Questions

### General

**Q: What is it at all, what is the difference between DrawnUi and Uno/Avalonia?**  
A: This is really not comparable, it's a library for .NET MAUI to avoid using native controls where possible.

**Q: Why avoid native controls?**  
A: A matter of choice to just draw anything without using pre-built designs.

**Q: Do I neded to know how to draw on a canvas??**  
A: Nope, just use prebuild drawn controls and customize them at will without limits. You would find that almost every method is virtual, all controls are initially designed to be subclassed and customized.

**Q: Can I still use XAML?**  
A: Yes you can use both XAML and code-behind to create your UI.  

**Q: Can I avoid using XAML at all costs?**  
A: Yes feel free to use code-behind, up to using background thread to access and modify drawn controls properties.

**Q: How do I create custom controls with DrawnUI?**  
A: Inherit from `SkiaControl` for basic controls or `SkiaLayout` for container controls. Override the `Paint` method to define your custom drawing logic using SkiaSharp.

**Q: Can I embed native MAUI controls inside DrawnUI?**  
A: Yes! Use `SkiaMauiElement` to embed native MAUI controls like WebView inside your DrawnUI canvas. This allows you to combine the best of both worlds.

### Advanced

**Q: How do I use set SkiaImage source?**  
A: You have several options:
1. Place images in `Resources/Raw` folder: `<draw:SkiaImage Source="baboon.jpg" />`
2. Set images directly: `mySkiaImage.SetImageInternal(mySkImage)` or `mySkiaImage.SetBitmapInternal(mySkBitmap)`

**Q: Can I use MAUI's default Images folder?**  
A: No.

**Q: How do I prevent touch events from passing through overlapping controls?**  
A: Use the `BlockGesturesBelow="True"` property on the top control. Note that `InputTransparent` makes the control itself avoid gestures, not controls below it in the Z-axis.

**Q: How can I enable mouse wheel scrolling in SkiaScroll?**  
A: Mouse wheel scrolling isn't built-in, but you can easily add it by subclassing SkiaScroll and overriding `ProcessGestures` to handle `TouchActionResult.Wheel` events. Check the discussions for a complete code example.

**Q: How do I maintain scroll position when reloading items in SkiaScroll?**  
A: Save the current `ViewportOffsetY` before reloading, then restore it after the new items are loaded using `ScrollTo(x, savedOffsetY, 0)`.

**Q: How do I internally rebuild the ItemsSource?**  
A: Directly call ApplyItemsSource().


---

**Can't find the answer to your question?** → **[Ask in GitHub Discussions](https://github.com/taublast/DrawnUi/discussions)** - The community is here to help!

## Need More Help?

- **[❓ Troubleshooting](fluent-extensions.md#troubleshooting)** - Common issues and solutions
- **[💬 GitHub Issues](https://github.com/taublast/DrawnUi.Maui/issues)** - Report bugs or ask questions
- **[📖 Background Article](https://taublast.github.io/posts/MauiJuly/)** - Why DrawnUi was created

---

**Ready to get started?** → **[Install and Setup Guide](getting-started.md)**