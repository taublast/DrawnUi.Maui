//reusing some code from #dotnetmaui Layout

using System.Collections.ObjectModel;

namespace DrawnUi.Maui.Draw;

public partial class SkiaLayout
{
    private SKRect _measuredFor;

    private int _listAdditionalMeasurements;
    /// <summary>
    /// Measuring column/row list with todo MeasureVisible
    /// </summary>
    /// <param name="rectForChildrenPixels"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public virtual ScaledSize MeasureList(SKRect rectForChildrenPixels, float scale)
    {

        if (IsTemplated && ItemsSource.Count > 0)
        {
            int measuredCount = 0;
            var itemsCount = ItemsSource.Count;
            ScaledSize measured = ScaledSize.Default;
            SKRect rectForChild = rectForChildrenPixels;//.Clone();

            SkiaControl[] nonTemplated = null;
            bool smartMeasuring = false;

            var stackHeight = 0.0f;
            var stackWidth = 0.0f;

            SkiaControl template = null;
            bool useOneTemplate = IsTemplated &&
                                  RecyclingTemplate != RecyclingTemplate.Disabled;

            if (useOneTemplate)
            {
                template = ChildrenFactory.GetTemplateInstance();
            }

            var maybeSecondPass = true;
            List<SecondPassArrange> listSecondPass = new();
            bool stopMeasuring = false;

            var inflate = (float)this.VirtualisationInflated * scale;
            var visibleArea = base.GetOnScreenVisibleArea(new (null, rectForChildrenPixels,scale), new (inflate,inflate));

            if (visibleArea.Pixels.Height < 1 || visibleArea.Pixels.Width < 1)
            {
                return ScaledSize.CreateEmpty(scale);
            }

            //ScaledRect visibleArea = _viewport;

            var rowsCount = itemsCount;
            var columnsCount = 1;
            if (Type == LayoutType.Row)
            {
                rowsCount = 1;
                columnsCount = itemsCount;
            }

            var rows = new List<List<ControlInStack>>();
            var columns = new List<ControlInStack>(columnsCount);

            //left to right, top to bottom
            var index = -1;
            for (var row = 0; row < rowsCount; row++)
            {
                if (stopMeasuring || index + 2 > itemsCount)
                {
                    break;
                }

                var rowMaxHeight = 0.0f;
                var maxWidth = 0.0f;

                // Calculate the width for each column
                float widthPerColumn;
                if (Type == LayoutType.Column)
                {
                    widthPerColumn = (float)Math.Round(columnsCount > 1 ?
                        (rectForChildrenPixels.Width - (columnsCount - 1) * Spacing * scale) / columnsCount :
                        rectForChildrenPixels.Width);
                }
                else
                {
                    widthPerColumn = rectForChildrenPixels.Width;
                }

                int column;
                for (column = 0; column < columnsCount; column++)
                {
                    try
                    {
                        if (index + 2 > itemsCount)
                        {
                            stopMeasuring = true;
                            break;
                        }

                        index++;
                        var cell = new ControlInStack()
                        {
                            Column = column,
                            Row = row,
                            ControlIndex = index
                        };

                        SkiaControl child = ChildrenFactory.GetChildAt(cell.ControlIndex, template, 0, true);

                        if (child == null)
                        {
                            Super.Log($"[MeasureStack] FAILED to get child at index {cell.ControlIndex}");
                            return ScaledSize.Default;
                        }

                        if (column == 0)
                            rectForChild.Top += GetSpacingForIndex(row, scale);
                        rectForChild.Left += GetSpacingForIndex(column, scale);

                        if (!child.CanDraw)
                        {
                            cell.Measured = ScaledSize.Default;
                        }
                        else
                        {
                            var rectFitChild = new SKRect(rectForChild.Left, rectForChild.Top, rectForChild.Left + widthPerColumn, rectForChild.Bottom);
                            measured = MeasureAndArrangeCell(rectFitChild, cell, child, rectForChildrenPixels, scale);

                            if (!visibleArea.Pixels.IntersectsWithInclusive(cell.Destination))
                            {
                                stopMeasuring = true;
                                break;
                            }

                            cell.Measured = measured;
                            cell.WasMeasured = true;

                            measuredCount++;

                            if (!measured.IsEmpty)
                            {
                                maxWidth += measured.Pixels.Width + GetSpacingForIndex(column, scale);

                                if (measured.Pixels.Height > rowMaxHeight)
                                    rowMaxHeight = measured.Pixels.Height;

                                //offset -->
                                rectForChild.Left += (float)(measured.Pixels.Width);
                            }
                        }

                        columns.Add(cell);
                    }
                    catch (Exception e)
                    {
                        Super.Log(e);
                        break;
                    }

                }//end of iterate columns

                rows.Add(columns);
                columns = new();

                if (maxWidth > stackWidth)
                    stackWidth = maxWidth;

                stackHeight += rowMaxHeight + GetSpacingForIndex(row, scale);
                rectForChild.Top += (float)(rowMaxHeight);

                rectForChild.Left = 0; //reset to start

            }//end of iterate rows

            if (HorizontalOptions.Alignment == LayoutAlignment.Fill || SizeRequest.Width >= 0)
            {
                stackWidth = rectForChildrenPixels.Width;
            }
            if (VerticalOptions.Alignment == LayoutAlignment.Fill || SizeRequest.Height >= 0)
            {
                stackHeight = rectForChildrenPixels.Height;
            }

            //second layout pass in some cases
            var autoRight = rectForChildrenPixels.Right;
            if (this.HorizontalOptions != LayoutOptions.Fill)
            {
                autoRight = rectForChildrenPixels.Left + stackWidth;
            }
            var autoBottom = rectForChildrenPixels.Bottom;
            if (this.VerticalOptions != LayoutOptions.Fill)
            {
                autoBottom = rectForChildrenPixels.Top + stackHeight;
            }
            var autoRect = new SKRect(
                rectForChildrenPixels.Left, rectForChildrenPixels.Top,
                autoRight,
                autoBottom);

            foreach (var secondPass in listSecondPass)
            {
                if (float.IsInfinity(secondPass.Cell.Area.Bottom))
                {
                    secondPass.Cell.Area = new(secondPass.Cell.Area.Left, secondPass.Cell.Area.Top, secondPass.Cell.Area.Right, secondPass.Cell.Area.Top + stackHeight);
                }
                else
                if (float.IsInfinity(secondPass.Cell.Area.Top))
                {
                    secondPass.Cell.Area = new(secondPass.Cell.Area.Left, secondPass.Cell.Area.Bottom - stackHeight, secondPass.Cell.Area.Right, secondPass.Cell.Area.Bottom);
                }

                if (secondPass.Cell.Area.Height > stackHeight)
                {
                    secondPass.Cell.Area = new(secondPass.Cell.Area.Left, secondPass.Cell.Area.Top,
                        secondPass.Cell.Area.Right, secondPass.Cell.Area.Top + stackHeight);
                }

                if (float.IsInfinity(secondPass.Cell.Area.Right))
                {
                    secondPass.Cell.Area = new(secondPass.Cell.Area.Left, secondPass.Cell.Area.Top, secondPass.Cell.Area.Left + stackWidth, secondPass.Cell.Area.Bottom);
                }
                else
                if (float.IsInfinity(secondPass.Cell.Area.Left))
                {
                    secondPass.Cell.Area = new(secondPass.Cell.Area.Right - stackWidth, secondPass.Cell.Area.Top, secondPass.Cell.Area.Right, secondPass.Cell.Area.Bottom);
                }

                if (secondPass.Cell.Area.Width > stackWidth)
                {
                    secondPass.Cell.Area = new(secondPass.Cell.Area.Left, secondPass.Cell.Area.Top, secondPass.Cell.Area.Left + stackWidth, secondPass.Cell.Area.Bottom);
                }

                LayoutCell(secondPass.Child.MeasuredSize, secondPass.Cell, secondPass.Child,
                    autoRect,
                    secondPass.Scale);
            }

            if (HorizontalOptions.Alignment == LayoutAlignment.Fill && WidthRequest < 0)
            {
                stackWidth = rectForChildrenPixels.Width;
            }
            if (VerticalOptions.Alignment == LayoutAlignment.Fill && HeightRequest < 0)
            {
                stackHeight = rectForChildrenPixels.Height;
            }

            var structure = new LayoutStructure(rows);
            if (InitializeTemplatesInBackgroundDelay > 0)
            {
                StackStructure = structure;
            }
            else
            {
                StackStructureMeasured = structure;
            }

            FirstVisibleIndex = -1;
            FirstMeasuredIndex = 0;

            LastVisibleIndex = -1;
            LastMeasuredIndex = measuredCount - 1;

            if (measuredCount > 0)
            {
                if (this.Type == LayoutType.Column)
                {
                    var medium = stackHeight / measuredCount;
                    stackHeight = medium * itemsCount;
                }
                else
                if (this.Type == LayoutType.Row)
                {
                    var medium = stackWidth / measuredCount;
                    stackWidth = medium * itemsCount;
                }
            }

            _listAdditionalMeasurements = 0;

            if (template != null)
            {
                ChildrenFactory.ReleaseTemplateInstance(template);
            }

            _measuredFor = rectForChildrenPixels;

            return ScaledSize.FromPixels(stackWidth, stackHeight, scale);
        }

        return ScaledSize.FromPixels(rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);
    }

