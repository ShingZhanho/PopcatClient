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
        /// <summary>
        /// The path to the directory which contains all files of new version
        /// </summary>
        public string ExtractedDir { get; internal set; }
        /// <summary>
        /// The path to the installer executable file
        /// </summary>
        public string InstallerExecutable { get; internal set; }
    }
}