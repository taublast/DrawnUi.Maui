using System.Windows.Input;
using AppoMobi.Specials;

namespace Sandbox
{
    public class CustomTabsSelector : SkiaLayout
    {
        /// <summary>
        /// This is called when processing stack of index changes.
        /// For example, you might have index changed 5 times
        /// during the time you were executing ApplySelectedIndex (playing the animations etc),
        /// so then you just need the latest index to be applied.
        /// At the same time ApplySelectedIndex will not be called again while its already running,
        /// this way you would visually apply only the latest most actual value instead of
        /// maybe freezing the UI for too many heavy to be rendered changes.
        /// </summary>
        public virtual async Task ApplySelectedIndex(int index)
        {
            await ApplySelectedIndex(false, index);
        }

        public async void ApplySelectedIndexInternal()
        {
            if (_isBusyApplyingIndex)
            {
                _needReApplyiIndex = true;
                return;
            }

            try
            {
                await ApplySelectedIndex(SelectedIndex);
            }
            catch (Exception e)
            {
                Super.Log(e);
            }
            finally
            {
                if (_needReApplyiIndex && !_isBusyApplyingIndex)
                {
                    _needReApplyiIndex = false;
                    Tasks.StartDelayed(TimeSpan.FromMicroseconds(1), async () =>
                    {
                        await Task.Run(ApplySelectedIndexInternal);
                    });
                }
            }
        }


        private bool _isBusyApplyingIndex;
        private bool _needReApplyiIndex;


        #region PROPERTIES

 
        private static void OnNeedRebuild(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is CustomTabsSelector control)
            {
                //control.Rebuild();
            }
        }
 

        public static readonly BindableProperty CommandTabReselectedProperty = BindableProperty.Create(nameof(CommandTabReselected), typeof(ICommand),
            typeof(CustomTabsSelector), null);

        public ICommand CommandTabReselected
        {
            get { return (ICommand)GetValue(CommandTabReselectedProperty); }
            set { SetValue(CommandTabReselectedProperty, value); }
        }

        public static readonly BindableProperty CommandTabSelectedProperty = BindableProperty.Create(nameof(CommandTabSelected), typeof(ICommand),
            typeof(CustomTabsSelector), null);

        public ICommand CommandTabSelected
        {
            get { return (ICommand)GetValue(CommandTabSelectedProperty); }
            set { SetValue(CommandTabSelectedProperty, value); }
        }

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
            nameof(SelectedIndex),
            typeof(int),
            typeof(CustomTabsSelector),
            defaultValue: -1,
            propertyChanged: SelectedIndexPropertyChanged);

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        private static void SelectedIndexPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabHostView = (CustomTabsSelector)bindable;
            int selectedIndex = (int)newvalue;
            if (selectedIndex < 0)
            {
                return;
            }
            tabHostView.LastSelectedIndex = (int)oldvalue;
            tabHostView.ApplySelectedIndexInternal();
        }

        #endregion

        //public void Rebuild()
        //{
        //    Tabs.Clear();
        //    foreach (var child in Views)
        //    {
        //        AddTab(child);
        //    }
        //}

        //private void AddTab(SkiaControl child)
        //{
        //    if (child.GetType() == TabType)
        //    {
        //        Tabs.Add(new TabEntry { VIew = child });
        //        ApplySelectedIndex(true, SelectedIndex);
        //    }
        //}

        //private void RemoveTab(SkiaControl child)
        //{
        //    if (child.GetType() == TabType)
        //    {
        //        Tabs.RemoveAll(x => x.VIew == child);
        //        if (!IsDisposed)
        //            ApplySelectedIndex(true, SelectedIndex);
        //    }
        //}

        public override void OnDisposing()
        {
            base.OnDisposing();

            IndicatorsUnselected?.Clear();
            IndicatorsSelected?.Clear();
        }

        //protected override void OnChildAdded(SkiaControl child)
        //{
        //    base.OnChildAdded(child);

        //    AddTab(child as SkiaControl);
        //}

        //protected override void OnChildRemoved(SkiaControl child)
        //{
        //    base.OnChildRemoved(child);

        //    RemoveTab(child);
        //}

        public List<SkiaControl> IndicatorsUnselected { get; set; } = new();

        public List<SkiaControl> IndicatorsSelected { get; set; } = new();

        private bool _trackingReselection;
        int _lastTapIndex;


        public int LastSelectedIndex
        {
            get
            {
                return _lastSelectedIndex;
            }

            set
            {
                if (_lastSelectedIndex != value)
                {
                    _lastSelectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }
        int _lastSelectedIndex = -1;

        public ICommand CommandTappedTab
        {
            get
            {
                return new Command(async (context) =>
                {
                    var index = $"{context}".ToInteger();
                    _trackingReselection = true;

                    if (SelectedIndex == index)
                    {
                        OnTabReselected();
                    }
                    else
                        SelectedIndex = index;
                });
            }
        }


        public virtual async Task ApplySelectedIndex(bool tabsChanged, int selectedIndex)
        {

            //clamp
            if (IndicatorsUnselected.Count == 0)
            {
                selectedIndex = 0;
            }
            if (selectedIndex > IndicatorsUnselected.Count)
            {
                selectedIndex = IndicatorsUnselected.Count - 1;
            }

            //set tabs views visual selection state
            //for (int index = 0; index < Tabs.Count; index++)
            //{
            //    Tabs[index].IsSelected = selectedIndex == index;
            //}

            //selected and reselected events
            if (_lastTapIndex != selectedIndex || selectedIndex == -1)
            {
                _lastTapIndex = selectedIndex;
                await OnTabSelectionChanged(tabsChanged, selectedIndex);

                //if (SelectedIndex >= 0 && !tabsChanged)
                //{
                //    for (int i = 0; i < Tabs.Count; i++)
                //    {
                //        var check = Tabs[i].VIew.Opacity;
                //    }
                //}
            }
            else
            {
                if (_trackingReselection)
                {
                    OnTabReselected();
                }
            }
        }

        public virtual async Task OnTabSelectionChanged(bool tabsChanged, int index)
        {
            CommandTabSelected?.Execute(index);
        }

        public virtual void OnTabReselected()
        {
            CommandTabReselected?.Execute(SelectedIndex);
        }
    }
}