    public int FirstMeasuredIndex { get; protected set; }

    public int LastMeasuredIndex { get; protected set; }

    public int FirstVisibleIndex { get; protected set; }

    public int LastVisibleIndex { get; protected set; }

    /// <summary>
    /// Renders Templated Column/Row todo in some cases..
    /// </summary>
    /// <param name="structure"></param>
    /// <param name="context"></param>
    /// <param name="destination"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    protected virtual int DrawList(
        DrawingContext ctx,
        LayoutStructure structure)
    {
        if (!IsTemplated || IsDisposing)
            return 0;

        //StackStructure was creating inside Measure.
        //While scrolling templated its not called again (checked).

        List<SkiaControlWithRect> tree = new();
        bool wasVisible = false;
        var needrebuild = templatesInvalidated;
        int countRendered = 0;
        int visibleIndex = -1;
        int visibleIndexEnd = -1;

        if (structure != null)
        {
            //draw children manually
            var inflate = (float)this.VirtualisationInflated * ctx.Scale;
            var visibleArea = GetOnScreenVisibleArea(ctx, new(inflate, inflate));

            var currentIndex = -1;
            foreach (var cell in structure.GetChildrenAsSpans())
            {
                currentIndex++;

                if (!cell.WasMeasured)
                {
                    continue;
                }

                if (cell.Destination == SKRect.Empty || cell.Measured.Pixels.IsEmpty)
                {
                    cell.IsVisible = false;
                }
                else
                {
                    //cell.Destination is what was measured, and we got x,y offsets from a parent, like scroll

                    var x = ctx.Destination.Left + cell.Destination.Left;
                    var y = ctx.Destination.Top + cell.Destination.Top;

                    cell.Drawn.Set(x, y, x + cell.Destination.Width, y + cell.Destination.Height);

                    if (Virtualisation != VirtualisationType.Disabled)
                    {
                        if (needrebuild && UsingCacheType == SkiaCacheType.None &&
                            Virtualisation == VirtualisationType.Smart
                            && !(IsTemplated && RecyclingTemplate == RecyclingTemplate.Enabled))
                        {
                            cell.IsVisible = true;
                        }
                        else
                        {
                            cell.IsVisible = cell.Drawn.IntersectsWith(visibleArea.Pixels);
                        }
                    }
                    else
                    {
                        cell.IsVisible = true;
                    }
                }

                if (!ChildrenFactory.TemplatesAvailable && InitializeTemplatesInBackgroundDelay > 0)
                {
                    break; //itemssource was changed by other thread
                }

                if (cell.IsVisible)
                {
                    if (visibleIndex < 0 && currentIndex > visibleIndex)
                    {
                        visibleIndex = currentIndex;
                    }

                    var child = ChildrenFactory.GetChildAt(cell.ControlIndex, null, GetSizeKey(cell.Measured.Pixels));
                    if (child == null) //ChildrenFactory.GetChildAt was unable to return child?..
                    {
                        return countRendered;
                    }

                    if (child is SkiaControl control && child.IsVisible)
                    {
                        if (child.NeedMeasure)
                        {
                            if (!child.WasMeasured || GetSizeKey(child.MeasuredSize.Pixels) != GetSizeKey(cell.Measured.Pixels))
                            {
                                child.Measure((float)cell.Area.Width, (float)cell.Area.Height, ctx.Scale);
                            }
                        }

                        SKRect destinationRect;
                        if (IsTemplated && RecyclingTemplate != RecyclingTemplate.Disabled)
                        {
                            //when context changes we need all available space for remeasuring cell
                            destinationRect = new SKRect(cell.Drawn.Left, cell.Drawn.Top,
                                cell.Drawn.Left + cell.Area.Width, cell.Drawn.Top + cell.Area.Bottom);
                        }
                        else
                        {
                            destinationRect = new SKRect(cell.Drawn.Left, cell.Drawn.Top, cell.Drawn.Right,
                                cell.Drawn.Bottom);
                        }


                        if (IsRenderingWithComposition)
                        {
                            if (DirtyChildrenInternal.Contains(child))
                            {
                                DrawChild(ctx.WithDestination(destinationRect), child);
                                countRendered++;
                            }
                            else
                            {
                                //skip drawing but need arrange :(
                                //todo set virtual offset between drawnrect and the new
                                child.Arrange(destinationRect, child.SizeRequest.Width, child.SizeRequest.Height, ctx.Scale);
                            }
                        }
                        else
                        {
                            DrawChild(ctx.WithDestination(destinationRect), child);
                            countRendered++;
                        }

                        //gonna use that for gestures and for item inside viewport detection and for hotreload children tree
                        tree.Add(new SkiaControlWithRect(control,
                            destinationRect,
                            control.LastDrawnAt,
                            currentIndex));
                    }
                }

                if (!cell.IsVisible)
                {
                    if (visibleIndexEnd < 0 && currentIndex > visibleIndexEnd)
                    {
                        visibleIndexEnd = currentIndex - 1;
                    }

                    ChildrenFactory.MarkViewAsHidden(cell.ControlIndex);
                }
            }
        }

        FirstVisibleIndex = visibleIndex;
        LastVisibleIndex = visibleIndexEnd;

        if (needrebuild && countRendered > 0)
        {
            templatesInvalidated = false;
        }

        RenderTree = tree;
        _builtRenderTreeStamp = _measuredStamp;

        if (Parent is IDefinesViewport viewport &&
            viewport.TrackIndexPosition != RelativePositionType.None)
        {
            viewport.UpdateVisibleIndex();
        }

        OnPropertyChanged(nameof(DebugString));

        return countRendered;
    }

    private MeasuredListCells _measuredCells;

    public int EstimatedTotalItems => ItemsSource?.Count ?? 0;

    // Returns how far we have measured content in units (vertical or horizontal)
    public double GetMeasuredContentEnd()
    {
        var structure = LatestStackStructure;
        if (structure != null)
        {
            var last = LatestStackStructure.GetChildren().LastOrDefault();
            if (last != null)
            {
                if (Type == LayoutType.Column)
                {
                    return last.Destination.Top / RenderingScale;
                }

                if (Type == LayoutType.Row)
                {
                    return last.Destination.Left / RenderingScale;
                }
            }
        }

        return double.PositiveInfinity;
    }


    private (float x, float y, int row, int col) GetNextItemPositionForIncremental(LayoutStructure structure)
    {
        if (structure.GetCount() == 0)
        {
            // No items measured yet
            return (0f, 0f, 0, 0);
        }

        var lastItem = structure.GetChildren().Last();

        int lastRow = lastItem.Row;
        int lastCol = lastItem.Column;
        int nextRow = lastRow;
        int nextCol = lastCol + 1;

        int columnsCount = (Split > 0) ? Split : 1;
        if (nextCol >= columnsCount)
        {
            // start a new row
            nextRow = lastRow + 1;
            nextCol = 0;
        }

        float startX = 0f;
        float startY = 0f;

        if (this.Type == LayoutType.Column)
        {
            startY = ComputeBottomOfRow(structure, lastRow) + (float)(Spacing * RenderingScale);
        }
        else
        {
            startX = ComputeRightOfColumn(structure, lastRow) + (float)(Spacing * RenderingScale);
        }

        // If we are placing item in the same row must find the position after last col
        if (nextCol > 0)
        {
            float columnWidth = ComputeColumnWidth(columnsCount);
            startX = nextCol * (columnWidth + (float)(Spacing * RenderingScale));
        }

        return (startX, startY, nextRow, nextCol);
    }

    private float ComputeColumnWidth(int columnsCount)
    {
        if (this.Type == LayoutType.Column)
        {
            return (float)Math.Round(columnsCount > 1 ?
                (MeasuredSize.Pixels.Width - (columnsCount - 1) * Spacing * RenderingScale) / columnsCount :
                MeasuredSize.Pixels.Width);
        }
        else
        {
            return MeasuredSize.Pixels.Width;
        }
    }



    private float ComputeRightOfColumn(LayoutStructure structure, int row)
    {
        var cell = structure.GetRow(row).Last();
        var right = cell.Area.Left + cell.Measured.Pixels.Width;
        return right;
    }

    private float ComputeBottomOfRow(LayoutStructure structure, int row)
    {
        // Find the max bottom of all items in that row
        float maxBottom = 0f;
        foreach (var cell in structure.GetRow(row))
        {
            var bottom = cell.Area.Top + cell.Measured.Pixels.Height;
            if (bottom > maxBottom)
                maxBottom = bottom;
        }
        return maxBottom;
    }

    private void AppendRowsToStructureMeasured(List<List<ControlInStack>> rows)
    {
        var structure = LatestStackStructure.Clone();
        structure.Append(rows);
        if (InitializeTemplatesInBackgroundDelay > 0)
        {
            StackStructure = structure;
        }
        else
        {
            StackStructureMeasured = structure;
        }
    }

    public int MeasureAdditionalItems(int batchSize, int aheadCount, float scale)
    {
        if (ItemsSource == null || ItemsSource.Count == 0)
            return 0;

        int startIndex = LastMeasuredIndex + 1;
        int endIndex = Math.Min(startIndex + batchSize + aheadCount, ItemsSource.Count);



        if (startIndex >= endIndex)
            return 0;

        int countToMeasure = endIndex - startIndex;
        if (countToMeasure <= 0)
            return 0;

        var structure = LatestMeasuredStackStructure.Clone();
        var (startX, startY, startRow, startCol) = GetNextItemPositionForIncremental(structure);
        int columnsCount = (Split > 0) ? Split : 1;

        float columnWidth = ComputeColumnWidth(columnsCount);
        float availableWidth = columnWidth;
        float availableHeight = float.PositiveInfinity;

        if (this.Type == LayoutType.Row)
        {
            availableHeight = columnWidth;
            availableWidth = float.PositiveInfinity;
        }

        var rows = new List<List<ControlInStack>>();
        var cols = new List<ControlInStack>(columnsCount);
        float currentX = startX;
        float currentY = startY;
        float rowHeight = 0f;

        int currentIndex = startIndex;
        int row = startRow;
        int col = startCol;

        float rowWidth = 0;
        var stackHeight = 0.0f;
        var stackWidth = 0.0f;

        SkiaControl template = null;
        bool useOneTemplate = IsTemplated && RecyclingTemplate != RecyclingTemplate.Disabled;

        if (useOneTemplate)
        {
            template = ChildrenFactory.GetTemplateInstance();
        }

        // Measure!

        try
        {

            while (currentIndex < endIndex)
            {
                stackHeight += GetSpacingForIndex(row, scale);

                var child = ChildrenFactory.GetChildAt(currentIndex, template, 0, true);
                if (child == null)
                {
                    return 0;
                }

                var rectForChild = new SKRect(
                    currentX,
                    currentY,
                    currentX + availableWidth,
                    currentY + availableHeight
                );

                var cell = new ControlInStack
                {
                    ControlIndex = currentIndex,
                    Destination = rectForChild,
                };

                var measured = MeasureAndArrangeCell(rectForChild, cell, child, rectForChild, scale);
                cols.Add(cell);

                // Update max row height
                if (measured.Pixels.Height > rowHeight)
                    rowHeight = measured.Pixels.Height;

                rowWidth += measured.Pixels.Width + GetSpacingForIndex(col, scale);

                // Move to next column
                col++;
                if (col >= columnsCount)
                {
                    // The row is complete
                    // Add this completed row to newRows
                    rows.Add(cols);

                    stackHeight += rowHeight;
                    stackWidth +=

                    // start next row
                    row++;
                    col = 0;
                    currentX = 0f;
                    currentY += rowHeight + (float)(Spacing * RenderingScale);
                    rowWidth = 0;
                    rowHeight = 0;
                    cols = new List<ControlInStack>(columnsCount);
                }
                else
                {
                    // Move to next column horizontally
                    currentX += columnWidth + (float)(Spacing * RenderingScale);
                }

                if (rowWidth > stackWidth)
                    stackWidth = rowWidth;

                if (template == null)
                    ChildrenFactory.ReleaseView(child);

                currentIndex++;
            }

            structure.Append(rows);
            if (InitializeTemplatesInBackgroundDelay > 0)
            {
                StackStructure = structure;
            }
            else
            {
                StackStructureMeasured = structure;
            }
            LastMeasuredIndex = startIndex + countToMeasure - 1;

            SKSize newSizePixels;
            var existingHeight = MeasuredSize.Pixels.Height;
            var existingWidth = MeasuredSize.Pixels.Width;

            if (Type == LayoutType.Column)
            {
                float spacingPixels = (float)(Spacing * scale);

                //first additional measurement
                if (_listAdditionalMeasurements == 0)
                {
                    //do not use approx size we have
                    stackHeight = structure.GetChildren().Sum(x => x.Measured.Pixels.Height) + spacingPixels * structure.MaxRows - 1;
                }
                else
                {
                    if (_listAdditionalMeasurements == 1)
                    {
                        //add some more space to be able to scroll
                        stackHeight += 1500 * scale;
                    }
                    if (endIndex == ItemsSource.Count)
                    {
                        stackHeight -= 1500 * scale;
                    }

                    stackHeight += existingHeight + spacingPixels;
                }

                if (existingWidth > stackWidth)
                    stackWidth = existingWidth;

                newSizePixels = new(stackWidth, stackHeight);

                SetMeasured(newSizePixels.Width, newSizePixels.Height, false, false, scale);
            }

            _listAdditionalMeasurements++;

            return countToMeasure;
        }
        finally
        {
            if (template != null)
            {
                ChildrenFactory.ReleaseTemplateInstance(template);
            }
        }

    }


}

public record MeasuredListCell(ControlInStack Cell, int Index);

public class MeasuredListCells : ReadOnlyCollection<MeasuredListCell>
{
    public MeasuredListCells(IList<MeasuredListCell> list) : base(list)
    {

    }
}

