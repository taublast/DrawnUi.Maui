using Microsoft.Maui.Layouts;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.Specialized;

namespace DrawnUi.Maui.Draw
{

    public partial class SkiaLayout : SkiaControl, ISkiaGestureListener, ISkiaGridLayout
    {
        #region HOTRELOAD

        public override void ReportHotreloadChildRemoved(SkiaControl control)
        {
            if (control == null)
                return;

            try
            {
                var index = this.ChildrenFactory.GetChildrenCount();
                if (Type == LayoutType.Row || Type == LayoutType.Column)
                {
                    if (RenderTree == null)
                        index = -1;
                    else
                        index = RenderTree.Length; //not Length-1 cuz already removed from RenderTree
                    VisualDiagnostics.OnChildRemoved(this, control, index);
                }
                else
                {
                    base.ReportHotreloadChildRemoved(control);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("-------------------------------------------------");
                Trace.WriteLine($"[HOTRELOAD] Exception OnChildRemoved {Tag} {e}");
                Trace.WriteLine("-------------------------------------------------");
            }
        }

        public override void ReportHotreloadChildAdded(SkiaControl child)
        {
            if (child == null)
                return;

            try
            {
                var index = GetVisualChildren().FindIndex(child);
                VisualDiagnostics.OnChildAdded(this, child, index);
            }
            catch (Exception e)
            {
                Trace.WriteLine("-------------------------------------------------");
                Trace.WriteLine($"[HOTRELOAD] Exception ReportHotreloadChildAdded {Tag} {e}");
                Trace.WriteLine("-------------------------------------------------");
            }
        }

        /// For Xaml HotReload. This is semetimes not called when we add and remove views.
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<IVisualTreeElement> GetVisualChildren()
        {

            try
            {
                return base.GetVisualChildren();

                //todo for IsTemplated
                if (Type == LayoutType.Row || Type == LayoutType.Column)
                {
                    var children = RenderTree.Where(w => w.Control != null)
                        .Select(x => x.Control)
                        .ToList().AsReadOnly();
                    //this works fine with hotreload..
                    return children;

                    //using var cells = ChildrenFactory.GetViewsIterator();
                    //return cells.ToList().AsReadOnly();
                }
                else
                {
                    return base.GetVisualChildren();
                }

            }
            catch (Exception e)
            {
                Super.Log("-------------------------------------------------");
                Super.Log($"[HOTRELOAD] Exception GetVisualChildren {Tag} {e}");
                Super.Log("-------------------------------------------------");
                return base.GetVisualChildren();
            }

        }

        #endregion

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            UpdateRowColumnBindingContexts();
        }

        //todo use rendering tree for templated!!
        //protected override void OnParentVisibilityChanged(bool newvalue)
        //{

        //    base.OnParentVisibilityChanged(newvalue);
        //}

        public override void OnPrintDebug()
        {
            Trace.WriteLine($"ViewsAdapter tpls: {ChildrenFactory.PoolSize}/{ChildrenFactory.PoolMaxSize}");
            if (IsTemplated)
            {
                ChildrenFactory.PrintDebugVisible();
            }
        }

        public override bool ShouldInvalidateByChildren
        {
            get
            {
                if (Type == LayoutType.Grid)
                {
                    return true; //we need this to eventually recalculate spans
                }

                if (!IsTemplated && (Type == LayoutType.Column || Type == LayoutType.Row))
                    return true;

                // do not invalidate if template didnt change from last time?
                // NOPE template could be the same but size could be different!
                //if (IsTemplated && _measuredNewTemplates)
                //{
                //    return false;
                //}

                return base.ShouldInvalidateByChildren;
            }
        }

        bool _measuredNewTemplates;

        /// <summary>
        /// Will be called by views adapter upot succsessfull execution of InitializeTemplates.
        /// When using InitializeTemplatesInBackground this is your callbacl to wait for.  
        /// </summary>
        /// <returns></returns>
        public virtual void OnTemplatesAvailable()
        {
            _measuredNewTemplates = false;
            NeedMeasure = true;
            InvalidateParent();
            Repaint();
        }

        protected override ScaledSize SetMeasured(float width, float height, float scale)
        {
            _measuredNewTemplates = true;

            return base.SetMeasured(width, height, scale);
        }

        public override void OnDisposing()
        {
            ChildrenFactory?.Dispose();

            ClearChildren();

            DirtyChild = null;

            base.OnDisposing();
        }

        //bindable property RecyclingTemplate
        public static readonly BindableProperty RecyclingTemplateProperty = BindableProperty.Create(nameof(RecyclingTemplate),
            typeof(RecyclingTemplate),
            typeof(SkiaLayout),
            RecyclingTemplate.Enabled,
            propertyChanged: ItemTemplateChanged);
        /// <summary>
        /// In case of ItemsSource+ItemTemplate set will define should we reuse already created views: hidden items views will be reused for currently visible items on screen.
        /// If set to true inside a SkiaScrollLooped will cause it to redraw constantly even when idle because of the looped scroll mechanics.
        /// </summary>
        public RecyclingTemplate RecyclingTemplate
        {
            get { return (RecyclingTemplate)GetValue(RecyclingTemplateProperty); }
            set { SetValue(RecyclingTemplateProperty, value); }
        }

        //protected override void AdaptCachedLayout(SKRect destination, float scale)
        //{
        //    base.AdaptCachedLayout(destination, scale);

        //    if (Parent == null || Parent is not IDefinesViewport)
        //    {
        //        RenderingViewport = new(DrawingRect);
        //    }
        //}

        //protected override void OnLayoutChanged()
        //{
        //    base.OnLayoutChanged();

        //    if (Parent == null || Parent is not IDefinesViewport)
        //    {
        //        RenderingViewport = new(DrawingRect);
        //    }

