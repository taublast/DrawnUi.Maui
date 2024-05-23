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
