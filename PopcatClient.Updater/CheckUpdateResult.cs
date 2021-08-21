namespace PopcatClient.Updater
{
    /// <summary>
    /// An object contains results of checking updates
    /// </summary>
    public class CheckUpdateResult
    {
        internal CheckUpdateResult() { }
        
        /// <summary>
        /// Indicates the status of the result.
        /// </summary>
        public CheckUpdateStatus Status { get; internal set; }
        /// <summary>
        /// The message if error occured.
        /// </summary>
        public string ExceptionMessage { get; internal set; }
        /// <summary>
        /// The stacktrace if error occured.
        /// </summary>
        public string ExceptionStacktrace { get; internal set; }
        /// <summary>
        /// The version of current application.
        /// </summary>
        public VersionName CurrentVersion { get; internal set; }
        /// <summary>
        /// The version of the latest version on server.
        /// </summary>
        public VersionName ServerLatestVersion { get; internal set; }
        public string AssetDownloadUrl { get; internal set; }
    }

    /// <summary>
    /// Represents the status of checking updates.
    /// </summary>
    public enum CheckUpdateStatus
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