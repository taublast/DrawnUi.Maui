// Deeply inspired by the Sharpnado ViewSwitcher https://github.com/roubachof/Sharpnado.Tabs

using DrawnUi.Maui.Infrastructure.Enums;
using Microsoft.Maui.Controls.Shapes;
using System.Diagnostics;

namespace DrawnUi.Maui.Controls
{
    /// <summary>
    /// Display and hide views, eventually animating them
    /// </summary>
    public class SkiaViewSwitcher : SkiaLayout, IDefinesViewport, IVisibilityAware
    {
        private NavigationStackEntry _ActiveView;
        public NavigationStackEntry ActiveView
        {
            get { return _ActiveView; }
            set
            {
                if (_ActiveView != value)
                {
                    _ActiveView = value;
                    OnPropertyChanged();
                }
            }
        }

        public SkiaViewSwitcher()
        {
            RowSpacing = 0;
            ColumnSpacing = 0;
        }

        public void SendOnAppearing(SkiaControl view)
        {
            if (view is IVisibilityAware aware)
            {
                aware.OnAppearing();
            }
        }


        public void SendOnAppeared(SkiaControl view)
        {
            if (view is IVisibilityAware aware)
            {
                aware.OnAppeared();
            }
        }

        public void SendOnDisappeared(SkiaControl view)
        {
            if (view is IVisibilityAware aware)
            {
                aware.OnDisappeared();
            }
        }

        public void SendOnLoaded(SkiaControl view)
        {
            if (view is IInsideViewport aware)
            {
                aware.OnLoaded();
            }
        }

        protected override void OnLayoutChanged()
        {
            base.OnLayoutChanged();

            foreach (var view in Views)
            {
                if (view is IInsideViewport viewport)
                {
                    viewport.OnViewportWasChanged(ScaledRect.FromPixels(DrawingRect, RenderingScale));
                }
            }
        }

        protected override void Draw(SkiaDrawingContext context, SKRect destination, float scale)
        {
            IsRendered = true;

            base.Draw(context, destination, scale);
        }

        public bool IsRendered;

        private bool ChildrenInvalidated;

        public static readonly BindableProperty IsAnimatingProperty = BindableProperty.Create(nameof(IsAnimating),
            typeof(bool),
            typeof(SkiaViewSwitcher),
            false, BindingMode.OneWayToSource);
        public bool IsAnimating
        {
            get { return (bool)GetValue(IsAnimatingProperty); }
            set { SetValue(IsAnimatingProperty, value); }
        }

        public static readonly BindableProperty ClipWithProperty = BindableProperty.Create(nameof(ClipWith), typeof(Geometry), typeof(SkiaViewSwitcher), null);
        public Geometry ClipWith
        {
            get { return (Geometry)GetValue(ClipWithProperty); }
            set { SetValue(ClipWithProperty, value); }
        }

        #region Tabs

        protected override void OnChildAdded(SkiaControl view)
        {
            if (view is IInsideViewport viewport)
            {
                viewport.OnViewportWasChanged(ScaledRect.FromPixels(DrawingRect, RenderingScale));
            }

            base.OnChildAdded(view);

            var previous = Views.Count - 1;
            if (previous >= 0)
                HideView(view as SkiaControl, Views.Count - 1, false);
        }


        public override void SetChildren(IEnumerable<ISkiaAttachable> views)
        {
            base.SetChildren(views);

            UpdateSelectedView(SelectedIndex, false);
        }




        #endregion

        public List<NavigationStackEntry> GetNavigationStack(int index)
        {
            var stack = new List<NavigationStackEntry>();
            try
            {
                if (NavigationStacks.TryGetValue(index, out var getStack))
                {
                    return getStack;
                }
                NavigationStacks[index] = stack;
                return stack;
            }
            catch (Exception e)
            {
                //Trace.WriteLine(e);
                NavigationStacks[index] = stack;
                return stack;
            }
        }

        /// <summary>
        /// Get tab view or tab top subview if has navigation stack
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns></returns>
        public NavigationStackEntry GetTopView(int selectedIndex)
        {
            try
            {
                var stack = GetNavigationStack(selectedIndex);
                var view = stack.LastOrDefault();
                if (view != null)
                    return stack.Last();

                return new NavigationStackEntry(Views[selectedIndex] as SkiaControl, false, false);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }

            return null;

        }

