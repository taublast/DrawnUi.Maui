namespace DrawnUi.Maui.Draw;

public class SkiaShaderEffect : SkiaEffect, IPostRendererEffect
{
    protected SKPaint PaintWithShader;

    public static readonly BindableProperty UseContextProperty = BindableProperty.Create(nameof(UseContext),
        typeof(bool),
        typeof(SkiaShaderEffect),
        true,
        propertyChanged: NeedUpdate);

    /// <summary>
    /// Use either context of global Superview background, default is True. 
    /// </summary>
    public bool UseContext
    {
        get { return (bool)GetValue(UseContextProperty); }
        set { SetValue(UseContextProperty, value); }
    }

    public static readonly BindableProperty AutoCreateInputTextureProperty = BindableProperty.Create(nameof(AutoCreateInputTexture),
        typeof(bool),
        typeof(SkiaShaderEffect),
        true,
        propertyChanged: NeedUpdate);

    /// <summary>
    /// Should create a texture from the current drawing to pass to shader as uniform shader iImage1, default is True. You need this set to False only if your shader is output-only.
    /// </summary>
    public bool AutoCreateInputTexture
    {
        get { return (bool)GetValue(AutoCreateInputTextureProperty); }
        set { SetValue(AutoCreateInputTextureProperty, value); }
    }

    /// <summary>
    /// Create snapshot from the current parent control drawing state to use as input texture for the shader
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    protected virtual SKImage CreateSnapshot(SkiaDrawingContext ctx, SKRect destination)
    {
        SKImage snapshot;
        if (UseContext)
        {
            ctx.Canvas.Flush();
            snapshot = ctx.Surface.Snapshot(new((int)destination.Left, (int)destination.Top,
                (int)destination.Right, (int)destination.Bottom));
        }
        else
        {
            //notice we read from the real canvas and we write to ctx.Canvas which can be cache
            ctx.Superview.CanvasView.Surface.Canvas.Flush();
            snapshot = ctx.Superview.CanvasView.Surface.Snapshot(new((int)destination.Left,
                (int)destination.Top, (int)destination.Right, (int)destination.Bottom));
        }
        return snapshot;
    }


    protected virtual SKImage GetPrimaryTextureImage(SkiaDrawingContext ctx, SKRect destination)
    {
        if (Parent?.RenderObject?.Image == null && AutoCreateInputTexture)
        {
            return CreateSnapshot(ctx, destination);
        }

        return Parent?.RenderObject?.Image;
    }

    /// <summary>
    /// EffectPostRenderer
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="destination"></param>
    public virtual void Render(SkiaDrawingContext ctx, SKRect destination)
    {
        if (PaintWithShader == null)
        {
            PaintWithShader = new SKPaint()
            {
                //todo check how if this affect anything after upcoming skiasharp3 fix
                //FilterQuality = SKFilterQuality.High,
                //IsDither = true
            };
        }

        SKImage source = GetPrimaryTextureImage(ctx, destination);

        PaintWithShader.Shader = CreateShader(ctx, destination, source);

        ctx.Canvas.DrawRect(destination, PaintWithShader);
    }

    protected SKShader PrimaryTexture;
    private SKImage _lastSource;

