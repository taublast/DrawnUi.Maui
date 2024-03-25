using DrawnUi.Maui.Infrastructure.Xaml;
using EasyCaching.Core;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace DrawnUi.Maui.Draw;

public interface IHasBanner
{
    /// <summary>
    /// Main image
    /// </summary>
    public string Banner { get; set; }

    /// <summary>
    /// Indicates that it's already preloading
    /// </summary>
    public bool BannerPreloadOrdered { get; set; }
}

public enum LoadPriority
{
    Low,
    Normal,
    High
}

public partial class SkiaImageManager : IDisposable
{

    #region HELPERS

    public virtual void PreloadImage(ImageSource source, CancellationTokenSource cancel = default)
    {
        try
        {
            if (cancel == null)
            {
                cancel = new();
            }

            if (source != null && !cancel.IsCancellationRequested)
            {
                Preload(source, cancel).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Super.Log(e);
        }
    }

    public virtual void PreloadImage(string source, CancellationTokenSource cancel = default)
    {
        try
        {
            if (cancel == null)
            {
                cancel = new();
            }

            if (!string.IsNullOrEmpty(source) && !cancel.IsCancellationRequested)
            {
                Preload(FrameworkImageSourceConverter.FromInvariantString(source), cancel).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Super.Log(e);
        }
    }

    public virtual void PreloadImages(IList<string> list, CancellationTokenSource cancel = default)
    {
        try
        {
            if (cancel == null)
            {
                cancel = new();
            }

            if (list.Count > 0 && !cancel.IsCancellationRequested)
            {
                var index = 0;
                var item = list[index];
                while (!cancel.IsCancellationRequested)
                {
                    Preload(FrameworkImageSourceConverter.FromInvariantString(item), cancel).ConfigureAwait(false);
                    index++;
                    if (index > list.Count - 1)
                    {
                        break;
                    }
                    item = list[index];
                }
            }
        }
        catch (Exception e)
        {
            Super.Log(e);
        }
    }

    public virtual void PreloadImages(IList<ImageSource> list, CancellationTokenSource cancel = default)
    {
        try
        {
            if (cancel == null)
            {
                cancel = new();
            }

            if (list.Count > 0 && !cancel.IsCancellationRequested)
            {
                var index = 0;
                var item = list[index];
                while (!cancel.IsCancellationRequested)
                {
                    Preload(item, cancel).ConfigureAwait(false);
                    index++;
                    if (index > list.Count - 1)
                    {
                        break;
                    }
                    item = list[index];
                }
            }
        }
        catch (Exception e)
        {
            Super.Log(e);
        }
    }

    public virtual void PreloadBanners<T>(IList<T> list, CancellationTokenSource cancel = default) where T : IHasBanner
    {
        try
        {
            if (cancel == null)
            {
                cancel = new();
            }

            if (list.Count > 0 && !cancel.IsCancellationRequested)
            {
                var index = 0;
                var item = list[index];
                while (!cancel.IsCancellationRequested)
                {
                    if (!item.BannerPreloadOrdered)
                    {
                        item.BannerPreloadOrdered = true;
                        Preload(item.Banner, cancel).ConfigureAwait(false);
                    }
                    index++;
                    if (index > list.Count - 1)
                    {
                        break;
                    }
                    item = list[index];
                }
            }
        }
        catch (Exception e)
        {
            Super.Log(e);
        }

    }

    #endregion



    /// <summary>
    /// If set to true will not return clones for same sources, but will just return the existing cached SKBitmap reference. Useful if you have a lot on images reusing same sources, but you have to be carefull not to dispose the shared image. SkiaImage is aware of this setting and will keep a cached SKBitmap from being disposed.
    /// </summary>
    public static bool ReuseBitmaps = false;

    /// <summary>
    /// Caching provider setting
    /// </summary>
    public static int CacheLongevitySecs = 1800; //30mins

    /// <summary>
    /// Convention for local files saved in native platform. Shared resources from Resources/Raw/ do not need this prefix.
    /// </summary>
    public static string NativeFilePrefix = "file://";

    public event EventHandler CanReload;

    private readonly IEasyCachingProvider _cachingProvider;

    public static bool LogEnabled = false;

    public static void TraceLog(string message)
    {
        if (LogEnabled)
        {
#if WINDOWS
            Trace.WriteLine(message);
#else
            Console.WriteLine("*******************************************");
            Console.WriteLine(message);
#endif
        }
    }

    static SkiaImageManager _instance;
    private static int _loadingTasksCount;
    private static int _queuedTasksCount;

    public static SkiaImageManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SkiaImageManager();

            return _instance;
        }
    }

