using Microsoft.Maui.Layouts;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

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
                if (IsStack)
                {
                    if (RenderTree == null)
                        index = -1;
                    else
                        index = RenderTree.Count; //not Length-1 cuz already removed from RenderTree
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

        public override void ApplyBindingContext()
        {
            UpdateRowColumnBindingContexts();

            base.ApplyBindingContext();
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

                if (!IsTemplated && IsStack)
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
        }

        protected override ScaledSize SetMeasured(float width, float height, bool widthCut, bool heightCut, float scale)
        {
            _measuredNewTemplates = true;

            return base.SetMeasured(width, height, widthCut, heightCut, scale);
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
                foreach (var child in GetUnorderedSubviews())
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


        public virtual void OnFocusChanged(bool focus)
        { }


        public SkiaLayout()
        {
            ChildrenFactory = new(this);

            PostponeInvalidation(nameof(OnItemSourceChanged), OnItemSourceChanged);
            //OnItemSourceChanged();
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
                if (IsTemplated || RenderTree == null)
                    return output + ChildrenFactory.GetDebugInfo();

                return output + $"visible {RenderTree.Count}, skipped {ChildrenFactory.GetChildrenCount() - RenderTree.Count}, total {ChildrenFactory.GetChildrenCount()}";
            }
        }

        public ViewsAdapter ChildrenFactory
        {
            get;
            protected set;
        }


        public static readonly BindableProperty SplitProperty = BindableProperty.Create(
            nameof(Split),
            typeof(int),
            typeof(SkiaLayout),
            0,
            propertyChanged: NeedUpdateItemsSource);

        /// <summary>
        /// Number of columns/rows to split into, If 0 will not split at all, if 1 will have single column/row.
        /// </summary>
        public int Split
        {
            get { return (int)GetValue(SplitProperty); }
            set { SetValue(SplitProperty, value); }
        }

        public static readonly BindableProperty SplitAlignProperty = BindableProperty.Create(
            nameof(SplitAlign),
            typeof(bool),
            typeof(SkiaLayout),
            true, propertyChanged: NeedUpdateItemsSource);

        /// <summary>
        /// Whether should keep same column width among rows
        /// </summary>
        public bool SplitAlign
        {
            get { return (bool)GetValue(SplitAlignProperty); }
            set { SetValue(SplitAlignProperty, value); }
        }

        public static readonly BindableProperty SplitSpaceProperty = BindableProperty.Create(
            nameof(SplitSpace),
            typeof(SpaceDistribution),
            typeof(SkiaLayout),
            SpaceDistribution.Auto,
            propertyChanged: NeedUpdateItemsSource);

        /// <summary>
        /// How to distribute free space between children
        /// </summary>
        public SpaceDistribution SplitSpace
        {
            get { return (SpaceDistribution)GetValue(SplitSpaceProperty); }
            set { SetValue(SplitSpaceProperty, value); }
        }

        public static readonly BindableProperty DynamicColumnsProperty = BindableProperty.Create(
            nameof(DynamicColumns),
            typeof(bool),
            typeof(SkiaLayout),
            false, propertyChanged: NeedUpdateItemsSource);

        /// <summary>
        /// If true, will not create additional columns to match SplitMax if there are less real columns, and take additional space for drawing
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
            if (ChildrenFactory == null)
                return;

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
            base.InvalidateViewsList(); //_orderedChildren = null;

            ActualizeSubviews();
        }

        public virtual void ActualizeSubviews()
        {
            needUpdateViews = false;

            ChildrenFactory?.UpdateViews();
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



        public override void Invalidate()
        {
            base.Invalidate();

            Update();
        }

        public override void InvalidateByChild(SkiaControl child)
        {
            if (!NeedAutoSize && child.NeedAutoSize)
                return;

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


        public override ScaledSize MeasureAbsolute(SKRect rectForChildrenPixels, float scale)
        {

            var count = ChildrenFactory.GetChildrenCount();
            if (count > 0)
            {
                if (!IsTemplated)
                {
                    var children = GetUnorderedSubviews();
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

                        if (!measured.IsEmpty)
                        {

                            if (measured.Pixels.Width > maxWidth
                                && child.HorizontalOptions.Alignment != LayoutAlignment.Fill)
                                maxWidth = measured.Pixels.Width;

                            if (measured.Pixels.Height > maxHeight
                                && child.VerticalOptions.Alignment != LayoutAlignment.Fill)
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
                            if (!measured.IsEmpty)
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


            //until we implement 2-threads rendering this is needed for ImageDoubleBuffered cache rendering
            if (IsDisposing || IsDisposed)
                return ScaledSize.Default;

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

                    case LayoutType.Wrap:
                    if (IsTemplated) //fix threads conflict when templates are initialized in background thread
                    {
                        var canMeasureTemplates = ChildrenFactory.TemplatesAvailable || force;

                        if (!canMeasureTemplates)
                            return ScaledSize.CreateEmpty(request.Scale);
                    }

                    ContentSize = MeasureStackPanel(constraints.Content, request.Scale);
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

                return SetMeasuredAdaptToContentSize(constraints, request.Scale);
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
                        InvalidateCache();
                        return SetMeasuredAsEmpty(request.Scale);
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
                Super.Log(e);
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
                var kill = StackStructure;
                StackStructure = StackStructureMeasured;
                StackStructureMeasured = null;
                if (kill != StackStructure)
                    kill?.Clear();
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
            if (IsDisposed || IsDisposing)
                return;

            ApplyMeasureResult();

            base.Draw(context, destination, scale);

            ViewportWasChanged = false;
        }

        bool _trackWasDrawn;


        protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
        {
            if (destination.Width == 0 || destination.Height == 0)
                return;

            if (Type == LayoutType.Grid || IsStack)
            {
                SetupCacheComposition(ctx, destination);
            }

            base.Paint(ctx, destination, scale, arguments);

            var rectForChildren = ContractPixelsRect(destination, scale, Padding);

            var drawnChildrenCount = 0;

            //placeholder for empty
            if (_emptyView != null && _emptyView.IsVisible)
            {
                drawnChildrenCount = DrawViews(ctx, rectForChildren, scale);
            }
            else
            //grid
            if (Type == LayoutType.Grid) //todo add optimization for OptimizeRenderingViewport
            {
                drawnChildrenCount = DrawChildrenGrid(ctx, rectForChildren, scale);
            }
            //else
            //if (Type == LayoutType.Row || Type == LayoutType.Column)
            //{
            //    drawnChildrenCount = DrawChildrenStack(ctx, rectForChildren, scale);
            //}
            else
            //stacklayout
            if (IsStack)
            {
                var structure = LatestStackStructure;
                if (structure != null && structure.GetCount() > 0)
                {
                    drawnChildrenCount = DrawStack(structure, ctx, rectForChildren, scale);
                }
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

        public override void OnDisposing()
        {
            ChildrenFactory?.Dispose();

            ClearChildren();

            DirtyChildren.Clear();

            DirtyChildrenInternal.Clear();

            StackStructure?.Clear();
            StackStructureMeasured?.Clear();

            base.OnDisposing();
        }



        void SetupCacheComposition(SkiaDrawingContext ctx, SKRect destination)
        {
            if (UsingCacheType == SkiaCacheType.ImageComposite)
            {
                DirtyChildrenInternal.Clear();


                var previousCache = RenderObjectPrevious;

                if (previousCache != null && ctx.IsRecycled) //not the first draw
                {
                    IsRenderingWithComposition = true;

                    var offset = new SKPoint(this.DrawingRect.Left - previousCache.Bounds.Left, DrawingRect.Top - previousCache.Bounds.Top);

                    //Super.Log($"[ImageComposite] {Tag} drawing cached at {offset}  {DrawingRect}");


                    // Add more children that are not already added but intersect with the dirty regions
                    var asSpans = CollectionsMarshal.AsSpan(DirtyChildren.GetList());
                    foreach (var item in asSpans)
                    {
                        DirtyChildrenInternal.Add(item);
                    }

                    //make intersecting children dirty too
                    var asSpan = CollectionsMarshal.AsSpan(RenderTree);
                    foreach (var cell in asSpan)
                    {
                        if (!DirtyChildrenInternal.Contains(cell.Control) &&
                            DirtyChildrenInternal.Any(dirtyChild => dirtyChild.DrawingRect.IntersectsWith(cell.Control.DrawingRect)))
                        {
                            DirtyChildrenInternal.Add(cell.Control);
                        }
                    }

                    DirtyChildren.Clear();

                    var count = 0;
                    foreach (var dirtyChild in DirtyChildrenInternal)
                    {
                        var clip = dirtyChild.DrawingRect;
                        clip.Offset(offset);

                        previousCache.Surface.Canvas.DrawRect(clip, PaintErase);

                        count++;
                    }
                }
                else
                {
                    //Super.Log($"[ImageComposite] {Tag} drawing new");

                    IsRenderingWithComposition = false;
                    DirtyChildren.Clear();
                }
            }
            else
            {
                IsRenderingWithComposition = false;
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

        /// <summary>
        /// Column/Row/Stack
        /// </summary>
        public bool IsStack
        {
            get
            {
                return this.Type == LayoutType.Column || Type == LayoutType.Row || Type == LayoutType.Wrap;
            }
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

            skiaControl.PostponeInvalidation(nameof(OnItemSourceChanged), skiaControl.OnItemSourceChanged);

            //skiaControl.OnItemSourceChanged();
        }

        private static void NeedUpdateItemsSource(BindableObject bindable, object oldvalue, object newvalue)
        {
            var skiaControl = (SkiaLayout)bindable;

            skiaControl.PostponeInvalidation(nameof(UpdateItemsSource), skiaControl.UpdateItemsSource);

            //skiaControl.OnItemSourceChanged();
            //skiaControl.Invalidate();
        }

        void UpdateItemsSource()
        {
            OnItemSourceChanged();

            Invalidate();
        }

        public override void OnItemTemplateChanged()
        {
            PostponeInvalidation(nameof(OnItemSourceChanged), OnItemSourceChanged);
            //OnItemSourceChanged();
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
            PostponeInvalidation(nameof(OnItemSourceChanged), OnItemSourceChanged);
            //OnItemSourceChanged();
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

                // Invalidate();

                // return;
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

            PostponeInvalidation(nameof(OnItemSourceChanged), OnItemSourceChanged);
            //OnItemSourceChanged();
            Update();

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
            //relative inside parent:
            var asSpan = CollectionsMarshal.AsSpan(RenderTree);
            for (int i = 0; i < asSpan.Length; i++)
            {
                var child = asSpan[i];

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
        }

        public ContainsPointResult GetChildIndexAt(SKPoint point)
        {
            //todo

            return ContainsPointResult.NotFound();
        }

    }
}
