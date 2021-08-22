using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Octokit;
using PopcatClient.Updater.Utils;

namespace PopcatClient.Updater
{
    public static class UpdateTools
    {
        private const string RepoOwner = "ShingZhanho";
        private const string RepoName = "PopcatClient";

        /// <summary>
        /// Gets update information from server.
        /// </summary>
        /// <param name="currentVersion">The current application's version</param>
        /// <param name="includeBeta">Whether include beta versions</param>
        /// <returns>The results object</returns>
        public static async Task<CheckUpdateResult> CheckUpdate(VersionName currentVersion, bool includeBeta = false)
        {
            var result = new CheckUpdateResult
            {
                CurrentVersion = currentVersion
            };

            try
            {
                var github = new GitHubClient(new ProductHeaderValue(nameof(UpdateTools)));
                var releases = (await github.Repository.Release.GetAll(RepoOwner, RepoName)).ToList();
            
                // remove unpublished drafts
                releases = releases.Where(release => !release.Draft).ToList();
                // remove unsupported version names
                releases = releases.Where(release => VersionName.VersionNameIsValid(release.TagName)).ToList();
                // remove beta if specified
                if (!includeBeta) releases = releases.Where(release => !release.Prerelease).ToList();
                // remove versions older than current
                releases = releases.Where(release => release.TagName > currentVersion).ToList();

                if (releases.Count == 0)
                {
                    result.ResultStatus = CheckUpdateResultStatus.UpToDate;
                    return result;
                }
                
                releases.Sort((a, b) => ((VersionName) b.TagName).CompareTo(a.TagName));
                // get the download asset's file name
                var assetFileName = releases.First().Body.StringBetween("<AssetFileName>", "</AssetFileName>");
                // no file name is found for the version
                if (string.IsNullOrEmpty(assetFileName))
                    throw new InvalidDataException("The received release information is unexpected.");
                // if no matching asset is found
                if (releases.First().Assets.All(asset => asset.Name != assetFileName))
                    throw new InvalidDataException("The received release does not contain an asset for downloading.");
                var assetId = releases.First().Assets.First(asset => asset.Name == assetFileName).Id;
                result.ResultStatus = CheckUpdateResultStatus.UpdateAvailable;
                result.ServerLatestVersion = releases.First().TagName;
                result.AssetDownloadUrl = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/assets/{assetId}";
            }
            catch (Exception e)
            {
                result.ResultStatus = CheckUpdateResultStatus.Failed;
                result.ExceptionMessage = e.Message;
                result.ExceptionStacktrace = e.StackTrace;
            }

            return result;
        }

        /// <summary>
        /// Download the specified file to the destination.
        /// </summary>
        /// <param name="uri">The file to download.</param>
        /// <param name="destination">The destination file.</param>
        public static async Task<DownloadUpdateResult> DownloadUpdateAsset(string uri, string destination)
        {
            var result = new DownloadUpdateResult();
            try
            {
                using var webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.UserAgent, nameof(UpdateTools));
                webClient.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");
                await webClient.DownloadFileTaskAsync(uri, destination);
                result.FilePath = destination;
            }
            catch (Exception e)
            {
                result.Status = BasicResultStatus.Failed;
                result.ExceptionMessage = e.Message;
                result.ExceptionStacktrace = e.StackTrace;
            }

            return result;
        }
    }
}