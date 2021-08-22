using System;
using System.IO;
using PopcatClient.Updater;

namespace PopcatClient
{
    public static class Program
    {
        public static CommandLineOptions Options { get; private set; }= CommandLineOptions.DefaultCommandLineOptions;

        public static void Main(string[] args)
        {
            Console.Title = $"Popcat Client {AssemblyData.InformationalVersion}";

            Options = new CommandLineOptions(args);
            
            CommandLine.WriteWarningVerbose("Verbose mode is enabled.");
            CommandLine.WriteWarningDebug("Debug mode is enabled.");
            
            ShowStartOptionsVerbose(Options);
            
            // software update
            if (Options.DisableUpdate)
                // Disabled
                CommandLine.WriteWarning("You have disabled software update. " +
                                         "Application might not work properly if you are using an outdated version.");
            else
            {
                CommandLine.WriteMessage("Checking for updates on server...");
                SoftwareUpdate(AssemblyData.InformationalVersion);
            }

            var leaderboardClient = new LeaderboardClient(Options);

            var popClient = new PopcatClient(Options, leaderboardClient);
            if (!Options.Debug) popClient.Run();

            Console.Read();
            leaderboardClient.Dispose();
            popClient.Dispose();
        }

        private static async void SoftwareUpdate(VersionName currentVersion)
        {
            // check for updates
            var checkResult = await UpdateTools.CheckUpdate(currentVersion,
                Options.IncludeBeta || ((VersionName) AssemblyData.InformationalVersion).PreRelease);
            switch (checkResult.ResultStatus)
            {
                case CheckUpdateResultStatus.UpToDate:
                    CommandLine.WriteSuccess("Your application is up-to-date.");
                    return;
                case CheckUpdateResultStatus.UpdateAvailable:
                    CommandLine.WriteWarning($"A new version ({checkResult.ServerLatestVersion.ToString()}) is available.");
                    break;
                case CheckUpdateResultStatus.Failed:
                    CommandLine.WriteError($"Failed to check update. Reason: {checkResult.ExceptionMessage}");
                    CommandLine.WriteErrorVerbose($"Further message about the update failure:\n{checkResult.ExceptionStackTrace}");
                    return;
                default:
                    CommandLine.WriteWarning("The application cannot determine whether your application is up-to-date.");
                    return;
            }

            // download from server
            CommandLine.WriteMessage("Downloading update asset from server...");
            CommandLine.WriteMessageVerbose($"GET {checkResult.AssetDownloadUrl}");
            var tempDir = Path.Combine(Path.GetTempPath(), "PopcatClient_Update");
            Directory.CreateDirectory(tempDir);
            
            var downloadResult = await UpdateTools.DownloadUpdateAsset(checkResult.AssetDownloadUrl,
                Path.Combine(tempDir, $"PopcatClient_{AssemblyData.InformationalVersion}.zip"));
            switch (downloadResult.Status)
            {
                case BasicResultStatus.Success:
                    CommandLine.WriteSuccess($"Asset downloaded to {downloadResult.FilePath}");
                    break;
                case BasicResultStatus.Failed:
                    CommandLine.WriteError($"Failed to download update asset. Reason: {downloadResult.ExceptionMessage}");
                    CommandLine.WriteErrorVerbose($"More information:\n{downloadResult.ExceptionStackTrace}");
                    return;
                default:
                    CommandLine.WriteWarning("The application cannot determine whether the asset is downloaded.");
                    return;
            }
            
            // extract downloaded file
            CommandLine.WriteMessage("Preparing update asset for installing.");
            var prepareResult = await UpdateTools.PrepareUpdateAsset(downloadResult.FilePath);
            switch (prepareResult.Status)
            {
                case BasicResultStatus.Success:
                    CommandLine.WriteSuccess("Asset is ready for install.");
                    break;
                case BasicResultStatus.Failed:
                    CommandLine.WriteError($"Failed to prepare update asset. Reason: {prepareResult.ExceptionMessage}");
                    CommandLine.WriteErrorVerbose($"More information:\n{prepareResult.ExceptionStackTrace}");
                    return;
                default:
                    CommandLine.WriteWarning("The application cannot determine whether the asset is prepared.");
                    return;
            }
        }

        private static void ShowStartOptionsVerbose(CommandLineOptions options)
        {
            CommandLine.WriteMessageVerbose("PopcatClient started with following settings:");
            CommandLine.WriteMessageVerbose($"Waiting time: {options.WaitTime}ms");
            CommandLine.WriteMessageVerbose($"Max sequential failures: {options.MaxFailures}");
            CommandLine.WriteMessageVerbose($"Initial pops: {Options.InitialPops}");
            CommandLine.WriteMessageVerbose("Leaderboard: " + (Options.DisableLeaderboard ? "Disabled" : "Enabled"));
            CommandLine.WriteMessageVerbose("Software update: " + (Options.DisableUpdate ? "Disabled" : "Enabled"));
            CommandLine.WriteMessageVerbose("Install beta versions: " + 
                                            (Options.IncludeBeta || ((VersionName)AssemblyData.InformationalVersion).PreRelease 
                                                ? "Install" : "Do not install"));
        }
    }
}