# DrawnUi Documentation

__NOTE: this is under heavy construction AND NOT READY TO USE YET, may contain some outdated or non-exact information!!!.__  

Rendering engine to draw your UI on a Skia canvas, with gestures and animations, designed to draw pixel-perfect custom controls instead of using native ones, powered by [SkiaSharp](https://github.com/mono/SkiaSharp)😍. 
Create and render your custom controls on a hardware-accelerated Skia canvas with an improved common MAUI layout system.

Supports **iOS**, **MacCatalyst**, **Android**, **Windows**.

* To use inside a usual MAUI app, consume drawn controls here and there inside `Canvas` views.
* Create a totally drawn app with just one `Canvas` as root view, `SkiaShell` is provided for navigation.
* Drawn controls are totally virtual, these are commands for the engine on what and how to draw on a skia canvas.
* Free to use under the MIT license, a nuget package is available.

## About

[A small article](https://taublast.github.io/posts/MauiJuly/) about the library and why it was created

## Demo Apps

* This repo includes a Sandbox project for some custom controls, with playground examples, custom controls, maps etc
* More creating custom controls examples inside the [Engine Demo](https://github.com/taublast/AppoMobi.Maui.DrawnUi.Demo) 🤩 __Updated with latest nuget!__
* A [dynamic arcade game](https://github.com/taublast/AppoMobi.Maui.DrawnUi.SpaceShooter) drawn with this engine, uses preview nuget with SkiaSharp v3.
* A [drawn CollectionView demo](https://github.com/taublast/SurfAppCompareDrawn) where you could see how simple and profitable it is to convert an existing recycled cells list into a drawn one
* [Shaders Carousel Demo](https://github.com/taublast/ShadersCarousel/) featuring SkiaSharp v3 capabilities


## Features

- **SkiaSharp Rendering**: All controls are rendered using SkiaSharp for maximum performance
- **Platform Styling**: Automatic styling based on the current platform (iOS, Android, Windows)
- **Rich Controls**: Buttons, switches, checkboxes, and more with full styling support
- **Animation**: Built-in animation capabilities for rich, interactive UIs
- **Customization**: Extensive customization options for all controls

## Documentation Structure

This documentation is organized into the following sections:

- [Getting Started](articles/getting-started.md): Quick start guide and installation
- [Controls](articles/controls/index.md): Detailed documentation for each control
- [API Reference](api/index.md): Complete API documentation
- [Samples](articles/samples.md): Example applications and code snippets