    public SkiaImageManager()
    {
        var factory = Super.Services.GetService<IEasyCachingProviderFactory>();
        _cachingProvider = factory.GetCachingProvider("skiaimages");

        var connected = Connectivity.Current.NetworkAccess;
        if (connected != NetworkAccess.Internet
            && connected != NetworkAccess.ConstrainedInternet)
        {
            IsOffline = true;
        }

        Tasks.StartDelayed(TimeSpan.FromMilliseconds(100), () =>
        {
            LaunchProcessQueue();
        });
    }


    private SemaphoreSlim semaphoreLoad = new(16, 16);

    private readonly object lockObject = new object();

    private bool _isLoadingLocked;
    public bool IsLoadingLocked
    {
        get => _isLoadingLocked;
        set
        {
            if (_isLoadingLocked != value)
            {
                _isLoadingLocked = value;
            }
        }
    }


    public void CancelAll()
    {
        //lock (lockObject)
        {
            while (_queue.Count > 0)
            {
                if (_queue.TryDequeue(out var item, out LoadPriority priority))
                    item.Cancel.Cancel();
            }
        }
    }

    public record QueueItem
    {
        public QueueItem(ImageSource source, CancellationTokenSource cancel, TaskCompletionSource<SKBitmap> task)
        {
            Source = source;
            Cancel = cancel;
            Task = task;
        }

        public ImageSource Source { get; init; }
        public CancellationTokenSource Cancel { get; init; }
        public TaskCompletionSource<SKBitmap> Task { get; init; }
    }

    private readonly SortedDictionary<LoadPriority, Queue<QueueItem>> _priorityQueue = new();

    private readonly PriorityQueue<QueueItem, LoadPriority> _queue = new();

    private readonly ConcurrentDictionary<string, Task<SKBitmap>> _trackLoadingBitmapsUris = new();

    //todo avoid conflicts, cannot use concurrent otherwise will loose data
    private readonly Dictionary<string, Stack<QueueItem>> _pendingLoadsLow = new();
    private readonly Dictionary<string, Stack<QueueItem>> _pendingLoadsNormal = new();
    private readonly Dictionary<string, Stack<QueueItem>> _pendingLoadsHigh = new();

    private Dictionary<string, Stack<QueueItem>> GetPendingLoadsDictionary(LoadPriority priority)
    {
        return priority switch
        {
            LoadPriority.Low => _pendingLoadsLow,
            LoadPriority.Normal => _pendingLoadsNormal,
            LoadPriority.High => _pendingLoadsHigh,
            _ => _pendingLoadsNormal,
        };
    }


    /// <summary>
    /// Direct load, without any queue or manager cache, for internal use. Please use LoadImageManagedAsync instead.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual Task<SKBitmap> LoadImageAsync(ImageSource source, CancellationToken token)
    {
        return LoadImageOnPlatformAsync(source, token);
    }

