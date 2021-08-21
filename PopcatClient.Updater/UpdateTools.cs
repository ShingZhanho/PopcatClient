using System;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

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
            
                // remove unsupported version names
                releases = releases.Where(release => !VersionName.VersionNameIsValid(release.TagName)).ToList();
                // remove beta if specified
                releases = releases.Where(release => !release.Prerelease).ToList();
                // remove versions older than current
                releases = releases.Where(release => release.TagName <= currentVersion).ToList();
                
                releases.Sort((a, b) => ((VersionName) b.TagName).CompareTo(a.TagName));
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