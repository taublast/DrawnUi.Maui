using System.Runtime.CompilerServices;

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

        public LayoutStructure LatestMeasuredStackStructure
        {
            get
            {
                if (StackStructureMeasured != null)
                    return StackStructureMeasured;

                return StackStructure;
            }
        }

        protected ScaledSize MeasureAndArrangeCell(SKRect destination,
            ControlInStack cell, SkiaControl child,
            SKRect rectForChildrenPixels, float scale)
        {
            cell.Area = destination;

            var measured = MeasureChild(child, cell.Area.Width, cell.Area.Height, scale);

            cell.Measured = measured;
            cell.WasMeasured = true;

            LayoutCell(measured, cell, child, rectForChildrenPixels, scale);

            return measured;
        }

        public virtual void LayoutCell(
            ScaledSize measured,
            ControlInStack cell,
            SkiaControl child,
            SKRect rectForChildrenPixels,
            float scale)
        {
            cell.Layout = rectForChildrenPixels;

            if (!measured.IsEmpty)
            {
                var area = cell.Area;
                if (child != null)
                {
                    if (Type == LayoutType.Column && child.VerticalOptions != LayoutOptions.Start && child.VerticalOptions != LayoutOptions.Fill)
                    {
                        area = new(area.Left,
                            rectForChildrenPixels.Top,
                            area.Right,
                            rectForChildrenPixels.Bottom);
                    }
                    else
                    if (Type == LayoutType.Row && child.HorizontalOptions != LayoutOptions.Start && child.HorizontalOptions != LayoutOptions.Fill)
                    {
                        area = new(rectForChildrenPixels.Left,
                            area.Top,
                            rectForChildrenPixels.Right,
                            area.Bottom);
                    }
                }

                child.Arrange(area, measured.Units.Width, measured.Units.Height, scale);

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

        /// <summary>
        /// Cell.Area contains the area for layout
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="child"></param>
        /// <param name="scale"></param>
        public record SecondPassArrange(ControlInStack Cell, SkiaControl Child, float Scale);

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

        protected int GetSizeKey(SKSize size)
        {
            int hKey = 0;
            if (RecyclingTemplate != RecyclingTemplate.Disabled)
            {
                if (Type == LayoutType.Column)
                {
                    hKey = (int)Math.Round(size.Height);
                }
                else
                if (Type == LayoutType.Row)
                {
                    hKey = (int)Math.Round(size.Width);
                }
            }
            return hKey;
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
                bool isRtl = Super.IsRtl; //todo!!!

                ScaledSize measured;
                SKRect rectForChild = rectForChildrenPixels;//.Clone();

                SkiaControl[] nonTemplated = null;
                if (!IsTemplated)
                {
                    //preload with condition..
                    nonTemplated = GetUnorderedSubviews().Where(c => c.CanDraw).ToArray();
                }
                else
                {
                    //List<SkiaControl> dirtyChildren = DirtyChildrenTracker.GetList();
                    //Debug.WriteLine($"Dirty: {dirtyChildren.Count}");
                }

                bool smartMeasuring = false;

                /*
                List<SkiaControl> dirtyChildren = DirtyChildrenTracker.GetList();

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
                else
                {


                }
                */

                SkiaControl template = null;
                ControlInStack firstCell = null;
                measured = ScaledSize.Default;

                var stackHeight = 0.0f;
                var stackWidth = 0.0f;

                var layoutStructure = BuildStackStructure(scale);

                bool useOneTemplate = IsTemplated && //ItemSizingStrategy == ItemSizingStrategy.MeasureFirstItem &&
                                      RecyclingTemplate != RecyclingTemplate.Disabled;

                if (useOneTemplate)
                {
                    template = ChildrenFactory.GetTemplateInstance();
                }

                var maybeSecondPass = true;
                List<SecondPassArrange> listSecondPass = new();
                bool stopMeasuring = false;
                ScaledRect visibleArea = _viewport;

                //measure
                //left to right, top to bottom
                var index = -1;
                for (var row = 0; row < layoutStructure.MaxRows; row++)
                {
                    if (stopMeasuring)
                    {
                        break;
                    }

                    index++;
                    var maxHeight = 0.0f;
                    var maxWidth = 0.0f;

                    var columnsCount = layoutStructure.GetColumnCountForRow(row);

                    var needMeasureAll = true;
                    if (useOneTemplate)
                    {
                        needMeasureAll = RecyclingTemplate == RecyclingTemplate.Disabled ||
                                         MeasureItemsStrategy == MeasuringStrategy.MeasureAll ||
                                         (MeasureItemsStrategy == MeasuringStrategy.MeasureFirst
                                          && columnsCount != Split)
                                         || !(MeasureItemsStrategy == MeasuringStrategy.MeasureFirst
                                              && firstCell != null);
                    }

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
                                child = ChildrenFactory.GetChildAt(cell.ControlIndex, template, 0, true);
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
                                bool needMeasure =
                                    needMeasureAll ||
                                    (MeasureItemsStrategy == MeasuringStrategy.MeasureFirst && columnsCount != Split)
                                    || !(MeasureItemsStrategy == MeasuringStrategy.MeasureFirst && firstCell != null);

                                if (needMeasure)
                                {
                                    measured = MeasureAndArrangeCell(rectFitChild, cell, child, rectForChildrenPixels, scale);
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
                                    cell.WasMeasured = true;
                                }
                            }
                            else
                            {
                                measured = MeasureAndArrangeCell(rectFitChild, cell, child, rectForChildrenPixels, scale);

                                if (maybeSecondPass) //has infinity in destination
                                {
                                    if (Type == LayoutType.Column && child.HorizontalOptions != LayoutOptions.Start)
                                    {
                                        listSecondPass.Add(new(cell, child, scale));
                                    }
                                    else
                                    if (Type == LayoutType.Row && child.VerticalOptions != LayoutOptions.Start)
                                    {
                                        listSecondPass.Add(new(cell, child, scale));
                                    }
                                }
                            }

                            if (!measured.IsEmpty)
                            {
                                maxWidth += measured.Pixels.Width + GetSpacingForIndex(column, scale);

                                if (measured.Pixels.Height > maxHeight)
                                    maxHeight = measured.Pixels.Height;

                                //offset -->
                                rectForChild.Left += (float)(measured.Pixels.Width);
                            }

                            cell.WasMeasured = true;

                            if (IsTemplated && MeasureItemsStrategy == MeasuringStrategy.MeasureVisible)
                            {
                                if (!visibleArea.Pixels.IntersectsWithInclusive(cell.Destination))
                                {
                                    stopMeasuring = true;
                                    break;
                                }
                            }

                            if (!useOneTemplate && IsTemplated)
                            {
                                ChildrenFactory.ReleaseView(child);
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

                if (useOneTemplate)
                {
                    ChildrenFactory.ReleaseTemplateInstance(template);
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


        #endregion
    }
}
