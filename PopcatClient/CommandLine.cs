#nullable enable
using System;

namespace PopcatClient
{
    public static class CommandLine
    {
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
        public static void WriteMessage(string text) => WriteMessage(text, null);
        /// <summary>
        /// Writes a formatted message to the console.
        /// </summary>
        /// <param name="text">The message to write</param>
        /// <param name="args">The format parameters</param>
        public static void WriteMessage(string text, params object[]? args) =>
            Output($"[{Strings.CommandLine.InfoTag()}] {text}", NormalText, NormalBack, args);
        /// <summary>
        /// Writes a verbose message to the console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The message to write</param>
        public static void WriteMessageVerbose(string text) => OutputWithTag($"[{Strings.CommandLine.InfoTag()}] {text}",
            Strings.CommandLine.VerboseTag(), NormalText, NormalBack);
        /// <summary>
        /// Writes a formatted verbose message to the console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The message to write.</param>
        /// <param name="args">The format parameters.</param>
        public static void WriteMessageVerbose(string text, params object[]? args) =>
            OutputWithTag($"[{Strings.CommandLine.InfoTag()}] {text}", Strings.CommandLine.VerboseTag(), NormalText,
                ErrorBack, args);
        /// <summary>
        /// Writes a debug message to the console. Has no effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The message to write</param>
        public static void WriteMessageDebug(string text) => OutputWithTag($"[{Strings.CommandLine.InfoTag()}] {text}", 
            Strings.CommandLine.InfoTag(), NormalText, NormalBack);
        
        /// <summary>
        /// Writes an error message to console.
        /// </summary>
        /// <param name="text">The error message to write.</param>
        public static void WriteError(string text) => 
            Output($"[{Strings.CommandLine.ErrorTag()}] {text}", ErrorText, ErrorBack);
        /// <summary>
        /// Writes a verbose error message to console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text"></param>
        public static void WriteErrorVerbose(string text) => OutputWithTag($"[{Strings.CommandLine.ErrorTag()}] {text}", 
            Strings.CommandLine.VerboseTag(), ErrorText, ErrorBack);
        /// <summary>
        /// Writes a debug error message to the console. Has no effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The error message to write</param>
        public static void WriteErrorDebug(string text) => OutputWithTag($"[{Strings.CommandLine.ErrorTag()}] {text}",
            Strings.CommandLine.DebugTag(), ErrorText, ErrorBack);
        
        /// <summary>
        /// Writes a warning message to console.
        /// </summary>
        /// <param name="text">The warning message to write</param>
        public static void WriteWarning(string text) => 
            Output($"[{Strings.CommandLine.WarningTag()}] {text}", WarningText, WarningBack);
        /// <summary>
        /// Writes a verbose error message to console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The warning message to write</param>
        public static void WriteWarningVerbose(string text) => OutputWithTag($"[{Strings.CommandLine.WarningTag()}] {text}",
            Strings.CommandLine.VerboseTag(), WarningText, WarningBack);
        /// <summary>
        /// Writes a debug warning message to the console. Has no effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The warning message to write</param>
        public static void WriteWarningDebug(string text) => OutputWithTag($"[{Strings.CommandLine.WarningTag()}] {text}", 
            Strings.CommandLine.DebugTag(), WarningText, WarningBack);

        /// <summary>
        /// Writes a success message to the console.
        /// </summary>
        /// <param name="text">The success message to write</param>
        public static void WriteSuccess(string text) => 
            Output($"[{Strings.CommandLine.SuccessTag()}] {text}", SuccessText, SuccessBack);
        /// <summary>
        /// Writes a verbose success message to the console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The success message to write</param>
        public static void WriteSuccessVerbose(string text) => OutputWithTag($"[{Strings.CommandLine.SuccessTag()}] {text}", 
            Strings.CommandLine.VerboseTag(), SuccessText, SuccessBack);
        /// <summary>
        /// Writes a debug success message to the console. Has not effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The success message to write</param>
        public static void WriteSuccessDebug(string text) => OutputWithTag($"[{Strings.CommandLine.SuccessTag()}] {text}", 
            Strings.CommandLine.DebugTag(), SuccessText, SuccessBack);

        private static void Output(string text, ConsoleColor textColour, ConsoleColor backgroundColour, object[]? args = null)
        {
            // Write timestamp
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{DateTime.Now:dd-MM-yyyy hh:mm:ss tt} ");
            // Change color
            Console.BackgroundColor = backgroundColour;
            Console.ForegroundColor = textColour;
            // Write message
            OutputText(text, args);
        }

        private static void OutputWithTag(string text, string tag,  ConsoleColor textColour, ConsoleColor backgroundColour, object[]? args = null)
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
            OutputText(text, args);
        }

        private static void OutputText(string text, object[]? args = null)
        {
            try
            {
                Console.WriteLine(text, args);
            }
            catch 
            {
                // write without formatting if error occured
                Console.WriteLine(text);
            }
        }
    }
}