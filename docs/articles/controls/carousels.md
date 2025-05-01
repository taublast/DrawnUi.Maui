# Carousel Controls

DrawnUi.Maui provides powerful carousel controls for creating interactive, swipeable displays of content. This article covers the carousel components available in the framework.

## SkiaCarousel

SkiaCarousel is a specialized scroll control designed specifically for creating swipeable carousels with automatic snapping to items.

### Basic Usage

```xml
<DrawUi:SkiaCarousel
    WidthRequest="400"
    HeightRequest="200"
    SelectedIndex="0">
    
    <!-- Item 1 -->
    <DrawUi:SkiaLayout BackgroundColor="Red">
        <DrawUi:SkiaLabel 
            Text="Slide 1" 
            FontSize="24" 
            HorizontalOptions="Center" 
            VerticalOptions="Center" />
    </DrawUi:SkiaLayout>
    
    <!-- Item 2 -->
    <DrawUi:SkiaLayout BackgroundColor="Green">
        <DrawUi:SkiaLabel 
            Text="Slide 2" 
            FontSize="24" 
            HorizontalOptions="Center" 
            VerticalOptions="Center" />
    </DrawUi:SkiaLayout>
    
    <!-- Item 3 -->
    <DrawUi:SkiaLayout BackgroundColor="Blue">
        <DrawUi:SkiaLabel 
            Text="Slide 3" 
            FontSize="24" 
            HorizontalOptions="Center" 
            VerticalOptions="Center" />
    </DrawUi:SkiaLayout>
    
</DrawUi:SkiaCarousel>
```

### Key Properties

| Property | Type | Description |
|----------|------|-------------|
| `SelectedIndex` | int | Current selected item index |
| `InTransition` | bool | Indicates if carousel is currently transitioning |
| `Spacing` | float | Space between carousel items |
| `SidesOffset` | float | Side padding to create a peek effect |
| `Bounces` | bool | Enables bouncing effect at edges |
| `ItemsSource` | IEnumerable | Data source for dynamically generating items |
| `ItemTemplate` | DataTemplate | Template for items when using ItemsSource |

### Peek Next/Previous Items

You can create a peek effect to show portions of adjacent slides:

```xml
<DrawUi:SkiaCarousel
    WidthRequest="400"
    HeightRequest="200"
    SidesOffset="40"
    SelectedIndex="0">
    
    <!-- Items here -->
    
</DrawUi:SkiaCarousel>
```

With `SidesOffset="40"`, 40 pixels on each side will be reserved to show portions of the previous and next items.

### Data Binding

SkiaCarousel supports data binding through `ItemsSource` and `ItemTemplate`:

```xml
<DrawUi:SkiaCarousel
    WidthRequest="400"
    HeightRequest="200"
    ItemsSource="{Binding CarouselItems}">
    
    <DrawUi:SkiaCarousel.ItemTemplate>
        <DataTemplate>
            <DrawUi:SkiaLayout BackgroundColor="{Binding Color}">
                <DrawUi:SkiaLabel 
                    Text="{Binding Title}" 
                    FontSize="24" 
                    HorizontalOptions="Center" 
                    VerticalOptions="Center" />
            </DrawUi:SkiaLayout>
        </DataTemplate>
    </DrawUi:SkiaCarousel.ItemTemplate>
    
</DrawUi:SkiaCarousel>
```

### Tracking Current Item

You can bind to the current item or monitor transitions:

```xml
<DrawUi:SkiaCarousel
    x:Name="MyCarousel"
    SelectedIndex="{Binding CurrentIndex, Mode=TwoWay}"
    WidthRequest="400"
    HeightRequest="200">
    
    <!-- Items here -->
    
</DrawUi:SkiaCarousel>

<!-- Display current state -->
<DrawUi:SkiaLabel 
    Text="{Binding Source={x:Reference MyCarousel}, Path=SelectedIndex, StringFormat='Current: {0}'}" 
    TextColor="Black" />

<DrawUi:SkiaLabel 
    Text="{Binding Source={x:Reference MyCarousel}, Path=InTransition, StringFormat='In Transition: {0}'}" 
    TextColor="Black" />
```

The `InTransition` property is particularly useful for disabling user interactions during transitions.

### Programmatic Control

You can control the carousel programmatically:

