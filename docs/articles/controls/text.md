# Text Controls

DrawnUi.Maui offers powerful text rendering capabilities through its specialized text controls. These controls provide high-performance text rendering with advanced formatting options while maintaining consistent appearance across all platforms.

## SkiaLabel

SkiaLabel is the primary text rendering control in DrawnUi.Maui, rendering text directly with SkiaSharp. Unlike traditional MAUI labels, SkiaLabel provides pixel-perfect text rendering with advanced formatting capabilities.

### Basic Usage

```xml
<DrawUi:SkiaLabel 
    Text="Hello World" 
    TextColor="Black" 
    FontSize="18" 
    HorizontalTextAlignment="Center" 
    VerticalTextAlignment="Center" />
```

### Key Properties

| Property | Type | Description |
|----------|------|-------------|
| `Text` | string | The text content to display |
| `TextColor` | Color | Text color |
| `FontFamily` | string | Font family name |
| `FontSize` | float | Font size in logical pixels |
| `FontWeight` | int | Font weight (100-900 scale, 400=normal, 700=bold) |
| `FontItalic` | bool | Whether text should be italic |
| `HorizontalTextAlignment` | DrawTextAlignment | Text horizontal alignment (Start, Center, End, Fill) |
| `VerticalTextAlignment` | DrawTextAlignment | Text vertical alignment (Start, Center, End) |
| `LineBreakMode` | LineBreakMode | How text should wrap or truncate |
| `MaxLines` | int | Maximum number of lines to display |
| `IsUnderline` | bool | Whether text should be underlined |
| `IsStrikeThrough` | bool | Whether text should have strikethrough |

### Formatting Text

SkiaLabel supports rich text formatting through its `Spans` collection:

```xml
<DrawUi:SkiaLabel>
    <DrawUi:SkiaLabel.Spans>
        <DrawUi:TextSpan Text="Hello " TextColor="Black" FontSize="18" />
        <DrawUi:TextSpan Text="Beautiful " TextColor="Red" FontSize="20" FontWeight="700" />
        <DrawUi:TextSpan Text="World!" TextColor="Blue" FontSize="18" FontItalic="True" />
    </DrawUi:SkiaLabel.Spans>
</DrawUi:SkiaLabel>
```

#### Interactive Spans

You can make any text span interactive by adding the `Tapped` event handler:

```xml
<DrawUi:SkiaLabel FontSize="15" LineSpacing="1.5" TextColor="Black">
    <DrawUi:TextSpan Text="Regular text " />
    <DrawUi:TextSpan 
        Text="tappable link" 
        TextColor="Purple" 
        Tapped="OnSpanTapped"
        Tag="link-id" 
        Underline="True" />
    <DrawUi:TextSpan Text=" more text..." />
</DrawUi:SkiaLabel>
```

In your code-behind:

```csharp
private void OnSpanTapped(object sender, EventArgs e)
{
    var span = sender as TextSpan;
    string tag = span?.Tag?.ToString();
    // Handle the tap event based on the span or its tag
}
```

#### Styling Spans

TextSpan supports various styling options:

```xml
<DrawUi:TextSpan Text="Bold text" IsBold="True" />
<DrawUi:TextSpan Text="Italic text" IsItalic="True" />
<DrawUi:TextSpan Text="Underlined text" Underline="True" />
<DrawUi:TextSpan Text="Strikethrough text" Strikeout="True" />
<DrawUi:TextSpan Text="Highlighted text" BackgroundColor="Yellow" />
```

#### Emoji Support

For emoji rendering, use the `AutoFindFont` property:

```xml
<DrawUi:TextSpan Text="Regular text " />
<DrawUi:TextSpan AutoFindFont="True" Text="🌐🚒🙎🏽👻🤖" />
<DrawUi:TextSpan Text=" more text..." />
```

This ensures proper emoji rendering by finding and using appropriate fonts.

### Text Effects

SkiaLabel supports various text effects:

#### Text Shadow

SkiaLabel provides powerful shadow effects through the `Shadows` collection property. You can add multiple shadows with different offsets, colors, and blur radii:

```xml
<DrawUi:SkiaLabel 
    Text="Shadowed Text" 
    FontSize="24" 
    TextColor="White">
    <DrawUi:SkiaLabel.Shadows>
        <DrawUi:SkiaShadow 
            Color="#80000000" 
            BlurRadius="3" 
            Offset="1,1" />
    </DrawUi:SkiaLabel.Shadows>
</DrawUi:SkiaLabel>
```

For more advanced shadow effects, you can layer multiple shadows:

