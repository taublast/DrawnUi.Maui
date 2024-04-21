using DrawnUi.Maui.Draw;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw
{
    public partial class SkiaLayout
    {
        #region StackLayout

        public class LayoutStructure : DynamicGrid<ControlInStack>
        {

        }



        public class DynamicGrid<T>
        {
            public int Count => grid.Count;

            private Dictionary<(int, int), T> grid = new Dictionary<(int, int), T>();

            public int MaxRows { get; private set; } = 0;
            public int MaxColumns { get; private set; } = 0;

            public void Add(T item, int row, int column)
            {
                grid[(row, column)] = item;

                // Update the size of the grid
                if (row >= MaxRows)
                    MaxRows = row + 1;
                if (column >= MaxColumns)
                    MaxColumns = column + 1;
            }

            public T Get(int row, int column)
            {
                grid.TryGetValue((row, column), out T item);
                return item;
            }

            public IEnumerable<T> GetColumn(int column)
            {
                for (int i = 0; i < MaxRows; i++)
                {
                    if (grid.TryGetValue((i, column), out T value))
                    {
                        yield return value;
                    }
                }
            }

            /// <summary>
            /// Try to retrieve the item at the specified index in the specified row
            /// </summary>
            /// <param name="row"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            public T GetAtIndexForRow(int row, int index)
            {
                if (grid.TryGetValue((row, index), out T value))
                {
                    return value;
                }

                return default(T);
            }

            /// <summary>
            /// Try to retrieve the item at the specified index in the specified column
            /// </summary>
            /// <param name="col"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            public T GetAtIndexForColumn(int col, int index)
            {
                if (grid.TryGetValue((index, col), out T value))
                {
                    return value;
                }

                return default(T);
            }

            public IEnumerable<T> GetRow(int row)
            {
                for (int j = 0; j < MaxColumns; j++)
                {
                    if (grid.TryGetValue((row, j), out T value))
                    {
                        yield return value;
                    }
                }
            }

            public IEnumerable<T> GetChildren()
            {
                return grid.Values;
            }
        }



        public class ControlInStack
        {
            public ControlInStack()
            {
                Drawn = new();
                Destination = new();
                Area = new();
            }

            /// <summary>
            /// Index inside enumerator that was passed for measurement OR index inside ItemsSource
            /// </summary>
            public int ControlIndex { get; set; }

            /// <summary>
            /// Measure result
            /// </summary>
            public ScaledSize Measured { get; set; }

            /// <summary>
            /// Available area for Arrange
            /// </summary>
            public SKRect Area { get; set; }

            /// <summary>
            /// PIXELS
            /// </summary>
            public SKRect Destination { get; set; }

            /// <summary>
            /// This will be null for recycled views
            /// </summary>
            public SkiaControl View { get; set; }

            /// <summary>
            /// Was used for actual drawing
            /// </summary>
            public DrawingRect Drawn { get; set; }

            /// <summary>
            /// For internal use by your custom controls
            /// </summary>
            public Vector2 Offset { get; set; }

            public bool IsVisible { get; set; }

            public int ZIndex { get; set; }
        }

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
                int row;
                int col;

                var visibleArea = GetOnScreenVisibleArea();

                //PASS 1 - VISIBILITY
                //we need this pass before drawing to recycle views that became hidden
                var viewsTotal = 0;
                foreach (var cell in structure.GetChildren())
                {
                    viewsTotal++;
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
                //SkiaControl[] nonTemplated = null;

                int countRendered = 0;

                foreach (var cell in visibleElements.OrderBy(x => x.ZIndex))
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
                        tree.Add(new SkiaControlWithRect(control, destinationRect, index));
                    }
                }

                //if (Tag == "Controls")
                //{
                //    Debug.WriteLine($"[?] {countRendered}");
                //}
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
            if (measured != ScaledSize.Empty)
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

        /// <summary>
        /// TODO for templated measure only visible?! and just reserve predicted scroll amount for scrolling
        /// </summary>
        /// <param name="rectForChildrenPixels"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public virtual ScaledSize MeasureStack(SKRect rectForChildrenPixels, float scale)
        {
            var layout = new BuildColumnLayout(this);

            var measuredLayout = layout.Build(rectForChildrenPixels, scale);
            return measuredLayout;
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


        /*

        private List<List<ControlInStack>> BuildStackStructure(float scale)
        {

            //build stack grid
            //fill table
            var column = 0;
            var row = 0;
            var rows = new List<List<ControlInStack>>();
            var columns = new List<ControlInStack>();
            var maxColumns = MaxColumns;
            int maxRows = MaxRows; // New MaxRows property

            //returns true if can continue
            bool ProcessStructure(int i, SkiaControl control)
            {
                var add = new ControlInStack { ControlIndex = i };
                if (control != null)
                {
                    add.ZIndex = control.ZIndex;
                }

                // vertical stack or if maxColumns is exceeded
                if (Type == LayoutType.Stack && maxColumns < 1 || (maxColumns > 0 && column >= maxColumns) ||
                    LineBreaks.Contains(i))
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

            if (InitializeTemplatesInBackgroundDelay > 0)
            {
                StackStructure = rows;
            }
            else
            {
                StackStructureMeasured = rows;
            }

            return rows;
        }

        //2 passes for FILL LAYOUT OPTIONS
        //not using this as fps drops
        /*
    case LayoutType.Stack:
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
                if (Type == LayoutType.Stack && maxColumns < 1 || (maxColumns > 0 && column >= maxColumns) || LineBreaks.Contains(index))
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
                        (Type == LayoutType.Stack && child.VerticalOptions.Alignment == LayoutAlignment.Fill && child.HeightRequest < 0)
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
