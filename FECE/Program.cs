namespace FECE;

public static class Program
{
    public const string ApplicationName = "FECE";
    private static readonly string DestFolder = Utils.GetDownloadFolderPath();

    private const string EosClientCampusId = "1dQaZYcJaBVzsRo1uOqPPXTfI7qO-0Zox";
    private const string EosClientAtHomeId = "1x1fyGOrcP5Fx58foNo-CcWtnPf-uzuHM";
    private const string PeaClientId = "1rx0t_fSFqc1IJ4NSIQqjVjWmd3t5r1sC";
    private const string SebForWindowsId = "15Xpcj1rtXmgUtG0psqTWQkAtf7QUV3Rg";

    public static void Main()
    {
        Console.Title = ApplicationName;
        var utils = new Utils();

        try
        {
            if (!Directory.Exists(DestFolder)) Directory.CreateDirectory(DestFolder);
            var exit = false;

            while (!exit)
            {
                AnsiConsole.Write(new Markup("[blue]FPT Exam Clients Enhanced[/]"));
                Console.WriteLine();
                Console.WriteLine();

                AnsiConsole.Write(
                    new Markup("[white]Select an option:[/]"));
                Console.WriteLine();
                AnsiConsole.Write(
                    new Markup("([blue]<Arrow>[/] key to select, [blue]<Enter>[/] to confirm)"));
                Console.WriteLine();
                Console.WriteLine();

                var isTokenExist = Directory.Exists("./token.json");
                var isEosOnCampusExist = Directory.Exists(Path.Join(Utils.DestFolder, "EOSClient_OnCampus"));
                var isEosAtHomeExist = Directory.Exists(Path.Join(Utils.DestFolder, "EOSClient_AtHome"));
                var isPeaExist = Directory.Exists(Path.Join(Utils.DestFolder, "PEA_Client_SU21"));
                var isSebExist = Directory.Exists(Path.Join(Utils.DestFolder, "SEBForWindows"));

                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(
                            isTokenExist ? "Log Out" : "Login",
                            isEosOnCampusExist ? "Open EOS Client (On Campus)" : "Download EOS Client (On Campus)",
                            isEosAtHomeExist ? "Open EOS Client (At Home)" : "Download EOS Client (At Home)",
                            isPeaExist ? "Open PEA" : "Download PEA",
                            isSebExist ? "Open SEB for Windows" : "Download SEB for Windows",
                            "Clear All Downloaded File",
                            "Exit"));

                switch (option)
                {
                    case "Login":
                        utils.Login();
                        break;
                    case "Log Out":
                        utils.Logout();
                        break;
                    case "Download EOS Client (On Campus)":
                        utils.Download(Path.Join(DestFolder, "EOSClient_OnCampus.zip"),
                            EosClientCampusId);
                        break;
                    case "Open EOS Client (On Campus)":
                        Utils.Open("EOSClient_OnCampus", "EOSClient.exe");
                        break;
                    case "Download EOS Client (At Home)":
                        utils.Download(Path.Join(DestFolder, "EOSClient_AtHome.zip"),
                            EosClientAtHomeId);
                        break;
                    case "Open EOS Client (At Home)":
                        Utils.Open("EOSClient_AtHome", "EOSClient.exe");
                        break;
                    case "Download PEA":
                        utils.Download(Path.Join(DestFolder, "PEA.zip"), PeaClientId);
                        break;
                    case "Open PEA":
                        Utils.Open("PEA_Client_SU21", "PEALogin.exe");
                        break;
                    case "Download SEB for Windows":
                        utils.Download(Path.Join(DestFolder, "SEBForWindows.zip"),
                            SebForWindowsId);
                        break;
                    case "Open SEB for Windows":
                        Utils.Open("SEBForWindows", "1.SafeExamBrowserInstaller2.3.exe");
                        break;
                    case "Clear All Downloaded File":
                        Utils.ClearAll();
                        break;
                    default:
                        exit = true;
                        break;
                }

                AnsiConsole.Clear();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}