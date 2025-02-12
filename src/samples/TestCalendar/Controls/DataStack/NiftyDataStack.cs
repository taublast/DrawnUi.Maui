using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

using AppoMobi.Touch;
using Microsoft.Maui.Controls.Internals;

namespace AppoMobi.Xam
{

 

    public class NiftyDataStack : AppoMobi.Touch.LegacyGesturesStackLayout
    {

        public event EventHandler<DownUpEventArgs> ItemDown;
        public event EventHandler<AppoMobi.Touch.TapEventArgs> ItemTapped;

        public void CallItemTapped(object item, TapEventArgs e)
        {
            ItemTapped?.Invoke(item, e);
        }

        public void CallItemDown(object item, DownUpEventArgs e)
        {
            ItemDown?.Invoke(item, e);
        }

        int SelectedRow = -1;
        AppoMobi.Touch.LegacyGesturesBoxView SelectionBox = null;

        public bool IsPressed { get; set; } = false;
        public bool IsPanned { get; set; } = false;

        
        public void Dispose()
        
        {
            DisposeChildrenBuffer();
        }
        
        protected void DisposeChildrenBuffer()
        
        {
            if (!ChildrenBuffer.Any()) return;
            //todo call all nifty cells dispose
            try
            {
                foreach (var child in ChildrenBuffer)
                {
                    try
                    {
                        var disposable = (NiftyCell)child;
                        disposable.Dispose();
                    }
                    catch (Exception e)
                    {
                    }
                    //try
                    //{
                    //    var disposable = (SliderCell)child;
                    //    disposable.Dispose();
                    //}
                    //catch (Exception e)
                    //{
                    //}
                }
                ChildrenBuffer.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        
        public NiftyDataStack()
        
        {
            //Console.WriteLine("*** Created new DATAStacK ***");
            Views = Children;
            Spacing = 0;
            Padding = 0;
           // SizeChanged += OnMySizeChanged;
            //ChildsToAdd=new List<View>();
        }


        //public void ShowLoader(bool show = true)
        //{
        //    try
        //    {
        //        var first = Views[0];
        //        if (first is Spinner)
        //        {
        //            if (!show)
        //            {
        //                //remove spinner
        //                MainThread.BeginInvokeOnMainThread(() =>
        //                {
        //                    // Update the UI
        //                    Children.RemoveAt(0);
        //                });
        //            }
        //        }
        //        else
        //        {
        //            if (show)
        //            {
        //                MainThread.BeginInvokeOnMainThread(() =>
        //                {
        //                    // Update the UI
        //                    Children.Insert(0, new Spinner());
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}
        bool busyProcessSizeUpdating { get; set; }
        /// <summary>
        /// Call this externally to force updating sizes for cells
        /// </summary>
        
        public void ProcessSizeUpdating(double basewidth)
        
        {
            if (busyProcessSizeUpdating) return;
            busyProcessSizeUpdating = true;
            var count = 0;
            foreach (var item in Views)
            {
                var cell = (NiftyCell)item;
                cell.OnParentSizeChangeding(basewidth);
            }
            busyProcessSizeUpdating = false;
        }

        bool busyProcessVisibility { get; set; }
        /// <summary>
        /// Call externally to force calling OnAppearing and OnDissapearing for cells
        /// For example call when your scroll the parent ScrollView
        /// ex: cDataStack.ProcessVisibility(e.ScrollY, MainScroll.Height);
        /// </summary>
        /// <param name="position"></param>
        /// <param name="viewport"></param>
        /// <param name="vertical"></param>
        
        public void ProcessVisibility(double position, double viewport, bool vertical = true)
        
        {
            if (busyProcessVisibility) return;
            busyProcessVisibility = true;
            var p = position + viewport;
            //	Debug.WriteLine("VISiBILITY {0} {1}-{2} {3}", viewport, position, p, Height+Y);

            var top = position + Y;
            var bottom = position + viewport + Y;

            var count = 0;
            foreach (var item in Views)
            {
                count++;
                var visible = false;
                
                if (!(item is NiftyCell)) continue;

                var cell = (NiftyCell)item;
                var amount = 0.0d;

                if (vertical)
                {
                    //vertical scroll
                    if (cell.Y <= bottom && cell.Y >= top)
                    {
                        visible = true; //fits partly from bottom
                        amount = (bottom - cell.Y) / cell.Height;
                        if (amount > 1) amount = 1.0d;
                        cell.AmountAppeared = amount; //0.0 - 1.0
                    }
                    else
                    if (cell.Bounds.Bottom >= top && cell.Bounds.Bottom <= bottom)
                    {
                        visible = true; //fits partly from top
                        amount = (cell.Bounds.Bottom - top) / cell.Height;
                        if (amount > 1) amount = 1.0d;
                        cell.AmountAppeared = amount; //0.0 - 1.0
                    }
                    else
                    {
                        cell.AmountAppeared = 0.0;
                    }
                }
                else
                {
                    //horizontal scroll
                    if (cell.X <= bottom && cell.X >= top)
                    {
                        visible = true; //fits partly from right
                        amount = (bottom - cell.X) / cell.Width;
                        if (amount > 1) amount = 1.0d;
                        cell.AmountAppeared = amount; //0.0 - 1.0
                    }
                    else
                    if (cell.Bounds.Right >= top && cell.Bounds.Right <= bottom)
                    {
                        visible = true; //fits partly from left
                        amount = (cell.Bounds.Right - top) / cell.Width;
                        if (amount > 1) amount = 1.0d;
                        cell.AmountAppeared = amount; //0.0 - 1.0
                    }
                    else
                    {
                        cell.AmountAppeared = 0.0;
                    }
                }

                cell.Appeared = visible;
                //if (visible)
                //    Debug.WriteLine("Cell {0} visible {1}", cell.Tag, cell.AmountAppeared);
            }
            busyProcessVisibility = false;
        }


        
        public void ProcessSlidesVisibility(double position, double viewport, bool vertical = true)
        
        {
            if (busyProcessVisibility) return;
            busyProcessVisibility = true;
            var p = position + viewport;
            //	Debug.WriteLine("VISiBILITY {0} {1}-{2} {3}", viewport, position, p, Height+Y);

            var top = position + Y;
            var bottom = position + viewport + Y;

            var count = 0;
            foreach (var item in Views)
            {
                count++;
                var visible = false;

                //if (!(item is SliderCell)) continue;

                //var cell = (SliderCell)item;
                //var amount = 0.0d;

                //if (vertical)
                //{
                //    //vertical scroll
                //    if (cell.Y <= bottom && cell.Y >= top)
                //    {
                //        visible = true; //fits partly from bottom
                //        amount = (bottom - cell.Y) / cell.Height;
                //        if (amount > 1) amount = 1.0d;
                //        cell.AmountAppeared = amount; //0.0 - 1.0
                //    }
                //    else
                //    if (cell.Bounds.Bottom >= top && cell.Bounds.Bottom <= bottom)
                //    {
                //        visible = true; //fits partly from top
                //        amount = (cell.Bounds.Bottom - top) / cell.Height;
                //        if (amount > 1) amount = 1.0d;
                //        cell.AmountAppeared = amount; //0.0 - 1.0
                //    }
                //    else
                //    {
                //        cell.AmountAppeared = 0.0;
                //    }
                //}
                //else
                //{
                //    //horizontal scroll
                //    if (cell.X <= bottom && cell.X >= top)
                //    {
                //        visible = true; //fits partly from right
                //        amount = (bottom - cell.X) / cell.Width;
                //        if (amount > 1) amount = 1.0d;
                //        cell.AmountAppeared = amount; //0.0 - 1.0
                //    }
                //    else
                //    if (cell.Bounds.Right >= top && cell.Bounds.Right <= bottom)
                //    {
                //        visible = true; //fits partly from left
                //        amount = (cell.Bounds.Right - top) / cell.Width;
                //        if (amount > 1) amount = 1.0d;
                //        cell.AmountAppeared = amount; //0.0 - 1.0
                //    }
                //    else
                //    {
                //        cell.AmountAppeared = 0.0;
                //    }
                //}

                //cell.Appeared = visible;
                //if (visible)
                //    Debug.WriteLine("Cell {0} visible {1}", cell.Tag, cell.AmountAppeared);
            }
            busyProcessVisibility = false;
        }
        
        protected override void OnPropertyChanged([CallerMemberName]string propertyName = null)
        
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameItemsSource:
                    ItemsSourceChanged();
                    break;

                //property changed
                case nameShowMax:

                    break;

                //property changed
                case nameGroupByFirstLetter:
                     ItemsSourceChanged();
                    break;


            }

        }
        
        
        public async Task SendCommandToCells(string command, object param)
        
