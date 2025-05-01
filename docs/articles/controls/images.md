# Image Controls

DrawnUi.Maui provides powerful image controls for high-performance image rendering with advanced features like effects, transformations, and sophisticated caching. This article covers the image components available in the framework.

## SkiaImage

SkiaImage is the core image control in DrawnUi.Maui, providing efficient image loading, rendering, and manipulation capabilities with direct SkiaSharp rendering.

### Basic Usage

```xml
<DrawUi:SkiaImage
    Source="image.png"
    Aspect="AspectFit"
    HorizontalOptions="Center"
    VerticalOptions="Center"
    WidthRequest="200"
    HeightRequest="200" />
```

### Key Properties

| Property | Type | Description |
|----------|------|-------------|
| `Source` | ImageSource | Source of the image (URL, file, resource, stream) |
| `Aspect` | TransformAspect | How the image scales to fit (AspectFit, AspectFill, etc.) |
| `HorizontalAlignment` | DrawImageAlignment | Horizontal positioning of the image |
| `VerticalAlignment` | DrawImageAlignment | Vertical positioning of the image |
| `RescalingQuality` | SKFilterQuality | Quality of image rescaling |
| `LoadSourceOnFirstDraw` | bool | Whether to defer loading until first render |
| `PreviewBase64` | string | Base64 encoded preview image to show while loading |
| `ImageBitmap` | LoadedImageSource | Loaded image source (internal representation) |
| `AddEffect` | SkiaImageEffect | Built-in image effect (None, Sepia, Tint, etc.) |
| `ColorTint` | Color | Tint color for image effect |
| `Brightness` | double | Adjusts image brightness |
| `Contrast` | double | Adjusts image contrast |
| `Saturation` | double | Adjusts image saturation |
| `Blur` | double | Applies blur effect |
| `Gamma` | double | Adjusts gamma |
| `Darken` | double | Darkens the image |
| `Lighten` | double | Lightens the image |
| `ZoomX`/`ZoomY` | double | Zoom/scaling factors |
| `HorizontalOffset`/`VerticalOffset` | double | Offset for image position |
| `SpriteWidth`/`SpriteHeight` | double | Sprite sheet cell size |
| `SpriteIndex` | int | Index of sprite to display |

> **Note:** The `UseCache` property is not directly on SkiaImage, but caching is handled by the SkiaControl base class or internally. You can set `Cache` on SkiaImage for caching strategies (e.g., `Cache="Image"`).

> **Note:** The `VisualEffects` property is inherited from SkiaControl. You can use `<DrawUi:SkiaControl.VisualEffects>` in XAML to apply effects like drop shadow or color presets.

### Aspect Modes

The `Aspect` property controls how the image is sized and positioned within its container. This is a critical property for ensuring that your images display correctly while maintaining their proportions when appropriate.

#### Available Aspect Modes

| Aspect Mode | Description | Visual Effect |
|-------------|-------------|---------------|
| `AspectFit` | Maintains aspect ratio while ensuring the entire image fits within available space | May leave empty space at sides or top/bottom |
| `AspectFill` | Maintains aspect ratio while filling the entire space | May crop portions of the image that don't fit |
| `Fill` | Stretches the image to fill the entire space | May distort the image proportions |
| `Center` | Centers the image at its original size | May clip or leave empty space depending on size |
| `TopLeft` | Positions the image at original size in the top-left corner | May clip or leave empty space |
| `TopCenter` | Positions the image at original size at the top-center | May clip or leave empty space |
| `TopRight` | Positions the image at original size in the top-right corner | May clip or leave empty space |
| `CenterLeft` | Positions the image at original size at the center-left | May clip or leave empty space |
| `CenterRight` | Positions the image at original size at the center-right | May clip or leave empty space |
| `BottomLeft` | Positions the image at original size in the bottom-left corner | May clip or leave empty space |
| `BottomCenter` | Positions the image at original size at the bottom-center | May clip or leave empty space |
| `BottomRight` | Positions the image at original size in the bottom-right corner | May clip or leave empty space |
| `ScaleDown` | Like AspectFit, but only scales down, never up | Small images remain at original size |
| `AspectRatioWidth` | Maintains aspect ratio as determined by width | Sets the height based on image aspect ratio |
| `AspectRatioHeight` | Maintains aspect ratio as determined by height | Sets the width based on image aspect ratio |
| `AspectCover` | Same as AspectFill | Alternative name for AspectFill |

#### Examples and Visual Guide

```xml
<!-- Maintain aspect ratio, fit within bounds -->
<DrawUi:SkiaImage Source="image.png" Aspect="AspectFit" />
```
This ensures the entire image is visible, possibly with letterboxing (empty space) on the sides or top/bottom.

```xml
<!-- Maintain aspect ratio, fill bounds (may crop) -->
<DrawUi:SkiaImage Source="image.png" Aspect="AspectFill" />
```
This fills the entire control with the image, possibly cropping parts that don't fit. Great for background images or thumbnails.

