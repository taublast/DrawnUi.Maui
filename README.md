# DrawnUI for .NET MAUI
![License](https://img.shields.io/github/license/taublast/DrawnUi.Maui.svg)
![NuGet Downloads](https://img.shields.io/nuget/dt/AppoMobi.Maui.DrawnUi.svg)

Rendering engine to draw your UI on a Skia canvas, with gestures and animations, designed to draw pixel-perfect custom controls instead of using native ones, powered by [SkiaSharp](https://github.com/mono/SkiaSharp)😍. Plug any existing and unique SkiaSharp code into MAUI layout!

Supports **iOS**, **MacCatalyst**, **Android**, **Windows**.

* To use inside a usual MAUI app, consume drawn controls here and there inside `Canvas` views.
* Create a totally drawn app with just one `Canvas` as root view, `SkiaShell` is provided for navigation.
* Drawn controls are totally virtual, these are commands for the engine on what and how to draw on a skia canvas.
* Free to use under the MIT license, a nuget package is available.
* A Light [version for Xamarin](https://github.com/taublast/DrawnUi.Xamarin) is there too.

_The current development state is __ALPHA__, features remain to be implemented, documentation incoming._

https://github.com/taublast/DrawnUi.Maui/assets/25801194/3b360229-ce3b-4d33-a85b-554d1cca8408

## What's new

* Canvas auto-size fix
* Nuget 1.2.3.8

## Demo Apps

* This repo includes a Sandbox project for some custom controls, with playground examples, custom controls, maps etc
* More creating custom controls examples inside the [Engine Demo](https://github.com/taublast/AppoMobi.Maui.DrawnUi.Demo) 🤩 __Updated with latest nuget!__
* A [dynamic arcade game](https://github.com/taublast/AppoMobi.Maui.DrawnUi.SpaceShooter) drawn with this engine, uses preview nuget with SkiaSharp v3.
* A [drawn CollectionView demo](https://github.com/taublast/SurfAppCompareDrawn) where you could see how simple and profitable it is to convert an existing recycled cells list into a drawn one
* For real published apps scroll below Features!

 [ShaderEffect.webm](https://github.com/taublast/DrawnUi.Maui/assets/25801194/47c97290-e16b-4928-bfa4-8b29fb0ff8e1)

Subclassed `SkiaShaderEffect`, implementing `ISkiaGestureProcessor`, `IStateEffect` and `IPostRendererEffect`.

___Please star ⭐ if you like it!___

## Features

* __Draw your UI using SkiaSharp with hardware acceleration__
* __Easily create your controls and animations__
* __Design in XAML or code-behind__
* __2D and 3D Transforms__
* __Animations and transforms__ targeting max fps
* __Gestures__ support for panning, scrolling and zooming (_rotation on the roadmap_)
* __Caching system__ for elements and images
* __Optimized for performance__, rendering only visible elements, recycling templates etc
* __Navigate__ on canvas using MAUI familiar Shell techniques 

* __Prebuilt Basic Ui Elements__	
	* __SkiaControl__ Your lego brick to create anything
	* __SkiaShape__ Path, Rectangle, Circle, Ellipse, Gauge etc, can wrap other elements to be clipped inside
	* __SkiaLabel__, multiline with many options like dropshadow, gradients etc
	* __SkiaImage__ with many options and filters
	* __SkiaSvg__ with many options
	* __SkiaLayout__ (Absolute, Grid, Vertical stack, Horizontal stack, _todo Masonry_) with templates support
	* __SkiaScroll__ (Horizontal, Vertical, Both) with header, footer, zoom support and adjustable inertia, bounce, snap and much more. Can act like a collectionview with custom refresh indicator, load more etc
	* __SkiaHotspot__ to handle gestures in a lazy way
	* __SkiaBackdrop__ to apply effects to background below, like blur etc
	* __SkiaMauiElement__ to embed maui controls in your canvas
	* __and more..__ 

* __Derived custom controls__
	* __SkiaScrollLooped__ for neverending scrolls
	* __RefreshIndicator__ can use lottie and anything for your scroll refresh view
    * __SkiaDrawer__ to swipe in and out your controls
	* __SkiaCarousel__ swipe and slide controls inside a carousel
	* __SkiaDecoratedGrid__ to draw shapes between rows and columns
	* __ScrollPickerWheel__ for creating wheel pickers
	* __SkiaTabsSelector__ create top and bottom tabs
	* __SkiaViewSwitcher__ switch your views, pop, push and slide	
	* __SkiaLottie__ with tint customization
	* __SkiaGif__ a dedicated lightweight GIF-player with playback properties
	* __SkiaMediaImage__ a subclassed `SkiaImage` for displaying any kind of images (image/animated gif/more..)
	* __SkiaRive__ (actually Windows only)
	* __SkiaButton__ include anything inside, text, images etc
	* __SkiaSlider__ incuding range selction capability
	* __SkiaHoverMask__ to overlay a clipping shape
	* __SkiaLabelFps__ for developement
	* __Create your own!__ 

* Animated Effects
	* __Ripple__
	* __Shimmer__
	* __BlinkColors__
	* __Commit yours!__

* Transforms
	* TranslationX
	* TranslationY
	* ScaleX
	* ScaleY
	* Rotation
	* CameraAngleX
	* CameraAngleY
	* CameraAngleZ
	* SkewX
	* SkewY
	* Perspective1
	* Perspective2

## Published Apps powered by DrawnUI For .Net MAUI

### Bug ID: Insect Identifier AI

_Totally drawn with just one root view `Canvas` and `SkiaShell` for navigation. First ever drawn MAUI app!_

GooglePlay: https://play.google.com/store/apps/details?id=com.niroapps.insects

### Racebox

_MAUI pages with canvases, custom navigation. All scrolls, cells collections, maps, buttons, labels and custom controls are drawn._

iOS: https://apps.apple.com/us/app/racebox-vehicle-dynamics/id6444165250  
GooglePlay: https://play.google.com/store/apps/details?id=com.raceboxcompanion.app


 ## Development Notes

* All files to be consumed (images etc) must be placed inside the MAUI app Resources/Raw folder, subfolders allowed. If you need to load from the native app folder use prefix "file://".
* Accessibility support is compatible and is on the roadmap.

## Installation

Install the package __AppoMobi.Maui.DrawnUi__ from NuGet, please install stable versions only, avoid -pre.

After that initialize the library inside your MauiProgram.cs file:

```csharp
builder.UseDrawnUi();
```

## ___Docs are under construction!___

## Quick Start

You will be mainly using Maui view `Canvas` that will wrap your SkiaControls.
Anywhere in your existing Maui app you can include a `Canvas` and start drawing your UI.
The `Canvas` control is aware of its children's size and will resize accordingly.
At the same time, you could set a fixed size for the `Canvas` and its children will adapt to it.

#### Xaml
Import the namespace:
```xml  
  xmlns:draw="http://schemas.appomobi.com/drawnUi/2023/draw"
```
Consume:

```xml  
<draw:Canvas>
     <draw:SkiaSvg
        Source="Svg/dotnet_bot.svg"
        LockRatio="1"
        TintColor="White"
        WidthRequest="44" />
</draw:Canvas>
```

As you can see in this example the Maui view `Canvas` will adapt its size to drawn content and should take 44x44 pts. `LockRatio="1"` tells the engine to take the highest calculated dimension and multiply it by 1, so even if we omitted `HeightRequest` it was set to 44.

#### Code behind

```csharp
	_todo_
```

Please check the demo app, it contains many examples of usage.

### MAUI Controls Replication

#### Draw a line or reserve space

Use a simple `SkiaControl` with height and background color set. For complex shapes use `SkiaShape` or `SkiaPath`.

#### Simulate MAUI Grid

`SkiaLayout` of `Grid` type, set children properties as usual (`Grid.`something)

#### Simulate MAUI StackLayout inside a ScrollView

`SkiaScroll` + `SkiaLayout` of type `Column`/`Row`. Its up to you to decide whether to cache the layout or its children only. It's best not to cache the layout for large stacks and the virtualisation will enter the game.

#### Simulate MAUI CollectionView

`SkiaScroll` + `SkiaLayout` of type `Column`/`Row` (`ItemTemplate`=...). Set cache of the cell to `ImageDoubleBuffered` or other appropriate. You might also what to order to create `ItemTemplate` in background not to freeze the UI by using `InitializeTemplatesInBackgroundDelay` property.


#### Simulate MAUI StackLayout with a BidableLayout.ItemTemplate 

`SkiaScroll` with `Virtualisation`=`Disabled` + `SkiaLayout` of type `Column`/`Row` (`ItemTemplate`=...) do not forget to cache your cell template. 

## Images

* Images loaded and converted for skia format are cached on a per-app run basis.

* When an image fails to load then if the app was offline the image will get its method `ReloadSource` invoked. So when you go online your missing images will get automatically loaded!

* 'SkiaScroll' can also control the loading of images via `VelocityImageLoaderLock` property, that would lock and unlock loading of images globally in case of a huge velocity scroll.

Base control for using images is `SkiaImage` with many virtuals to be easily subclassed for your needs. The `Source` property is a usual maui `ImageSource`.
`SkiaImageManager` loads platform sources and converts them to skia format. It falls back to default MAUI methods for loading `ImageSource` in some cases.

On Android the `Glide` library is used for loading urls. It caches images, making it so that if the app is offline it still gets its images from cache if they were loaded previously.

### Gestures

To make your root `Canvas` catch gestures you need to attach a `TouchEffect` to it. This can be done automatically by setting `GesturesEnabled="True"` for the `Canvas`.
After that skia controls can process gestures in multiple ways:
* At low level by implementing an `ISkiaGestureListener` interface and overriding `OnGestureReceived`.
* At control level by overriding `ProcessGestures`, recommended for custom controls.
* Attaching an `AddGestures` effect that has properties similar to `SkiaHotspot`.
* Including a `SkiaHotspot` as a child.
* Using a `SkiaButton`.

Parent controls have full control over gestures and passing them to children. 
In a base scenario, a gesture would be passed all along to the ends of a view tree to its ends for every top-level control.
If a gesture is marked as consumed (by returning the reference of the consumer, not consumed if `null`) a control would typically stop processing gestures at its level. 

By overriding `ProcessGestures` any control might process gestures with or without passing them to children.

When creating a custom control the standard code for the override would be to pass gestures below by calling `base` then processing at the current level. You might choose to do it differently according to your needs.

The engine is designed to pass the ending gestures to those who already returned "consumed" for preceding gestures even if the following gestures are out of their hitbox.

__More docs will be added on this matter.__


### Caching System

_!_ Without caching animations going beyond simple wouldn't be possible. 
Caching makes complicated processing needed just once for layout calculation 
and then the caching result is redrawn on every frame.

Caching is controlled using a property `UseCache` of the following type:

```csharp
public enum SkiaCacheType
{
    /// <summary>
    /// True and old school
    /// </summary>
    None,

    /// <summary>
    /// Create and reuse SKPicture. Try this first for labels, svg etc. 
    /// Do not use this when dropping shadows or with other effects, better use Bitmap. 
    /// </summary>
    Operations,

    /// <summary>
    /// Will use simple SKBitmap cache type, will not use hardware acceleration.
    /// Slower but will work for sizes bigger than graphics memory if needed.
    /// </summary>
    Image,

    /// <summary>
    /// Using `Image` cache type with double buffering. Best for fast animated scenarios, this must be implemented by a specific control, not all controls support this, will fallback to 'Image' if anything.
    /// </summary>
    ImageDoubleBuffered,

    /// <summary>
    /// The way to go when dealing with images surrounded by shapes etc.
    /// The cached surface will use the same graphic context as your canvas.
    /// If hardware acceleration is enabled will try to cache as Bitmap inside graphics memory. Will fallback to simple Bitmap cache type if not possible. If you experience issues using it, switch to Memory cache type.
    /// </summary>
    GPU,
}

```

You should tweak your design caching to avoid unnecessary re-drawing of elements. 
The basic approach here is to cache small elements at some level. 
When you start using any kind of animations you should start using caching to max your FPS. You can check the __DemoApp__ for such examples.

#### Caching Tips:

* Cache shapes, svg and text as `Operations`.
* Prefer caching shadows and gradients as `Image` instead of `Operations`.
* Cache large static overlays as `GPU`, large static blocks as `Image`.
* For dynamically changing controls consider `ImageDoubleBuffered`, it consumes double the memory as `Image` but doesn't slow down rendering: you would see the latest prepared cache until the actual state wouldn't finish rendering itsself to a hidden cache layer in background.
* Do not include controls cached with `GPU` inside controls that use different type of cache, except for `Disabled`, will get a native crash otherwise.

#### Loaded Images

We are using the __EasyCaching.InMemory__ library for caching loaded bitmaps. Its impact can much be seen when using recycled cells inside a scroll. 
_todo add options and link to ImageLoader and SkiaImage docs_

_!_ When using images inside dynamic scenes, like a templated stack with scroll or other 
you should try to set the image cache to `Image` this would most probably climb your fps.
This is due to the fact that image sources are usually of the wrong size and they need processing 
before being drawn. When using `Image` cache the image would be processed only once and 
then just redrawn.

### Transforms

* TranslationX
* TranslationY
* ScaleX
* ScaleY
* Rotation
* CameraAngleX
* CameraAngleY
* CameraAngleZ
* SkewX
* SkewY
* Perspective1
* Perspective2


### Animations

One would create animations and effects using animators. Animators are attached to controls, but technically register themselves at the root canvas level and their code is executed on every rendering frame. If the canvas is not redrawing then animators will not be executed.
When the canvas has a registered animator running it would constantly force re-drawing itself until all animators are stopped.

There are two types of animators: 
* __Animators__ are executed before the drawing, so you can move and transform elements before the are rendered on the canvas.
* __PostAnimators__ are executed after their parent element was drawn, so you can paint an effect over the existing result, or execute any other post-drawing logic.

### Layout

For initial items positioning you would be using `SkiaLayout`. 
Its `Absolute` layout type is already built-in inside every skia control..

You can position your children using familiar properties like
`HorizonalOptions`, `VerticalOptions`, `Margin`, parent `Padding`,
`WidthRequest`, `HeightRequest`,`MinimumWidthRequest`, `MinimumHeightRequest` 
and additional `MaximumWidthRequest`,  `MaximumHeightRequest`, `HorizontalFillRatio`, `VerticalFillRatio` and `LockRatio` properties.

* `LockRatio` will be used to calculate the width when the height is set or vice versa. If it's above 0 the max value will be applied, if it's below 0 the min value will be applied. If it's 0 then the ratio will be ignored.
* `HorizontalFillRatio`, `VerticalFillRatio` will let you take a percent and the available size if you don't set a numeric request. For example if `HorizontalFillRatio` is set to 0.5 you will take 50% of the available width, at the same time being able to align the control at start, center or at the and in a usual way.
* `HorizontalPositionOffsetRatio` and `VerticalPositionOffsetRatio` let you offset the item after it was aligned by applying a ratio to its computed size. Defaults are 0.0. For example, HorizontalPositionOffsetRatio of -0.5 will offset the item by -Width/2, practically centering it horizontally along the current x-position. Works almost like translation but relatively to the current size.

For dynamic positioning or other precise cases use `TranslationX` and `TranslationY`.

When you need to layout children in a more arranged way you will want to wrap them with a `SkiaLayout`
of different `LayoutType` : `Grid`, `Colum`, `Row` and others.

Layout supports `ItemTemplate` for most of layout types.

For grid, we have so useful feats like, instead of using `RowDefinitions="32,32,32,32"` you can just do i nice `DefaultRowDefinition="32"`.

Some differences from Xamarin.Forms/Maui to notice:

* Layouts as other controls come with `HorizontalOptions` and `VerticalOptions` as `Start` and not `Fill` by default, so if your children do not request a size explicitly, 
the parent layout will not take any space at all unless you ask it to `Fill` the desired dimension, for example a `Column` needs its  `HorizontalOptions` to be `Fill` or specified explicitly.

* Actually for performance reasons in `Column` and `Row` layouts children cannot have `Fill`, `End` and `Center` in the direction of the layout, only `Start`. For example, if you have a `Column` children must have `VerticalOptions` set to `Start`. When you still need these options for children please use `Absolute` or `Grid` layouts.

_!_ Layouts `Column` and `Row`, whether templated or not, 
will always check if child is out of the visible screen bounds and avoid rendering it in that case.
That is especially useful when the layout is inside a `SkiaScroll`, this way we always render 
only the visible part. You can tweak this but setting a `SkiaLayout` property `HiddenAmountToRender` in points, how many of the hidden amount outside the visible bounds should still be rendered. 
This system ensures that you can have an infinite-size layout inside a scroll and it will work just fine drawing only the visible area.
At the same time if you want a `SkiaScroll` to <s>lye</s> communicate to its content that everything is visible on the screen you can set its `VirtualizationEnabled="False"`.

_!_ You can absolutely use the `Margin` property in a usual Maui way. In case if you would need to bind a specific margin you can switch to 
using `MarginLeft`, `MarginTop`, `MarginRight`, `MarginBottom` properties, -1.0 by default, 
if set they will override the specific value from `Margin`, and the result would be accessible via `Margins` read-only bindable static property.  
Even more, sometimes you might want to bind your code to `AddMarginTop`, `AddMarginLeft`, `AddMarginRight`, `AddMarginBottom`..  
When designing custom controls please use `Margins` property to read the final margin value.

#### Loading sources

SkiaImage, SkiaLottie and other controls that have a `Source` property, can load data from web, from bundled resources and from native file system.
The conventions are the following:
- if a web url is detected the source is loaded from web
- if a file path starts with file:// it will be loaded from native file system
- otherwise will try to load from bundled resources with root folder 'Resources\Raw'. 

The example below will load animation from `Resources\Raw\Lottie\Loader.json`.

```xml  
            <draw:SkiaLottie
                InputTransparent="True"
                AutoPlay="True"
                ColorTint="{StaticResource ColorPrimary}"
                HorizontalOptions="Center"
                ="1"
                Opacity="0.85"
                Repeat="-1"
                Source="Lottie/Loader.json"
                Tag="Loader"
                VerticalOptions="Center"
                WidthRequest="56" />
```


#### Enhanced usage

When dealing with subviews in code behind you could typically use two ways. 

Example for adding a subview:
 
* Use method `SetParent` passing new parent. In this case parent layout will not be invalidated, use this for optimized logic when you know what you are doing. You can mainly use this way when just constructing parent, knowing it will be measured at start anyway.
* Call parent's method `AddSubView` passing subview Parent's layout will be invalidated, and OnChildAdded will be called on parent.
* When working with `Children` property use `Add` method, it will set `Views` to a new instance of appropriate collection, and call `AddSubView` for each item.

For removing a subview the usual options would be:

* Call `SetParent` passing null, for soft removal.
* Call parent's method `RemoveSubView` passing subview`.Parent`'s layout will be invalidated, and OnChildRemoved will be called on parent.	
* When working with `Children` property use `Remove` method, it will set `Views` to a new instance of appropriate collection, and call `RemoveSubView` for each item.
 
#### In Deep

When XAML would be constructed it would fill `Children` property with views, this property is for high-level access. `Views` 

Will be then filled internally. When you add or remove items in `Children` methods `AddSubView` and `RemoveSubView` will be called for managing `Views`.


 

## Maui Views

### `Canvas`

#### _Events_:

__`WillDraw`__ 

__`WillFirstTimeDraw`__ 

__`WasDrawn`__ 

#### _Methods_: 

__`TakeScreenShot(Action<SKImage> callback)`__ Takes screenshot on next draw. Passes an `SKImage`, can be null. If you need an `SKBitmap` use `SKBitmap.FromImage` on it.

#### _Properties_:

__`Surface`__ 


### `SkiaShell`

The usage is almost the same as the standard Maui Shell, 
with some extra features.

`SkiaShell` is derived from `FastShell` that uses Maui interfaces 
and implements methods for standard maui navigation, 
then adds features to be able to navigate inside the Canvas.

Some additional features to be mentioned are actions that can be executed for specific routes.
code example:

```csharp  
RegisterRoute("profile", typeof(ScreenUserProfile));

RegisterActionRoute("settings", () =>
{
    //select settings tab
    this.NavigationLayout.SelectedIndex = 4;
});

```

#### Usage

Please see the demo.

## Drawn Controls

### `SkiaControl`

Base drawn element, derived from Maui `Element` to assure basic Maui Xaml compatibility, could derive from a `BindableObject` if Maui would provide public interfaces for that matter.

#### _Properties_:

__`LockRatio`__ a numeric value that will be used to calculate the width when the height is set or vice versa. If it's above 0 the max value will be applied, if it's below 0 the min value will be applied. If it's 0 then the ratio will be ignored.

Example 1:
```xml  
  <draw:SkiaShape
                        LockRatio="1"
                        WidthRequest="40" />
```
HeightRequest wasn't specified but this control will request 40 by 40 pts.

Example 2:
```xml  
  <draw:SkiaShape
	LockRatio="-1"
	HeightRequest="30"
	WidthRequest="40" />
```
This control will request 30 by 30 pts.

__`VisualEffects`__ a list of effects you can attach to affect to change how your controls is drawing. 
It's easy to create effects for dirrerent tasks, would it be making your control black and white or animate it with conditions.
Actually you can attach different types of effects to every control:
* One effect changing the color filter, impements `IColorEffect`.
* One effect changing the image filter, impements `IImageEffect`.
* Any number of effects affecting the rendering before the controls drawing is finalized and eventually saved to cache, implementing `IRenderEffect`, applied in chain.
* Any number of effects implementing `IStateEffect`, thoses can be used to change your control state, animate etc.
* One post renderer, impements `IPostRendererEffect`, this one will render the cache in its own way, if you'd want to apply a shader etc.
* Any effect can implement `ISkiaGestureProcessor`, to become gestures-aware.  

### `SkiaScroll`

`SkiaScroll` is a scrollable container that supports virtualization and recycling of its children.

If you include a `SkiaLayout` inside a `SkiaScroll` only visible on screen items will be rendered.

If the include a `SkiaLayout` that uses `ItemTemplate` this combination will automatically become virtualized and you will get sort of a CollectionView with recycled cells at your disposal. It is a good practice to use it for long lists of items.

#### _Properties_:

__`Orientation`__ a value of type `ScrollOrientation` that can be `Vertical` or `Horizontal`.

__`FrictionScrolled`__  Use this to control how fast the scroll will decelerate. Values 0.1f - 0.3f are the best, default is 0.1f.

__`IgnoreWrongDirection`__  Will ignore gestures of the wrong direction, for example, if this Orientation is Horizontal will ignore gestures with vertical direction velocity. Might want to set it to true when you have a horizontal scroll inside a vertical scroll, this will let the parent scroll start scrolling vertically even if the gesture started inside its horizontal scroll child.

### `SkiaLayout`

`SkiaLayout` is a container that supports various layout types: `Absolute`, `Grid`, `Row`, `Column`, and others. 

It also supports virtualization and recycling of its children with `ItemTemplate` property.

Controls inside templated `SkiaLayout` can implement `ISkiaCell` interface to eventually receive information about their state:
* `OnAppearing`
* `OnDisapearing`
* `OnScrolled`

This lets one to create custom controls that can react to scrolling and other events with animations etc.

### `SkiaShape`

`SkiaShape` is a base class for all shapes. You could fill it, stroke, drop shadows, apply gradients, and even clip other controls with it.

### `SkiaImage`

`SkiaImage` is a control that renders images. It can't apply filters and transformations.



### `SkiaSvg`

`SkiaSvg` is a control that renders svg files. It can't tint the svg with a color or gradient, and apply some transforms to it.


### `SkiaLabel`

A multi-line label fighting for his place under the sun.

#### _Properties_:

__`FontWeight`__ a numeric value used in case you have properly registered your fonts to support weights. 
You can use your font the usual Maui way but in the case of custom font files used from resources you might want to register them, using the following example:
```csharp
.ConfigureFonts(fonts =>
{
   fonts.AddFont("Gilroy-Regular.ttf", "FontText", FontWeight.Regular);
   fonts.AddFont("Gilroy-Medium.ttf", "FontText", FontWeight.Medium);
});
```
Now if you set the `FontWeight` to `500` the control will use the `Gilroy-Medium.ttf` file.
This might come in very handy when your Figma design shows you to use this weight and you want just to pass it over to SkiaLabel.

 __`HorizontalTextAlignment`__  : 
 ```csharp
public enum DrawTextAlignment
{
	Start,
	Center,
	End,
	FillWords,
	FillWordsFull,
	FillCharacters,
	FillCharactersFull,
}
```


### `SkiaLottie`

`SkiaLottie` is a control that renders Lottie files. It can even tint some colors inside your animation!

### `SkiaRive`

Actually for Windows only, this plays and controls Rive animation files. Other platforms will be added soon, poke if you would like to help binding some c++;

### SkiaHoverMask

A control deriving from SkiaShape that can be used to create hover effects. 
It will render a mask over its children when hovered, think of it as an inverted shape.

## MAUI Compatibility Limitations

* Binding RelativeSource with FindAncestorBindingContext not working yet.

## Previously

* Breaking ``ISkiaCell`` changed, check out demo app FestCellWithBanner new usage.
* Android loop changed along with its OpenGL renderer.
* Gestures fix: will not trigger Tapped after raised Longpressing.
* Nuget 1.2.3.6

* `SkiaScroll` fixed scrolling vertically + horizontally at the same time (`Orientation="Both"`).
* `SkiaShell` Navigated and Navigating events now report popups too.
* `SkiaMediaImage` a subclassed `SkiaImage` for displaying any kind of images (image/animated gif/more..)
* `SkiaGif` a dedicated lightweight GIF-player with playback control properties. See Sandbox project.
* Fixed gestures inside `ImageCacheComposite` cache. 
* Fixed bug `SkiaShell` navigation gets locked when spamming popups.
* Layout optimizations.
* Nuget 1.2.3.4
* 
* `SkiaLayout` Column/Row uses 2 layout passes when needed, can now use full alignment  options inside.
* Critical fixes for Release builds.

* New: SvgSpan for SkiaLabel.
* Critical fix for Release builds native crash while using ImageDoubleBuffered cache.
* Fixes for: gestures, SkiaMarkdownLabel, SkiaDrawer, SkiaLayout, Canvas and more.
* Added real published apps in the repo Readme.

* In Sandbox project we have new demos with features: Markdown, Xaml2Pdf. Note we could soon write tagged Pdfs when SkiaSharp appropriate nuget comes out.
* `SkiaMarkdownLabel` now more colors options, supports bullet/numbered lists and code.
* Added `Files` native helper to framework Features, used for Xaml2Pdf Sandbox demo.
* Fixes for the new SkiaLayout type `Wrap`. Awesome for Tags/Chips etc. Used in Sandbox for main screen buttons.
  Fixes for the new `ImageComposite` cache type that redraws changed areas only, gamechanger for large scrolls with many items.
* Canvas auto-size fixes, `ImageDoubleBuffered` cache fixes and many others..
  
* `SkiaRadioButton` can be auto-groupped by parent or by group name, seen in Sandbox project.
* `SkiaLottie` got new properties `DefaultFrame`, `DefaultFrameWhenOn` and `IsOn`, to be able to easily switch the look between two states. In the future it might be replaced with State + States with default frames ranges for them, looking at how **Rive** is designed for this mater.
* `SkiaMarkdownLabel` can now correctly render `code`.
* `SkiaBackdrop` can now use canvas or context background while inside cache, control with UseContext` property.
* `SkiaShape` became a `ContentLayout`, its `CornerRadius` property changed to MAUI `CornerRadius` struct type.
* Derived controls that bring additional dependencies moved into separate assemblies/packages: 
-- DrawnUi.Maui.Camera
-- DrawnUi.Maui.MapsUi
-- DrawnUi.MauiGraphics
-- DrawnUi.Maui.Rive
* Fixed random crashes due to disposed items when using `ImageDoubleBuffered` cache type.

As SkiaSharp3 stable version is expected towards the end of the year we will have DrawnUI skia2 version updated constantly and a skia3-preview version available in parallel.  
New DrawnUI features/fixes will apply to both skia2/skia3 versions. You can manage how this project builds using `<UseSkiaSharp3>true</UseSkiaSharp3>`.

* SkiaSharp v3 __preview__ 21 [nuget](https://www.nuget.org/packages/AppoMobi.Maui.DrawnUi) with hardware accelerated Windows canvas, CPU-shaders, [latest SKSL syntax](https://shaders.skia.org/) etc.
* SkiaSharp v2 [nuget](https://www.nuget.org/packages/AppoMobi.Maui.DrawnUi).
* Every SkiaControl has a new `VisualEffects` property, you can blur/tint/apply other effects to anything in your app, see Sandbox app.
* `Maui.Graphics` support, can reuse your existing works inside drawn layouts/controls, see Sandox app.
* FPS is now mainly display-synched (choreographer/display link..)
* Hardware accelerated iOS Metal has its `SkiaBackdrop` fixed, blur/affect anything below it (demo app bottom tabs).
* MauiElement and drawn entry/editor get more fixes again.
* SkiaImage resizing quality improved and support options.
* SkiaImageManager preload functions added.
* Dropped support for .net7
* Too many other feats and fixes, hoping to have time for docs one day..

* WIndows platform FPS stabilized.
* .NET 8 support.

### SkiaSharp version 3

https://github.com/mono/SkiaSharp/pull/2547

### Docs under construction


 
