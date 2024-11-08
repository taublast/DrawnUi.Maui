using DrawnUi.Maui.Draw;
using SkiaSharp;

namespace ShadersCarouselDemo.Controls.Carousel;

public class ShaderTransitionEffect : ShaderDoubleTexturesEffect, IStateEffect, ISkiaGestureProcessor
{

    protected virtual void OnTransitionEnded()
    {
        TransitionEnded?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler TransitionEnded;

    private PointF _mouse;

    #region IStateEffect

    /// <summary>
    /// Will be invoked before actually painting but after gestures processing and other internal calculations. By SkiaControl.OnBeforeDrawing method. Beware if you call Update() inside will never stop updating.
    /// </summary>
    public virtual void UpdateState()
    {

    }

    public override void Attach(SkiaControl parent)
    {
        base.Attach(parent);

        UpdateState();
    }

    #endregion

    #region GESTURES

    public virtual ISkiaGestureListener ProcessGestures(
        SkiaGesturesParameters args,
        GestureEventProcessingInfo apply)
    {
        _mouse = args.Event.Location;

        return null;
    }

    #endregion

    void ApplyTargetControl(SkiaControl control)
    {
        if (_controlSource == control)
            return;

        DetachTo();
        _controlSource = control;
        if (_controlSource != null)
        {
            _controlSource.CreatedCache += OnCacheCreatedTo;
        }
    }

    public static readonly BindableProperty TargetProperty = BindableProperty.Create(
        nameof(Target),
        typeof(SkiaControl),
        typeof(ShaderTransitionEffect),
        defaultValue: null,
        propertyChanged: ApplyTargetProperty);

    private SkiaControl _controlSource;

    public SkiaControl Target
    {
        get { return (SkiaControl)GetValue(TargetProperty); }
        set { SetValue(TargetProperty, value); }
    }

    private static void ApplyTargetProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (oldvalue != newvalue && bindable is ShaderTransitionEffect control)
        {
            control.ApplyTargetControl((SkiaControl)newvalue);
        }
    }

    protected override SKShader GetSecondaryTexture()
    {
        if (Target == null)
        {
            //from file
            return base.GetSecondaryTexture();
        }

        if (!_secondarySourceSet && ParentReady())
        {
            SetSecondaryFromTarget();
        }
        return SecondaryTexture;
    }

    private bool _secondarySourceSet;



    #region TargetCache

    void SetSecondaryFromTarget()
    {
        if (Target != null && Target.RenderObject != null && Target.RenderObject.Image != null)
        {
            var dispose = SecondaryTexture;

            SecondaryTexture = Target.RenderObject.Image.ToShader(TilingSecondaryTexture, TilingSecondaryTexture);

            if (dispose != SecondaryTexture)
                dispose?.Dispose();

            _secondarySourceSet = true;

            Update();
        }
    }

    protected void DetachTo()
    {
        if (Target != null)
        {
            Target.CreatedCache -= OnCacheCreatedTo;
            Target = null;
        }
    }

    void OnCacheCreatedTo(object sender, CachedObject e)
    {
        Update();
    }


    protected override void OnDisposing()
    {
        base.OnDisposing();
        DetachTo();
    }

    #endregion

    #region PROGRESS  



    public double Progress { get; set; }

    protected override SKRuntimeEffectUniforms CreateUniforms(SKRect destination)
    {
        var uniforms = base.CreateUniforms(destination);

        uniforms["progress"] = (float)Progress;
        uniforms["ratio"] = (float)(destination.Width / destination.Height);

        //uniforms["iMouse"] = new[] { _mouse.X, _mouse.Y, 0f, 0f };

        return uniforms;
    }

    #endregion




}