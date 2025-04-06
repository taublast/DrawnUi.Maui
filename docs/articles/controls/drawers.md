# Drawer Controls

DrawnUi.Maui provides powerful drawer controls for creating sliding panels that can appear from any edge of the screen. This article covers the drawer components available in the framework.

## SkiaDrawer

SkiaDrawer is a versatile control that provides a sliding panel (drawer) with animated transitions and gesture support. It can slide in from any edge, making it perfect for navigation menus, filter panels, property drawers, and more.

### Basic Usage

```xml
<DrawUi:SkiaDrawer
    Direction="FromBottom"
    HeaderSize="60"
    IsOpen="False"
    HeightRequest="500"
    HorizontalOptions="Fill"
    VerticalOptions="End">
    
    <DrawUi:SkiaLayout
        HorizontalOptions="Fill"
        VerticalOptions="Fill">
        
        <!-- Header (visible when drawer is closed) -->
        <DrawUi:SkiaShape
            BackgroundColor="Blue"
            CornerRadius="20,20,0,0"
            HeightRequest="60"
            HorizontalOptions="Fill">
            
            <DrawUi:SkiaLabel
                Text="Drag Me"
                TextColor="White"
                HorizontalOptions="Center"
                VerticalOptions="Center" />
                
        </DrawUi:SkiaShape>
        
        <!-- Content (scrolls within drawer) -->
        <DrawUi:SkiaLayout
            BackgroundColor="White"
            Padding="20"
            Type="Column"
            Spacing="16"
            AddMarginTop="60">
            
            <DrawUi:SkiaLabel
                Text="Drawer Content"
                FontSize="20"
                TextColor="Black" />
                
            <!-- Additional content -->
                
        </DrawUi:SkiaLayout>
        
    </DrawUi:SkiaLayout>
    
</DrawUi:SkiaDrawer>
```

### Key Properties

| Property | Type | Description |
|----------|------|-------------|
| `Direction` | DrawerDirection | Direction from which the drawer appears |
| `HeaderSize` | double | Size of the area that remains visible when drawer is closed |
| `IsOpen` | bool | Controls whether the drawer is open or closed |
| `AmplitudeSize` | double | Optional override for drawer movement calculation |

### Drawer Direction

The `Direction` property controls the edge from which the drawer appears:

```xml
<!-- Bottom drawer -->
<DrawUi:SkiaDrawer
    Direction="FromBottom"
    VerticalOptions="End">
    <!-- Content -->
</DrawUi:SkiaDrawer>

<!-- Top drawer -->
<DrawUi:SkiaDrawer
    Direction="FromTop"
    VerticalOptions="Start">
    <!-- Content -->
</DrawUi:SkiaDrawer>

<!-- Left drawer -->
<DrawUi:SkiaDrawer
    Direction="FromLeft"
    HorizontalOptions="Start">
    <!-- Content -->
</DrawUi:SkiaDrawer>

<!-- Right drawer -->
<DrawUi:SkiaDrawer
    Direction="FromRight"
    HorizontalOptions="End">
    <!-- Content -->
</DrawUi:SkiaDrawer>
```

Note that you should set the appropriate alignment options (`VerticalOptions` and `HorizontalOptions`) to match the drawer direction.

### Header and Content

The drawer typically consists of two main parts:
- **Header**: Remains partially or fully visible when the drawer is closed
- **Content**: The main body of the drawer that slides in and out

The `HeaderSize` property determines how much of the drawer remains visible when closed.

### Controlling the Drawer

You can control the drawer programmatically:

```csharp
// Open the drawer
myDrawer.IsOpen = true;

// Close the drawer
myDrawer.IsOpen = false;

// Toggle the drawer
myDrawer.IsOpen = !myDrawer.IsOpen;
```

You can also use binding:

```xml
<DrawUi:SkiaDrawer
    IsOpen="{Binding IsDrawerOpen, Mode=TwoWay}">
    <!-- Content -->
</DrawUi:SkiaDrawer>
```

### Commands

