# Button Controls

DrawnUi provides highly customizable button controls with platform-specific styling.

## SkiaButton

`SkiaButton` is a versatile button control with support for different button styles and platform-specific appearance.

### Basic Usage

```xml
<draw:SkiaButton
    Text="Click Me"
    WidthRequest="120"
    HeightRequest="40"
    BackgroundColor="Blue"
    TextColor="White"
    CornerRadius="8"
    Clicked="OnButtonClicked" />
```

### Button Style Types

SkiaButton supports multiple style variants through the `ButtonStyle` property:

- `Contained`: Standard filled button with background color (default)
- `Outlined`: Button with outline border and transparent background
- `Text`: Button with no background or border, only text

```xml
<draw:SkiaButton
    Text="Outlined Button"
    ButtonStyle="Outlined"
    BackgroundColor="Blue"
    TextColor="Blue" />
```

### Platform-Specific Styling

Set the `ControlStyle` property to apply platform-specific styling:

- `Platform`: Automatically selects the appropriate style for the current platform
- `Cupertino`: iOS-style button
- `Material`: Android Material Design button
- `Windows`: Windows-style button

```xml
<draw:SkiaButton
    Text="iOS Style"
    ControlStyle="Cupertino" />
```

### Elevation

Buttons can have elevation (shadow) effects:

```xml
<draw:SkiaButton
    Text="Elevated Button"
    ElevationEnabled="True" />
```

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Text` | string | The text displayed on the button |
| `TextColor` | Color | The color of the button text |
| `BackgroundColor` | Color | The background color of the button |
| `CornerRadius` | float | The corner radius of the button |
| `ButtonStyle` | ButtonStyleType | The button style (Contained, Outlined, Text) |
| `ControlStyle` | PrebuiltControlStyle | The platform-specific style |
| `ElevationEnabled` | bool | Whether the button has a shadow effect |
| `TextCase` | TextTransform | The text case transformation (None, Uppercase, Lowercase) |

### Events

- `Clicked`: Raised when the button is clicked/tapped
- `Pressed`: Raised when the button is pressed down
- `Released`: Raised when the button is released