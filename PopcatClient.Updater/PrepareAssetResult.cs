namespace PopcatClient.Updater
{
    /// <summary>
    /// Contains information about the result of the task of preparing
    /// </summary>
    public class PrepareAssetResult : BasicTaskResult
    {
        /// <summary>
        /// Represents the status of the result
        /// </summary>
        public BasicResultStatus Status { get; internal set; }
        public string ExtractedDir { get; internal set; }
        public string InstallerExecutable { get; internal set; }
    }
}