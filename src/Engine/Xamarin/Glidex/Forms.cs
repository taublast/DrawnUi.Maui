#nullable enable
using Android.App;
using Android.OS;
using System;

namespace Android.Glide
{
	/// <summary>
	/// Class for initializing glidex.forms
	/// </summary>
	public static class Forms
	{
		internal static Activity? Activity { get; private set; }

		internal static IGlideHandler? GlideHandler { get; private set; }

		static readonly ActivityLifecycle lifecycle = new ActivityLifecycle ();

		/// <summary>
		/// Initializes glidex.forms, put this after your `Xamarin.Forms.Forms.Init (this, bundle);` call.
		/// </summary>
		/// <param name="debug">Enables debug logging. Turn this on to verify Glide is being used in your app.</param>
		[Obsolete ("Use Forms.Init(Activity,bool) instead.")]
		public static void Init (bool debug = false)
		{
			Init ((Activity)Xamarin.Forms.Forms.Context, debug: debug);
		}

		/// <summary>
		/// Initializes glidex.forms, put this after your `Xamarin.Forms.Forms.Init (this, bundle);` call.
		/// </summary>
		/// <param name="activity">The MainActivity of your application.</param>
		/// <param name="handler">An implementation of IGlideHandler for customizing calls to Glide.</param>
		/// <param name="debug">Enables debug logging. Turn this on to verify Glide is being used in your app.</param>
		public static void Init (Activity activity, IGlideHandler? handler = default, bool debug = false)
		{
			Activity = activity;
			GlideHandler = handler;
			IsDebugEnabled = debug;
			activity.Application?.RegisterActivityLifecycleCallbacks (lifecycle);
		}

		/// <summary>
		/// A flag indicating if Debug logging is enabled
		/// </summary>
		public static bool IsDebugEnabled {
			get;
			private set;
		}

		const string Tag = "glidex";

		internal static void Warn (string format, params object [] args)
		{
			Util.Log.Warn (Tag, format, args);
		}

		internal static void Debug (string format, params object [] args)
		{
			if (IsDebugEnabled)
				Util.Log.Debug (Tag, format, args);
		}

		class ActivityLifecycle : Java.Lang.Object, Application.IActivityLifecycleCallbacks
		{
			public void OnActivityCreated (Activity activity, Bundle? savedInstanceState) { }

			public void OnActivityDestroyed (Activity activity)
			{
				if (Activity == activity) {
					Activity = null;
				}
			}

			public void OnActivityPaused (Activity activity) { }

			public void OnActivityResumed (Activity activity) { }

			public void OnActivitySaveInstanceState (Activity activity, Bundle outState) { }

			public void OnActivityStarted (Activity activity) { }

			public void OnActivityStopped (Activity activity) { }
		}
	}
}
