using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PopcatClient
{
    public class CommandLineOptions
    {
        public CommandLineOptions() { }
        
        /// <summary>
        /// Extract command line arguments.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public CommandLineOptions(string[] args)
        {
            args ??= Array.Empty<string>();
            if (args.Length == 0) return;
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
            // --lang-fallback
            //
            FallbackLanguageId = ParseInt("--lang-fallback", args,
                value => CultureInfo.GetCultures(CultureTypes.AllCultures).Any(culture => culture.LCID == value),
                DefaultFallbackLanguage);
            //
            // --include-beta (shortname: -b)
            //
            IncludeBeta = args.Contains("--include-beta") || args.Contains("-b");
            //
            // --lang
            //
            LanguageId = ParseInt("--lang", args,
                value => CultureInfo.GetCultures(CultureTypes.AllCultures).Any(culture => culture.LCID == value),
                DefaultLanguageId);
            //
            // --max-failures
            //
            MaxFailures = ParseInt("--max-failures", args, value => value >= 1, DefaultMaxFailures);
            //
            // --verbose (shortname: -v)
            //
            Verbose = args.Contains("--verbose") || args.Contains("-v");
            //
            // --wait-time
            //
            WaitTime = ParseInt("--wait-time", args, value => value >= 30 * 1000, DefaultWaitTime);
        }

        private static int ParseInt(string argName, IReadOnlyList<string> args, Func<int, bool> criteria, int fallback)
        {
            if (!args.Contains(argName)) return fallback;
            if (args.ToList().IndexOf(argName) + 1 == args.Count)
            {
                CommandLine.WriteWarning(
                    Strings.CommandLineOptions.WarnMsg_NoParameterSpecified(argName, fallback.ToString()));
                return fallback;
            }

            var s = args[args.ToList().IndexOf(argName) + 1];
            
            if (!int.TryParse(s, out var result))
            {
                CommandLine.WriteWarning(
                    Strings.CommandLineOptions.WarnMsg_InvalidParameterSpecified(argName, fallback.ToString()));
                result = fallback;
                return result;
            }

            if (criteria(result)) return result;
            
            CommandLine.WriteWarning(
                Strings.CommandLineOptions.WarnMsg_ParameterSpecifiedDoesNotFit(argName, fallback.ToString()));
            result = fallback;
            return result;
        }
        
        /// <summary>
        /// Indicates whether clear the temp folder.
        /// </summary>
        public bool ClearTempDir { get; }
        /// <summary>
        /// Indicates whether debug mode is enabled.
        /// </summary>
        public bool Debug { get; }
        /// <summary>
        /// Indicates whether disable the leaderboard
        /// </summary>
        public bool DisableLeaderboard { get; }
        /// <summary>
        /// Indicates whether disable checking for updates on launching.
        /// </summary>
        public bool DisableUpdate { get; }
        /// <summary>
        /// The LCID of the fallback app display language
        /// </summary>
        public int FallbackLanguageId { get; } = DefaultFallbackLanguage;
        private const int DefaultFallbackLanguage = 2057;
        /// <summary>
        /// Indicates if to include beta versions when updating. This option will always be ignored if the app is already a beta.
        /// </summary>
        public bool IncludeBeta { get; }
        /// <summary>
        /// The LCID of the desired app language.
        /// </summary>
        public int LanguageId { get; } = DefaultLanguageId;
        private static readonly int DefaultLanguageId = CultureInfo.CurrentCulture.LCID;
        /// <summary>
        /// Indicates how many times of failures in a row should the application exit.
        /// </summary>
        public int MaxFailures { get; } = DefaultMaxFailures;
        private const int DefaultMaxFailures = 5;
        /// <summary>
        /// Indicates whether verbose mode is enabled.
        /// </summary>
        public bool Verbose { get; }
        /// <summary>
        /// Indicates the time should the program wait between each pop in ms.
        /// </summary>
        public int WaitTime { get; } = DefaultWaitTime;
        private const int DefaultWaitTime = 30 * 1000;
    }
}