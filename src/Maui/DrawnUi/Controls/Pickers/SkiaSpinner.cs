using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DrawnUi.Controls;

public enum SelectionPosition
{
    Top,
    Right,
    Bottom,
    Left
}

/// <summary>
/// A wheel-of-names spinner control that displays items in a circular arrangement
/// and allows spinning to select an item through gesture interaction.
/// </summary>
[ContentProperty("ItemTemplate")]
public class SkiaSpinner : SkiaLayout
{


    public class SpinnerSlice : SkiaShape
    {

    }

    public SkiaSpinner()
    {
        CreateUi();
    }

    #region BINDABLE PROPERTIES

    public override void ResetItemsSource()
    {
        SyncItemsSource();
    }

    public override void OnItemSourceChanged()
    {
        SyncItemsSource();
    }

    protected override void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        SyncItemsSource();
    }

    void SyncItemsSource()
    {
        if (_wheelShape != null)
        {
            _wheelShape.ItemsSource = this.ItemsSource;
            _wheelShape.ItemTemplate = this.ItemTemplate ?? DefaultTemplate;
            _wheelShape.InverseVisualRotation = this.InverseVisualRotation; 
        }
        UpdateSelectedIndexFromRotation();
    }

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex),
        typeof(int),
        typeof(SkiaSpinner),
        -1,
        BindingMode.TwoWay,
        propertyChanged: OnSelectedIndexChanged);

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public static readonly BindableProperty WheelRotationProperty = BindableProperty.Create(
        nameof(WheelRotation),
        typeof(double),
        typeof(SkiaSpinner),
        0.0,
        propertyChanged: NeedApplyRotation);

    public double WheelRotation
    {
        get => (double)GetValue(WheelRotationProperty);
        set => SetValue(WheelRotationProperty, value);
    }

    static Color[] DefaultColors = new[]
    {
        Colors.Cyan,
        Colors.Blue,
        Colors.Green,
        Colors.Lime,
        Colors.Yellow,
        Colors.Orange,
        Colors.Red,
        Colors.Purple,
        Colors.Magenta,
        Colors.Pink,
    };

    /// <summary>
    /// To avoid repeating two same in a row
    /// </summary>
    private static Color _lastRandomColor = Colors.Transparent;

    /// <summary>
    /// To avoid repeating two same in start and end
    /// </summary>
    private static Color _firstRandomColor = Colors.Transparent;

    private static DataTemplate DefaultTemplate = new DataTemplate(() =>
    {
        var cell = new SkiaMarkdownLabel()
        {
            UseCache = SkiaCacheType.Image,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HorizontalTextAlignment = DrawTextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        }.ObserveSelf((me, prop) =>
        {
            if (prop == nameof(BindingContext) && me.BindingContext != null)
            {
                me.Text = me.BindingContext as string;

                //get random color but never let them repeat with the previous one or at the edges
                //checks will work only if all cells are created in a single pass
                var color = Colors.Transparent;
                while (color.Equals(Colors.Transparent) || color.Equals(_lastRandomColor))
                {
                    color = DefaultColors[Random.Next(0, DefaultColors.Length - 1)];

                    //check if we are the last cell, avoid repeating color with the first cell
                    if (me.Parent is SkiaLayout layout && me.ContextIndex == layout.ItemsSource.Count - 1)
                    {
                        if (color.Equals(_firstRandomColor))
                        {
                            color = Colors.Transparent;
                        }
                    }
                }
                _lastRandomColor = color;
                me.BackgroundColor = color;
                if (me.ContextIndex == 0)
                {
                    _firstRandomColor = color;
                }
            }
        });

        return cell;
    });

    public new DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly BindableProperty WheelRadiusProperty = BindableProperty.Create(
        nameof(WheelRadius),
        typeof(double),
        typeof(SkiaSpinner),
        100.0,
        propertyChanged: NeedDraw);

    public double WheelRadius
    {
        get => (double)GetValue(WheelRadiusProperty);
        set => SetValue(WheelRadiusProperty, value);
    }

    public static readonly BindableProperty SelectionPositionProperty = BindableProperty.Create(
        nameof(SelectionPosition),
        typeof(SelectionPosition),
        typeof(SkiaSpinner),
        SelectionPosition.Right,
        propertyChanged: OnSelectionPositionChanged);

    /// <summary>
    /// Determines where on the wheel the selected item is calculated (Top, Right, Bottom, Left)
    /// </summary>
    public SelectionPosition SelectionPosition
    {
        get => (SelectionPosition)GetValue(SelectionPositionProperty);
        set => SetValue(SelectionPositionProperty, value);
    }

    static void OnSelectionPositionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SkiaSpinner control)
        {
            control.UpdateSelectedIndexFromRotation();
        }
    }

    public static readonly BindableProperty InverseVisualRotationProperty = BindableProperty.Create(
        nameof(InverseVisualRotation),
        typeof(bool),
        typeof(SkiaSpinner),
        false,
        propertyChanged: NeedDraw);

    /// <summary>
    /// Controls the visual orientation direction. False = normal (readable at right), True = inverted (readable at left)
    /// </summary>
    public bool InverseVisualRotation
    {
        get => (bool)GetValue(InverseVisualRotationProperty);
        set => SetValue(InverseVisualRotationProperty, value);
    }

    public static readonly BindableProperty RespondsToGesturesProperty = BindableProperty.Create(
        nameof(RespondsToGestures),
        typeof(bool),
        typeof(SkiaSpinner),
        true);

    /// <summary>
    /// If disabled will not scroll using gestures. Scrolling will still be possible by code.
    /// </summary>
    public bool RespondsToGestures
    {
        get { return (bool)GetValue(RespondsToGesturesProperty); }
        set { SetValue(RespondsToGesturesProperty, value); }
    }

    #endregion

    #region EVENTS

    public event EventHandler<int> SelectedIndexChanged;

    #endregion

    #region PRIVATE FIELDS

    SkiaWheelShape _wheelShape;

    //readonly List<object> _itemsList = new();
    bool _isUpdatingFromRotation;

    #endregion

    #region PROPERTY CHANGED HANDLERS

    static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SkiaSpinner control && !control._isUpdatingFromRotation)
        {
            control.UpdateWheelRotationFromIndex();
            control.SelectedIndexChanged?.Invoke(control, (int)newValue);
        }
    }

    static void NeedApplyRotation(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SkiaSpinner spinner)
        {
            spinner.UpdateSelectedIndexFromRotation();
            spinner._wheelShape.Rotation = spinner.WheelRotation;
        }
    }

    #endregion

    #region PRIVATE METHODS

    new void CreateUi()
    {
        _wheelShape = new SkiaWheelShape
        {
            Type = ShapeType.Circle,
            BackgroundColor = Colors.Black,
            StrokeColor = Colors.CadetBlue,
            StrokeWidth = 2,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        // Bind the wheel rotation between spinner and wheel shape
        _wheelShape.SetBinding(SkiaWheelShape.WheelRotationProperty,
            new Binding(nameof(WheelRotation), source: this));
        _wheelShape.SetBinding(SkiaWheelShape.WheelRadiusProperty,
            new Binding(nameof(WheelRadius), source: this));
        _wheelShape.SetBinding(SkiaWheelShape.InverseVisualRotationProperty,
            new Binding(nameof(InverseVisualRotation), source: this));

        Children = new List<SkiaControl>() { _wheelShape };
    }


    protected int ItemsCount
    {
        get
        {
            if (ItemsSource == null)
            {
                return 0;
            }
            return ItemsSource.Count;
        }
    }

    void UpdateSelectedIndexFromRotation()
    {
        if (ItemsCount == 0)
        {
            return;
        }

        _isUpdatingFromRotation = true;

        var anglePerItem = 360.0 / ItemsCount;
        var positionOffset = GetSelectionPositionOffset();

        var adjustedRotation = (-WheelRotation + positionOffset) % 360;
        if (adjustedRotation < 0) adjustedRotation += 360;

        // When visual rotation is inverted, we need to flip the rotation calculation
        if (InverseVisualRotation)
        {
            adjustedRotation = (-adjustedRotation + positionOffset * 2) % 360;
            if (adjustedRotation < 0) adjustedRotation += 360;
        }

        var index = (int)Math.Round(adjustedRotation / anglePerItem) % ItemsCount;

        if (SelectedIndex != index)
        {
            SelectedIndex = index;
        }

        _isUpdatingFromRotation = false;
    }

    void UpdateWheelRotationFromIndex()
    {
        if (ItemsCount == 0) return;

        var anglePerItem = 360.0 / ItemsCount;
        var positionOffset = GetSelectionPositionOffset();

        var targetRotation = -(SelectedIndex * anglePerItem + positionOffset);

        // When visual rotation is inverted, flip the rotation calculation
        if (InverseVisualRotation)
        {
            targetRotation = -targetRotation - positionOffset * 2;
        }

        if (Math.Abs(WheelRotation - targetRotation) > 0.1)
        {
            WheelRotation = targetRotation;
        }
    }



    /// <summary>
    /// Gets the angle offset for the current selection position
    /// </summary>
    public double GetSelectionPositionOffset()
    {
        return SelectionPosition switch
        {
            SelectionPosition.Top => 0,
            SelectionPosition.Right => 90,
            SelectionPosition.Bottom => 180,
            SelectionPosition.Left => 270,
            _ => 90
        };
    }



    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Spins the wheel to a random position with animation
    /// </summary>
    public void SpinToRandom()
    {
        if (ItemsCount == 0) return;

        var random = new Random();
        var randomIndex = random.Next(ItemsCount);
        var extraSpins = random.Next(3, 8); // 3-7 full rotations for dramatic effect

        var anglePerItem = 360.0 / ItemsCount;
        var targetRotation = -(randomIndex * anglePerItem) + (extraSpins * 360);

        Rotate(targetRotation, (uint)(2000 + random.Next(1000))); // 2-3 seconds
    }

    /// <summary>
    /// Animates the wheel to a specific rotation
    /// </summary>
    public void Rotate(double targetRotation, uint durationMs = 500)
    {
        var animator = new RangeAnimator(this);
        animator.Start(value =>
        {
            WheelRotation = value;

        }, WheelRotation, targetRotation, durationMs, Easing.CubicOut);
    }

    #endregion
}
