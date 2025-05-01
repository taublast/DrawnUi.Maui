# Controls Overview

DrawnUi positions itsself as an angine providing a toolset to create and use custom drawn controls. Out-of-the box it provides you with base controls that can be used a lego-bricks to composite custom controls, and proposes some useful pre-made custom controls.

The main spirit is to have all controlls subclassable and customizable at the maximum possible extent.

DrawnUi provides a comprehensive set of UI controls rendered with SkiaSharp for optimal performance. All controls support platform-specific styling and extensive customization options.

## Control Categories

DrawnUi controls are organized into several categories:

### Button Controls
- [SkiaButton](buttons.md): Standard button with platform-specific styling
- [Custom button variants](buttons.md#variants): Outlined, text-only, and other button styles

### Toggle Controls
- [SkiaSwitch](switches.md#skiaswitch): Platform-styled toggle switch
- [SkiaCheckbox](switches.md#skiacheckbox): Platform-styled checkbox
- [SkiaToggle](switches.md#skiatoggle): Base toggle class for custom toggles

### Layout Controls
- [SkiaLayout](layouts.md#skialayout): Base layout container
- [GridLayout](layouts.md#gridlayout): Grid-based layout
- [HStack/VStack](layouts.md#stack): Horizontal and vertical stack layouts

### Text Controls
- [SkiaLabel](text.md#skialabel): High-performance text rendering
- [SkiaMarkdownLabel](text.md#skiamarkdownlabel): Markdown-capable text control

### Image Controls
- [SkiaImage](images.md#skiaimage): High-performance image rendering
- [SkiaSvg](images.md#skiasvg): SVG rendering
- [SkiaGif](images.md#skiagif): Animated GIF support