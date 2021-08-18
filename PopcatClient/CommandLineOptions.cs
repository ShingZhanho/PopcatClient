using System.Linq;

namespace PopcatClient
{
    public class CommandLineOptions
    {
        /// <summary>
        /// Extract command line arguments.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public CommandLineOptions(string[] args)
        {
            args ??= System.Array.Empty<string>();
            Verbose = args.Contains("--verbose");
            Debug = args.Contains("--debug");
        }
        
        /// <summary>
        /// Indicates whether verbose mode is enabled.
        /// </summary>
        public bool Verbose { get; }
        /// <summary>
        /// Indicates whether debug mode is enabled.
        /// </summary>
        public bool Debug { get; }
    }
}