```xml
<DrawUi:SkiaLabel 
    Text="Advanced Shadows" 
    FontSize="28" 
    TextColor="White">
    <DrawUi:SkiaLabel.Shadows>
        <!-- Inner shadow -->
        <DrawUi:SkiaShadow 
            Color="#80000000" 
            BlurRadius="2" 
            Offset="0,1" />
        <!-- Outer glow -->
        <DrawUi:SkiaShadow 
            Color="#4000BBFF" 
            BlurRadius="8" 
            Offset="0,0" />
    </DrawUi:SkiaLabel.Shadows>
</DrawUi:SkiaLabel>
```

You can create various text effects using shadows:
- Text outlines by using zero offset and small blur radius
- Glow effects by using a bright color with larger blur
- 3D text effects with strategically placed shadows

#### Text Gradient

```xml
<DrawUi:SkiaLabel 
    Text="Gradient Text" 
    FontSize="24">
    <DrawUi:SkiaLabel.TextGradient>
        <DrawUi:SkiaGradient 
            Type="Linear" 
            StartColor="Red" 
            EndColor="Blue" 
            StartPoint="0,0" 
            EndPoint="1,1" />
    </DrawUi:SkiaLabel.TextGradient>
</DrawUi:SkiaLabel>
```

#### Outlined Text

```xml
<DrawUi:SkiaLabel 
    Text="Outlined Text" 
    FontSize="24" 
    TextColor="White" 
    StrokeColor="Black" 
    StrokeWidth="1" />
```

### Auto-sizing Text

SkiaLabel features powerful automatic font sizing capabilities that can dynamically adjust text to fit your container:

```xml
<DrawUi:SkiaLabel 
    Text="This text will resize to fit the available space" 
    AutoSizeText="True" 
    FontSize="24" 
    MaxLines="1" />
```

There are two different auto-sizing mechanisms in SkiaLabel:

#### 1. AutoSizeText Property

When you set `AutoSizeText="True"`, the label will automatically select the optimal font size to fit the text within the available space. This is ideal for when you want the text to fill the available area but still remain legible.

```xml
<DrawUi:SkiaLabel
    Text="FIT TEXT"
    AutoSizeText="True"
    WidthRequest="200"
    HeightRequest="50"
    HorizontalTextAlignment="Center"
    VerticalTextAlignment="Center"
    TextColor="Black" />
```

#### 2. AutoSize Enumeration

For more granular control, you can use the `AutoSize` property with the following options:

```xml
<DrawUi:SkiaLabel 
    Text="This text will adjust to width constraints" 
    AutoSize="Width" 
    FontSize="24" 
    MaxLines="2" 
    MinAutoSizeFontSize="12" />
```

The `AutoSize` property accepts:
- `None`: No auto-sizing
- `Width`: Adjust font size to fit width
- `Height`: Adjust font size to fit height
- `Both`: Adjust font size to fit both dimensions

### Auto-size Configuration

Control the auto-sizing behavior with these properties:

| Property | Type | Description |
|----------|------|-------------|
| `AutoSizeText` | bool | Enable automatic text fitting (fills available space) |
| `AutoSize` | AutoSizeType | More granular control over which dimensions to auto-size |
| `MinAutoSizeFontSize` | float | Minimum font size when auto-sizing |
| `AutoSizeFontStep` | float | Step size for font reduction during auto-sizing process |
| `ScaleX` | float | Horizontal scaling factor for text |
| `IsAutoFontSilent` | bool | Whether to suppress exceptions in auto-sizing calculations |

The difference between `AutoSizeText` and `AutoSize` is that `AutoSizeText` is designed to maximize text size to fill the available space, while `AutoSize` is designed to reduce text size until it fits within constraints.

### Emoji Support

SkiaLabel has built-in support for emoji rendering:

```xml
<DrawUi:SkiaLabel 
    Text="I ❤️ DrawnUi.Maui! 🚀" 
    FontSize="18" />
```

### Character Animation

For special effects, SkiaLabel supports character-by-character rendering:

```xml
<DrawUi:SkiaLabel 
    Text="Animated Text" 
    FontSize="18" 
    CharByChar="True" />
```

This allows for advanced animations like typing effects or character-by-character color changes.

### Monospaced Text Rendering

SkiaLabel provides the ability to render text in a monospaced style, regardless of the font used:

```xml
<DrawUi:SkiaLabel 
    Text="This text will be monospaced" 
    FontSize="18" 
    IsMonospaced="True" />
```

This is particularly useful for:
- Code displays where character alignment is important
- Creating tabular data without using a monospace font
- Ensuring consistent character widths for animations or special layouts

You can customize the monospace behavior with these properties:

| Property | Type | Description |
|----------|------|-------------|
| `IsMonospaced` | bool | Enables monospaced text rendering |
| `MonoSpacedDigitsOnly` | bool | Only make digits monospaced (useful for numbers in mixed text) |
| `ForceMonoWidth` | float | Force a specific width for all characters (leave at 0 to use auto-calculated width) |

