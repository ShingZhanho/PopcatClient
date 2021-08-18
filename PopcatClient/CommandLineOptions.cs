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
                    CommandLine.WriteWarning("No parameter specified for --wait-time. Using default value (300000).");
                else
                {
                    if (int.TryParse(args[args.ToList().IndexOf("--wait-time") + 1], out var result))
                        WaitTime = result;
                    else
                        CommandLine.WriteWarning("Invalid parameter specified for --wait-time. Using default value (300000).");
                }
            }
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
        public int WaitTime { get; private init; }

        public static readonly CommandLineOptions DefaultCommandLineOptions = new()
        {
            Verbose = false,
            Debug = false,
            WaitTime = 30 * 1000
        };
    }
}