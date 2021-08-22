namespace PopcatClient.Updater
{
    /// <summary>
    /// An object represents the result of a downloading task
    /// </summary>
    public class DownloadUpdateResult : BasicTaskResult
    {
        internal DownloadUpdateResult() {}
        
        /// <summary>
        /// The status of the download task.
        /// </summary>
        public BasicResultStatus Status { get; internal set; }
        /// <summary>
        /// The path of the downloaded file.
        /// </summary>
        public string FilePath { get; internal set; }
    }
}