using System.Linq;

namespace PopcatClient
{
    public class CommandLineOptions
    {
        private CommandLineOptions(){ }
        
        /// <summary>
        /// Extract command line arguments.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public CommandLineOptions(string[] args)
        {
            args ??= System.Array.Empty<string>();
            Verbose = args.Contains("--verbose");
            Debug = args.Contains("--debug");
            if (args.Contains("--wait-time"))
            {
                if (args.ToList().IndexOf("--wait-time") + 1 > args.Length - 1)
                    CommandLine.WriteWarning("No parameter specified for --wait-time. Using default value (30000).");
                else
                {
                    if (int.TryParse(args[args.ToList().IndexOf("--wait-time") + 1], out var result) && result > 30000)
                        WaitTime = result;
                    else
                        CommandLine.WriteWarning("Invalid parameter specified for --wait-time. Using default value (30000).");
                }
            }
            if (args.Contains("--max-failures"))
            {
                var indexOfOption = args.ToList().IndexOf("--max-failures");
                if (indexOfOption + 1 > args.Length - 1)
                    CommandLine.WriteWarning("No parameter specified for --max-failures. Using default value 3.");
                else
                {
                    if (int.TryParse(args[indexOfOption + 1], out var result) && result > 0)
                        MaxFailures = result;
                    else
                        CommandLine.WriteWarning("Invalid parameter specified for --wait-time. Using default value 3.");
                }
            }

            if (args.Contains("--init-pops"))
            {
                var indexOfOption = args.ToList().IndexOf("--init-pops");
                if (indexOfOption + 1 > args.Length - 1)
                    CommandLine.WriteWarning("No parameter specified for --init-pops. Using default value 1.");
                else
                {
                    if (int.TryParse(args[indexOfOption + 1], out var result) && result is <= 800 and >= 0)
                        InitialPops = result;
                    else 
                        CommandLine.WriteWarning("Invalid parameter specified for --init-pops. Using default value 1.");
                }
            }
            DisableLeaderboard = args.Contains("--disable-leaderboard");
            DisableUpdate = args.Contains("--disable-updates");
        }
        
        /// <summary>
        /// Indicates whether verbose mode is enabled.
        /// </summary>
        public bool Verbose { get; private init; }
        /// <summary>
        /// Indicates whether debug mode is enabled.
        /// </summary>
        public bool Debug { get; private init;  }

        /// <summary>
        /// Indicates the time should the program wait between each pop in ms.
        /// </summary>
        public int WaitTime { get; private init; } = 30 * 1000;
        /// <summary>
        /// Indicates how many times of failures in a row should the application exit.
        /// </summary>
        public int MaxFailures { get; private init; } = 3;
        /// <summary>
        /// Indicates how many pops should the application send to the server for the first time.
        /// </summary>
        public int InitialPops { get; private init; } = 1;
        /// <summary>
        /// Indicates whether disable the leaderboard
        /// </summary>
        public bool DisableLeaderboard { get; private init; }
        /// <summary>
        /// Indicates whether disable checking for updates on launching.
        /// </summary>
        public bool DisableUpdate { get; private init; }

        public static readonly CommandLineOptions DefaultCommandLineOptions = new()
        {
            Verbose = false,
            Debug = false,
            WaitTime = 30 * 1000,
            MaxFailures = 3,
            InitialPops = 1,
            DisableLeaderboard = false,
            DisableUpdate = false
        };
    }
}