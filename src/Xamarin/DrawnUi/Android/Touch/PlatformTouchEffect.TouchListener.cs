using Android.Content;
using Android.Views;
using AppoMobi.Framework.Forms.UI.Touch;
using AppoMobi.Maui.Gestures;
using AppoMobi.Maui.Gestures;
using System;
using System.Drawing;
using Xamarin.Forms;
using View = Android.Views.View;

[assembly: ResolutionGroupName("AMFEffects")]
[assembly: ExportEffect(typeof(PlatformTouchEffect), "TouchEffect")]

namespace AppoMobi.Maui.Gestures;

public partial class PlatformTouchEffect
{

	public class TouchListener : GestureDetector.SimpleOnGestureListener, View.IOnTouchListener
	{

		//public static bool UseLowCpu = false;

		public bool PinchEnabled = true;

		#region ROTATION

		private static readonly float AngleThreshold = 15f; // Adjust the angle threshold as needed
		private float _startAngle;
		bool _isRotating;

		private static float GetAngle(MotionEvent e)
		{
			try
			{
				float dx = e.GetX(0) - e.GetX(1);
				float dy = e.GetY(0) - e.GetY(1);
				return (float)(Math.Atan2(dy, dx) * (180 / Math.PI));
			}
			catch (Exception exception)
			{
				//exception
			}
			return 0;
		}

		#endregion

		#region PINCH

		ScaleListener _scaleListener;

		ScaleGestureDetector scaleGestureDetector;

		public void OnScaleChanged(object sender, ScaleEventArgs e)
		{
			//Debug.WriteLine($"[TOUCH] Android: OnScaleChanged {e.Scale:0.000}");
			_parent.Pinch = e;
		}

		#endregion

		public TouchListener(PlatformTouchEffect platformEffect, Context ctx)
		{
			_parent = platformEffect; //todo remove on dispose

			//!!! todo ADD CLEANUP !!!
			_scaleListener = new(ctx, this);
			scaleGestureDetector = new ScaleGestureDetector(ctx, _scaleListener);
		}

		private volatile PlatformTouchEffect _parent;


		void LockInput(View sender)
		{
			sender.Parent?.RequestDisallowInterceptTouchEvent(true);
			//    Debug.WriteLine($"[****MODE2*] LOCKED");
		}

		void UnlockInput(View sender)
		{
			sender.Parent?.RequestDisallowInterceptTouchEvent(false);
			//      Debug.WriteLine($"[****MODE2*] UN-LOCKED");
		}




		public bool IsPinching { get; set; }

		DateTime _lastEventTime = DateTime.Now;

		public bool OnTouch(View sender, MotionEvent motionEvent)
		{
			//System.Diagnostics.Debug.WriteLine($"[TOUCH] Android: {motionEvent.Action} {motionEvent.RawY:0}");

			//var _parent = GetParent(sender);

			if (_parent.FormsEffect.TouchMode == TouchHandlingStyle.Disabled)
				return false;

			_parent.CountFingers = motionEvent.PointerCount;

			// Get the pointer index
			int pointerIndex = motionEvent.ActionIndex;

			// Get the id that identifies a finger over the course of its progress
			int id = motionEvent.GetPointerId(pointerIndex);

			//Pixels relative to the view, not the screen 
			var coorsInsideView = new PointF(motionEvent.GetX(pointerIndex), motionEvent.GetY(pointerIndex));
			_parent.isInsideView = coorsInsideView.X >= 0 && coorsInsideView.X <= sender.Width && coorsInsideView.Y >= 0 && coorsInsideView.Y <= sender.Height;

			try
			{
				switch (motionEvent.ActionMasked)
				{

				//detect additional pointers (i.e., fingers) going down on the screen after the initial touch.
				//typically used in multi-touch scenarios when multiple fingers are involved.
				case MotionEventActions.PointerDown:
				_parent.FireEvent(id, TouchActionType.Pressed, coorsInsideView);
				break;

				case MotionEventActions.Down:
				//case MotionEventActions.PointerDown:

				if (_parent.FormsEffect.TouchMode == TouchHandlingStyle.Lock)
					LockInput(sender);
				else
					UnlockInput(sender);

				_parent.FireEvent(id, TouchActionType.Pressed, coorsInsideView);

				break;

				case MotionEventActions.Move:

				// Multiple Move events are bundled, so handle them in a loop
				for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
				{
					id = motionEvent.GetPointerId(pointerIndex);

					coorsInsideView = new PointF(motionEvent.GetX(pointerIndex), motionEvent.GetY(pointerIndex));

					_parent.FireEvent(id, TouchActionType.Moved, coorsInsideView);
				}

				break;

				case MotionEventActions.PointerUp:
				_parent.FireEvent(id, TouchActionType.Released,
					coorsInsideView);
				break;

				case MotionEventActions.Up:
				UnlockInput(sender);

				_parent.FireEvent(id, TouchActionType.Released,
					coorsInsideView);

				break;
				case MotionEventActions.Cancel:

				//Debug.WriteLine($"[TOUCH] Android native: {motionEvent.ActionMasked} - {_parent.capture}");

				UnlockInput(sender);

				_parent.FireEvent(id, TouchActionType.Cancelled, coorsInsideView);

				break;

				default:

				UnlockInput(sender);

				break;

				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"[TOUCH] Android: {motionEvent} - {e}");
			}

			return true;
		}


	}
}