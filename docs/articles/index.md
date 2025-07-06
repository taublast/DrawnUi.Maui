# Articles

__NOTE: this is under heavy construction AND NOT READY TO USE YET, may contain some outdated or non-exact information!!!.__  

This section contains documentation articles and guides for using DrawnUi.

## Getting Started

- [Installation and Setup](getting-started.md)
- [Your First DrawnUi App](first-app.md)
- [Understanding the Drawing Pipeline](drawing-pipeline.md)

## Controls

- [Overview](controls/index.md)
- [Buttons](controls/buttons.md)
- [Switches and Toggles](controls/switches.md)
- [Layout Controls](controls/layouts.md)
- [Text and Labels](controls/text.md)
- [Images](controls/images.md)

## Advanced Topics

- [Platform-Specific Styling](advanced/platform-styling.md)
- [Layout System Architecture](advanced/layout-system.md)
- [Gradients](advanced/gradients.md)
- [Game UI & Interactive Games](advanced/game-ui.md)
- [SkiaScroll & Virtualization](advanced/skiascroll.md)
- [Gestures & Touch Input](advanced/gestures.md)


## FAQ

- Is it DrawnUI or DrawnUi?
- Both are totally fine.

- How do i create my custom button?
- While you can use SkiaButton and set a custom content to it, You can also use a click handler Tapped with ANY control you like.

- I have an exting MAUI app, how DrawnUi can be beneficial to me?
- You can definetely speed your app by replacing slow UI parts with faster drawn sections, replacing a horde or native controls with just one canvas. Check out the [Porting MAUI](porting-maui.md) guide.

- Knowing that properties are in points, how do i create a line or stroke of exactly 1 pixel?
- When working with SkiaShape use a negative value (ex: -1) to pass pixels instead of points to compatible properties like StrokeWidth and similar.

- How do i bind SkiaImage source not to a file/url for to an existing bitmap?
- Use `ImageBitmap` property for that, type is `LoadedImageSource`.