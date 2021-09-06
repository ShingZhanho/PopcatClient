﻿using System;

namespace PopcatClient
{
    /// <summary>
    /// Represents a message that is being written to the console and the log file.
    /// </summary>
    public class Message
    {
        public Message(string messageBody, MessageType type = MessageType.Info, MessageMode mode = MessageMode.Normal)
        {
            MessageBody = messageBody;
            Type = type;
            Mode = mode;
            MessageTime = DateTime.Now;
        }

        public MessageType Type { get; }
        public MessageMode Mode { get; }
        public string MessageBody { get; }
        public DateTime MessageTime { get; }
    }

    public enum MessageType
    {
        Info, Success, Error, Warning
    }

    public enum MessageMode
    {
        Normal, Verbose, Debug
    }
}