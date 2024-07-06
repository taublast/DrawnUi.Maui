using AppoMobi.Maui.Gestures;

namespace Sandbox.Views.Controls;

/// <summary>
/// Will animate from Parent control to Secondary then call TransitionEnded event that could make Parent invisible, dispose, whatever.
/// </summary>
public class ShaderTransitionEffect : ShaderDoubleTexturesEffect, IStateEffect, ISkiaGestureProcessor
{

    protected virtual void OnTransitionEnded()
    {
        TransitionEnded?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler TransitionEnded;

    bool _initialized;
    private PointF _mouse;

    #region IStateEffect

    public void UpdateState()
    {
        if (Parent != null && !_initialized && Parent.IsLayoutReady)
        {
            _initialized = true;
            if (_animator == null)
            {
                _animator = new(Parent);

                _animator.Start((v) =>
                {
                    this.Progress = v;
                    Update();
                }, 0, 1, 3500);
            }
        }

        base.Update();
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

        if (args.Type == TouchActionResult.Down && _initialized)
        {

            //var ripple = CreateRipple(_mouse);

            ////run new animator for every Down
            ////we use this helper task so that every new rangeanimator is disposed properly at the end
            //Task.Run(async () =>
            //{
            //    await Parent.AnimateRangeAsync((v) =>
            //    {
            //        ripple.Progress = v;
            //        Update();
            //    }, 0, 1, 4500);

            //    RemoveRipple(ripple.Uid);

            //}).ConfigureAwait(false);

        }

        return null;
    }

    #endregion

    void ApplyReflectionSourceControl(SkiaControl control)
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
            control.ApplyReflectionSourceControl((SkiaControl)newvalue);
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

    #region PROGRESS ANIMATOR

    private PingPongAnimator _animator;

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