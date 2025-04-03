using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Windows.Storage;

namespace DrawnUi.Draw
{
    public partial class Super
    {

        /// <summary>
        /// When set to true will run loop uponCompositionTarget.Rendering hits instead of a timer looper that targets 120 fps. Default is false because for Windows double buffering we need twice the standart calls..
        /// </summary>
        public static bool UseDisplaySync = false;

        static bool _loopStarting = false;
        static bool _loopStarted = false;

        public static void Init()
        {
            if (Initialized)
                return;

            Initialized = true;

            if (Super.NavBarHeight < 0)

                Super.NavBarHeight = 50; //manual

            Super.StatusBarHeight = 0;

            //VisualDiagnostics.VisualTreeChanged += OnVisualTreeChanged;
            InitShared();

            if (UseDisplaySync)
            {
                Tasks.StartDelayed(TimeSpan.FromMilliseconds(250), async () =>
                {
                    while (!_loopStarted)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            if (_loopStarting)
                                return;
                            _loopStarting = true;

                            if (MainThread.IsMainThread) //UI thread is available
                            {
                                if (!_loopStarted)
                                {
                                    _loopStarted = true;
                                    try
                                    {
                                        CompositionTarget.Rendering += (s, a) =>
                                        {
                                            OnFrame?.Invoke(null, null);
                                        };
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                }
                            }

                            _loopStarting = false;
                        });
                        await Task.Delay(100);
                    }
                });
            }
            else
            {
                Looper = new(() =>
                {
                    OnFrame?.Invoke(null, null);
                });

                Looper.StartOnMainThread(120);
            }
        }


        [DllImport("user32.dll")]
        public static extern bool SetFocus(IntPtr hWnd);

        static Looper Looper { get; set; }

        public static event EventHandler OnFrame;

        /// <summary>
        /// Opens web link in native browser
        /// </summary>
        /// <param name="link"></param>
        public static void OpenLink(string link)
        {
            try
            {
                Windows.System.Launcher.LaunchUriAsync(new Uri(link));
            }
            catch (Exception e)
            {
                Super.Log(e);
            }
        }

        /// <summary>
        /// Lists assets inside the Resources/Raw subfolder
        /// </summary>
        /// <param name="subfolder"></param>
        /// <returns></returns>
        public static List<string> ListAssets(string subfolder)
        {
            StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder sub = installFolder.GetFolderAsync(subfolder).GetAwaiter().GetResult();
            IReadOnlyList<StorageFile> files = sub.GetFilesAsync().GetAwaiter().GetResult();

            return files.Select(f => f.Name).ToList();
        }

        public static async Task<byte[]> CaptureScreenshotAsync()
        {
            var screen = await Screenshot.CaptureAsync();
            using var input = await screen.OpenReadAsync();

            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                {
                    var data = ms.ToArray();
                    return data;
                }
            }
        }

    }


}

