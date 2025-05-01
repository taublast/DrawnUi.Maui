using Foundation;

namespace DrawnUi.Infrastructure
{
    public partial class Files
    {
        public static void RefreshSystem(FileDescriptor file)
        {

        }

        public static List<string> ListAssets(string subfolder)
        {
            NSBundle mainBundle = NSBundle.MainBundle;
            string resourcesPath = mainBundle.ResourcePath;
            string subfolderPath = Path.Combine(resourcesPath, subfolder);

            if (Directory.Exists(subfolderPath))
            {
                string[] files = Directory.GetFiles(subfolderPath);
                return files.Select(Path.GetFileName).ToList();
            }
            else
            {
                return new List<string>();
            }
        }

        public static string GetPublicDirectory()
        {
            return FileSystem.Current.AppDataDirectory;
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