The monospacer determines the width by finding the widest glyph and making all characters use that width, or by using the `ForceMonoWidth` value if specified.

### Performance Considerations

- For static text, set `Cache="Image"` to render once and cache as bitmap
- For frequently updated text, use `Cache="Operations"` for best performance
- Consider setting `MaxLines` when appropriate to avoid unnecessary layout calculations
- For large blocks of text, monitor performance and consider breaking into multiple labels
- Use `IsMonospaced` only when needed as it adds some calculation overhead
- For complex shadow effects, consider using `Cache="Image"` to optimize rendering

## SkiaMarkdownLabel

SkiaMarkdownLabel extends SkiaLabel to provide Markdown formatting capabilities. It parses Markdown syntax and renders properly formatted text.

### Basic Usage

```xml
<DrawUi:SkiaMarkdownLabel>
# Markdown Title

This is a paragraph with **bold** and *italic* text.

- List item 1
- List item 2

[Visit Documentation](https://link.example.com)

`Inline code` looks like this.

```csharp
// Code block
var label = new SkiaMarkdownLabel();
```
</DrawUi:SkiaMarkdownLabel>
```

### Supported Markdown Features

- **Headings** (# H1, ## H2)
- **Text formatting** (bold, italic, strikethrough)
- **Lists** (bulleted and numbered)
- **Links** (with customizable styling)
- **Code** (inline and blocks)
- **Paragraphs** (with proper spacing)

### Customizing Markdown Style

```xml
<DrawUi:SkiaMarkdownLabel 
    LinkColor="Blue" 
    CodeTextColor="DarkGreen" 
    CodeBackgroundColor="#EEEEEE" 
    CodeBlockBackgroundColor="#F5F5F5" 
    StrikeoutColor="Red" 
    PrefixBullet="• " 
    PrefixNumbered="{0}. " 
    UnderlineLink="True" 
    UnderlineWidth="1">
# Custom Styled Markdown
This has **custom** styling for [links](https://example.com) and `code blocks`.
</DrawUi:SkiaMarkdownLabel>
```

### Link Handling

SkiaMarkdownLabel provides built-in support for handling link taps:

```xml
<DrawUi:SkiaMarkdownLabel 
    LinkTapped="OnLinkTapped" 
    CommandLinkTapped="{Binding OpenLinkCommand}">
Check out [this link](https://example.com)!
</DrawUi:SkiaMarkdownLabel>
```

In your code-behind:

```csharp
private void OnLinkTapped(object sender, LinkTappedEventArgs e)
{
    // e.Link contains the link URL
    Browser.OpenAsync(e.Link);
}
```

### Implementation Notes

- SkiaMarkdownLabel implements a lightweight Markdown parser optimized for display, not full CommonMark compliance
- The parser focuses on the most commonly used Markdown syntax for mobile applications
- For more complex Markdown rendering needs, consider creating a custom renderer

## Special Labels

### SkiaLabelFps

A specialized label for displaying frames-per-second (FPS) metrics, useful for performance monitoring during development:

```xml
<DrawUi:SkiaLabelFps
    TextColor="Green"
    FontSize="12"
    HorizontalOptions="End"
    VerticalOptions="Start"
    Margin="0,20,20,0" />
```

## Example: Text Card

```xml
<DrawUi:SkiaShape
    Type="Rectangle"
    BackgroundColor="White"
    CornerRadius="8"
    Padding="16"
    WidthRequest="300">
    
    <DrawUi:SkiaShape.Shadows>
        <DrawUi:SkiaShadow
            Color="#22000000"
            BlurRadius="10"
            Offset="0,2" />
    </DrawUi:SkiaShape.Shadows>
    
    <DrawUi:SkiaLayout LayoutType="Column" Spacing="8">
        <DrawUi:SkiaLabel
            Text="Article Title"
            FontSize="20"
            FontWeight="700"
            TextColor="#333333" />
            
        <DrawUi:SkiaLabel
            Text="Published on April 3, 2025"
            FontSize="12"
            TextColor="#666666" />
            
        <DrawUi:SkiaLabel
            Text="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam in dui mauris. Vivamus hendrerit arcu sed erat molestie vehicula. Sed auctor neque eu tellus rhoncus ut eleifend nibh porttitor."
            FontSize="14"
            TextColor="#444444"
            LineHeight="1.5" />
            
        <DrawUi:SkiaLabel>
            <DrawUi:SkiaLabel.Spans>
                <DrawUi:TextSpan Text="Read more " TextColor="#444444" FontSize="14" />
                <DrawUi:TextSpan Text="here" TextColor="Blue" FontSize="14" IsUnderline="True" />
            </DrawUi:SkiaLabel.Spans>
        </DrawUi:SkiaLabel>
    </DrawUi:SkiaLayout>
</DrawUi:SkiaShape>
```