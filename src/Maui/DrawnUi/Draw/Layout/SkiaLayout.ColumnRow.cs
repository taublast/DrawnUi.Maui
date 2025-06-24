using System.Runtime.CompilerServices;

namespace DrawnUi.Draw
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
                    if (Type == LayoutType.Column && child.VerticalOptions != LayoutOptions.Start &&
                        child.VerticalOptions != LayoutOptions.Fill)
                    {
                        area = new(area.Left,
                            rectForChildrenPixels.Top,
                            area.Right,
                            rectForChildrenPixels.Bottom);
                    }
                    else if (Type == LayoutType.Row && child.HorizontalOptions != LayoutOptions.Start &&
                             child.HorizontalOptions != LayoutOptions.Fill)
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

        public int GetSizeKey(SKSize size)
        {
            int hKey = 0;
            //if (RecyclingTemplate != RecyclingTemplate.Disabled)
            {
                if (Type == LayoutType.Column)
                {
                    hKey = (int)Math.Round(size.Height);
                }
                else if (Type == LayoutType.Row)
                {
                    hKey = (int)Math.Round(size.Width);
                }
            }
            return hKey;
        }

        //todo code 5656
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


        /// <summary>
        /// Measuring column/row with 3-pass approach to handle Fill options correctly
        /// </summary>
        public virtual ScaledSize MeasureStackNonTemplated(SKRect rectForChildrenPixels, float scale)
        {
            if (ChildrenFactory.GetChildrenCount() <= 0)
                return ScaledSize.FromPixels(rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);

            var nonTemplated = GetUnorderedSubviews().Where(c => c.CanDraw).ToArray();
            var layoutStructure = BuildStackStructure(scale);

            return MeasureStackCore(rectForChildrenPixels, scale, layoutStructure, false, null, nonTemplated);
        }

        /// <summary>
        /// Measuring column/row for templated for fastest way possible
        /// </summary>
        public virtual ScaledSize MeasureStackTemplated(SKRect rectForChildrenPixels, float scale)
        {
            if (ChildrenFactory.GetChildrenCount() <= 0)
                return ScaledSize.FromPixels(rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);

            var layoutStructure = BuildStackStructure(scale);
            var useOneTemplate = IsTemplated && RecyclingTemplate != RecyclingTemplate.Disabled;
            var template = useOneTemplate ? ChildrenFactory.GetTemplateInstance() : null;

            try
            {
                return MeasureStackCore(rectForChildrenPixels, scale, layoutStructure, true, template, null);
            }
            finally
            {
                if (useOneTemplate && template != null)
                {
                    ChildrenFactory.ReleaseTemplateInstance(template);
                }
            }
        }

        /// <summary>
        /// Core measurement logic shared between templated and non-templated scenarios
        /// </summary>
        private ScaledSize MeasureStackCore(SKRect rectForChildrenPixels, float scale, LayoutStructure layoutStructure,
            bool isTemplated, SkiaControl template, SkiaControl[] nonTemplated)
        {
            var stackHeight = 0.0f;
            var stackWidth = 0.0f;
            var firstCell = (ControlInStack)null;
            var useOneTemplate = template != null;

            // Reuse existing collections to avoid allocations
            _tempSecondPassList.Clear();
            _tempCellsToRelease.Clear();

            // Handle Fill children pre-calculation for non-templated
            var hasFillHandling = !isTemplated;
            var fixedSpaceUsed = 0.0f;
            var spacingUsed = 0.0f;
            var spacePerFillChild = 0.0f;

            if (hasFillHandling)
            {
                CalculateFillSpace(rectForChildrenPixels, scale, layoutStructure, nonTemplated,
                    ref fixedSpaceUsed, ref spacingUsed, ref spacePerFillChild);
            }

            var rectForChild = rectForChildrenPixels;
            var index = -1;

            try
            {
                for (var row = 0; row < layoutStructure.MaxRows; row++)
                {
                    var maxHeight = 0.0f;
                    var maxWidth = 0.0f;
                    var columnsCount = GetEffectiveColumnsCount(layoutStructure, row);
                    var needMeasureAll = ShouldMeasureAll(isTemplated, useOneTemplate, columnsCount, firstCell);
                    var widthPerColumn = CalculateWidthPerColumn(rectForChildrenPixels, columnsCount, scale);

                    for (int column = 0; column < columnsCount; column++)
                    {
                        if (layoutStructure.GetColumnCountForRow(row) < column + 1)
                            continue;

                        index++;
                        var cell = layoutStructure.Get(column, row);
                        var child = GetChildForMeasurement(cell.ControlIndex, isTemplated, template, nonTemplated);

                        if (child?.CanDraw != true)
                        {
                            if (child != null) cell.Measured = ScaledSize.Default;
                            continue;
                        }

                        ApplySpacing(ref rectForChild, row, column, scale);
                        var rectFitChild = CreateChildMeasureRect(rectForChild, widthPerColumn, cell,
                            hasFillHandling, spacePerFillChild, nonTemplated);

                        var measured = MeasureChildCell(rectFitChild, cell, child, rectForChildrenPixels, scale,
                            isTemplated, needMeasureAll, ref firstCell);

                        if (!measured.IsEmpty)
                        {
                            UpdateRowDimensions(ref maxWidth, ref maxHeight, measured, child, column, scale, isTemplated);
                            rectForChild.Left += measured.Pixels.Width;
                        }

                        CheckSecondPassNeeded(cell, child, scale, isTemplated);
                        cell.WasMeasured = true;
                    }

                    UpdateStackSize(ref stackWidth, ref stackHeight, maxWidth, maxHeight, row, scale);
                    rectForChild.Top += maxHeight;
                    rectForChild.Left = 0;
                }

                ApplyFillConstraints(ref stackWidth, ref stackHeight, rectForChildrenPixels);
                ProcessSecondPass(rectForChildrenPixels, stackWidth, stackHeight);

                return ScaledSize.FromPixels(stackWidth, stackHeight, scale);
            }
            finally
            {
                if (isTemplated && !useOneTemplate)
                {
                    foreach (var cell in _tempCellsToRelease)
                    {
                        ChildrenFactory.ReleaseViewInUse(cell.ContextIndex, cell);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate space requirements for Fill children (PASS 1)
        /// </summary>
        private void CalculateFillSpace(SKRect rectForChildrenPixels, float scale, LayoutStructure layoutStructure,
            SkiaControl[] nonTemplated, ref float fixedSpaceUsed, ref float spacingUsed, ref float spacePerFillChild)
        {
            var fillChildrenCount = 0;

            for (var row = 0; row < layoutStructure.MaxRows; row++)
            {
                if (row > 0) spacingUsed += GetSpacingForIndex(row, scale);

                var columnsCount = GetEffectiveColumnsCount(layoutStructure, row);

                for (int column = 0; column < columnsCount; column++)
                {
                    if (layoutStructure.GetColumnCountForRow(row) < column + 1)
                        continue;

                    var cell = layoutStructure.Get(column, row);
                    var child = nonTemplated[cell.ControlIndex];

                    if (child?.CanDraw != true) continue;

                    var isFillChild = IsChildFill(child);

                    if (isFillChild)
                    {
                        fillChildrenCount++;
                    }
                    else
                    {
                        fixedSpaceUsed += CalculateChildFixedSpace(child, rectForChildrenPixels, scale);
                    }
                }
            }

            var totalAvailableSpace = Type == LayoutType.Column
                ? rectForChildrenPixels.Height
                : rectForChildrenPixels.Width;
            var remainingSpace = Math.Max(0, totalAvailableSpace - fixedSpaceUsed - spacingUsed);
            spacePerFillChild = fillChildrenCount > 0 ? remainingSpace / fillChildrenCount : 0;
        }

        /// <summary>
        /// Get child for measurement based on strategy
        /// </summary>
        private SkiaControl GetChildForMeasurement(int controlIndex, bool isTemplated, SkiaControl template, SkiaControl[] nonTemplated)
        {
            if (isTemplated)
            {
                var child = ChildrenFactory.GetViewForIndex(controlIndex, template, 0,
                    RecyclingTemplate != RecyclingTemplate.Disabled);
                if (template == null)
                {
                    _tempCellsToRelease.Add(child);
                }
                return child;
            }

            return controlIndex < nonTemplated.Length ? nonTemplated[controlIndex] : null;
        }

        /// <summary>
        /// Create measurement rectangle for child
        /// </summary>
        private SKRect CreateChildMeasureRect(SKRect rectForChild, float widthPerColumn, ControlInStack cell,
            bool hasFillHandling, float spacePerFillChild, SkiaControl[] nonTemplated)
        {
            var rectFitChild = new SKRect(rectForChild.Left, rectForChild.Top,
                rectForChild.Left + widthPerColumn, rectForChild.Bottom);

            if (hasFillHandling)
            {
                var child = nonTemplated[cell.ControlIndex];
                var isFillChild = IsChildFill(child);

                if (isFillChild)
                {
                    if (Type == LayoutType.Column)
                    {
                        rectFitChild = new SKRect(rectFitChild.Left, rectFitChild.Top,
                            rectFitChild.Right, rectFitChild.Top + spacePerFillChild);
                    }
                    else
                    {
                        rectFitChild = new SKRect(rectFitChild.Left, rectFitChild.Top,
                            rectFitChild.Left + spacePerFillChild, rectFitChild.Bottom);
                    }
                }
            }

            return rectFitChild;
        }

        /// <summary>
        /// Measure individual child cell
        /// </summary>
        private ScaledSize MeasureChildCell(SKRect rectFitChild, ControlInStack cell, SkiaControl child,
            SKRect rectForChildrenPixels, float scale, bool isTemplated, bool needMeasureAll, ref ControlInStack firstCell)
        {
            if (isTemplated)
            {
                if (needMeasureAll)
                {
                    var measured = MeasureAndArrangeCell(rectFitChild, cell, child, rectForChildrenPixels, scale);
                    firstCell = cell;
                    return measured;
                }
                else
                {
                    var offsetX = rectFitChild.Left - firstCell.Area.Left;
                    var offsetY = rectFitChild.Top - firstCell.Area.Top;
                    var arranged = firstCell.Destination;
                    arranged.Offset(new(offsetX, offsetY));

                    cell.Area = rectFitChild;
                    cell.Measured = firstCell.Measured.Clone();
                    cell.Destination = arranged;
                    cell.WasMeasured = true;

                    return firstCell.Measured;
                }
            }

            return MeasureAndArrangeCell(rectFitChild, cell, child, rectForChildrenPixels, scale);
        }

        /// <summary>
        /// Helper methods to reduce code duplication
        /// </summary>
        private int GetEffectiveColumnsCount(LayoutStructure layoutStructure, int row)
        {
            var columnsCount = layoutStructure.GetColumnCountForRow(row);
            return !DynamicColumns && columnsCount < Split ? Split : columnsCount;
        }

        private float CalculateWidthPerColumn(SKRect rectForChildrenPixels, int columnsCount, float scale)
        {
            if (Type == LayoutType.Column)
            {
                return (float)Math.Round(columnsCount > 1
                    ? (rectForChildrenPixels.Width - (columnsCount - 1) * Spacing * scale) / columnsCount
                    : rectForChildrenPixels.Width);
            }
            return rectForChildrenPixels.Width;
        }

        private bool ShouldMeasureAll(bool isTemplated, bool useOneTemplate, int columnsCount, ControlInStack firstCell)
        {
            if (!isTemplated || !useOneTemplate) return true;

            return RecyclingTemplate == RecyclingTemplate.Disabled ||
                   MeasureItemsStrategy == MeasuringStrategy.MeasureAll ||
                   (MeasureItemsStrategy == MeasuringStrategy.MeasureFirst && columnsCount != Split) ||
                   !(MeasureItemsStrategy == MeasuringStrategy.MeasureFirst && firstCell != null);
        }

        private bool IsChildFill(SkiaControl child)
        {
            return (Type == LayoutType.Column &&
                    child.VerticalOptions.Alignment == LayoutAlignment.Fill &&
                    child.HeightRequest < 0) ||
                   (Type == LayoutType.Row &&
                    child.HorizontalOptions.Alignment == LayoutAlignment.Fill &&
                    child.WidthRequest < 0);
        }

        private float CalculateChildFixedSpace(SkiaControl child, SKRect rectForChildrenPixels, float scale)
        {
            if (Type == LayoutType.Column && child.HeightRequest >= 0)
            {
                return (float)(child.HeightRequest * scale);
            }
            else if (Type == LayoutType.Row && child.WidthRequest >= 0)
            {
                return (float)(child.WidthRequest * scale);
            }
            else
            {
                var tempRect = Type == LayoutType.Column
                    ? new SKRect(0, 0, rectForChildrenPixels.Width, float.PositiveInfinity)
                    : new SKRect(0, 0, float.PositiveInfinity, rectForChildrenPixels.Height);

                var tempMeasured = MeasureChild(child, tempRect.Width, tempRect.Height, scale);
                return Type == LayoutType.Column ? tempMeasured.Pixels.Height : tempMeasured.Pixels.Width;
            }
        }

        private void ApplySpacing(ref SKRect rectForChild, int row, int column, float scale)
        {
            if (column == 0)
                rectForChild.Top += GetSpacingForIndex(row, scale);

            rectForChild.Left += GetSpacingForIndex(column, scale);
        }

        private void UpdateRowDimensions(ref float maxWidth, ref float maxHeight, ScaledSize measured,
            SkiaControl child, int column, float scale, bool isTemplated)
        {
            if (isTemplated || child.HorizontalOptions != LayoutOptions.Fill || Type != LayoutType.Column)
            {
                maxWidth += measured.Pixels.Width + GetSpacingForIndex(column, scale);
            }

            if (isTemplated || child.VerticalOptions != LayoutOptions.Fill || Type != LayoutType.Row)
            {
                if (measured.Pixels.Height > maxHeight)
                    maxHeight = measured.Pixels.Height;
            }
        }

        private void UpdateStackSize(ref float stackWidth, ref float stackHeight, float maxWidth, float maxHeight, int row, float scale)
        {
            if (maxWidth > stackWidth)
                stackWidth = maxWidth;

            stackHeight += maxHeight + GetSpacingForIndex(row, scale);
        }

        private void CheckSecondPassNeeded(ControlInStack cell, SkiaControl child, float scale, bool isTemplated)
        {
            if (!isTemplated)
            {
                if ((Type == LayoutType.Column && child.HorizontalOptions != LayoutOptions.Start) ||
                    (Type == LayoutType.Row && child.VerticalOptions != LayoutOptions.Start))
                {
                    _tempSecondPassList.Add(new(cell, child, scale));
                }
            }
        }

        private void ApplyFillConstraints(ref float stackWidth, ref float stackHeight, SKRect rectForChildrenPixels)
        {
            if (HorizontalOptions.Alignment == LayoutAlignment.Fill || SizeRequest.Width >= 0)
            {
                stackWidth = rectForChildrenPixels.Width;
            }

            if (VerticalOptions.Alignment == LayoutAlignment.Fill || SizeRequest.Height >= 0)
            {
                stackHeight = rectForChildrenPixels.Height;
            }
        }

        private void ProcessSecondPass(SKRect rectForChildrenPixels, float stackWidth, float stackHeight)
        {
            var autoRight = HorizontalOptions != LayoutOptions.Fill
                ? rectForChildrenPixels.Left + stackWidth
                : rectForChildrenPixels.Right;

            var autoBottom = VerticalOptions != LayoutOptions.Fill
                ? rectForChildrenPixels.Top + stackHeight
                : rectForChildrenPixels.Bottom;

            var autoRect = new SKRect(rectForChildrenPixels.Left, rectForChildrenPixels.Top, autoRight, autoBottom);

            foreach (var secondPass in _tempSecondPassList)
            {
                AdjustSecondPassCell(secondPass.Cell, stackWidth, stackHeight);
                LayoutCell(secondPass.Child.MeasuredSize, secondPass.Cell, secondPass.Child, autoRect, secondPass.Scale);
            }
        }

        private void AdjustSecondPassCell(ControlInStack cell, float stackWidth, float stackHeight)
        {
            if (float.IsInfinity(cell.Area.Bottom))
            {
                cell.Area = new(cell.Area.Left, cell.Area.Top, cell.Area.Right, cell.Area.Top + stackHeight);
            }
            else if (float.IsInfinity(cell.Area.Top))
            {
                cell.Area = new(cell.Area.Left, cell.Area.Bottom - stackHeight, cell.Area.Right, cell.Area.Bottom);
            }

            if (cell.Area.Height > stackHeight)
            {
                cell.Area = new(cell.Area.Left, cell.Area.Top, cell.Area.Right, cell.Area.Top + stackHeight);
            }

            if (float.IsInfinity(cell.Area.Right))
            {
                cell.Area = new(cell.Area.Left, cell.Area.Top, cell.Area.Left + stackWidth, cell.Area.Bottom);
            }
            else if (float.IsInfinity(cell.Area.Left))
            {
                cell.Area = new(cell.Area.Right - stackWidth, cell.Area.Top, cell.Area.Right, cell.Area.Bottom);
            }

            if (cell.Area.Width > stackWidth)
            {
                cell.Area = new(cell.Area.Left, cell.Area.Top, cell.Area.Left + stackWidth, cell.Area.Bottom);
            }
        }

        // Reusable collections to avoid allocations - these should be class fields
        private readonly List<SecondPassArrange> _tempSecondPassList = new();
        private readonly List<SkiaControl> _tempCellsToRelease = new();

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
                var add = new ControlInStack { ControlIndex = i, View = control };
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

            StackStructureMeasured = structure;

            return structure;
        }

        // Cache for visible area calculation
        private struct VisibleAreaCache
        {
            public SKRect Destination;
            public ScaledRect VisibleArea;
            public DateTime CalculatedAt;
        }

        private VisibleAreaCache? _visibleAreaCache;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMilliseconds(16); // 60fps

        /// <summary>   
        /// REPLACE your existing GetOnScreenVisibleArea call in DrawStack with this:
        /// </summary>
        private ScaledRect GetVisibleAreaCached(DrawingContext ctx)
        {
            var now = DateTime.Now;

            // Check if we can reuse cached calculation
            if (_visibleAreaCache.HasValue)
            {
                var cache = _visibleAreaCache.Value;
                var positionDelta = Math.Abs(ctx.Destination.Left - cache.Destination.Left) +
                                    Math.Abs(ctx.Destination.Top - cache.Destination.Top);
                var age = now - cache.CalculatedAt;

                // Reuse if viewport moved less than 5px and cache is fresh
                if (positionDelta < 5 && age < _cacheLifetime)
                {
                    return cache.VisibleArea;
                }
            }

            // Calculate new visible area (expensive operation)
            var inflate = (float)(this.VirtualisationInflated * ctx.Scale);
            var visibleArea = GetOnScreenVisibleArea(ctx, new(inflate, inflate));

            // Cache the result
            _visibleAreaCache = new VisibleAreaCache
            {
                Destination = ctx.Destination, VisibleArea = visibleArea, CalculatedAt = now
            };

            return visibleArea;
        }


        /// <summary>
        /// Can be called by some layouts after they calculated the list of visible children to be drawn, but have not drawn them yet
        /// </summary>
        protected virtual void OnBeforeDrawingVisibleChildren(DrawingContext ctx, LayoutStructure structure,
            List<ControlInStack> visibleElements)
        {
        }

        /// <summary>
        /// Renders stack/wrap layout.
        /// Returns number of drawn children.
        /// </summary>
        protected virtual int DrawStack(DrawingContext ctx, LayoutStructure structure)
        {
            var drawn = 0;
            List<SkiaControlWithRect> tree = new();
            var needrebuild = templatesInvalidated;
            List<ControlInStack> visibleElements = new();
            bool updateInternal = false;

            if (structure != null)
            {
                //var inflate = (float)(this.VirtualisationInflated * ctx.Scale);

                var visibilityArea = GetVisibleAreaCached(ctx);

                var recyclingAreaPixels = visibilityArea.Pixels;
                var expendRecycle = ((float)RecyclingBuffer * ctx.Scale);
                recyclingAreaPixels.Inflate(expendRecycle, expendRecycle);

                //PASS 1 - VISIBILITY
                Vector2 offsetOthers = Vector2.Zero;
                var currentIndex = -1;
                foreach (var cell in structure.GetChildrenAsSpans())
                {
                    if (!cell.WasMeasured)
                        continue;

                    currentIndex++;

                    if (cell.Destination == SKRect.Empty || cell.Measured.Pixels.Width < 1 ||
                        cell.Measured.Pixels.Height < 1)
                    {
                        cell.IsVisible = false;
                    }
                    else
                    {
                        // Calculate screen position (unchanged)
                        var x = ctx.Destination.Left + cell.Destination.Left;
                        var y = ctx.Destination.Top + cell.Destination.Top;

                        cell.Drawn.Set(x, y, x + cell.Destination.Width, y + cell.Destination.Height);

                        offsetOthers += cell.OffsetOthers;

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
                                // SOLUTION PART 1: Use normal area for visibility
                                cell.IsVisible = cell.Drawn.IntersectsWith(visibilityArea.Pixels);
                            }
                        }
                        else
                        {
                            cell.IsVisible = true;
                        }
                    }

                    cell.OffsetOthers = Vector2.Zero;
                    cell.WasLastDrawn = false;

                    // SOLUTION PART 2: Use EXPANDED area for recycling  
                    bool shouldKeepInMemory = true;
                    if (Virtualisation != VirtualisationType.Disabled &&
                        cell.Destination != SKRect.Empty &&
                        !cell.Measured.Pixels.IsEmpty)
                    {
                        // Only recycle if cell is FAR outside the expanded recycling area
                        shouldKeepInMemory = cell.Drawn.IntersectsWith(recyclingAreaPixels);
                    }

                    // SOLUTION PART 3: Only recycle views that are truly far away
                    if (!shouldKeepInMemory)
                    {
                        ChildrenFactory.MarkViewAsHidden(cell.ControlIndex);
                    }

                    // Add to visible elements for drawing
                    if (cell.IsVisible)
                    {
                        visibleElements.Add(cell);
                    }
                }

                if (OutputDebug)
                {
                    Super.Log(
                        $"[SkiaLayout] visibility area {visibilityArea}, recycling area {recyclingAreaPixels}, visible items: {visibleElements.Count}");
                }

                OnBeforeDrawingVisibleChildren(ctx, structure, visibleElements);

                //PASS 2 DRAW VISIBLE
                bool hadAdjustments = false;
                bool wasVisible = false;
                var index = -1;
                var cellsToRelease = new List<SkiaControl>();
                int countRendered = 0;
                offsetOthers = Vector2.Zero;

                try
                {
                    foreach (var cell in CollectionsMarshal.AsSpan(visibleElements))
                    {
                        if (!cell.WasMeasured)
                            continue;

                        index++;

                        SkiaControl child = null;
                        if (IsTemplated)
                        {
                            child = ChildrenFactory.GetViewForIndex(cell.ControlIndex, null,
                                GetSizeKey(cell.Measured.Pixels));
                            if (child == null)
                            {
                                return countRendered;
                            }

                            //Trace.WriteLine($"[CELL] DRAW {index} {child.Uid}");

                            cellsToRelease.Add(child);
                        }
                        else
                        {
                            child = cell.View;
                        }

                        if (child is SkiaControl control && child.IsVisible)
                        {
                            SKRect destinationRect;
                            var x = offsetOthers.X + cell.Drawn.Left;
                            var y = offsetOthers.Y + cell.Drawn.Top;

                            if (child.NeedMeasure)
                            {
                                if (!child.WasMeasured || InvalidatedChildrenInternal.Contains(child) ||
                                    GetSizeKey(child.MeasuredSize.Pixels) != GetSizeKey(cell.Measured.Pixels))
                                {
                                    var oldSize = child.MeasuredSize.Pixels;
                                    child.Measure((float)cell.Area.Width, (float)cell.Area.Height, ctx.Scale);
                                    if (oldSize != SKSize.Empty && !CompareSize(oldSize, MeasuredSize.Pixels, 1f))
                                    {
                                        //Trace.WriteLine($"[CELL] remeasured {child.Uid}");
                                        var diff = child.MeasuredSize.Pixels - oldSize;
                                        cell.OffsetOthers = new Vector2(diff.Width, diff.Height);
                                    }
                                }
                            }

                            if (child.MeasuredSize.Pixels.Width >= 1 && child.MeasuredSize.Pixels.Height >= 1)
                            {
                                if (IsTemplated)
                                {
                                    destinationRect = new SKRect(x, y,
                                        x + cell.Area.Width, y + cell.Area.Bottom);
                                }
                                else
                                {
                                    destinationRect = new SKRect(x, y, x + cell.Drawn.Width,
                                        y + cell.Drawn.Height);
                                }

                                if (IsRenderingWithComposition)
                                {
                                    if (child.PostAnimators.Count > 0)
                                    {
                                        updateInternal = true;
                                    }

                                    if (DirtyChildrenInternal.Contains(child) || child.PostAnimators.Count > 0)
                                    {
                                        DrawChild(ctx.WithDestination(destinationRect), child);
                                        countRendered++;
                                    }
                                    else
                                    {
                                        child.Arrange(destinationRect, child.SizeRequest.Width,
                                            child.SizeRequest.Height,
                                            ctx.Scale);
                                    }
                                }
                                else
                                {
                                    DrawChild(ctx.WithDestination(destinationRect), child);
                                    countRendered++;
                                }

                                cell.WasLastDrawn = true;

                                drawn++;

                                tree.Add(new SkiaControlWithRect(control,
                                    destinationRect,
                                    control.LastDrawnAt,
                                    index));
                            }
                        }
                    }
                }
                finally
                {
                    if (IsTemplated)
                        foreach (var cell in cellsToRelease)
                        {
                            ChildrenFactory.ReleaseViewInUse(cell.ContextIndex, cell);
                        }
                }
            }

            if (needrebuild && visibleElements.Count > 0)
            {
                templatesInvalidated = false;
            }

            SetRenderingTree(tree);

            if (Parent is IDefinesViewport viewport &&
                viewport.TrackIndexPosition != RelativePositionType.None)
            {
                viewport.UpdateVisibleIndex();
            }

            OnPropertyChanged(nameof(DebugString));

            if (updateInternal)
            {
                Update();
            }

            return drawn;
        }
    }

    #endregion
}
