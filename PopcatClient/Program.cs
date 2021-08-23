using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PopcatClient.Languages;
using PopcatClient.Updater;

namespace PopcatClient
{
    public static class Program
    {
        public static CommandLineOptions Options { get; private set; } = new();

        public static void Main(string[] args)
        {
            // load language packs
            LanguageManager.FallbackLanguage = new LanguageFile(
                Path.GetFullPath($"./Langs/{Options.FallbackLanguageId}/lang-pack.json"));
            
            var langPackPath = Path.GetFullPath($"./Langs/{Options.LanguageId}/lang-pack.json");
            LanguageManager.Language = File.Exists(langPackPath)
                ? new LanguageFile(langPackPath)
                : LanguageManager.FallbackLanguage;
            
            Console.Title = $"Popcat Client {AssemblyData.InformationalVersion}";

            Options = new CommandLineOptions(args);
            
            CommandLine.WriteWarningVerbose(Strings.Program.WarningMsg_VerboseMode());
            CommandLine.WriteWarningDebug(Strings.Program.WarningMsg_DebugMode());
            
            ShowStartOptionsVerbose();
            
            // software update
            if (Options.DisableUpdate)
                // Disabled
                CommandLine.WriteWarning(Strings.SoftwareUpdates.WarningMsg_UpdateDisabled());
            else
            {
                CommandLine.WriteMessage(Strings.SoftwareUpdates.Msg_CheckingUpdates());
                SoftwareUpdate(AssemblyData.InformationalVersion);
            }
            
            // clear temporary folder
            if (Options.ClearTempDir)
            {
                CommandLine.WriteMessage(Strings.Program.Msg_ClearTempDir());
                Directory.Delete(Path.Combine(Path.GetTempPath(), "PopcatClient_Update"), true);
                CommandLine.WriteSuccess(Strings.Program.SuccessMsg_TempDirCleared());
            }

            var leaderboardClient = new LeaderboardClient(Options);

            var popClient = new PopcatClient(Options, leaderboardClient);
            if (!Options.Debug) popClient.Run();

            leaderboardClient.Dispose();
            popClient.Dispose();
            Console.Read();
        }

        private static async void SoftwareUpdate(VersionName currentVersion)
        {
            // check for updates
            var checkResult = await UpdateTools.CheckUpdateAsync(currentVersion,
                Options.IncludeBeta || ((VersionName) AssemblyData.InformationalVersion).PreRelease);
            switch (checkResult.ResultStatus)
            {
                case CheckUpdateResultStatus.UpToDate:
                    CommandLine.WriteSuccess(Strings.SoftwareUpdates.SucMsg_ApplicationUpToDate());
                    return;
                case CheckUpdateResultStatus.UpdateAvailable:
                    CommandLine.WriteWarning(Strings.SoftwareUpdates.WarningMsg_NewVersionAvailable(checkResult.ServerLatestVersion));
                    break;
                case CheckUpdateResultStatus.Failed:
                    CommandLine.WriteError(Strings.SoftwareUpdates.ErrMsg_UpdateCheckFailed(checkResult.ExceptionMessage));
                    CommandLine.WriteErrorVerbose(Strings.SoftwareUpdates.Verbose_ErrMsg_UpdateCheckFailed(checkResult.ExceptionStackTrace));
                    return;
                default:
                    CommandLine.WriteWarning(Strings.SoftwareUpdates.WarningMsg_CannotDetermineIfUpToDate());
                    return;
            }

            // download from server
            CommandLine.WriteMessage(Strings.SoftwareUpdates.Msg_DownloadingAsset());
            CommandLine.WriteMessageVerbose($"GET {checkResult.AssetDownloadUrl}");
            var tempDir = Path.Combine(Path.GetTempPath(), "PopcatClient_Update");
            Directory.CreateDirectory(tempDir);
            
            var downloadResult = await UpdateTools.DownloadUpdateAssetAsync(checkResult.AssetDownloadUrl,
                Path.Combine(tempDir, $"PopcatClient_{AssemblyData.InformationalVersion}.zip"));
            switch (downloadResult.Status)
            {
                case BasicResultStatus.Success:
                    CommandLine.WriteSuccess(Strings.SoftwareUpdates.SucMsg_AssetDownloaded(downloadResult.FilePath));
                    break;
                case BasicResultStatus.Failed:
                    CommandLine.WriteError(Strings.SoftwareUpdates.ErrMsg_DownloadAssetFailed(downloadResult.ExceptionMessage));
                    CommandLine.WriteErrorVerbose(Strings.SoftwareUpdates.Verbose_ErrMsg_DownloadAssetFailed(downloadResult.ExceptionStackTrace));
                    return;
                default:
                    CommandLine.WriteWarning(Strings.SoftwareUpdates.WarningMsg_CannotDetermineIfDownloaded());
                    return;
            }
            
            // extract downloaded file
            CommandLine.WriteMessage(Strings.SoftwareUpdates.Msg_PreparingAsset());
            var prepareResult = await UpdateTools.PrepareUpdateAssetAsync(downloadResult.FilePath);
            switch (prepareResult.Status)
            {
                case BasicResultStatus.Success:
                    CommandLine.WriteSuccess(Strings.SoftwareUpdates.SucMsg_AssetIsReady());
                    break;
                case BasicResultStatus.Failed:
                    CommandLine.WriteError(Strings.SoftwareUpdates.ErrMsg_AssetPrepareFailed(prepareResult.ExceptionMessage));
                    CommandLine.WriteErrorVerbose(Strings.SoftwareUpdates.Verbose_ErrMsg_AssetPrepareFailed(prepareResult.ExceptionStackTrace));
                    return;
                default:
                    CommandLine.WriteWarning(Strings.SoftwareUpdates.WarningMsg_CannotDetermineIfReady());
                    return;
            }
            
            // run installer
            CommandLine.WriteMessage(Strings.SoftwareUpdates.Msg_InstallingUpdate());
            CommandLine.WriteWarning(Strings.SoftwareUpdates.WarningMsg_AppWillRestartAfterUpdate());
            var installerArgs = $"\"{prepareResult.ExtractedDir}\" " +
                                $"\"{Environment.CurrentDirectory}\" " +
                                $"{Environment.ProcessId.ToString()} " +
                                Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                    string.Join(" ", Environment.CommandLine.Split(' ').Skip(1))));
            var installerProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = prepareResult.InstallerExecutable,
                    Arguments = installerArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            
            installerProcess.Start();
            await installerProcess.WaitForExitAsync();
            
