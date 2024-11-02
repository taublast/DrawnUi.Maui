using System.Collections;

namespace DrawnUi.Maui.Draw;

/// <summary>
/// Top level class for working with ItemTemplates. Holds visible views.
/// </summary>
public class ViewsAdapter : IDisposable
{
    public static bool LogEnabled = false;

    private readonly SkiaLayout _parent;

    public ViewsAdapter(SkiaLayout parent)
    {
        _parent = parent;
    }

    public void Dispose()
    {
        DisposeViews();
    }

    object lockVisible = new();

    protected void UpdateVisibleViews()
    {
        lock (lockVisible)
        {
            foreach (var view in _dicoCellsInUse.Values.ToList())
            {
                view.InvalidateInternal();
            }

        }
    }

    protected void DisposeVisibleViews()
    {
        lock (lockVisible)
        {
            foreach (var view in _dicoCellsInUse.Values)
            {
                view.Dispose();
            }
            _dicoCellsInUse.Clear();
        }
    }

    protected void DisposeViews()
    {
        _templatedViewsPool?.Dispose();
        DisposeVisibleViews();
    }


    /// <summary>
    /// Holds visible prepared views with appropriate context, index is inside ItemsSource 
    /// </summary>
    private readonly Dictionary<int, SkiaControl> _dicoCellsInUse = new(256);

    public void MarkAllViewsAsHidden()
    {
        lock (lockVisible)
        {
            // Add all visible views back to the recycling pool (e.g., _viewModelPool.Return(hiddenView))
            foreach (var hiddenView in _dicoCellsInUse.Values)
                ReleaseView(hiddenView);

            _dicoCellsInUse.Clear();

        }
    }

    public void MarkViewAsHidden(int index)
    {
        lock (lockVisible)
        {
            if (_parent.IsTemplated && _parent.RecyclingTemplate == RecyclingTemplate.Enabled)
            {
                if (_dicoCellsInUse.ContainsKey(index))
                {
                    if (_dicoCellsInUse.TryGetValue(index, out SkiaControl hiddenView))
                    {
                        //if (hiddenView is ISkiaCell notify)
                        //{
                        //    notify.OnDisappeared();
                        //}

                        _dicoCellsInUse.Remove(index);
                        ReleaseView(hiddenView);
                    }

                    //Debug.WriteLine($"[InUse] {_dicoCellsInUse.Keys.Select(k => k.ToString()).Aggregate((current, next) => $"{current},{next}")}");
                }
            }
        }
    }



