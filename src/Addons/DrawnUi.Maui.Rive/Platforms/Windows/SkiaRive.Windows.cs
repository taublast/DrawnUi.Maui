using DrawnUi.Draw;
using RiveSharp;
using SkiaSharp;

namespace DrawnUi.Rive;

public partial class SkiaRive : SkiaControl
{

    // _scene is used on the render thread exclusively.
    Scene _scene = new Scene();


    void UpdateScene(SceneUpdates updates, byte[] sourceFileData = null)
    {
        if (updates >= SceneUpdates.File)
        {
            _scene.LoadFile(sourceFileData);
        }
        if (updates >= SceneUpdates.Artboard)
        {
            _scene.LoadArtboard(_artboardName);
        }
        if (updates >= SceneUpdates.AnimationOrStateMachine)
        {
            if (!string.IsNullOrEmpty(_stateMachineName))
            {
                _scene.LoadStateMachine(_stateMachineName);
            }
            else if (!string.IsNullOrEmpty(_animationName))
            {
                _scene.LoadAnimation(_animationName);
            }
            else
            {
                if (!_scene.LoadStateMachine(null))
                {
                    _scene.LoadAnimation(null);
                }
            }
        }
    }

    // Called from the render thread. Computes alignment based on the size of _scene.
    private Mat2D ComputeAlignment(double width, double height)
    {
        return ComputeAlignment(new AABB(0, 0, (float)width, (float)height));
    }

    // Called from the render thread. Computes alignment based on the size of _scene.
    private Mat2D ComputeAlignment(AABB frame)
    {
        return Renderer.ComputeAlignment(Fit.Contain, Alignment.Center, frame,
            new AABB(0, 0, _scene.Width, _scene.Height));
    }

    bool Advance()
    {
        var needUpdate = false;
        var now = DateTime.Now;
        if (_lastPaintTime == null) //first frame
        {
            _lastPaintTime = now;
        }
        var time = (now - _lastPaintTime).Value.TotalSeconds;

        needUpdate = _scene.AdvanceAndApply(time);

        _lastPaintTime = now;
        return needUpdate;
    }

    protected override void Paint(DrawingContext ctx)
    {
        base.Paint(ctx);

        if (_scene != null && _scene.IsLoaded)
        {
            if (canAdvance)
            {
                Advance();
            }

            var renderer = new Renderer(ctx.Context.Canvas);
            renderer.Save();
            renderer.Translate(DrawingRect.Left, DrawingRect.Top);
            renderer.Transform(ComputeAlignment(DrawingRect.Width, DrawingRect.Height));
            _scene.Draw(renderer);
            renderer.Restore();

            if (IsPlaying)
                Update();
        }

    }

    public void SetBool(string name, bool value)
    {
        if (_scene == null)
        {
            return;
        }
        _scene.SetBool(name, value);
    }

    public void SetNumber(string name, float value)
    {
        if (_scene == null)
        {
            return;
        }
        _scene.SetNumber(name, value);
    }

    public void FireTrigger(string name)
    {
        if (_scene == null)
        {
            return;
        }
        _scene.FireTrigger(name);
    }





}
