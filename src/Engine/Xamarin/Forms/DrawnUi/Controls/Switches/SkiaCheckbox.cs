
using AppoMobi.Maui.Gestures;

namespace DrawnUi.Maui.Draw;

/// <summary>
/// Switch-like control, can include any content inside. It's aither you use default content (todo templates?..)
/// or can include any content inside, and properties will by applied by convention to a SkiaShape with Tag `Frame`, SkiaShape with Tag `Thumb`. At the same time you can override ApplyProperties() and apply them to your content yourself.
/// </summary>
public class SkiaCheckbox : SkiaToggle
{
	#region DEFAULT CONTENT

	protected override void CreateDefaultContent()
	{
		// TODO
		/*
        //todo can make different upon platform!
        if (!DefaultChildrenCreated && this.Views.Count == 0)
        {
            if (CreateChildren == null)
            {
                DefaultChildrenCreated = true;

                if (this.WidthRequest < 0)
                    this.WidthRequest = 50;
                if (this.HeightRequest < 0)
                    this.HeightRequest = 32;

                var shape = new SkiaShape
                {
                    Tag = "Frame",
                    Type = ShapeType.Rectangle,
                    CornerRadius = 20,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                };
                this.AddSubView(shape);

                this.AddSubView(new SkiaShape()
                {
                    UseCache = SkiaCacheType.Operations,
                    Type = ShapeType.Circle,
                    Margin = 2,
                    LockRatio = -1,
                    Tag = "Thumb",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Fill,
                });

                var hotspot = new SkiaHotspot()
                {
                    TransformView = this.Thumb,
                };
                hotspot.Tapped += (s, e) =>
                {
                    IsToggled = !IsToggled;
                };
                this.AddSubView(hotspot);

                ApplyProperties();
            }

        }
        */
	}

	#endregion

	protected override void OnLayoutChanged()
	{
		base.OnLayoutChanged();

		ApplyProperties();
	}

	public virtual void ApplyOff()
	{
		if (ViewOn != null)
		{
			ViewOn.IsVisible = false;
		}
	}

	public virtual void ApplyOn()
	{
		if (ViewOn != null)
		{
			ViewOn.IsVisible = true;
		}
	}

	public SkiaControl ViewOff;
	public SkiaControl ViewOn;


	protected virtual void FindViews()
	{
		ViewOn = FindView<SkiaControl>("ViewOn");
		ViewOff = FindView<SkiaControl>("ViewOff");
	}

	public override void ApplyProperties()
	{
		if (ViewOn == null)
		{
			FindViews();
		}

		if (IsToggled)
		{
			ApplyOn();
		}
		else
		{
			ApplyOff();
		}
	}

	public override ISkiaGestureListener ProcessGestures(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
	{
		if (args.Type == TouchActionResult.Tapped)
		{
			IsToggled = !IsToggled;
			return this;
		}

		return base.ProcessGestures(args, apply);
	}


}