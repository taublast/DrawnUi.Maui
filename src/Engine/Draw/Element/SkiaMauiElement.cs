namespace DrawnUi.Maui.Draw
{

    /*
	The concept:
	
	To create and move/transform the native view to where the skia placeholder is sitting.
	
	ANDROID + WINDOWS:
	To respond to fast skia updates (ex: while scrolling) we are forced to make a native view snapshot, 
	hide the native view and draw the snapshot while we are animating.
	Basically we launch a restarting timer everytime the native view detects it needs to 
	change its layout/tansform, we display the snapshot instead of native view until 
	restarting timer expires (its relaunched every time we detect a layout/transform change), 
	after that we can show the real maui view again.
	This snapshot activation mechanics is an option set by static property bool AnimateSnapshot. 
	So inside scroll this property must be set to true to have the maui view scroll smoothly with 
	other skia elements.
	
	OTHER PLATFORMS:
	Do not need a snapshot, maui view is moved/transformed directly.
	 */

    [ContentProperty("Content")]
    public partial class SkiaMauiElement : SkiaControl
    {
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (Element != null)
                Element.BindingContext = this.BindingContext;
        }

        /// <summary>
        /// Measure and arrange VisualElement using Maui methods
        /// </summary>
        /// <param name="ptsWidth"></param>
        /// <param name="ptsHeight"></param>
        /// <returns></returns>
        public virtual Size MeasureAndArrangeMauiElement(double ptsWidth, double ptsHeight)
        {
            //if you pass 0 to initial measure whatever you pass later will not fix the broken maui control afterwards
            //so never pass 0
            try
            {
                if (Element is IView view && Element.Handler != null && ptsWidth > 0 && ptsHeight > 0 && LayoutReady)
                {
                    var measured = view.Measure(
                        ptsWidth - (this.Padding.Left + this.Padding.Right),
                        ptsHeight - (this.Padding.Top + this.Padding.Bottom)
                        );
                    //var arranged = view.Arrange(new Rect(0, 0, ptsWidth, ptsHeight));
                    return measured;
                }
            }
            catch (Exception e)
            {
                Super.Log(e);
            }

            try
            {
#if ANDROID
                return new(VisualTransformNative.Rect.Size.Width - (this.Padding.Left + this.Padding.Right) * RenderingScale, VisualTransformNative.Rect.Size.Height - (this.Padding.Top + this.Padding.Bottom) * RenderingScale);
#else
                return new(VisualTransformNative.Rect.Size.Width - (this.Padding.Left + this.Padding.Right), VisualTransformNative.Rect.Size.Height - (this.Padding.Top + this.Padding.Bottom));
#endif
            }
            catch (Exception e)
            {
                Super.Log(e);
            }

            return Size.Zero;
        }

        private object lockLayout = new();


        public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
        {
            //lock (lockLayout)
            {
                var bounds = base.Measure(widthConstraint, heightConstraint, scale);

                if (Element is IView view && Element.Handler != null)
                {
                    ContentSizeUnits = MeasureAndArrangeMauiElement(bounds.Units.Width, bounds.Units.Height);

                    //Super.Log($"[Measure] ContentSizeUnits {ContentSizeUnits}");
                }

                return bounds;
            }
        }


        protected Size ContentSizeUnits;


#if ANDROID || WINDOWS

        public SKImage GetSnapshot()
        {
            if (CachedBitmap != null)
            {
                return CachedBitmap.Snapshot();
            }
            return null;
        }

        public bool ShowSnapshot
        {
            get
            {
                return _showSnapshot;
            }

            protected set
            {
                if (_showSnapshot != value)
                {
                    _showSnapshot = value;
                    OnPropertyChanged();
                    //Super.Log($"ShowSnapshot] {value}");
                    Update();
                }
            }
        }
        bool _showSnapshot;


        public virtual void TakeSnapshot()
        {
            var width = (int)(ElementSize.X);
            var height = (int)(ElementSize.Y);

            //create snapshot container
            if (CachedBitmap == null || height != CacheSurfaceInfo.Height || width != CacheSurfaceInfo.Width)
            {
                CachedBitmap?.Dispose();
                CacheSurfaceInfo = new SKImageInfo(width, height);
                // hardware accelerated surface will not do here as we might use different gfx contexts with ui thread
                //normal one
                CachedBitmap = SKSurface.Create(CacheSurfaceInfo);
            }
            else
            {
                CachedBitmap.Canvas.Clear();
            }

            //flush native view into snapshot container

            TakeNativeSnapshot(CachedBitmap);

        }


#endif

        public SKSurface CachedBitmap { get; protected set; }

        public SKImageInfo CacheSurfaceInfo { get; set; }

        protected override void OnWillBeDisposed()
        {
            base.OnWillBeDisposed();

            SubscribeToRenderingChain(false);

#if ANDROID || IOS || WINDOWS || MACCATALYST
            RemoveMauiElement(Element);
#endif

        }

        public override void OnDisposing()
        {

            CachedBitmap?.Dispose();

            CachedBitmap = null;

            base.OnDisposing();
        }

        protected RestartingTimer<object> TimerShowMauiView;

        protected void PostponeShowingNativeView(int ms)
        {
            if (!ShowSnapshot)
                return;

            if (Element != null && Element.Handler != null && TimerShowMauiView == null)
            {
                TimerShowMauiView = new(TimeSpan.FromMilliseconds(ms), (arg) =>
                {
                    if (Element != null)
                    {
#if ANDROID || WINDOWS
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            ShowSnapshot = false;
                            if (ContentSizeUnits == Size.Zero)
                            {
                                Invalidate();
                            }

                            LayoutNativeView(Element);
                        });
#else
                        LayoutNativeView(Element);
#endif
                    }
                });
                TimerShowMauiView.Start(null);
            }
            else
            {
                TimerShowMauiView.Restart(null);
            }
        }

        /// <summary>
        /// For HotReload
        /// </summary>
        /// <returns></returns>
        public override IReadOnlyList<IVisualTreeElement> GetVisualChildren()
        {
            var ret = new List<IVisualTreeElement>();
            if (Content != null)
            {
                ret.Add(Content);
            }
            return ret.AsReadOnly(); ;
        }

        /// <summary>
        /// Prevent usage of subviews as we are using Content property for this control
        /// </summary>
        /// <param name="views"></param>
        public override void SetChildren(IEnumerable<ISkiaAttachable> views)
        {
            //do not use subviews as we are using Content property for this control
            // so we just override not calling base
        }

        /// <summary>
        /// Prevent usage of subviews as we are using Content property for this control
        /// </summary>
        protected override void OnChildAdded(SkiaControl child)
        {
            if (this.Views.Count > 0)
            {
                throw new Exception(
                    "SkiaMauiElement cannot have SkiaControl subviews. Please use Content property of type VisualElement.");
            }

            base.OnChildAdded(child);
        }

        public override void SuperViewChanged()
        {
            base.SuperViewChanged();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                SetContent(Content);
            });

        }

        /// <summary>
        /// Use Content property for direct access
        /// </summary>
        protected virtual void SetContent(VisualElement view)
        {
            if (Element == view)
            {
                //better update layout
#if ANDROID || WINDOWS
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ShowSnapshot = false;
                    if (ContentSizeUnits == Size.Zero)
                    {
                        Invalidate();
                    }

                    LayoutNativeView(Element);
                });