SkiaDrawer provides built-in commands for programmatic control:

```xml
<DrawUi:SkiaButton
    Text="Open Drawer"
    CommandTapped="{Binding Source={x:Reference MyDrawer}, Path=CommandOpen}" />
    
<DrawUi:SkiaButton
    Text="Close Drawer"
    CommandTapped="{Binding Source={x:Reference MyDrawer}, Path=CommandClose}" />
    
<DrawUi:SkiaButton
    Text="Toggle Drawer"
    CommandTapped="{Binding Source={x:Reference MyDrawer}, Path=CommandToggle}" />
```

### Scrollable Drawer Content

For scrollable content within the drawer, combine with SkiaScroll:

```xml
<DrawUi:SkiaDrawer
    x:Name="BottomDrawer"
    Direction="FromBottom"
    HeaderSize="60"
    HeightRequest="500"
    VerticalOptions="End">
    
    <DrawUi:SkiaLayout>
        <!-- Header -->
        <DrawUi:SkiaShape
            BackgroundColor="Blue"
            HeightRequest="60"
            CornerRadius="20,20,0,0">
            <!-- Header content -->
        </DrawUi:SkiaShape>
        
        <!-- Scrollable content -->
        <DrawUi:SkiaScroll
            AddMarginTop="60"
            Bounces="False"
            BackgroundColor="White">
            
            <DrawUi:SkiaLayout
                Type="Column"
                Padding="20"
                Spacing="16">
                <!-- Many items here -->
            </DrawUi:SkiaLayout>
            
        </DrawUi:SkiaScroll>
    </DrawUi:SkiaLayout>
    
</DrawUi:SkiaDrawer>
```

The `AddMarginTop` property on SkiaScroll helps create space for the header.

## Examples

### Bottom Sheet with Form

```xml
<DrawUi:SkiaDrawer
    Direction="FromBottom"
    HeaderSize="60"
    IsOpen="False"
    HeightRequest="400"
    VerticalOptions="End"
    HorizontalOptions="Fill">
    
    <DrawUi:SkiaLayout>
        <!-- Header -->
        <DrawUi:SkiaShape
            BackgroundColor="#3498DB"
            CornerRadius="20,20,0,0"
            HeightRequest="60">
            
            <DrawUi:SkiaLayout
                HorizontalOptions="Fill"
                VerticalOptions="Fill">
                
                <DrawUi:SkiaLabel
                    Text="Settings"
                    TextColor="White"
                    FontSize="18"
                    HorizontalOptions="Center"
                    VerticalOptions="Center" />
                    
                <DrawUi:SkiaShape
                    Type="Rectangle"
                    WidthRequest="40"
                    HeightRequest="4"
                    CornerRadius="2"
                    BackgroundColor="White"
                    Margin="0,10,0,0"
                    HorizontalOptions="Center"
                    VerticalOptions="Start" />
                    
            </DrawUi:SkiaLayout>
            
        </DrawUi:SkiaShape>
        
        <!-- Content -->
        <DrawUi:SkiaScroll
            AddMarginTop="60"
            BackgroundColor="White">
            
            <DrawUi:SkiaLayout
                Type="Column"
                Padding="20"
                Spacing="16">
                
                <!-- Form fields -->
                <DrawUi:SkiaLabel
                    Text="Username"
                    FontSize="14"
                    TextColor="#333333" />
                    
                <DrawUi:SkiaMauiEntry
                    PlaceholderText="Enter your username"
                    HeightRequest="50"
                    BackgroundColor="#F5F5F5"
                    TextColor="#333333" />
                    
                <DrawUi:SkiaLabel
                    Text="Email"
                    FontSize="14"
                    TextColor="#333333" />
                    
                <DrawUi:SkiaMauiEntry
                    PlaceholderText="Enter your email"
                    HeightRequest="50"
                    BackgroundColor="#F5F5F5"
                    TextColor="#333333" />
                    
                <DrawUi:SkiaButton
                    Text="SAVE"
                    BackgroundColor="#2ECC71"
                    TextColor="White"
                    HeightRequest="50"
                    Margin="0,20,0,0" />
                    
            </DrawUi:SkiaLayout>
            
        </DrawUi:SkiaScroll>
    </DrawUi:SkiaLayout>
    
</DrawUi:SkiaDrawer>
```