        //}

        public static readonly BindableProperty TemplatedHeaderProperty = BindableProperty.Create(nameof(TemplatedHeader), typeof(SkiaControl),
            typeof(SkiaControl), null, propertyChanged: ItemTemplateChanged);
        /// <summary>
        /// Kind of BindableLayout.DrawnTemplate
        /// </summary>
        public SkiaControl TemplatedHeader
        {
            get { return (SkiaControl)GetValue(TemplatedHeaderProperty); }
            set { SetValue(TemplatedHeaderProperty, value); }
        }

        public static readonly BindableProperty TemplatedFooterProperty = BindableProperty.Create(nameof(TemplatedFooter), typeof(SkiaControl),
            typeof(SkiaControl), null, propertyChanged: ItemTemplateChanged);
        /// <summary>
        /// Kind of BindableLayout.DrawnTemplate
        /// </summary>
        public SkiaControl TemplatedFooter
        {
            get { return (SkiaControl)GetValue(TemplatedFooterProperty); }
            set { SetValue(TemplatedFooterProperty, value); }
        }


        public override bool IsTemplated =>
            ((this.ItemTemplate != null || ItemTemplateType != null) && this.ItemsSource != null);

        public SKRect GetChildRect(int index)
        {
            ISkiaControl child = null;
            if (IsTemplated)
            {
                throw new Exception("Cannot get child rect for a templated view");

                return SKRect.Empty;
            }
            return GetChildRect(child);
        }

        public SKRect GetChildRect(ISkiaControl child)
        {
            if (IsTemplated)
            {
                throw new Exception("Cannot get child rect for a templated view");
            }
            else
            {
                if (child != null)
                    return child.Destination;
            }
            return SKRect.Empty;
        }

        public SkiaControl GetChildAt(float x, float y)
        {
            if (IsTemplated)
            {
                //todo 
                throw new Exception("Cannot get child at for a templated view");
            }
            else
            {
                foreach (var child in GetOrderedSubviews())
                {

                    var rect = GetChildRect(child);
                    if (rect.ContainsInclusive(x, y))
                    {
                        return child as SkiaControl;
                    }
                }
            }
            return null;
        }

        protected IEnumerable<ISkiaGestureListener> EnumerateGestureListeners()
        {

            if (IsTemplated)
            {
                if (RenderTree != null)
                    foreach (var listener in RenderTree.Select(s => s.Control as ISkiaGestureListener).Where(listener => listener.CanDraw && !listener.InputTransparent))
                    {
                        yield return listener;
                    }
                ////carousel etc
                //var childrenCount = ChildrenFactory.GetChildrenCount();
                //for (int index = 0; index < childrenCount; index++)
                //{
                //    var child = ChildrenFactory.GetChildAt(index, null);
                //    if (child is ISkiaGestureListener listener && listener.CanDraw && !listener.InputTransparent)
                //    {
                //        yield return listener;
                //    }
                //}
            }
            else
                //base cases
                foreach (var listener in GestureListeners.Where(listener => listener.CanDraw && !listener.InputTransparent))
                {
                    yield return listener;
                }

        }