#else
                LayoutNativeView(Element);
#endif
                return;
            }

            if (Superview != null && Element != null)
            {

#if ANDROID || IOS || WINDOWS || MACCATALYST
                RemoveMauiElement(Element);
#endif
            }

            Element = null;

            if (Superview != null && view != null && LayoutReady)
            {
                CreateMauiElement(view);
            }
        }

        protected override void OnLayoutReady()
        {
            base.OnLayoutReady();

            SetContent(this.Content);
        }

        public void CreateMauiElement(VisualElement element)
        {
            Element = element;

            Element.BindingContext = this.BindingContext;

#if ANDROID || IOS || WINDOWS || MACCATALYST
            SetupMauiElement(Element);
#endif
        }

        public bool NeedsLayoutNative { get; set; } = true;

        public void ApplyTransform(VisualTransform transform)
        {
            _transform = transform;

            if (Element == null || Element.Handler == null)
                return;

            var outRect = DrawingRect;
            //outRect.Intersect(Superview.DrawingRect);
            var native = transform.ToNative(outRect, RenderingScale);

            //Debug.WriteLine($"[ME] {native.IsVisible}");

            if (native != VisualTransformNative)// && !ShowSnapshot)
            {
                UpdateElementSize();
                VisualTransformNative = native;
                NeedsLayoutNative = true;
            }

        }

        public void Refresh()
        {
#if ANDROID || IOS || WINDOWS || MACCATALYST
            LayoutMauiElement();
#endif
            Update();
        }

        protected override void OnLayoutChanged()
        {
            base.OnLayoutChanged();

            UpdateElementSize();
        }

        void UpdateElementSize()
        {
            ElementSize = new(DrawingRect.Width, DrawingRect.Height);
        }

        SKRect _destination;

        bool _rendered;

        VisualTreeChain _chain;

        protected void SubscribeToRenderingChain(bool subscribe)
        {
            if (Superview != null)
            {
                if (subscribe)
                {
                    if (_chain == null)
                    {
                        _chain = GenerateParentChain();
                        _chain.AddNode(this); //this control might have its own transforms too
                        Superview.RegisterRenderingChain(_chain);
                    }
                }
                else
                {
                    Superview.UnregisterRenderingChain(this);
                }
            }
        }

        public override void Render(SkiaDrawingContext context, SKRect destination, float scale)
        {
            Superview = context.Superview;
            _destination = destination;

            if (Content != null)
            {
                /*
                    if (Element == null) //was set in xaml before we attached to superview..
                    {
                        if (Superview != null)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                CreateMauiElement(Content);
                            });
                        }
                    }
                */

                //might become nut after CreateMauiElement
                if (Element != null)
                    SubscribeToRenderingChain(true);
            }

            base.Render(context, destination, scale);
        }