### Navigation Drawer

```xml
<Grid>
    <!-- Main content -->
    <DrawUi:Canvas
        HorizontalOptions="Fill"
        VerticalOptions="Fill">
        
        <DrawUi:SkiaLayout
            BackgroundColor="White"
            HorizontalOptions="Fill"
            VerticalOptions="Fill">
            
            <!-- Main app content here -->
            
            <DrawUi:SkiaButton
                Text="Open Menu"
                CommandTapped="{Binding Source={x:Reference SideDrawer}, Path=CommandOpen}"
                HorizontalOptions="Start"
                VerticalOptions="Start"
                Margin="20" />
                
        </DrawUi:SkiaLayout>
        
    </DrawUi:Canvas>
    
    <!-- Left side drawer -->
    <DrawUi:SkiaDrawer
        x:Name="SideDrawer"
        Direction="FromLeft"
        HeaderSize="0"
        IsOpen="False"
        WidthRequest="280"
        HorizontalOptions="Start"
        VerticalOptions="Fill">
        
        <DrawUi:SkiaLayout
            BackgroundColor="#2C3E50"
            HorizontalOptions="Fill"
            VerticalOptions="Fill">
            
            <DrawUi:SkiaScroll>
                <DrawUi:SkiaLayout
                    Type="Column"
                    Padding="0,40,0,0">
                    
                    <!-- User info -->
                    <DrawUi:SkiaLayout
                        Type="Column"
                        Padding="20"
                        Spacing="8">
                        
                        <DrawUi:SkiaShape
                            Type="Circle"
                            WidthRequest="80"
                            HeightRequest="80"
                            BackgroundColor="#3498DB">
                            
                            <DrawUi:SkiaLabel
                                Text="JD"
                                FontSize="36"
                                TextColor="White"
                                HorizontalOptions="Center"
                                VerticalOptions="Center" />
                                
                        </DrawUi:SkiaShape>
                        
                        <DrawUi:SkiaLabel
                            Text="John Doe"
                            FontSize="18"
                            TextColor="White"
                            Margin="0,10,0,0" />
                            
                        <DrawUi:SkiaLabel
                            Text="john.doe@example.com"
                            FontSize="14"
                            TextColor="#BBBBBB" />
                            
                    </DrawUi:SkiaLayout>
                    
                    <!-- Menu items -->
                    <DrawUi:SkiaShape
                        Type="Rectangle"
                        HeightRequest="1"
                        BackgroundColor="#405060"
                        Margin="0,20,0,20" />
                        
                    <!-- Menu item 1 -->
                    <DrawUi:SkiaHotspot Tapped="OnMenuItemTapped">
                        <DrawUi:SkiaLayout
                            Type="Row"
                            Padding="20,15"
                            Spacing="16">
                            
                            <DrawUi:SkiaShape
                                Type="Rectangle"
                                WidthRequest="24"
                                HeightRequest="24"
                                BackgroundColor="#3498DB" />
                                
                            <DrawUi:SkiaLabel
                                Text="Home"
                                FontSize="16"
                                TextColor="White" />
                                
                        </DrawUi:SkiaLayout>
                    </DrawUi:SkiaHotspot>
                    
                    <!-- Additional menu items -->
                    
                </DrawUi:SkiaLayout>
            </DrawUi:SkiaScroll>
            
        </DrawUi:SkiaLayout>
        
    </DrawUi:SkiaDrawer>
</Grid>
```

## Performance Considerations

- For complex drawers, consider using `Cache="Operations"` on content that doesn't change often
- Use appropriate header size to ensure smooth gestures in the grabbable area
- For large drawers with many child elements, enable virtualization in nested scrolling content
- Avoid doing heavy work in `IsOpen` change handlers as this can cause animation stuttering