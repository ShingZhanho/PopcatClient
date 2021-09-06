using System;

namespace PopcatClient
{
    /// <summary>
    /// A class contains the colour scheme to be used to display messages.
    /// </summary>
    public class ColourScheme
    {
        // TODO: Load a colour scheme file (*.pcc.json) to override the default scheme
        
        // normal messages colours
        public ConsoleColor NormalMsgText { get; } = ConsoleColor.White;
        public ConsoleColor NormalMsgBack { get; } = ConsoleColor.Black;
        public ConsoleColor NormalMsgTagText { get; } = ConsoleColor.Black;
        public ConsoleColor NormalMsgTagBack { get; } = ConsoleColor.White;
        
        // warning messages colours
        public ConsoleColor WarningMsgText { get; } = ConsoleColor.Yellow;
        public ConsoleColor WarningMsgBack { get; } = ConsoleColor.Black;
        public ConsoleColor WarningMsgTagText { get; } = ConsoleColor.Black;
        public ConsoleColor WarningMsgTagBack { get; } = ConsoleColor.Yellow;
        
        // success messages colours
        public ConsoleColor SuccessMsgText { get; } = ConsoleColor.Green;
        public ConsoleColor SuccessMsgBack { get; } = ConsoleColor.Black;
        public ConsoleColor SuccessMsgTagText { get; } = ConsoleColor.Black;
        public ConsoleColor SuccessMsgTagBack { get; } = ConsoleColor.Green;
        
        // error messages colours
        public ConsoleColor ErrorMsgText { get; } = ConsoleColor.Red;
        public ConsoleColor ErrorMsgBack { get; } = ConsoleColor.Black;
        public ConsoleColor ErrorMsgTagText { get; } = ConsoleColor.Black;
        public ConsoleColor ErrorMsgTagBack { get; } = ConsoleColor.Red;
    }
}