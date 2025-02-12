using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace AppoMobi.Xam
{

    public class NiftyDataGrid : AppoMobi.Touch.GesturesGrid

    {
	    int SelectedRow = -1;
	    AppoMobi.Touch.LegacyGesturesBoxView SelectionBox = null;

        public bool IsPressed { get; set; } = false;
        public bool IsPanned { get; set; } = false;

        //
        //public void ShowLoader(bool show = true)
        //
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
        //                    var spinner = new Spinner();

        //                    Children.Add(spinner);
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}

        /// <summary>
        /// Raised when children are drawn
        /// </summary>
        public event EventHandler FinishedDrawing = null;


        public void Dispose()

        {
            if (Children == null) return;
            //todo call all nifty cells dispose
            foreach (var child in Children)
            {
                try
                {
                    var disposable = (NiftyCell)child;
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                }
            }
        }

        public NiftyDataGrid()
        {
            Views = Children;
        }


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Update the UI
                switch (propertyName)
                {
                case nameItemsSource:
                ItemsSourceChanged();
                break;

                //property changed
                case nameStartColumn:
                ItemsSourceChanged();
                break;

                //property changed
                case nameColumns:
                if (Columns < 1)
                    Columns = 1;

                BuildGrid();

                break;
                }

            });


        }

        //-------------------------------------------------------------
        // ItemsLoaded
        //-------------------------------------------------------------
        const string nameItemsLoaded = "ItemsLoaded";
        public static readonly BindableProperty ItemsLoadedProperty = BindableProperty.Create(nameItemsLoaded, typeof(bool), typeof(NiftyDataGrid), false); //, BindingMode.TwoWay
        public bool ItemsLoaded
        {
            get { return (bool)GetValue(ItemsLoadedProperty); }
            private set { SetValue(ItemsLoadedProperty, value); }
        }


        // Tag
        const string nameTag = "Tag";
        public static readonly BindableProperty TagProperty = BindableProperty.Create(nameTag, typeof(string), typeof(NiftyDataGrid), string.Empty);
        public string Tag
        {
            get { return (string)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }



        // SelectedParams
        const string nameSelectedParams = "SelectedParams";
        public static readonly BindableProperty SelectedParamsProperty = BindableProperty.Create(nameSelectedParams, typeof(string), typeof(NiftyDataGrid), string.Empty);
        public string SelectedParams
        {
            get { return (string)GetValue(SelectedParamsProperty); }
            set { SetValue(SelectedParamsProperty, value); }
        }



        public virtual void OnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs args)

        {

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Update the UI


                int row = 0;
                int col = 0;
                int count = 1;
                int startAt;
                int cells = Columns;//number of cells per row

                BuildGrid();

                /*
                            switch (args.Action)
                                {
                                    //""""""""""""""""""""""""""""""""""""""""
                                    case NotifyCollectionChangedAction.Add:
                                    //""""""""""""""""""""""""""""""""""""""""
                                    {

                                        int rows = -1;
                                        int index = args.NewStartingIndex;
                                            if (args.NewItems != null)
                                            {
                                                foreach (var newItem in args.NewItems)
                                                {

                                                    startAt = index+1;//cell number to start at // todo check android where +1 wasnt needed!!!
                                                    col = (startAt - 1) % cells;
                                                    row = (startAt - 1) / cells;
                                                    if (row > rows)
                                                        rows = row;

                                                var view = (View)ItemTemplate.CreateContent();
                                                    var bindableObject = view as BindableObject;
                                                    if (bindableObject != null)
                                                        bindableObject.BindingContext = newItem;
                                                    Children.Add(view, col, row);
                                                    index++;
                                                }

                                            }
                                        if (rows > -1)
                                            Rows = rows + 1;
                                    }
                                        break;
                                    //""""""""""""""""""""""""""""""""""""""""
                                    case NotifyCollectionChangedAction.Move:
                                    //""""""""""""""""""""""""""""""""""""""""
                                    {
                                        var view = (View)Children[args.OldStartingIndex];
                                            Children.RemoveAt(args.OldStartingIndex);
                                            Children.Insert(args.NewStartingIndex, view);
                                     }
                                     break;
                                    //""""""""""""""""""""""""""""""""""""""""
                                    case NotifyCollectionChangedAction.Remove:
                                    //""""""""""""""""""""""""""""""""""""""""
                                    {
                                        Children.RemoveAt(args.OldStartingIndex);
                                        }
                                        break;
                                    //""""""""""""""""""""""""""""""""""""""""
                                    case NotifyCollectionChangedAction.Replace:
                                    //""""""""""""""""""""""""""""""""""""""""
                                    {
                                        var view = (View)ItemTemplate.CreateContent();
                                            var bindableObject = view as BindableObject;
                                            if (bindableObject != null)
                                                bindableObject.BindingContext = args.NewItems[0];
                                            Children.RemoveAt(args.OldStartingIndex);
                                            Children.Insert(args.NewStartingIndex, view);
                                        }
                                        break;
                                    //""""""""""""""""""""""""""""""""""""""""
                                    case NotifyCollectionChangedAction.Reset:
                                    //""""""""""""""""""""""""""""""""""""""""
                                    Children.Clear();
                                        Rows = 0;
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

                    */


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

            });

        }


        public virtual void BuildGrid()

        {
            Rows = 0;

            Children.Clear();
            if (ItemsSource == null || ItemsSource.Count==0)
            {
                return;
            }

            int row = 0;
            int col = 0;
            int count = 1;
            int startAt;
            int cells = Columns;//number of cells per row

            var addsome = 0;
            if (StartColumn > 0 && StartColumn < cells)
            {
                addsome = StartColumn;
            }

            int rows = (ItemsSource.Count + addsome) / cells;
            Rows = rows;



            RowDefinitions.Clear();
            for (int a = 0; a < Rows; a++)
            {
                RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            ColumnDefinitions.Clear();
            double myWidth = 100 / (double)Columns;
            for (int a = 0; a < Columns; a++)
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(myWidth, GridUnitType.Star) });
            }

            var shit = new Microsoft.Maui.Controls.BoxView();
            shit.InputTransparent = true;
            shit.HorizontalOptions = LayoutOptions.FillAndExpand;
            shit.VerticalOptions = LayoutOptions.Fill;
            shit.HeightRequest = 10;
            shit.BackgroundColor = Colors.Transparent;

            //new
            //add some invisible shit to start if needed 
            //to start from cell other than 0
            if (addsome > 0)
            {
                while (col < addsome)
                {
                    col++;
                    this.Add(shit, col, row);
                }
            }

            count += addsome;
            foreach (var newItem in ItemsSource)
            {
                startAt = count;//cell number to start at
                row = (startAt - 1) / cells;
                //if (row > 0)
                //    addedshit = 0;
                if (row > rows)
                    rows = row;
                col = (startAt - 1) % cells;

                //CREATE ITEM <---------------------------------------------------------- + item
                var view = (View)ItemTemplate.CreateContent();
                if (view is NiftyCell)
                {
                    ((NiftyCell)view).ParentWidth = Width;
                }
                var bindableObject = view as BindableObject;
                if (bindableObject != null)
                    bindableObject.BindingContext = newItem; // send data to item 
                this.Add(view, col, row);

                Task.Delay(1);
                count++;
            }
            //add some invisible shit to empty cells in last row
            //needed to remove bug seen on iOS
            if (col < Columns - 1)
            {
                var addshit = 0;
                while (col < Columns - 1)
                {
                    col++;
                    this.Add(shit, col, row);
                }
            }


            if (rows > -1)
                Rows = rows;

            ItemsLoaded = true;
            FinishedDrawing?.Invoke(this, new EventArgs());
        }

        public IList<IView> Views { get; private set; } = null;
        INotifyCollectionChanged oldSource { get; set; }

        void ItemsSourceChanged()
        {
            ItemsLoaded = false;

            BuildGrid();

            //new fix
            if (oldSource != null)
                oldSource.CollectionChanged -= OnCollectionChanged;
            oldSource = ItemsSource as INotifyCollectionChanged;

            var notifyCollection = ItemsSource as INotifyCollectionChanged;
            if (notifyCollection != null)
            {
                notifyCollection.CollectionChanged += OnCollectionChanged;
            }
        }



        //-------------------------------------------------------------
        // StartColumn
        //-------------------------------------------------------------
        const string nameStartColumn = "StartColumn";
        public static readonly BindableProperty StartColumnProperty = BindableProperty.Create(nameStartColumn, typeof(int), typeof(NiftyDataGrid), 0); //, BindingMode.TwoWay
        public int StartColumn
        {
            get { return (int)GetValue(StartColumnProperty); }
            set { SetValue(StartColumnProperty, value); }
        }



        // Columns
        const string nameColumns = "Columns";
        public static readonly BindableProperty ColumnsProperty = BindableProperty.Create(nameColumns, typeof(int), typeof(NiftyDataGrid), 1); //, BindingMode.TwoWay
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }


        // Rows
        const string nameRows = "Rows";
        public static readonly BindableProperty RowsProperty = BindableProperty.Create(nameRows, typeof(int), typeof(NiftyDataGrid), 0); //, BindingMode.TwoWay
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            private set { SetValue(RowsProperty, value); }
        }




        public DataTemplate ItemTemplate

        {
            get;
            set;
        }



        // ItemsSource
        const string nameItemsSource = "ItemsSource";
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameItemsSource, typeof(IList), typeof(NiftyDataGrid), null, BindingMode.TwoWay);
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }



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
                typeof(NiftyDataGrid), null, propertyChanged: OnRefreshCommandChanged);
        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        static void OnRefreshCommandChanged(BindableObject bindable, object oldValue, object newValue)

        {
            var lv = (NiftyDataGrid)bindable;
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
            IsRefreshingProperty = BindableProperty.Create("IsRefreshing", typeof(bool), typeof(NiftyDataGrid), false, BindingMode.TwoWay);
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


        public void SendCellAppearing(Cell cell)

        {
            EventHandler<ItemVisibilityEventArgs> handler = ItemAppearing;
            if (handler != null)
                handler(this, new ItemVisibilityEventArgs(cell.BindingContext, 0));
        }






    }
}