    protected virtual void AttachView(SkiaControl view, int index)
    {
        //todo check how it behaves when sources changes
        //lock (_lockTemplates)
        {
            view.Parent = _parent;
            if (index == 0 || view.ContextIndex != index)
            //if (view.BindingContext == null || _parent.RecyclingTemplate == RecyclingTemplate.Enabled)
            {
                try
                {
                    if (index < _dataContexts?.Count)
                    {
                        var context = _dataContexts[index];
                        view.ContextIndex = index;
                        view.BindingContext = context;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }


        }

    }


    public int AddedMore { get; protected set; }

    /// <summary>
    /// Keep pool size with `n` templated more oversized, so when we suddenly need more templates they would already be ready, avoiding lag spike,
    /// This method is likely to reserve templated views once on layout size changed.
    /// </summary>
    /// <param name="oversize"></param>
    public void AddMoreToPool(int oversize)
    {
        if (_templatedViewsPool != null && AddedMore < oversize)
        {
            var add = oversize - AddedMore;
            AddedMore = oversize;
            if (add > 0)
            {
                for (int i = 0; i < add; i++)
                {
                    _templatedViewsPool?.Reserve();
                }
            }
        }
    }


    /// <summary>
    /// Use to manually pre-create views from item templates so when we suddenly need more templates they would already be ready, avoiding lag spike,
    /// This will respect pool MaxSize in order not to overpass it.
    /// </summary>
    /// <param name="size"></param>
    public void FillPool(int size, IList context)
    {
        FillPool(size);

        if (context == null)
        {
            return;
        }

        //todo?..
    }

    /// <summary>
    /// Use to manually pre-create views from item templates so when we suddenly need more templates they would already be ready, avoiding lag spike,
    /// This will respect pool MaxSize in order not to overpass it.
    /// </summary>
    /// <param name="size"></param>
    public void FillPool(int size)
    {
        if (size > 0)
        {
            while (_templatedViewsPool.Size < size && _templatedViewsPool.Size < _templatedViewsPool.MaxSize)
            {
                _templatedViewsPool.Reserve();
            }
        }
    }

    public string GetDebugInfo()
    {
        return $"Total cells: {PoolSize + _dicoCellsInUse.Count}, using: {_dicoCellsInUse.Count}, in pool: {PoolSize}/{PoolMaxSize}";
    }

    public SkiaControl GetChildAt(int index, SkiaControl template = null)
    {
        if (index >= 0)
        {
            //lock (lockVisible)
            {
                if (_parent.IsTemplated)
                {

                    if (_dicoCellsInUse.TryGetValue(index, out SkiaControl ready))
                    {
                        if (LogEnabled)
                        {
                            Trace.WriteLine($"[ViewsAdapter] {_parent.Tag} returned a ready view {ready.Uid}");
                        }

                        //#if DEBUG
                        //                        try
                        //                        {
                        //                            if (!object.Equals(_dataContexts[index], ready.BindingContext))
                        //                            {
                        //                                Trace.WriteLine($"[ViewsAdapter] {_parent.Tag} ready view has different context!");
                        //                            }
                        //                        }
                        //                        catch (Exception e)
                        //                        {
                        //                            Trace.WriteLine(e);
                        //                        }
                        //#endif

                        AttachView(ready, index);
                        return ready;
                    }

                    var view = GetViewAtIndex(index, template);

                    if (view == null)
                    {
                        return null;
                    }

                    AttachView(view, index);

                    //save visible view for future use only if template is not provided
                    if (template == null)
                    {
                        if (!_dicoCellsInUse.Values.Contains(view))
                        {
                            _dicoCellsInUse.TryAdd(index, view);
                        }

                        //Debug.WriteLine($"[InUse] {_dicoCellsInUse.Keys.Select(k => k.ToString()).Aggregate((current, next) => $"{current},{next}")}");

                        //if (view is ISkiaCell notify)
                        //{
                        //    notify.OnAppearing();
                        //}
                    }

                    return view;

                }
                else
                {
                    var children = _parent.GetUnorderedSubviews();
                    if (index < children.Count())
                        return children.ElementAt(index);
                }

                return null;

            }
        }
        return null;
    }

    public int GetChildrenCount()
    {
        if (!_parent.IsTemplated)
        {
            var children = _parent.GetUnorderedSubviews();

            return children.Count();
        }

        if (_parent.ItemsSource != null)
        {
            return _parent.ItemsSource.Count;
        }

        return 0;
    }

    private TemplatedViewsPool _templatedViewsPool;
    private IList _dataContexts;

    protected readonly object _lockTemplates = new object();

    private readonly Dictionary<int, ViewsIterator> _wrappers =
        new();

    public bool TemplesInvalidating;

    public bool TemplatesInvalidated
    {
        get => _templatesInvalidated;
        set
        {
            //lock (_lockTemplates)
            {
                _templatesInvalidated = value;
            }
        }
    }

    public int PoolMaxSize
    {
        get
        {
            if (_templatedViewsPool == null)
            {
                return int.MinValue;
            }
            return _templatedViewsPool.MaxSize;
        }
    }

    public int PoolSize
    {
        get
        {
            if (_templatedViewsPool == null)
            {
                return int.MinValue;
            }
            return _templatedViewsPool.Size;
        }
    }

    public bool TemplatesBusy;
    private bool _templatesInvalidated;
    private float _forScale;
    private int _forSplit;

    /// <summary>
    /// Main method to initialize templates, can use InitializeTemplatesInBackground as an option. 
    /// </summary>
    /// <param name="template"></param>
    /// <param name="dataContexts"></param>
    /// <param name="poolSize"></param>
    /// <param name="reserve">Pre-create number of views to avoid lag spikes later, useful to do in backgound.</param>
    public void InitializeTemplates(Func<object> template, IList dataContexts, int poolSize, int reserve = 0)
    {
        {
            //Debug.WriteLine("[CELLS] InitializeTemplates");
            if (template == null)
            {
                TemplatesInvalidated = false;
                TemplesInvalidating = false;
                return;
            }

            async Task InitializeFull(bool measure)
            {
                lock (_lockTemplates)
                {
                    lock (_parent.LockMeasure)
                    {
                        TemplesInvalidating = false;

                        var kill = _templatedViewsPool;

                        _dicoCellsInUse.Clear();
                        _templatedViewsPool = new TemplatedViewsPool(template, poolSize);

                        FillPool(reserve, dataContexts);

                        if (kill != null)
                        {
                            //we need a delay here for several threads access, if previous cells are still being drawn. not elegant but.. remains in global todo to find a better way.
                            Tasks.StartDelayed(TimeSpan.FromSeconds(3.5), () =>
                            {
                                kill?.Dispose();
                            });
                        }

                        if (measure)
                        {
                            //Debug.WriteLine($"Measuring upon InitializeFull for {_parent.Tag} for H: {_parent._lastMeasuredForHeight}");
                            //_parent.MeasureLayout(new(_parent._lastMeasuredForWidth, _parent._lastMeasuredForHeight, _parent.RenderingScale), true);
                            //_parent.Update();
                            _parent.Invalidate();
                        }

                        TemplatesAvailable();
                    }

                    Monitor.PulseAll(_lockTemplates);

                }


            }

            void InitializeSoft(bool layoutChanged)
            {

                lock (_lockTemplates)
                {
                    TemplesInvalidating = false;

                    _templatedViewsPool.MaxSize = poolSize;
                    if (layoutChanged)
                    {
                        foreach (var view in _dicoCellsInUse.Values)
                        {
                            view.InvalidateChildrenTree();
                        }
                    }
                    //MarkAllViewsAsHidden(); //todo think
                    Monitor.PulseAll(_lockTemplates);
                }

                TemplatesAvailable();
            }

            void TemplatesAvailable()
            {
                _dataContexts = dataContexts;
                TemplatesInvalidated = false;
                TemplatesBusy = false;
                _parent.OnTemplatesAvailable();
            }

            bool CheckTemplateChanged()
            {
                var ret = _templatedViewsPool.CreateTemplate != template
                           //|| _parent.SplitMax != _forColumns || _parent.MaxRows != _forRows
                           ;
                return ret;
            }

            var layoutChanged = true //todo cannot really optimize as can have same nb of cells, same references for  _dataContexts != dataContexts but different contexts
             || _parent.RenderingScale != _forScale || _parent.Split != _forSplit;

            if (layoutChanged || _templatedViewsPool == null || _dataContexts != dataContexts || CheckTemplateChanged())
            {
                //temporarily fixed to android until issue found
                //lock (_lockTemplates)
                {
                    //kill provider ability to provide deprecated templates
                    _wrappers.Clear();
                    _forScale = _parent.RenderingScale;
                    _forSplit = _parent.Split;
                    _dataContexts = null;
                    AddedMore = 0;
                }

                if (dataContexts.Count == 0)
                {
                    TemplatesAvailable();
                    return;
                }

                if (_parent.InitializeTemplatesInBackgroundDelay > 0)
                {
                    //postpone initialization to be executed in background
                    Tasks.StartDelayed(TimeSpan.FromMilliseconds(_parent.InitializeTemplatesInBackgroundDelay),
                    () =>
                    {
                        Task.Run(async () => //100% background thread
                        {


                            try
                            {
                                await InitializeFull(_parent.RecyclingTemplate == RecyclingTemplate.Disabled || _parent.NeedAutoSize);
                            }
                            catch (Exception e)
                            {
                                Super.Log(e);
                            }

                        }).ConfigureAwait(false);
                    });
                }
                else
                {
                    InitializeFull(false).ConfigureAwait(false);
                }
            }
            else
            {
                //looks like only itemssource has changed, resize pool, keep old templates, be fast
                InitializeSoft(layoutChanged);
            }
        }
    }

    public void ContextCollectionChanged(Func<object> template, IList dataContexts, int poolSize, int reserve = 0)
    {
        if (_templatedViewsPool == null)
        {
            InitializeTemplates(template, dataContexts, poolSize, reserve);
            return;
        }

        _templatedViewsPool.MaxSize = poolSize;
        //        if (layoutChanged)
        {
            foreach (var view in _dicoCellsInUse.Values)
            {
                view.InvalidateChildrenTree();
            }
        }
    }

    public void UpdateViews(IEnumerable<SkiaControl> views = null)
    {

        if (_parent.IsTemplated)
        {
            UpdateVisibleViews();
        }

    }


    /// <summary>
    /// An important check to consider before consuming templates especially if you initialize templates in background
    /// </summary>
    public bool TemplatesAvailable
    {
        get
        {
            return (_templatedViewsPool != null
                    && _dataContexts != null)
                   && !TemplatesInvalidated;
        }
    }

    public ViewsIterator GetViewsIterator()
    {
        //lock (_lockTemplates)
        {
            if (_parent.IsTemplated)
            {
                if (!TemplatesAvailable)
                {
                    throw new InvalidOperationException("Templates have not been initialized.");
                }

                int threadId = Thread.CurrentThread.ManagedThreadId;

                if (!_wrappers.TryGetValue(threadId, out ViewsIterator wrapper))
                {
                    wrapper = new ViewsIterator(_templatedViewsPool, _dataContexts);
                    _wrappers.TryAdd(threadId, wrapper);
                }

                return wrapper;
            }
            else
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                var children = _parent.GetUnorderedSubviews();

                if (!_wrappers.TryGetValue(threadId, out ViewsIterator iterator))
                {
                    iterator = new ViewsIterator(children);

                    _wrappers.TryAdd(threadId, iterator);
                }
                else
                {
                    iterator.SetViews(children);
                }

                return iterator;
            }

        }
    }

    public void DisposeWrapper()
    {
        int threadId = Thread.CurrentThread.ManagedThreadId;

        if (_wrappers.TryGetValue(threadId, out ViewsIterator wrapper))
        {
            _wrappers.Remove(threadId);
            wrapper.Dispose();
        }
    }

    public SkiaControl GetViewAtIndex(int index, SkiaControl template = null)
    {
        //lock (_lockTemplates) //to avoid getting same view for different indexes
        {

            if (_templatedViewsPool == null || _dataContexts == null)
            {
                throw new InvalidOperationException("Templates have not been initialized.");
            }

            if (index < 0 || index >= _dataContexts.Count)
            {
                return null;
            }

            if (template == null)
            {
                template = _templatedViewsPool.Get();
                if (LogEnabled)
                {
                    Trace.WriteLine($"[ViewsAdapter] {_parent.Tag} for index {index} returned tpl {template.Uid}");
                }
            }
            else
            {
                if (LogEnabled)
                {
                    Trace.WriteLine($"[ViewsAdapter] {_parent.Tag} for index {index} used passed returned tpl {template.Uid}");
                }
            }

            return template;//new ViewModelWrapper(this, template);
        }
    }

    public void PrintDebugVisible()
    {
        lock (lockVisible)
        {
            Trace.WriteLine($"Visible views {_dicoCellsInUse.Count}:");
            foreach (var view in _dicoCellsInUse.Values)
            {
                Trace.WriteLine($"└─ {view} {view.Width:0.0}x{view.Height:0.0} pts ({view.MeasuredSize.Pixels.Width:0.0}x{view.MeasuredSize.Pixels.Height:0.0} px) ctx: {view.BindingContext}");
            }
        }
    }

    public void ReleaseView(SkiaControl viewModel, bool reset = false)
    {
        //lock (_lockTemplates)
        {
            if (reset)
            {
                viewModel.SetParent(null);
                //viewModel.BindingContext = null;
            }
            _templatedViewsPool.Return(viewModel);
        }
    }

    public SkiaControl GetTemplateInstance()
    {
        lock (_lockTemplates)
        {
            if (_templatedViewsPool == null || _dataContexts == null)
            {
                throw new InvalidOperationException("Templates have not been initialized.");
            }

            SkiaControl viewModel = _templatedViewsPool.Get();

            return viewModel;
        }
    }


    /// <summary>
    /// Used by ViewsProvider
    /// </summary>
    public class TemplatedViewsPool : IDisposable
    {
        private readonly Stack<SkiaControl> _pool;
        public Func<object> CreateTemplate { get; protected set; }
        public int MaxSize { get; set; }
        private readonly object _syncLock = new object();
        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var viewModel in _pool)
                    {
                        if (viewModel != null)
                        {
                            viewModel.ContextIndex = -1;
                            viewModel.BindingContext = null;
                            if (viewModel is IDisposable disposableViewModel)
                            {
                                disposableViewModel.Dispose();
                            }
                        }
                    }
                }

