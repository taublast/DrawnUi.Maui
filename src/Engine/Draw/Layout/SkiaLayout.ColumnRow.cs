using DrawnUi.Maui.Draw;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DrawnUi.Maui.Draw
{
    public partial class SkiaLayout
    {
        #region StackLayout


        /// <summary>
        /// Used for StackLayout (Stack, Row) kind of layout
        /// </summary>
        public LayoutStructure StackStructure { get; set; }

        /// <summary>
        /// When measuring we set this, and it will be swapped with StackStructure upon drawing so we don't affect the drawing if measuring in background.
        /// </summary>
        public LayoutStructure StackStructureMeasured { get; set; }

        public LayoutStructure LatestStackStructure
        {
            get
            {
                if (StackStructure != null)
                    return StackStructure;

                return StackStructureMeasured;
            }
        }

        protected ScaledSize MeasureAndArrangeCell(SKRect destination, ControlInStack cell, SkiaControl child, float scale)
        {
            cell.Area = destination;

            var measured = MeasureChild(child, cell.Area.Width, cell.Area.Height, scale);

            cell.Measured = measured;

            LayoutCell(measured, cell, child, scale);

            return measured;
        }

        void LayoutCell(ScaledSize measured, ControlInStack cell, SkiaControl child, float scale)
        {
            if (!measured.IsEmpty)
            {
                child.Arrange(cell.Area, measured.Units.Width, measured.Units.Height, scale);

                var maybeArranged = child.Destination;

                var arranged =
                    new SKRect(cell.Area.Left, cell.Area.Top,
                        cell.Area.Left + cell.Measured.Pixels.Width,
                        cell.Area.Top + cell.Measured.Pixels.Height);

                if (float.IsNormal(maybeArranged.Height))
                {
                    arranged.Top = maybeArranged.Top;
                    arranged.Bottom = maybeArranged.Bottom;
                }
                if (float.IsNormal(maybeArranged.Width))
                {
                    arranged.Left = maybeArranged.Left;
                    arranged.Right = maybeArranged.Right;
                }

                cell.Destination = arranged;
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float GetSpacingForIndex(int forIndex, float scale)
        {
            var spacing = 0.0f;
            if (forIndex > 0)
            {
                spacing = (float)Math.Round(Spacing * scale);
            }
            return spacing;
        }

        /// <summary>
        /// Measuring column/row
        /// </summary>
        /// <param name="rectForChildrenPixels"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public virtual ScaledSize MeasureStack(SKRect rectForChildrenPixels, float scale)
        {
            if (ChildrenFactory.GetChildrenCount() > 0)
            {
                ScaledSize measured;
                SKRect rectForChild = rectForChildrenPixels;//.Clone();

                SkiaControl[] nonTemplated = null;
                if (!IsTemplated)
                {
                    //preload with condition..
                    nonTemplated = GetUnorderedSubviews().Where(c => c.CanDraw).ToArray();
                }

                bool smartMeasuring = false;

                /*
                List<SkiaControl> dirtyChildren = DirtyChildren.GetList();

                if (Superview != null)
                {
                    //enable measuring one changed item in foreground only,
                    //for background thread need full measurement
                    smartMeasuring =
                        WasMeasured
                        && dirtyChildren.Count > 0
                        && Superview.DrawingThreadId == Thread.CurrentThread.ManagedThreadId
                                     && UsingCacheType != SkiaCacheType.ImageDoubleBuffered;
                }

                var dirty = dirtyChildren.FirstOrDefault();
                if (smartMeasuring && dirty != null)
                {
                    if (Tag == "InsectStack")
                    {
                        Super.Log($"[S] Measuring, smart ON");
                    }

                    //measure only changed child
                    var viewIndex = -1;
                    if (IsTemplated)
                    {
                        viewIndex = dirty.ContextIndex;
                        if (viewIndex >= 0)
                        {
                            ScaledSize newContentSize = null;
                            SKSize sizeChange = new();

                            IReadOnlyList<SkiaControl> views = null;
                            if (!IsTemplated)
                            {
                                views = GetUnorderedSubviews();
                            }

                            var index = -1;
                            foreach (var cell in LatestStackStructure.GetChildren())
                            {
                                index++;

                                if (newContentSize != null)
                                {
                                    //Offset the subsequent children
                                    cell.Area = new SKRect(
                                        cell.Area.Left + sizeChange.Width,
                                        cell.Area.Top + sizeChange.Height,
                                        cell.Area.Right + sizeChange.Width,
                                        cell.Area.Bottom + sizeChange.Height);

                                    //todo layout cell ?
                                    if (views != null)
                                    {
                                        LayoutCell(cell.Measured, cell, views[index], scale);
                                    }
                                }
                                else
                                if (cell.ControlIndex == viewIndex)
                                {
                                    //Measure only DirtyChild
                                    measured = MeasureAndArrangeCell(cell.Area, cell, dirty, scale);

                                    //todo offset other children accroding new size of this cell
                                    //and adjust new content size to be returned

                                    sizeChange = new SKSize(measured.Pixels.Width - cell.Measured.Pixels.Width,
                                        measured.Pixels.Height - cell.Measured.Pixels.Height);

                                    newContentSize = ScaledSize.FromPixels(MeasuredSize.Pixels.Width + sizeChange.Width, MeasuredSize.Pixels.Height + sizeChange.Height, scale);
                                }
                            }

                            if (newContentSize != null)
                            {
                                return newContentSize;
                            }
                        }
                    }
                    else
                    if (false) //todo for non templated too!
                    {
                        viewIndex = nonTemplated.FindIndex(dirty);
                        if (viewIndex >= 0)
                        {
                            ScaledSize newContentSize = null;
                            SKSize sizeChange = new();

                            IReadOnlyList<SkiaControl> views = null;
                            if (!IsTemplated)
                            {
                                views = GetUnorderedSubviews();
                            }

                            var index = -1;
                            foreach (var cell in LatestStackStructure.GetChildren())
                            {
                                index++;

                                if (newContentSize != null)
                                {
                                    //Offset the subsequent children
                                    cell.Area = new SKRect(
                                        cell.Area.Left + sizeChange.Width,
                                        cell.Area.Top + sizeChange.Height,
                                        cell.Area.Right + sizeChange.Width,
                                        cell.Area.Bottom + sizeChange.Height);

                                    //todo layout cell ?
                                    if (views != null)
                                    {
                                        LayoutCell(cell.Measured, cell, views[index], scale);
                                    }
                                }
                                else
                                if (cell.ControlIndex == viewIndex)
                                {
                                    if (dirty.CanDraw)
                                    {
                                        //Measure only DirtyChild
                                        measured = MeasureAndArrangeCell(cell.Area, cell, dirty, scale);

                                        //todo offset other children accroding new size of this cell
                                        //and adjust new content size to be returned

                                        sizeChange = new SKSize(measured.Pixels.Width - cell.Measured.Pixels.Width,
                                            measured.Pixels.Height - cell.Measured.Pixels.Height);

                                        newContentSize = ScaledSize.FromPixels(MeasuredSize.Pixels.Width + sizeChange.Width, MeasuredSize.Pixels.Height + sizeChange.Height, scale);
                                    }
                                    else
                                    {
                                        if (cell.Measured != ScaledSize.Default)
                                        {
                                            //add new space
                                            sizeChange = new SKSize(measured.Pixels.Width + cell.Measured.Pixels.Width,
                                                measured.Pixels.Height + cell.Measured.Pixels.Height);

                                            newContentSize = ScaledSize.FromPixels(MeasuredSize.Pixels.Width - sizeChange.Width, MeasuredSize.Pixels.Height - sizeChange.Height, scale);
                                        }
                                        cell.Measured = ScaledSize.Default;
                                    }
                                }



                            }

                            if (newContentSize != null)
                            {
                                return newContentSize;
                            }
                        }
                    }
                }
                else
                {
                    if (Tag == "InsectStack")
                    {
                        Super.Log($"[S] Measuring, smart OFF");
                    }
                }
                */

                SkiaControl template = null;
                ControlInStack firstCell = null;
                measured = ScaledSize.Default;

                var stackHeight = 0.0f;
                var stackWidth = 0.0f;

                var layoutStructure = BuildStackStructure(scale);

                bool useOneTemplate =
                                       //ItemSizingStrategy == ItemSizingStrategy.MeasureFirstItem &&
                                       RecyclingTemplate == RecyclingTemplate.Enabled;

                if (IsTemplated && useOneTemplate)
                {
                    template = ChildrenFactory.GetTemplateInstance();
                }

                //measure
                //left to right, top to bottom
                for (var row = 0; row < layoutStructure.MaxRows; row++)
                {
                    var maxHeight = 0.0f;
                    var maxWidth = 0.0f;

                    var columnsCount = layoutStructure.GetColumnCountForRow(row);

                    if (!DynamicColumns && columnsCount < Split)
                    {
                        columnsCount = Split;
                    }

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
                            if (layoutStructure.GetColumnCountForRow(row) < column + 1)
                                continue; //case when we last row with less items to fill all columns

                            var cell = layoutStructure.Get(column, row);

                            SkiaControl child = null;
                            if (IsTemplated)
                            {
                                child = ChildrenFactory.GetChildAt(cell.ControlIndex, template);
                            }
                            else
                            {
                                child = nonTemplated[cell.ControlIndex];
                            }

                            if (child == null)
                            {
                                Trace.WriteLine($"[MeasureStack] FAILED to get child at index {cell.ControlIndex}");
                                return ScaledSize.Default;
                            }

                            if (!child.CanDraw)
                            {
                                cell.Measured = ScaledSize.Default;
                            }

                            if (column == 0)
                                rectForChild.Top += GetSpacingForIndex(row, scale);

                            rectForChild.Left += GetSpacingForIndex(column, scale);
                            var rectFitChild = new SKRect(rectForChild.Left, rectForChild.Top, rectForChild.Left + widthPerColumn, rectForChild.Bottom);

                            if (IsTemplated)
                            {
                                bool needMeasure = (ItemSizingStrategy == ItemSizingStrategy.MeasureFirstItem && columnsCount != Split) || !(ItemSizingStrategy == ItemSizingStrategy.MeasureFirstItem && firstCell != null);
                                if (needMeasure)
                                {
                                    measured = MeasureAndArrangeCell(rectFitChild, cell, child, scale);
                                    firstCell = cell;
                                }
                                else
                                {
                                    //apply first measured size to cell
                                    var offsetX = rectFitChild.Left - firstCell.Area.Left;
                                    var offsetY = rectFitChild.Top - firstCell.Area.Top;
                                    var arranged = firstCell.Destination;
                                    arranged.Offset(new(offsetX, offsetY));

                                    cell.Area = rectFitChild;
                                    cell.Measured = measured.Clone();
                                    cell.Destination = arranged;
                                }
                            }
                            else
                            {
                                measured = MeasureAndArrangeCell(rectFitChild, cell, child, scale);
                            }

                            if (!measured.IsEmpty)
                            {
                                maxWidth += measured.Pixels.Width + GetSpacingForIndex(column, scale);

                                if (measured.Pixels.Height > maxHeight)
                                    maxHeight = measured.Pixels.Height;

                                //offset -->
                                rectForChild.Left += (float)(measured.Pixels.Width);
                            }

                        }
                        catch (Exception e)
                        {
                            Super.Log(e);
                            break;
                        }

                    }//end of iterate columns

                    if (maxWidth > stackWidth)
                        stackWidth = maxWidth;

                    stackHeight += maxHeight + GetSpacingForIndex(row, scale);
                    rectForChild.Top += (float)(maxHeight);

                    rectForChild.Left = 0; //reset to start

                }//end of iterate rows

                if (IsTemplated && useOneTemplate)
                {
                    ChildrenFactory.ReleaseView(template);
                }

                if (HorizontalOptions.Alignment == LayoutAlignment.Fill && WidthRequest < 0)
                {
                    stackWidth = rectForChildrenPixels.Width;
                }
                if (VerticalOptions.Alignment == LayoutAlignment.Fill && HeightRequest < 0)
                {
                    stackHeight = rectForChildrenPixels.Height;
                }

                return ScaledSize.FromPixels(stackWidth, stackHeight, scale);
            }

            return ScaledSize.FromPixels(rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);
        }

        private LayoutStructure BuildStackStructure(float scale)
        {

            //build stack grid
            //fill table
            var column = 0;
            var row = 0;
            var rows = new List<List<ControlInStack>>();
            var columns = new List<ControlInStack>();
            var maxColumns = Split;
            int maxRows = 0;

            //returns true if can continue
            bool ProcessStructure(int i, SkiaControl control)
            {
                var add = new ControlInStack
                {
                    ControlIndex = i,
                    View = control
                };
                if (control != null)
                {
                    add.ZIndex = control.ZIndex;
                    add.ControlIndex = i;
                }

                if (Type == LayoutType.Row)
                {
                    var stop = 1;
                }

                // vertical stack or if maxColumns is exceeded
                if (Type == LayoutType.Column && column >= maxColumns
                    || Type == LayoutType.Row && (maxColumns > 0 && column >= maxColumns)
                    || LineBreaks.Contains(i))
                {
                    if (i > 0)
                    {
                        //insert a vbreak between all children
                        rows.Add(columns);
                        columns = new();
                        column = 0;
                        row++;
                    }
                }

                // If maxRows is reached and exceeded, break the loop
                if (maxRows > 0 && row >= maxRows)
                {
                    return false;
                }

                columns.Add(add);
                column++;

                return true;
            }

            if (!IsTemplated)
            {
                var index = -1;
                foreach (var view in GetUnorderedSubviews())
                {
                    if (!view.CanDraw) //this is a critical point, we do not store invisible stuff in structure!
                        continue;

                    index++;
                    if (!ProcessStructure(index, view))
                        break;
                }
            }
            else
            {
                var childrenCount = ChildrenFactory.GetChildrenCount();
                for (int index = 0; index < childrenCount; index++)
                {
                    if (!ProcessStructure(index, null))
                        break;
                }
            }

            rows.Add(columns);

            var structure = new LayoutStructure(rows);

            if (InitializeTemplatesInBackgroundDelay > 0)
            {
                StackStructure = structure;
            }
            else
            {
                StackStructureMeasured = structure;
            }

            return structure;
        }

        //2 passes for FILL LAYOUT OPTIONS
        //not using this as fps drops
        /*
    case LayoutType.Column:
    case LayoutType.Row:

        if (ViewsMaster.GetChildrenCount() > 0)
        {
            float AddSpacing(int pos)
            {
                var spacing = 0.0f;
                if (pos > 0)
                {
                    spacing = (float)(Spacing * scale);
                }
                return spacing;
            }

            SKRect rectForChild = rectForChildrenPixels.Clone();

            var column = 0;
            var row = 0;
            var rows = new List<List<ControlInStack>>();
            var columns = new List<ControlInStack>();
            int maxColumns = MaxColumns; // New MaxColumns property
            int maxRows = MaxRows; // New MaxRows property

            for (int index = 0; index < ViewsMaster.GetChildrenCount(); index++)
            {
                // vertical stack or if maxColumns is exceeded
                if (Type == LayoutType.Column && maxColumns < 1 || (maxColumns > 0 && column >= maxColumns) || LineBreaks.Contains(index))
                {
                    if (index > 0)
                    {
                        //insert a vbreak between all children
                        rows.Add(columns);
                        columns = new();
                        column = 0;
                        row++;
                    }
                }

                // If maxRows is reached and exceeded, break the loop
                if (maxRows > 0 && row >= maxRows)
                {
                    break;
                }

                columns.Add(new ControlInStack { ControlIndex = index });
                column++;
            }

            rows.Add(columns);
            StackStructure = rows;

            SkiaControl template = null;
            if (IsTemplated)
            {
                template = ViewsMaster.GetTemplateInstance();
            }

            var stackHeight = 0.0f;
            var stackWidth = 0.0f;
            var maxHeight = 0.0f;
            var maxWidth = 0.0f;
            bool hasFills = false;

            var takenHeight = 0f;
            var takenWidth = 0f;

            //PASS 1
            for (row = 0; row < rows.Count; row++)
            {
                maxHeight = 0.0f; //max row height
                rectForChild.Top += AddSpacing(row);

                stackWidth = 0.0f;
                var columnsCount = rows[row].Count;

                if (!DynamicColumns && columnsCount < maxColumns)
                {
                    columnsCount = Math.Min(1, MaxColumns);
                }

                var widthPerColumn = (float)(columnsCount > 1 ?
                    (rectForChildrenPixels.Width - (columnsCount - 1) * Spacing * scale) / columnsCount :
                    rectForChildrenPixels.Width);

                for (column = 0; column < columnsCount; column++)
                {
                    rectForChild.Left += AddSpacing(column);

                    var rectFitChild = new SKRect(rectForChild.Left, rectForChild.Top, rectForChild.Left + widthPerColumn, rectForChild.Bottom);

                    var cell = rows[row][column];

                    var child = ViewsMaster.GetChildAt(cell.ControlIndex, template);

                    //Trace.WriteLine($"[PASS 1] LAYOUT - {child.Tag}");

                    if (child == null)
                    {
                        ContentSize = ScaledSize.FromPixels(rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);
                        widthConstraint = AdaptWidthContraintToContentRequest(widthConstraint, ContentSize, constraintLeft + constraintRight);
                        heightConstraint = AdaptHeightContraintToContentRequest(heightConstraint, ContentSize, constraintTop + constraintBottom);
                        return SetMeasured(widthConstraint, heightConstraint, scale);
                    }
                    else
                    {
                        //reset calculated stuff as this might be a template being reused
                        if (child is SkiaControl control)
                        {
                            control.InvalidateInternal();
                        }
                    }

                    if (
                        (Type == LayoutType.Row && child.HorizontalOptions.Alignment == LayoutAlignment.Fill && child.WidthRequest < 0)
                        ||
                        (Type == LayoutType.Column && child.VerticalOptions.Alignment == LayoutAlignment.Fill && child.HeightRequest < 0)
                        )
                    {
                        hasFills = true;
                        cell.Tmp = rectFitChild;
                        cell.Expands = true;
                        continue;
                    }

                    cell.Expands = false;

                    var measured = MeasureChild(child,
                        rectFitChild.Width, rectFitChild.Height,
                    scale);

                    cell.Measured = ScaledSize.FromPixels(measured, scale);

                    if (measured != SKSize.Empty)
                    {
                        child.Arrange(rectFitChild, measured.Width, measured.Height, scale);

                        var maybeArranged = child.Destination;

                        var arranged = new SKRect(rectFitChild.Left, rectFitChild.Top,
                                rectFitChild.Left + cell.Measured.Pixels.Width,
                                rectFitChild.Top + cell.Measured.Pixels.Height);

                        if (float.IsNormal(maybeArranged.Height))
                        {
                            arranged.Top = maybeArranged.Top;
                            arranged.Bottom = maybeArranged.Bottom;
                        }
                        if (float.IsNormal(maybeArranged.Width))
                        {
                            arranged.Left = maybeArranged.Left;
                            arranged.Right = maybeArranged.Right;
                        }

                        cell.Destination = arranged;

                        var width = measured.Width;
                        var height = measured.Height;

                        stackWidth += width + AddSpacing(column);

                        if (measured.Height > maxHeight)
                            maxHeight = height;

                        //offset -->
                        rectForChild.Left += (float)(width);
                    }

                }//end of iterate columns

                if (stackWidth > maxWidth)
                    maxWidth = stackWidth;

                stackHeight += maxHeight + AddSpacing(row);
                rectForChild.Top += (float)(maxHeight);

                rectForChild.Left = 0; //reset to start

            }//end of iterate rows

            //PASS 2
            if (hasFills)
            {
                rectForChild = rectForChildrenPixels.Clone();

                var offsetMoveY = 0f;

                for (row = 0; row < rows.Count; row++)
                {
                    var offsetMoveX = 0f;

                    rectForChild.Top += AddSpacing(row);
                    stackWidth = 0.0f;
                    maxHeight = 0.0f;

                    var columnsCount = rows[row].Count;

                    if (!DynamicColumns && columnsCount < maxColumns)
                    {
                        columnsCount = Math.Min(1, MaxColumns);
                    }

                    var widthPerColumn = (float)(columnsCount > 1 ?
                        (rectForChildrenPixels.Width - (columnsCount - 1) * Spacing * scale) / columnsCount :
                        rectForChildrenPixels.Width);

                    for (column = 0; column < columnsCount; column++)
                    {

                        var cell = rows[row][column];

                        var child = ViewsMaster.GetChildAt(cell.ControlIndex, template);

                        if (!cell.Expands)
                        {
                            if (offsetMoveY > 0 || offsetMoveX > 0)
                            {
                                //newly filled made us move
                                var itBecaime = new SKRect(cell.Destination.Left + offsetMoveX, cell.Destination.Top + offsetMoveY,
                                    cell.Destination.Right + offsetMoveX, cell.Destination.Bottom + offsetMoveY);
                                cell.Destination = itBecaime;
                            }
                            rectForChild.Left += cell.Measured.Pixels.Width + AddSpacing(column);

                            //usual end of row
                            stackWidth += cell.Measured.Pixels.Width + AddSpacing(column);
                            maxHeight = cell.Measured.Pixels.Height;
                            continue;
                        }

                        var availableWidth = rectForChildrenPixels.Width -
                                                 (rectForChild.Left - rectForChildrenPixels.Left)
                                             - CalculateTakenWidthRight(row, column, (float)(Spacing * scale));

                        var availableHeight = rectForChildrenPixels.Height -
                                              (rectForChild.Top - rectForChildrenPixels.Top)
                                              - CalculateTakenHeightBelow(row, (float)(Spacing * scale));

                        //Trace.WriteLine($"[PASS 2] LAYOUT - {child.Tag}");

                        var measured = MeasureChild(child,
                            availableWidth, availableHeight, scale);

                        cell.Measured = ScaledSize.FromPixels(measured, scale);

                        if (measured != SKSize.Empty)
                        {
                            //child.InvalidateChildren();
                            child.Arrange(new SKRect(rectForChild.Left, rectForChild.Top, rectForChild.Left + availableWidth, rectForChild.Top + availableHeight),
                                measured.Width, measured.Height, scale);

                            //child.InvalidateChildren();

                            var maybeArranged = child.Destination;

                            var arranged = new SKRect(cell.Tmp.Left, cell.Tmp.Top,
                                    cell.Tmp.Left + cell.Measured.Pixels.Width,
                                    cell.Tmp.Top + cell.Measured.Pixels.Height);

                            if (float.IsNormal(maybeArranged.Height))
                            {
                                arranged.Top = maybeArranged.Top;
                                arranged.Bottom = maybeArranged.Bottom;
                            }
                            if (float.IsNormal(maybeArranged.Width))
                            {
                                arranged.Left = maybeArranged.Left;
                                arranged.Right = maybeArranged.Right;
                            }

                            cell.Destination = arranged;

                            var width = measured.Width;
                            var height = measured.Height;

                            offsetMoveY += height;
                            offsetMoveX += width;

                            stackWidth += width + AddSpacing(column);

                            if (measured.Height > maxHeight)
                                maxHeight = height;

                            //offset -->
                            rectForChild.Left += (float)(width);
                        }
                    }//end of iterate columns

                    if (stackWidth > maxWidth)
                        maxWidth = stackWidth;

                    stackHeight += maxHeight + AddSpacing(row);
                    rectForChild.Top += (float)(maxHeight);
                    rectForChild.Left = 0; //reset to start

                }//end of iterate rows

            }

            if (IsTemplated)
            {
                ViewsMaster.ReleaseView(template);
            }

            ContentSize = ScaledSize.FromPixels(maxWidth, stackHeight, scale);

            widthConstraint = AdaptWidthContraintToContentRequest(widthConstraint, ContentSize, constraintLeft + constraintRight);
            heightConstraint = AdaptHeightContraintToContentRequest(heightConstraint, ContentSize, constraintBottom + constraintTop);

            childrenmeasured = true;
        }

        break;
        */



        #endregion
    }
}