        {
            foreach (var item in Views)
            {
                if (item is NiftyCell)
                {
                    await ((NiftyCell) item).CommandFromParent(command, param);
                }
            }
        }

        
        // IsSubbed
        const string nameIsSubbed = "IsSubbed";
        public static readonly BindableProperty IsSubbedProperty = BindableProperty.Create(nameIsSubbed, typeof(bool), typeof(NiftyDataStack), false); //, BindingMode.TwoWay
        public bool IsSubbed
        {
            get { return (bool)GetValue(IsSubbedProperty); }
            set { SetValue(IsSubbedProperty, value); }
        }	
   

        
        // ShowMax
        const string nameShowMax = "ShowMax";
        public static readonly BindableProperty ShowMaxProperty = BindableProperty.Create(nameShowMax, typeof(int), typeof(NiftyDataStack), -1); //, BindingMode.TwoWay
        public int ShowMax
        {
            get { return (int)GetValue(ShowMaxProperty); }
            set { SetValue(ShowMaxProperty, value); }
        }

        
        // Tag
        const string nameTag = "Tag";
        public static readonly BindableProperty TagProperty = BindableProperty.Create(nameTag, typeof(string), typeof(NiftyDataStack), string.Empty);
        public string Tag
        {
            get { return (string)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }


        
        // SelectedParams
        const string nameSelectedParams = "SelectedParams";
        public static readonly BindableProperty SelectedParamsProperty = BindableProperty.Create(nameSelectedParams, typeof(string), typeof(NiftyDataStack), string.Empty);
        public string SelectedParams
        {
            get { return (string)GetValue(SelectedParamsProperty); }
            set { SetValue(SelectedParamsProperty, value); }
        }

        //*******************************************************************************
        public class GrouppedCell
        //*******************************************************************************
        {
            public string Title { get; set; }
            public NiftyCell Cell { get; set; }
        }


        public IList<IView> Views { get; private set; } = null;
        public List<View> ChildrenBuffer { get; private set; } = new List<View>();
    
        public List<GrouppedCell> GroupsBuffer { get; private set; } = new List<GrouppedCell>();


        //-------------------------------------------------------------
        // DelayRendering
        //-------------------------------------------------------------
        const string nameDelayRendering = "DelayRendering";
        public static readonly BindableProperty DelayRenderingProperty = BindableProperty.Create(nameDelayRendering, typeof(bool), typeof(NiftyDataStack), false); //, BindingMode.TwoWay
        /// <summary>
        /// Render must be called manually to render items on UI thread
        /// </summary>
        public bool DelayRendering
        {
            get { return (bool)GetValue(DelayRenderingProperty); }
            set { SetValue(DelayRenderingProperty, value); }
        }


        //-------------------------------------------------------------
        // Rendered
        //-------------------------------------------------------------
        const string nameRendered = "Rendered";
        public static readonly BindableProperty RenderedProperty = BindableProperty.Create(nameRendered, typeof(bool), typeof(NiftyDataStack), false); //, BindingMode.TwoWay
        public bool Rendered
        {
            get { return (bool)GetValue(RenderedProperty); }
            protected set { SetValue(RenderedProperty, value); }
        }	




        
        void BuildFromZero()
        
        {
            //DisposeChildrenBuffer();
            if (ItemsSource == null)
            {
                return;
            }

            var max = ItemsSource.Count;
            if (max > 0)
            {
                if (ShowMax > 0 && ShowMax < max) max = ShowMax;
            }
            else
            {
                return;
            }

            Rendered = false;
            RenderedPosition = -1;
            RenderedPage = -1;

            string cellType = "";
            var found = false;

            //doublebuffering
            ChildrenBuffer.Clear();
            GroupsBuffer.Clear();

            var typeDetected = false;

            for (int c = 0; c < max; c++) // loop though all items
            {
                // CREATING CELLL FROM TEMPLATE 
                var newItem = ItemsSource[c];
                View view = null;
                if (ItemTemplate is DataTemplateSelector)
                {
                    var tpl = ItemTemplate.SelectDataTemplate(newItem, this);
                    view = (View)tpl.CreateContent(); // <------------------------------ !!! create empty cell
                }
                else
                {
                    view = (View)ItemTemplate.CreateContent(); // <------------------------------ !!! create empty cell
                }

                //detect cell type
                if (!typeDetected)  
                {
                    try
                    {
                        var tryCell = (NiftyCell)view;
                        cellType = "NiftyCell";
                        found = true;
                    }
                    catch (Exception e)
                    {
                    }
                    //if (!found)
                    //{
                    //    try
                    //    {
                    //        var tryCell = (SliderCell)view;
                    //        cellType = "SliderCell";
                    //        found = true;
                    //    }
                    //    catch (Exception e)
                    //    {
                    //    }
                    //}
                    typeDetected = true;
                }
                var bindableObject = view as BindableObject;
                if (bindableObject != null)
                {
                    if (found)
                    {
                        if (cellType == "NiftyCell")
                        {
                            ((NiftyCell)view).Tag = cellType + " " + c.ToString();
                            ((NiftyCell)view).Index = c;
                            ((NiftyCell)view).ParentWidth = Width; //NEW for thumbnails
                        }
                        //else
                        //if (cellType == "SliderCell")
                        //{
                        //    ((SliderCell)view).Tag = cellType + " " + c.ToString();
                        //    ((SliderCell)view).Index = c;
                        //}
                    }
                    bindableObject.BindingContext = newItem; // <------------------------------ !!! send data to cell
                    
                    //get info for header after cell initialized
                    if (GroupByFirstLetter && cellType == "NiftyCell")
                    {
                        var title = ((NiftyCell)view).Title;
                        if (string.IsNullOrEmpty(title)) title = "A";
                        GroupsBuffer.Add(new GrouppedCell { Cell = (NiftyCell)view, Title = title });
                    }
                    else
                    {
                        ChildrenBuffer.Add(view);
                    }
                }
            }

            #region GroupByFirstLetter
            if (GroupByFirstLetter && cellType == "NiftyCell")
            {
                var orderedGroups = GroupsBuffer.OrderBy(x => x.Title).ToList();
                Groups = orderedGroups.GroupBy(s => s.Title[0]).ToList();
                //create letter views
                foreach (var group in Groups)
                {
                    var view = (View)GroupHeaderTemplate.CreateContent();
                    var bindableObject = view as BindableObject;
                    if (bindableObject != null)
                    {
                        bindableObject.BindingContext = group.Key.ToString();
                        ChildrenBuffer.Add(view); //add header
                    }
                    var orderedGroup = group.OrderBy(x => x.Title);
                    foreach (var cell in orderedGroup)
                    {
                        ChildrenBuffer.Add(cell.Cell);
                    }
                }
            }
            #endregion

            //doublebuffering
            if (!DelayRendering)
                Render();

            ItemsLoaded = true;
        }



         
        public List<IGrouping<char, GrouppedCell>> Groups { get; set; }
        


        //-------------------------------------------------------------
        // PageSize
        //-------------------------------------------------------------
        const string namePageSize = "PageSize";
        public static readonly BindableProperty PageSizeProperty = BindableProperty.Create(namePageSize, typeof(int), typeof(NiftyDataStack), 0); //, BindingMode.TwoWay
        /// <summary>
        /// Paged output size in items. Disabled if PageSize is less than 1
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        /// <summary>
        /// Counting from 0
        /// </summary>
        public int RenderedPosition { get; protected set; }

        /// <summary>
        /// Counting from 0
        /// </summary>
        public int RenderedPage { get; protected set; }

        /// <summary>
        /// Counting from 1
        /// </summary>
        public int TotalPages
        {
            get
            {
                if (PageSize < 1) return 1;
                var total = (int)(ChildrenBuffer.Count / PageSize);
                if (total * PageSize < Views.Count) total++;
                return total;
            }
        }

        //property changed
        //       case namePageSize:

        protected bool busyRendering { get; set; }
        
        public void Render(int page = 0)
        
        {


            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (busyRendering) return;
                busyRendering = true;

                try
                {
                    // Update the UI
                    //doublebuffering
                    if (page == 0)
                    {

                        //Children.Clear();
                        Children.Clear();
                    }

                    var startPos = PageSize * page;

                    var pos = 0;
                    var unlockCells = new List<NiftyCell>();
                    foreach (var child in ChildrenBuffer)
                    {
                        var render = false;
                        if (PageSize > 0)
                        {
                            if (pos >= startPos)
                            {
                                render = true;
                            }
                        }
                        else
                        {
                            render = true;
                        }

                        if (render)
                        {
                            if (child is NiftyCell)
                            {
                                ((NiftyCell)child).LockRendering = true;
                                unlockCells.Add((NiftyCell)child);
                            }
                            Children.Add(child);
                            RenderedPosition = pos;
                            //Task.Delay(1);
                        }

                        pos++;
                        if (PageSize > 0)
                        {
                            if (pos > startPos + PageSize) { break; }
                        }
                    }

                    foreach (var cell in unlockCells)
                    {
                        cell.LockRendering = false;
                    }

                    RenderedPage = page;
                    Rendered = true;
                    FinishedDrawing?.Invoke(this, new EventArgs());         
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                busyRendering = false;
            });

        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int index = args.NewStartingIndex;
                        if (args.NewItems != null)
                        {
                            if (Children.Count > 0)
                            {
                                //insert somewhere
                                foreach (var newItem in args.NewItems)
                                {
                                    View view = null;
                                    try
                                    {
                                        view = (View)ItemTemplate.CreateContent();
                                    }
                                    catch (Exception ex)
                                    {
                                        var stopp = ex;
                                    }
                                    var bindableObject = view as BindableObject;
                                    if (bindableObject != null)
                                    {
                                        bindableObject.BindingContext = newItem;
                                        Children.Insert(index++, view);
                                    }
                                }
                            }
                            else
                            {
                                BuildFromZero();
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move: //todo for list
                    {
                        var view = (View)Children[args.OldStartingIndex];
                        Children.RemoveAt(args.OldStartingIndex);
                        Children.Insert(args.NewStartingIndex, view);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove://todo for list
                    {
                        Children.RemoveAt(args.OldStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace://todo for list
                    {
                        //var view = (View)ItemTemplate.CreateContent();
                        //var bindableObject = view as BindableObject;
                        //if (bindableObject != null)
                        //    bindableObject.BindingContext = args.NewItems[0];
                        //Children.RemoveAt(args.OldStartingIndex);
                        //Children.Insert(args.NewStartingIndex, view);

                        int index = args.NewStartingIndex;

                        if (Children.Count > 0)
                        {
                            //process list
                            foreach (var item in args.NewItems)
                            {
                                //get the existing view
                                //update binding context or other refresh method
                                View view = null;
                                var max = ItemsSource.Count;
                                if (max > 0)
                                {
                                    if (ShowMax > 0 && ShowMax < max) max = ShowMax;
                                }
                                try
                                {
                                    view = (View)Children[index];
                                    view.BindingContext = item;
                                    index++;
                                    if (index > max-1) break;
                                }
                                catch (Exception ex)
                                {
                                    break;
                                }
                            }

                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    DisposeChildrenBuffer();
                    Children.Clear();
                    if (args.NewItems != null)
                    {
                        foreach (var newItem in args.NewItems)
                        {
                            var view = (View)ItemTemplate.CreateContent();
                            var bindableObject = view as BindableObject;
                            if (bindableObject != null)
                                bindableObject.BindingContext = newItem;
                            Children.Add(view);
                        }
                    }
                    break;
            }

                      
        }

        INotifyCollectionChanged oldSource { get; set; }

        
        void ItemsSourceChanged()
        
        {
            Debug.WriteLine("[DataStack] ItemsSourceChanged");
            ItemsLoaded = false;

            //bug
            //cannot process this when not on uithread



            //DisposeChildrenBuffer();
            //Children.Clear();

            if (ItemsSource == null)
            {
                // Update the UI
                DisposeChildrenBuffer();
                //Children.Clear();
                //MainThread.BeginInvokeOnMainThread(() =>
                //{
                    
                //});

                return;
            }

            
            BuildFromZero();

            //new fix
            if (oldSource != null)
                oldSource.CollectionChanged -= OnCollectionChanged;

            oldSource = ItemsSource as INotifyCollectionChanged;


            var notifyCollection = ItemsSource as INotifyCollectionChanged;
            if (notifyCollection != null)
            {
                notifyCollection.CollectionChanged += OnCollectionChanged;

                //   if (args.NewItems != null)
                //   {

                //       foreach (var newItem in args.NewItems)
                //       {

                //           var view = (View)ItemTemplate.CreateContent();
                //           var bindableObject = view as BindableObject;
                //           if (bindableObject != null)
                //               bindableObject.BindingContext = newItem;
                //           Children.Add(view);
                //       }
                //   }
                //   else
                //   {
                //       Children.Clear();
                //   }
                //if (args.OldItems != null)
                //{
                //  not supported
                // Children.RemoveAt(args.OldStartingIndex);
                //}
                //if (args.OldItems != null)
                //{
                //    // not supported
                //    Children.RemoveAt(args.OldStartingIndex);
                //}
            };
        }


        
        public DataTemplate ItemTemplate
        
        {
            get;
            set;
        }

        
        public DataTemplate GroupHeaderTemplate
        
        {
            get;
            set;
        }

        //
        //// SubitemsSource
        //
        //private const string nameSubitemsSource = "SubitemsSource";
        //public static readonly BindableProperty SubitemsSourceProperty = BindableProperty.Create(nameSubitemsSource, typeof(IList), typeof(NiftyDataStack), null, BindingMode.TwoWay);
        //public IList SubitemsSource
        //{
        //    get { return (IList)GetValue(SubitemsSourceProperty); }
        //    set { SetValue(SubitemsSourceProperty, value); }
        //}


        //-------------------------------------------------------------
        // GroupByFirstLetter
        //-------------------------------------------------------------
        const string nameGroupByFirstLetter = "GroupByFirstLetter";
        public static readonly BindableProperty GroupByFirstLetterProperty = BindableProperty.Create(nameGroupByFirstLetter, typeof(bool), typeof(NiftyDataStack), false); //, BindingMode.TwoWay
        public bool GroupByFirstLetter
        {
            get { return (bool)GetValue(GroupByFirstLetterProperty); }
            set { SetValue(GroupByFirstLetterProperty, value); }
        }	


        
        // ItemsSource
        const string nameItemsSource = "ItemsSource";
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameItemsSource, typeof(IList), typeof(NiftyDataStack), null, BindingMode.TwoWay);
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }


        //-------------------------------------------------------------
        // ItemsLoaded
        //-------------------------------------------------------------
        const string nameItemsLoaded = "ItemsLoaded";
        public static readonly BindableProperty ItemsLoadedProperty = BindableProperty.Create(nameItemsLoaded, typeof(bool), typeof(NiftyDataStack), false); //, BindingMode.TwoWay
        public bool ItemsLoaded
        {
            get { return (bool)GetValue(ItemsLoadedProperty); }
            private set { SetValue(ItemsLoadedProperty, value); }
        }


        ////-------------------------------------------------------------
        //// WithSubchilds
        ////-------------------------------------------------------------
        //private const string nameWithSubchilds = "WithSubchilds";
        //public static readonly BindableProperty WithSubchildsProperty = BindableProperty.Create(nameWithSubchilds, typeof(bool), typeof(NiftyDataStack), false); //, BindingMode.TwoWay
        //public bool WithSubchilds
        //{
        //    get { return (bool)GetValue(WithSubchildsProperty); }
        //    set { SetValue(WithSubchildsProperty, value); }
        //}	

        /// <summary>
        /// Raised when children are drawn
        /// </summary>
        public event EventHandler FinishedDrawing = null;

        
        // Tapped
        
        public new event EventHandler LongPressed = null;

        
        /// <summary>
        ///     Controls whether anything happens in BeginRefresh(), is set based on RefreshCommand.CanExecute
        /// </summary>
        bool _refreshAllowed = true;
        public bool RefreshAllowed
        
        {
            set
            {
                if (_refreshAllowed == value)
                    return;

                _refreshAllowed = value;
                OnPropertyChanged();
            }
            get { return _refreshAllowed; }
        }

        
        public void BeginRefresh()
        
        {
            if (!RefreshAllowed)
                return;

            SetValue(IsRefreshingProperty, true);
            OnRefreshing(EventArgs.Empty);

            ICommand command = RefreshCommand;
            if (command != null)
                command.Execute(null);
        }

        public event EventHandler Refreshing;

        public static readonly BindableProperty
            RefreshCommandProperty = BindableProperty.Create("RefreshCommand", typeof(ICommand),
                typeof(NiftyDataStack), null, propertyChanged: OnRefreshCommandChanged);
        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }
        
        static void OnRefreshCommandChanged(BindableObject bindable, object oldValue, object newValue)
        
        {
            var lv = (NiftyDataStack)bindable;
            var oldCommand = (ICommand)oldValue;
            var command = (ICommand)newValue;

            lv.OnRefreshCommandChanged(oldCommand, command);
        }

        
        void OnRefreshCommandChanged(ICommand oldCommand, ICommand newCommand)
        
        {
            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= OnCommandCanExecuteChanged;
            }

            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += OnCommandCanExecuteChanged;
                RefreshAllowed = newCommand.CanExecute(null);
            }
            else
            {
                RefreshAllowed = true;
            }
        }
        
        void OnCommandCanExecuteChanged(object sender, EventArgs eventArgs)
        
        {
            RefreshAllowed = RefreshCommand.CanExecute(null);
        }
        
        void OnRefreshing(EventArgs e)
        
        {
            EventHandler handler = Refreshing;
            if (handler != null)
                handler(this, e);
        }
        
        public void EndRefresh()
        
        {
            SetValue(IsRefreshingProperty, false);
        }
        //
        // Summary:
        //     Gets or sets a value that tells whether the list view is currently refreshing.
        //
        // Remarks:
        //     To be added.
        public static readonly BindableProperty
            IsRefreshingProperty = BindableProperty.Create("IsRefreshing", typeof(bool), typeof(NiftyDataStack), false, BindingMode.TwoWay);
        //
        public bool IsRefreshing
        {
            get { return (bool)GetValue(IsRefreshingProperty); }
            set { SetValue(IsRefreshingProperty, value); }
        }
        
        //
        // Summary:
        //     Occurs when the visual representation of an item is being added to the visual
        //     layout.
        //
        // Remarks:
        //     This method is guaranteed to fire at some point before the element is on screen.
        public event EventHandler<ItemVisibilityEventArgs> ItemAppearing;
        public event EventHandler<ItemVisibilityEventArgs> ItemDisappearing;
        public event EventHandler<ItemVisibilityEventArgs> CellAppearing;
        public event EventHandler<ItemVisibilityEventArgs> CellDisappearing;

        
        //public void SendCellAppearing(SliderCell cell)
        
        //{
        //    EventHandler<ItemVisibilityEventArgs> handler = ItemAppearing;
        //    if (handler != null)
        //        handler(this, new ItemVisibilityEventArgs(cell.BindingContext,0));
        //    EventHandler<ItemVisibilityEventArgs> handler2 = CellAppearing;
        //    if (handler2 != null)
        //        handler2(this, new ItemVisibilityEventArgs(cell, 0));
        //}
        
        //public void SendCellDisappearing(SliderCell cell)
        
        //{
        //    EventHandler<ItemVisibilityEventArgs> handler = ItemDisappearing;
        //    if (handler != null)
        //        handler(this, new ItemVisibilityEventArgs(cell.BindingContext, 0));
        //    EventHandler<ItemVisibilityEventArgs> handler2 = CellDisappearing;
        //    if (handler2 != null)
        //        handler2(this, new ItemVisibilityEventArgs(cell,0));
        //}

        
        public void SendCellAppearing(NiftyCell cell)
        
        {
            EventHandler<ItemVisibilityEventArgs> handler = ItemAppearing;
            if (handler != null)
                handler(this, new ItemVisibilityEventArgs(cell.BindingContext,0));
            EventHandler<ItemVisibilityEventArgs> handler2 = CellAppearing;
            if (handler2 != null)
                handler2(this, new ItemVisibilityEventArgs(cell,0));
        }
        
        public void SendCellDisappearing(NiftyCell cell)
        
        {
            EventHandler<ItemVisibilityEventArgs> handler = ItemDisappearing;
            if (handler != null)
                handler(this, new ItemVisibilityEventArgs(cell.BindingContext, 0));
            EventHandler<ItemVisibilityEventArgs> handler2 = CellDisappearing;
            if (handler2 != null)
                handler2(this, new ItemVisibilityEventArgs(cell,0));
        }
        
        // Tapped
        
        public new event EventHandler Tapped = null;
        //private async void OnTapped(object sender, EventArgs e)
        //{
        //  Tapped?.Invoke(this, EventArgs.Empty);
        //}


        //This is for accessing the passed listview item object        
        public class MySelEventArgs : EventArgs
        {
            public string Tag { get; set; }
            public string Params { get; set; }
        }





    }
}