```xml
<!-- Stretch to fill bounds (may distort) -->
<DrawUi:SkiaImage Source="image.png" Aspect="Fill" />
```
This stretches the image to fill the control exactly, potentially distorting the image proportions.

```xml
<!-- Center the image without scaling -->
<DrawUi:SkiaImage Source="image.png" Aspect="Center" />
```
This displays the image at its original size, centered in the control. Parts may be clipped if the image is larger than the control.

#### Combining Aspect and Alignment

You can combine `Aspect` with `HorizontalAlignment` and `VerticalAlignment` for precise control:

```xml
<!-- AspectFit with custom alignment -->
<DrawUi:SkiaImage 
    Source="image.png" 
    Aspect="AspectFit"
    HorizontalAlignment="Start"
    VerticalAlignment="End" />
```
This would fit the image within bounds while aligning it to the bottom-left corner of the available space.

#### Choosing the Right Aspect Mode

- For user photos or content images: `AspectFit` ensures the entire image is visible
- For backgrounds or covers: `AspectFill` ensures no empty space is visible
- For icons that need to fill a specific area: `Fill` may be appropriate
- For pixel-perfect icons: `Center` or other position-specific modes maintain original dimensions
- For responsive layouts: `AspectRatioWidth` or `AspectRatioHeight` can help maintain proportions while adapting to container changes

### Image Alignment

Control the alignment of the image within its container:

```xml
<DrawUi:SkiaImage
    Source="image.png"
    Aspect="AspectFit"
    HorizontalAlignment="End"
    VerticalAlignment="Start" />
```

This will position the image at the top-right of its container.

### Image Effects

SkiaImage supports various built-in effects through the `AddEffect` property:

```xml
<!-- Apply a sepia effect -->
<DrawUi:SkiaImage
    Source="image.png"
    AddEffect="Sepia" />

<!-- Apply a tint effect -->
<DrawUi:SkiaImage
    Source="image.png"
    AddEffect="Tint"
    ColorTint="Red" />

<!-- Apply grayscale effect -->
<DrawUi:SkiaImage
    Source="image.png"
    AddEffect="BlackAndWhite" />
```

### Image Adjustments

Fine-tune image appearance with various adjustment properties:

```xml
<DrawUi:SkiaImage
    Source="image.png"
    Brightness="1.2"
    Contrast="1.1"
    Saturation="0.8"
    Blur="2" />
```

### Advanced Effects

For more complex effects, use the VisualEffects collection:

```xml
<DrawUi:SkiaImage Source="image.png">
    <DrawUi:SkiaControl.VisualEffects>
        <DrawUi:DropShadowEffect 
            Blur="8"
            X="2"
            Y="2"
            Color="#80000000" />
        <DrawUi:ChainColorPresetEffect Preset="Sepia" />
    </DrawUi:SkiaControl.VisualEffects>
</DrawUi:SkiaImage>
```

### Sprite Sheets

SkiaImage supports sprite sheets for displaying a single sprite from a larger image:

```xml
<DrawUi:SkiaImage
    Source="sprite-sheet.png"
    SpriteWidth="64"
    SpriteHeight="64"
    SpriteIndex="2" />
```

This shows the third sprite (index 2) from the sprite sheet, assuming each sprite is 64x64 pixels.

### Preview Images

Show a low-resolution placeholder while loading the main image:

```xml
<DrawUi:SkiaImage
    Source="https://example.com/large-image.jpg"
    PreviewBase64="data:image/png;base64,iVBORw0KGgoAA..."
    Aspect="AspectFit" />
```

### Loading Options

Control how and when images are loaded:

```xml
<!-- Immediate loading (default) -->
<DrawUi:SkiaImage
    Source="image.png"
    LoadSourceOnFirstDraw="False" />

<!-- Deferred loading (load when first rendered) -->
<DrawUi:SkiaImage
    Source="image.png"
    LoadSourceOnFirstDraw="True" />
```

### Caching Strategies

Optimize performance with various caching options:

```xml
<!-- Use double-buffered image caching (good for changing content) -->
<DrawUi:SkiaImage
    Source="image.png"
    UseCache="ImageDoubleBuffered" />

<!-- Use simple image caching (good for static content) -->
<DrawUi:SkiaImage
    Source="image.png"
    UseCache="Image" />

<!-- Cache drawing operations rather than bitmap (memory efficient) -->
<DrawUi:SkiaImage
    Source="image.png"
    UseCache="Operations" />

<!-- No caching (for frequently changing images) -->
<DrawUi:SkiaImage
    Source="image.png"
    UseCache="None" />
```

### Handling Load Events

You can respond to image load success or failure in code-behind:

```csharp
public MainPage()
{
    InitializeComponent();
    
    MyImage.OnSuccess += (sender, e) => {
        // Image loaded successfully
    };
    
    MyImage.OnError += (sender, e) => {
        // Image failed to load
        // e.Contains the error information
    };
}
```

