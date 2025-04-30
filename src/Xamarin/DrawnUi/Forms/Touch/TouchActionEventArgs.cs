using AppoMobi.Maui.Gestures;
using System.Drawing;

namespace AppoMobi.Maui.Gestures
{
	/// <summary>
	/// Everything is in pixels!!! Convert to points if needed
	/// </summary>
	public class TouchActionEventArgs : EventArgs
	{
		public float DeltaTimeMs { get; set; }
		public DateTime Timestamp { get; set; } = DateTime.Now;

		public static void FillDistanceInfo(TouchActionEventArgs current, TouchActionEventArgs previous)
		{
			if (previous == null)
			{
				current.Distance = new DistanceInfo();
				return;
			}

			current.StartingLocation = previous.StartingLocation;
			current.IsInContact = previous.IsInContact;

			current.DeltaTimeMs = (float)(current.Timestamp - previous.Timestamp).TotalMilliseconds;

			var distance = new TouchActionEventArgs.DistanceInfo
			{
				Start = previous.Location,
				End = current.Location,
				Delta = current.Location.Subtract(previous.Location),
			};

			if (current.Type == TouchActionType.Released || current.Type == TouchActionType.Cancelled ||
				current.Type == TouchActionType.Exited)
			{
				//we don't care about delta if it's the last event because it could be anywhere  
				//but velocity would be recalculated based on this event time
				distance = new TouchActionEventArgs.DistanceInfo
				{
					Start = previous.Location,
					End = previous.Location,
					Delta = new(0, 0),
				};
			}

			distance.Total = previous.Distance.Total.Add(distance.Delta);
			current.Distance = distance;

			current.Distance.Velocity = GetVelocity(current, previous);
			distance.TotalVelocity = previous.Distance.TotalVelocity.Add(current.Distance.Velocity);
		}

		public static PointF GetVelocity(TouchActionEventArgs current, TouchActionEventArgs previous)
		{
			var velocity = new PointF(0, 0);

			if (previous != null)
			{
				PointF deltaDistance;
				float deltaSeconds;

				if (current.Distance.Delta.X == 0 && current.Distance.Delta.Y == 0 && (current.Type == TouchActionType.Released || current.Type == TouchActionType.Cancelled || current.Type == TouchActionType.Exited))
				{
					//we gonna recalc velocity based on this event time, usually this is the case of finger released

					var prevDeltaSeconds = 0.0f;
					var prevDeltaSecondsX = previous.Distance.Delta.X / previous.Distance.Velocity.X;
					var prevDeltaSecondsY = previous.Distance.Delta.Y / previous.Distance.Velocity.Y;

					if (!double.IsNaN(prevDeltaSecondsX))
					{
						prevDeltaSeconds = prevDeltaSecondsX;
					}
					else
					if (!double.IsNaN(prevDeltaSecondsY))
					{
						prevDeltaSeconds = prevDeltaSecondsY;
					}
					deltaDistance = new(previous.Distance.Delta.X, previous.Distance.Delta.Y);
					deltaSeconds = (float)((current.Time - previous.Time).TotalMilliseconds / 1000.0f + prevDeltaSeconds);
					velocity = new PointF(deltaDistance.X / deltaSeconds, deltaDistance.Y / deltaSeconds);

					//Debug.WriteLine($"[V-LAST] {current.Type}  distance {deltaDistance.Y:0.0} time {deltaSeconds:0.000} => {velocity.Y}");
				}
				else
				{
					deltaDistance = new(current.Distance.Delta.X, current.Distance.Delta.Y);
					deltaSeconds = (float)((current.Time - previous.Time).TotalMilliseconds / 1000.0f);
					velocity = new PointF(deltaDistance.X / deltaSeconds, deltaDistance.Y / deltaSeconds);
				}
			}

			return velocity;
		}
		/// <summary>
		/// Using Distance.Delta and Time of previous args
		/// </summary>
		/// <param name="previous"></param>
		public void CalculateVelocity(TouchActionEventArgs previous)
		{
			var velocity = GetVelocity(this, previous);
			this.Distance.Velocity = velocity;
		}

		public TouchActionEventArgs(long id, TouchActionType type,
			PointF location,
			object elementBindingContext)
		{
			Id = id;
			Time = DateTime.Now;
			Type = type;
			Location = location;
			Context = elementBindingContext;
			Distance = new DistanceInfo();
		}

		public DateTime Time { get; private set; }

		public TouchActionEventArgs()
		{
			Time = DateTime.Now;
			Distance = new DistanceInfo();
		}



		public PointF GetScaledLocation(float scale)
		{
			return new PointF(Location.X * scale, Location.Y * scale);
		}

		public long Id { private set; get; }

		/// <summary>
		/// This is used in some cases, ex: can set this to true inside LongPressing handler to avoid calling Tapped
		/// </summary>
		public bool PreventDefault { get; set; }

		public TouchActionType Type { private set; get; }

		/// <summary>
		/// In pixels inside parent view,
		/// 0,0 is top-left corner of the view
		/// </summary>
		public PointF Location { set; get; }

		/// <summary>
		/// In pixels inside parent view,
		/// 0,0 is top-left corner of the view
		/// </summary>
		public PointF StartingLocation { set; get; }


		/// <summary>
		/// Gesture started inside view
		/// </summary>
		public bool IsInContact { set; get; }

		/// <summary>
		/// Current hit is inside the view
		/// </summary>
		public bool IsInsideView { set; get; }


		/// <summary>
		/// Parameter to pass to commands
		/// </summary>
		public object Context { get; set; }

		/// <summary>
		/// How many fingers we have down actually
		/// </summary>
		public int NumberOfTouches { get; set; }

		public TouchEffect.WheelEventArgs Wheel { get; set; }

		/// <summary>
		/// In pixels inside parent view,
		/// 0,0 is top-left corner of the view
		/// </summary>
		public DistanceInfo Distance
		{
			get;
			set;
		}

		public ManipulationInfo Manipulation
		{
			get;
			set;
		}

		public record ManipulationInfo(
			PointF Center,
			PointF PreviousCenter,
			double Scale,
			double Rotation,
			double ScaleTotal,
			double RotationTotal,
			int TouchesCount);

		/// <summary>
		/// In pixels inside parent view,
		/// 0,0 is top-left corner of the view
		/// </summary>
		public record DistanceInfo
		{
			public DistanceInfo()
			{
				Delta = PointF.Empty;
				Total = PointF.Empty;
				Start = PointF.Empty;
				End = PointF.Empty;
			}

			/// <summary>
			/// In pixels inside parent view,
			/// 0,0 is top-left corner of the view
			/// </summary>
			public PointF Delta
			{
				get;
				set;
			}

			/// <summary>
			/// In pixels inside parent view,
			/// 0,0 is top-left corner of the view
			/// </summary>
			public virtual PointF Total
			{
				get;
				set;
			}

			/// <summary>
			/// Pixels per second
			/// </summary>
			public virtual PointF TotalVelocity
			{
				get;
				set;
			}

			/// <summary>
			/// Pixels per second
			/// </summary>
			public PointF Velocity
			{
				get;
				set;
			}

			/// <summary>
			/// In pixels inside parent view,
			/// 0,0 is top-left corner of the view
			/// </summary>
			public PointF Start
			{
				get;
				set;
			}

			/// <summary>
			/// In pixels inside parent view,
			/// 0,0 is top-left corner of the view
			/// </summary>
			public PointF End
			{
				get;
				set;
			}

		}

	}
}