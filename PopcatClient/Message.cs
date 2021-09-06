﻿using System;

namespace PopcatClient
{
    /// <summary>
    /// Represents a message that is being written to the console and the log file.
    /// </summary>
    public class Message
    {
        public Message(string messageBody, MessageType type, MessageMode mode, DateTime messageTime)
        {
            MessageBody = messageBody;
            Type = type;
            Mode = mode;
            MessageTime = messageTime;
        }

        public Message(string messageBody, DateTime messageTime) : this(messageBody, MessageType.Normal,
            MessageMode.Normal, messageTime) { }

        public MessageType Type { get; }
        public MessageMode Mode { get; }
        public string MessageBody { get; }
        public DateTime MessageTime { get; }
    }

    public enum MessageType
    {
        Normal, Success, Error, Warning
    }

    public enum MessageMode
    {
        Normal, Verbose, Debug
    }
}