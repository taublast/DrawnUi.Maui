# Porting Native to Drawn with DrawnUi

There can come a time when You feel that some complex parts of the app are not rendering the way You wish, or you cannot implement some UI with out-of-the box native controls. At the same time You want to stay with MAUI and not rewrite the app in something else.  
We can then replace chunks of our UI with drawn controls. Or for the whole app.

Why even bother?

* __You want complex layouts not to affect your app performance__  
_In some scenarios native layouts can be slower than drawn ones. Like 5 horses vs a car with a 5 horse power engine: for example, app has to handle 5 natives views instead of just 1 - the Canvas. Rasterized caching makes shadows and other heavy-duty elements never affect your performance._

* __Your designer gave you something to implement that pre-built controls can't handle__  
_DrawnUi is designed with freedom in mind, to be able to draw just about anything you can imagine. With direct access to canvas you can achieve exactly your unique result._

* __You want consistency across platforms__  
_On all platforms the rendering is done with same logic, make you certain that font, controls and layouts will render the same way._

* __You want to be in control__   
_DrawnUi is a lightweight open-source project that can be directly referenced and customized up to your app needs. When you meet a bug you can can hotfix it in the engine source code, and if you miss some property/control You can easily add them._

This guide will help you port your existing native controls to DrawnUi.

## Prerequisites

First please follow the Getting [Started guide](./getting-started.md) to setup your project for DrawnUi.

## The theory

To replace native controls with DrawnUi ones would take several steps: 

1. Put used images inside `Resources/Raw` folder.

2. Create copies of your existing views

3. Replace native views names with DrawnUi ones. 

4. Fix properties/event handlers mismatch

5. Optimize: add caching etc

## Native vs Drawn names table

There are some direct alternatives to native controls You can use. At the same time now that you can "draw" you controls You can create your own controls from scratch.  
You can also just place MAUI controls over the canvas if You need to stick with native, use `SkiaMauiElement` as wrapper for them.

| Native MAUI Control | DrawnUi Equivalent | Notes |
|---------------------|-------------------|-------|
| **Layout Controls** |
| `Frame` | `SkiaFrame` | Alias for SkiaShape with Rectangle type |
| `VerticalStackLayout` | `SkiaStack` | Alias for SkiaLayout type Column with horizontal Fill |
| `HorizontalStackLayout` | `SkiaRow` | Alias for SkiaLayout type Row |
| `AbsoluteLayout` | ❌ Do not use | See "Grid (single cell)" instead |
| `Grid` (single row/col) | `SkiaLayer` | Layering controls one over another with alignements |
| `Grid` | `SkiaGrid` | Grid supporting children alignements |
| `StackLayout` | `SkiaLayout` | Use Type="Column" or Type="Row" |
| `FlexLayout` | `SkiaLayout` | Use Type="Wrap" |
| `ScrollView` | `SkiaScroll` | Scrolling container with virtualization |
| **Text Controls** |
| `Label` | `SkiaLabel` | Renders unicode, spans support |
| `Label` (with markdown) | `SkiaMarkdownLabel` | For complex formatting, emojis, different languages, auto-finds fonts |
| **Input Controls** |
| `Entry` | `SkiaMauiEntry` | Native entry wrapped for DrawnUi |
| `Editor` | `SkiaMauiEditor` | Native editor wrapped for DrawnUi |
| **Button Controls** |
| `Button` | `SkiaButton` | Platform-specific styling via ControlStyle |
| **Toggle Controls** |
| `Switch` | `SkiaSwitch` | Platform-styled toggle switch |
| `CheckBox` | `SkiaCheckbox` | Platform-styled checkbox |
| `RadioButton` | `SkiaRadioButton` | Subclassed from SkiaToggle |
| **Image Controls** |
| `Image` | `SkiaImage` | High-performance image rendering |
| **Media Controls** |
| `Image` (media) | `SkiaMediaImage` | Subclassed SkiaImage for media |
| **Graphics Controls** |
| N/A | `SkiaSvg` | SVG rendering support |
| N/A | `SkiaGif` | Animated GIF support |
| N/A | `SkiaLottie` | Lottie animation support |
| **Shapes Controls** |
| `Frame` | `SkiaShape` | Container with border |
| `Border` | `SkiaShape` | Border decoration |
| `Ellipse` | `SkiaShape` | Ellipse shape |
| `Line` | `SkiaShape` | Line shape |
| `Path` | `SkiaShape` | Vector path shape |
| `Polygon` | `SkiaShape` | Polygon shape |
| `Polyline` | `SkiaShape` | Polyline shape |
| `Rectangle` | `SkiaShape` | Rectangle shape |
| `RoundRectangle` | `SkiaShape` | Rounded rectangle |
| **Navigation Controls** |
| `Shell` | `SkiaShell` | Navigation framework |
| `TabbedPage` | `SkiaViewSwitcher` | View switching functionality |
| **Scroll recycled cells** |
| `CollectionView` | `SkiaScroll`+`SkiaLayout` | Virtualized item collection |
| `ListView` | `SkiaScroll`+`SkiaLayout` | Simple item list |
| **Specialized Controls** |
| N/A | `SkiaDecoratedGrid` | Grid with shape drawing between cells |
| `CarouselView` | `SkiaCarousel` | Swipeable carousel with snap points |
| `SwipeView` | `SkiaDrawer` | Swipe actions on items |
| `RefreshView` | `LottieRefreshIndicator`/anything | Pull-to-refresh functionality |
| `ActivityIndicator` | `LottieRefreshIndicator`/anything | Loading/busy indicator |
| `Map` | `SkiaMapsUi` | Map control, SkiaMapsUi addon |
| N/A | `SkiaDrawer` | Swipe-in/out panel |
| N/A | `SkiaCamera` | Mlti-platform camera, SkiaCamera addon  |
| **Use native (wrap over canvas)** |
| `WebView` | `SkiaMauiElement`+`WebView` | wrap native over the canvas |
| `MediaElement` | `SkiaMauiElement`+`MediaElement` | Video/audio playback |
| `Picker` | `SkiaMauiElement`+`Picker` | Dropdown selection, create custom |
| `DatePicker` | `SkiaMauiElement`+`DatePicker` | Date selection control, create custom |
| `TimePicker` | `SkiaMauiElement`+`TimePicker` | Time selection control, create custom |
| `Slider` | `SkiaMauiElement`+`Slider`| Range input control, create custom |
| `Stepper` | `SkiaMauiElement`+`Slider` | Increment/decrement numeric input, create custom |
| `ProgressBar` | `SkiaMauiElement`+`Slider` | Progress indication, create custom |
| `TableView` | `SkiaMauiElement`+`TableView` | Grouped table display, create custom |
| `SearchBar` | ❌ Do not use, create custom | Search input with built-in styling |