## Image Management

### SkiaImageManager

DrawnUi.Maui includes a powerful image management system through the `SkiaImageManager` class. This provides centralized image loading, caching, and resource management.

#### Preloading Images

Preload images to ensure they're ready when needed:

```csharp
// Preload a single image
await SkiaImageManager.Instance.PreloadImage("Images/my-image.jpg");

// Preload multiple images
await SkiaImageManager.Instance.PreloadImages(new List<string> 
{
    "Images/image1.jpg",
    "Images/image2.jpg",
    "Images/image3.jpg"
});
```

#### Managing Memory Usage

Configure the image manager for optimal memory usage:

```csharp
// Enable bitmap reuse for better memory usage
SkiaImageManager.Instance.ReuseBitmaps = true;

// Set the maximum cache size (in bytes)
SkiaImageManager.Instance.MaxCacheSize = 50 * 1024 * 1024; // 50MB

// Clear unused cached images
SkiaImageManager.Instance.ClearUnusedImages();

// Clear all cached images
SkiaImageManager.Instance.ClearAll();
```

## Advanced Usage

### Loading from Base64

Load images directly from base64 strings:

```csharp
var base64String = "data:image/png;base64,iVBORw0KGgoAA...";
myImage.SetFromBase64(base64String);
```

### Applying Transformations

Apply transformations to the displayed image:

```xml
<DrawUi:SkiaImage
    Source="image.png"
    ZoomX="1.2"
    ZoomY="1.2"
    HorizontalOffset="10"
    VerticalOffset="-5" />
```

### Creating Images in Code

Create and configure SkiaImage controls programmatically:

```csharp
var image = new SkiaImage
{
    Source = "Images/my-image.jpg",
    LoadSourceOnFirstDraw = false,
    Aspect = Aspect.AspectFit,
    RescalingQuality = SKFilterQuality.Medium,
    AddEffect = SkiaImageEffect.Sepia,
    WidthRequest = 200,
    HeightRequest = 200,
    HorizontalOptions = LayoutOptions.Center,
    VerticalOptions = LayoutOptions.Center
};

myLayout.Children.Add(image);
```

## Performance Considerations

### Optimization Tips

1. **Image Size**
   - Resize images to their display size before including in your app
   - Use compressed formats (WebP, optimized PNG/JPEG) when possible
   - Consider providing different image sizes for different screen densities

2. **Caching**
   - Use `UseCache="Image"` for static images that don't change
   - Use `UseCache="ImageDoubleBuffered"` for images that change occasionally
   - Use `UseCache="Operations"` for images with effects but static content
   - Use `UseCache="None"` only for frequently changing images

3. **Loading Strategy**
   - Use `LoadSourceOnFirstDraw="True"` for off-screen images
   - Preload important images with SkiaImageManager.PreloadImages()
   - Provide preview images with `PreviewBase64` for large remote images

4. **Rendering Quality**
   - Set appropriate `RescalingQuality` based on your needs:
     - `None`: Fastest but lowest quality
     - `Low`: Good balance for scrolling content
     - `Medium`: Good for static content
     - `High`: Best quality but slowest (use sparingly)

5. **Memory Management**
   - Enable bitmap reuse with `SkiaImageManager.Instance.ReuseBitmaps = true`
   - Set reasonable cache limits with `MaxCacheSize`
   - Call `ClearUnusedImages()` when appropriate

### Examples of Optimized Image Loading

#### For Lists/Carousels

```xml
<DrawUi:SkiaImage
    Source="{Binding ImageUrl}"
    LoadSourceOnFirstDraw="True"
    UseCache="ImageDoubleBuffered"
    RescalingQuality="Low"
    Aspect="AspectFill" />
```

#### For Hero/Cover Images

```xml
<DrawUi:SkiaImage
    Source="{Binding CoverImage}"
    PreviewBase64="{Binding CoverImagePreview}"
    LoadSourceOnFirstDraw="False"
    UseCache="Image"
    RescalingQuality="Medium"
    Aspect="AspectFill" />
```

#### For Image Galleries

```xml
<DrawUi:SkiaScroll Orientation="Horizontal">
    <DrawUi:SkiaLayout LayoutType="Row" Spacing="10">
        <!-- Images that are initially visible -->
        <DrawUi:SkiaImage
            Source="{Binding Images[0]}"
            LoadSourceOnFirstDraw="False"
            UseCache="Image"
            WidthRequest="300"
            HeightRequest="200" />
            
        <!-- Images that may be scrolled to -->
        <DrawUi:SkiaImage
            Source="{Binding Images[1]}"
            LoadSourceOnFirstDraw="True"
            UseCache="Image"
            WidthRequest="300"
            HeightRequest="200" />
            
        <!-- More images... -->
    </DrawUi:SkiaLayout>
</DrawUi:SkiaScroll>
```