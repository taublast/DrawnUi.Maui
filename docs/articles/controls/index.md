# Controls Overview

DrawnUi positions itsself as an angine providing a toolset to create and use custom drawn controls. Out-of-the box it provides you with base controls that can be used a lego-bricks to composite custom controls, and proposes some useful pre-made custom controls.

The main spirit is to have all controlls subclassable and customizable at the maximum possible extent.

DrawnUi provides a comprehensive set of UI controls rendered with SkiaSharp for optimal performance. All controls support platform-specific styling and extensive customization options.

## Control Categories

DrawnUi controls can be organized into several categories:

### Aliases
There are controls that are aliases for other controls, te make porting existing native apps easier, to replace one name in code with another:
- SkiaFrame is an alias for SKiaShape of Rectangle type, MAUI Frame
- SkiaStack is for SKiaLayout type Column with default horizontal Fill, MAUI VerticalStackLayout
- SkiaRow is for SkiaLayout type Row, MAUI HorizontalStackLayout
- SkiaLayer is for SkiaLayout type Absolute with default horizontal Fill, MAUI Grid with 1 col/row used for layering
- SkiaGrid is for SkiaLayout type Grid with default horizontal Fill, MAUI Grid
- SkiaWrap is for SkiaLayout type Wrap with default horizontal Fill, a bit similar to MAUI FlexLayout

### Layout Controls
- [SkiaLayout](layouts.md#skialayout): Base layout container. supported types: Absolute, Grid, Column, Row, Wrap. 
- SkiaScroll: Scrolling container with virtualization support
- SnappingLayout: Base class for snap points
- SkiaDrawer: Swipe-in/out panel, subclassed SnappingLayout,
- SkiaCarousel: Swipeable carousel, subclassed SnappingLayout
- [ContentLayout](layouts.md#gridlayout): Optimized for single child, SKiaShape derives from this one
- SkiaShape: Base class for all shapes, can wrap other elements to be clipped inside

### Text Controls
- [SkiaLabel](text.md#skialabel): High-performance text rendering, supports spans
- [SkiaMarkdownLabel](text.md#skiamarkdownlabel): Markdown-capable text, unicode friendly, autofinds font its text (auto-creates spans). Use for text with complex formatting, smileys, and different languages

### Graphics Controls
- [SkiaImage](images.md#skiaimage): Image rendering
- [SkiaSvg](images.md#skiasvg): SVG rendering
- [SkiaGif](images.md#skiagif): Animated GIF support
- SkiaMediaImage: Media image, subclassed SkiaImage
- SkiaLottie: Lottie animation, subclassed SkiaImage

### Button Controls
It's important to notice that every control can behaive like a button with gestures attached, but here is a pre-made button control, providing different platforms looks via `ControlStyle` property:
- [SkiaButton](buttons.md): Standard button with platform-specific styling

### Toggle Controls
- [SkiaSwitch](switches.md#skiaswitch): Platform-styled toggle switch
- [SkiaCheckbox](switches.md#skiacheckbox): Platform-styled checkbox
- [SkiaToggle](switches.md#skiatoggle): Base toggle class for custom toggles
- SkiaRadioButton: Radio button, subclassed SkiaToggle

### Navigation control
- [SkiaShell](shell.md): Navigation framework, subclassed SkiaLayout
- SkiaViewSwitcher: View switcher, subclassed SkiaLayout

### Helper controls
- SkiaDecoratedGrid: A grid able to draw shapes between cells
