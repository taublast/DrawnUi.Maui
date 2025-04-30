using AppoMobi.Maui.Gestures;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace AppoMobi.Maui.Gestures;

public class MultitouchTracker
{
	private readonly Dictionary<long, PointF> _positions = new();

	private double _totalScaleChange;
	private double _totalRotationChange;
	private TouchState? _touchState;
	private TouchState? _previousTouchState;

	private TouchState GetTouchState()
	{
		var positions = _positions.Values.ToArray();

		if (positions.Length == 0)
			return null;

		if (positions.Length == 1)
			return new TouchState(positions[0], null, null, positions.Length);

		var (centerX, centerY) = GetCenter(positions);
		var radius = Distance(centerX, centerY, positions[0].X, positions[0].Y);
		var angle = Math.Atan2(positions[1].Y - positions[0].Y, positions[1].X - positions[0].X) * 180.0 / Math.PI;

		return new TouchState(new PointF((float)centerX, (float)centerY), radius, angle, positions.Length);
	}

	private static double Distance(double x1, double y1, double x2, double y2)
		=> Math.Sqrt(Math.Pow(x1 - x2, 2.0) + Math.Pow(y1 - y2, 2.0));

	private static (double centerX, double centerY) GetCenter(ReadOnlySpan<PointF> touches)
	{
		double centerX = 0;
		double centerY = 0;

		foreach (var location in touches)
		{
			centerX += location.X;
			centerY += location.Y;
		}

		centerX /= touches.Length;
		centerY /= touches.Length;

		return (centerX, centerY);
	}

	/// <summary>
	/// For on first Down
	/// </summary>
	public void Restart(long id, PointF position)
	{
		Reset();
		_positions[id] = position;
		if (_positions.Count == 1) // Not sure if this check is necessary.
		{
			_totalRotationChange = 0;
			_totalScaleChange = 0;
			_touchState = GetTouchState();
			_previousTouchState = null;
		}
	}

	/// <summary>
	/// For on last Up
	/// </summary>
	public void Reset()
	{
		_positions.Clear();
	}

	/// <summary>
	/// For Panning
	/// </summary>
	/// <param name="id"></param>
	/// <param name="position"></param>
	/// <returns></returns>
	public TouchActionEventArgs.ManipulationInfo AddMovement(long id, PointF position)
	{
		//if (!_positions.ContainsKey(id) && _positions.Count > 0)
		//{
		//    Debug.WriteLine($"[TOUCH] added NEW id {id}, total: {_positions.Count + 1}");
		//}

		_positions[id] = position;

		return Calculate();
	}

	public void RemoveTouch(long id)
	{
		if (_positions.Remove(id))
		{
			//Debug.WriteLine($"[TOUCH] Removed id {id}, total: {_positions.Count}");
		}
	}

	public TouchActionEventArgs.ManipulationInfo Calculate()
	{
		var touchState = GetTouchState();

		_previousTouchState = _touchState;
		_touchState = touchState;

		if (_positions.Count == 1)
			return null; // Will not change anything so don't return a manipulation.

		if (!(touchState?.LocationsLength == _previousTouchState?.LocationsLength))
		{
			// If the finger count changes this is considered a reset.
			_totalRotationChange = 0;
			_totalScaleChange = 0;
			_previousTouchState = null;
			// Note, there is the unlikely change that one finger is lifted exactly when 
			// another is touched down. This should also be ignored, but we can only
			// do that if we had the touch ids. We accept this problem. It will not crash the system.
			return null;
		}

		if (touchState is null)
		{
			_totalScaleChange = 0;
			_totalRotationChange = 0;
		}


		if (_touchState is null)
			return null;

		if (_previousTouchState is null)
			return null; // There is a touch but no previous touch so no manipulation.

		var scaleChange = _touchState.GetScaleChange(_previousTouchState);
		var rotationChange = _touchState.GetRotationChange(_previousTouchState);

		if (touchState is not null && _previousTouchState is not null)
		{
			_totalRotationChange += rotationChange;
			_totalScaleChange += scaleChange;
		}

		if (_touchState.Equals(_previousTouchState))
			return null; // The default will not change anything so don't return a manipulation.

		return new TouchActionEventArgs.ManipulationInfo(_touchState.Center, _previousTouchState.Center, scaleChange, rotationChange, _totalScaleChange, _totalRotationChange, _positions.Count);
	}

	private record TouchState(PointF Center, double? Radius, double? Angle, int LocationsLength)
	{
		public double GetRotationChange(TouchState previousTouchState)
		{
			if (Angle is null)
				return 0;
			if (previousTouchState.Angle is null)
				return 0;
			return Angle.Value - previousTouchState.Angle.Value;
		}

		public double GetScaleFactor(TouchState previousTouchState)
		{
			if (Radius is null)
				return 1;
			if (previousTouchState.Radius is null)
				return 1;
			return Radius.Value / previousTouchState.Radius.Value;
		}

		public double GetScaleChange(TouchState previousTouchState)
		{
			if (Radius is null)
				return 0;
			if (previousTouchState.Radius is null)
				return 0;
			var ratio = Radius.Value / previousTouchState.Radius.Value;
			if (ratio == 1)
			{
				return 0;
			}
			if (ratio < 1)
			{
				return ratio - 1;
			}
			return ratio - 1;
		}
	}
}