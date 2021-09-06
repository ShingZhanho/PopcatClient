using System.Collections.Generic;
using System.Linq;

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
            
        }
    }
}