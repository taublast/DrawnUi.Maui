#nullable enable
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request.Target;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Bumptech.Glide.Glide;

namespace Android.Glide
{
    public static class GlideExtensions
    {

        public static IImageSourceHandler? GetHandler(this ImageSource source)
        {
            //Image source handler to return
            IImageSourceHandler? returnValue = null;
            //check the specific source type and return the correct image source handler
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            else if (source is FontImageSource)
            {
                returnValue = new FontImageSourceHandler();
            }

            return returnValue;
        }

        public static async Task<Bitmap?> LoadOriginalViaGlide(this ImageSource source, Context context, CancellationToken token)
        {
            try
            {
                if (source is null)
                {
                    Forms.Debug("`{0}` is null", nameof(ImageSource));
                    return null;
                }

                var isAlive = IsActivityAlive(context, source);

                //		Console.WriteLine($"[LoadOriginalViaGlide] alive {isAlive} source {source}");

                if (!isAlive)
                    return null;

                RequestManager request = With(context);
                RequestBuilder? builder = null;


                switch (source)
                {
                case FileImageSource fileSource:

                builder = HandleFileImageSource(request, fileSource, context);
                break;

                case UriImageSource uriSource:
                builder = HandleUriImageSource(request, uriSource);
                break;

                case StreamImageSource streamSource:
                builder = await HandleStreamImageSource(request, streamSource, token, () => !IsActivityAlive(context, source));
                break;
                }

                var handler = Forms.GlideHandler;
                if (handler != null)
                {
                    Forms.Debug("Calling into {0} of type `{1}`.", nameof(IGlideHandler), handler.GetType());
                    handler.Build(source, ref builder, token);
                }

                if (builder is null)
                    return null;

                var future = builder
                    .Override(Target.SizeOriginal, Target.SizeOriginal)
                    .Submit();


                var result = await Task.Run(() => future.Get(), token);

                if (result is BitmapDrawable drawable)
                    return drawable.Bitmap;

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                //Since developers can't catch this themselves, I think we should log it and silently fail
                Forms.Warn("Unexpected exception in glidex: {0}", exc);
            }
            return null;
        }







        public static async Task LoadViaGlide(this ImageView imageView, ImageSource source, CancellationToken token)
        {
            try
            {
                if (!IsActivityAlive(imageView, source))
                {
                    CancelGlide(imageView);
                    return;
                }

                RequestManager request = With(imageView.Context);
                RequestBuilder? builder = null;

                if (source is null)
                {
                    Forms.Debug("`{0}` is null, clearing image", nameof(ImageSource));
                    Clear(request, imageView);
                    return;
                }

                switch (source)
                {
                case FileImageSource fileSource:
                if (imageView.Context != null)
                    builder = HandleFileImageSource(request, fileSource, imageView.Context);
                break;
                case UriImageSource uriSource:
                builder = HandleUriImageSource(request, uriSource);
                break;
                case StreamImageSource streamSource:
                builder = await HandleStreamImageSource(request, streamSource, token, () => !IsActivityAlive(imageView, source));
                break;
                }

                var handler = Forms.GlideHandler;
                if (handler != null)
                {
                    Forms.Debug("Calling into {0} of type `{1}`.", nameof(IGlideHandler), handler.GetType());
                    if (handler.Build(imageView, source, builder, token))
                    {
                        return;
                    }
                }

                if (builder is null)
                {
                    Clear(request, imageView);
                }
                else
                {
                    imageView.Visibility = ViewStates.Visible;
                    builder.Into(imageView);
                }
            }
            catch (Exception exc)
            {
                //Since developers can't catch this themselves, I think we should log it and silently fail
                Forms.Warn("Unexpected exception in glidex: {0}", exc);
            }
        }

        /// <summary>
        /// Should only be used internally for IImageSourceHandler calls
        /// </summary>
        internal static async Task<Bitmap?> LoadViaGlide(this ImageSource source, Context context, CancellationToken token)
        {
            try
            {
                if (source is null)
                {
                    Forms.Debug("`{0}` is null", nameof(ImageSource));
                    return null;
                }
                if (!IsActivityAlive(context, source))
                    return null;

                RequestManager request = With(context);
                RequestBuilder? builder = null;

                switch (source)
                {
                case FileImageSource fileSource:
                builder = HandleFileImageSource(request, fileSource, context);
                break;

                case UriImageSource uriSource:
                builder = HandleUriImageSource(request, uriSource);
                break;

                case StreamImageSource streamSource:
                builder = await HandleStreamImageSource(request, streamSource, token, () => !IsActivityAlive(context, source));
                break;
                }

                var handler = Forms.GlideHandler;
                if (handler != null)
                {
                    Forms.Debug("Calling into {0} of type `{1}`.", nameof(IGlideHandler), handler.GetType());
                    handler.Build(source, ref builder, token);
                }

                if (builder is null)
                    return null;

                var future = builder.Submit();

                var result = await Task.Run(() => future.Get(), token);

                if (result is BitmapDrawable drawable)
                    return drawable.Bitmap;

            }
            catch (Exception exc)
            {
                //Since developers can't catch this themselves, I think we should log it and silently fail
                Forms.Warn("Unexpected exception in glidex: {0}", exc);
            }
            return null;
        }

