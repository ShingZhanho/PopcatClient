using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using PopcatClient.Updater.Utils;

namespace PopcatClient.Updater
{
    public static class UpdateTools
    {
        private const string RepoOwner = "ShingZhanho";
        private const string RepoName = "PopcatClient";

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
                    result.Status = CheckUpdateStatus.UpToDate;
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
                result.Status = CheckUpdateStatus.UpdateAvailable;
                result.ServerLatestVersion = releases.First().TagName;
                result.AssetDownloadUrl = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/{assetId}";
            }
            catch (Exception e)
            {
                result.Status = CheckUpdateStatus.Failed;
                result.ExceptionMessage = e.Message;
                result.ExceptionStacktrace = e.StackTrace;
            }

            return result;
        }
    }
}