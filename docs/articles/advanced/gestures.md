# Advanced Gesture Handling in DrawnUi.Maui

DrawnUi.Maui provides a robust and extensible gesture system for building interactive, touch-driven UIs. This article covers how to use built-in gestures, implement custom gesture logic, and best practices for advanced scenarios.

## Gesture System Overview
- **Unified gesture model** for tap, drag, swipe, pinch, long-press, and multi-touch
- **ISkiaGestureListener** interface for custom gesture handling
- **SkiaHotspot** and gesture listeners for declarative and code-based gestures
- **Gesture locking and propagation control** for complex UI hierarchies

## Basic Tap and Click Handling

Use `SkiaHotspot` for simple tap/click detection:

```xml
<DrawUi:SkiaHotspot Tapped="OnTapped">
    <DrawUi:SkiaShape Type="Circle" BackgroundColor="Blue" WidthRequest="80" HeightRequest="80" />
</DrawUi:SkiaHotspot>
```

```csharp
private void OnTapped(object sender, EventArgs e)
{
    // Handle tap
}
```

## Handling Drag, Swipe, and Multi-Touch

Implement `ISkiaGestureListener` for advanced gestures:

```csharp
public class DraggableShape : SkiaShape, ISkiaGestureListener
{
    private float _x, _y;
    public override void OnParentChanged()
    {
        base.OnParentChanged();
        RegisterGestureListener(this);
    }
    public bool OnGestureEvent(TouchActionType type, TouchActionEventArgs args, TouchActionResult result)
    {
        if (type == TouchActionType.Pan)
        {
            _x = args.Location.X;
            _y = args.Location.Y;
            Invalidate();
            return true;
        }
        return false;
    }
}
```

## Gesture Locking and Propagation

Use the `LockChildrenGestures` property to control gesture propagation:

- `LockTouch.Enabled`: Prevents children from receiving gestures
- `LockTouch.Disabled`: Allows all gestures to propagate
- `LockTouch.PassTap`: Only tap events pass through
- `LockTouch.PassTapAndLongPress`: Tap and long-press pass through

Example:

```xml
<DrawUi:SkiaLayout LockChildrenGestures="PassTap">
    <!-- Only tap gestures reach children -->
</DrawUi:SkiaLayout>
```

## Custom Gesture Handling in Code

Override `OnGestureEvent` for fine-grained control:

```csharp
public override ISkiaGestureListener OnGestureEvent(TouchActionType type, TouchActionEventArgs args, TouchActionResult result, SKPoint childOffset, SKPoint childOffsetDirect)
{
    // Custom logic for gesture routing or handling
    return base.OnGestureEvent(type, args, result, childOffset, childOffsetDirect);
}
```

## Multi-Touch and Pinch-to-Zoom

Listen for pinch and multi-touch events:

```csharp
public bool OnGestureEvent(TouchActionType type, TouchActionEventArgs args, TouchActionResult result)
{
    if (type == TouchActionType.Pinch)
    {
        // args.PinchScale, args.Center, etc.
        // Handle zoom
        return true;
    }
    return false;
}
```

## Gesture Utilities and Best Practices
- Use `HadInput` to track which listeners have received input
- Use `InputTransparent` to make controls ignore gestures
- For performance, avoid deep gesture listener hierarchies
- Use debug logging to trace gesture flow

## Example: Swipe-to-Delete List Item

```xml
<DrawUi:SkiaLayout ItemsSource="{Binding Items}">
    <DrawUi:SkiaLayout.ItemTemplate>
        <DataTemplate>
            <local:SwipeToDeleteItem />
        </DataTemplate>
    </DrawUi:SkiaLayout.ItemTemplate>
</DrawUi:SkiaLayout>
```

```csharp
public class SwipeToDeleteItem : SkiaLayout, ISkiaGestureListener
{
    public override void OnParentChanged()
    {
        base.OnParentChanged();
        RegisterGestureListener(this);
    }
    public bool OnGestureEvent(TouchActionType type, TouchActionEventArgs args, TouchActionResult result)
    {
        if (type == TouchActionType.Pan && result == TouchActionResult.Panning)
        {
            // Move item horizontally
            this.TranslationX = args.Location.X;
            Invalidate();
            return true;
        }
        if (type == TouchActionType.Pan && result == TouchActionResult.Up)
        {
            if (Math.Abs(this.TranslationX) > 100)
            {
                // Trigger delete
                // ...
            }
            this.TranslationX = 0;
            Invalidate();
            return true;
        }
        return false;
    }
}
```

## Debugging and Extending Gestures
- Use debug output to trace gesture events and propagation
- Extend or compose gesture listeners for complex scenarios
- Integrate with platform-specific gesture APIs if needed

## Summary
DrawnUi.Maui’s gesture system enables rich, interactive UIs with tap, drag, swipe, pinch, and custom gestures. Use SkiaHotspot for simple cases, ISkiaGestureListener for advanced logic, and gesture locking for complex layouts.