    /// <summary>
    /// Uses queue and manager cache
    /// </summary>
    /// <param name="source"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual Task<SKBitmap> LoadImageManagedAsync(ImageSource source, CancellationTokenSource token, LoadPriority priority = LoadPriority.Normal)
    {

        var tcs = new TaskCompletionSource<SKBitmap>();

        string uri = null;

        if (!source.IsEmpty)
        {
            if (source is UriImageSource sourceUri)
            {
                uri = sourceUri.Uri.ToString();
            }
            else
            if (source is FileImageSource sourceFile)
            {
                uri = sourceFile.File;
            }

            // 1 Try to get from cache
            var cacheKey = uri;

            var cachedBitmap = _cachingProvider.Get<SKBitmap>(cacheKey);
            if (cachedBitmap.HasValue)
            {
                if (ReuseBitmaps)
                {
                    tcs.TrySetResult(cachedBitmap.Value);
                }
                else
                {
                    tcs.TrySetResult(cachedBitmap.Value.Copy());
                }
                TraceLog($"ImageLoadManager: Returning cached bitmap for UriImageSource {uri}");

                //if (pendingLoads.Any(x => x.Value.Count != 0))
                //{
                //    RunProcessQueue();
                //}

                return tcs.Task;
            }
            TraceLog($"ImageLoadManager: Not found cached UriImageSource {uri}");

            // 2 put to queue
            var tuple = new QueueItem(source, token, tcs);

            if (uri == null)
            {
                //no queue, maybe stream
                TraceLog($"ImageLoadManager: DIRECT ExecuteLoadTask !!!");
                Tasks.StartDelayedAsync(TimeSpan.FromMicroseconds(1), async () =>
                {
                    await ExecuteLoadTask(tuple);
                });
            }
            else
            {
                var urlAlreadyLoading = _trackLoadingBitmapsUris.ContainsKey(uri);
                if (urlAlreadyLoading)
                {
                    // we're currently loading the same image, save the task to pendingLoads
                    TraceLog($"ImageLoadManager: Same image already loading, pausing task for UriImageSource {uri}");

                    var pendingLoads = GetPendingLoadsDictionary(priority);

                    if (pendingLoads.TryGetValue(uri, out var stack))
                    {
                        stack.Push(tuple);
                    }
                    else
                    {
                        var pendingStack = new Stack<QueueItem>();
                        pendingStack.Push(tuple);
                        pendingLoads[uri] = pendingStack;
                    }

                }
                else
                {
                    // We're about to load this image, so add its Task to the loadingBitmaps dictionary
                    _trackLoadingBitmapsUris[uri] = tcs.Task;

                    lock (lockObject)
                    {
                        _queue.Enqueue(tuple, priority);
                    }

                    TraceLog($"ImageLoadManager: Enqueued {uri} (queue {_queue.Count})");
                }

            }



        }

        return tcs.Task;
    }

    void LaunchProcessQueue()
    {
        Task.Run(async () =>
        {
            ProcessQueue();

        }).ConfigureAwait(false);
    }

#if (!ONPLATFORM)

    public static async Task<SKBitmap> LoadImageOnPlatformAsync(ImageSource source, CancellationToken cancel)
    {
        throw new NotImplementedException();
    }

#endif

    private void ProcessPendingLoads()
    {
        ProcessPendingLoadsForPriority(LoadPriority.High);
        ProcessPendingLoadsForPriority(LoadPriority.Normal);
        ProcessPendingLoadsForPriority(LoadPriority.Low);
    }

    private void ProcessPendingLoadsForPriority(LoadPriority priority)
    {
        var pendingLoads = GetPendingLoadsDictionary(priority);
        foreach (var uri in pendingLoads.Keys.ToList())
        {
            if (pendingLoads[uri].TryPop(out var nextTcs))
            {
                // Process nextTcs...
                // Check and potentially remove empty stacks and URIs
                if (!pendingLoads[uri].Any())
                {
                    pendingLoads.Remove(uri);
                }
            }
        }
    }

