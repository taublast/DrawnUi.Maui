using System.Numerics;

namespace Sandbox.Views.Controls;

/// <summary>
/// Sublclassed SkiaCarousel showing a shader effect for transitions
/// </summary>
public class CarouselWithTransitions : SkiaCarousel
{
    public CarouselWithTransitions()
    {
        Effect = new()
        {
            ShaderSource = ShaderFilename,
            ShaderTemplate = @"Shaders\transitions\_template.sksl"
        };
    }

    private string _ShaderFilename = @"Shaders\transitions\fade.sksl";
    public string ShaderFilename
    {
        get
        {
            return _ShaderFilename;
        }
        set
        {
            if (_ShaderFilename != value)
            {
                _ShaderFilename = value;

                if (Effect != null)
                    Effect.ShaderSource = value;

                OnPropertyChanged();
            }
        }
    }

    private ShaderTransitionEffect Effect { get; }

    public override void Render(SkiaDrawingContext context, SKRect destination, float scale)
    {
        if (!this.VisualEffects.Contains(Effect))
        {
            VisualEffects.Add(Effect); //all the magic will be done with this effect
        }

        base.Render(context, destination, scale);
    }

    protected virtual void OnFromToChanged()
    {

    }



    public virtual void SetupFromTo()
    {
        IndexToLast = IndexTo;
        IndexFromLast = IndexFrom;

        var viewFrom = ChildrenFactory.GetChildAt(IndexFrom);
        var viewTo = ChildrenFactory.GetChildAt(IndexTo);

        if (viewFrom == null || viewTo == null)
        {
            throw new ApplicationException("Unexpected null");
        }

        Effect.ControlFrom = viewFrom;
        Effect.ControlTo = viewTo;

        //Debug.WriteLine($"Set new sources {IndexFrom} ({viewFrom.BindingContext}) <=> {IndexTo} ({viewTo.BindingContext}) at progress {progress:0.00}, scroll {ScrollProgress:0.00}");
    }

    private bool initialized;

    protected override void OnChildrenInitialized()
    {

        IndexFrom = -1;
        IndexTo = -1;
        IndexFromLast = -1;
        IndexToLast = -1;
        initialized = false;

        base.OnChildrenInitialized();
    }

    protected override void OnScrollProgressChanged()
    {
        if (!initialized || ScrollProgress >= 0 && ScrollProgress <= 1) //ignore bouncing
        {
            var currentIndex = 0;
            if (ScrollProgress > 0)
                currentIndex = (int)Math.Floor((MaxIndex) * this.ScrollProgress);

            var progress = this.TransitionProgress;

            if (IndexFrom != currentIndex || !initialized)
            {
                if (currentIndex < MaxIndex)
                {
                    IndexTo = currentIndex + 1;
                    IndexFrom = currentIndex;

                    if (IndexToLast != IndexTo || IndexFromLast != IndexFrom)
                    {
                        SetupFromTo();
                    }

                }
                else
                {
                    progress = 1.0;
                }

                OnFromToChanged();
            }

            initialized = true;

            Effect.Progress = progress;

            Effect.Update();
        }

    }



    //to skip default slides animation via translation, not calling base
    protected override void AnimateVisibleChild(SkiaControl view, Vector2 position)
    {
    }


    private int IndexFrom = -1;
    private int IndexTo = -1;
    private int IndexFromLast = -1;
    private int IndexToLast = -1;

}