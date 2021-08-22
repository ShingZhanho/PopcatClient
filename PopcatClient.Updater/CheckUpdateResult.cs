namespace PopcatClient.Updater
{
    /// <summary>
    /// An object contains results of checking updates
    /// </summary>
    public class CheckUpdateResult : BasicTaskResult
    {
        internal CheckUpdateResult() { }
        
        /// <summary>
        /// Indicates the status of the result.
        /// </summary>
        public CheckUpdateResultStatus ResultStatus { get; internal set; }
        /// <summary>
        /// The version of current application.
        /// </summary>
        public VersionName CurrentVersion { get; internal init; }
        /// <summary>
        /// The version of the latest version on server.
        /// </summary>
        public VersionName ServerLatestVersion { get; internal set; }
        /// <summary>
        /// The Url to the asset file.
        /// </summary>
        public string AssetDownloadUrl { get; internal set; }
    }

    /// <summary>
    /// Represents the status of checking updates.
    /// </summary>
    public enum CheckUpdateResultStatus
    {
        /// <summary>
        /// The application is up-to-date
        /// </summary>
        UpToDate, 
        /// <summary>
        /// There is an update available on server
        /// </summary>
        UpdateAvailable,
        /// <summary>
        /// Error occured while checking update
        /// </summary>
        Failed
    }
}