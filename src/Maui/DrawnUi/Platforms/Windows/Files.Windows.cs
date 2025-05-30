﻿using Windows.Storage;

namespace DrawnUi.Infrastructure
{
    public partial class Files
    {
        public static void RefreshSystem(FileDescriptor file)
        {

        }

        public static string GetPublicDirectory()
        {
            return Windows.Storage.KnownFolders.DocumentsLibrary.Path;
        }

        public static List<string> ListAssets(string sub)
        {
            StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder subfolder = installFolder.GetFolderAsync(sub).GetAwaiter().GetResult();
            IReadOnlyList<StorageFile> files = subfolder.GetFilesAsync().GetAwaiter().GetResult();
            return files.Select(f => f.Name).ToList();
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
