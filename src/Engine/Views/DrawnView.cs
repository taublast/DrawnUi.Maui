using DrawnUi.Maui.Infrastructure.Enums;
using DrawnUi.Maui.Infrastructure.Extensions;
using Microsoft.Maui.Controls.Internals;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Text;

namespace DrawnUi.Maui.Views
{

    [ContentProperty("Children")]
    public partial class DrawnView : ContentView, IDrawnBase, IAnimatorsManager, IVisualTreeElement
    {
        public virtual void Update()
        {
            if (!Super.EnableRendering)
                return;

#if ONPLATFORM
            UpdatePlatform();
#endif
        }

        public bool IsUsingHardwareAcceleration
        {
            get
            {
#if SKIA3
                return HardwareAcceleration != HardwareAccelerationMode.Disabled;
#else
                return HardwareAcceleration != HardwareAccelerationMode.Disabled
                       && DeviceInfo.Platform != DevicePlatform.WinUI
                       && DeviceInfo.Platform != DevicePlatform.MacCatalyst;
#endif
            }
        }

        public bool NeedRedraw { get; set; }

        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }

            set
            {
                if (_isDirty != value)
                {
                    if (!value && UpdateMode == UpdateMode.Constant)
                    {
                        value = true;
                    }
                    _isDirty = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _isDirty;


        public Queue<IDisposable> ToBeDisposed { get; } = new();

        public virtual bool IsVisibleInViewTree()
        {
            return IsVisible; //todo
        }

        public void TakeScreenShot(Action<SKImage> callback)
        {
            CallbackScreenshot = callback;
            Update();
        }

        public virtual void InvalidateByChild(SkiaControl child)
        {
            Invalidate();
        }

        public ScaledRect GetOnScreenVisibleArea()
        {
            var bounds = new SKRect(0, 0, (int)(Width * RenderingScale), (int)(Height * RenderingScale));

            return ScaledRect.FromPixels(bounds, (float)RenderingScale);
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

#if ANDROID
            OnHandlerChangedInternal();
#endif

            if (Handler == null)
            {
                DestroySkiaView();

#if ONPLATFORM
                DisposePlatform();
#endif
            }
            else
            {
                CreateSkiaView();

#if ONPLATFORM
                SetupRenderingLoop();
#endif
            }

            HandlerWasSet?.Invoke(this, Handler != null);
            //InvalidateChildren(); //need clear gfx cache
        }

        public event EventHandler<bool> HandlerWasSet;

        protected virtual void TakeScreenShotInternal(SKSurface surface)
        {
            if (surface != null)
            {
                //Console.WriteLine($"[DrawnView] TakeScreenShotInternal ------------------------- START");
                surface.Canvas.Flush();
                CallbackScreenshot?.Invoke(surface.Snapshot());
                //Console.WriteLine($"[DrawnView] TakeScreenShotInternal ------------------------- END");
            }
            CallbackScreenshot = null;
        }

        /// <summary>
        /// Postpone the action to be executed before the next frame being drawn. Exception-safe.
        /// </summary>
        /// <param name="action"></param>
        public void PostponeExecutionBeforeDraw(Action action)
        {
            ExecuteBeforeDraw.Enqueue(action);
        }

        /// <summary>
        ///Postpone the action to be executed after the current frame is drawn. Exception-safe.
        /// </summary>
        /// <param name="action"></param>
        public void PostponeExecutionAfterDraw(Action action)
        {
            ExecuteAfterDraw.Enqueue(action);
        }

        public Queue<Action> ExecuteBeforeDraw { get; } = new();
        public Queue<Action> ExecuteAfterDraw { get; } = new();

        protected Action<SKImage> CallbackScreenshot;

        protected Dictionary<SkiaControl, VisualTreeChain> RenderingTrees = new(128);

        public void RegisterRenderingChain(VisualTreeChain chain)
        {
            // Generate NodeIndices dictionary
            RenderingTrees.TryAdd(chain.Child, chain);

            //initial state
            for (int i = 0; i < chain.Nodes.Count; i++)
            {
                UpdateRenderingChains(chain.Nodes[i]);
            }
        }

        public void UnregisterRenderingChain(SkiaControl child)
        {
            RenderingTrees.Remove(child, out VisualTreeChain whatever);
        }

        public VisualTreeChain GetRenderingChain(SkiaControl child)
        {
            RenderingTrees.TryGetValue(child, out VisualTreeChain chain);
            return chain;
        }

        public void UpdateRenderingChains(SkiaControl node)
        {
            // Check each chain
            foreach (VisualTreeChain chain in RenderingTrees.Values)
            {
                // If the chain includes the node, update the chain's transform
                if (chain.NodeIndices.TryGetValue(node, out int index))
                {
                    // If the node is the root of the chain, reset the chain's transform
                    if (index == 0)
                    {
                        chain.Transform = new VisualTransform();
                    }

                    chain.Transform.IsVisible = chain.Transform.IsVisible && node.CanDraw;
                    var translation = new SKPoint((float)node.UseTranslationX, (float)node.UseTranslationY);
                    chain.Transform.Translation += translation;
                    chain.Transform.Opacity *= (float)node.Opacity;
                    chain.Transform.Rotation += (float)node.Rotation;
                    chain.Transform.Scale = new SKPoint((float)(chain.Transform.Scale.X * node.ScaleX), (float)(chain.Transform.Scale.Y * node.ScaleY));

                    //var log = $"{node.GetType().Name} {translation} | ";
                    //Debug.WriteLine(log);
                    //chain.Transform.Logs += log;
                    chain.Transform.RenderedNodes++;
                }
            }
        }

        IReadOnlyList<IVisualTreeElement> IVisualTreeElement.GetVisualChildren()
        {
            return Views.Cast<IVisualTreeElement>()
                .ToList();
        }

        public void RegisterGestureListener(ISkiaGestureListener gestureListener)
        {
            if (gestureListener == null)
                return;

            lock (LockIterateListeners)
            {
                gestureListener.GestureListenerRegistrationTime = DateTime.UtcNow;
                GestureListeners.Add(gestureListener);
            }
        }

        public void UnregisterGestureListener(ISkiaGestureListener gestureListener)
        {
            if (gestureListener == null)
                return;

            lock (LockIterateListeners)
            {
                GestureListeners.Remove(gestureListener);
            }
        }

        protected object LockIterateListeners = new();


        /// <summary>
        /// Children we should check for touch hits
        /// </summary>
        public SortedSet<ISkiaGestureListener> GestureListeners { get; } = new(new DescendingZIndexGestureListenerComparer());


        public SKRect DrawingRect
        {
            get
            {
                return new SKRect(0, 0, (float)(Width * RenderingScale), (float)(Height * RenderingScale));
            }
        }

        public void AddAnimator(ISkiaAnimator animator)
        {
            PostponeExecutionBeforeDraw(() =>
            {
                lock (LockAnimatingControls)
                {
                    animator.IsDeactivated = false;
                    AnimatingControls.TryAdd(animator.Uid, animator);
                }
            });

            Update();
        }

        public void RemoveAnimator(Guid uid)
        {
            lock (LockAnimatingControls)
            {
                if (AnimatingControls.TryGetValue(uid, out var animator))
                {
                    animator.IsDeactivated = true;
                }
            }
        }

        /// <summary>
        /// Called by a control that whats to be constantly animated or doesn't anymore,
        /// so we know whether we should refresh canvas non-stop 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="animating"></param>
        public bool RegisterAnimator(ISkiaAnimator animator)
        {
            AddAnimator(animator);

            return true;
        }

        public void UnregisterAnimator(Guid uid)
        {
            RemoveAnimator(uid);
        }

        public virtual IEnumerable<ISkiaAnimator> UnregisterAllAnimatorsByType(Type type)
        {
            lock (LockAnimatingControls)
            {
                var ret = AnimatingControls.Where(x => x.Value.GetType() == type).Select(s => s.Value).ToArray();
                foreach (var animator in ret)
                {
                    try
                    {
                        animator.IsDeactivated = true;
                    }
                    catch (Exception e)
                    {
                        Super.Log(e);
                    }
                }
                return ret;
            }
        }

        public virtual IEnumerable<ISkiaAnimator> UnregisterAllAnimatorsByParent(SkiaControl parent)
        {
            lock (LockAnimatingControls)
            {
                var ret = AnimatingControls.Values.Where(x => x.Parent == parent).ToArray();
                foreach (var animator in ret)
                {
                    try
                    {
                        animator.IsDeactivated = true;
                    }
                    catch (Exception e)
                    {
                        Super.Log(e);
                    }
                }
                return ret;
            }
        }

        public virtual IEnumerable<ISkiaAnimator> SetPauseStateOfAllAnimatorsByParent(SkiaControl parent, bool state)
        {
            lock (LockAnimatingControls)
            {
                var ret = AnimatingControls.Values.Where(x => x.Parent == parent).ToArray();
                foreach (var animator in ret)
                {
                    try
                    {
                        if (state)
                            animator.Pause();
                        else
                            animator.Resume();
                    }
                    catch (Exception e)
                    {
                        Super.Log(e);
                    }
                }
                return ret;
            }
        }

        public int ExecutePostAnimators(SkiaDrawingContext context, double scale)
        {
            var executed = 0;

            try
            {
                if (PostAnimators.Count == 0)
                    return executed;

                foreach (var skiaAnimation in PostAnimators)
                {
                    if (skiaAnimation.IsRunning && !skiaAnimation.IsPaused)
                    {
                        executed++;
                        var finished = skiaAnimation.TickFrame(context.FrameTimeNanos);
                        if (skiaAnimation is ICanRenderOnCanvas renderer)
                        {
                            var renderedrawn = renderer.Render(this, context, scale);
                        }

                        if (finished)
                        {
                            skiaAnimation.Stop();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Super.Log(e);
            }

            return executed;
        }

        protected object LockAnimatingControls = new();

        /// <summary>
        /// Executed after the rendering
        /// </summary>
        public List<IOverlayEffect> PostAnimators { get; } = new(128);

        List<Guid> _listRemoveAnimators = new(512);

        /// <summary>
        /// Tracking controls that what to be animated right now so we constantly refresh
        /// canvas until there is none left
        /// </summary>
        public Dictionary<Guid, ISkiaAnimator> AnimatingControls { get; } = new(512);

        protected int ExecuteAnimators(long nanos)
        {

            var executed = 0;

            //lock (LockAnimatingControls)
            {
                try
                {

                    if (AnimatingControls.Count == 0)
                        return executed;

                    _listRemoveAnimators.Clear();

                    foreach (var key in AnimatingControls.Keys)
                    {
                        var skiaAnimation = AnimatingControls[key];

                        if (skiaAnimation.IsDeactivated
                            || skiaAnimation.Parent != null && skiaAnimation.Parent.IsDisposed)
                        {
                            _listRemoveAnimators.Add(key);
                            continue;
                        }

                        bool canPlay = !(skiaAnimation.Parent != null && !skiaAnimation.Parent.IsVisibleInViewTree());

                        if (canPlay)
                        {
                            if (skiaAnimation.IsPaused)
                                skiaAnimation.Resume(); //continue anim from current time instead of the old one

                            skiaAnimation.TickFrame(nanos);
                            executed++;
                        }
                        else
                        {
                            if (!skiaAnimation.IsPaused)
                                skiaAnimation.Pause();
                        }
                    }

                    foreach (var key in _listRemoveAnimators)
                    {
                        AnimatingControls.Remove(key);
                    }

                }
                catch (Exception e)
                {
                    Super.Log(e);
                }

                return executed;
            }

        }

        public virtual void OnCanvasViewChanged()
        {
            Update();
        }

        public ISkiaDrawable CanvasView
        {
            get => _canvasView;
            protected set
            {
                if (Equals(value, _canvasView)) return;
                _canvasView = value;
                renderedFrames = 0;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanvasFps));
                OnPropertyChanged(nameof(CanDraw));
                OnCanvasViewChanged();
            }
        }

        private bool _initialized;
        private void Init()
        {
            if (!_initialized)
            {
                _initialized = true;

                HorizontalOptions = LayoutOptions.Start;
                VerticalOptions = LayoutOptions.Start;
                Padding = new Thickness(0);

                SizeChanged += ViewSizeChanged;

                //bug this creates garbage on aandroid on every frame
                // DeviceDisplay.Current.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
            }
        }

        private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            switch (e.DisplayInfo.Rotation)
            {
            case Microsoft.Maui.Devices.DisplayRotation.Rotation90:
            DeviceRotation = 90;
            break;
            case Microsoft.Maui.Devices.DisplayRotation.Rotation180:
            DeviceRotation = 180;
            break;
            case Microsoft.Maui.Devices.DisplayRotation.Rotation270:
            DeviceRotation = 270;
            break;
            case Microsoft.Maui.Devices.DisplayRotation.Rotation0:
            default:
            DeviceRotation = 0;
            break;
            }

            if (Parent != null)
                RenderingScale = (float)e.DisplayInfo.Density;
        }

        public void SetDeviceOrientation(int rotation)
        {
            DeviceRotation = rotation;
        }

        public event EventHandler<int> DeviceRotationChanged;

        public static readonly BindableProperty DisplayRotationProperty = BindableProperty.Create(
            nameof(DeviceRotation),
            typeof(int),
            typeof(DrawnView),
            0,
            propertyChanged: UpdateRotation);

        private static void UpdateRotation(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is DrawnView control)
            {
                control.DeviceRotationChanged?.Invoke(control, (int)newvalue); ;
            }
        }

        public int DeviceRotation
        {
            get { return (int)GetValue(DisplayRotationProperty); }
            set { SetValue(DisplayRotationProperty, value); }
        }

        //{
        //    HorizontalOptions = LayoutOptions.Fill,
        //    VerticalOptions = LayoutOptions.Fill
        //};

        public bool HasHandler { get; set; }

        public virtual void DisconnectedHandler()
        {
            HasHandler = false;
            Super.NeedGlobalRefresh -= OnNeedUpdate;
        }

        public long NeedGlobalRefreshCount { get; set; }

        private void OnNeedUpdate(object sender, EventArgs e)
        {
            NeedCheckParentVisibility = true;
            NeedGlobalRefreshCount++;
            Update();
        }

        public virtual void ConnectedHandler()
        {
            HasHandler = true;
            Super.NeedGlobalRefresh -= OnNeedUpdate;
            Super.NeedGlobalRefresh += OnNeedUpdate;
        }


        protected void FixDensity()
        {
            if (RenderingScale <= 0.0)
            {
                RenderingScale = (float)GetDensity();

                if (RenderingScale <= 0.0)
                {
                    RenderingScale = (float)(CanvasView.CanvasSize.Width / this.Width);
                }
                OnDensityChanged();
            }
        }

        /// <summary>
        /// Set this to true if you do not want the canvas to be redrawn as transparent and showing content below the canvas (splash?..) when UpdateLocked is True
        /// </summary>
        public bool StopDrawingWhenUpdateIsLocked { get; set; }


        public DateTime TimeDrawingStarted { get; protected set; }
        public DateTime TimeDrawingComplete { get; protected set; }

        volatile bool _isWaiting = false;

        public virtual void InvalidateViewport()
        {
            Update();
        }

        public virtual void Repaint()
        {
            Update();
        }


        public bool OrderedDraw { get; protected set; }


        double _lastUpdateTimeNanos;

        public void ResetUpdate()
        {
            NeedCheckParentVisibility = true;
            OrderedDraw = false;
            InvalidatedCanvas = 0;
        }

        /// <summary>
        /// A very important tracking prop to avoid saturating main thread with too many updates
        /// </summary>
        protected long InvalidatedCanvas { get; set; }

        public bool IsRendering { get; protected set; }

        protected Grid Delayed { get; set; }

        public static double GetDensity()
        {
            return Super.Screen.Density;
        }

        protected void SwapToDelayed()
        {
            if (Delayed != null)
            {
                var normal = Delayed.Children[0] as SkiaView;
                var accel = Delayed.Children[1] as SkiaViewAccelerated;
                var kill = CanvasView;
                kill.OnDraw = null;
                accel.OnDraw = OnDrawSurface;
                CanvasView = accel;
                Delayed = null;

                normal?.Disconnect();
            }
        }

        /// <summary>
        /// Will safely destroy existing if any
        /// </summary>
        protected void CreateSkiaView()
        {
            DestroySkiaView();

#if ONPLATFORM
            PlatformHardwareAccelerationChanged();
#endif

            if (IsUsingHardwareAcceleration)
            {
                if (HardwareAcceleration == HardwareAccelerationMode.Prerender)
                {
                    //create normal view, then after it has rendered 3 frames swap to the accelerated view, to avoid the blank screen when accelerated view is initializing (slow)
                    var pre = new SkiaView(this);
                    pre.OnDraw = OnDrawSurface;
                    CanvasView = pre;
                    var accel = new SkiaViewAccelerated(this);

                    var content = new Grid()
                    {

                    };
                    content.Children.Add(pre);
                    content.Children.Add(accel);
                    Content = content;
                    Delayed = content;
                    return;
                }
                else
                if (HardwareAcceleration == HardwareAccelerationMode.Enabled)
                {
                    var view = new SkiaViewAccelerated(this);
                    view.OnDraw = OnDrawSurface;
                    CanvasView = view;
                }
            }
            else
            {
                var view = new SkiaView(this);
                view.OnDraw = OnDrawSurface;
                CanvasView = view;
            }

            Content = CanvasView as View;
        }

        protected void DestroySkiaView()
        {
            var kill = CanvasView;
            if (kill != null)
                CanvasView = null;
            if (kill != null)
            {
                kill.OnDraw = null;
                kill.Dispose();
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            try
            {
                this.SizeChanged -= OnFormsSizeChanged;

                OnDisposing();

                IsDisposed = true;

                Parent = null;

                PaintSystem?.Dispose();

                DestroySkiaView();

                DisposeDisposables();

                ClearChildren();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        this.Handler?.DisconnectHandler();

                        Content = null;
                    }
                    catch (Exception e)
                    {
                        Super.Log(e);
                    }
                });
            }
            catch (Exception e)
            {
                Super.Log(e);
            }

        }
        /// <summary>
        /// Makes the control dirty, in need to be remeasured and rendered but this doesn't call Update, it's up yo you
        /// </summary>
        public virtual void Invalidate()
        {
            NeedMeasure = true;

            //InvalidateChildren();
        }

        public void InvalidateParents()
        {
            Invalidate();
        }

        public virtual void OnSuperviewShouldRenderChanged(bool state)
        {
            foreach (var view in Views) //will crash?
            {
                view.OnSuperviewShouldRenderChanged(state);
            }
        }

        /// <summary>
        /// We need to invalidate children maui changed our storyboard size
        /// </summary>
        public void InvalidateChildren()
        {

            foreach (var view in Views)
            {
                view.InvalidateWithChildren();
            }

            NeedMeasure = true;

            Update();
        }


        public void PrintDebug(string indent = "")
        {
            Console.WriteLine($"{indent}└─ {GetType().Name} {Width:0.0},{Height:0.0} pts");
            foreach (var view in Views)
            {
                //Console.Write($"{indent}    ├─ ");
                view.PrintDebug(indent);
            }
        }


        public bool NeedAutoSize
        {
            get
            {
                return NeedAutoHeight || NeedAutoWidth;
            }
        }

        public bool NeedAutoHeight
        {
            get
            {
                return VerticalOptions.Alignment != LayoutAlignment.Fill && HeightRequest < 0;
            }
        }
        public bool NeedAutoWidth
        {
            get
            {
                return HorizontalOptions.Alignment != LayoutAlignment.Fill && WidthRequest < 0;
            }
        }

        public virtual bool ShouldInvalidateByChildren
        {
            get
            {
                return NeedAutoSize;
            }
        }

        public static readonly BindableProperty UpdateLockedProperty = BindableProperty.Create(
            nameof(UpdateLocked),
            typeof(bool),
            typeof(DrawnView),
            false);

        public bool UpdateLocked
        {
            get { return (bool)GetValue(UpdateLockedProperty); }
            set { SetValue(UpdateLockedProperty, value); }
        }




        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);


            if (propertyName == nameof(HeightRequest)
                || propertyName == nameof(WidthRequest)
                || propertyName == nameof(Padding)
                || propertyName == nameof(Margin)
                || propertyName == nameof(HorizontalOptions)
                || propertyName == nameof(VerticalOptions)
                )
            {
                Invalidate();
            }
        }

        public Guid Uid { get; } = Guid.NewGuid();

        public static (double X1, double Y1, double X2, double Y2) LinearGradientAngleToPoints(double direction)
        {
            //adapt to css style
            direction -= 90;

            //allow negative angles
            if (direction < 0)
                direction = 360 + direction;

            if (direction > 360)
                direction = 360;

            (double x, double y) pointOfAngle(double a)
            {
                return (x: Math.Cos(a), y: Math.Sin(a));
            };

            double degreesToRadians(double d)
            {
                return ((d * Math.PI) / 180);
            }

            var eps = Math.Pow(2, -52);
            var angle = (direction % 360);
            var startPoint = pointOfAngle(degreesToRadians(180 - angle));
            var endPoint = pointOfAngle(degreesToRadians(360 - angle));

            if (startPoint.x <= 0 || Math.Abs(startPoint.x) <= eps)
                startPoint.x = 0;

            if (startPoint.y <= 0 || Math.Abs(startPoint.y) <= eps)
                startPoint.y = 0;

            if (endPoint.x <= 0 || Math.Abs(endPoint.x) <= eps)
                endPoint.x = 0;

            if (endPoint.y <= 0 || Math.Abs(endPoint.y) <= eps)
                endPoint.y = 0;

            return (startPoint.x, startPoint.y, endPoint.x, endPoint.y);
        }


        public virtual void OnDensityChanged()
        {
            Update(); //todo for children!!!!!
        }


        #region DISPOSE BY XAMARIN FORMS

        //private bool _isRendererSet;
        //protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    base.OnPropertyChanged(propertyName);
        //    if (propertyName == "Renderer")
        //    {
        //        _isRendererSet = !_isRendererSet;
        //        if (!_isRendererSet)
        //        {
        //            Dispose();
        //            Debug.WriteLine("SkiaBaseView disposed!");
        //        }
        //        else
        //        {
        //            IsDisposed = false;
        //        }
        //    }
        //}

        #endregion

        private bool _IsGhost;
        public bool IsGhost
        {
            get { return _IsGhost; }
            set
            {
                if (_IsGhost != value)
                {
                    _IsGhost = value;
                    OnPropertyChanged();
                }
            }
        }

        private void ViewSizeChanged(object sender, EventArgs e)
        {
            //xamarin forms changed our size if used inside xamarin layout
            OnSizeChanged();
        }

        public DrawnView()
        {
            Init();
        }

        public Action<SKPath, SKRect> Clipping { get; set; }

        public virtual SKPath CreateClip(object arguments, bool usePosition)
        {
            var path = new SKPath();
            if (usePosition)
            {
                path.AddRect(DrawingRect);
            }
            else
            {
                path.AddRect(new(0, 0, DrawingRect.Width, DrawingRect.Height));
            }
            return path;
        }

        public string Tag { get; set; }

        public bool IsRootView(float width, float height, SKRect destination)
        {
            if (this.Parent != null)
            {
                return true; //Xamarin/MAUI
            }

            return this.Parent == null && destination.Width == width && destination.Height == height;
        }

        //-------------------------------------------------------------
        /// <summary>
        ///  destination in PIXELS, requests in UNITS. resulting Destination prop will be filed in PIXELS.
        /// </summary>
        /// <param name="destination">PIXELS</param>
        /// <param name="widthRequest">UNITS</param>
        /// <param name="heightRequest">UNITS</param>
        /// <param name="scale"></param>
        public SKRect CalculateLayout(SKRect destination, double widthRequest,
                double heightRequest, double scale = 1.0)
        //-------------------------------------------------------------
        {
            var scaledOffsetMargin = 0;

            var rectWidth = destination.Width - scaledOffsetMargin * 2;
            var wants = Math.Round(widthRequest * scale);
            if (wants > 0 && wants < rectWidth)
                rectWidth = (int)wants;

            var rectHeight = destination.Height - scaledOffsetMargin * 2;
            wants = Math.Round(heightRequest * scale);
            if (wants > 0 && wants < rectHeight)
                rectHeight = (int)wants;

            var availableWidth = destination.Width - scaledOffsetMargin * 2;
            var availableHeight = destination.Height - scaledOffsetMargin * 2;

            var layoutHorizontal = new LayoutOptions(HorizontalOptions.Alignment, HorizontalOptions.Expands);
            var layoutVertical = new LayoutOptions(VerticalOptions.Alignment, VerticalOptions.Expands);


            //todo sensor rotation

            //initial fill
            var left = destination.Left + scaledOffsetMargin;
            var top = destination.Top + scaledOffsetMargin;
            var right = left + rectWidth;
            var bottom = top + rectHeight;


            //layoutHorizontal
            if (layoutHorizontal.Alignment == LayoutAlignment.Center)
            {
                //center
                left += (availableWidth - rectWidth) / 2.0f;
                right = left + rectWidth;

                if (left < destination.Left)
                {
                    left = (float)(destination.Left);
                    right = left + rectWidth;
                }

                if (right > destination.Right)
                {
                    right = (float)(destination.Right);
                }
            }
            else
            if (layoutHorizontal.Alignment == LayoutAlignment.End)
            {
                //end
                right = destination.Right;
                left = right - rectWidth;
                if (left < destination.Left)
                {
                    left = (float)(destination.Left);
                }
            }
            else
            {


                //start or fill
                right = left + rectWidth;
                if (right > destination.Right)
                {
                    right = (float)(destination.Right);
                }
            }

            //VerticalOptions
            if (layoutVertical.Alignment == LayoutAlignment.Center)
            {
                //center
                top += availableHeight / 2.0f - rectHeight / 2.0f;
                bottom = top + rectHeight;
                if (top < destination.Top)
                {
                    top = (float)(destination.Top);
                    bottom = top + rectHeight;
                }
                else
                if (bottom > destination.Bottom)
                {
                    bottom = (float)(destination.Bottom);
                    top = bottom - rectHeight;
                }
            }
            else
            if (layoutVertical.Alignment == LayoutAlignment.End && double.IsFinite(destination.Bottom))
            {
                //end
                bottom = destination.Bottom;
                top = bottom - rectHeight;
                if (top < destination.Top)
                {
                    top = (float)(destination.Top);
                }

            }
            else
            {
                //start or fill
                bottom = top + rectHeight;
                if (bottom > destination.Bottom)
                {
                    bottom = (float)(destination.Bottom);
                }

            }

            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }


        //-------------------------------------------------------------
        /// <summary>
        ///  destination in PIXELS, requests in UNITS. resulting Destination prop will be filed in PIXELS.
        /// </summary>
        /// <param name="destination">PIXELS</param>
        /// <param name="widthRequest">UNITS</param>
        /// <param name="heightRequest">UNITS</param>
        /// <param name="scale"></param>
        public virtual void Arrange(SKRect destination, double widthRequest, double heightRequest, double scale = 1.0)
        //-------------------------------------------------------------
        {


            Destination = CalculateLayout(destination, widthRequest, heightRequest, scale);

            if (Destination.Width < 0 || Destination.Height < 0)
            {
                var stop = true;
            }
        }

        public ScaledSize MeasuredSize { get; set; } = new();



        public virtual ScaledSize Measure(float widthConstraintPts, float heightConstraintPts)
        {
            if (!IsVisible)
            {
                return SetMeasured(0, 0, (float)RenderingScale);
            }

            if (widthConstraintPts < 0 || heightConstraintPts < 0)
            {
                //not setting NeedMeasure=false;
                return ScaledSize.Empty;
            }

            var widthPixels = (float)((WidthRequest));
            var heightPixels = (float)((HeightRequest));

            if (WidthRequest > 0 && widthPixels < widthConstraintPts)
                widthConstraintPts = widthPixels;

            if (HeightRequest > 0 && heightPixels < heightConstraintPts)
                heightConstraintPts = heightPixels;

            return SetMeasured(widthConstraintPts, heightConstraintPts, (float)RenderingScale);
        }

        protected virtual bool NeedMeasure { get; set; } = true;

        protected ScaledSize SetMeasured(float width, float height, float scale)
        {
            NeedMeasure = false;

            if (!double.IsNaN(height))
            {
                //Height = height / scale;
            }
            else
            {
                height = -1;
                //Height = height;
            }

            if (!double.IsNaN(width))
            {
                //Width = width / scale;
            }
            else
            {
                width = -1;
                //Width = width;
            }

            MeasuredSize = ScaledSize.FromUnits(width, height, scale);

            //SetValueCore(RenderingScaleProperty, scale, SetValueFlags.None);

            OnMeasured();

            return MeasuredSize;
        }

        protected virtual void OnMeasured()
        {
            Measured?.Invoke(this, MeasuredSize);
        }

        public event EventHandler<ScaledSize> Measured;

        private void OnFormsSizeChanged(object sender, EventArgs e)
        {
            Update();
        }

        public bool IsDisposed { get; protected set; }



        SkiaDrawingContext CreateContext(SKCanvas canvas)
        {
            return new SkiaDrawingContext()
            {
                Superview = this,
                FrameTimeNanos = CanvasView.FrameTime,
                Canvas = canvas,
                Width = canvas.DeviceClipBounds.Width,
                Height = canvas.DeviceClipBounds.Height
            };
        }

        /// <summary>
        /// Can use this to manage double buffering to detect if we are in the drawing thread or in background.
        /// </summary>
        public int DrawingThreadId { get; protected set; }

        protected bool WasRendered { get; set; }

        public event EventHandler<SkiaDrawingContext?> WasDrawn;
        public event EventHandler<SkiaDrawingContext?> WillDraw;
        public event EventHandler<SkiaDrawingContext?> WillFirstTimeDraw;

        private bool OnDrawSurface(SKCanvas canvas, SKRect rect)
        {
            //lock (LockDraw)
            {

                if (!OnStartRendering(canvas))
                    return UpdateMode == UpdateMode.Constant;

                try
                {
                    if (NeedMeasure)
                    {
                        FixDensity();
                    }

                    var args = CreateContext(canvas);

                    this.DrawingThreadId = Thread.CurrentThread.ManagedThreadId;

                    if (!WasRendered)
                    {
                        WillFirstTimeDraw?.Invoke(this, args);
                    }

                    WillDraw?.Invoke(this, null);

                    Draw(args, rect, (float)RenderingScale);
                }
                finally
                {
                    OnFinalizeRendering();

                    WasRendered = true;
                }

                return IsDirty;
            }

        }



        protected virtual bool OnStartRendering(SKCanvas canvas)
        {
            var monitor = InvalidatedCanvas;
            monitor--;
            if (monitor < 0)
            {
                monitor = 0;
            }
            InvalidatedCanvas = monitor;
            OrderedDraw = false;

            if (!CanDraw || canvas == null)
                return false;

            TimeDrawingStarted = DateTime.Now;
            IsRendering = true;

            IsDirty = false; //any control can set to true after that

            return true;
        }

        protected virtual void OnFinalizeRendering()
        {

            TimeDrawingComplete = DateTime.Now;

            IsRendering = false;

            if (UpdateMode == UpdateMode.Constant)
                IsDirty = true;

            if (IsDirty)
            {
                Update();
            }

            //track and cap queued updates
            var monitor = InvalidatedCanvas;
            monitor--;
            if (monitor < 0)
            {
                monitor = 0;
            }
            InvalidatedCanvas = monitor;

            WasDrawn?.Invoke(this, null);
        }



        public virtual void OnDisposing()
        {
#if ONPLATFORM
            DisposePlatform();
#endif

            DeviceDisplay.Current.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
        }



        //public virtual void Render(SkiaDrawingContext context, SKRect destination, float scale,
        //    )
        //{
        //    AvailableDestination = destination;
        //    Draw(context, destination, scale);
        //}
        public SKRect AvailableDestination { get; set; }


        /// <summary>
        /// will be reset to null by InvalidateViewsList()
        /// </summary>
        private IReadOnlyList<SkiaControl> _orderedChildren;

        /// <summary>
        /// For non templated simple subviews
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<SkiaControl> GetOrderedSubviews()
        {
            if (_orderedChildren == null)
            {
                _orderedChildren = Views.OrderBy(x => x.ZIndex).ToList();
            }
            return _orderedChildren;
        }

        /// <summary>
        /// To make GetOrderedSubviews() regenerate next result instead of using cached
        /// </summary>
        public void InvalidateViewsList()
        {
            _orderedChildren = null;
        }

        #region FPS

        private double _fpsAverage;
        private int _fpsCount;
        protected double _fps;

        /// <summary>
        /// Frame started rendering nanoseconds
        /// </summary>
        public long FrameTime { get; protected set; }

        /// <summary>
        /// Actual FPS
        /// </summary>
        public double CanvasFps
        {
            get
            {
                if (CanvasView == null)
                    return 0.0;
                return CanvasView.FPS;
            }
        }

        /// <summary>
        /// Average FPS
        /// </summary>
        public double FPS { get; protected set; }

        #endregion

        protected object LockDraw = new();

        long renderedFrames;

        public virtual void DisposeDisposables()
        {
            try
            {
                while (ToBeDisposed.TryDequeue(out var disposable))
                {
                    disposable?.Dispose();
                }
            }
            catch (Exception e)
            {
                Super.Log("****************************************************");
                Super.Log(e);
                Super.Log("****************************************************");
                throw e;
            }
        }

        /// <summary>
        /// For debugging purposes check if dont have concurrent threads
        /// </summary>
        public int DrawingThreads { get; protected set; }

        protected virtual void Draw(SkiaDrawingContext context, SKRect destination, float scale)
        {
            ++renderedFrames;


            //if (CanvasView is SkiaViewAccelerated accelerated)
            //{
            //    var c = accelerated.GRContext;
            //    Console.WriteLine($"[FRAME] {++renderedFrames} {c} {destination.Width}x{destination.Height} at {scale}");
            //}

            DisposeDisposables();

            //Trace.WriteLine($"[1] {destination.Width}x{destination.Height} at {scale}");

            if (IsDisposed || UpdateLocked
                           //|| Super.StopRenderingInBackground
                           )
            {
                return;
            }


            {


                if (NeedGlobalRefreshCount != _globalRefresh)
                {
                    _globalRefresh = NeedGlobalRefreshCount;
                    foreach (var item in Children)
                    {
                        item.InvalidateMeasureInternal();
                    }
                }

                try
                {
                    DrawingThreads++;

                    FrameTime = CanvasView.FrameTime;
                    //context.FrameTimeNanos = FrameTime;

                    FPS = CanvasFps;

                    while (ExecuteBeforeDraw.TryDequeue(out Action action))
                    {
                        try
                        {
                            action?.Invoke();
                        }
                        catch (Exception e)
                        {
                            Super.Log(e);
                        }
                    }

                    var executed = ExecuteAnimators(context.FrameTimeNanos);

                    if (this.Width > 0 && this.Height > 0)
                    {
                        // ABSOLUTE like inside grid
                        var children = GetOrderedSubviews();

                        foreach (var child in children)
                        {
                            child.OnBeforeDraw(); //could set IsVisible or whatever inside
                            if (child.CanDraw) //still visible
                            {
                                var rectForChild = new SKRect(
                                    destination.Left + (float)Math.Round((Padding.Left) * scale),
                                    destination.Top + (float)Math.Round((Padding.Top) * scale),
                                    destination.Right - (float)Math.Round((Padding.Right) * scale),
                                    destination.Bottom - (float)Math.Round((Padding.Bottom) * scale));
                                child.Render(context, rectForChild, (float)scale);
                            }
                        }

                        //notify registered tree final nodes of rendering tree state
                        foreach (var tree in RenderingTrees)
                        {
                            //todo what was this case? disabled as bugging
                            //if (tree.Value.Nodes.Count != tree.Value.Transform.RenderedNodes)
                            //{
                            //tree.Value.Transform.IsVisible = false;
                            //}
                            tree.Key.SetVisualTransform(tree.Value.Transform);
                        }

                        var postExecuted = ExecutePostAnimators(context, scale);

                        //Kick to redraw if need animate
                        if (executed + postExecuted > 0)
                        {
                            IsDirty = true;
                        }

                        if (CallbackScreenshot != null)
                        {
                            TakeScreenShotInternal(context.Superview.CanvasView.Surface);
                        }
                    }

                    while (ExecuteAfterDraw.TryDequeue(out Action action))
                    {
                        try
                        {
                            action?.Invoke();
                        }
                        catch (Exception e)
                        {
                            Super.Log($"[DrawnView] Handled ExecuteAfterDraw: {e}");
                        }
                    }


                    if (renderedFrames is >= 3 and < 5 && HardwareAcceleration == HardwareAccelerationMode.Prerender)
                    {
                        //looks like we have finally loaded
                        SwapToDelayed();
                    }

                }
                catch (Exception e)
                {
                    Super.Log(e); //most probably data was modified while drawing

                    NeedMeasure = true;
                    IsDirty = true;
                }
                finally
                {
                    DrawingThreads--;
                }
            }


        }




        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(DrawnView),
            Colors.Transparent,
            propertyChanged: RedrawCanvas);
        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }

        public static readonly BindableProperty ClearColorProperty = BindableProperty.Create(nameof(ClearColor), typeof(Color), typeof(DrawnView),
            Colors.Transparent,
            propertyChanged: RedrawCanvas);
        public Color ClearColor
        {
            get { return (Color)GetValue(ClearColorProperty); }
            set { SetValue(ClearColorProperty, value); }
        }

        public static readonly BindableProperty RenderingScaleProperty = BindableProperty.Create(nameof(RenderingScale), typeof(float), typeof(DrawnView),
            -1.0f,
            propertyChanged: RedrawCanvas);
        public float RenderingScale
        {
            get
            {
                var value = (float)GetValue(RenderingScaleProperty);
                if (value < 0.1)
                {
                    return (float)GetDensity();
                }
                return value;
            }
            set
            {
                SetValue(RenderingScaleProperty, value);
            }
        }


        private static void OnHardwareModeChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is DrawnView control && control.Handler != null)
            {
                control.CreateSkiaView();
            }
        }

        public static readonly BindableProperty HardwareAccelerationProperty = BindableProperty.Create(nameof(HardwareAcceleration),
        typeof(HardwareAccelerationMode),
        typeof(DrawnView),
        HardwareAccelerationMode.Disabled,
        propertyChanged: OnHardwareModeChanged);

        public HardwareAccelerationMode HardwareAcceleration
        {
            get { return (HardwareAccelerationMode)GetValue(HardwareAccelerationProperty); }
            set { SetValue(HardwareAccelerationProperty, value); }
        }

        public static readonly BindableProperty CanRenderOffScreenProperty = BindableProperty.Create(nameof(CanRenderOffScreen),
        typeof(bool),
        typeof(DrawnView),
        false);
        /// <summary>
        /// If this is check you view will be refreshed even offScreen or hidden
        /// </summary>
        public bool CanRenderOffScreen
        {
            get { return (bool)GetValue(CanRenderOffScreenProperty); }
            set { SetValue(CanRenderOffScreenProperty, value); }
        }


        /// <summary>
        /// Indicates that it is allowed to be rendered by engine, internal use
        /// </summary>
        /// <returns></returns>
        public bool CanDraw
        {
            get
            {
                var canRenderOffScreen = !IsHiddenInViewTree || CanRenderOffScreen;
                return CanvasView != null && !IsDisposed && IsVisible && Handler != null && canRenderOffScreen;
            }
        }

        /// <summary>
        /// Indicates that view is either hidden or offscreen.
        /// This disables rendering if you don't set CanRenderOffScreen to true
        /// </summary>
        public bool IsHiddenInViewTree { get; protected set; }


        public bool NeedCheckParentVisibility
        {
            get
            {
                return _needCheckParentVisibility;
            }

            set
            {
                if (_needCheckParentVisibility != value)
                {
                    _needCheckParentVisibility = value;
                    OnPropertyChanged();
                    OnSuperviewShouldRenderChanged(value); //maybe use just event handler???
                }
            }
        }
        bool _needCheckParentVisibility;
        private long _globalRefresh;



        public static MemoryStream StreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }


        protected SKPaint PaintSystem { get; set; }

        public SKRect Destination { get; protected set; }

        public void PaintTintBackground(SKCanvas canvas)
        {
            if (TintColor != null && TintColor != Colors.Transparent)
            {
                if (PaintSystem == null)
                {
                    PaintSystem = new SKPaint();
                }
                PaintSystem.Style = SKPaintStyle.StrokeAndFill;
                PaintSystem.Color = TintColor.ToSKColor();
                canvas.DrawRect(Destination, PaintSystem);
            }
        }

        public void PaintClearBackground(SKCanvas canvas)
        {
            if (ClearColor != Colors.Transparent)
            {
                if (PaintSystem == null)
                {
                    PaintSystem = new SKPaint();
                }
                PaintSystem.Style = SKPaintStyle.StrokeAndFill;
                PaintSystem.Color = ClearColor.ToSKColor();
                canvas.DrawRect(Destination, PaintSystem);
            }
        }

        //static int countRedraws = 0;
        protected static void RedrawCanvas(BindableObject bindable, object oldvalue, object newvalue)
        {

            var control = bindable as DrawnView;
            {
                if (control != null && !control.IsDisposed)
                {
                    control.Update();
                }
            }
        }


        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            foreach (var view in this.Views)
            {
                view.BindingContext = BindingContext;
            }
        }

        #region GRADIENTS

        public SKShader CreateGradientAsShader(SKRect destination, SkiaGradient gradient)
        {
            if (gradient != null && gradient.Type != GradientType.None)
            {
                var colors = new List<SKColor>();
                foreach (var color in gradient.Colors)
                {
                    var usingColor = color;
                    if (gradient.Light < 1.0)
                    {
                        usingColor = usingColor.MakeDarker(100 - gradient.Light * 100);
                    }
                    else if (gradient.Light > 1.0)
                    {
                        usingColor = usingColor.MakeLighter(gradient.Light * 100 - 100);
                    }

                    var newAlpha = usingColor.Alpha * gradient.Opacity;
                    usingColor = usingColor.WithAlpha(newAlpha);
                    colors.Add(usingColor.ToSKColor());
                }

                float[] colorPositions = null;
                if (gradient.ColorPositions?.Count == colors.Count)
                {
                    colorPositions = gradient.ColorPositions.Select(x => (float)x).ToArray();
                }

                switch (gradient.Type)
                {
                case GradientType.Sweep:

                return SKShader.CreateSweepGradient(
                     new SKPoint(destination.Left + destination.Width / 2.0f,
                        destination.Top + destination.Height / 2.0f),
                    colors.ToArray(),
                    colorPositions,
                    gradient.TileMode, (float)Value1, (float)(Value1 + Value2));

                case GradientType.Circular:
                return SKShader.CreateRadialGradient(
                    new SKPoint(destination.Left + destination.Width / 2.0f,
                        destination.Top + destination.Height / 2.0f),
                    Math.Max(destination.Width, destination.Height) / 2.0f,
                    colors.ToArray(),
                    colorPositions,
                    gradient.TileMode);

                case GradientType.Linear:
                default:
                return SKShader.CreateLinearGradient(
                    new SKPoint(destination.Left + destination.Width * gradient.StartXRatio,
                        destination.Top + destination.Height * gradient.StartYRatio),
                    new SKPoint(destination.Left + destination.Width * gradient.EndXRatio,
                        destination.Top + destination.Height * gradient.EndYRatio),
                    colors.ToArray(),
                    colorPositions,
                    gradient.TileMode);
                break;
                }

            }

            return null;
        }



        #endregion

        #region SUBVIEWS

        public List<SkiaControl> Views { get; } = new();

        public virtual void ClearChildren()
        {
            foreach (var child in Views.ToList())
            {
                RemoveSubView(child);
                child.Dispose();
            }

            Views.Clear();
            InvalidateViewsList();
        }

        public virtual void SetChildren(IEnumerable<SkiaControl> views)
        {
            ClearChildren();
            foreach (var child in views)
            {
                AddOrRemoveView(child, true);
            }
        }

        public void AddSubView(SkiaControl control)
        {
            if (control == null)
                return;
            control.SetParent(this);
            OnChildAdded(control);
            //if (Debugger.IsAttached)
            ReportHotreloadChildAdded(control);
        }

        public virtual void ReportHotreloadChildAdded(SkiaControl child)
        {
            if (child == null)
                return;

            var index = Views.FindIndex(child);
            VisualDiagnostics.OnChildAdded(this, child, index);
        }

        public void RemoveSubView(SkiaControl control)
        {
            if (control == null)
                return;

            //if (Debugger.IsAttached)
            ReportHotreloadChildRemoved(control);

            control.SetParent(null);
            OnChildRemoved(control);
        }

        public virtual void ReportHotreloadChildRemoved(SkiaControl control)
        {
            if (control == null)
                return;

            var index = Views.FindIndex(control);
            VisualDiagnostics.OnChildRemoved(this, control, index);
        }

        protected virtual void OnChildAdded(SkiaControl child)
        {
            InvalidateViewsList();
        }

        protected virtual void OnChildRemoved(SkiaControl child)
        {
            InvalidateViewsList();
        }



        #endregion

        #region Children

