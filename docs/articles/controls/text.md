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
| `FontAttributes` | FontAttributes | Bold/Italic/None |
| `HorizontalTextAlignment` | DrawTextAlignment | Text horizontal alignment (Start, Center, End, Fill) |
| `VerticalTextAlignment` | DrawTextAlignment | Text vertical alignment (Start, Center, End) |
| `LineBreakMode` | LineBreakMode | How text should wrap or truncate |
| `MaxLines` | int | Maximum number of lines to display |
| `StrokeColor` | Color | Outline color |
| `StrokeWidth` | double | Outline width |
| `DropShadowColor` | Color | Shadow color |
| `DropShadowSize` | double | Shadow blur radius |
| `DropShadowOffsetX`/`DropShadowOffsetY` | double | Shadow offset |
| `AutoSize` | AutoSizeType | Auto-sizing mode |
| `AutoSizeText` | string | Text to use for auto-sizing calculations |
| `LineSpacing` | double | Line spacing multiplier |
| `ParagraphSpacing` | double | Paragraph spacing multiplier |
| `CharacterSpacing` | double | Character spacing multiplier |
| `IsMonospaced` | bool | Enables monospaced text rendering |
| `MonoForDigits` | string | Use mono width for digits (e.g. "8") |

### Rich Text Formatting (Spans)

SkiaLabel supports rich text formatting through its `Spans` collection:

```xml
<DrawUi:SkiaLabel>
    <DrawUi:SkiaLabel.Spans>
        <DrawUi:TextSpan Text="Hello " TextColor="Black" FontSize="18" />
        <DrawUi:TextSpan Text="Beautiful " TextColor="Red" FontSize="20" FontWeight="700" />
        <DrawUi:TextSpan Text="World!" TextColor="Blue" FontSize="18" FontAttributes="Italic" />
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
<DrawUi:TextSpan Text="Bold text" FontAttributes="Bold" />
<DrawUi:TextSpan Text="Italic text" FontAttributes="Italic" />
<DrawUi:TextSpan Text="Underlined text" Underline="True" />
<DrawUi:TextSpan Text="Strikethrough text" Strikeout="True" />
<DrawUi:TextSpan Text="Highlighted text" BackgroundColor="Yellow" />
```

#### Emoji Support

For emoji rendering, use the `AutoFont` property:

```xml
<DrawUi:TextSpan Text="Regular text " />
<DrawUi:TextSpan AutoFont="True" Text="🌐🚒🙎🏽👻🤖" />
<DrawUi:TextSpan Text=" more text..." />
```

This ensures proper emoji rendering by finding and using appropriate fonts.

### Text Effects

SkiaLabel supports various text effects:

#### Drop Shadow

Use the following properties for shadow effects:
- `DropShadowColor`: Shadow color
- `DropShadowSize`: Blur radius
- `DropShadowOffsetX`, `DropShadowOffsetY`: Shadow offset

```xml
<DrawUi:SkiaLabel 
    Text="Shadowed Text" 
    FontSize="24" 
    TextColor="White"
    DropShadowColor="#80000000"
    DropShadowSize="3"
    DropShadowOffsetX="1"
    DropShadowOffsetY="1" />
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

#### Gradient Text

```xml
<DrawUi:SkiaLabel 
    Text="Gradient Text" 
    FontSize="24"
    FillGradient="{StaticResource MyGradient}" />
```

### Auto-sizing Text

SkiaLabel features powerful automatic font sizing capabilities that can dynamically adjust text to fit your container:

```xml
<DrawUi:SkiaLabel 
    Text="This text will resize to fit the available space" 
    AutoSize="TextToView" 
    FontSize="24" 
    MaxLines="1" />
```

- `AutoSize`: Controls auto-sizing mode (None, TextToWidth, TextToHeight, TextToView)
- `AutoSizeText`: Text to use for sizing calculations

### Monospaced Text Rendering

SkiaLabel provides the ability to render text in a monospaced style, regardless of the font used:

```xml
<DrawUi:SkiaLabel 
    Text="This text will be monospaced" 
    FontSize="18" 
    MonoForDigits="8" />
```

- `MonoForDigits`: Use mono width for digits (e.g. "8")

### Performance Considerations

- For static text, set `Cache="Image"` to render once and cache as bitmap
- For frequently updated text, use `Cache="Operations"` for best performance
- Consider setting `MaxLines` when appropriate to avoid unnecessary layout calculations
- For large blocks of text, monitor performance and consider breaking into multiple labels
- Use monospaced features only when needed as it adds some calculation overhead
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