namespace DrawnUi.Maui.Draw;

public interface ISkiaGestureProcessor
{
	ISkiaGestureListener ProcessGestures(
		SkiaGesturesParameters args,
		GestureEventProcessingInfo apply);
}