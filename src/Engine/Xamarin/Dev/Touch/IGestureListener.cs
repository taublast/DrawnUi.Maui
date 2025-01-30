using AppoMobi.Maui.Gestures;

namespace AppoMobi.Maui.Gestures
{
	public interface IGestureListener
	{
		public void OnGestureEvent(
			TouchActionType type,
			TouchActionEventArgs args,
			TouchActionResult action);

		public bool InputTransparent { get; }
	}
}