#if ANDROID || WINDOWS

        protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
        {
            base.Paint(ctx, destination, scale, arguments); //paint background color

            if (ShowSnapshot && CachedBitmap != null)
            {
                //todo apply transforms
                DrawSnapshot(ctx.Canvas, destination);
            }
        }

        protected bool SnapshotReady { get; set; }

        protected virtual void DrawSnapshot(SKCanvas canvas, SKRect destination)
        {
            var point = new SKPoint(destination.Left, destination.Top);
            canvas.DrawSurface(CachedBitmap, point);

            if (SnapshotReady && IsNativeVisible)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SetNativeVisibility(false);
                });
            }
        }

#endif

        public bool IsNativeVisible { get; protected set; }

        public bool WasRendered { get; protected set; }

        public void LayoutMauiElement(bool manageMainThread = true)
        {
            if (Element == null || Element.Handler == null || !NeedsLayoutNative)
            {
                if (ShowSnapshot)
                {
                    PostponeShowingNativeView(FreezeTimeMs);
                }
                //Tasks.StartDelayed(TimeSpan.FromMilliseconds(100), () =>
                //{
                //    MainThread.BeginInvokeOnMainThread(() =>
                //    {
                //        Element.InvalidateMeasureNonVirtual(Microsoft.Maui.Controls.Internals.InvalidationTrigger.HorizontalOptionsChanged);
                //    });
                //});
                return;
            }

            NeedsLayoutNative = false;

            void Act()
            {
                LayoutNativeView(Element);

#if ANDROID || WINDOWS

                if (AnimateSnapshot && WasRendered && VisualTransformNative.IsVisible)
                {
                    if (!ShowSnapshot)
                    {
                        if (VisualTransformNative.IsVisible)
                        {
                            SnapshotReady = false;
                            TakeSnapshot();
                            ShowSnapshot = true;
                            LayoutMauiElement();
                        }
                    }
                    else
                        PostponeShowingNativeView(FreezeTimeMs); //to set ShowSnapshot to false;
                }
                else
                {
                    SetNativeVisibility(VisualTransformNative.IsVisible);
                }
#else
                //SetNativeVisibility(VisualTransformNative.IsVisible);
#endif
            }

            if (!manageMainThread)
            {
                Act();
                return;
            }

#if ANDROID || WINDOWS
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Act();
            });
#else
            Act();
#endif

        }

        public override void SetVisualTransform(VisualTransform transform)
        {
            //Super.Log($"SetVisualTransform {transform.Translation.X},{transform.Translation.Y} Tree: " + transform.Logs);

            ApplyTransform(transform);

#if ANDROID || IOS || WINDOWS || MACCATALYST
            LayoutMauiElement();
#endif
        }


        /// <summary>
        /// Maui Element to be rendered
        /// </summary>
        public VisualElement Element { get; protected set; }

        /// <summary>
        /// PIXELS, for faster checks
        /// </summary>
        public SKPoint ElementSize { get; set; }


        #region PROPERTIES

        public static readonly BindableProperty FreezeTimeMsProperty = BindableProperty.Create(
            nameof(FreezeTimeMs),
            typeof(int),
            typeof(SkiaMauiElement),
            250);

        public int FreezeTimeMs
        {
            get { return (int)GetValue(FreezeTimeMsProperty); }
            set { SetValue(FreezeTimeMsProperty, value); }
        }

        public static readonly BindableProperty AnimateSnapshotProperty = BindableProperty.Create(
            nameof(AnimateSnapshot),
            typeof(bool),
            typeof(SkiaMauiElement),
            false);

        /// <summary>
        /// Set to true if you are hosting the control inside a scroll or similar case
        /// where the control position/transforms are animated fast.
        /// </summary>
        public bool AnimateSnapshot
        {
            get { return (bool)GetValue(AnimateSnapshotProperty); }
            set { SetValue(AnimateSnapshotProperty, value); }
        }

        public static readonly BindableProperty ContentProperty = BindableProperty.Create(
            nameof(Content),
            typeof(VisualElement), typeof(SkiaMauiElement),
            null,
            propertyChanged: OnReplaceContent);

        private VisualTransform _transform;

        private static void OnReplaceContent(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SkiaMauiElement control)
            {

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    control.SetContent(newvalue as VisualElement);
                });

            }
        }

        public VisualElement Content
        {
            get { return (VisualElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public VisualTransformNative VisualTransformNative { get; protected set; }

        #endregion

#if ((NET7_0 || NET8_0) && !ANDROID && !IOS && !MACCATALYST && !WINDOWS && !TIZEN)

        protected virtual void LayoutNativeView(VisualElement element)
        {
            throw new NotImplementedException();
        }
        public virtual void SetNativeVisibility(bool state)
        {
            throw new NotImplementedException();
        }

        public bool ShowSnapshot => false;

#endif
    }
}
