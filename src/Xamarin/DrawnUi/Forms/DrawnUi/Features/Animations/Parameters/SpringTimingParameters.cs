using System;
using System.Runtime.CompilerServices;

namespace DrawnUi.Draw
{

	public class SpringTimingParameters : IDampingTimingParameters
	{
		public Spring Spring { get; }
		public float Displacement { get; }
		public float InitialVelocity { get; }
		public float Threshold { get; }
		private IDampingTimingParameters Impl { get; }

		public SpringTimingParameters(Spring spring, float displacement, float initialVelocity, float threshold)
		{
			if (spring.DampingRatio <= 0 || spring.DampingRatio > 1)
			{
				throw new ArgumentException("dampingRatio should be greater than 0 and less than or equal to 1");
			}

			Spring = spring;
			Displacement = displacement;
			InitialVelocity = initialVelocity;
			Threshold = threshold;

			if (spring.DampingRatio >= 1)
			{
				Impl = new CriticallyDampedSpringTimingParameters(spring, displacement, initialVelocity, threshold);
			}
			else
			{
				Impl = new UnderdampedSpringTimingParameters(spring, displacement, initialVelocity, threshold);
			}
		}

		public float DurationSecs => Impl.DurationSecs;

		public float ValueAt(float offsetSecs)
		{
			return Impl.ValueAt(offsetSecs);
		}

		public float AmplitudeAt(float offsetSecs)
		{
			return Impl.AmplitudeAt(offsetSecs);
		}
	}


	public class CriticallyDampedSpringTimingParameters : IDampingTimingParameters
	{
		private readonly Spring _spring;
		private readonly float _displacement;
		private readonly float _initialVelocity;
		private readonly float _threshold;

		public CriticallyDampedSpringTimingParameters(Spring spring, float displacement, float initialVelocity, float threshold)
		{
			_spring = spring;
			_displacement = displacement;
			_initialVelocity = initialVelocity;
			_threshold = threshold;
		}

		public float DurationSecs
		{
			get
			{
				if (_displacement == 0 && _initialVelocity == 0)
				{
					return 0;
				}

				float b = Beta();
				var e = Math.Exp(1);

				var t1 = 1 / b * Math.Log(2 * Math.Abs(_c1) / _threshold);
				var t2 = 2 / b * Math.Log(4 * Math.Abs(_c2) / (e * b * _threshold));

				return (float)Math.Max(t1, t2);
			}
		}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public float ValueAt(float offsetSecs)
		//{
		//    float t = offsetSecs;
		//    return MathF.Exp(-Beta() * t) * (_c1 + _c2 * t);
		//}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public float AmplitudeAt(float offsetSecs)
		//{
		//    var value = ValueAt(offsetSecs);
		//    return MathF.Abs(value);
		//}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float ValueAt(float offsetSecs)
		{
			float t = offsetSecs;
			double expValue = Math.Exp(-Beta() * t);
			float expValueFloat = (float)expValue;
			return expValueFloat * (_c1 + _c2 * t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float AmplitudeAt(float offsetSecs)
		{
			var value = ValueAt(offsetSecs);
			return Math.Abs(value);
		}

		private float _c1 => _displacement;
		private float _c2 => _initialVelocity + Beta() * _displacement;

		private float Beta()
		{
			return _spring.DampingRatio * NaturalFrequency() * 2;
		}

		private float NaturalFrequency()
		{
			return (float)Math.Sqrt(_spring.Stiffness / _spring.Mass);
		}
	}

	public class UnderdampedSpringTimingParameters : IDampingTimingParameters
	{
		private readonly Spring _spring;
		private readonly float _displacement;
		private readonly float _initialVelocity;
		private readonly float _threshold;

		public UnderdampedSpringTimingParameters(Spring spring, float displacement, float initialVelocity, float threshold)
		{
			_spring = spring;
			_displacement = displacement;
			_initialVelocity = initialVelocity;
			_threshold = threshold;
		}

		public float DurationSecs
		{
			get
			{
				if (_displacement == 0 && _initialVelocity == 0)
				{
					return 0;
				}

				return (float)(Math.Log((Math.Abs(_c1) + Math.Abs(_c2)) / _threshold) / Beta());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float ValueAt(float offsetSecs)
		{
			float t = offsetSecs;
			float wd = DampedNaturalFrequency();
			return (float)(Math.Exp(-Beta() * t) * (_c1 * Math.Cos(wd * t) + _c2 * Math.Sin(wd * t)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float AmplitudeAt(float offsetSecs)
		{
			var value = ValueAt(offsetSecs);
			return Math.Abs(value);
		}

		private float _c1 => _displacement;
		private float _c2 => (_initialVelocity + Beta() * _displacement) / DampedNaturalFrequency();

		private float Beta()
		{
			return _spring.DampingRatio * NaturalFrequency() * 2;
		}

		private float NaturalFrequency()
		{
			return (float)Math.Sqrt(_spring.Stiffness / _spring.Mass);
		}

		private float DampedNaturalFrequency()
		{
			return (float)(NaturalFrequency() * Math.Sqrt(1 - _spring.DampingRatio * _spring.DampingRatio));
		}
	}




}