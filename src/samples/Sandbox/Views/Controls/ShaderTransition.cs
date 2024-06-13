using DrawnUi.Maui.Infrastructure;

namespace Sandbox.Views.Controls;

public class ShaderTransitionEffect : ShaderAnimatedEffect
{




}

public class TestShaderEffect : SkiaShader
{

    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(nameof(Progress),
    typeof(double),
    typeof(TestShaderEffect),
    0.0);
    public double Progress
    {
        get { return (double)GetValue(ProgressProperty); }
        set { SetValue(ProgressProperty, value); }
    }

}

public class AnimatedShaderTransition : ShaderTransition
{
    public AnimatedShaderTransition()
    {
        //  "transitions/fade.sksl";
        //  "transitions/doorway.sksl";
        //ShaderFilename = "transitions/cube.sksl";
        //ShaderFilename = "transitions/crosswarp.sksl";
        ShaderFilename = "transitions/new.sksl";

    }

    private PingPongAnimator _animator;

    protected override void OnLayoutReady()
    {
        base.OnLayoutReady();

        if (_animator == null)
        {
            _animator = new(this);

            _animator.Start((v) =>
            {
                this.Progress = v;
                Update();
            }, 0, 1, 3500);
        }

    }

}

public class ShaderTransition : SkiaControl
{
    protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
    {
        base.Paint(ctx, destination, scale, arguments);

        if (_compiledShader != null && _passTextures != null)
        {
            var viewport = DrawingRect;

            SKSize iResolution = new(viewport.Width, viewport.Height);
            SKSize iImageResolution = new(_controlFrom.RenderObject.Image.Width, _controlFrom.RenderObject.Image.Height);
            var uniforms = new SKRuntimeEffectUniforms(_compiledShader);

            uniforms["iOffset"] = new[] { viewport.Left, viewport.Top };
            uniforms["iResolution"] = new[] { iResolution.Width, iResolution.Height };
            uniforms["iImageResolution"] = new[] { iImageResolution.Width, iImageResolution.Height };
            uniforms["progress"] = (float)Progress;
            uniforms["ratio"] = (float)(viewport.Width / viewport.Height);

            using var paintWithShader = new SKPaint();

#if SKIA3 
            paintWithShader.Shader = _compiledShader.ToShader(uniforms, _passTextures);
#else
            paintWithShader.Shader = _compiledShader.ToShader(false, uniforms, _passTextures);
#endif

            ctx.Canvas.DrawRect(destination, paintWithShader);

        }
    }

    SKRuntimeEffect _compiledShader;
    SkiaControl _controlFrom;
    SkiaControl _controlTo;

    SKShader _texture1;
    SKShader _texture2;
    /// <summary>
    /// _texture wrapper for later use
    /// </summary>
    private SKRuntimeEffectChildren _passTextures;

    public double Progress { get; set; }
    public string ShaderFilename { get; set; }
    void CreateShader()
    {
        string shaderCode = SkSl.LoadFromResources($"{MauiProgram.ShadersFolder}/{ShaderFilename}");
        _compiledShader = SkSl.Compile(shaderCode);
    }

    public static readonly BindableProperty ControlFromProperty = BindableProperty.Create(
        nameof(ControlFrom),
        typeof(SkiaControl), typeof(ShaderTransition),
        null,
        propertyChanged: ApplyControlFromProperty);

    public SkiaControl ControlFrom
    {
        get { return (SkiaControl)GetValue(ControlFromProperty); }
        set { SetValue(ControlFromProperty, value); }
    }

    public static readonly BindableProperty ControlToProperty = BindableProperty.Create(
        nameof(ControlTo),
        typeof(SkiaControl), typeof(ShaderTransition),
        null,
        propertyChanged: ApplyControlToProperty);

    public SkiaControl ControlTo
    {
        get { return (SkiaControl)GetValue(ControlToProperty); }
        set { SetValue(ControlToProperty, value); }
    }

    private static void ApplyControlFromProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (oldvalue != newvalue && bindable is ShaderTransition control)
        {
            control.ApplyControlFrom(newvalue as SkiaControl);
        }
    }

    private static void ApplyControlToProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (oldvalue != newvalue && bindable is ShaderTransition control)
        {
            control.ApplyControlTo(newvalue as SkiaControl);
        }
    }

    void ApplyControlFrom(SkiaControl control)
    {
        if (_controlFrom == control)
            return;

        DetachFrom();
        _controlFrom = control;
        if (_controlFrom != null)
        {
            _controlFrom.CreatedCache += OnCacheCreatedFrom;
            _controlFrom.DelegateDrawCache += DrawContentImageFrom;
        }
    }

    void ApplyControlTo(SkiaControl control)
    {
        if (_controlTo == control)
            return;

        DetachTo();
        _controlTo = control;
        if (_controlTo != null)
        {
            _controlTo.CreatedCache += OnCacheCreatedTo;
            _controlTo.DelegateDrawCache += DrawContentImageTo;
        }
    }

    public override void OnDisposing()
    {
        base.OnDisposing();

        DetachFrom();
        DetachTo();

        _compiledShader?.Dispose();
        _passTextures?.Dispose();

    }
    void DetachFrom()
    {
        if (_controlFrom != null)
        {
            _controlFrom.CreatedCache -= OnCacheCreatedFrom;
            _controlFrom.DelegateDrawCache -= DrawContentImageFrom;
            _controlFrom = null;
        }
    }

    void DetachTo()
    {
        if (_controlTo != null)
        {
            _controlTo.CreatedCache -= OnCacheCreatedTo;
            _controlTo.DelegateDrawCache -= DrawContentImageTo;
            _controlTo = null;
        }
    }

    private void DrawContentImageFrom(CachedObject arg1, SkiaDrawingContext arg2, SKRect arg3)
    { }
    private void DrawContentImageTo(CachedObject arg1, SkiaDrawingContext arg2, SKRect arg3)
    {

    }
    private void OnCacheCreatedFrom(object sender, CachedObject e)
    {
        UpdateTextures();
    }

    private void OnCacheCreatedTo(object sender, CachedObject e)
    {
        UpdateTextures();
    }

    void UpdateTextures()
    {
        if (_controlTo == null || _controlFrom == null
                || _controlFrom.RenderObject == null || _controlFrom.RenderObject.Image == null
                || _controlTo.RenderObject == null || _controlTo.RenderObject.Image == null)
            return;

        if (_compiledShader == null)
        {
            CreateShader();
        }

        BuildTextures(_controlFrom.RenderObject.Image, _controlTo.RenderObject.Image);
    }

    void BuildTextures(SKImage from, SKImage to)
    {
        if (_compiledShader == null)
            return;

        try
        {
            var disposeTexture1 = _texture1;
            var disposeTexture2 = _texture2;

            _texture1 = from.ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
            _texture2 = to.ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);

            _passTextures = new SKRuntimeEffectChildren(_compiledShader)
            {
                { "iImage1", _texture1 },
                { "iImage2", _texture2 }
            };

            disposeTexture1?.Dispose();
            disposeTexture2?.Dispose();
        }
        catch (Exception e)
        {
            Super.Log(e);
        }
    }

}