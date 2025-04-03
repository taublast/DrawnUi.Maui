using System.Runtime.CompilerServices;

namespace DrawnUi.Draw;

public abstract class StackLayoutStructure
{
    protected readonly SkiaLayout _layout;

    public long ChildrenCount;

    public StackLayoutStructure(SkiaLayout layout)
    {
        _layout = layout;
    }

    public virtual IEnumerable<SkiaControl> EnumerateViewsForMeasurement()
    {
        bool standalone = false;
        SkiaControl template = null;
        IReadOnlyList<SkiaControl> views = null;

        bool useOneTemplate = _layout.IsTemplated;
        //ItemSizingStrategy == ItemSizingStrategy.MeasureFirstItem &&
        //&& _layout.RecyclingTemplate == RecyclingTemplate.Enabled;


        if (_layout.IsTemplated)
        {
            //in the other case template will be null and views adapter will get us a fresh template
            if (useOneTemplate)
            {
                standalone = true;
                template = _layout.ChildrenFactory.GetTemplateInstance();
            }
            ChildrenCount = _layout.ChildrenFactory.GetChildrenCount();
        }
        else
        {
            views = _layout.GetUnorderedSubviews();
            ChildrenCount = views.Count;
        }

        for (int index = 0; index < ChildrenCount; index++)
        {
            SkiaControl child = null;
            if (_layout.IsTemplated)
            {
                child = _layout.ChildrenFactory.GetChildAt(index, template);
            }
            else
            {
                child = views[index];
            }

            if (child == null)
                continue;

            yield return child;
        }

        if (useOneTemplate)
        {
            if (standalone)
                _layout.ChildrenFactory.ReleaseTemplateInstance(template);
            else
                _layout.ChildrenFactory.ReleaseView(template);
        }
    }

    /// <summary>
    /// Will measure children and build appropriate stack structure for the layout
    /// </summary>
    public abstract ScaledSize Build(SKRect rectForChildrenPixels, float scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual ScaledSize MeasureAndArrangeCell(SKRect destination, ControlInStack cell, SkiaControl child, float scale)
    {
        cell.Area = destination;

        var measured = _layout.MeasureChild(child, cell.Area.Width, cell.Area.Height, scale);

        cell.Measured = measured;

        LayoutCell(measured, cell, child, scale);

        return measured;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual ScaledSize MeasureCell(SKRect destination,
        ControlInStack cell, SkiaControl child,
        float scale)
    {
        cell.Area = destination;

        var measured = _layout.MeasureChild(child, cell.Area.Width, cell.Area.Height, scale);

        cell.Measured = measured;

        return measured;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual ScaledSize LayoutCell(
        ControlInStack cell, SkiaControl child,
        float scale)
    {
        var measured = cell.Measured;

        LayoutCell(measured, cell, child, scale);

        return measured;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void LayoutCell(
        ScaledSize measured,
        ControlInStack cell,
        SkiaControl child,
        float scale)
    {
        if (!measured.IsEmpty)
        {
            child.Arrange(cell.Area, measured.Units.Width, measured.Units.Height, scale);

            var maybeArranged = child.Destination;

            var arranged =
                new SKRect(cell.Area.Left, cell.Area.Top,
                    cell.Area.Left + cell.Measured.Pixels.Width,
                    cell.Area.Top + cell.Measured.Pixels.Height);

            if (maybeArranged.Height.IsNormal())
            {
                arranged.Top = maybeArranged.Top;
                arranged.Bottom = maybeArranged.Bottom;
            }
            if (maybeArranged.Width.IsNormal())
            {
                arranged.Left = maybeArranged.Left;
                arranged.Right = maybeArranged.Right;
            }

            cell.Destination = arranged;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual float GetSpacingForIndex(int forIndex, float scale)
    {
        var spacing = 0.0f;
        if (forIndex > 0)
        {
            spacing = (float)Math.Round(_layout.Spacing * scale);
        }
        return spacing;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual ControlInStack CreateWrapper(int i, SkiaControl control)
    {
        var add = new ControlInStack
        {
            ControlIndex = i
        };
        if (control != null)
        {
            add.ZIndex = control.ZIndex;
            add.View = control;
        }

        return add;
    }

}