    private async Task ExecuteLoadTask(QueueItem queueItem)
    {
        if (queueItem != null)
        {
            //do not limit local file loads
            bool useSemaphore = queueItem.Source is UriImageSource;

            try
            {
                if (useSemaphore)
                    await semaphoreLoad.WaitAsync();

                TraceLog($"ImageLoadManager: LoadImageOnPlatformAsync {queueItem.Source}");

                SKBitmap bitmap = await LoadImageOnPlatformAsync(queueItem.Source, queueItem.Cancel.Token);


                // Add the loaded bitmap to the context cache
                if (bitmap != null)
                {
                    if (queueItem.Source is UriImageSource sourceUri)
                    {
                        string uri = sourceUri.Uri.ToString();
                        // Add the loaded bitmap to the cache
                        _cachingProvider.Set(uri, bitmap, TimeSpan.FromSeconds(CacheLongevitySecs));
                        TraceLog($"ImageLoadManager: Loaded bitmap for UriImageSource {uri}");
                        // Remove the Task from the loadingBitmaps dictionary now that we're done loading this image
                        _trackLoadingBitmapsUris.TryRemove(uri, out _);
                    }
                    else
                    if (queueItem.Source is FileImageSource sourceFile)
                    {
                        string uri = sourceFile.File;

                        // Add the loaded bitmap to the cache
                        _cachingProvider.Set(uri, bitmap, TimeSpan.FromSeconds(CacheLongevitySecs));
                        TraceLog($"ImageLoadManager: Loaded bitmap for FileImageSource {uri}");
                        // Remove the Task from the loadingBitmaps dictionary now that we're done loading this image
                        _trackLoadingBitmapsUris.TryRemove(uri, out _);
                    }

                    if (ReuseBitmaps)
                    {
                        queueItem.Task.TrySetResult(bitmap);
                    }
                    else
                    {
                        queueItem.Task.TrySetResult(bitmap.Copy());
                    }

                }
                else
                {
                    TraceLog($"ImageLoadManager: BITMAP NULL for {queueItem.Source}");
                }


            }
            catch (Exception ex)
            {
                TraceLog($"ImageLoadManager: Exception {ex}");

                if (ex is OperationCanceledException || ex is System.Threading.Tasks.TaskCanceledException)
                {
                    queueItem.Task.TrySetCanceled();
                }
                else
                {
                    queueItem.Task.TrySetException(ex);
                }

                if (queueItem.Source is UriImageSource sourceUri)
                {
                    _trackLoadingBitmapsUris.TryRemove(sourceUri.Uri.ToString(), out _);
                }
                else
                if (queueItem.Source is FileImageSource sourceFile)
                {
                    _trackLoadingBitmapsUris.TryRemove(sourceFile.File, out _);
                }
            }
            finally
            {
                if (useSemaphore)
                    semaphoreLoad.Release();
            }
        }
    }


    public bool IsDisposed { get; protected set; }

    private QueueItem GetPendingItemLoadsForPriority(LoadPriority priority)
    {
        var pendingLoads = GetPendingLoadsDictionary(priority);
        foreach (var pendingPair in pendingLoads)
        {
            if (pendingPair.Value.Count != 0 && pendingPair.Value.TryPop(out var nextTcs))
            {
                TraceLog($"ImageLoadManager: [UNPAUSED] task for {pendingPair.Key}");

                return nextTcs;
            }
        }
        return null;
    }

    private async void ProcessQueue()
    {
        while (!IsDisposed)
        {
            try
            {
                if (IsLoadingLocked || semaphoreLoad.CurrentCount < 1)
                {
                    TraceLog($"ImageLoadManager: Loading Locked!");
                    await Task.Delay(50);
                    continue;
                }

                QueueItem queueItem = GetPendingItemLoadsForPriority(LoadPriority.High);
                if (queueItem == null)
                    queueItem = GetPendingItemLoadsForPriority(LoadPriority.Normal);
                if (queueItem == null)
                    queueItem = GetPendingItemLoadsForPriority(LoadPriority.Low);

                // If we didn't find a task in pendingLoads, try the main queue.
                lock (lockObject)
                {
                    if (queueItem == null && _queue.TryDequeue(out queueItem, out LoadPriority priority))
                    {
                        TraceLog($"[DEQUEUE]: {queueItem.Source} (queue {_queue.Count})");
                    }
                }

                if (queueItem != null)
                {
                    //the only really async that works okay 
                    Tasks.StartDelayedAsync(TimeSpan.FromMicroseconds(1), async () =>
                    {
                        await ExecuteLoadTask(queueItem);
                    });
                }
                else
                {
                    await Task.Delay(50);
                }
            }
            catch (Exception e)
            {
                Super.Log(e);
            }
            finally
            {

            }

        }


    }


    public void UpdateInCache(string uri, SKBitmap bitmap, int cacheLongevityMinutes)
    {
        _cachingProvider.Set(uri, bitmap, TimeSpan.FromMinutes(cacheLongevityMinutes));
    }

    /// <summary>
    /// Returns false if key already exists
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="bitmap"></param>
    /// <param name="cacheLongevityMinutes"></param>
    /// <returns></returns>
    public bool AddToCache(string uri, SKBitmap bitmap, int cacheLongevitySecs)
    {
        if (_cachingProvider.Exists(uri))
            return false;

        _cachingProvider.Set(uri, bitmap, TimeSpan.FromSeconds(cacheLongevitySecs));
        return true;
    }

