# Switches and Toggles

DrawnUi provides toggle controls with platform-specific styling, including switches and checkboxes.

## SkiaSwitch

`SkiaSwitch` is a toggle control styled according to platform conventions, similar to an on/off switch.

### Basic Usage

```xml
<draw:SkiaSwitch
    IsToggled="false"
    WidthRequest="50"
    HeightRequest="30"
    ColorFrameOff="Gray"
    ColorFrameOn="Green"
    ColorThumbOff="White"
    ColorThumbOn="White"
    Toggled="OnSwitchToggled" />
```

### Platform-Specific Styling

Set the `ControlStyle` property to apply platform-specific styling:

- `Platform`: Automatically selects the appropriate style for the current platform
- `Cupertino`: iOS-style switch with pill-shaped track
- `Material`: Android Material Design switch
- `Windows`: Windows-style switch

```xml
<draw:SkiaSwitch
    ControlStyle="Cupertino"
    IsToggled="true" />
```

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsToggled` | bool | Whether the switch is toggled on or off |
| `ColorFrameOn` | Color | The color of the track when toggled on |
| `ColorFrameOff` | Color | The color of the track when toggled off |
| `ColorThumbOn` | Color | The color of the thumb when toggled on |
| `ColorThumbOff` | Color | The color of the thumb when toggled off |
| `ControlStyle` | PrebuiltControlStyle | The platform-specific style |
| `IsAnimated` | bool | Whether state changes are animated |

### Events

- `Toggled`: Raised when the switch is toggled on or off

## SkiaCheckbox

`SkiaCheckbox` is a toggle control styled as a checkbox with platform-specific appearance.

### Basic Usage

```xml
<draw:SkiaCheckbox
    IsToggled="false"
    WidthRequest="24"
    HeightRequest="24"
    ColorFrameOff="Gray"
    ColorFrameOn="Blue"
    ColorThumbOff="Transparent"
    ColorThumbOn="White"
    Toggled="OnCheckboxToggled" />
```

### Platform-Specific Styling

Like SkiaSwitch, SkiaCheckbox supports platform-specific styling through the `ControlStyle` property.

### Properties

SkiaCheckbox shares most properties with SkiaSwitch, both inheriting from SkiaToggle.

## SkiaToggle

`SkiaToggle` is the base class for toggle controls. You can use it to create custom toggle controls with similar behavior to switches and checkboxes.

### Key Properties

| Property | Type | Description |
|----------|------|-------------|
| `IsToggled` | bool | Whether the control is toggled on or off |
| `DefaultValue` | bool | The default toggle state |
| `ColorFrameOn/Off` | Color | The color of the frame in each state |
| `ColorThumbOn/Off` | Color | The color of the thumb in each state |
| `IsAnimated` | bool | Whether state changes are animated |

### Events

- `Toggled`: Raised when the toggle state changes