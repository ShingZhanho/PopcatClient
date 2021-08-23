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
            // load fallback language
            var langPackPath = Path.GetFullPath($"./Langs/{Options.FallbackLanguageId}/lang-pack.json");
            try
            {
                LanguageManager.FallbackLanguage = new LanguageFile(langPackPath);
            }
            catch (Exception e)
            {
                // the following messages only has English version
                // since the packs are not loaded
                CommandLine.WriteError($"Could not load language pack {langPackPath}. Reason: {e.Message}");
                CommandLine.WriteErrorVerbose($"Further information: {e.StackTrace}");
            }
            
            // load language packs
            try
            {
                langPackPath = $"./Langs/{Options.LanguageId}/lang-pack.json";
                LanguageManager.Language = new LanguageFile(Path.GetFullPath(langPackPath));
            }
            catch (Exception e)
            {
                LanguageManager.Language = LanguageManager.FallbackLanguage;
                CommandLine.WriteWarning($"Failed to load language pack {langPackPath}. Using fallback language pack.");
                CommandLine.WriteError($"Fail reason: {e.Message}");
                CommandLine.WriteErrorVerbose($"Further information: {e.StackTrace}");
            }

            // display lang pack info
            CommandLine.WriteMessage(Strings.PackInfo.LanguagePack() +
                                     $" ({LanguageManager.Language.LanguageInfo.DisplayName}):" +
                                     Strings.PackInfo.PackInfo_Authors(LanguageManager.Language.Authors));
            if (LanguageManager.Language.Filename != LanguageManager.FallbackLanguage.Filename)
                CommandLine.WriteMessage(Strings.PackInfo.FallbackLanguagePack() +
                                         $" ({LanguageManager.FallbackLanguage.LanguageInfo.DisplayName}):" +
                                         Strings.PackInfo.PackInfo_Authors(LanguageManager.FallbackLanguage.Authors));
            
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

            
            CommandLine.WriteError(Strings.SoftwareUpdates.ErrMsg_InstallerExitCode(installerProcess.ExitCode));
            switch (installerProcess.ExitCode)
            {
                case 1:
                    // PID invalid
                    CommandLine.WriteErrorVerbose(Strings.SoftwareUpdates.Verbose_ErrMsg_InstallerExitCode_1());
                    break;
                case 2:
                    // new version not exist
                    CommandLine.WriteErrorVerbose(Strings.SoftwareUpdates.Verbose_ErrMsg_InstallerExitCode_2());
                    break;
                case 3:
                    // current working directory does not exist
                    CommandLine.WriteErrorVerbose(Strings.SoftwareUpdates.Verbose_ErrMsg_InstallerExitCode_3());
                    break;
                default:
                    CommandLine.WriteErrorVerbose(Strings.SoftwareUpdates.Verbose_ErrMsg_InstallerExitCode_Others());
                    break;
            }
            CommandLine.WriteErrorVerbose(await installerProcess.StandardOutput.ReadToEndAsync());
            CommandLine.WriteErrorVerbose(await installerProcess.StandardError.ReadToEndAsync());
        }

        private static void ShowStartOptionsVerbose()
        {
            CommandLine.WriteMessageVerbose(Strings.Program.Msg_ClientStartOptions());
            
            CommandLine.WriteMessageVerbose(Strings.Program.Format_StartupOptionsEntries(),
                Strings.Program.StartupOptions_WaitTime(), $"{Options.WaitTime}ms");
            
            CommandLine.WriteMessageVerbose(Strings.Program.Format_StartupOptionsEntries(),
                Strings.Program.StartupOptions_MaxFailures(), Options.MaxFailures);
            
            CommandLine.WriteMessageVerbose(Strings.Program.Format_StartupOptionsEntries(),
                Strings.Program.StartupOptions_Leaderboard(),
                Options.DisableLeaderboard ? Strings.Common.Disabled() : Strings.Common.Enabled());
            
            CommandLine.WriteMessageVerbose(Strings.Program.Format_StartupOptionsEntries(),
                Strings.Program.StartupOptions_CheckUpdate(),
                Options.DisableUpdate ? Strings.Common.No() : Strings.Common.Yes());
            
            CommandLine.WriteMessageVerbose(Strings.Program.Format_StartupOptionsEntries(),
                Strings.Program.StartupOptions_InstallBeta(),
                Options.IncludeBeta || ((VersionName)AssemblyData.InformationalVersion).PreRelease
                ? Strings.Common.Yes() : Strings.Common.No());
            
            CommandLine.WriteMessageVerbose(Strings.Program.Format_StartupOptionsEntries(),
                Strings.Program.StartupOptions_ClearTempDir(), Options.ClearTempDir ? Strings.Common.Yes() : Strings.Common.No());
            
            CommandLine.WriteMessageVerbose(Strings.Program.Format_StartupOptionsEntries(),
                Strings.Program.StartupOptions_LanguageId(), Options.LanguageId);
            
            CommandLine.WriteMessageVerbose(Strings.Program.Format_StartupOptionsEntries(),
                Strings.Program.StartupOptions_FallbackLanguageId(), Options.FallbackLanguageId);
        }
    }
}