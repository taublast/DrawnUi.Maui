using AppoMobi.Specials;

namespace Sandbox.Views.Controls;

/// <summary>
/// Base shader effect class that has 2 input textures
/// </summary>
public class ShaderDoubleTexturesEffect : SkiaShaderEffect
{
    protected bool ParentReady()
    {
        return !(Parent == null || Parent.DrawingRect.Width <= 0 || Parent.DrawingRect.Height <= 0);
    }

    #region SecondaryTexture

    #region FromFile

    protected SKShaderTileMode TilingSecondaryTexture = SKShaderTileMode.Mirror;

    protected override SKRuntimeEffectChildren CreateTexturesUniforms(SkiaDrawingContext ctx, SKRect destination, SKShader texture1)
    {
        var texture2 = GetSecondaryTexture();

        if (texture1 != null && texture2 != null)
        {
            //var texture1 = snapshot.ToShader();

            return new SKRuntimeEffectChildren(CompiledShader)
            {
                { "iImage1", texture1 }, //main
                { "iImage2", texture2 } //secondary
            };
        }
        else
        {
            return new SKRuntimeEffectChildren(CompiledShader)
            {
            };
        }
    }

    public static readonly BindableProperty SecondarySourceProperty = BindableProperty.Create(
        nameof(SecondarySource),
        typeof(string),
        typeof(ShaderDoubleTexturesEffect),
        defaultValue: null,
        propertyChanged: ApplySecondarySourceProperty);

    public string SecondarySource
    {
        get { return (string)GetValue(SecondarySourceProperty); }
        set { SetValue(SecondarySourceProperty, value); }
    }

    private static void ApplySecondarySourceProperty(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (oldvalue != newvalue && bindable is ShaderDoubleTexturesEffect control)
        {
            control.ApplySecondarySource((string)newvalue);
        }
    }

    void ApplySecondarySource(string source)
    {
        Task.Run(async () =>
        {
            await LoadSource(source);
            if (ParentReady() && _loadedReflectionBitmap != null)
            {
                CompileSecondaryTexture();
            }

        });
    }

    private bool _secondarySourceSet;

    public void CompileSecondaryTexture()
    {
        SKImage image = null;

        if (_loadedReflectionBitmap != null)
        {
            var outRect = Parent.DrawingRect;
            var info = new SKImageInfo((int)outRect.Width, (int)outRect.Height);
            var resizedBitmap = new SKBitmap(info);
            using (var canvas = new SKCanvas(resizedBitmap))
            {
                // This will stretch the original image to fill the new size
                var rect = new SKRect(0, 0, (int)outRect.Width, (int)outRect.Height);
                canvas.DrawBitmap(_loadedReflectionBitmap, rect);
                canvas.Flush();
            }

            image = SKImage.FromBitmap(resizedBitmap);
        }

        var dispose = SecondaryTexture;

        if (image != null)
        {
            SecondaryTexture = image.ToShader(TilingSecondaryTexture, TilingSecondaryTexture);
        }

        if (dispose != SecondaryTexture)
            dispose?.Dispose();

        _secondarySourceSet = true;

        Update();
    }

    protected SKShader SecondaryTexture;

    protected virtual SKShader GetSecondaryTexture()
    {
        if (!_secondarySourceSet && ParentReady())
        {
            CompileSecondaryTexture();
        }
        return SecondaryTexture;
    }


    private SemaphoreSlim _semaphoreLoadFile = new(1, 1);

    /// <summary>
    /// Loading from local files only
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>
    public async Task LoadSource(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return;

        await _semaphoreLoadFile.WaitAsync();

        try
        {
            if (fileName.SafeContainsInLower("file://"))
            {
                var fullFilename = fileName.Replace("file://", "", StringComparison.InvariantCultureIgnoreCase);
                using var stream = new FileStream(fullFilename, System.IO.FileMode.Open);
                _loadedReflectionBitmap = SKBitmap.Decode(stream);
            }
            else
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                _loadedReflectionBitmap = SKBitmap.Decode(stream);
            }

            _secondarySourceSet = false;
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine($"LoadSource failed to load animation {fileName}");
            Console.WriteLine(e);
            return;
        }
        finally
        {
            _semaphoreLoadFile.Release();
        }
    }

    protected override void OnDisposing()
    {
        base.OnDisposing();

        SecondaryTexture?.Dispose();
        _loadedReflectionBitmap?.Dispose();
    }

    SKBitmap _loadedReflectionBitmap;

    #endregion


    #endregion
}