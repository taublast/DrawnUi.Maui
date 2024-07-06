using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DrawnUi.Maui.Draw
{
    public partial class SkiaLayout
    {

        /// <summary>
        /// TODO for templated measure only visible?! and just reserve predicted scroll amount for scrolling
        /// </summary>
        /// <param name="rectForChildrenPixels"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public virtual ScaledSize MeasureWrap(SKRect rectForChildrenPixels, float scale)
        {
            var layout = new BuildWrapLayout(this);

            var measuredLayout = layout.Build(rectForChildrenPixels, scale);
            return measuredLayout;
        }


        /// <summary>
        /// Renders stack/wrap layout.
        /// Returns number of drawn children.
        /// </summary>
        protected virtual int DrawStack(
            LayoutStructure structure,
            SkiaDrawingContext context, SKRect destination, float scale)
        {
            var drawn = 0;
            //StackStructure was creating inside Measure.
            //While scrolling templated its not called again (checked).

            List<SkiaControlWithRect> tree = new();

            var needrebuild = templatesInvalidated;
            List<ControlInStack> visibleElements = new();

            if (structure != null)
            {
                //draw children manually

                var visibleArea = GetOnScreenVisibleArea();

                //PASS 1 - VISIBILITY
                //we need this pass before drawing to recycle views that became hidden
                var currentIndex = -1;
                foreach (var cell in structure.GetChildrenAsSpans())
                {
                    currentIndex++;

                    if (cell.Destination == SKRect.Empty || cell.Measured.Pixels.IsEmpty)
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
                //SkiaControl[] nonTemplated = null;

                int countRendered = 0;

                foreach (var cell in CollectionsMarshal.AsSpan(visibleElements))
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
                        child = cell.View;
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

                        if (IsRenderingWithComposition)
                        {
                            if (DirtyChildrenInternal.Contains(child))
                            {
                                DrawChild(context, destinationRect, child, scale);
                                countRendered++;
                            }
                            else
                            {
                                //skip drawing but need arrange :(
                                //todo set virtual offset between drawnrect and the new
                                child.Arrange(destinationRect, child.SizeRequest.Width, child.SizeRequest.Height, scale);
                            }
                        }
                        else
                        {
                            DrawChild(context, destinationRect, child, scale);
                            countRendered++;
                        }

                        drawn++;

                        //gonna use that for gestures and for item inside viewport detection and for hotreload children tree
                        tree.Add(new SkiaControlWithRect(control,
                            destinationRect,
                            control.LastDrawnAt,
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
                    Tasks.StartDelayedAsync(TimeSpan.FromMilliseconds(30), async () =>
                    {
                        ChildrenFactory.AddMoreToPool(reserve);
                    });
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


        /// <summary>
        /// Implementation for LayoutType.Wrap 
        /// </summary>
        public class BuildWrapLayout : StackLayoutStructure
        {
            public BuildWrapLayout(SkiaLayout layout) : base(layout)
            {
            }

            public override ScaledSize Build(SKRect rectForChildrenPixels, float scale)
            {


                bool isRtl = Super.IsRtl;

                //content size
                float stackHeight = 0.0f;
                float stackWidth = 0.0f;

                // row size
                float maxHeight = 0.0f;
                float currentLineWidth = 0.0f;
                float currentLineRealWidth = 0.0f;

                // for autosize
                float maxWidth = 0.0f;

                var structure = new LayoutStructure();

                //for current row, to layout upon finalizing
                var cellsToLayoutLater = new List<ControlInStack>();

                var maxAvailableSpace = rectForChildrenPixels.Width;
                float sizePerChunk = maxAvailableSpace;

                SKRect rectForChild = rectForChildrenPixels;
                bool useFixedSplitSize = _layout.Split > 0;
                bool alignSplit = _layout.SplitAlign && useFixedSplitSize;

                if (_layout.Split > 1)
                {
                    sizePerChunk = (float)Math.Round(
                        (maxAvailableSpace - (_layout.Split - 1) * _layout.Spacing * scale) / _layout.Split);
                }

                var column = 0;
                var row = 0;
                bool rowFinalized = true;
                bool colFinalized = true;

                SKRect rectFitChild = rectForChild; //will be adjusted by StartColumn
                if (isRtl)
                {
                    rectForChild.Left = rectForChildrenPixels.Right - sizePerChunk;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void StartRow()
                {
                    rowFinalized = false;

                    maxHeight = 0.0f;
                    currentLineWidth = 0f;
                    currentLineRealWidth = 0f;

                    rectForChild.Left = isRtl ? rectForChildrenPixels.Right : 0;  //reset to start

                    var add = GetSpacingForIndex(row, scale);

                    stackHeight += add;
                    rectForChild.Top += add;

                    column = 0;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void FinalizeRow(float addHeight)
                {
                    rowFinalized = true;

                    if (currentLineRealWidth > maxWidth)
                    {
                        maxWidth = currentLineRealWidth;
                    }

                    if (currentLineWidth > stackWidth)
                        stackWidth = currentLineWidth;

                    //layout cells in this row for the final height:
                    LayoutCellsInternal(); //todo move to  second pass !!!

                    stackHeight += addHeight;
                    rectForChild.Top += addHeight;

                    row++;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void LayoutCellsInternal()
                {
                    //layout cells in this row for the final height:
                    if (!_layout.IsTemplated)
                    {
                        var layoutMaxHeight = float.IsFinite(rectFitChild.Height) ? rectFitChild.Height : maxHeight;
                        var layoutHeight = _layout.VerticalOptions == LayoutOptions.Fill
                            ? layoutMaxHeight : maxHeight;

                        foreach (var controlInStack in CollectionsMarshal.AsSpan(cellsToLayoutLater))
                        {
                            //todo adjust cell.Area for the new height
                            controlInStack.Area = new(
                                controlInStack.Area.Left,
                                controlInStack.Area.Top,
                                controlInStack.Area.Right,
                                controlInStack.Area.Top + layoutHeight);

                            LayoutCell(controlInStack, controlInStack.View, scale);
                        }
                    }
                    cellsToLayoutLater.Clear();
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void StartColumn()
                {
                    colFinalized = false;

                    var add = GetSpacingForIndex(column, scale);

                    if (isRtl)
                    {
                        rectForChild.Left -= add;
                    }
                    else
                    {
                        rectForChild.Left += add;
                    }

                    currentLineWidth += add;
                    currentLineRealWidth += add;

                    if (useFixedSplitSize)
                    {
                        //always take the full column width
                        rectFitChild = new SKRect(
                            rectForChild.Left,
                            rectForChild.Top,
                            rectForChild.Left + sizePerChunk,
                            rectForChild.Bottom);
                    }
                    else
                    {
                        //take the remaining width
                        rectFitChild = new SKRect(
                            rectForChild.Left,
                            rectForChild.Top,
                            rectForChild.Right,
                            rectForChild.Bottom);
                    }
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void BreakRow()
                {
                    var removeSpacing = GetSpacingForIndex(column, scale);
                    currentLineWidth -= removeSpacing;
                    currentLineRealWidth -= removeSpacing;
                    FinalizeRow(maxHeight);
                    StartRow();
                    StartColumn();
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                void FinalizeColumn(float width, float height)
                {
                    colFinalized = true;

                    if (height > maxHeight)
                        maxHeight = height;

                    if (alignSplit) //todo check space distribution for full/auto
                    {
                        if (_layout.NeedAutoWidth)
                        {
                            currentLineRealWidth += width;
                            width = sizePerChunk;
                        }
                        else
                        {
                            width = sizePerChunk;
                            currentLineRealWidth += width;
                        }
                    }
                    else
                    {
                        currentLineRealWidth += width;
                    }

                    currentLineWidth += width;

                    if (isRtl)
                    {
                        rectForChild.Left -= width;
                    }
                    else
                    {
                        rectForChild.Left += width;
                    }

                    column++;
                }

                int index = -1;
                ControlInStack measuredCell = null; //if strategy is to measure first cell only

                var allStak = EnumerateViewsForMeasurement().ToList();

                foreach (var child in EnumerateViewsForMeasurement())
                {
                    index++;

                    child.OnBeforeMeasure();
                    if (!child.CanDraw)
                    {
                        continue;
                    }

                    if (rowFinalized)
                    {
                        StartRow();
                    }
                    if (colFinalized)
                    {
                        StartColumn();
                    }

                    var view = _layout.IsTemplated ? null : child;
                    var cell = CreateWrapper(index, view);

                    var remainingSize = rectForChild.Width;
                    if (useFixedSplitSize)
                    {
                        remainingSize = rectFitChild.Width;
                    }

                    if (remainingSize <= 0)
                    {
                        //start new line
                        BreakRow();
                        remainingSize = rectForChild.Width;
                        if (useFixedSplitSize)
                        {
                            remainingSize = rectFitChild.Width;
                        }
                    }

                    ScaledSize measured;

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    ScaledSize MeasureCellInternal()
                    {
                        if (_layout.IsTemplated)
                        {
                            bool needMeasure =
                                _layout.ItemSizingStrategy != ItemSizingStrategy.MeasureFirstItem ||
                                _layout.RecyclingTemplate == RecyclingTemplate.Disabled ||
                                measuredCell == null;

                            if (needMeasure)
                            {
                                measuredCell = cell;
                                return MeasureAndArrangeCell(rectFitChild, cell, child, scale);
                            }
                            else
                            {
                                //apply first measured size to cell
                                var offsetX = rectFitChild.Left - measuredCell.Area.Left;
                                var offsetY = rectFitChild.Top - measuredCell.Area.Top;
                                var arranged = measuredCell.Destination;
                                arranged.Offset(new(offsetX, offsetY));

                                cell.Area = rectFitChild;
                                cell.Measured = measuredCell.Measured.Clone();
                                cell.Destination = arranged;

                                return cell.Measured;
                            }
                        }

                        //non-templated
                        //return MeasureAndArrangeCell(rectFitChild, cell, child, scale);
                        return MeasureCell(rectFitChild, cell, child, scale);
                    }

                    measured = MeasureCellInternal();

                    if (!measured.IsEmpty)
                    {
                        //add logic for col row

                        //check we are within bounds
                        var fitsH = cell.Measured.Pixels.Width <= remainingSize && !cell.Measured.WidthCut;

                        if (!fitsH && !useFixedSplitSize)
                        {
                            BreakRow();
                            measured = MeasureCellInternal();
                        }
                        else
                        {
                            var fitsV = cell.Measured.Pixels.Height <= maxHeight;
                            //todo keke we need bigger height
                        }

                        structure.Add(cell, column, row);
                        FinalizeColumn(cell.Measured.Pixels.Width, cell.Measured.Pixels.Height);

                        cellsToLayoutLater.Add(cell);

                        //check we need a new line?
                        if (!rowFinalized
                             && _layout.Split > 0 && column >= _layout.Split)
                        {
                            FinalizeRow(maxHeight);
                        }

                        //SplitMax = 2  Split Auto, Even, EvenAdaptive

                    }
                }

                if (!rowFinalized)
                {
                    FinalizeRow(maxHeight);
                }

                if (_layout.InitializeTemplatesInBackgroundDelay > 0)
                {
                    _layout.StackStructure = structure;
                }
                else
                {
                    _layout.StackStructureMeasured = structure;
                }

                if (_layout.HorizontalOptions.Alignment == LayoutAlignment.Fill && _layout.WidthRequest < 0)
                {
                    stackWidth = rectForChildrenPixels.Width;
                }
                if (_layout.VerticalOptions.Alignment == LayoutAlignment.Fill && _layout.HeightRequest < 0)
                {
                    stackHeight = rectForChildrenPixels.Height;
                }

                return ScaledSize.FromPixels(stackWidth, stackHeight, scale);
            }

        }

    }
}


