using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PopcatClient
{
    public class LeaderboardClient : IDisposable
    {
        public LeaderboardClient(CommandLineOptions options)
        {
            _options = options;
            LeaderboardRunning = !_options.DisableLeaderboard;
        }

        private readonly CommandLineOptions _options;
        private readonly HttpClient _client = new();
        private const string UserAgentString =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36";
        private const string RequestUrl = "https://leaderboard.popcat.click/";
        private Thread _leaderboardThread;
        private bool _terminateThread;

        /// <summary>
        /// Indicates whether the leaderboard thread is running
        /// </summary>
        public bool LeaderboardRunning { get; private set; }

        /// <summary>
        /// Stores all countries' pop counts
        /// </summary>
        public Dictionary<string, long> Leaderboard { get; private set; } = new();
        /// <summary>
        /// Raised each time fetch is successful.
        /// </summary>
        public event EventHandler<LeaderboardFetchFinishedEventArgs> LeaderboardFetchFinished; 

        public void Run()
        {
            // configure http client
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", UserAgentString);
            
            _leaderboardThread = new Thread(LeaderboardPrivateRunner);
            _leaderboardThread.Start();
        }

        private void LeaderboardPrivateRunner()
        {
            while (!_terminateThread)
            {
                CommandLine.WriteMessageVerbose("Trying to get leaderboard information.");
                CommandLine.WriteMessageVerbose($"GET {RequestUrl}");
                // get leaderboard information
                var response = _client.GetAsync(RequestUrl).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                // check results
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // 200 OK
                    CommandLine.WriteMessageVerbose($"Returned: {responseString}");
                    ExtractLeaderboard(responseString);
                    LeaderboardFetchFinished?.Invoke(this, new LeaderboardFetchFinishedEventArgs(Leaderboard));
                }
                else
                {
                    // other failures
                    CommandLine.WriteErrorVerbose($"Failed to get leaderboard information. Status: " +
                                                  $"{(int)response.StatusCode} - {response.StatusCode.ToString()}");
                }
                Thread.Sleep(_options.WaitTime);
            }

            LeaderboardRunning = false;
        }

        private void ExtractLeaderboard(string json)
        {
            CommandLine.WriteMessageVerbose("Deserializing leaderboard JSON");
            var jObject = (JObject) JToken.Parse(json);
            var dict = new Dictionary<string, long>();
            foreach (var (key, value) in jObject) dict.Add(key, long.Parse(value.ToString()));
            Leaderboard = dict;
        }

        public void Dispose()
        {
            _terminateThread = true;
        }
    }

    public class LeaderboardFetchFinishedEventArgs : EventArgs
    {
        public LeaderboardFetchFinishedEventArgs(Dictionary<string, long> leaderboard)
        {
            Leaderboard = leaderboard;
        }

        public Dictionary<string, long> Leaderboard { get; }
    }
}