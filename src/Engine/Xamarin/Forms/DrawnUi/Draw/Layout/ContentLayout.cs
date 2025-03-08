using System.Linq;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw;

[ContentProperty("Content")]
public partial class ContentLayout : SkiaControl, IVisibilityAware, ISkiaGestureListener
{

    public override void Invalidate()
    {
        base.Invalidate();

        Update();
    }

    public override void InvalidateByChild(SkiaControl child)
    {
        if (!NeedAutoSize && child.NeedAutoSize || !NeedAutoSize && IsTemplated)
        {
            UpdateByChild(child);
            return;
        }

        base.InvalidateByChild(child);
    }

    public override void OnWillDisposeWithChildren()
    {
        base.OnWillDisposeWithChildren();

        Content?.Dispose();
    }

    public virtual bool OnFocusChanged(bool focus)
    {
        return false;
    }

    public virtual void OnAppeared()
    {
        if (Content is IVisibilityAware aware)
        {
            aware.OnAppeared();
        }
    }

    public virtual void OnDisappeared()
    {
        if (Content is IVisibilityAware aware)
        {
            aware.OnDisappeared();
        }
    }

    public virtual void OnDisappearing()
    {
        if (Content is IVisibilityAware aware)
        {
            aware.OnDisappearing();
        }

    }

    public virtual void OnAppearing()
    {
        if (Content is IVisibilityAware aware)
        {
            aware.OnAppearing();
        }
    }


    protected bool IsContentActive
    {
        get
        {
            return Content != null && Content.IsVisible;
        }
    }


    public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
    {
        if (IsMeasuring)
        {
            NeedRemeasuring = true;
            return MeasuredSize;
        }

        //background measuring or invisible or self measure from draw because layout will never pass -1
        if (!CanDraw || widthConstraint < 0 || heightConstraint < 0)
        {
            return MeasuredSize;
        }

        try
        {
            IsMeasuring = true;

            var request = CreateMeasureRequest(widthConstraint, heightConstraint, scale);
            if (request.IsSame)
            {
                return MeasuredSize;
            }

            if (request.WidthRequest == 0 || request.HeightRequest == 0)
            {
                InvalidateCacheWithPrevious();

                return SetMeasuredAsEmpty(request.Scale);
            }

            var constraints = GetMeasuringConstraints(request);

            var children = new List<SkiaControl> { Content };
            ContentSize = MeasureContent(children, constraints.Content, scale);

            return SetMeasuredAdaptToContentSize(constraints, scale);
        }
        finally
        {
            IsMeasuring = false;
        }


    }

    public ScaledRect Viewport { get; protected set; } = new();

    //public override void Arrange(SKRect destination, float widthRequest, float heightRequest, float scale)
    //{
    //    if (!CanDraw)
    //        return;

    //    if (NeedMeasure)
    //    {
    //        var adjustedDestination = CalculateLayout(destination, SizeRequest.Width, SizeRequest.Height, scale);
    //        Measure(adjustedDestination.Width, adjustedDestination.Height, scale);
    //        IsLayoutDirty = true;
    //    }

    //    base.Arrange(destination, MeasuredSize.Units.Width, MeasuredSize.Units.Height, scale);
    //}



    /// <summary>
    /// In PIXELS
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    protected virtual SKRect GetContentAvailableRect(SKRect destination)
    {
        return destination;
    }


    public override ScaledRect GetOnScreenVisibleArea(float inflateByPixels = 0)
    {
        var inflated = DrawingRect;
        inflated.Inflate(inflateByPixels, inflateByPixels);
        return ScaledRect.FromPixels(inflated, RenderingScale);
    }

    protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
    {
        base.Paint(ctx, destination, scale, arguments);

        DrawViews(ctx, destination, scale);
    }


    protected override int DrawViews(SkiaDrawingContext context, SKRect destination, float scale,
        bool debug = false)
    {
        if (!IsContentActive || destination.Width <= 0 || destination.Height <= 0)
        {
            return 0;
        }
        var drawViews = new List<SkiaControl> { Content };

        return RenderViewsList(drawViews, context, destination, scale);
    }


    public SKRect ContentAvailableSpace { get; protected set; }

    public override void SetChildren(IEnumerable<SkiaControl> views)
    {
        //do not use subviews as we are using Content property for this control

        return;
    }

    public override void ApplyBindingContext()
    {
        base.ApplyBindingContext();


        if (Content?.BindingContext == null)
        {
            Content?.SetInheritedBindingContext(BindingContext);
        }
    }

    protected virtual void SetContent(SkiaControl view)
    {
        var oldContent = Views.FirstOrDefault(x => x == Content);
        if (view != oldContent)
        {
            if (oldContent != null)
            {
                RemoveSubView(oldContent);
            }
            if (view != null)
            {
                AddSubView(view);
            }
        }
    }


    protected override void CreateChildrenFromCode()
    {
        if (Content == null && !DefaultChildrenCreated)
        {
            DefaultChildrenCreated = true;
            if (CreateChildren != null)
            {
                try
                {
                    var children = CreateChildren();
                    Content = children.FirstOrDefault();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }
        }
    }

    #region PROPERTIES


    private static void OnReplaceContent(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is ContentLayout control)
        {
            control.SetContent(newvalue as SkiaControl);
        }
    }

    public static readonly BindableProperty ContentProperty = BindableProperty.Create(
        nameof(Content),
        typeof(SkiaControl), typeof(ContentLayout),
        null,
        propertyChanged: OnReplaceContent);

    public SkiaControl Content
    {
        get { return (SkiaControl)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(ScrollOrientation), typeof(ContentLayout),
        ScrollOrientation.Vertical,
        propertyChanged: NeedDraw);
    /// <summary>
    /// <summary>Gets or sets the scrolling direction of the ScrollView. This is a bindable property.</summary>
    /// </summary>
    public ScrollOrientation Orientation
    {
        get { return (ScrollOrientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly BindableProperty ScrollTypeProperty = BindableProperty.Create(nameof(ViewportScrollType), typeof(ViewportScrollType), typeof(ContentLayout),
        ViewportScrollType.Scrollable,
        propertyChanged: NeedDraw);
    /// <summary>
    /// <summary>Gets or sets the scrolling direction of the ScrollView. This is a bindable property.</summary>
    /// </summary>
    public ViewportScrollType ScrollType
    {
        get { return (ViewportScrollType)GetValue(ScrollTypeProperty); }
        set { SetValue(ScrollTypeProperty, value); }
    }

    public static readonly BindableProperty VirtualizationProperty = BindableProperty.Create(
        nameof(Virtualisation),
        typeof(VirtualisationType),
        typeof(ContentLayout),
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




    #endregion



    protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(Orientation))
        {
            Invalidate();
        }
    }






}