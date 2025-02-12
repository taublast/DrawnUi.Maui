using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AppoMobi.Specials;
using AppoMobi.Touch;

namespace AppoMobi.Xam
{
    public class NiftyCell : LegacyGesturesContentView
    {
        public string Tag { get; set; }

        public EventHandler RendererCommand { get; set; }

        public NiftyCell()
        {
            UID = Guid.NewGuid().ToString();
            IsClippedToBounds = true;
        }

        public bool ShouldRender
        {
            get
            {
                if (LockRendering)
                {
	                return false;
                }

                return ShouldRenderCheck();
            }
        }

        /// <summary>
        /// For internal use by DataStack
        /// </summary>
        public bool LockRendering { get; set; }

        public virtual bool ShouldRenderCheck()
        {
            return BindingContext != null;
        }

        /// <summary>
        /// Set this for groupping by first letter of alphabet
        /// </summary>
        public string Title { get; set; }

        public int Index { get; set; }

        public string UID { get; set; }


        public virtual void Dispose()
        {
            if (BindingContext != null)
            {
                INotifyPropertyChanged ctx = null;
                try
                {
                    ctx = (INotifyPropertyChanged)BindingContext;
                    ctx.PropertyChanged -= OnBindingContextPropertyChanged;
                }
                catch
                {
                }
            }
        }

        public virtual void OnCommandReceived(string command, object param)
        {

        }


        public async Task CommandFromParent(string command, object param)
        {
            OnCommandReceived(command, param);
        }

        public virtual void OnRendered()
        {
            //var container = RealParent as NiftyDataStack;
            // if (container != null)
            //     container.SendCellAppearing(this);
        }


        public virtual void OnAppeared()

        {
            var container = RealParent as NiftyDataStack;
            // if (container != null)
            //     container.SendCellAppearing(this);
        }

        public virtual void OnDisapeared()

        {
            var container = RealParent as NiftyDataStack;
            // if (container != null)
            //     container.SendCellDisappearing(this);
        }


        public virtual void OnParentSizeChangeding(double basewidth)

        {

        }

        public double ParentWidth { get; set; }

        public double AmountAppeared { get; set; }
        bool _Appeared;
        public bool Appeared
        {
            get { return _Appeared; }
            set
            {
                if (_Appeared != value)
                {
                    _Appeared = value;
                    OnPropertyChanging();
                    if (value)
                    {
	                    OnAppeared();
                    }
                    else
                    {
	                    OnDisapeared();
                    }

                    OnPropertyChanged();
                }
            }
        }

        INotifyPropertyChanged _oldBindingContext;

        protected override void OnBindingContextChanged()

        {
            if (_oldBindingContext != null)
            {
                _oldBindingContext.PropertyChanged -= OnBindingContextPropertyChanged;
            }

            if (BindingContext != null)
            {
                INotifyPropertyChanged ctx = null;
                try
                {
                    ctx = (INotifyPropertyChanged)BindingContext;
                }
                catch
                {
                    //Debug.WriteLine("NIFTY CELL: BindingContext is not INotifyPropertyChanged");
                    base.OnBindingContextChanged();
                    return;
                }
                ctx.PropertyChanged += OnBindingContextPropertyChanged;
            }
            _oldBindingContext = (INotifyPropertyChanged)BindingContext;

            OnCellBindingContextChanged();

            base.OnBindingContextChanged();
        }

        public virtual void OnCellBindingContextChanged()

        {



        }

        void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e)

        {
            OnCellBindingContextPropertyChanged(e?.PropertyName);
        }

        public virtual void OnCellBindingContextPropertyChanged(string propertyName)

        {

        }


        public bool InitGesturesForCell(dynamic view)

        {
            if (view is AppoMobi.Touch.LegacyGesturesGrid)
            {
                var sender = (AppoMobi.Touch.LegacyGesturesGrid)view;
                sender.Down += OnDown;
                sender.Up += OnUp;
                sender.Panned += OnPanned;
                SelectionBox = view;
            }
            else if (view is AppoMobi.Touch.LegacyGesturesStackLayout)
            {
                var sender = (AppoMobi.Touch.LegacyGesturesStackLayout)view;
                sender.Down += OnDown;
                sender.Up += OnUp;
                sender.Panned += OnPanned;
                SelectionBox = view;
            }
            else
            {
	            return false;
            }

            return true;
        }

        public event EventHandler<DownUpEventArgs> CellOnDown;
        public event EventHandler<DownUpEventArgs> CellOnUp;

        public event EventHandler<PanEventArgs> CellOnPanned;
        dynamic SelectionBox { get; set; } = null;


        protected void OnDown(object sender, DownUpEventArgs e)

        {
            foreach (var child in Children)
            {
                var name = Reflection.GetPropertyValueFor(child, "Name");
            }
            CellOnDown?.Invoke(SelectionBox, e);
        }


        protected void OnUp(object sender, DownUpEventArgs e)

        {
            CellOnUp?.Invoke(SelectionBox, e);
        }

        protected void OnPanned(object sender, PanEventArgs e)

        {
            CellOnPanned?.Invoke(SelectionBox, e);
        }

    }
}