                _disposed = true;
            }
        }

        public TemplatedViewsPool(Func<object> initialViewModel, int maxSize)
        {
            CreateTemplate = initialViewModel;
            MaxSize = maxSize;
            _pool = new();
        }

        public int Size
        {
            get
            {
                //lock (_syncLock)
                {
                    return _pool.Count;
                }
            }
        }

        /// <summary>
        /// unsafe
        /// </summary>
        /// <returns></returns>
        SkiaControl CreateFromTemplate()
        {
            var create = CreateTemplate();

            if (LogEnabled)
                Trace.WriteLine($"[ViewsAdapter] created new view !");

            if (create is SkiaControl element)
            {
                return element;
            }
            return (SkiaControl)create;
        }

        /// <summary>
        /// Just create template and save for the future
        /// </summary>
        public void Reserve()
        {
            //lock (_syncLock)
            {
                if (_pool.Count < MaxSize)
                {
                    try
                    {
                        _pool.Push(CreateFromTemplate());
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e);
                        throw;
                    }
                }
            }

        }

        public SkiaControl Get()
        {
            //lock (_syncLock)
            {
                SkiaControl viewModel = null;

                try
                {
                    if (_pool.Count > 0)
                        viewModel = _pool.Pop();
                    if (viewModel != null)
                    {
                        return viewModel;
                    }

                    if (_pool.Count < MaxSize)
                    {
                        return CreateFromTemplate();
                    }

                    // Wait and try again if the pool is full
                    viewModel = _pool.Pop();
                    while (viewModel != null)
                    {
                        System.Threading.Thread.Sleep(10); //todo add cancellation
                        viewModel = _pool.Pop();
                    }
                }
                catch (Exception e)
                {
                    Super.Log(e);
                    throw e;
                }



                return viewModel;
            }


        }


        public void Return(SkiaControl viewModel)
        {
            //lock (_syncLock)
            {
                _pool.Push(viewModel);
            }
        }
    }


    /// <summary>
    /// To iterate over virtual views
    /// </summary>
    public class ViewsIterator : IEnumerable<SkiaControl>, IDisposable
    {
        private TemplatedViewsPool _templatedViewsPool;
        private IList _dataContexts;
        private IEnumerable<SkiaControl> _views;
        private DataContextIterator _iterator;

        public bool IsTemplated { get; }

        public TemplatedViewsPool TemplatedViewsPool => _templatedViewsPool;
        public IList DataContexts => _dataContexts;
        public IEnumerable<SkiaControl> Views => _views;

        public ViewsIterator(TemplatedViewsPool templatedViewsPool, IList dataContexts)
        {
            _templatedViewsPool = templatedViewsPool;
            _dataContexts = dataContexts;
            IsTemplated = true;
        }

        public void SetViews(IEnumerable<SkiaControl> views)
        {
            _views = views;
        }

        public ViewsIterator(IEnumerable<SkiaControl> views)
        {
            _views = views;
            IsTemplated = false;
        }

        public IEnumerator<SkiaControl> GetEnumerator()
        {
            _iterator = new DataContextIterator(this);
            return _iterator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            if (_iterator != null)
            {
                _iterator.Dispose();
                _iterator = null;
            }
        }
    }


    public class DataContextIterator : IEnumerator<SkiaControl>
    {
        private readonly ViewsIterator _viewsProvider;
        private int _currentIndex;
        private SkiaControl _currentViewModel;
        private IEnumerator<SkiaControl> _viewEnumerator;

        //added this to use more that 1 view at a time
        private readonly Queue<SkiaControl> _viewsInUse;

        public DataContextIterator(ViewsIterator viewsProvider)
        {
            _viewsProvider = viewsProvider;
            _currentIndex = -1;
            _viewsInUse = new Queue<SkiaControl>();

            if (!_viewsProvider.IsTemplated)
            {
                _viewEnumerator = _viewsProvider.Views.GetEnumerator();
            }
        }

        public SkiaControl Current => _currentViewModel;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_viewsProvider.IsTemplated)
            {

                // Dequeue and return the oldest view if we're at capacity.
                if (_viewsInUse.Count >= _viewsProvider.TemplatedViewsPool.MaxSize)
                {
                    var oldestView = _viewsInUse.Dequeue();

                    _viewsProvider.TemplatedViewsPool.Return(oldestView);
                }

                _currentIndex++;

                if (_currentIndex < _viewsProvider.DataContexts.Count)
                {
                    _currentViewModel = _viewsProvider.TemplatedViewsPool.Get();

                    _viewsInUse.Enqueue(_currentViewModel);  // Keep track of the views in use.

                    _currentViewModel.ContextIndex = _currentIndex;
                    _currentViewModel.BindingContext = _viewsProvider.DataContexts[_currentIndex];
                    return true;
                }




                return false;
            }

            bool hasMore = _viewEnumerator.MoveNext();
            if (hasMore)
            {
                _currentViewModel = _viewEnumerator.Current;
            }
            return hasMore;

        }

        public void Reset()
        {
            if (_viewsProvider.IsTemplated)
            {
                if (_currentIndex >= 0 && _currentIndex < _viewsProvider.DataContexts.Count)
                {
                    _viewsProvider.TemplatedViewsPool.Return(_currentViewModel);
                }

                _currentIndex = -1;
            }
            else
            {
                _viewEnumerator.Reset();
            }
        }

        public void Dispose()
        {
            if (_viewsProvider.IsTemplated && _currentIndex >= 0 && _currentIndex < _viewsProvider.DataContexts.Count)
            {
                _viewsProvider.TemplatedViewsPool.Return(_currentViewModel);
            }
        }
    }


}