#pragma warning disable NU1605, CS0108


        public static readonly BindableProperty ChildrenProperty = BindableProperty.Create(
            nameof(Children),
            typeof(IList<SkiaControl>),
            typeof(DrawnView),
            defaultValueCreator: (instance) =>
            {
                var created = new ObservableCollection<SkiaControl>();
                ChildrenPropertyChanged(instance, null, created);
                return created;
            },
            validateValue: (bo, v) => v is IList<SkiaControl>,
            propertyChanged: ChildrenPropertyChanged);


        public IList<SkiaControl> Children
        {
            get => (IList<SkiaControl>)GetValue(ChildrenProperty);
            set => SetValue(ChildrenProperty, value);
        }

#pragma warning restore NU1605, CS0108

        protected void AddOrRemoveView(SkiaControl subView, bool add)
        {
            if (subView != null)
            {
                if (add)
                {
                    AddSubView(subView);
                    subView.BindingContext = this.BindingContext;
                }
                else
                {
                    RemoveSubView(subView);
                    subView.BindingContext = null;
                    //subView.Dispose(); ?????
                }

            }
        }

        private static void ChildrenPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is DrawnView skiaControl)
            {
                var enumerableChildren = (IEnumerable<SkiaControl>)newvalue;

                if (oldvalue != null)
                {
                    var oldViews = (IEnumerable<SkiaControl>)oldvalue;

                    if (oldvalue is INotifyCollectionChanged oldCollection)
                    {
                        oldCollection.CollectionChanged -= skiaControl.OnChildrenCollectionChanged;
                    }

                    foreach (var subView in oldViews)
                    {
                        skiaControl.AddOrRemoveView(subView, false);
                    }
                }

                //foreach (var subView in enumerableChildren)
                //{
                //	skiaControl.SetChildren(enumerableChildren);
                //}
                skiaControl.SetChildren(enumerableChildren);

                if (newvalue is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged += skiaControl.OnChildrenCollectionChanged;
                }

                skiaControl.Update();

            }

        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case NotifyCollectionChangedAction.Add:
            foreach (SkiaControl newChildren in e.NewItems)
            {
                newChildren.SetParent(this);
            }

            break;

            case NotifyCollectionChangedAction.Reset:
            case NotifyCollectionChangedAction.Remove:
            foreach (SkiaControl oldChildren in e.OldItems ?? new SkiaControl[0])
            {
                oldChildren.SetParent(null);
            }

            break;
            }

            Update();
        }

        #endregion

        public static readonly BindableProperty UpdateModeProperty = BindableProperty.Create(
            nameof(UpdateMode),
            typeof(UpdateMode),
            typeof(DrawnView),
            UpdateMode.Dynamic,
            propertyChanged: ChangeUpdateMode);

        private static void ChangeUpdateMode(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is DrawnView control)
            {
                control.Update();
            }
        }

        public UpdateMode UpdateMode
        {
            get { return (UpdateMode)GetValue(UpdateModeProperty); }
            set { SetValue(UpdateModeProperty, value); }
        }

        public static readonly BindableProperty ClipEffectsProperty = BindableProperty.Create(
            nameof(ClipEffects),
            typeof(bool),
            typeof(DrawnView),
            true);

        public bool ClipEffects
        {
            get { return (bool)GetValue(ClipEffectsProperty); }
            set { SetValue(ClipEffectsProperty, value); }
        }

        public static readonly BindableProperty Value1Property = BindableProperty.Create(
            nameof(Value1),
            typeof(double),
            typeof(DrawnView),
            0.0,
            propertyChanged: RedrawCanvas);

        public double Value1
        {
            get { return (double)GetValue(Value1Property); }
            set { SetValue(Value1Property, value); }
        }

        public static readonly BindableProperty Value2Property = BindableProperty.Create(
            nameof(Value2),
            typeof(double),
            typeof(DrawnView),
            0.0,
            propertyChanged: RedrawCanvas);

        public double Value2
        {
            get { return (double)GetValue(Value2Property); }
            set { SetValue(Value2Property, value); }
        }

        public static readonly BindableProperty Value3Property = BindableProperty.Create(
            nameof(Value3),
            typeof(double),
            typeof(DrawnView),
            0.0,
            propertyChanged: RedrawCanvas);

        public double Value3
        {
            get { return (double)GetValue(Value3Property); }
            set { SetValue(Value3Property, value); }
        }

        public static readonly BindableProperty Value4Property = BindableProperty.Create(
            nameof(Value4),
            typeof(double),
            typeof(DrawnView),
            0.0,
            propertyChanged: RedrawCanvas);

        public double Value4
        {
            get { return (double)GetValue(Value4Property); }
            set { SetValue(Value4Property, value); }
        }


        private bool _FocusLocked;
        public bool FocusLocked
        {
            get
            {
                return _FocusLocked;
            }
            set
            {
                if (_FocusLocked != value)
                {
                    _FocusLocked = value;
                    OnPropertyChanged();
                }
            }
        }

        public event EventHandler<FocusedItemChangedArgs> FocusedItemChanged;

        public class FocusedItemChangedArgs : EventArgs
        {
            public FocusedItemChangedArgs(SkiaControl item, bool isFocused)
            {
                Item = item;
                IsFocused = isFocused;
            }

            public bool IsFocused { get; set; }

            public SkiaControl Item { get; set; }
        }

        public ISkiaGestureListener FocusedChild
        {
            get
            {
                return _focusedChild;
            }

            set
            {
                if (_focusedChild != value && !FocusLocked)
                {
                    if (_focusedChild != null)
                    {
                        //Debug.WriteLine($"[UNFOCUSED] {_focusedChild}");
                        _focusedChild.OnFocusChanged(false);
                        FocusedItemChanged?.Invoke(this, new(_focusedChild as SkiaControl, false));
                    }
                    _focusedChild = value;
                    if (_focusedChild != null)
                    {
                        //Debug.WriteLine($"[FOCUSED] {_focusedChild}");
                        _focusedChild.OnFocusChanged(true);
                        FocusedItemChanged?.Invoke(this, new(_focusedChild as SkiaControl, true));
                    }
                    else
                    {
                        this.Focus();
                        TouchEffect.CloseKeyboard();
                        FocusedItemChanged?.Invoke(this, new(null, false));
                    }
                    //Debug.WriteLine($"[FOCUSED] {value}");

                    OnPropertyChanged();
                }
            }
        }
        ISkiaGestureListener _focusedChild;
        private ISkiaDrawable _canvasView;

#if !ONPLATFORM

        public void CheckElementVisibility(Element element)
        {
            NeedCheckParentVisibility = false;
        }

        protected virtual void OnSizeChanged()
        {
        }

#endif

    }


}