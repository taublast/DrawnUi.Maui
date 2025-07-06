# DrawnUi.Maui

**Build beautiful, high-performance mobile apps with C# code-behind instead of XAML**

DrawnUi is a rendering engine that draws your entire UI on a hardware-accelerated Skia canvas. Create pixel-perfect custom controls with gestures and animations, all powered by [SkiaSharp](https://github.com/mono/SkiaSharp) 😍.

**Supports:** iOS • MacCatalyst • Android • Windows

---

## 🚀 Quick Start

### 1. Install DrawnUi
```bash
dotnet add package DrawnUi.Maui
```

### 2. Learn the Basics
- **[📖 Getting Started](articles/getting-started.md)** - Installation and setup guide
- **[🎯 Your First App](articles/first-app.md)** - Build your first DrawnUI app in 5 minutes
- **[⚡ Fluent Extensions](articles/fluent-extensions.md)** - Master the code-behind fluent API

### 3. Explore Controls
- **[🎛️ All Controls](articles/controls/index.md)** - Buttons, layouts, animations, and more
- **[📱 Live Demo](demo.md)** - Interactive examples you can try

### 4. Go Advanced
- **[🏗️ Advanced Topics](articles/advanced/index.md)** - Architecture, performance, and platform-specific features
- **[📚 API Reference](api/index.md)** - Complete technical documentation

---

## ✨ Why DrawnUi?

**🎨 Code-Behind First**
- Write UI in C# with fluent extensions - no XAML needed
- Type-safe, IntelliSense-friendly development
- Reactive property observation without traditional bindings

**⚡ High Performance**
- Hardware-accelerated Skia rendering
- Efficient caching and virtualization
- Smooth 60fps animations and gestures

**🎯 Pixel Perfect**
- Consistent UI across all platforms
- Custom controls that look exactly how you want
- Full control over every pixel

**🔧 Flexible Architecture**
- Use alongside existing MAUI controls
- Or go fully drawn with SkiaShell navigation
- MIT licensed and production-ready

---

## 📱 See It In Action

**Live Examples:**
- **[Engine Demo](https://github.com/taublast/AppoMobi.Maui.DrawnUi.Demo)** - Comprehensive control showcase
- **[Space Shooter Game](https://github.com/taublast/AppoMobi.Maui.DrawnUi.SpaceShooter)** - Full arcade game built with DrawnUI
- **[CollectionView Demo](https://github.com/taublast/SurfAppCompareDrawn)** - Performance comparison with native controls
- **[Shaders Carousel](https://github.com/taublast/ShadersCarousel/)** - Advanced SkiaSharp v3 effects

---

## ❓ Frequently Asked Questions

### Getting Started

**Q: Can I use DrawnUI with .NET 9?**
A: Yes! DrawnUI works with .NET 9. Remember that SkiaLabel, SkiaLayout etc. are virtual drawn controls that must be placed inside a `Canvas`: `<draw:Canvas>your skia controls</draw:Canvas>`. Only `Canvas` has handlers for normal and hardware accelerated views.

**Q: Can I use MAUI's default Images folder with DrawnUI?**
A: Unfortunately no. DrawnUI can read from `Resources/Raw` folder and from native storage if the app has written there, but not from the Images folder. The Images folder is "hardcoded-designed for MAUI views". Place your images in `Resources/Raw` instead, subfolders are allowed.

**Q: How do I use my own SKBitmap or MAUI resource images with SkiaImage?**
A: You have two options:
1. Place images in `Resources/Raw` folder: `<draw:SkiaImage Source="baboon.jpg" />`
2. Set images directly: `mySkiaImage.SetImageInternal(mySkImage)` or `mySkiaImage.SetBitmapInternal(mySkBitmap)`

### Controls & Gestures

**Q: How do I prevent touch events from passing through overlapping controls?**
A: Use the `BlockGesturesBelow="True"` property on the top control. Note that `InputTransparent` makes the control itself avoid gestures, not controls below it in the Z-axis.

**Q: How can I enable mouse wheel scrolling in SkiaScroll?**
A: Mouse wheel scrolling isn't built-in, but you can easily add it by subclassing SkiaScroll and overriding `ProcessGestures` to handle `TouchActionResult.Wheel` events. Check the discussions for a complete code example.

**Q: How do I maintain scroll position when reloading items in SkiaScroll?**
A: Save the current `ViewportOffsetY` before reloading, then restore it after the new items are loaded using `ScrollTo(x, savedOffsetY, 0)`.

### Advanced Usage

**Q: Can I embed native MAUI controls inside DrawnUI?**
A: Yes! Use `SkiaMauiElement` to embed native MAUI controls like WebView inside your DrawnUI canvas. This allows you to combine the best of both worlds.

**Q: How do I create custom controls with DrawnUI?**
A: Inherit from `SkiaControl` for basic controls or `SkiaLayout` for container controls. Override the `Paint` method to define your custom drawing logic using SkiaSharp.

---

## 🆘 Need Help?

- **[❓ Troubleshooting](articles/fluent-extensions.md#troubleshooting)** - Common issues and solutions
- **[💬 GitHub Issues](https://github.com/taublast/DrawnUi.Maui/issues)** - Report bugs or ask questions
- **[🗨️ GitHub Discussions](https://github.com/taublast/DrawnUi/discussions)** - Ask questions and get community help
- **[📖 Background Article](https://taublast.github.io/posts/MauiJuly/)** - Why DrawnUI was created

**Can't find the answer to your question?** → **[Ask in GitHub Discussions](https://github.com/taublast/DrawnUi/discussions)** - The community is here to help!

---

**Ready to get started?** → **[Install and Setup Guide](articles/getting-started.md)**