## The practice

Let's take a look at a simple example of porting a native MAUI page to DrawnUi.

### Before: Native MAUI

Here's a typical MAUI page with native controls:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MyApp.MainPage">

    <ScrollView>
        <VerticalStackLayout Spacing="25" Padding="30,0">

            <Frame BackgroundColor="LightBlue"
                   Padding="20"
                   CornerRadius="10">
                <Label Text="Welcome to MAUI!"
                       FontSize="18"
                       HorizontalOptions="Center" />
            </Frame>

            <HorizontalStackLayout Spacing="10">
                <Image Source="icon.png"
                       WidthRequest="50"
                       HeightRequest="50" />
                <Label Text="Hello World"
                       FontSize="16"
                       VerticalOptions="Center" />
            </HorizontalStackLayout>

            <Button Text="Click me"
                    BackgroundColor="Blue"
                    TextColor="White"
                    CornerRadius="8"
                    Clicked="OnButtonClicked" />

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

### After: DrawnUi

Here's the same page converted to DrawnUi:

```xml
<draw:DrawnUiBasePage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                      xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                      xmlns:draw="http://schemas.appomobi.com/drawnUi/2023/draw"
                      x:Class="MyApp.MainPage">

    <draw:Canvas RenderingMode="Accelerated"
                 Gestures="Enabled"
                 HorizontalOptions="Fill"
                 VerticalOptions="Fill">

        <draw:SkiaScroll>
            <draw:SkiaStack Spacing="25" Padding="30,0">

                <draw:SkiaFrame BackgroundColor="LightBlue"
                                Padding="20"
                                CornerRadius="10">
                    <draw:SkiaLabel Text="Welcome to DrawnUi!"
                                    FontSize="18"
                                    HorizontalOptions="Center" />
                </draw:SkiaFrame>

                <draw:SkiaRow Spacing="10">
                    <draw:SkiaImage Source="icon.png"
                                    WidthRequest="50"
                                    HeightRequest="50" />
                    <draw:SkiaLabel Text="Hello World"
                                    FontSize="16"
                                    VerticalOptions="Center" />
                </draw:SkiaRow>

                <draw:SkiaButton Text="Click me"
                                 BackgroundColor="Blue"
                                 TextColor="White"
                                 CornerRadius="8"
                                 WidthRequest="120"
                                 HeightRequest="44"
                                 Clicked="OnButtonClicked" />

            </draw:SkiaStack>
        </draw:SkiaScroll>

    </draw:Canvas>

</draw:DrawnUiBasePage>
```

### Key Changes Made

1. **Root Container**: Changed from `ContentPage` to `draw:DrawnUiBasePage` just for keyboard support. You don't need it leave `ContentPage` as it is.
2. **Canvas**: Added `draw:Canvas` as the root drawing surface
3. **Layout Controls**:
   - `ScrollView` → `draw:SkiaScroll`
   - `VerticalStackLayout` → `draw:SkiaStack`
   - `HorizontalStackLayout` → `draw:SkiaRow`
   - `Frame` → `draw:SkiaFrame`
4. **Content Controls**:
   - `Label` → `draw:SkiaLabel`
   - `Image` → `draw:SkiaImage`
   - `Button` → `draw:SkiaButton`
5. **Button Sizing**: Added explicit `WidthRequest` and `HeightRequest` to button (DrawnUi buttons need explicit sizing)

### Code-Behind Changes

The code-behind remains mostly the same, but the event signature is slightly different:

```csharp
// Before (MAUI)
private void OnButtonClicked(object sender, EventArgs e)
{
    // Handle click
}

// After (DrawnUi)
private void OnButtonClicked(SkiaButton button, SkiaGesturesParameters args)
{
    // Handle click - note the different parameters
}
```

### Optimize: add caching

Imagine your page redrawing.. What could stay same if you redraw one element?

```xml
        <draw:SkiaScroll>
            <!--this is a small stack, just cache it in whole -->
            <!--"composite" will redraw only changed areas, for instance the clicked button,
            leaving other area raster unchaged -->
            <draw:SkiaStack Spacing="25" Padding="30,0" UseCache="ImageComposite">

            <!-- unchaged code -->

            </draw:SkiaStack>

            </draw:SkiaStack>
        </draw:SkiaScroll>
```
 
### Performance Benefits

After conversion, you'll get:
- **Better Performance**: Single canvas instead of multiple native views
- **Consistent Rendering**: Same appearance across all platforms
- **Advanced Caching**: Built-in rasterization and caching capabilities
- **Custom Drawing**: Ability to add custom graphics and effects

### Example with dynamic content

TODO point is with dynamic we need other type of caching, groupping controls into cached layers


