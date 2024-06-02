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

        /// <summary>
        /// Renders stack layout.
        /// Returns number of drawn children.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destination"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        protected virtual int DrawChildrenStack(SkiaDrawingContext context, SKRect destination, float scale)
        {
            var drawn = 0;
            //StackStructure was creating inside Measure.
            //While scrolling templated its not called again (checked).

            List<SkiaControlWithRect> tree = new();

            var needrebuild = templatesInvalidated;
            List<ControlInStack> visibleElements = new();

            var structure = LatestStackStructure;
            if (structure != null)
            {
                //draw children manually

                var visibleArea = GetOnScreenVisibleArea();

                //PASS 1 - VISIBILITY
                //we need this pass before drawing to recycle views that became hidden
                var viewsTotal = 0;

                foreach (var cell in structure.GetChildren())
                {

                    viewsTotal++;

                    if (cell.Destination == SKRect.Empty)
                    {
                        cell.IsVisible = false;
                    }
                    else
                    {
                        var x = destination.Left + cell.Destination.Left;
                        var y = destination.Top + cell.Destination.Top;

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

                    if (!cell.IsVisible)
                    {
                        ChildrenFactory.MarkViewAsHidden(cell.ControlIndex);
                    }
                    else
                    {
                        visibleElements.Add(cell);
                    }
                }


                //PASS 2 DRAW VISIBLE
                //using precalculated rects
                bool wasVisible = false;
                var index = -1;

                //todo modify structure upon ZIndex or something... do not order on every frame!!!!!
                foreach (var cell in visibleElements)//.OrderBy(x => x.ZIndex))
                {
                    index++;

                    SkiaControl child = null;
                    if (IsTemplated)
                    {
                        if (!ChildrenFactory.TemplatesAvailable && InitializeTemplatesInBackgroundDelay > 0)
                        {
                            break; //itemssource was changed by other thread
                        }
                        child = ChildrenFactory.GetChildAt(cell.ControlIndex, null);
                    }
                    else
                    {
                        try
                        {
                            child = cell.View;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[HANDLED] {ex}");
                            child = null;
                        }

                    }

                    if (child == null) //ChildrenFactory.GetChildAt was unable to return child?..
                    {
                        //NeedMeasure = true;
                        return drawn;
                    }

                    if (child is SkiaControl control && child.IsVisible)
                    {
                        SKRect destinationRect;
                        if (IsTemplated && ItemSizingStrategy == ItemSizingStrategy.MeasureAllItems)
                        {
                            //when context changes we need all available space for remeasuring cell
                            destinationRect = new SKRect(cell.Drawn.Left, cell.Drawn.Top, cell.Drawn.Left + cell.Area.Width, cell.Drawn.Top + cell.Area.Bottom);
                        }
                        else
                        {
                            destinationRect = new SKRect(cell.Drawn.Left, cell.Drawn.Top, cell.Drawn.Right, cell.Drawn.Bottom);
                        }

                        //fixes case we changed size of columns/cells and there where already measured..
                        /*
                        if (IsTemplated
                            && (DynamicColumns || ItemSizingStrategy == Microsoft.Maui.Controls.ItemSizingStrategy.MeasureAllItems)
                            && RecyclingTemplate == RecyclingTemplate.Enabled
                            && child.RenderedAtDestination != SKRect.Empty
                            && (destinationRect.Width != child.RenderedAtDestination.Width
                                || destinationRect.Height != child.RenderedAtDestination.Height))
                        {
                            //size is different but template is the same - need to invalidate!
                            //for example same template rendering on 2 columns in one row and 1 column on the last one
                            InvalidateChildrenTree(control);
                        }
                        */

                        DrawChild(context, destinationRect, child, scale);
                        drawn++;

                        //gonna use that for gestures and for item inside viewport detection and for hotreload children tree

                        tree.Add(new SkiaControlWithRect(control,
                            destinationRect, control.LastDrawnAt,
                            index));

                    }
                }
            }

            //_stopwatchRender.Restart();

            if (needrebuild && visibleElements.Count > 0)
            {
                //reserve for one row above and one row below
                var row = Split;
                if (row < 1)
                    row = 1;
                var reserve = row * 3;
                if (IsTemplated
                    && RecyclingTemplate == RecyclingTemplate.Enabled
                    && ChildrenFactory.AddedMore < reserve)
                {
                    //basically we have to do this here becase now we know the quantity
                    //of visible cells onscreen. so we can oversize the pool a bit to avoid
                    //a lag spike when scrolling would start.
                    Task.Run(async () =>
                    {

                        ChildrenFactory.AddMoreToPool(reserve);

                    }).ConfigureAwait(false);
                }

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

            //if (IsTemplated)
            //{
            //    Trace.WriteLine(ChildrenFactory.GetDebugInfo());
            //}

            return drawn;
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
        /// TODO for templated measure only visible?! and just reserve predicted scroll amount for scrolling
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
