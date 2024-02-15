using AppoMobi.Maui.Navigation;
using AppoMobi.Specials;
using DrawnUi.Maui.Draw;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Controls
{
    /*
       The main concept is we take the maui shell implementation 
      and add additional actions that would draw skia content 
      on the MainPage Canvas instead of using maui INavigation 
      implementation. This drawn action fully support maui routing,

      Can also routes use  ex:
      `await GoToAsync("details?id=123")`.

      For that to work we need to set some base drawing points like RootLayout, ViewSwitcher..
      You are responsible to set those on your own, normally inside your MainPage.xaml.cs
      after InitializeComponent() you need to call:

      Shell.Initialize(canvas);

       */

    /// <summary>
    /// A Canvas with Navigation capabilities
    /// </summary>
    public partial class SkiaShell : DrawnUiBasePage
    {


        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler != null)
            {
#if ANDROID
                //we replace some layout visual parts with our own, this is totally optional
                InitializeNative(Handler);
#endif
            }
        }

        //public static readonly BindableProperty CanvasProperty = BindableProperty.Create(
        //    nameof(Canvas),
        //    typeof(Canvas),
        //    typeof(SkiaShell),
        //    defaultValueCreator: CreateDefaultCanvas);

        //public Canvas Canvas
        //{
        //    get { return (Canvas)GetValue(CanvasProperty); }
        //    set { SetValue(CanvasProperty, value); }
        //}

        //private static object CreateDefaultCanvas(BindableObject bindable)
        //{
        //    var defaultCanvas = new Canvas()
        //    {
        //        HorizontalOptions = LayoutOptions.Fill,
        //        VerticalOptions = LayoutOptions.Fill
        //    };
        //    return defaultCanvas;
        //}

        public static Color ToastBackgroundColor = Color.Parse("#CC000000");
        public static Color ToastTextColor = Colors.White;
        public static string ToastTextFont = null;
        public static int ToastTextFontWeight = 0;
        public static double ToastTextSize = 16.0;
        public static double ToastTextMargins = 24.0;
        public static float PopupsAnimationSpeed = 150;
        public static bool LogEnabled = false;

        public static int ZIndexModals = 1000;
        public static int ZIndexPopups = 2000;
        public static int ZIndexToasts = 3000;

        public SkiaShell()
        {
            Services = Super.Services;

            Popups = new(this, true);
            Toasts = new(this, false);

            //close jeyboard on app startup
            Tasks.StartDelayed(TimeSpan.FromSeconds(2), TouchEffect.CloseKeyboard);
        }

        public SkiaControl ShellLayout { get; set; }

        /// <summary>
        /// The main control that pushes pages, switches tabs etc
        /// </summary>
        public SkiaViewSwitcher NavigationLayout { get; set; }

        /// <summary>
        /// Use this for covering everything in a modal way, precisely tabs
        /// </summary>
        public SkiaControl RootLayout { get; set; }

        private SkiaDrawer _drawer;

        public SkiaControl GetTopPopup()
        {
            lock (_lockLayers)
            {
                var popup = Popups.NavigationStack.LastOrDefault();
                if (popup != null)
                {
                    return popup;
                }
                return null;
            }
        }

        public SkiaControl GetTopModal()
        {
            lock (_lockLayers)
            {
                var modal = NavigationStackModals.LastOrDefault();
                if (modal != null)
                {
                    if (modal.Page is SkiaControl control)
                    {
                        return control;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the topmost visible view:
        /// if no popups and modals are open then return NavigationLayout
        /// otherwise return the topmost popup or modal
        /// depending which ZIndexModals or ZIndexPopups is higher
        /// </summary>
        /// <returns></returns>
        public SkiaControl GetTopmostView()
        {

            SkiaControl topmost = null;

            if (ZIndexModals > ZIndexPopups)
            {
                topmost = GetTopModal();
                if (topmost == null)
                    topmost = GetTopPopup();
            }
            else
            {
                topmost = GetTopPopup();
                if (topmost == null)
                    topmost = GetTopModal();
            }

            if (topmost == null)
            {
                if (NavigationLayout != null)
                {
                    topmost = NavigationLayout;
                }
            }

            return topmost;
        }



        #region SCREENS

        public virtual async Task<SkiaControl> RemoveAsync(SkiaControl modal, bool? animate, IDictionary<string, object> arguments = null)
        {
            if (modal == null)
                return null;

            //await LockNavigation.WaitAsync();

            try
            {
                var presentation = Shell.GetPresentationMode(modal);

                if (animate == null)
                {
                    if (presentation == PresentationMode.Animated || presentation == PresentationMode.ModalAnimated)
                    {
                        animate = true;
                    }
                }

                if (presentation == PresentationMode.ModalNotAnimated || presentation == PresentationMode.ModalAnimated || presentation == PresentationMode.Modal)
                {

                    //todo find the existing drawer if any
                    var inStack = NavigationStackModals.FirstOrDefault(x => x.Page is SkiaDrawer drawer && drawer.Content == modal);
                    if (inStack != null)
                    {
                        modal = inStack.Page as SkiaDrawer;
                    }

                    var removed = await this.PopModalAsync(modal, animate.GetValueOrDefault());
                    //if wasnt animater drawer will not scroll
                    //so kill it manually
                    if (removed != null && !animate.GetValueOrDefault())
                    {
                        await RemoveModal(modal, animate.GetValueOrDefault());

                        return modal;
                    }
                }
                else
                {
                    //todo !!!!
                    //await this.PopAsync(skia, animate.GetValueOrDefault());
                }

                return modal;
            }
            finally
            {
                //LockNavigation.Release();
            }
        }
        public virtual async Task<SkiaControl> PresentAsync(string registered, bool? animate, IDictionary<string, object> arguments = null)
        {
            //await LockNavigation.WaitAsync();

            try
            {
                var page = GetOrCreateContentSetArguments<BindableObject>(registered, arguments);
                if (page != null)
                {
                    var skia = page as SkiaControl;

                    if (page is ISkiaAttachable attachable)
                    {
                        skia = attachable.AttachControl;
                    }

                    var presentation = Shell.GetPresentationMode(page);

                    if (animate == null)
                    {
                        if (presentation == PresentationMode.Animated || presentation == PresentationMode.ModalAnimated)
                        {
                            animate = true;
                        }
                    }

                    if (presentation == PresentationMode.ModalNotAnimated || presentation == PresentationMode.ModalAnimated || presentation == PresentationMode.Modal)
                    {
                        if (animate == null)
                        {
                            if (presentation == PresentationMode.ModalAnimated)
                            {
                                animate = true;
                            }
                        }
                        skia = await this.PushModalAsync(skia, false, animate.GetValueOrDefault());
                    }
                    else
                    {
                        await this.PushAsync(skia, animate.GetValueOrDefault());
                    }

                    AddToCurrentRouteNodes(registered, skia);

                    return skia;
                }

                throw new Exception($"SkiaShell PresentAsync failed  for '{registered}'!");
            }
            finally
            {
                //LockNavigation.Release();
            }

        }

        public virtual async Task PushRegisteredPageAsync(string registered, bool animate, IDictionary<string, object> arguments = null)
        {
            //await LockNavigation.WaitAsync();

            try
            {
                var page = GetOrCreateContentSetArguments<BindableObject>(registered, arguments);
                if (page != null)
                {
                    SetArguments(page, arguments);

                    if (page is ISkiaAttachable attachable)
                    {
                        await this.PushAsync(attachable.AttachControl, animate);
                        return;
                    }

                }

                throw new Exception($"SkiaShell PushRegisteredPageAsync failed  for '{registered}'!");
            }
            finally
            {
                //LockNavigation.Release();
            }

        }



        /// <summary>
        /// Uses ViewSwitcher to push a view on the canvas, into the current tab if any.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="animated"></param>
        /// <returns></returns>
        public virtual async Task PushAsync(BindableObject page, bool animated = true)
        {
            await AwaitNavigationLock();

            try
            {
                NavigationLayout.PushView(page as ISkiaAttachable, animated, false);
                NavigationStackScreens.AddLast(new PageInStack
                {
                    Page = page
                });

            }
            finally
            {
                UnlockNavigation();
            }
        }
        /// <summary>
        /// Uses ViewSwitcher to push a view on the canvas, into the current tab if any. We can use a route with arguments to instantiate the view instead of passing an instance.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="animated"></param>
        /// <returns></returns>
        public virtual async Task PushAsync(string registered, bool animated = true, IDictionary<string, object> arguments = null)
        {
            var page = GetOrCreateContentSetArguments<BindableObject>(registered, arguments);

            await PushAsync(page, animated);
        }

        public interface IHandleGoBack
        {
            /// <summary>
            /// Return true if comsumed, false will use default system behaivour.
            /// </summary>
            /// <returns></returns>
            bool OnShellGoBack(bool animate);
        }

        public static IHandleGoBack OnShellGoBack { get; set; }

        public virtual bool GoBack(bool animate)
        {
            bool ret = false;

            if (OnShellGoBack != null)
            {
                ret = OnShellGoBack.OnShellGoBack(animate);
            }

            if (!ret)
            {
                ret = GoBackDefault(animate);
            }

            return ret;
        }

        public bool GoBackDefault(bool animate)
        {
            var consumed = false;
            if (CheckCanGoBack(animate))
            {
                if (Popups.NavigationStack.Any())
                {
                    ClosePopupAsync(animate).ConfigureAwait(false);
                    consumed = true;
                }
                else
                if (ModalStack.Any())
                {
                    PopModalAsync(animate).ConfigureAwait(false);
                    consumed = true;
                }
                else
                if (NavigationStackScreens.Any())
                {
                    PopAsync(animate).ConfigureAwait(false);
                    consumed = true;
                }
            }
            else
            {
                consumed = true;
            }

            return consumed;
        }

        /// <summary>
        /// This will not affect popups
        /// </summary>
        /// <param name="animate"></param>
        /// <returns></returns>
        protected async Task<SkiaControl> GoBackInRoute(bool animate)
        {

            if (ModalStack.Any())
            {
                return await PopModalAsync(animate) as SkiaControl;
            }

            if (NavigationStackScreens.Any())
            {
                return await PopAsync(animate) as SkiaControl;
            }

            return null;
        }



        /// <summary>
        /// Returns the page so you could dispose it if needed. Uses ViewSwitcher.
        /// </summary>
        /// <param name="animated"></param>
        /// <returns></returns>
        public virtual async Task<BindableObject> PopAsync(bool animated = true)
        {
            await AwaitNavigationLock();
            try
            {
                var inStack = NavigationStackScreens.LastOrDefault();
                if (inStack != null)
                {
                    await NavigationLayout.PopPage();

                    NavigationStackScreens.RemoveLast();

                    RemoveFromCurrentRouteNodes(inStack.Page as SkiaControl);

                    return inStack.Page;
                }
                return null;
            }
            finally
            {
                UnlockNavigation();
            }
        }

        #endregion

        #region HELPERS

        SKColor GetAverageColor(SKImage image)
        {
            using var bitmap = SKBitmap.FromImage(image);
            using var scaledBitmap = bitmap.Resize(new SKImageInfo(1, 1), SKFilterQuality.Medium);
            SKColor color = scaledBitmap.GetPixel(0, 0);
            return color;
        }

        public float FrozenBackgroundBlur = 3;
        public float FrozenBackgroundDim = 0.25f;

        /// <summary>
        /// Override this to create your own image with your own effect of the screenshot to be placed under modal controls. Default is image with Darken Effect.
        /// </summary>
        /// <param name="screenshot"></param>
        /// <returns></returns>
        public virtual SkiaImage WrapScreenshot(SkiaControl control, SKImage screenshot, bool animated)
        {
            //blur will create alpha on borders, so we need a background color
            var background = new SkiaImage()
            {
                UseCache = SkiaCacheType.Image,
                BackgroundColor = Colors.Black, // corners will be blurred vs this color
                Tag = "RootLayoutSnapshot",
                ZIndex = RootLayout.ZIndex + 1,
                IsClippedToBounds = true,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Aspect = TransformAspect.AspectCover,
                Effect = SkiaImageEffect.Darken,
                Darken = FrozenBackgroundDim,
                Opacity = 0,
                WhenLayoutIsReady = async (sender) =>
                {
                    await FreezeRootLayoutInternal(control, sender, animated);
                },
            };

            if (FrozenBackgroundBlur > 0)
            {
                var color = GetAverageColor(screenshot);
                background.BackgroundColor = color.ToMauiColor();
                background.Blur = FrozenBackgroundBlur;
            }

            background.SetImageInternal(screenshot);

            return background;
        }




        #endregion

        #region MODALS

        protected virtual SkiaDrawer CreateModalDrawer()
        {
            _drawer = new SkiaDrawer()
            {
                Tag = "Modal",
                Bounces = false,
                ZIndex = ZIndexModals + ModalStack.Count,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Direction = DrawerDirection.FromBottom,
                HeaderSize = 0,
            };

            return _drawer;
        }

        /// <summary>
        /// Creates a SkiaDrawer opening over the RootLayout with the passed content. Override this method to create your own implementation.
        /// Default freezing background is True, control with frozenLayerBackgroundParameters parameter.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="animated"></param>
        /// <returns></returns>
        public virtual async Task<SkiaControl> PushModalAsync(BindableObject page,
            bool useGestures,
            bool animated = true,
            VisibilityParameters? frozenLayerBackgroundParameters = null,
            IDictionary<string, object> arguments = null)
        {
            await AwaitNavigationLock();

            try
            {
                if (frozenLayerBackgroundParameters == null)
                {
                    frozenLayerBackgroundParameters = VisibilityParameters.Visible;
                }

                if (arguments != null)
                {
                    SetArguments(page, arguments);
                }

                var drawer = CreateModalDrawer();

                NavigationStackModals.AddLast(new PageInStack
                {
                    Page = drawer
                });

                if (CanFreezeLayout() && frozenLayerBackgroundParameters.Value.IsVisible)
                    await FreezeRootLayout(drawer, animated);

                drawer.RespondsToGestures = useGestures;

                try
                {
                    var content = (page as ISkiaAttachable).AttachControl;
                    drawer.BindingContext = content.BindingContext;

                    drawer.Content = content;
                    drawer.Animated = animated;

                    drawer.Tag = $"ModalDrawer_{content.GetType().Name}";

                    drawer.OnViewportReady += ViewportReadyHandler;
                    drawer.OnTransitionChanged += OnModalDrawerScrolled;

                    _pushModalWaitingAnimatedOpen = false;

                    void ViewportReadyHandler(object sender, EventArgs e)
                    {
                        if (sender is SkiaDrawer control)
                        {
                            control.OnViewportReady -= ViewportReadyHandler;
                            MainThread.BeginInvokeOnMainThread(() => //to unlock ui thread before this
                            {
                                _pushModalWaitingAnimatedOpen = !animated;
                                if (control is IVisibilityAware aware)
                                {
                                    aware.OnAppearing();
                                }
                                control.IsOpen = true;
                            });
                        }
                    }

                    drawer.SetParent(ShellLayout);

                    while (!_pushModalWaitingAnimatedOpen)
                    {
                        await Task.Delay(20); //switch thread, wait until drawer animation completes
                    }

                    return drawer.Content;
                }
                catch (Exception e)
                {
                    _pushModalWaitingAnimatedOpen = true;
                    Trace.WriteLine(e);
                    OnLayersChanged();
                }

                return null;

            }
            finally
            {
                UnlockNavigation();
            }

        }

        volatile bool _pushModalWaitingAnimatedOpen;

        async void OnModalDrawerScrolled(object sender, bool inTransition)
        {
            if (!inTransition && sender is SkiaDrawer control)
            {
                if (control.IsOpen)
                {
                    //animated to open, display frozen layer if any
                    if (CanUnfreezeLayout())
                    {
                        await SetFrozenLayerVisibility(control, VisibilityParameters.Visible);
                    }

                    OnLayersChanged();

                    _pushModalWaitingAnimatedOpen = true;
                }
                else
                {
                    //animated to closed
                    await RemoveModal(control, true);
                }
            }
        }


        protected void SetCurrentRouteNodes(List<ShellStackChild> children)
        {
            CurrentRoute.Nodes = children;

            OnCurrentRouteChanged();
        }

        protected void AddToCurrentRouteNodes(string registered, SkiaControl created)
        {
            if (created != null)
            {
                CurrentRoute.Nodes.Add(new()
                {
                    Control = created,
                    Part = registered
                });
                OnCurrentRouteChanged();
            }
        }

        protected bool RemoveFromCurrentRouteNodes(SkiaControl control)
        {
            if (control != null)
            {
                var inStack = CurrentRoute.Nodes.FirstOrDefault(x => x.Control == control);
                if (inStack != null)
                {
                    CurrentRoute.Nodes.Remove(inStack);

                    OnCurrentRouteChanged();

                    return true;
                }
            }

            return false;

        }

        protected virtual void OnCurrentRouteChanged()
        {
            Debug.WriteLine($"[ROUTE] {CurrentRouteAuto}");
        }

        public string CurrentRouteAuto
        {
            get
            {
                if (CurrentRoute == null)
                {
                    return string.Empty;
                }
                var build = "//";
                foreach (var node in CurrentRoute.Nodes)
                {
                    build += "/" + node.Part;
                }
                return build; //todo use stringbuilder
            }
        }

        public virtual async Task PushModalAsync(string registered,
            bool useGestures,
            bool animated = true,
            VisibilityParameters? frozenLayerBackgroundParameters = null,
            IDictionary<string, object> arguments = null)
        {
            var page = GetOrCreateContentSetArguments<BindableObject>(registered, arguments);

            if (page == null)
            {
                throw new Exception($"Failed to create modal for route {registered}");
            }

            var created = await PushModalAsync(page, useGestures, animated, frozenLayerBackgroundParameters);

            AddToCurrentRouteNodes(registered, created);
        }

        public virtual async Task<BindableObject> PopModalAsync(bool animated)
        {
            var inStack = NavigationStackModals.LastOrDefault();

            return await PopModalAsync(inStack, animated);
        }

        void UnlockGesturesForTop()
        {
            var visible = NavigationStackModals.LastOrDefault();
            if (visible != null) //unlock previous todo check if still needed
            {
                if (visible.Page is SkiaControl control)
                {
                    control.InputTransparent = false;
                }
            }
        }

        bool CheckCanGoBack(bool animate)
        {
            var modal = GetTopModal();

            if (modal != null)
            {
                if (modal.BindingContext is IHandleGoBack onGoBack)
                {
                    if (onGoBack.OnShellGoBack(animate))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected async Task<SkiaControl> PopModalAsync(SkiaControl modal, bool animated)
        {
            SkiaControl removed = null;

            if (modal != null)
            {
                if (modal.IsVisibleInViewTree())
                {
                    if (modal is SkiaDrawer drawer)
                    {
                        drawer.Animated = animated;
                        if (animated)
                        {
                            drawer.IsOpen = false; //gonna kill manually upstairs
                        }
                        removed = drawer.Content;
                    }
                    else
                    {
                        await modal.TranslateToAsync(0, ShellLayout.MeasuredSize.Units.Height, 250, Easing.BounceOut);

                        removed = modal;
                    }

                }

                return removed;
            }

            return null;
        }

        async Task RemoveModal(SkiaControl control, bool animated)
        {
            if (control != null)
            {
                control.IsVisible = false;

                if (control is SkiaDrawer drawer && drawer.Content is SkiaControl removed)
                {
                    RemoveFromCurrentRouteNodes(removed);

                    var inStack = NavigationStackModals.FirstOrDefault(x => x.Page == drawer);
                    if (inStack != null)
                    {
                        NavigationStackModals.Remove(inStack);
                    }

                    drawer.OnTransitionChanged -= OnModalDrawerScrolled;
                    if (removed is IVisibilityAware aware)
                    {
                        aware.OnDisappearing();
                    }
                }
                else
                {
                    RemoveFromCurrentRouteNodes(control);
                }

                if (CanUnfreezeLayout())
                    await UnfreezeRootLayout(control, animated);

                OnLayersChanged(control);

                control.SetParent(null); //unregister gestures etc
                control.Dispose();
            }
        }


        bool isBusyClosingModal;

        public virtual async Task<BindableObject> PopModalAsync(PageInStack inStack,
            bool animated)
        {
            await AwaitNavigationLock();

            try
            {
                var modal = inStack.Page as SkiaControl; //GetTopModal();

                if (modal.IsVisibleInViewTree())
                {
                    //will close drawer..
                    SkiaControl removed = await PopModalAsync(modal, animated);

                    //if wasnt animater drawer will not scroll
                    //so kill it manually
                    if (removed != null && !animated)
                    {
                        await RemoveModal(modal, animated);
                        return modal;
                    }
                    else
                    {
                        NavigationStackModals.Remove(inStack);
                    }

                    //RootLayout?.Update();

                    return removed;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                UnlockNavigation();
            }
        }

        #endregion

        public Task PopTabToRoot()
        {
            return NavigationLayout.PopTabToRoot();
        }

        /// <summary>
        /// Override this if you have custom navigation layers and custom logic to decide if we can unfreeze background.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanFreezeLayout()
        {
            return true;
        }

        /// <summary>
        /// Override this if you have custom navigation layers and custom logic to decide if we can unfreeze background.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanUnfreezeLayout()
        {
            return true;
        }

        protected SemaphoreSlim LockLayers = new(1, 1);

        protected SemaphoreSlim LockNavigation = new(1, 1);

        protected void SetupFrozenLayersVisibility(SkiaControl except)
        {
            foreach (var frozenLayersValue in FrozenLayers.Values)
            {
                frozenLayersValue.IsVisible = frozenLayersValue == except;
                frozenLayersValue.InputTransparent = !frozenLayersValue.IsVisible;
            }
        }

        public List<SkiaControl> FreezingModals { get; protected set; } = new();


        /// <summary>
        /// Display or hide the background scrrenshot assotiated with an overlay control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected async Task SetFrozenLayerVisibility(SkiaControl control, VisibilityParameters parameters)
        {
            await AwaitLayersLock();
            try
            {
                FrozenLayers.TryGetValue(control, out var screenshot);
                if (screenshot != null)
                {
                    screenshot.IsClippedToBounds = true;
                    screenshot.IsVisible = parameters.IsVisible;
                }
            }
            finally
            {
                UnlockLayers();
            }
        }

        /// <summary>
        /// pass who frozen the layout
        /// </summary>
        /// <param name="control"></param>
        /// <param name="animated"></param>
        /// <returns></returns>
        public virtual async Task UnfreezeRootLayout(SkiaControl control, bool animated)
        {
            await AwaitLayersLock();

            try
            {
                FrozenLayers.Remove(control, out var screenshot);

                if (FreezingModals.Count > 0)
                {
                    FreezingModals.Remove(control);
                    var topmost = FreezingModals.LastOrDefault();
                    if (topmost != null)
                    {
                        await SetupModalsVisibility(topmost);
                        FrozenLayers.TryGetValue(topmost, out var frozen);
                        if (frozen != null)
                        {
                            SetupFrozenLayersVisibility(frozen);
                            if (LogEnabled)
                                Super.Log("[SHELL] Unfrozen layout");
                        }
                        else
                        {
                            if (LogEnabled)
                                Super.Log(($"[SHELL] FrozenLayer not found for {topmost}!"));
                        }
                    }
                }

                if (FreezingModals.Count < 1)
                    RootLayout.IsVisible = true;

                if (screenshot != null)
                {
                    if (animated && screenshot.IsVisibleInViewTree())
                        await screenshot.FadeToAsync(0, 250);

                    RootLayout.RemoveSubView(screenshot);

                    //GC.Collect(0, GCCollectionMode.Optimized);
                }
            }
            finally
            {
                UnlockLayers();
            }

        }

        /// <summary>
        /// Freezes layout below the overlay: takes screenshot of RootLayout, places it over, then hides RootLayout to avoid rendering it. Can override
        /// </summary>
        public virtual async Task FreezeRootLayout(SkiaControl control, bool animated)
        {
            //System.Diagnostics.Debug.WriteLine("[LockLayers] lock FreezeRootLayout");
            await AwaitLayersLock();

            FreezingModals.Add(control);

            RootLayout.Superview.TakeScreenShot((screenshot) =>
            {
                if (screenshot != null)
                {
                    var background = WrapScreenshot(control, screenshot, animated);
                    //background.ZIndex = int.MinValue;
                    ShellLayout.AddSubView(background);
                    FrozenLayers.TryAdd(control, background);
                    //will use FreezeRootLayoutInternal to hide views below when screenshot is drawn 
                }

                //System.Diagnostics.Debug.WriteLine("[LockLayers] unlock FreezeRootLayout");
                UnlockLayers();
            });
        }

        public virtual async Task FreezeRootLayoutInternal(SkiaControl control, SkiaControl screenshot, bool animated)
        {
            await AwaitLayersLock();

            try
            {
                if (FreezingModals.Contains(control))
                {
                    if (animated)
                        await screenshot.FadeToAsync(1, 250);
                    else
                        screenshot.Opacity = 1;

                    RootLayout.IsVisible = false; //todo hide other modals in stacks below control
                    SetupFrozenLayersVisibility(screenshot);
                    await SetupModalsVisibility(control);
                    if (LogEnabled)
                        Super.Log("[SHELL] Frozen layout");
                }

            }
            finally
            {
                UnlockLayers();
            }
        }

        public virtual async Task<bool> SetupModalsVisibility(SkiaControl control)
        {
            bool wasSet = false; //handle the cas when control was removed from stack just before this executed..

            foreach (var modal in NavigationStackModals)
            {
                if (modal.Page is SkiaControl skia)
                {
                    skia.IsVisible = skia == control;
                    skia.InputTransparent = !skia.IsVisible;
                    if (skia.IsVisible)
                        wasSet = true;
                }
            }

            foreach (var popup in Popups.NavigationStack)
            {
                if (popup is SkiaControl skia)
                {
                    skia.IsVisible = skia == control;
                    skia.InputTransparent = !skia.IsVisible;
                    if (skia.IsVisible)
                        wasSet = true;
                }
            }


            return wasSet;
        }

        //todo weakreference !!!!!!!!
        protected SkiaControl _topmost;

        protected object _lockLayers = new();
        /// <summary>
        /// Setup _topmost and send OnAppeared / OnDisappeared to views.
        /// Occurs when layers configuration changes,
        /// some layer go visible, some not, some are added, some are removed.
        /// </summary>
        protected virtual void OnLayersChanged(SkiaControl dissapeared = null)
        {
            lock (_lockLayers)
            {

                var newTopmost = GetTopmostView();

                if (_topmost != newTopmost)
                {
                    if (_topmost == dissapeared)
                    {
                        dissapeared = null;
                    }
                    SetupDisappeared(_topmost);
                    _topmost = GetTopmostView();
                    SetupAppeared(_topmost);
                }

                if (dissapeared != null)
                {
                    SetupDisappeared(dissapeared);
                }

                RootLayout?.Update();
            }
        }

        protected virtual void SetupAppeared(SkiaControl control)
        {
            if (control != null && !control.IsDisposed)
            {
                control.InputTransparent = false;
                if (control is IVisibilityAware aware)
                {
                    aware.OnAppeared();
                }
            }
        }

        protected virtual void SetupDisappeared(SkiaControl control)
        {
            if (control != null && !control.IsDisposed)
            {
                control.InputTransparent = true;
                if (control is IVisibilityAware aware)
                {
                    aware.OnDisappeared();
                }
            }
        }


        #region POPUPS

        /// <summary>
        /// TODO make this non-concurrent
        /// </summary>
        public Dictionary<SkiaControl, SkiaControl> FrozenLayers { get; } = new();

        public Controls.SkiaShell.NavigationLayer<SkiaControl> Popups { get; }


        /// <summary>
        /// Close topmost popup
        /// </summary>
        /// <param name="animated"></param>
        /// <returns></returns>
        public async Task ClosePopupAsync(bool animated)
        {
            var popup = Popups.NavigationStack.LastOrDefault();
            if (popup != null)
            {
                await ClosePopupAsync(popup, animated);
            }
        }

        public async Task ClosePopupAsync(SkiaControl popup, bool animated)
        {
            if (popup != null)
            {
                await AwaitNavigationLock();

                try
                {
                    if (animated && popup.IsVisibleInViewTree())
                    {
                        await Task.WhenAll(
                            popup.FadeToAsync(0, PopupsAnimationSpeed),
                            popup.ScaleToAsync(0, 0, PopupsAnimationSpeed));
                    }

                    //popup disposed inside
                    await Popups.Close(popup, animated);
                    OnLayersChanged();
                }
                finally
                {
                    UnlockNavigation();
                }
            }
        }

        public class PopupWrapper : ContentLayout
        {
            private readonly bool _closeWhenBackgroundTapped;
            private readonly bool _animated;
            private readonly SkiaShell _shell;

            public PopupWrapper(bool closeWhenBackgroundTapped, bool animated, SkiaShell shell)
            {
                _closeWhenBackgroundTapped = closeWhenBackgroundTapped;
                _animated = animated;
                _shell = shell;
            }

            public override ISkiaGestureListener ProcessGestures(TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
                SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
            {
                if (_closeWhenBackgroundTapped
                    && Content != null
                    && touchAction == TouchActionResult.Tapped)
                {
                    var point = TranslateInputOffsetToPixels(args.Location, childOffset);
                    if (!Content.HitIsInside(point.X, point.Y))
                    {
                        _shell.ClosePopupAsync(this, _animated).ConfigureAwait(false);
                    }
                }

                return base.ProcessGestures(type, args, touchAction, childOffset, childOffsetDirect, alreadyConsumed);
            }
        }

        /// <summary>
        /// Pass pixelsScaleInFrom you you want popup to scale in not from the center of the screen but from that point. Can override appearig animation with taskAnimateAppearence parameter.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="animated"></param>
        /// <param name="closeWhenBackgroundTapped"></param>
        /// <param name="scaleInFrom"></param>
        /// <returns></returns>
        public async Task<SkiaControl> OpenPopupAsync(
            SkiaControl content,
            bool animated = true,
            bool closeWhenBackgroundTapped = true,
            SKPoint? pixelsScaleInFrom = null,
            Func<SkiaControl, Task> taskAnimateAppearence = null)
        {
            await AwaitNavigationLock();

            try
            {
                TaskCompletionSource<SkiaControl> taskCompletionSource = new TaskCompletionSource<SkiaControl>();

                SkiaControl popup = null;
                SkiaControl control = null;

                control = new PopupWrapper(closeWhenBackgroundTapped, animated, this)
                {
                    BindingContext = content.BindingContext,
                    ZIndex = ZIndexPopups + Popups.NavigationStack.Count,
                    Opacity = 0,
                    ScaleX = 0.1,
                    ScaleY = 0.1,
                    WhenLayoutIsReady = async (sender) =>
                    {
                        try
                        {
                            popup = sender;

                            if (taskAnimateAppearence != null)
                            {
                                await taskAnimateAppearence(sender);
                            }
                            else
                            {
                                if (animated)
                                {
                                    await sender.AnimateWith(
                                        (c) => c.FadeToAsync(1, PopupsAnimationSpeed),
                                        (c) => c.ScaleToAsync(1, 1, PopupsAnimationSpeed));
                                }
                                else
                                {
                                    sender.Scale = 1;
                                    sender.Opacity = 1;
                                }
                            }

                            taskCompletionSource.SetResult(popup);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(e);
                        }
                        finally
                        {
                            OnLayersChanged();
                        }
                    },
                    WhenDisposing = (sender) =>
                    {
                        popup = null;
                    },
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Content = content
                };

                popup = control;

                if (pixelsScaleInFrom != null)
                {
                    control.TransformPivotPointX = pixelsScaleInFrom.Value.X / RootLayout.MeasuredSize.Pixels.Width;
                    control.TransformPivotPointY = pixelsScaleInFrom.Value.Y / RootLayout.MeasuredSize.Pixels.Height;
                }

                await Popups.Open(control, animated);

                if (animated)
                {
                    var completedTask = await Task.WhenAny(taskCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(10)));

                    if (completedTask == taskCompletionSource.Task)
                    {
                        return await taskCompletionSource.Task;
                    }
                    else
                    {
                        //timed out
                        taskCompletionSource.SetResult(popup);
                        return await taskCompletionSource.Task;
                    }
                }
                else
                {
                    // If not animated, set the result immediately.
                    taskCompletionSource.SetResult(popup);
                    return await taskCompletionSource.Task;
                }

            }
            finally
            {
                UnlockNavigation();
            }

        }

        #endregion

        #region TOASTS


        /*
                     await LockNavigation.WaitAsync();
            try
            {

            }
            finally
            {
                LockNavigation.Release();
            }

         */
        public Controls.SkiaShell.NavigationLayer<SkiaControl> Toasts { get; }

        public void ShowToast(SkiaControl content, int msShowTime = 4000)
        {
            Task.Run(async () =>
            {

                await AwaitNavigationLock();

                try
                {
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(TimeSpan.FromSeconds(6));

                    var control = new SkiaLayout()
                    {
                        Tag = "Toast",
                        UseCache = SkiaCacheType.Operations,
                        ZIndex = ZIndexToasts + Toasts.NavigationStack.Count,
                        Opacity = 0,
                        WhenLayoutIsReady = (sender) =>
                        {
                            sender.TranslationY = sender.Height;
                            sender.TranslateToAsync(0, 0, 300);
                            sender.FadeToAsync(1, 300);
                            var cancel = new CancellationTokenSource();
                            Tasks.StartDelayed(TimeSpan.FromMilliseconds(msShowTime), cancel.Token, async () =>
                            {
                                await Task.WhenAll(
                                    sender.TranslateToAsync(0, sender.Height, 250),
                                    sender.FadeToAsync(0, 250));
                                //disposed inside
                                await Toasts.Close(sender, true);
                            });
                        },
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.End,
                        BackgroundColor = ToastBackgroundColor,
                        CreateChildren = () => new()
                        {
                            new SkiaLayout()
                            {
                                HorizontalOptions = LayoutOptions.Fill,
                                CreateChildren = () => new()
                                {
                                    content
                                }
                            }
                        }
                    };

                    await Toasts.Open(control, true);
                }
                finally
                {
                    UnlockNavigation();
                }

            });
        }

        protected virtual void UnlockNavigation([CallerMemberName] string caller = null)
        {
            //if (LogEnabled)
            Super.Log($"[Shell] Navigation UNlocked by {caller}");
            LockNavigation.Release();
        }

        protected virtual async Task AwaitNavigationLock([CallerMemberName] string caller = null)
        {
            // if (LogEnabled)
            Super.Log($"[Shell] Navigation LOCKED by {caller}");
            await LockNavigation.WaitAsync();
        }

        protected virtual void UnlockLayers([CallerMemberName] string caller = null)
        {
            //if (LogEnabled)
            Super.Log($"[Shell] Layers UNlocked by {caller}");
            LockLayers.Release();
        }

        protected virtual async Task AwaitLayersLock([CallerMemberName] string caller = null)
        {
            // if (LogEnabled)
            Super.Log($"[Shell] Layers LOCKED by {caller}");
            await LockLayers.WaitAsync();
        }

        public virtual void ShowToast(string text,
            int msShowTime = 4000)
        {

            var content = new SkiaMarkdownLabel()
            {
                TextColor = ToastTextColor,
                Text = text,
                FontFamily = ToastTextFont,
                FontWeight = ToastTextFontWeight,
                FontSize = ToastTextSize,
                Margin = new Thickness(ToastTextMargins),
            };

            ShowToast(content, msShowTime);
        }



        #endregion


        public bool HasTopmostModalBindingContext<T>()
        {
            if (GetTopModal() is SkiaDrawer drawer)
            {
                if (drawer.Content?.BindingContext is T)
                {
                    return true;
                }
            }
            return false;
        }


        //todo pass canvas props to content
        public virtual void ApplyShellPropertiesToCanvas()
        {

        }

        //        protected override void OnHandlerChanged()
        //    {
        //        base.OnHandlerChanged();

        //        if (Handler != null)
        //        {
        //#if ANDROID
        //            //we replace some layout visual parts with our own, this is totally optional
        //            InitializeNative(Handler);
        //#endif
        //        }
        //    }

        public Canvas Canvas
        {
            get
            {
                return Content as Canvas;
            }
        }

        protected virtual void ReplaceShellLayout(ISkiaControl newLayout)
        {
            if (newLayout == ShellLayout)
                return;

            var kill = ShellLayout;
            ShellLayout = newLayout as SkiaControl;

            ImportShellLayout();

            var canvas = Content as Canvas;
            void Act()
            {
                canvas.Content = ShellLayout;
            }
            if (MainThread.IsMainThread)
            {
                Act();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Act();
                });
            }
            kill?.Dispose();
        }

        protected virtual void ReplaceRootLayout(ISkiaControl newLayout)
        {
            if (newLayout == RootLayout)
                return;

            var kill = RootLayout;

            if (RootLayout != null)
            {
                ShellLayout.RemoveSubView(RootLayout);
            }

            RootLayout = newLayout as SkiaControl;

            ShellLayout.AddSubView(RootLayout);

            ImportRootLayout();

            kill?.Dispose();
        }

        protected virtual void ImportShellLayout()
        {
            if (ShellLayout != null)
            {
                RootLayout = ShellLayout.FindViewByTag("RootLayout");
                if (RootLayout == null)
                {
                    throw new Exception("[DrawnUI] `RootLayout` Tag not found while initializing SkiaShell");
                }
                ImportRootLayout();
            }
        }

        protected virtual void ImportRootLayout()
        {
            if (RootLayout != null)
            {
                //optional
                NavigationLayout = RootLayout.FindView<SkiaViewSwitcher>("NavigationLayout");
            }
        }

        public virtual void Initialize(string route)
        {
            if (route == null)
            {
                route = "";
            }

            var content = Content as Canvas;

            if (content == null || content.Content == null)
            {
                throw new Exception("Shell must contain a Canvas with content.");
            }

            //required
            ShellLayout = content.Content as SkiaControl;

            ImportShellLayout();

            ParsedRoute startupRoute = null;
            if (!string.IsNullOrEmpty(route))
            {

                //we support  a route with subroutes for startup ex: "//main/chats?id-123"
                GoToAsync(route, false);

                //todo move this!!!
                this.Navigated?.Invoke(this, new ShellNavigatedEventArgs(
                    previous: "",
                    OrderedRoute,
                    ShellNavigationSource.Insert));
            }

            OnStarted();
        }

        protected virtual void OnStarted()
        {

        }


        public static void RegisterActionRoute(string route, Action switchToTab)
        {
            if (string.IsNullOrEmpty(route))
                return;

            tab_routes[route] = switchToTab;
        }

        public static bool ExecuteActionRoute(string route)
        {
            if (tab_routes.TryGetValue(route, out var action))
            {
                action?.Invoke();
                return true;
            }
            return false;
        }

        static Dictionary<string, Action> tab_routes = new Dictionary<string, Action>();

        #region ROUTER

        static Dictionary<string, TypeRouteFactory> s_routes = new();

        public void RegisterRoute(string route, Type type)
        {
            RegisterRoute(route, new TypeRouteFactory(this, type));
        }

        public BindableObject GetOrCreateContent(string route)
        {
            BindableObject result = null;

            if (s_routes.TryGetValue(route, out var content))
            {
                //var createContent = content.GetOrCreate(_services);

                var createContent = content.GetOrCreateObject(Services);

                result = createContent;
            }

            if (result == null)
            {
                // okay maybe its a type, we'll try that just to be nice to the user
                var type = Type.GetType(route);
                if (type != null)
                    result = Activator.CreateInstance(type) as Element;
            }

            if (result != null)
                SetRoute(result, route);

            return result;
        }

        public static void SetRoute(BindableObject obj, string value)
        {
            obj.SetValue(RouteProperty, value);
        }

        public static readonly BindableProperty RouteProperty =
            BindableProperty.CreateAttached("Route", typeof(string), typeof(Routing), null,
                defaultValueCreator: CreateDefaultRoute);

        static object CreateDefaultRoute(BindableObject bindable)
        {
            return $"{DefaultPrefix}{bindable.GetType().Name}{++s_routeCount}";
        }

        static int s_routeCount = 0;

        public class TypeRouteFactory : RouteFactory
        {
            readonly Type _type;
            private readonly SkiaShell _shell;

            public TypeRouteFactory(SkiaShell shell, Type type)
            {
                _shell = shell;
                _type = type;
            }

            public override Element GetOrCreate()
            {
                return (Element)Activator.CreateInstance(_type);
            }

            public BindableObject GetOrCreateObject(IServiceProvider services)
            {
                try
                {
                    if (services != null)
                    {
                        var o = services.GetService(_type);
                        if (o == null)
                        {
                            o = Activator.CreateInstance(_type);
                        }

                        return (BindableObject)o;
                    }
                    return (BindableObject)Activator.CreateInstance(_type);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _shell.DisplayAlert("SkiaShell", $"{e}", "OK");
                    });
                }

                return null;
            }

            public override Element GetOrCreate(IServiceProvider services)
            {
                if (services != null)
                {
                    var o = services.GetService(_type);
                    if (o == null)
                    {
                        o = Activator.CreateInstance(_type);
                    }

                    return (Element)o;
                }
                return (Element)Activator.CreateInstance(_type);
            }

            public override bool Equals(object obj)
            {
                if ((obj is TypeRouteFactory typeRouteFactory))
                    return typeRouteFactory._type == _type;

                return false;
            }

            public override int GetHashCode()
            {
                return _type.GetHashCode();
            }
        }
        static void ValidateRoute(string route, RouteFactory routeFactory)
        {
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentNullException(nameof(route), "Route cannot be an empty string");

            routeFactory = routeFactory ?? throw new ArgumentNullException(nameof(routeFactory), "Route Factory cannot be null");

            var uri = new Uri(route, UriKind.RelativeOrAbsolute);

            var parts = uri.OriginalString.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (IsImplicit(part))
                    throw new ArgumentException($"Route contains invalid characters in \"{part}\"");
            }

            TypeRouteFactory existingRegistration = null;

            if (s_routes.TryGetValue(route, out existingRegistration) && !existingRegistration.Equals(routeFactory))
                throw new ArgumentException($"Duplicated Route: \"{route}\"");
        }
        internal static bool IsImplicit(string source)
        {
            return source.StartsWith(ImplicitPrefix, StringComparison.Ordinal);
        }
        public static string FormatRoute(List<string> segments)
        {
            var route = FormatRoute(String.Join(PathSeparator, segments));
            return route;
        }

        public static string FormatRoute(string route)
        {
            return route;
        }

        public static void RegisterRoute(string route, TypeRouteFactory factory)
        {
            if (!String.IsNullOrWhiteSpace(route))
                route = FormatRoute(route);
            ValidateRoute(route, factory);

            s_routes[route] = factory;
        }

        const string ImplicitPrefix = "IMPL_";
        const string DefaultPrefix = "D_FAULT_";
        internal const string PathSeparator = "/";


        #endregion


        #region TODO INavigation



        public async Task PopToRootAsync()
        {
            NavigationLayout.PopAllTabsToRoot();
        }

        public IReadOnlyList<Page> ModalStack
        {
            get
            {
                return NavigationStackModals.Select(s => s.Page as Page).ToList();
                //            return _navigation.ModalStack;
            }
        }

        public IReadOnlyList<Page> NavigationStack
        {
            get
            {
                return NavigationStackScreens.Select(s => s.Page as Page).ToList();
                //return _navigation.NavigationStack;
            }
        }

        public class PageInStack
        {
            public string Route { get; set; }
            public IDictionary<string, object> Arguments { get; set; }
            public BindableObject Page { get; set; }
        }

        public LinkedList<PageInStack> NavigationStackScreens { get; } = new LinkedList<PageInStack>();

        public LinkedList<PageInStack> NavigationStackModals { get; } = new LinkedList<PageInStack>();

        //public static PageInStack GetItemAtIndex(LinkedList<PageInStack> linkedStack, int index)
        //{
        //    // Check for invalid index
        //    if (index < 0 || index >= linkedStack.Count)
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        //    }

        //    LinkedListNode<PageInStack> currentNode = linkedStack.First;
        //    for (int i = 0; i < index; i++)
        //    {
        //        currentNode = currentNode.Next;
        //    }

        //    return currentNode.Value;
        //}


        #endregion

        #region IAppShell

        public string OrderedRoute
        {
            get => _orderedRoute;
            protected set
            {
                if (value == _orderedRoute) return;
                _orderedRoute = value;
                OnPropertyChanged();
            }
        }

        public virtual async Task GoToAsync(ShellNavigationState state)
        {
            await GoToAsync(state, false);
        }

        protected string _rootRoute;

        protected readonly IServiceProvider Services;
        private string _orderedRoute = "";

        public static string BuildRoute(string host, IDictionary<string, object> arguments = null)
        {
            var ret = host;
            if (arguments != null)
            {
                ret += "?";
                foreach (var key in arguments)
                {
                    ret += $"{key.Key}={key.Value}&";
                }
            }
            return ret;
        }

        /// <summary>
        /// Main control inside RootLayout
        /// </summary>
        /// <param name="shellLayout"></param>
        protected virtual void SetupRoot(ISkiaControl shellLayout)
        {

        }

        /// <summary>
        /// Returns true if was replaced
        /// </summary>
        /// <param name="host"></param>
        /// <param name="replace"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual SkiaControl SetRoot(string host, bool replace, IDictionary<string, object> arguments = null)
        {
            //var currentRoute = BuildRoute(host, arguments);
            if (host == _rootRoute && !replace)
                return null;

            var page = GetOrCreateContentSetArguments<BindableObject>(host, arguments);
            if (page != null)
            {
                _rootRoute = host;

                SetArguments(page, arguments);

                if (page is SkiaControl control)
                {
                    ReplaceRootLayout(page as SkiaControl);

                    return control;
                }

            }

            throw new Exception($"SkiaShell failed to create page for '{host}'!");
        }

        protected virtual void SetArguments(BindableObject page, IDictionary<string, object> arguments)
        {
            if (page != null && arguments != null)
            {
                if (page.BindingContext is IQueryAttributable needQuery)
                {
                    needQuery.ApplyQueryAttributes(arguments);
                }
                else
                if (page.BindingContext != null)
                {
                    var type = page.BindingContext.GetType();
                    var t = type.GetAttribute<QueryPropertyAttribute>();
                    if (t is QueryPropertyAttribute attribute)
                    {
                        Reflection.TrySetPropertyValue(page.BindingContext, attribute.Name,
                            arguments[attribute.QueryId]);
                    }
                }
            }
        }

        public record ParsedRoute
        {
            public string Original { get; set; }
            public string[] Parts { get; set; }
            public IDictionary<string, object> Arguments { get; set; }
        }

        public static ParsedRoute ParseState(ShellNavigationState state)
        {
            if (!state.Location.IsAbsoluteUri)
            {
                var route = state.Location.OriginalString.Trim();
                return ParseRoute(route);
            }
            return null;
        }

        public static ParsedRoute ParseRoute(string route)
        {

            var fix = new Uri("fix://" + route.Trim('/'));

            var arguments = System.Web.HttpUtility.ParseQueryString(fix.Query);
            var dict = new Dictionary<string, object>();
            foreach (string key in arguments.AllKeys)
            {
                dict.Add(key, arguments[key]);
            }

            List<string> parts = new List<string>
            {
                fix.Host
            };
            foreach (var segment in fix.Segments)
            {
                var part = segment.Replace("/", "");
                if (!string.IsNullOrEmpty(part))
                {
                    parts.Add(part);
                }
            }

            return new ParsedRoute
            {
                Original = route,
                Parts = parts.ToArray(),
                Arguments = dict
            };

        }



        public virtual T GetOrCreateContentSetArguments<T>(string part, IDictionary<string, object> arguments) where T : BindableObject
        {

            var content = GetOrCreateContent(part) as T;
            if (content != null)
            {
                if (arguments != null)
                {
                    SetArguments(content, arguments);
                }
            }
            return content;
        }

        public record ShellStackChild
        {
            public string Part { get; set; }
            public SkiaControl Control { get; set; }
        }

        public record ShellCurrentRoute
        {
            //public ParsedRoute Route { get; set; }
            public List<ShellStackChild> Nodes { get; set; }
        }

        public ShellCurrentRoute CurrentRoute { get; protected set; } = new()
        {
            Nodes = new()
        };


        /// <summary>
        /// Navigate to a registered route. Arguments will be taken from query string of can be passed as parameter. You can receive them by implementing IQueryAttributable or using attribute [QueryProperty] in the page itsself or in the ViewModel that must be the screen's BindingContext upon registered screen instatiation.
        /// Animate can be specified otherwise will use it from Shell.PresentationMode attached property. This property will be also used for pushing as page, modal etc.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="animate"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public virtual async Task GoToAsync(ShellNavigationState state, bool? animate = null, IDictionary<string, object> arguments = null)
        {

            //TODO add semaphore!!!!!!

            var route = state.Location.OriginalString.Trim();

            if (!state.Location.IsAbsoluteUri)
            {
                var parsed = ParseRoute(route);
                bool replaceChain = false;
                if (parsed != null)
                {

                    try
                    {
                        IDictionary<string, object> passArguments = null;
                        if (arguments != null)
                        {
                            passArguments = arguments;
                        }

                        var index = 0;
                        var existingIndex = 0; //for CurrentRoute

                        var lastIndex = parsed.Parts.Length;

                        List<ShellStackChild> children = new();

                        foreach (var part in parsed.Parts)
                        {
                            index++;
                            existingIndex++;

                            //set args to last page only
                            if (index == lastIndex && parsed.Arguments.Count > 0)
                            {
                                passArguments = parsed.Arguments;
                            }

                            //go back
                            if (part == "..")
                            {
                                if (index == 1)
                                {
                                    var useAnimate = animate.GetValueOrDefault();
                                    if (animate == null)
                                        useAnimate = true;

                                    var removed = await GoBackInRoute(useAnimate);
                                    if (removed != null)
                                    {
                                        existingIndex--;
                                    }
                                }

                                children.Add(new()
                                {
                                    Part = part,
                                    Control = null
                                });

                                //todo go up the tree as ..\something
                                break;
                            }

                            async Task AddRoutePart()
                            {
                                if (!ExecuteActionRoute(part))
                                {
                                    var created = await PresentAsync(part, animate, passArguments);
                                    children.Add(new()
                                    {
                                        Part = part,
                                        Control = created
                                    });
                                }
                            }

                            //set root
                            if (index == 1)
                            {
                                if (route.Left(2) == "//" || CurrentRoute.Nodes.Count == 0) //root specified or no root yet
                                {
                                    var created = SetRoot(part, false, passArguments);
                                    if (created != null)
                                    {
                                        children.Add(new()
                                        {
                                            Part = part,
                                            Control = created
                                        });
                                    }
                                    else
                                    {
                                        children.Add(CurrentRoute.Nodes[existingIndex - 1]);
                                    }

                                    continue;
                                }
                                else
                                {

                                    if (parsed.Parts.Length == 1)
                                    {
                                        //just add to current route
                                        children.AddRange(CurrentRoute.Nodes);
                                        await AddRoutePart();
                                        break;
                                    }
                                    else
                                    {
                                        //otherwise navigate from root
                                        children.Add(CurrentRoute.Nodes[0]);
                                        existingIndex++;
                                    }
                                }
                            }

                            //check if part is same as existing at this level 

                            if (CurrentRoute.Nodes.Count >= existingIndex)
                            {
                                if (CurrentRoute.Nodes[existingIndex - 1].Part == part)
                                {
                                    children.Add(CurrentRoute.Nodes[existingIndex - 1]);
                                    continue;
                                }

                                //todo remove all existing pages from this level
                                List<ShellStackChild> remove = new();
                                for (int i = existingIndex - 1; i < CurrentRoute.Nodes.Count; i++)
                                {
                                    remove.Add(CurrentRoute.Nodes[i]);
                                }

                                await AddRoutePart();


                                foreach (var removeNode in remove)
                                {
                                    var removed = await RemoveAsync(removeNode.Control, false);
                                    if (removed != null)
                                    {
                                        existingIndex--;
                                    }
                                }
                            }


                        }

                        var backup = OrderedRoute;

                        OrderedRoute = route;
                        SetCurrentRouteNodes(children);



                        var args = new ShellNavigatedEventArgs(
                            previous: backup,
                            OrderedRoute,
                            ShellNavigationSource.Push);
                        this.Navigated?.Invoke(this, args);
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e);
                    }


                    return;
                }

            }

            Trace.WriteLine($"[FastShell] Unsupported URI {route}");
        }

        public event EventHandler<ShellNavigatedEventArgs> Navigated;

        public event EventHandler<ShellNavigatingEventArgs> Navigating;

        public void InvalidateNavBar()
        {
            OnNavBarInvalidated();

        }

        public virtual void OnNavBarInvalidated()
        {

        }

        public event EventHandler<RotationEventArgs> OnRotation;

        public event EventHandler<IndexArgs> TabReselected;
        public ObservableCollection<MenuPageItem> MenuItems { get; } = new ObservableCollection<MenuPageItem>();


        #endregion

        #region BUFFER

        /// <summary>
        /// Can use to pass items as models between viewmodels
        /// </summary>
        public static Dictionary<string, object> Buffer { get; } = new();

        #endregion

    }
}
