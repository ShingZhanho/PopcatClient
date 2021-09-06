#nullable enable
using System;
using PopcatClient.Languages;

namespace PopcatClient
{
    public static class CommandLine
    {
        /// <summary>
        /// Writes a message to the console.
        /// </summary>
        /// <param name="text">The message to write</param>
        public static void WriteMessage(string text) => Program.MessagesPool.AppendMessage(new Message(text));
        
        /// <summary>
        /// Writes a formatted message to the console.
        /// </summary>
        /// <param name="text">The message to write</param>
        /// <param name="args">The format parameters</param>
        public static void WriteMessage(string text, params object[]? args) =>
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Info, MessageMode.Normal, args));

        /// <summary>
        /// Writes a verbose message to the console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The message to write</param>
        public static void WriteMessageVerbose(string text) =>
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Info, MessageMode.Verbose));

        /// <summary>
        /// Writes a formatted verbose message to the console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The message to write.</param>
        /// <param name="args">The format parameters.</param>
        public static void WriteMessageVerbose(string text, params object[]? args) =>
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Info, MessageMode.Verbose, args));

        /// <summary>
        /// Writes a debug message to the console. Has no effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The message to write</param>
        public static void WriteMessageDebug(string text) =>
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Info, MessageMode.Debug));
        
        /// <summary>
        /// Writes an error message to console.
        /// </summary>
        /// <param name="text">The error message to write.</param>
        public static void WriteError(string text) => 
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Error));
        
        /// <summary>
        /// Writes a verbose error message to console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text"></param>
        public static void WriteErrorVerbose(string text) => 
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Error, MessageMode.Verbose));
        
        /// <summary>
        /// Writes a debug error message to the console. Has no effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The error message to write</param>
        public static void WriteErrorDebug(string text) => 
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Error, MessageMode.Debug));
        
        /// <summary>
        /// Writes a warning message to console.
        /// </summary>
        /// <param name="text">The warning message to write</param>
        public static void WriteWarning(string text) => 
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Warning));
        
        /// <summary>
        /// Writes a verbose error message to console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The warning message to write</param>
        public static void WriteWarningVerbose(string text) => 
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Warning, MessageMode.Verbose));
        
        /// <summary>
        /// Writes a debug warning message to the console. Has no effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The warning message to write</param>
        public static void WriteWarningDebug(string text) => 
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Warning, MessageMode.Debug));

        /// <summary>
        /// Writes a success message to the console.
        /// </summary>
        /// <param name="text">The success message to write</param>
        public static void WriteSuccess(string text) => 
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Success));
        
        /// <summary>
        /// Writes a verbose success message to the console. Has no effect if verbose mode is not enabled.
        /// </summary>
        /// <param name="text">The success message to write</param>
        public static void WriteSuccessVerbose(string text) => 
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Success, MessageMode.Verbose));
        
        /// <summary>
        /// Writes a debug success message to the console. Has not effect if debug mode is not enabled.
        /// </summary>
        /// <param name="text">The success message to write</param>
        public static void WriteSuccessDebug(string text) => 
            Program.MessagesPool.AppendMessage(new Message(text, MessageType.Success, MessageMode.Debug));
    }
}