    public SKBitmap GetFromCache(string url)
    {
        return _cachingProvider.Get<SKBitmap>(url)?.Value;
    }

    public async Task Preload(ImageSource source, CancellationTokenSource cts)
    {
        if (source.IsEmpty)
        {
            TraceLog($"Preload: Empty source");
            return;
        }
        string uri = null;
        if (source is UriImageSource sourceUri)
        {
            uri = sourceUri.Uri.ToString();
        }
        else
        if (source is FileImageSource sourceFile)
        {
            uri = sourceFile.File;
        }
        if (string.IsNullOrEmpty(uri))
        {
            TraceLog($"Preload: Invalid source {uri}");
            return;
        }

        var cacheKey = uri;

        // Check if the image is already cached or being loaded
        if (_cachingProvider.Get<SKBitmap>(cacheKey).HasValue || _trackLoadingBitmapsUris.ContainsKey(uri))
        {
            TraceLog($"Preload: Image already cached or being loaded for Uri {uri}");
            return;
        }

        var tcs = new TaskCompletionSource<SKBitmap>();
        var tuple = new QueueItem(source, cts, tcs);

        try
        {
            _queue.Enqueue(tuple, LoadPriority.Low);

            // Await the loading to ensure it's completed before returning
            await tcs.Task;
        }
        catch (Exception ex)
        {
            TraceLog($"Preload: Exception {ex}");
        }
    }

    private string GetUriFromImageSource(ImageSource source)
    {
        if (source is StreamImageSource)
            return Guid.NewGuid().ToString();
        else if (source is UriImageSource sourceUri)
            return sourceUri.Uri.ToString();
        else if (source is FileImageSource sourceFile)
            return sourceFile.File;

        return null;
    }



    public void Dispose()
    {
        IsDisposed = true;

        semaphoreLoad?.Dispose();

        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
    }

    public bool IsOffline { get; protected set; }

    private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        var connected = e.NetworkAccess;
        bool isOffline = connected != NetworkAccess.Internet
                        && connected != NetworkAccess.ConstrainedInternet;
        if (IsOffline && !isOffline)
        {
            CanReload?.Invoke(this, null);
        }
        IsOffline = isOffline;
    }

    public static async Task<SKBitmap> LoadFromFile(string filename, CancellationToken cancel)
    {

        try
        {
            cancel.ThrowIfCancellationRequested();

            SKBitmap bitmap = SkiaImageManager.Instance.GetFromCache(filename);
            if (bitmap != null)
            {
                TraceLog($"ImageLoadManager: Loaded local bitmap from cache {filename}");
                return bitmap;
            }

            TraceLog($"ImageLoadManager: Loading local {filename}");

            cancel.ThrowIfCancellationRequested();

            if (filename.SafeContainsInLower(SkiaImageManager.NativeFilePrefix))
            {
                var fullFilename = filename.Replace(SkiaImageManager.NativeFilePrefix, "", StringComparison.InvariantCultureIgnoreCase);
                using var stream = new FileStream(fullFilename, FileMode.Open);
                cancel.Register(stream.Close);  // Register cancellation to close the stream
                bitmap = SKBitmap.Decode(stream);
            }
            else
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(filename);  // Pass cancellation token
                using var reader = new StreamReader(stream);
                bitmap = SKBitmap.Decode(stream);
            }

            cancel.ThrowIfCancellationRequested();

            if (bitmap != null)
            {
                TraceLog($"ImageLoadManager: Loaded local bitmap {filename}");

                if (SkiaImageManager.Instance.AddToCache(filename, bitmap, SkiaImageManager.CacheLongevitySecs))
                {
                    return ReuseBitmaps ? bitmap : bitmap.Copy();
                }
            }
            else
            {
                TraceLog($"ImageLoadManager: FAILED to load local {filename}");
            }

            return bitmap;

        }
        catch (OperationCanceledException)
        {
            TraceLog("ImageLoadManager loading was canceled.");
            return null;
        }
        catch (Exception e)
        {
            Super.Log(e);
        }

        return null;

    }

}