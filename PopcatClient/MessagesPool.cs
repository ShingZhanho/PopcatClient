using System.Collections.Generic;

namespace PopcatClient
{
    /// <summary>
    /// Represents a collection of messages that will be processed sequentially.
    /// </summary>
    public class MessagesPool
    {
        private readonly List<Message> _messages = new();
    }
}