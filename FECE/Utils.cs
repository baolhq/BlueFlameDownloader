namespace FECE;

public class Utils
{
    private readonly string[] _scopes = { DriveService.Scope.DriveReadonly };
    public static readonly string DestFolder = Utils.GetDownloadFolderPath();
    private UserCredential? Credential { get; set; }

    /// <summary>
    /// Generate user credential by reading credentials.json if token.json does not exist
    /// </summary>
    /// <returns>The credential need to use google services</returns>
    private UserCredential GenerateCredential()
    {
        using var stream =
            new FileStream("credentials.json", FileMode.Open, FileAccess.Read);

        const string credPath = "token.json";

        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            _scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true)).Result;

        return credential;
    }

    /// <summary>
    /// Unzip file to destination folder
    /// </summary>
    /// <param name="src">The file source to extract</param>
    /// <param name="dest">Folder destination to extract</param>
    private static void Unzip(string src, string dest)
    {
        if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);

        using var archive = ZipArchive.Open(src);

        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
        {
            entry.WriteToDirectory(dest, new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = true
            });
        }

        archive.Dispose();
        File.Delete(src);
        Console.WriteLine($"Extracted at {dest}");
    }

    public static string GetDownloadFolderPath() =>
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FECE");

    private static void DeleteDirectory(string targetDir)
    {
        var files = Directory.GetFiles(targetDir);
        var dirs = Directory.GetDirectories(targetDir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (var dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(targetDir, false);
    }

    public void Login()
    {
        AnsiConsole.Write(new Markup("Please login with your [blue]FPT Account[/] in the browser.."));
        Credential = GenerateCredential();
    }

    public void Logout()
    {
        if (Directory.Exists("./token.json")) DeleteDirectory("./token.json");
        Credential = null;
    }

    public static void ClearAll()
    {
        try
        {
            var eosClientOnCampus = Path.Join(DestFolder, "EOSClient_OnCampus");
            if (Directory.Exists(eosClientOnCampus)) Utils.DeleteDirectory(eosClientOnCampus);

            var eosClientAtHome = Path.Join(DestFolder, "EOSClient_AtHome");
            if (Directory.Exists(eosClientAtHome)) Utils.DeleteDirectory(eosClientAtHome);

            var peaClient = Path.Join(DestFolder, "PEA_Client_SU21");
            if (Directory.Exists(peaClient)) Utils.DeleteDirectory(peaClient);

            var sebForWindows = Path.Join(DestFolder, "SEBForWindows");
            if (Directory.Exists(sebForWindows)) Utils.DeleteDirectory(sebForWindows);
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void Download(string downloadedFileName, string fileId)
    {
        // If user is not logged in, go back to main menu
        if (Credential == null)
        {
            AnsiConsole.Write(new Markup("You must [blue]login with an FPT account[/] to use this feature.\n"));
            AnsiConsole.Write(new Markup("Press [blue]<Enter>[/] to continue."));
            Console.ReadLine();
            return;
        }

        AnsiConsole.Write(new Markup("LOG: [blue]Downloading..[/]     "));
        Downloader.Download(Credential, fileId, downloadedFileName);

        AnsiConsole.Write(new Markup("LOG: [blue]Extracting..[/]      "));
        Unzip(downloadedFileName, DestFolder);
    }

    public static void Open(string extractedFolderName, string executableFileName)
    {
        var filePath = Path.Join(DestFolder, extractedFolderName, executableFileName);
        var proc = new ProcessStartInfo
        {
            Arguments = "/select, \"" + filePath + "\"",
            FileName = "explorer.exe",
            UseShellExecute = true
        };
        Process.Start(proc);
    }
}