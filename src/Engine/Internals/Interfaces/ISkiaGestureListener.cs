namespace DrawnUi.Maui.Draw;

public interface ISkiaGestureListener
{
	/// <summary>
	/// Called when a gesture is detected.  
	/// </summary>
	/// <param name="type"></param>
	/// <param name="args"></param>
	/// <param name="args.Action"></param>
	/// <param name="inside"></param>
	/// <returns>WHO CONSUMED if gesture consumed and blocked to be passed, NULL if gesture not locked and could be passed below.
	/// If you pass this to subview you must set your own offset parameters, do not pass what you received its for this level use.</returns>
	public ISkiaGestureListener OnSkiaGestureEvent(SkiaGesturesParameters args, GestureEventProcessingInfo apply);

	public bool InputTransparent { get; }

	bool CanDraw { get; }

	string Tag { get; }

	Guid Uid { get; }

	int ZIndex { get; }

	DateTime GestureListenerRegistrationTime { get; set; }


	/// <summary>
	/// This will be called only for views registered at Superview.FocusedChild. The view will return it's overriden value to indicate if it accepts focus or grabs it.
	/// </summary>
	/// <param name="focus"></param>
	/// <returns></returns>
	public bool OnFocusChanged(bool focus);

	public bool HitIsInside(float x, float y);
}