    protected virtual SKShader CompilePrimaryTexture(SKImage source)
    {
        //if (_lastSource == null && source != null) //snapshot changed
        if (source != _lastSource && source != null) //snapshot changed
        {
            _lastSource = source;
            var dispose = PrimaryTexture;
            PrimaryTexture = source.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);
            if (dispose != PrimaryTexture)
                dispose?.Dispose();
        }
        return PrimaryTexture;
    }

    public virtual SKShader CreateShader(SkiaDrawingContext ctx, SKRect destination, SKImage source)
    {
        //we need to
        //step 1: compile shader
        //step 2: prepare textures to pass
        //step 3: prepare uniforms to pass, including those textures
        //step 4: with all above create an SKShader to use in SKPaint

        if (CompiledShader == null || _hasNewShader)
        {
            CompileShader();
            _hasNewShader = false;
        }

        if (NeedApply)
        {
            if (source == null && AutoCreateInputTexture)
            {
                source = CreateSnapshot(ctx, destination);
            }

            var killTextures = TexturesUniforms;
            TexturesUniforms = CreateTexturesUniforms(ctx, destination, CompilePrimaryTexture(source));

#if SKIA3
            if (Parent != null && killTextures != null)
                Parent.DisposeObject(killTextures);
#endif

            var kill = Shader;
            var uniforms = CreateUniforms(destination);
#if SKIA3 
            Shader = CompiledShader.ToShader(uniforms, TexturesUniforms);
#else
            Shader = CompiledShader.ToShader(false, uniforms, TexturesUniforms);
#endif
            if (kill != null)
                Parent.DisposeObject(kill);
        }

        return Shader;
    }

    public override bool NeedApply
    {
        get
        {
            return base.NeedApply && CompiledShader != null;// && Shader == null;
        }
    }

    protected SKRuntimeEffectChildren TexturesUniforms;
    protected SKRuntimeEffect CompiledShader;
    public SKShader Shader { get; set; }

    protected virtual SKRuntimeEffectUniforms CreateUniforms(SKRect destination)
    {
        var viewport = destination;

        SKSize iResolution = new(viewport.Width, viewport.Height);
        SKSize iImageResolution = iResolution; //since we have no textures in base its same
        var uniforms = new SKRuntimeEffectUniforms(CompiledShader);

        uniforms["iOffset"] = new[] { viewport.Left, viewport.Top };
        uniforms["iResolution"] = new[] { iResolution.Width, iResolution.Height };
        uniforms["iImageResolution"] = new[] { iImageResolution.Width, iImageResolution.Height };

        return uniforms;
    }

    protected virtual SKRuntimeEffectChildren CreateTexturesUniforms(SkiaDrawingContext ctx, SKRect destination, SKShader primaryTexture)
    {
        if (primaryTexture != null)
        {
            //.ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
            return new SKRuntimeEffectChildren(CompiledShader)
            {
                { "iImage1", primaryTexture },
                //{ "iImage2", _texture2 }
            };
        }
        else
        {
            return new SKRuntimeEffectChildren(CompiledShader)
            {
            };
        }
    }

    protected string _template = null;
    protected string _templatePlacehodler = "//script-goes-here";

    protected virtual void CompileShader()
    {
        if (!string.IsNullOrEmpty(ShaderTemplate))
        {
            if (string.IsNullOrEmpty(_template))
                _template = SkSl.LoadFromResources(ShaderTemplate);
        }
        string shaderCode = SkSl.LoadFromResources(ShaderSource);
        if (!string.IsNullOrEmpty(_template))
        {
            shaderCode = _template.Replace(_templatePlacehodler, shaderCode);
        }
        CompiledShader = SkSl.Compile(shaderCode, ShaderSource);
    }

    protected virtual void ApplyShaderSource()
    {
        _hasNewShader = true;
        _template = null;
        Update();
    }

    private bool _hasNewShader;

    protected static void NeedChangeSource(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaShaderEffect control)
        {
            control.ApplyShaderSource();
        }
    }

    public static readonly BindableProperty ShaderSourceProperty = BindableProperty.Create(nameof(ShaderSource),
        typeof(string),
        typeof(SkiaShaderEffect),
        string.Empty, propertyChanged: NeedChangeSource);

    public string ShaderSource
    {
        get { return (string)GetValue(ShaderSourceProperty); }
        set { SetValue(ShaderSourceProperty, value); }
    }


    public static readonly BindableProperty ShaderTemplateProperty = BindableProperty.Create(nameof(ShaderTemplate),
        typeof(string),
        typeof(SkiaShaderEffect),
        string.Empty, propertyChanged: NeedChangeSource);

    public string ShaderTemplate
    {
        get { return (string)GetValue(ShaderTemplateProperty); }
        set { SetValue(ShaderTemplateProperty, value); }
    }

    public override void Update()
    {
        var kill = Shader;
        Shader = null;
        if (Parent != null && kill != null)
        {
            Parent.DisposeObject(kill);
        }
        else
            kill?.Dispose();

        //Parent?.Repaint();

        base.Update();
    }

    protected override void OnDisposing()
    {
        Shader?.Dispose();
        Shader = null;
        _lastSource = null;

        CompiledShader?.Dispose();
#if SKIA3
        TexturesUniforms?.Dispose();
#endif
        PaintWithShader?.Dispose();

        PrimaryTexture?.Dispose();

        base.OnDisposing();
    }

}