        static RequestBuilder HandleFileImageSource(RequestManager request, FileImageSource source, Context context)
        {
            RequestBuilder builder;

            var fileName = source.File;

            var drawable = ResourceManager.GetDrawableId(context, fileName);
            if (drawable != 0)
            {
                Forms.Debug("Loading `{0}` as an Android resource", fileName);
                builder = request.Load(drawable);
            }
            else
            {
                Forms.Debug("Loading `{0}` from disk", fileName);
                builder = request.Load(fileName);
            }
            return builder;
        }

        static RequestBuilder HandleUriImageSource(RequestManager request, UriImageSource source)
        {
            var url = source.Uri.OriginalString;
            Forms.Debug("Loading `{0}` as a web URL", url);
            return request.Load(url);
        }



        static async Task<RequestBuilder?> HandleStreamImageSource(RequestManager request, StreamImageSource source, CancellationToken token, Func<bool> cancelled)
        {
            Forms.Debug("Loading `{0}` as a byte[]. Consider using `AndroidResource` instead, as it would be more performant", nameof(StreamImageSource));

            using var memoryStream = new MemoryStream();
            using var stream = await source.Stream(token);
            if (token.IsCancellationRequested || cancelled())
                return null;
            if (stream is null)
                return null;
            await stream.CopyToAsync(memoryStream, token);
            if (token.IsCancellationRequested || cancelled())
                return null;

            return request
                //.AsBitmap() //bug fixed
                .Load(memoryStream.ToArray());
        }

        static bool IsActivityAlive(ImageView imageView, ImageSource source)
        {
            // The imageView.Handle could be IntPtr.Zero? Meaning we somehow have a reference to a disposed ImageView...
            // I think this is within the realm of "possible" after the await call in LoadViaGlide().
            if (imageView.Handle == IntPtr.Zero)
            {
                Forms.Warn("imageView.Handle is IntPtr.Zero, aborting image load for `{0}`.", source);
                return false;
            }

            return IsActivityAlive(imageView.Context, source);
        }

        /// <summary>
        /// NOTE: see https://github.com/bumptech/glide/issues/1484#issuecomment-365625087
        /// </summary>
        static bool IsActivityAlive(Context? context, ImageSource source)
        {
            //NOTE: in some cases ContextThemeWrapper is Context
            var activity = context as Activity ?? Forms.Activity;
            if (activity != null)
            {
                if (activity.IsFinishing)
                {
                    Forms.Warn("Activity of type `{0}` is finishing, aborting image load for `{1}`.", activity.GetType().FullName, source);
                    return false;
                }
                if (activity.IsDestroyed)
                {
                    Forms.Warn("Activity of type `{0}` is destroyed, aborting image load for `{1}`.", activity.GetType().FullName, source);
                    return false;
                }
            }
            else if (context != null)
            {
                Forms.Warn("Context `{0}` is not an Android.App.Activity and could not use Android.Glide.Forms.Activity, aborting image load for `{1}`.", context, source);
                return false;
            }
            else
            {
                Forms.Warn("Context is null and could not use Android.Glide.Forms.Activity, aborting image load for `{0}`.", source);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Cancels the Request and "clears" the ImageView
        /// </summary>
        static void Clear(RequestManager request, ImageView imageView)
        {
            imageView.Visibility = ViewStates.Gone;
            imageView.SetImageBitmap(null);

            //We need to call Clear for Glide to know this image is now unused
            //https://bumptech.github.io/glide/doc/targets.html
            request.Clear(imageView);
        }

        internal static void CancelGlide(this ImageView imageView)
        {
            if (imageView.Handle == IntPtr.Zero)
            {
                return;
            }

            //NOTE: we may be doing a Cancel after the Activity has just exited
            // To make this work we have to use the Application.Context
            With(App.Application.Context).Clear(imageView);
        }
    }
}