        public override ISkiaGestureListener ProcessGestures(TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
            SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
        {

            if (TouchEffect.LogEnabled)
            {
                Trace.WriteLine($"[STACK] {this.Tag} Got {touchAction}.. {Uid}");
            }

            lock (LockIterateListeners)
            {
                if (CheckChildrenGesturesLocked(touchAction))
                    return null;

                if (Superview == null)
                {
                    Trace.WriteLine($"[OnGestureEvent] layout captured by unassigned view {this.GetType()} {this.Tag}");
                    return null;
                }

                ISkiaGestureListener consumed = null;
                try
                {
                    bool manageChildFocus = false;

                    //using RenderTree
                    if ((RenderTree != null && (
                            Type == LayoutType.Grid ||
                            Type == LayoutType.Column || Type == LayoutType.Row) //normal stacklayout or..
                        || (IsTemplated && Type == LayoutType.Absolute) //templated carousel etc
                        ))
                    {
                        var thisOffset = TranslateInputCoords(childOffset);
                        var x = args.Location.X + thisOffset.X;
                        var y = args.Location.Y + thisOffset.Y;

                        for (int i = RenderTree.Length - 1; i >= 0; i--)
                        //for (int i = 0; i < RenderTree.Length; i++)
                        {
                            var child = RenderTree[i];

                            if (child == Superview.FocusedChild)
                                manageChildFocus = true;

                            ISkiaGestureListener listener = child.Control.GesturesEffect;
                            if (listener == null && child.Control is ISkiaGestureListener listen)
                            {
                                listener = listen;
                            }

                            if (listener != null && !child.Control.InputTransparent && child.Control.CanDraw)
                            {

                                var forChild = child.Rect.ContainsInclusive(x, y) || child.Control == Superview.FocusedChild;
                                if (forChild)
                                {
                                    //Trace.WriteLine($"[HIT] for cell {i} at Y {y:0.0}");
                                    if (manageChildFocus && listener == Superview.FocusedChild)
                                    {
                                        manageChildFocus = false;
                                    }

                                    if (AddGestures.AttachedListeners.TryGetValue(child.Control, out var effect))
                                    {
                                        var c = effect.OnSkiaGestureEvent(type, args, touchAction, thisOffset,
                                            TranslateInputCoords(childOffsetDirect, false), alreadyConsumed);
                                        if (c != null)
                                        {
                                            consumed = effect;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        var c = listener.OnSkiaGestureEvent(type, args, touchAction, thisOffset,
                                            TranslateInputCoords(childOffsetDirect, false), alreadyConsumed);
                                        if (c != null)
                                        {
                                            consumed = c;//listener;
                                            break;
                                        }
                                    }

                                }
                            }
                            if (manageChildFocus)
                            {
                                Superview.FocusedChild = null;
                            }
                        }

                        return consumed;
                    }

                    return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);

                    return this;
                }
            }

        }

        public virtual void OnFocusChanged(bool focus)
        { }


        public SkiaLayout()
        {
            ChildrenFactory = new(this);

            OnItemSourceChanged();
        }



        #region PROPERTIES


        public static readonly BindableProperty VirtualizationProperty = BindableProperty.Create(
            nameof(Virtualisation),
            typeof(VirtualisationType),
            typeof(SkiaLayout),
            VirtualisationType.Enabled,
            propertyChanged: NeedInvalidateMeasure);

        /// <summary>
        /// Default is Enabled, children get the visible viewport area for rendering and can virtualize.
        /// </summary>
        public VirtualisationType Virtualisation
        {
            get { return (VirtualisationType)GetValue(VirtualizationProperty); }
            set { SetValue(VirtualizationProperty, value); }
        }


        public static readonly BindableProperty JustifyContentProperty = BindableProperty.Create(nameof(JustifyContent), typeof(FlexJustify), typeof(SkiaLayout),
            FlexJustify.Start,
            propertyChanged: NeedDraw);
        /// <summary>
        /// ToDo
        /// </summary>
        public FlexJustify JustifyContent
        {
            get { return (FlexJustify)GetValue(JustifyContentProperty); }
            set { SetValue(JustifyContentProperty, value); }
        }







        #endregion

        #region STACK ROW/GRID
        protected List<ISkiaControl> ChildrenGrid { get; set; }


        public void BreakLine()
        {
            LineBreaks.Add(Views.Count);
        }

        protected List<int> LineBreaks = new List<int>();


        /// <summary>
        /// Rect is real drawing position
        /// </summary>
        /// <param name="Control"></param>
        /// <param name="Rect"></param>
        /// <param name="Index"></param>
        public record SkiaControlWithRect(SkiaControl Control, SKRect Rect, int Index);

        protected long _measuredStamp;

        protected long _builtRenderTreeStamp;


        /// <summary>
        /// Last rendered controls tree. Used by gestures etc..
        /// </summary>
        public ImmutableArray<SkiaControlWithRect> RenderTree { get; protected set; } =
            ImmutableArray<SkiaControlWithRect>.Empty;

        private Stopwatch _stopwatchRender = new();
        List<string> elapsedTimes = new();


        public override ScaledRect GetOnScreenVisibleArea()
        {
            var onscreen = base.GetOnScreenVisibleArea();

            var expand = (float)Math.Round(RenderingScale * HiddenAmountToRender);

            var visible = SKRect.Intersect(onscreen.Pixels, DrawingRect);

            return ScaledRect.FromPixels(new(visible.Left - expand, visible.Top - expand,
                visible.Right + expand, visible.Bottom + expand), RenderingScale);
        }

        public override void DrawRenderObject(CachedObject cache, SkiaDrawingContext ctx, SKRect destination)
        {
            var visibleArea = GetOnScreenVisibleArea();

            base.DrawRenderObject(cache, ctx, visibleArea.Pixels);
        }

        protected SkiaControl _emptyView;

        protected virtual void SetupViews()
        {
            if (EmptyView != _emptyView)
            {
                RemoveSubView(_emptyView);
                _emptyView?.Dispose();

                if (EmptyView != null)
                {
                    _emptyView = EmptyView;
                    CheckAndSetupIfEmpty();
                    AddSubView(_emptyView);
                }

                Update();
            }
        }

        private bool _IsEmpty;
        public bool IsEmpty
        {
            get
            {
                return _IsEmpty;
            }
            set
            {
                if (_IsEmpty != value)
                {
                    _IsEmpty = value;
                    OnPropertyChanged();
                    IsEmptyChanged?.Invoke(this, value);
                }
            }
        }

        public event EventHandler<bool> IsEmptyChanged;


        protected virtual void ApplyIsEmpty(bool value)
        {
            IsEmpty = value;

            if (_emptyView != null)
            {
                _emptyView.IsVisible = value;
            }
        }

        protected virtual bool CheckAndSetupIfEmpty()
        {
            var value = false;

            if (ItemTemplate != null)
            {
                value = ItemsSource == null || ItemsSource.Count == 0;
            }
            else
            {
                value = this.ChildrenFactory.GetChildrenCount() == 0;
            }

            ApplyIsEmpty(value);

            return value;
        }

        public override string DebugString
        {
            get
            {
                var output = $"{GetType().Name} Tag {Tag}, ";
                if (IsTemplated)
                    return output + ChildrenFactory.GetDebugInfo();

                return output + $"visible {RenderTree.Length}, skipped {ChildrenFactory.GetChildrenCount() - RenderTree.Length}, total {ChildrenFactory.GetChildrenCount()}";
            }
        }

        public ViewsAdapter ChildrenFactory
        {
            get;
            protected set;
        }

        public static readonly BindableProperty MaxRowsProperty = BindableProperty.Create(
            nameof(MaxRows),
            typeof(int),
            typeof(SkiaLayout),
            0, propertyChanged: NeedUpdateItemsSource);

        /// <summary>
        /// Number of rows to use for Column and Row layout types, If 0, rows will be created dynamically based layout type.
        /// </summary>
        public int MaxRows
        {
            get { return (int)GetValue(MaxRowsProperty); }
            set { SetValue(MaxRowsProperty, value); }
        }


        public static readonly BindableProperty MaxColumnsProperty = BindableProperty.Create(
            nameof(MaxColumns),
            typeof(int),
            typeof(SkiaLayout),
            0, propertyChanged: NeedUpdateItemsSource);

        /// <summary>
        /// Number of columns to use for Column and Row layout types, If 0, columns will be created dynamically based layout type.
        /// </summary>
        public int MaxColumns
        {
            get { return (int)GetValue(MaxColumnsProperty); }
            set { SetValue(MaxColumnsProperty, value); }
        }

        public static readonly BindableProperty DynamicColumnsProperty = BindableProperty.Create(
            nameof(DynamicColumns),
            typeof(bool),
            typeof(SkiaLayout),
            false, propertyChanged: NeedUpdateItemsSource);

        /// <summary>
        /// If true, columns will be created dynamically based on the available space, otherwise MaxColumns will be used
        /// </summary>
        public bool DynamicColumns
        {
            get { return (bool)GetValue(DynamicColumnsProperty); }
            set { SetValue(DynamicColumnsProperty, value); }
        }


        #endregion


        #region RENDERiNG




        protected ScaledRect _viewport;

        public virtual void OnViewportWasChanged(ScaledRect viewport)
        {

            _viewport = viewport;

            //RenderingViewport = new(viewport.Pixels);

            //cells will get OnScrolled
            ViewportWasChanged = true;

            return;
        }

        protected bool ViewportWasChanged { get; set; }

        protected virtual bool DrawChild(
            SkiaDrawingContext context,
            SKRect dest, ISkiaControl child, float scale)
        {
            child.OnBeforeDraw(); //could set IsVisible or whatever inside

            if (!child.CanDraw)
                return false; //child set himself invisible

            if (ViewportWasChanged)
            {
                //if (child is IInsideViewport viewport)
                //{
                //    var intersection = SKRect.Intersect(_viewport.Pixels, dest);
                //    viewport.OnViewportWasChanged(ScaledRect.FromPixels(intersection, RenderingScale));
                //}

                if (child is ISkiaCell cell)
                {
                    cell.OnScrolled();

                    //Task.Run(() =>
                    //{
                    //    cell.OnScrolled();
                    //}).ConfigureAwait(false);
                }

            }

            child.Render(context, dest, scale);

            return true;
        }



        //protected void Build()
        //{
        //	if (AvailableDestination != SKRect.Empty)
        //	{
        //		Measure(AvailableDestination.Width, AvailableDestination.Height);
        //	}
        //	Update();
        //}

        public override void SetChildren(IEnumerable<SkiaControl> views)
        {
            base.SetChildren(views);

            Invalidate();
        }


        protected override void OnMeasured()
        {
            base.OnMeasured();

            _measuredStamp++;
        }

        public override void InvalidateInternal()
        {
            templatesInvalidated = true;

            base.InvalidateInternal();
        }

        bool templatesInvalidated;

        public override void InvalidateWithChildren()
        {
            ChildrenFactory.TemplatesInvalidated = true;

            base.InvalidateWithChildren();
        }

        //protected override void OnChildAdded(SkiaControl child)
        //{
        //    base.OnChildAdded(child);

        //    needUpdateViews = true;
        //}

        //protected override void OnChildRemoved(SkiaControl child)
        //{
        //    base.OnChildRemoved(child);

        //    needUpdateViews = true;
        //}

        public override void InvalidateViewsList()
        {
            base.InvalidateViewsList();

            ActualizeSubviews();
        }

        public virtual void ActualizeSubviews()
        {
            //if (IsTemplated)
            //{
            //    this.ChildrenFactory.TemplatesInvalidated = true;
            //}

            needUpdateViews = false;
            if (ChildrenFactory != null)
            {
                ChildrenFactory.UpdateViews();
                //Update();
            }
        }

        bool needUpdateViews;

        protected virtual int GetTemplatesPoolPrefill()
        {
            if (RecyclingTemplate == RecyclingTemplate.Disabled)
            {
                return ItemsSource.Count;
            }

            return 0;
        }

        protected virtual int GetTemplatesPoolLimit()
        {
            if (ItemTemplatePoolSize > 0)
                return ItemTemplatePoolSize;

            if (ItemsSource == null)
                return 0;

            return ItemsSource.Count + 2;
        }

        /// <summary>
        /// Will be used while measuring then set to null. This is set by InvalidateByChild override.
        /// </summary>
        protected SkiaControl DirtyChild { get; set; }

        public override void Invalidate()
        {
            base.Invalidate();

            Update();
        }

        public override void InvalidateByChild(SkiaControl child)
        {
            if (!NeedAutoSize && child.NeedAutoSize)
                return;

            DirtyChild = child;

            if (Type == LayoutType.Absolute)
            {
                //check if this child changed your size, if not exit
                //todo
            }

            base.InvalidateByChild(child);
        }

        protected object lockMeasure = new();

        SemaphoreSlim semaphoreItemTemplates = new(1);

        protected async Task CreateTemplatesInBackground()
        {
            await semaphoreItemTemplates.WaitAsync();
            try
            {

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            finally
            {
                semaphoreItemTemplates.Release();
            }
        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="rectForChildrenPixels"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public virtual ScaledSize MeasureMasonryColumns(SKRect rectForChildrenPixels, float scale)
        {
            int maxColumns = MaxColumns;

            if (maxColumns < 1)
                maxColumns = 1;

            List<List<SkiaControl>> columns = new List<List<SkiaControl>>(maxColumns);

            for (int i = 0; i < maxColumns; i++)
                columns.Add(new List<SkiaControl>());

            using var cells = ChildrenFactory.GetViewsIterator();
            foreach (var child in cells)
            {
                // Add each child to the shortest column
                var shortestColumn = columns.OrderBy(c => c.Sum(child => child.Height)).First();
                shortestColumn.Add(child);
            }

            float columnWidth = DrawingRect.Width / maxColumns; //todo add Spacing
            for (int i = 0; i < maxColumns; i++)
            {
                var column = columns[i];
                float y = 0;

                foreach (var child in column)
                {
                    // measure and todo Arrange each child within the column 
                    //child.Arrange(new SKRect(i * columnWidth, y, columnWidth, child.DrawingRect.Height));
                    //y += child.Height;
                }
            }

            return ScaledSize.FromPixels(rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="rectForChildrenPixels"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public virtual ScaledSize MeasureMasonryRows(SKRect rectForChildrenPixels, float scale)
        {


            return ScaledSize.FromPixels(rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);
        }


        public virtual ScaledSize MeasureGrid(SKRect rectForChildrenPixels, float scale)
        {


            var constarints = GetSizeInPoints(rectForChildrenPixels.Size, scale);

            BuildGridLayout(constarints);

            GridStructureMeasured.DecompressStars(constarints);

            var maxHeight = 0.0f;
            var maxWidth = 0.0f;
            var spacing = 0.0;

            if (GridStructureMeasured.Columns.Length > 1)
            {
                spacing = (GridStructureMeasured.Columns.Length - 1) * ColumnSpacing;
            }

            var contentWidth = (float)(GridStructureMeasured.Columns.Sum(x => x.Size) + spacing + Padding.Left + Padding.Right) * scale;

            spacing = 0.0;
            if (GridStructureMeasured.Rows.Length > 1)
            {
                spacing = (GridStructureMeasured.Rows.Length - 1) * RowSpacing;
            }
            var contentHeight = (float)(GridStructureMeasured.Rows.Sum(x => x.Size) + spacing + Padding.Top + Padding.Bottom) * scale;


            if (contentWidth > maxWidth)
                maxWidth = contentWidth;
            if (contentHeight > maxHeight)
                maxHeight = contentHeight;


            return ScaledSize.FromPixels(maxWidth, maxHeight, scale);


            // return ScaledSize.FromPixels(rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);
        }

        public override ScaledSize MeasureAbsolute(SKRect rectForChildrenPixels, float scale)
        {

            var count = ChildrenFactory.GetChildrenCount();
            if (count > 0)
            {
                if (!IsTemplated)
                {
                    var children = GetOrderedSubviews();
                    return MeasureContent(children, rectForChildrenPixels, scale);
                }

                var maxHeight = 0.0f;
                var maxWidth = 0.0f;

                if (!ChildrenFactory.TemplatesAvailable)
                {
                    return ScaledSize.CreateEmpty(scale);
                }
                else
                {
                    if (this.ItemSizingStrategy == ItemSizingStrategy.MeasureFirstItem)
                    {
                        var template = ChildrenFactory.GetTemplateInstance();

                        var child = ChildrenFactory.GetChildAt(0, template);

                        //if (child == null)
                        //{
                        //    child = template;
                        //}
                        var measured = MeasureChild(child, rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);

                        if (measured != ScaledSize.Empty)
                        {
                            if (measured.Pixels.Width > maxWidth && child.HorizontalOptions.Alignment != LayoutAlignment.Fill)
                                maxWidth = measured.Pixels.Width;
                            if (measured.Pixels.Height > maxHeight && child.VerticalOptions.Alignment != LayoutAlignment.Fill)
                                maxHeight = measured.Pixels.Height;
                        }

                        ChildrenFactory.ReleaseView(template);
                    }
                    else
                    if (this.ItemSizingStrategy == ItemSizingStrategy.MeasureAllItems
                        || RecyclingTemplate == RecyclingTemplate.Disabled)
                    {
                        var childrenCount = ChildrenFactory.GetChildrenCount();
                        for (int index = 0; index < childrenCount; index++)
                        {
                            var child = ChildrenFactory.GetChildAt(index, null);

                            var measured = MeasureChild(child, rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);
                            if (measured != ScaledSize.Empty)
                            {
                                if (measured.Pixels.Width > maxWidth && child.HorizontalOptions.Alignment != LayoutAlignment.Fill)
                                    maxWidth = measured.Pixels.Width;
                                if (measured.Pixels.Height > maxHeight && child.VerticalOptions.Alignment != LayoutAlignment.Fill)
                                    maxHeight = measured.Pixels.Height;
                            }

                        }
                    }
                    return ScaledSize.FromPixels(maxWidth, maxHeight, scale);
                }

            }
            //empty container
            else
            if (NeedAutoHeight || NeedAutoWidth)
            {
                return ScaledSize.CreateEmpty(scale);
                //return SetMeasured(0, 0, scale);
            }

            return ScaledSize.FromPixels(rectForChildrenPixels.Width, rectForChildrenPixels.Height, scale);
        }

        protected object lockMeasureLayout = new();

        public virtual ScaledSize MeasureLayout(MeasureRequest request, bool force)
        {

            lock (lockMeasureLayout)
            {
                _measuredNewTemplates = false;

                var constraints = GetMeasuringConstraints(request);

                GridStructureMeasured = null;

                if (!CheckAndSetupIfEmpty())
                {
                    switch (Type)
                    {
                    case LayoutType.Absolute:
                    ContentSize = MeasureAbsolute(constraints.Content, request.Scale);
                    break;

                    case LayoutType.Grid:

                    ContentSize = MeasureGrid(constraints.Content, request.Scale);
                    break;

                    case LayoutType.Column:
                    case LayoutType.Row:
                    if (IsTemplated) //fix threads conflict when templates are initialized in background thread
                    {
                        var canMeasureTemplates = ChildrenFactory.TemplatesAvailable || force;

                        if (!canMeasureTemplates)
                            return ScaledSize.CreateEmpty(request.Scale);
                    }

                    ContentSize = MeasureStack(constraints.Content, request.Scale);
                    break;

                    case LayoutType.MasonryColumns:
                    ContentSize = MeasureMasonryColumns(constraints.Content, request.Scale);
                    break;

                    case LayoutType.MasonryRows:
                    ContentSize = MeasureMasonryRows(constraints.Content, request.Scale);
                    break;

                    default:
                    ContentSize = ScaledSize.FromPixels(constraints.Content.Width, constraints.Content.Height, request.Scale);
                    break;
                    }
                }
                else
                {
                    ContentSize = MeasureAbsoluteBase(constraints.Content, request.Scale);
                }

                var width = AdaptWidthConstraintToContentRequest(constraints.Request.Width, ContentSize, constraints.Margins.Left + constraints.Margins.Right);
                var height = AdaptHeightConstraintToContentRequest(constraints.Request.Height, ContentSize, constraints.Margins.Top + constraints.Margins.Bottom);

                //Debug.WriteLine($"[Remeasured] {this.Tag} {this.Uid}");

                var invalidated = !CompareSize(new SKSize(width, height), MeasuredSize.Pixels, 0);
                if (invalidated)
                    RenderObjectNeedsUpdate = true;

                return SetMeasured(width, height, request.Scale);
            }

        }


        /// <summary>
        /// If you call this while measurement is in process (IsMeasuring==True) will return last measured value.
        /// </summary>
        /// <param name="widthConstraint"></param>
        /// <param name="heightConstraint"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
        {

            //background measuring or invisible or self measure from draw because layout will never pass -1
            if (IsMeasuring || !CanDraw || (widthConstraint < 0 || heightConstraint < 0)
                || (IsTemplated && ChildrenFactory.TemplatesBusy))
            {
                return MeasuredSize;
            }

            try
            {

                IsMeasuring = true;

                CreateDefaultContent();

                //lock (lockMeasure)
                {
                    var request = CreateMeasureRequest(widthConstraint, heightConstraint, scale);
                    if (request.IsSame)
                    {
                        return MeasuredSize;
                    }

                    if (request.WidthRequest == 0 || request.HeightRequest == 0)
                    {
                        RenderObjectNeedsUpdate = true;
                        return SetMeasured(0, 0, request.Scale);
                    }

                    if (IsTemplated)
                    {
                        if (ChildrenFactory.TemplatesInvalidated)
                        {
                            ApplyNewItemsSource = false;
                            ChildrenFactory.InitializeTemplates(CreateContentFromTemplate, ItemsSource,
                                GetTemplatesPoolLimit(),
                                GetTemplatesPoolPrefill());
                        }
                    }
                    else
                    {
                        if (needUpdateViews)
                        {
                            ActualizeSubviews();
                        }

                    }

                    return MeasureLayout(request, false);
                } //end lock

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return MeasuredSize;
            }
            finally
            {
                IsMeasuring = false;
            }

        }


        public override void ApplyMeasureResult()
        {
            if (StackStructureMeasured != null)
            {
                StackStructure = StackStructureMeasured;
                StackStructureMeasured = null;
                CheckAndSetupIfEmpty();
            }

            if (GridStructureMeasured != null)
            {
                GridStructure = GridStructureMeasured;
                GridStructureMeasured = null;
                CheckAndSetupIfEmpty();
            }

            base.ApplyMeasureResult();
        }

        protected override void Draw(SkiaDrawingContext context, SKRect destination, float scale)
        {
            ApplyMeasureResult();

            base.Draw(context, destination, scale);

            ViewportWasChanged = false;
        }

        bool _trackWasDrawn;

        protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
        {
            if (destination.Width == 0 || destination.Height == 0)
                return;

            base.Paint(ctx, destination, scale, arguments);

            var rectForChildren = ContractPixelsRect(destination, scale, Padding);

            var drawnChildrenCount = 0;

            if (_emptyView != null && _emptyView.IsVisible)
            {
                drawnChildrenCount = DrawViews(ctx, rectForChildren, scale);
            }
            else
            if (Type == LayoutType.Grid) //todo add optimization for OptimizeRenderingViewport
            {
                drawnChildrenCount = DrawChildrenGrid(ctx, rectForChildren, scale);
            }
            else
            if (Type == LayoutType.Row || Type == LayoutType.Column)
            {
                drawnChildrenCount = DrawChildrenStack(ctx, rectForChildren, scale);
            }
            else
            //absolute layout
            {
                drawnChildrenCount = DrawViews(ctx, rectForChildren, scale);
            }

            ApplyIsEmpty(drawnChildrenCount == 0);

            if (!_trackWasDrawn && LayoutReady)
            {
                _trackWasDrawn = true;
                OnAppeared();
            }
        }

        protected override int DrawViews(SkiaDrawingContext context, SKRect destination, float scale, bool debug = false)
        {
            var drawn = 0;

            if (IsTemplated)
            {
                if (ChildrenFactory.TemplatesAvailable)
                {
                    using var children = ChildrenFactory.GetViewsIterator();
                    drawn = RenderViewsList(children, context, destination, scale, debug);
                }
                if (drawn == 0 && _emptyView != null && _emptyView.IsVisible)
                {
                    var drawViews = new List<SkiaControl> { _emptyView };
                    RenderViewsList(drawViews, context, destination, scale);
                    return 0;
                }
            }
            else
            {
                drawn = base.DrawViews(context, destination, scale, debug);
                if (drawn == 0 && _emptyView != null && _emptyView.IsVisible)
                {
                    var drawViews = new List<SkiaControl> { _emptyView };
                    RenderViewsList(drawViews, context, destination, scale);
                    return 0;
                }
            }
            return drawn;
        }

        public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type), typeof(LayoutType), typeof(SkiaLayout),
            LayoutType.Absolute,
            propertyChanged: NeedInvalidateMeasure);
        public LayoutType Type
        {
            get { return (LayoutType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly BindableProperty HiddenAmountToRenderProperty = BindableProperty.Create(nameof(HiddenAmountToRender), typeof(double), typeof(SkiaLayout),
            0.0,
            propertyChanged: NeedInvalidateMeasure);
        /// <summary>
        /// How much of the hidden content out of visible bounds should be considered visible for rendering,
        /// default is 0.
        /// Basically how much should be expand in every direction of the visible area prior to checking if content falls
        /// into its bounds for rendering controlled with Virtualisation.
        /// </summary>
        public double HiddenAmountToRender
        {
            get { return (double)GetValue(HiddenAmountToRenderProperty); }
            set { SetValue(HiddenAmountToRenderProperty, value); }
        }

        #endregion

        public Action<SKPath, SKRect> ClipCustom;

        #region ItemsSource

        public static readonly BindableProperty InitializeTemplatesInBackgroundDelayProperty = BindableProperty.Create(
            nameof(InitializeTemplatesInBackgroundDelay),
            typeof(int),
            typeof(SkiaLayout),
            0, propertyChanged: ItemsSourcePropertyChanged);

        /// <summary>
        /// Whether should initialize templates in background instead of blocking UI thread, default is 0.
        /// Set your delay in Milliseconds to enable.
        /// When this is enabled and RecyclingTemplate is Disabled will also measure the layout in background
        /// when templates are available without blocking UI-tread.
        /// After that OnTemplatesAvailable will be called on parent layout.
        /// </summary>
        public int InitializeTemplatesInBackgroundDelay
        {
            get { return (int)GetValue(InitializeTemplatesInBackgroundDelayProperty); }
            set { SetValue(InitializeTemplatesInBackgroundDelayProperty, value); }
        }

        public static readonly BindableProperty ItemSizingStrategyProperty = BindableProperty.Create(
            nameof(ItemSizingStrategy),
            typeof(ItemSizingStrategy),
            typeof(SkiaLayout),
            ItemSizingStrategy.MeasureFirstItem,
            propertyChanged: ItemsSourcePropertyChanged);

        public ItemSizingStrategy ItemSizingStrategy
        {
            get { return (ItemSizingStrategy)GetValue(ItemSizingStrategyProperty); }
            set { SetValue(ItemSizingStrategyProperty, value); }
        }

        public static readonly BindableProperty ItemTemplatePoolSizeProperty = BindableProperty.Create(nameof(ItemTemplatePoolSize),
        typeof(int),
        typeof(SkiaLayout),
        -1, propertyChanged: ItemsSourcePropertyChanged);
        /// <summary>
        /// Default is -1, the number od template instances will not be less than data collection count. You can manually set to to a specific number to fill your viewport etc. Beware that if you set this to a number that will not be enough to fill the viewport binding contexts will contasntly be changing triggering screen update.
        /// </summary>
        public int ItemTemplatePoolSize
        {
            get { return (int)GetValue(ItemTemplatePoolSizeProperty); }
            set { SetValue(ItemTemplatePoolSizeProperty, value); }
        }

        public static readonly BindableProperty EmptyViewProperty = BindableProperty.Create(
            nameof(EmptyView),
            typeof(SkiaControl),
            typeof(SkiaLayout),
            null, propertyChanged: (b, o, n) =>
            {
                if (b is SkiaLayout control)
                {
                    control.SetupViews();
                }
            });

        public SkiaControl EmptyView
        {
            get { return (SkiaControl)GetValue(EmptyViewProperty); }
            set { SetValue(EmptyViewProperty, value); }
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList),
            typeof(SkiaLayout),
            null,
            //validateValue: (bo, v) => v is IList,
            propertyChanged: ItemsSourcePropertyChanged);

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void ItemsSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var skiaControl = (SkiaLayout)bindable;

            if (oldvalue != null)
            {
                //if (oldvalue is IList oldList)
                //{
                //	foreach (var context in oldList)
                //	{
                //		//todo
                //	}
                //}

                if (oldvalue is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= skiaControl.ItemsSourceCollectionChanged;
                }

            }

            //if (newvalue is IList newList)
            //{
            //	foreach (var context in newList)
            //	{
            //		//todo
            //	}
            //}

            if (newvalue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged -= skiaControl.ItemsSourceCollectionChanged;
                newCollection.CollectionChanged += skiaControl.ItemsSourceCollectionChanged;
            }

            skiaControl.OnItemSourceChanged();
        }

        private static void NeedUpdateItemsSource(BindableObject bindable, object oldvalue, object newvalue)
        {
            var skiaControl = (SkiaLayout)bindable;

            skiaControl.OnItemSourceChanged();
        }

        public override void OnItemTemplateChanged()
        {
            OnItemSourceChanged();
        }

        public bool ApplyNewItemsSource { get; set; }

        public virtual void OnItemSourceChanged()
        {

            if (!BindingContextWasSet && ItemsSource == null) //do not create items from templates until the context was changed properly to avoid bugs
            {
                return;
            }

            if (IsTemplated && !IsMeasuring)
            {
                void Apply()
                {
                    this.ChildrenFactory.TemplatesInvalidated = true;
                    ApplyNewItemsSource = true;
                    Invalidate();
                }

                if (IsMeasuring)
                {
                    Tasks.StartDelayed(TimeSpan.FromMilliseconds(500), Apply);
                }
                else
                {
                    Apply();
                }
            }

        }


        public virtual void ResetScroll()
        {
            if (Parent is IDefinesViewport viewport)
            {
                viewport.ScrollTo(0, 0, 0);
            }
        }

        protected virtual void BuildFromZero()
        {
            OnItemSourceChanged();
        }

        protected virtual void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {

            if (!IsTemplated)
                return;

            switch (args.Action)
            {

            //case NotifyCollectionChangedAction.Add:
            //{
            //    int index = args.NewStartingIndex;
            //    if (args.NewItems != null)
            //    {
            //        if (ChildrenFactory.GetChildrenCount() > 0)
            //        {
            //            //insert somewhere
            //            foreach (var newItem in args.NewItems)
            //            {
            //                SkiaControl view = null;
            //                try
            //                {
            //                    view = (SkiaControl)ItemTemplate.CreateContent();
            //                }
            //                catch (Exception ex)
            //                {
            //                    var stopp = ex;
            //                }
            //                var bindableObject = view as BindableObject;
            //                if (bindableObject != null)
            //                {
            //                    view.Parent = this;
            //                    bindableObject.BindingContext = newItem;
            //                    Children.Insert(index++, view);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            BuildFromZero();
            //        }
            //    }
            //}
            //break;
            //case NotifyCollectionChangedAction.Add:
            //{
            //    int index = args.NewStartingIndex;
            //    if (args.NewItems != null)
            //    {
            //        if (Views.Count > 0)
            //        {
            //            //insert somewhere
            //            foreach (var newItem in args.NewItems)
            //            {
            //                SkiaControl view = CreateControl(ItemTemplate);
            //                if (view != null)
            //                {
            //                    view.Parent = this;
            //                    view.BindingContext = newItem;
            //                    Views.Insert(index++, view);
            //                }
            //            }
            //            Invalidate();
            //        }
            //        else
            //        {
            //            OnItemSourceChanged();
            //        }
            //    }
            //}
            //break;
            //case NotifyCollectionChangedAction.Move:
            //{
            //    try
            //    {
            //        var view = Views[args.OldStartingIndex];
            //        Views.RemoveAt(args.OldStartingIndex);
            //        Views.Insert(args.NewStartingIndex, view);
            //    }
            //    catch (Exception e)
            //    {
            //        Trace.WriteLine($"[SkiaLayout] {e}");
            //    }
            //}
            //Invalidate();
            //break;
            //case NotifyCollectionChangedAction.Remove:
            //{
            //    if (args.NewItems.Count > 1)
            //    {
            //        throw new NotImplementedException("ToDo Remove more than 1");
            //    }
            //    try
            //    {
            //        var remove = Views[args.OldStartingIndex];
            //        Views.RemoveAt(args.OldStartingIndex);
            //        remove.Dispose();
            //    }
            //    catch (Exception e)
            //    {
            //        Trace.WriteLine($"[SkiaLayout] {e}");
            //    }
            //}
            //Invalidate();
            //break;

            //case NotifyCollectionChangedAction.Replace: //todo for more than 1
            //{
            //    if (args.NewItems.Count > 1)
            //    {
            //        throw new NotImplementedException("ToDo Replace more than 1");
            //    }
            //    var view = CreateControl(ItemTemplate);
            //    if (view != null)
            //    {
            //        view.Parent = this;
            //        view.BindingContext = args.NewItems[0]; ;
            //        var remove = Views[args.OldStartingIndex];
            //        Views.RemoveAt(args.OldStartingIndex);
            //        Views.Insert(args.NewStartingIndex, view);
            //        remove.Dispose();
            //    }
            //}
            //Invalidate();
            //break;


            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:

            if (IsTemplated && !IsMeasuring)
            {

                ApplyNewItemsSource = false;
                ChildrenFactory.ContextCollectionChanged(CreateContentFromTemplate, ItemsSource,
                    GetTemplatesPoolLimit(),
                    GetTemplatesPoolPrefill());

                Invalidate();

                return;
            }

            break;

            case NotifyCollectionChangedAction.Reset:
            ResetScroll();
            //ClearChildren();
            //if (args.NewItems != null)
            //{
            //	foreach (var newItem in args.NewItems)
            //	{
            //		SkiaControl view = CreateControl(ItemTemplate);
            //		if (view != null)
            //		{
            //			view.Parent = this;
            //			view.BindingContext = newItem;
            //			Views.Add(view);
            //		}
            //	}
            //}
            //Invalidate();
            break;
            }

            OnItemSourceChanged();

        }



        #endregion

        protected override void OnLayoutReady()
        {
            base.OnLayoutReady();


        }

        public virtual void OnAppearing()
        {

        }

        public virtual void OnDisappearing()
        {

        }

        public virtual void OnAppeared()
        {

        }

        public virtual void OnDisappeared()
        {

        }

        public virtual void OnLoaded()
        {

        }

        public virtual ContainsPointResult GetVisibleChildIndexAt(SKPoint point)
        {


            for (int i = 0; i < RenderTree.Length; i++)
            {
                var child = RenderTree[i];
                if (child.Rect.ContainsInclusive(point))
                {
                    return new ContainsPointResult()
                    {
                        Index = child.Index,
                        Area = child.Rect,
                        Point = point
                    };
                }
            }

            return ContainsPointResult.NotFound();
            /*
			   {
				   if (StackStructure != null)
				   {
					   var stackStructure = StackStructure;

					   int index = -1;
					   int row;
					   int col;
					   for (row = 0; row < stackStructure.Count; row++)
					   {
						   var rowContent = stackStructure[row];
						   for (col = 0; col < rowContent.Count; col++)
						   {
							   index++;
							   var childInfo = rowContent[col];

							   if (childInfo.Destination.ContainsInclusive(point))
							   {
								   return new ContainsPointResult()
								   {
									   Index = index,
									   Area = childInfo.Destination,
									   Point = point
								   };
							   }

							   //  if (Orientation == ScrollOrientation.Horizontal)
						   }
					   }
				   }

			   }
			*/

            return ContainsPointResult.NotFound();
        }

        public ContainsPointResult GetChildIndexAt(SKPoint point)
        {
            //todo

            return ContainsPointResult.NotFound();
        }

    }
}