```csharp
// Jump to a specific index
myCarousel.SelectedIndex = 2;

// Animate to a specific index
myCarousel.ScrollTo(2, true);

// Track selection changes
myCarousel.PropertyChanged += (sender, e) => {
    if (e.PropertyName == nameof(SkiaCarousel.SelectedIndex))
    {
        // Handle selection change
        var index = myCarousel.SelectedIndex;
    }
};
```

## Advanced Examples

### Image Gallery Carousel

```xml
<DrawUi:SkiaCarousel
    WidthRequest="400"
    HeightRequest="300"
    SidesOffset="50"
    Bounces="True">
    
    <DrawUi:SkiaShape Type="Rectangle" BackgroundColor="White" CornerRadius="12">
        <DrawUi:SkiaShape.Shadows>
            <DrawUi:SkiaShadow Color="#40000000" BlurRadius="10" Offset="0,4" />
        </DrawUi:SkiaShape.Shadows>
        <DrawUi:SkiaImage Source="image1.jpg" Aspect="AspectFill" />
    </DrawUi:SkiaShape>
    
    <DrawUi:SkiaShape Type="Rectangle" BackgroundColor="White" CornerRadius="12">
        <DrawUi:SkiaShape.Shadows>
            <DrawUi:SkiaShadow Color="#40000000" BlurRadius="10" Offset="0,4" />
        </DrawUi:SkiaShape.Shadows>
        <DrawUi:SkiaImage Source="image2.jpg" Aspect="AspectFill" />
    </DrawUi:SkiaShape>
    
    <DrawUi:SkiaShape Type="Rectangle" BackgroundColor="White" CornerRadius="12">
        <DrawUi:SkiaShape.Shadows>
            <DrawUi:SkiaShadow Color="#40000000" BlurRadius="10" Offset="0,4" />
        </DrawUi:SkiaShape.Shadows>
        <DrawUi:SkiaImage Source="image3.jpg" Aspect="AspectFill" />
    </DrawUi:SkiaShape>
    
</DrawUi:SkiaCarousel>
```

### Card Carousel with Indicators

```xml
<DrawUi:SkiaLayout LayoutType="Column" HorizontalOptions="Fill">
    
    <DrawUi:SkiaCarousel
        x:Name="CardCarousel"
        WidthRequest="400"
        HeightRequest="300"
        SelectedIndex="{Binding CurrentCardIndex, Mode=TwoWay}">
        
        <!-- Card items here -->
        
    </DrawUi:SkiaCarousel>
    
    <!-- Page indicators -->
    <DrawUi:SkiaLayout 
        LayoutType="Row" 
        Spacing="8" 
        HorizontalOptions="Center"
        Margin="0,16,0,0">
        
        <DrawUi:SkiaShape 
            Type="Circle" 
            WidthRequest="12" 
            HeightRequest="12" 
            BackgroundColor="{Binding Source={x:Reference CardCarousel}, Path=SelectedIndex, Converter={StaticResource SelectedIndexConverter}, ConverterParameter=0}" />
        
        <DrawUi:SkiaShape 
            Type="Circle" 
            WidthRequest="12" 
            HeightRequest="12" 
            BackgroundColor="{Binding Source={x:Reference CardCarousel}, Path=SelectedIndex, Converter={StaticResource SelectedIndexConverter}, ConverterParameter=1}" />
        
        <DrawUi:SkiaShape 
            Type="Circle" 
            WidthRequest="12" 
            HeightRequest="12" 
            BackgroundColor="{Binding Source={x:Reference CardCarousel}, Path=SelectedIndex, Converter={StaticResource SelectedIndexConverter}, ConverterParameter=2}" />
        
    </DrawUi:SkiaLayout>
    
</DrawUi:SkiaLayout>
```

With a converter to change color based on selection:

```csharp
public class SelectedIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int selectedIndex = (int)value;
        int targetIndex = int.Parse(parameter.ToString());
        
        return selectedIndex == targetIndex ? Colors.Blue : Colors.LightGray;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

## Performance Considerations

- For optimal performance, use `Cache="Operations"` or `Cache="Image"` on complex carousel items
- Avoid placing too many items directly in the carousel; use virtualization through `ItemsSource` for large collections
- Consider using lightweight content for peek items if they'll be partially visible most of the time
- Monitor the performance using `SkiaLabelFps` during development to ensure smooth scrolling