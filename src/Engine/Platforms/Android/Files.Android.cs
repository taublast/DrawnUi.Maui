using Android.Content.Res;
using Android.Media;

namespace DrawnUi.Maui.Infrastructure
{
    public partial class Files
    {
        public static void RefreshSystem(FileDescriptor file)
        {
            var context = Platform.AppContext;

            MediaScannerConnection.ScanFile(context,
            new String[] { file.FullFilename },
            null, null);
        }

        //public static string GetPublicDirectory()
        //{

        //    return Android.OS.Environment
        //        .GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads)
        //        .AbsolutePath;
        //}

        /// <summary>
        /// tries to get all resources from assets folder Resources/Raw/{subfolder}
        /// </summary>
        /// <returns></returns>
        public static List<string> ListAssets(string subfolder)
        {
            AssetManager assets = Platform.AppContext.Assets;
            return assets.List(subfolder).ToList();
        }

        public static void Share(string message, IEnumerable<string> fullFilenames)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var files = fullFilenames.Select(x => new ShareFile(x)).ToList();
                    await Microsoft.Maui.ApplicationModel.DataTransfer.Share.Default.RequestAsync(new ShareMultipleFilesRequest
                    {
                        Title = message,
                        Files = files
                    });
                }
                catch (Exception e)
                {
                    Super.Log(e);
                }

            });
        }

    }
}
