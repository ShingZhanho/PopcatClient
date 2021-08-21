using System;
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
            var checkResult = await UpdateTools.CheckUpdate(currentVersion,
                Options.IncludeBeta || ((VersionName) AssemblyData.InformationalVersion).PreRelease);
            switch (checkResult.Status)
            {
                case CheckUpdateStatus.UpToDate:
                    CommandLine.WriteSuccess("Your application is up-to-date.");
                    return;
                case CheckUpdateStatus.UpdateAvailable:
                    break;
                case CheckUpdateStatus.Failed:
                    CommandLine.WriteError($"Failed to check update. Reason: {checkResult.ExceptionMessage}");
                    CommandLine.WriteErrorVerbose($"Further message about the update failure:\n{checkResult.ExceptionStacktrace}");
                    return;
                default:
                    CommandLine.WriteWarning("The application cannot determine whether your application is up-to-date.");
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
        }
    }
}