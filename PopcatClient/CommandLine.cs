using System;

namespace PopcatClient
{
    public class CommandLine
    {
        public CommandLine(CommandLineOptions options = null)
        {
            _options = options ?? new CommandLineOptions(null);
        }

        private readonly CommandLineOptions _options;
        
        // colour scheme
        private const ConsoleColor NormalText = ConsoleColor.White;
        private const ConsoleColor NormalBack = ConsoleColor.Black;
        private const ConsoleColor ErrorText = ConsoleColor.Red;
        private const ConsoleColor ErrorBack = ConsoleColor.Black;
        private const ConsoleColor WarningText = ConsoleColor.Yellow;
        private const ConsoleColor WarningBack = ConsoleColor.Black;
        private const ConsoleColor SuccessText = ConsoleColor.Green;
        private const ConsoleColor SuccessBack = ConsoleColor.Black;

        /// <summary>
        /// Writes a message to the console.
        /// </summary>
        /// <param name="text">The message to write</param>
        public static void WriteMessage(string text) => Output($"[INFO] {text}", NormalText, NormalBack);
        /// <summary>
        /// Writes a verbose message to the console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The message to write</param>
        public static void WriteMessageVerbose(string text) => OutputWithTag($"[INFO] {text}", "VERBOSE", NormalText, NormalBack);
        /// <summary>
        /// Writes a debug message to the console. Has no effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The message to write</param>
        public static void WriteMessageDebug(string text) => OutputWithTag($"[INFO] {text}", "DEBUG", NormalText, NormalBack);
        
        /// <summary>
        /// Writes an error message to console.
        /// </summary>
        /// <param name="text">The error message to write.</param>
        public static void WriteError(string text) => Output($"[ERROR] {text}", ErrorText, ErrorBack);
        /// <summary>
        /// Writes a verbose error message to console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text"></param>
        public static void WriteErrorVerbose(string text) => OutputWithTag($"[ERROR] {text}", "VERBOSE", ErrorText, ErrorBack);
        /// <summary>
        /// Writes a debug error message to the console. Has no effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The error message to write</param>
        public static void WriteErrorDebug(string text) => OutputWithTag($"[ERROR] {text}", "DEBUG", ErrorText, ErrorBack);
        
        /// <summary>
        /// Writes a warning message to console.
        /// </summary>
        /// <param name="text">The warning message to write</param>
        public static void WriteWarning(string text) => Output($"[WARNING] {text}", WarningText, WarningBack);
        /// <summary>
        /// Writes a verbose error message to console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The warning message to write</param>
        public static void WriteWarningVerbose(string text) => OutputWithTag($"[WARNING] {text}", "VERBOSE", WarningText, WarningBack);
        /// <summary>
        /// Writes a debug warning message to the console. Has no effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The warning message to write</param>
        public static void WriteWarningDebug(string text) =>   
            OutputWithTag($"[WARNING] {text}", "DEBUG", WarningText, WarningBack);

        /// <summary>
        /// Writes a success message to the console.
        /// </summary>
        /// <param name="text">The success message to write</param>
        public static void WriteSuccess(string text) => Output($"[SUCCESS] {text}", SuccessText, SuccessBack);
        /// <summary>
        /// Writes a verbose success message to the console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The success message to write</param>
        public static void WriteSuccessVerbose(string text) => OutputWithTag($"[SUCCESS] {text}", "VERBOSE", SuccessText, SuccessBack);
        /// <summary>
        /// Writes a debug success message to the console. Has not effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The success message to write</param>
        public static void WriteSuccessDebug(string text) =>
            OutputWithTag($"[SUCCESS] {text}", "DEBUG", SuccessText, SuccessBack);

        private static void Output(string text, ConsoleColor textColour, ConsoleColor backgroundColour)
        {
            // Write timestamp
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{DateTime.Now:dd-MM-yyyy hh:mm:ss tt} ");
            // Change color
            Console.BackgroundColor = backgroundColour;
            Console.ForegroundColor = textColour;
            // Write message
            Console.WriteLine(text);
        }

        private static void OutputWithTag(string text, string tag,  ConsoleColor textColour, ConsoleColor backgroundColour)
        {
            switch (tag)
            {
                case "DEBUG" when !Program.Options.Debug:
                case "VERBOSE" when !Program.Options.Verbose:
                    return;
            }

            // Write timestamp
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{DateTime.Now:dd-MM-yyyy hh:mm:ss tt} ");
            // Write tag
            Console.BackgroundColor = textColour;
            Console.ForegroundColor = backgroundColour;
            Console.Write($" [{tag}] ");
            // Separator
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" ");
            // Write message
            Console.BackgroundColor = backgroundColour;
            Console.ForegroundColor = textColour;
            Console.WriteLine(text);
        }
    }
}