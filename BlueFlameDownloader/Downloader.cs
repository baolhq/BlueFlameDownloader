namespace BlueFlameDownloader;

public static class Downloader
{
    /// <summary>
    /// Download file from Google Drive with FileID and save to destination
    /// </summary>
    /// <param name="credential">User credential to download file</param>
    /// <param name="fileId">ID of the file on Drive</param>
    /// <param name="dest">Which folder to save this file</param>
    public static void Download(UserCredential credential, string fileId, string dest)
    {
        using var service = new DriveService(
            new BaseClientService.Initializer
                { HttpClientInitializer = credential, ApplicationName = Program.ApplicationName });

        var request = service.Files.Get(fileId);
        var memoryStream = new MemoryStream();

        request.MediaDownloader.ProgressChanged +=
            progress =>
            {
                using var fileStream = File.OpenWrite(dest);

                switch (progress.Status)
                {
                    case DownloadStatus.NotStarted:
                        break;
                    case DownloadStatus.Downloading:
                        break;
                    case DownloadStatus.Completed:
                        Console.WriteLine($"Download complete at {dest}.");
                        memoryStream.WriteTo(fileStream);
                        break;
                    case DownloadStatus.Failed:
                        Console.WriteLine("Download failed.");
                        break;
                    default:
                        Console.WriteLine("An unexpected error occured, please contact us.");
                        break;
                }
            };
        request.Download(memoryStream);
    }
}