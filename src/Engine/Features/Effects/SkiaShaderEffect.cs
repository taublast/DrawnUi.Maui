﻿namespace DrawnUi.Maui.Draw;

public class ShaderAnimatedEffect : SkiaShaderEffect
{

}



public class StateEffect : SkiaEffect, IStateEffect
{
    public virtual void UpdateState()
    {

    }
}


public class SkiaShaderEffect : SkiaEffect, IPostRendererEffect
{
    protected SKPaint _paintWithShader;

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
    /// Create snapshot from the current parent drawing state to use as input texture for the shader
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


    public virtual void Render(SkiaDrawingContext ctx, SKRect destination)
    {
        if (_paintWithShader == null)
        {
            _paintWithShader = new SKPaint();
        }

        SKImage source = Parent.RenderObject.Image;

        _paintWithShader.Shader = CreateShader(ctx, destination, source);

        ctx.Canvas.DrawRect(destination, _paintWithShader);
    }

    public virtual SKShader CreateShader(SkiaDrawingContext ctx, SKRect destination, SKImage source)
    {
        //we need to
        //step 1: compile shader
        //step 2: prepare textures to pass
        //step 3: prepare uniforms to pass, including those textures
        //step 4: with all above create an SKShader to use in SKPaint

        if (CompiledShader == null)
        {
            CompileShader();
        }

        if (NeedApply)
        {
            if (source == null)
            {
                source = CreateSnapshot(ctx, destination);
            }

            //if textures didn't change.. use previous?
            var killTextures = TexturesUniforms;
            TexturesUniforms = CreateTexturesUniforms(ctx, destination, source);

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

    protected virtual SKRuntimeEffectChildren CreateTexturesUniforms(SkiaDrawingContext ctx, SKRect destination, SKImage snapshot)
    {
        if (snapshot != null)
        {
            var texture1 = snapshot
                .ToShader();
            //.ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
            return new SKRuntimeEffectChildren(CompiledShader)
            {
                { "iImage1", texture1 },
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

    protected virtual void CompileShader()
    {
        string shaderCode = SkSl.LoadFromResources(ShaderSource);
        CompiledShader = SkSl.Compile(shaderCode);
    }

    public static readonly BindableProperty ShaderFilenameProperty = BindableProperty.Create(nameof(ShaderSource),
        typeof(string),
        typeof(SkiaShaderEffect),
        string.Empty, propertyChanged: NeedUpdate);
    public string ShaderSource
    {
        get { return (string)GetValue(ShaderFilenameProperty); }
        set { SetValue(ShaderFilenameProperty, value); }
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

        base.Update();
    }

    protected override void OnDisposing()
    {
        Shader?.Dispose();
        Shader = null;

        CompiledShader?.Dispose();
#if SKIA3
        TexturesUniforms?.Dispose();
#endif
        _paintWithShader?.Dispose();

        base.OnDisposing();
    }

}