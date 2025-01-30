using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace DrawnUi.Maui.Draw
{
    public partial class Super
    {



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

            Looper = new(() =>
            {
                OnFrame?.Invoke(null, null);
            });

            Looper.StartOnMainThread(120);
        }

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
    }


}

