using AppoMobi.Maui.Gestures;

namespace Sandbox.Views.Controls;

public class MultiRippleWithTouchEffect : SkiaShader, IStateEffect, ISkiaGestureProcessor
{
    public MultiRippleWithTouchEffect()
    {
        ShaderFilename = "Shaders/ripples.sksl";
    }

    bool _initialized;
    private PointF _mouse;

    public void UpdateState()
    {
        if (Parent != null && !_initialized && Parent.IsLayoutReady)
        {
            _initialized = true;
        }

        base.Update();
    }

    public override void Attach(SkiaControl parent)
    {
        base.Attach(parent);

        UpdateState();
    }

    protected override SKRuntimeEffectUniforms CreateUniforms(SKRect destination)
    {
        var uniforms = base.CreateUniforms(destination);

        var activeRipples = GetActiveRipples();

        var mouseArray = new float[10 * 2];
        var progressArray = new float[10];

        for (int i = 0; i < 10; i++)
        {
            if (i < activeRipples.Count)
            {
                var ripple = activeRipples[i];
                mouseArray[i * 2] = ripple.Origin.X;
                mouseArray[i * 2 + 1] = ripple.Origin.Y;
                progressArray[i] = (float)ripple.Progress;
            }
            else
            {
                mouseArray[i * 2] = 0;
                mouseArray[i * 2 + 1] = 0;
                progressArray[i] = -1f; // inactive
            }
        }

        uniforms["origins"] = mouseArray;
        uniforms["progresses"] = progressArray;

        //was for just one ripple
        //uniforms["progress"] = (float)Progress;
        //uniforms["iMouse"] = new[] { _mouse.X, _mouse.Y, 0f, 0f };

        return uniforms;
    }

    public class Ripple
    {
        public Guid Uid { get; set; }
        public PointF Origin { get; set; }
        public long Time { get; set; }
        public double Progress { get; set; }
    }

    Dictionary<Guid, Ripple> Ripples = new();

    public Ripple CreateRipple(PointF origin)
    {
        var ripple = new Ripple
        {
            Uid = Guid.NewGuid(),
            Origin = origin,
            Time = Super.GetCurrentTimeNanos()
        };
        Ripples.Add(ripple.Uid, ripple);
        return ripple;
    }

    public void RemoveRipple(Guid key)
    {
        Ripples.Remove(key);
    }

    public List<Ripple> GetActiveRipples()
    {
        return Ripples.Values.OrderByDescending(x => x.Time).Take(10).ToList();
    }

    public virtual ISkiaGestureListener ProcessGestures(
        SkiaGesturesParameters args,
        GestureEventProcessingInfo apply)
    {
        _mouse = args.Event.Location;

        if (args.Type == TouchActionResult.Down && _initialized)
        {

            var ripple = CreateRipple(_mouse);

            //run new animator for every Down
            //we use this helper task so that every new rangeanimator is disposed properly at the end
            Task.Run(async () =>
            {
                await Parent.AnimateRangeAsync((v) =>
                {
                    ripple.Progress = v;
                    Update();
                }, 0, 1, 4500);

                RemoveRipple(ripple.Uid);

            }).ConfigureAwait(false);

        }

        return null;
    }
}