            // app should be killed and restarted if installation is successful
            // if the app is not killed and the following lines are run, the installer must failed at some point

            
            CommandLine.WriteError($"One or more errors occured while installing new version. Exit code: {installerProcess.ExitCode}");
            switch (installerProcess.ExitCode)
            {
                case 1:
                    // PID invalid
                    CommandLine.WriteErrorVerbose("The PID argument was invalid.");
                    break;
                case 2:
                    // new version not exist
                    CommandLine.WriteErrorVerbose("The new version directory does not exist.");
                    break;
                case 3:
                    // current working directory does not exist
                    CommandLine.WriteErrorVerbose("The current working directory does not exist.");
                    break;
                default:
                    CommandLine.WriteErrorVerbose($"Unknown error. Exit code: {installerProcess.ExitCode}");
                    break;
            }
            CommandLine.WriteErrorVerbose(await installerProcess.StandardOutput.ReadToEndAsync());
            CommandLine.WriteErrorVerbose(await installerProcess.StandardError.ReadToEndAsync());
        }

        private static void ShowStartOptionsVerbose()
        {
            CommandLine.WriteMessageVerbose("PopcatClient started with following settings:");
            CommandLine.WriteMessageVerbose($"Waiting time: {Options.WaitTime}ms");
            CommandLine.WriteMessageVerbose($"Max sequential failures: {Options.MaxFailures}");
            CommandLine.WriteMessageVerbose($"Initial pops: {Options.InitialPops}");
            CommandLine.WriteMessageVerbose("Leaderboard: " + (Options.DisableLeaderboard ? "Disabled" : "Enabled"));
            CommandLine.WriteMessageVerbose("Software update: " + (Options.DisableUpdate ? "Disabled" : "Enabled"));
            CommandLine.WriteMessageVerbose("Install beta versions: " + 
                                            (Options.IncludeBeta || ((VersionName)AssemblyData.InformationalVersion).PreRelease 
                                                ? "Install" : "Do not install"));
            CommandLine.WriteMessageVerbose("Clear temporary directory: " + (Options.ClearTempDir ? "Yes" : "No"));
            CommandLine.WriteMessageVerbose(
                $"Display language pack ID: {Options.LanguageId} " +
                $"({CultureInfo.GetCultureInfo(Options.LanguageId).DisplayName})");
            CommandLine.WriteMessageVerbose(
                $"Fallback display language pack ID: {Options.FallbackLanguageId} " +
                $"({CultureInfo.GetCultureInfo(Options.FallbackLanguageId).DisplayName})");
        }
    }
}