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
            // parsing options
            //
            // --clear-temp
            //
            ClearTempDir = args.Contains("--clear-temp");
            //
            // --debug (shortname: -d)
            //
            Debug = args.Contains("--debug") || args.Contains("-d");
            //
            // --disable-leaderboard (shortname: -l)
            //
            DisableLeaderboard = args.Contains("--disable-leaderboard") || args.Contains("-l");
            //
            // --disable-updates (shortname: -u)
            //
            DisableUpdate = args.Contains("--disable-updates") || args.Contains("-u");
            //
            // --include-beta (shortname: -b)
            //
            IncludeBeta = args.Contains("--include-beta") || args.Contains("-b");
            //
            // --init-pops
            //
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
            //
            // --max-failures
            //
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
            //
            // --verbose (shortname: -v)
            //
            Verbose = args.Contains("--verbose") || args.Contains("-v");
            //
            // --wait-time
            //
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
        }
        
        /// <summary>
        /// Indicates whether clear the temp folder.
        /// </summary>
        public bool ClearTempDir { get; private init; }
        /// <summary>
        /// Indicates whether debug mode is enabled.
        /// </summary>
        public bool Debug { get; private init;  }
        /// <summary>
        /// Indicates whether disable the leaderboard
        /// </summary>
        public bool DisableLeaderboard { get; private init; }
        /// <summary>
        /// Indicates whether disable checking for updates on launching.
        /// </summary>
        public bool DisableUpdate { get; private init; }
        /// <summary>
        /// Indicates if to include beta versions when updating. This option will always be ignored if the app is already a beta.
        /// </summary>
        public bool IncludeBeta { get; private init; }
        /// <summary>
        /// Indicates how many pops should the application send to the server for the first time.
        /// </summary>
        public int InitialPops { get; private init; } = 1;
        /// <summary>
        /// Indicates how many times of failures in a row should the application exit.
        /// </summary>
        public int MaxFailures { get; private init; } = 3;
        /// <summary>
        /// Indicates whether verbose mode is enabled.
        /// </summary>
        public bool Verbose { get; private init; }
        /// <summary>
        /// Indicates the time should the program wait between each pop in ms.
        /// </summary>
        public int WaitTime { get; private init; } = 30 * 1000;

        /// <summary>
        /// An instance with default options
        /// </summary>
        public static readonly CommandLineOptions DefaultCommandLineOptions = new()
        {
            Debug = false,
            DisableLeaderboard = false,
            DisableUpdate = false,
            IncludeBeta = false,
            InitialPops = 1,
            MaxFailures = 3,
            Verbose = false,
            WaitTime = 30 * 1000
        };
    }
}