        public NavigationStackEntry GetRootView(int selectedIndex)
        {
            try
            {
                return new NavigationStackEntry(Views[selectedIndex] as SkiaControl, false, false);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }

            return null;
        }

        public async void PushView(ISkiaAttachable page, bool animated, bool preserve = false, int tab = -1)
        {
            if (NavigationBusy)
                return;

            NavigationBusy = true;

            try
            {
                var view = page.AttachControl;

                if (view == null)
                {
                    throw new Exception("AttachControl is null");
                }

                if (tab >= 0)
                {
                    SelectedIndex = tab;
                }

                try
                {
                    var index = tab;
                    if (index < 0)
                        index = SelectedIndex;

                    var stack = GetNavigationStack(index);
                    stack.Add(new NavigationStackEntry(view, animated, preserve));

                    AddSubView(view);

                    IsPushing = true; //will be set to false after transition completes

                    UpdateSelectedView(index, animated);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            finally
            {
                NavigationBusy = false;
            }
        }


        public SkiaControl GetCurrentPage(int tab = -1)
        {
            List<NavigationStackEntry> stack;
            try
            {
                if (tab < 0)
                    tab = SelectedIndex;

                stack = GetNavigationStack(tab);
                var subView = stack.LastOrDefault();
                if (subView != null)
                {
                    return subView.View;
                }
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                //NavigationStacks[SelectedIndex] = stack;
            }
            return null;
        }

        public T FindPageInStack<T>(int tab = -1) where T : SkiaControl
        {
            List<NavigationStackEntry> stack;
            try
            {
                if (tab < 0)
                    tab = SelectedIndex;

                stack = GetNavigationStack(tab);
                if (stack.FirstOrDefault(x => x.View.GetType() == typeof(T)) is T subView)
                {
                    return subView;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                //NavigationStacks[SelectedIndex] = stack;
            }
            return default(T);
        }

        public record NavigationStackEntry(SkiaControl View, bool Animated, bool Preserve);

        public void RemovePageFromStack<T>(int tab = -1) where T : SkiaControl
        {
            List<NavigationStackEntry> stack;
            try
            {
                if (tab < 0)
                    tab = SelectedIndex;

                stack = GetNavigationStack(tab);
                var foundEntry = stack.FirstOrDefault(x => x.View is T);

                if (foundEntry != null)
                {
                    stack.Remove(foundEntry);
                    UpdateSelectedView(tab, AnimatePages);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public bool BringPageToFront(Type type, int tab = -1)
        {
            List<NavigationStackEntry> stack;
            try
            {
                if (tab < 0)
                    tab = SelectedIndex;

                stack = GetNavigationStack(tab);
                var subView = stack.FirstOrDefault(x => x.GetType() == type);
                if (subView != null)
                {
                    if (stack.IndexOf(subView) != stack.Count - 1)
                    {
                        stack.Remove(subView);
                        stack.Add(subView);
                        //todo make visible
                        UpdateSelectedView(tab, false);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                //NavigationStacks[SelectedIndex] = stack;
            }
            return false;
        }

        public void PopModal(bool animated)
        {

        }

        public async Task PopPage(int tab = -1)
        {
            //animation off

            await SemaphoreNavigationStack.WaitAsync();

            if (tab < 0)
                tab = SelectedIndex;

            List<NavigationStackEntry> stack;
            try
            {
                stack = GetNavigationStack(tab);

                var subView = stack.Last();

                //todo send disappearing

                stack.Remove(subView);

                NavigationStacks[tab] = stack;

                //show view under

                IsPopping = true;

                UpdateSelectedView(tab, AnimatePages);
            }
            catch (Exception e)
            {
                stack = new List<NavigationStackEntry>();
                NavigationStacks[tab] = stack;
            }
            finally
            {
                SemaphoreNavigationStack.Release();
            }

        }

        public int GetCurrentTabNavigationIndex()
        {
            var index = 0;
            List<NavigationStackEntry> stack;
            try
            {
                stack = GetNavigationStack(SelectedIndex);
                index = stack.Count;
            }
            catch (Exception e)
            {
                stack = new List<NavigationStackEntry>();
                NavigationStacks[SelectedIndex] = stack;
            }

            return index;
        }

        /// <summary>
        /// Set IsVisible, reset transforms and opacity and send OnAppeared event
        /// </summary>
        /// <param name="newVisibleView"></param>
        protected virtual void RevealNavigationView(NavigationStackEntry newVisibleView)
        {
            if (newVisibleView != null)
            {
                newVisibleView.View.TranslationX = 0;
                newVisibleView.View.TranslationY = 0;
                newVisibleView.View.Opacity = 1.0;
                ChangeViewVisibility(newVisibleView.View, true);
                SendOnAppeared(newVisibleView.View);
            }
        }

        /// <summary>
        /// Must be launched on main thread only !!!
        /// </summary>
        public async Task PopTabToRoot()
        {

            while (_isApplyingIdex)
            {
                await Task.Delay(10);
            }

            await SemaphoreNavigationStack.WaitAsync();

            List<NavigationStackEntry> stack;
            try
            {
                //show root view, hopefully behind the stack :)
                RevealNavigationView(GetRootView(SelectedIndex));

                stack = GetNavigationStack(SelectedIndex);
                foreach (var subView in stack.ToList())
                {
                    //todo send disappearing
                    SendOnDisappearing(subView.View);
                    RemoveSubView(subView.View);
                    if (!subView.Preserve)
                        subView.View.Dispose();
                }

                stack.Clear();
                NavigationStacks[SelectedIndex] = stack;
            }
            catch (Exception e)
            {
                stack = new List<NavigationStackEntry>();
                NavigationStacks[SelectedIndex] = stack;
            }
            finally
            {
                SemaphoreNavigationStack.Release();
            }

        }

        public void PopAllTabsToRoot()
        {
            //todo
            PopTabToRoot();
        }

        /// <summary>
        /// for navigation inside pseudo tabs
        /// </summary>
        public Dictionary<int, List<NavigationStackEntry>> NavigationStacks = new();

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
            nameof(SelectedIndex),
            typeof(int),
            typeof(SkiaViewSwitcher),
            defaultValue: -1,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: SelectedIndexPropertyChanged);

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public static readonly BindableProperty DisplayingIndexProperty = BindableProperty.Create(
            nameof(DisplayingIndex),
            typeof(int),
            typeof(SkiaViewSwitcher),
            defaultValue: -1);

        public int DisplayingIndex
        {
            get => (int)GetValue(DisplayingIndexProperty);
            set => SetValue(DisplayingIndexProperty, value);
        }

        public bool AnimatePages { get; set; } = true;

        public bool AnimateTabs { get; set; } = false;

        public void Reset()
        {
            foreach (var key in NavigationStacks.Keys.ToList())
            {
                NavigationStacks.Remove(key);
            }

            ClearChildren();

            previousVisibleViewIndex = -1;
            PreviousVisibleView = null;
        }


        public override void OnAppearing()
        {
            base.OnAppearing();

            if (TopView is IVisibilityAware aware)
            {
                aware.OnAppearing();
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            if (TopView is IVisibilityAware aware)
            {
                aware.OnDisappearing();
            }
        }

        public override void OnAppeared()
        {
            base.OnAppeared();

            if (TopView is IVisibilityAware aware)
            {
                aware.OnAppeared();
            }
        }


        private static void SelectedIndexPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {

            if (bindable is SkiaViewSwitcher viewSwitcher)
            {
                viewSwitcher.UpdateSelectedView((int)newvalue, null);
            }

        }

        public bool IsPushing { get; protected set; }
        public bool IsPopping { get; protected set; }

        //-------------------------------------------------------------
        // NavigationBusy
        //-------------------------------------------------------------
        private const string nameNavigationBusy = "NavigationBusy";
        public static readonly BindableProperty NavigationBusyProperty = BindableProperty.Create(nameNavigationBusy,
            typeof(bool), typeof(SkiaViewSwitcher), false, BindingMode.OneWayToSource);
        public bool NavigationBusy
        {
            get { return (bool)GetValue(NavigationBusyProperty); }
            set { SetValue(NavigationBusyProperty, value); }
        }

        public static readonly BindableProperty AnimationSpeedProperty = BindableProperty.Create(nameof(AnimationSpeed),
            typeof(int), typeof(SkiaViewSwitcher), 250);
        public int AnimationSpeed
        {
            get { return (int)GetValue(AnimationSpeedProperty); }
            set { SetValue(AnimationSpeedProperty, value); }
        }

        public static readonly BindableProperty AnimationEasingProperty = BindableProperty.Create(nameof(AnimationEasing),
            typeof(Easing), typeof(SkiaViewSwitcher), Custom);
        public Easing AnimationEasing
        {
            get { return (Easing)GetValue(AnimationEasingProperty); }
            set { SetValue(AnimationEasingProperty, value); }
        }

        public static readonly Easing Custom = new Easing((x) => (x - 1) * (x - 1) * ((_sideCoeff + 1) * (x - 1) + _sideCoeff) + 1);

        int previousVisibleViewIndex = -1;

        public NavigationStackEntry PreviousVisibleView
        {
            get;
            set;
        }

        protected void LauchTimerOnLock(int seconds, (int index, bool animate) args)
        {
            if (TimerUpdateLocked == null)
            {
                TimerUpdateLocked = new RestartingTimer<(int, bool)>(TimeSpan.FromSeconds(seconds), (args) =>
                {
                    UpdateSelectedView(args.Item1, args.Item2);
                });
                TimerUpdateLocked.Start(args);
            }
            else
            {
                TimerUpdateLocked.Restart(args);
            }
        }

        protected RestartingTimer<(int, bool)> TimerUpdateLocked;


        public SkiaControl TopView
        {
            get;
            protected set;
        }

        protected void ChangeViewVisibility(SkiaControl view, bool state)
        {
            if (view.IsVisible != state)
            {
                if (state && view is IInsideViewport viewport)
                {
                    viewport.OnViewportWasChanged(ScaledRect.FromPixels(DrawingRect, RenderingScale));
                }

                view.IsVisible = state;
            }
        }

        private object lockIdex = new();

        private int lastSelectedIndex = -1;

        //protected CancellationTokenSource CancelLastNavigation;


        private readonly LimitedQueue<OrderedIndex> _queue = new();

        private bool _processing = false;
        public void PushIndex(OrderedIndex index)
        {
            //Debug.WriteLine($"[INDEX] Push {index.Index}");

            _queue.Push(index);

            if (!_processing)
            {
                _processing = true;
                // Tasks.StartDelayedAsync(TimeSpan.FromMicroseconds(1), async () =>
                // {
                //     await ProcessIndexBufferAsync().ConfigureAwait(false);
                // });
                // MainThread.BeginInvokeOnMainThread(async  () =>
                // {
                //     await ProcessIndexBufferAsync();
                // });
                ProcessIndexBufferAsync().ConfigureAwait(false);
            }
        }


        protected SemaphoreSlim SemaphoreNavigationStack = new(1, 1);

        public async void DebugAction()
        {
            var newVisibleView = GetTopView(1);
            var previousVisibleView = PreviousVisibleView;

            newVisibleView.View.IsVisible = true;
            newVisibleView.View.Opacity = 1;

            previousVisibleView.View.TranslationX = 150;
            newVisibleView.View.TranslationX = -150;
        }

        public async Task ProcessIndexBufferAsync()
        {
            await SemaphoreNavigationStack.WaitAsync();

            if (_isApplyingIdex)
                return;

            _isApplyingIdex = true;

            try
            {
                while (NavigationBusy)
                {
                    await Task.Delay(20);
                }

                NavigationBusy = true;

                OrderedIndex applyMe = null;
                var index = new OrderedIndex(-1, false);
                while (index != null)
                {
                    try
                    {
                        index = _queue.Pop();

                        if (index != null && index.Index >= 0)
                        {
                            applyMe = index;
                            //await Dispatcher.DispatchAsync(() => ApplySelectedIndex(index));
                            //    .WithCancellation(cancel.Token);
                        }
                    }
                    catch (Exception e)
                    {
                        _isApplyingIdex = false;
                        Trace.WriteLine(e);
                    }
                    finally
                    {
                        _isApplyingIdex = false;
                    }
                }
                if (applyMe != null)
                {
                    await ApplySelectedIndex(applyMe);
                }
                applyMe = null;
            }
            catch (Exception e)
            {
                Super.Log(e);
            }
            finally
            {
                NavigationBusy = false;
                _processing = false;
                SemaphoreNavigationStack.Release();
            }

        }

        //void FadeInFromRight(SKPaint paint, SKRect destination)
        //{
        //	var shader = SKShader.CreateLinearGradient(
        //		new SKPoint(0, 0),
        //		new SKPoint(destination.Width, 0),
        //		new SKColor[] { SKColors.White.WithAlpha((byte)(255 / this._fadeOpacity)), SKColors.White },
        //		null,
        //		SKShaderTileMode.Clamp);

        //	// Apply the shader to the paint object
        //	paint.Shader = shader;
        //}


        //void FadeInFromLeft(SKPaint paint, SKRect destination)
        //{
        //	var shader = SKShader.CreateLinearGradient(
        //		new SKPoint(0, 0),
        //		new SKPoint(destination.Width, 0),
        //		new SKColor[] { SKColors.White, SKColors.White.WithAlpha((byte)(255 / this._fadeOpacity)) },
        //		null,
        //		SKShaderTileMode.Clamp);

        //	// Apply the shader to the paint object
        //	paint.Shader = shader;
        //}

        float _fadeOpacity;

        bool firstViewAppreared;
        private bool _isApplyingIdex;
        private async Task ApplySelectedIndex(OrderedIndex index)
        {


            try
            {
                Superview.FocusedChild = null;

                var selectedIndex = index.Index;

                DisplayingIndex = selectedIndex;

                if (selectedIndex < 0)
                {
                    return;
                }

                lastSelectedIndex = selectedIndex;

                var newVisibleView = GetTopView(selectedIndex);

                if (newVisibleView == null || TopView == newVisibleView.View)
                {
                    //seems we have not created view yet
                    return;
                }



                bool shown = false;

                bool animating = false;
                var previousVisibleView = PreviousVisibleView;

                TopView = newVisibleView.View;

                void OnViewLoaded(SkiaControl loadedView)
                {
                    if (IsRendered || !firstViewAppreared)
                    {
                        firstViewAppreared = true;
                        SendOnAppearing(loadedView);
                    }
                }

                if (previousVisibleView?.View != newVisibleView?.View)
                {
                    void Reveal()
                    {
                        RevealNavigationView(newVisibleView);

                        if (previousVisibleView != null)
                        {
                            //ChangeViewVisibility(previousVisibleView.View, true);
                            ChangeViewVisibility(previousVisibleView.View, false);
                            SendOnDisappeared(previousVisibleView.View);
                        }

                    }

                    //Show view without animation. For first view etc
                    void SetNewVisibleViewAsOneVisible()
                    {
                        OnViewLoaded(newVisibleView.View);
                        ActiveView = newVisibleView;
                        Reveal();
                    }

                    bool needAnimate = index.Animate.GetValueOrDefault();
                    if (IsPushing || IsPopping)
                    {
                        if (index.Animate == null)
                            needAnimate = AnimatePages;
                    }
                    else
                    {
                        if (index.Animate == null)
                            needAnimate = AnimateTabs;
                    }

                    //have a pair of views to animate
                    if (previousVisibleView != null
                        && previousVisibleView.View.CanDraw)
                    {

                        if (needAnimate && //base flag
                            ((IsPushing || IsPopping) // navigating
                             || (previousVisibleViewIndex != selectedIndex &&
                                 previousVisibleViewIndex > -1))) //switching tabs
                        {

                            //todo select navigating animations AND implement POP
                            animating = true;

                            try
                            {
                                //prepare animation
                                Action<SKPaint, SKRect> fromViewDraw = null;
                                Action<SKPaint, SKRect> toViewDraw = null;
                                int speed = AnimationSpeed;
                                //int addForLazy = 50;
                                Easing easing = AnimationEasing;
                                var translateTo = this.Width;

                                //todo add option;
                                var transition = TransitionType.SwitchTabsModern;

                                if (IsPopping)
                                    transition = TransitionType.Pop;
                                else if (IsPushing)
                                    transition = TransitionType.Push;

                                //prepare

                                switch (transition)
                                {
                                case TransitionType.Pop:
                                easing = Easing.Linear;
                                //from left to right
                                translateTo = this.Width;
                                break;
                                case TransitionType.Push:

                                previousVisibleView.View.ZIndex = -1;
                                newVisibleView.View.ZIndex = 0;

                                easing = Easing.Linear;
                                //from right to left
                                translateTo = this.Width;
                                newVisibleView.View.Opacity = 0.001;
                                newVisibleView.View.TranslationX = (float)translateTo;
                                break;

                                //this began bugging when a tab child has transforms,
                                //surprisingly looks like somewhere that canvas is not calling Restore
                                //when we switch from tab 3 where we have that shape with transforms
                                //to other tab at the left
                                case TransitionType.SwitchTabsModern:

                                easing = AnimationEasing;
                                speed = AnimationSpeed;

                                newVisibleView.View.ZIndex = 1;
                                previousVisibleView.View.ZIndex = 0;

                                if (selectedIndex > previousVisibleViewIndex)
                                {
                                    //f===>
                                    translateTo = this.Width;
                                    newVisibleView.View.Opacity = 0.001;
                                    newVisibleView.View.TranslationX = (float)translateTo * 0.75;
                                    //toViewDraw = FadeInFromRight;
                                }
                                else
                                {
                                    // <====
                                    translateTo = -this.Width;
                                    newVisibleView.View.Opacity = 0.001;
                                    newVisibleView.View.TranslationX = (float)translateTo * 0.75;
                                    //toViewDraw = FadeInFromLeft;
                                }



                                break;

                                //actually using this for tabs
                                case TransitionType.SwitchTabs:
                                //easing = _easing;
                                easing = AnimationEasing;
                                speed = AnimationSpeed;
                                if (selectedIndex > previousVisibleViewIndex)
                                {
                                    //from right to left
                                    translateTo = this.Width;
                                }
                                else
                                {
                                    //from left to right
                                    translateTo = -this.Width;
                                }

                                newVisibleView.View.Opacity = 1;
                                newVisibleView.View.TranslationX = (float)translateTo;
                                break;

                                default:
                                newVisibleView.View.TranslationX = 0;
                                newVisibleView.View.TranslationY = 0;
                                newVisibleView.View.Opacity = 1;
                                break;
                                }

                                ChangeViewVisibility(newVisibleView.View, true);
                                SendOnLoaded(newVisibleView.View);

                                //await Task.Delay(10); //update native ui

                                //animate
                                switch (transition)
                                {
                                case TransitionType.Pop:

                                Task animateOld1 = previousVisibleView.View.TranslateToAsync(translateTo, 0, 250, Easing.Linear);
                                Task animateOld2 = previousVisibleView.View.FadeToAsync(0.9, 250, Easing.Linear);

                                try
                                {
                                    var cancelAnimation = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                                    await Task.WhenAll(animateOld1, animateOld2).WithCancellation(cancelAnimation.Token);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e);
                                }
                                break;

                                case TransitionType.Push:
                                Task in1 = newVisibleView.View.TranslateToAsync(0, 0, 250, Easing.Linear);
                                Task in2 = newVisibleView.View.FadeToAsync(1.0, 250, Easing.Linear);
                                try
                                {
                                    var cancelAnimation = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                                    await Task.WhenAll(in1, in2).WithCancellation(cancelAnimation.Token).WithCancellation(cancelAnimation.Token);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e);
                                }
                                break;

                                case TransitionType.SwitchTabsModern:

                                Task animateOldM = previousVisibleView.View.TranslateToAsync(-translateTo, 0, (uint)speed, easing);
                                Task animateNewM = newVisibleView.View.TranslateToAsync(0, 0, (uint)speed, easing);
                                Task animateNewM1 = newVisibleView.View.FadeToAsync(1.0, (uint)speed, Easing.Linear);

                                try
                                {
                                    var cancelAnimation =
                                        new CancellationTokenSource(TimeSpan.FromSeconds(2));

                                    await Task.WhenAll(animateOldM, animateNewM, animateNewM1).WithCancellation(cancelAnimation.Token);

                                    //var cancelAnimation = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                                    //await newVisibleView.View.AnimateAsync((value) =>
                                    //{

                                    //	_fadeOpacity = (float)value;
                                    //	newVisibleView.View.Repaint();

                                    //}, (uint)1000, easing, cancelAnimation);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e);
                                }
                                break;

                                case TransitionType.SwitchTabs:

                                Task animateOld = previousVisibleView.View.TranslateToAsync(-translateTo, 0, (uint)speed, easing);
                                Task animateNew = newVisibleView.View.TranslateToAsync(0, 0, (uint)speed, easing);

                                try
                                {
                                    var cancelAnimation =
                                        new CancellationTokenSource(TimeSpan.FromSeconds(2));
                                    //PrintDebug();
                                    await Task.WhenAll(animateOld, animateNew).WithCancellation(cancelAnimation.Token);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e);
                                }
                                break;
                                }


                                SetNewVisibleViewAsOneVisible();

                                if (IsPopping)
                                {
                                    RemoveSubView(previousVisibleView.View);
                                    PreviousVisibleView = null;
                                }

                                UnloadView(previousVisibleView);

                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }
                            finally
                            {
                                //
                                newVisibleView.View.TranslationX = 0;
                                newVisibleView.View.TranslationY = 0;
                                newVisibleView.View.Opacity = 1.0;
                                ChangeViewVisibility(newVisibleView.View, true);
                                ChangeViewVisibility(previousVisibleView.View, false);

                                previousVisibleViewIndex = selectedIndex;
                                PreviousVisibleView = newVisibleView;

                                IsPushing = false;
                                IsPopping = false;


                            }


                        }
                        else
                        {
                            SetNewVisibleViewAsOneVisible();
                        }

                        foreach (var view in Views)
                        {
                            if (animating && view == previousVisibleView?.View)
                                continue;

                            if (view == newVisibleView?.View)
                                continue;

                            UnloadView(new NavigationStackEntry(view, false, false));
                        }



                    }
                    else
                    {
                        //no previous view, just show top view
                        SetNewVisibleViewAsOneVisible();
                    }


                    PreviousVisibleView = newVisibleView;
                    previousVisibleViewIndex = selectedIndex;

                }

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            finally
            {

                Invalidate();

                NavigationBusy = false; //can be still animating!

                //PrintDebug();


            }


        }

        private async void UpdateSelectedView(int selectedIndex, bool? animate = null)
        {
            if (selectedIndex >= 0)
            {
                PushIndex(new OrderedIndex(selectedIndex, animate));
            }
        }

        public event EventHandler<bool> BusyChanged;
        public event EventHandler<SkiaControl> LoadedLazyView;



        private static readonly double _sideCoeff = 0.55;
        private readonly Easing _easing = new Easing((x) => (x - 1) * (x - 1) * ((_sideCoeff + 1) * (x - 1) + _sideCoeff) + 1);

        public void SendOnDisappearing(SkiaControl view)
        {

            if (view is IVisibilityAware aware)
            {
                aware.OnDisappearing();
            }

        }
        protected void UnloadView(NavigationStackEntry view)
        {
            if (view == null)
                return;

            ChangeViewVisibility(view.View, false);

            if (ActiveView == view)
            {
                ActiveView = null;
            }

            if (IsPopping)
            {
                SendOnDisappearing(view.View);
                if (!view.Preserve && view.View is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            else
            {
                Task.Run(() =>
                {
                    SendOnDisappearing(view.View);
                }).ConfigureAwait(false);
            }
        }

        private void HideView(SkiaControl view, int viewIndex, bool animate)
        {
            ChangeViewVisibility(view, false);
        }

        #region IDefinesViewport

        public ScaledRect Viewport { get; }

        public void UpdateVisibleIndex()
        {

        }

        public RelativePositionType TrackIndexPosition { get; }

        public void ScrollTo(float x, float y, float maxTimeSecs)
        {

        }

        #endregion

    }
}
