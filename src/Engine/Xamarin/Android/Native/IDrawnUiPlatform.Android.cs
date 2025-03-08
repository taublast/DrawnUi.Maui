using Android.App;
using Android.Glide;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using AppoMobi.Specials;
using AppoMobi.Xamarin.DrawnUi.Droid;
using Bumptech.Glide;
using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Views;
using SkiaSharp;
using SkiaSharp.Views.Android;
using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;

[assembly: Xamarin.Forms.Dependency(typeof(AppoMobi.Xamarin.DrawnUi.Droid.DrawnUi))]

namespace AppoMobi.Xamarin.DrawnUi.Droid
{
	[Preserve(AllMembers = true)]
	public class DrawnUi : IDrawnUiPlatform
	{
		public DrawnUi()
		{

		}

		public bool CheckNativeVisibility(object handler)
		{
			if (handler is Android.Views.View nativeView)
			{
				if (nativeView.Visibility != Android.Views.ViewStates.Visible)
				{
					return false;
				}
			}

			return true;
		}

		public static void Initialize<T>(Activity activity) where T : Application
		{
			Activity = activity;

			Super.AppAssembly = typeof(T).Assembly;

			if (!DisableCache)
				Android.Glide.Forms.Init(activity);

			/*
		Tasks.StartDelayed(TimeSpan.FromMilliseconds(250), async () =>
		{
			_frameCallback = new FrameCallback((nanos) =>
			{
				OnFrame?.Invoke(null, null);
				Choreographer.Instance.PostFrameCallback(_frameCallback);
			});

			while (!_loopStarted)
			{
				try
				{
					MainThread.BeginInvokeOnMainThread(async () =>
					{
						if (_loopStarting)
							return;

						_loopStarting = true;

						if (MainThread.IsMainThread) //Choreographer is available
						{
							if (!_loopStarted)
							{
								_loopStarted = true;
								Choreographer.Instance.PostFrameCallback(_frameCallback);
							}
						}
						_loopStarting = false;
					});
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					throw;
				}

				await Task.Delay(100);
			}
		});
		*/

			Looper = new(() =>
			{
				OnFrame?.Invoke(null, null);
			});

			Looper.StartOnMainThread(120);
		}

		private static FrameCallback _frameCallback;
		static bool _loopStarting = false;
		static bool _loopStarted = false;
		public static event EventHandler OnFrame;


		static Looper Looper { get; set; }

		public void RegisterLooperCallback(EventHandler callback)
		{
			OnFrame += callback;
		}

		public void UnregisterLooperCallback(EventHandler callback)
		{
			OnFrame -= callback;
		}

		public static Activity Activity { get; protected set; }

		public static bool DisableCache;

		async Task<SKBitmap> IDrawnUiPlatform.LoadImageOnPlatformAsync(ImageSource source, CancellationToken cancel)
		{
			if (source == null)
				return null;

			Bitmap androidBitmap = null;
			try
			{
				if (DisableCache)
				{
					if (source is UriImageSource uri)
					{
						using var client = new WebClient();
						var data = await client.DownloadDataTaskAsync(uri.Uri);
						return SKBitmap.Decode(data);
					}

					var handler = source.GetHandler();
					androidBitmap = await handler.LoadImageAsync(source, Android.App.Application.Context, cancel);
				}
				else
				{
					androidBitmap = await source.LoadOriginalViaGlide(Android.App.Application.Context, cancel);
				}

				if (androidBitmap != null)
				{
					return androidBitmap.ToSKBitmap();
				}
			}
			catch (Exception e)
			{
				Super.Log($"[LoadSKBitmapAsync] {e}");
			}

			return null;
		}

		void IDrawnUiPlatform.ClearImagesCache()
		{
			if (DisableCache)
				return;

			var glide = Glide.Get(Activity);

			Task.Run(async () =>
			{
				glide.ClearDiskCache();
			}).ConfigureAwait(false);

			Device.BeginInvokeOnMainThread(() =>
			{
				glide.ClearMemory();
			});
		}

		public class FrameCallback : Java.Lang.Object, Choreographer.IFrameCallback
		{
			public FrameCallback(Action<long> callback)
			{
				_callback = callback;
			}

			Action<long> _callback;

			public void DoFrame(long frameTimeNanos)
			{
				_callback?.Invoke(frameTimeNanos);
			}

		}
	}
}