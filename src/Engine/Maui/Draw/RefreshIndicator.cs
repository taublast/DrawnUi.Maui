namespace DrawnUi.Draw;

public class RefreshIndicator : SkiaLayout, IRefreshIndicator
{
    public RefreshIndicator()
    {
        InputTransparent = true;
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Start;
        SetDragRatio(0);
    }

    public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation),
        typeof(ScrollOrientation), typeof(RefreshIndicator),
        ScrollOrientation.Vertical,
        propertyChanged: (bindable, old, changed) =>
        {
            if (bindable is RefreshIndicator refresh && changed is ScrollOrientation orientation)
            {
                if (orientation == ScrollOrientation.Both)
                {
                    throw new NotImplementedException();
                }
                refresh.UpdateLayout();
            }
        });
    /// <summary>
    /// <summary>Gets or sets the scrolling direction of the ScrollView. This is a bindable property.</summary>
    /// </summary>
    public ScrollOrientation Orientation
    {
        get { return (ScrollOrientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    protected virtual void UpdateLayout()
    {
        if (Orientation == ScrollOrientation.Vertical)
        {
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Start;
        }
        else
        if (Orientation == ScrollOrientation.Horizontal)
        {
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Fill;
        }

        Invalidate();
        Update();
    }

    /// <summary>
    /// 0 - 1
    /// </summary>
    /// <param name="ratio"></param>
    public virtual void SetDragRatio(float ratio)
    {
        double VisibleRatio = Math.Min(1.0, ratio / 0.98);

        double opacity = Math.Clamp(Math.Pow(ratio, 2), 0, 1);

        if (IsRunning)
            opacity = 1;

        Opacity = opacity;
        IsVisible = Opacity != 0;

        if (Orientation == ScrollOrientation.Vertical)
        {
            if (Height > 0)
            {
                TranslationY = (float)(-Height + Height * VisibleRatio);
            }
        }
        else
        if (Orientation == ScrollOrientation.Horizontal)
        {
            if (Width > 0)
            {
                TranslationX = (float)(-Width + Width * VisibleRatio);
            }
        }

        IsRunning = Math.Abs(VisibleRatio) >= 1;
    }

    public float VisibleRatio { get; set; }

    protected virtual void OnIsRunningChanged(bool value)
    {

    }

    //protected double OverScrollMax { get; set; }
    //public void SetOverScrollMax(double value)
    //{
    //    OverScrollMax = value;
    //}


    private bool _IsRunning;
    /// <summary>
    /// ReadOnly
    /// </summary>
    public bool IsRunning
    {
        get
        {
            return _IsRunning;
        }
        protected set
        {
            if (_IsRunning != value)
            {
                _IsRunning = value;
                OnPropertyChanged();
                OnIsRunningChanged(value);
            }
        }
    }

}