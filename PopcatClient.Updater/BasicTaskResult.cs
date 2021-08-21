namespace PopcatClient.Updater
{
    public class BasicTaskResult
    {
        internal BasicTaskResult() {}
        
        /// <summary>
        /// The message if error occured.
        /// </summary>
        public string ExceptionMessage { get; internal set; }
        /// <summary>
        /// The stacktrace if error occured.
        /// </summary>
        public string ExceptionStacktrace { get; internal set; }
    }

    /// <summary>
    /// Indicates a basic task's status
    /// </summary>
    public enum BasicResultStatus
    {
        /// <summary>
        /// Represents that the task was completed successfully.
        /// </summary>
        Success,
        /// <summary>
        /// Represents that the task has failed.
        /// </summary>
        Failed
    }
}