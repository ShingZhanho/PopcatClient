using System;
using System.Collections.Generic;
using System.Linq;
using PopcatClient.Languages;

namespace PopcatClient
{
    /// <summary>
    /// Represents a collection of messages that will be processed sequentially.
    /// </summary>
    public class MessagesPool
    {
        private readonly List<Message> _messages = new();

        private bool _isMessageWriterRunning;

        /// <summary>
        /// Append a message to the pool.
        /// </summary>
        /// <param name="message">The message to be appended.</param>
        public void AppendMessage(Message message)
        {
            _messages.Add(message);
            if (!_isMessageWriterRunning) Internal_MessageWriter();
        }

        /// <summary>
        /// Writes all messages in the pool to the console.
        /// </summary>
        private void Internal_MessageWriter()
        {
            // triggers this method when new message is added
            _isMessageWriterRunning = true;

            while (_messages.Count > 0)
            {
                Internal_WriteToConsole(_messages[0]);
                _messages.RemoveAt(0);
            }

            _isMessageWriterRunning = false;
        }

        /// <summary>
        /// Writes the message to the console.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        private void Internal_WriteToConsole(Message message)
        {
            // writes timestamp
            Console.BackgroundColor = Program.ColourScheme.NormalMsgBack;
            Console.ForegroundColor = Program.ColourScheme.NormalMsgText;
            Console.Write(
                $"{DateTime.Now.ToString(Strings.Common.Format_Datetime(), LanguageManager.Language.LanguageInfo)} ");

            // get tag colours
            ConsoleColor tagTextColour, tagBackColour;
            try
            {
                tagTextColour = message.Type switch
                {
                    MessageType.Info => Program.ColourScheme.NormalMsgTagText,
                    MessageType.Error => Program.ColourScheme.ErrorMsgTagText,
                    MessageType.Warning => Program.ColourScheme.WarningMsgTagText,
                    MessageType.Success => Program.ColourScheme.SuccessMsgTagText,
                    _ => throw new ArgumentOutOfRangeException(nameof(message.Type), "Invalid message type.")
                };
                
                tagBackColour = message.Type switch
                {
                    MessageType.Info => Program.ColourScheme.NormalMsgTagBack,
                    MessageType.Error => Program.ColourScheme.ErrorMsgTagBack,
                    MessageType.Warning => Program.ColourScheme.WarningMsgTagBack,
                    MessageType.Success => Program.ColourScheme.SuccessMsgTagBack,
                    _ => throw new ArgumentOutOfRangeException(nameof(message.Type), "Invalid message type.")
                };
            }
            catch (ArgumentException)
            {
                AppendMessage(new Message(
                    Strings.MessagesPool.ErrMsg_UnexpectedMessageType((int)message.Type, message.MessageBody),
                    MessageType.Error, MessageMode.Verbose));
                return;
            }
            
            // writes a tag in front of the body if any
            if (message.Mode != MessageMode.Normal)
            {
                Console.BackgroundColor = tagBackColour;
                Console.ForegroundColor = tagTextColour;
                try
                {
                    Console.Write($@"[{message.Mode switch
                    {
                        MessageMode.Debug => Strings.CommandLine.DebugTag(),
                        MessageMode.Verbose => Strings.CommandLine.VerboseTag(),
                        _ => throw new ArgumentOutOfRangeException(nameof(message.Mode), "Invalid message mode.")
                    }}] ");
                }
                catch (ArgumentOutOfRangeException)
                {
                    AppendMessage(new Message(
                        Strings.MessagesPool.ErrMsg_UnexpectedMessageMode((int)message.Mode, message.MessageBody),
                        MessageType.Error, MessageMode.Verbose));
                    return;
                }
            }

            // switches the colours to the corresponding colours
            switch (message.Type)
            {
                case MessageType.Info:
                    Console.BackgroundColor = Program.ColourScheme.NormalMsgBack;
                    break;
                case MessageType.Warning:
                    Console.BackgroundColor = Program.ColourScheme.WarningMsgBack;
                    break;
                case MessageType.Success:
                    Console.BackgroundColor = Program.ColourScheme.SuccessMsgBack;
                    break;
                case MessageType.Error:
                    Console.BackgroundColor = Program.ColourScheme.ErrorMsgBack;
                    break;
                default:
                    return;
            }
            switch (message.Type)
            {
                case MessageType.Info:
                    Console.ForegroundColor = Program.ColourScheme.NormalMsgText;
                    break;
                case MessageType.Warning:
                    Console.ForegroundColor = Program.ColourScheme.WarningMsgText;
                    break;
                case MessageType.Success:
                    Console.ForegroundColor = Program.ColourScheme.SuccessMsgText;
                    break;
                case MessageType.Error:
                    Console.ForegroundColor = Program.ColourScheme.ErrorMsgText;
                    break;
                default:
                    return;
            }
            
            // writes tag
            switch (message.Type)
            {
                case MessageType.Info:
                    Console.Write($"[{Strings.CommandLine.InfoTag()}] ");
                    break;
                case MessageType.Success:
                    Console.Write($"[{Strings.CommandLine.SuccessTag()}] ");
                    break;
                case MessageType.Error:
                    Console.Write($"[{Strings.CommandLine.ErrorTag()}] ");
                    break;
                case MessageType.Warning:
                    Console.Write($"[{Strings.CommandLine.WarningTag()}] ");
                    break;
                default:
                    return;
            }
            
            Console.WriteLine(message.MessageBody);
        }
    }
}