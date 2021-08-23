using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace PopcatClient
{
    public class PopcatClient : IDisposable
    {
        public PopcatClient(CommandLineOptions options, LeaderboardClient leaderboardClient)
        {
            Options = options;
            _leaderboard = leaderboardClient;
            _leaderboard.LeaderboardFetchFinished += OnLeaderboardFetchFinished;
        }

        /// <summary>
        /// The captcha token being sent to the server
        /// </summary>
        public string Token { get; private set; } = "8964";
        /// <summary>
        /// The total pop count the application has sent.
        /// </summary>
        public int TotalPops { get; private set; }
        /// <summary>
        /// The settings for the object to use.
        /// </summary>
        public CommandLineOptions Options { get; }
        /// <summary>
        /// The code of the current location.
        /// </summary>
        public string LocationCode { get; private set; }

        private readonly HttpClient _client = new();
        private const string UserAgentString =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36";
        private const string RequestUrl = "https://stats.popcat.click/pop";

        private readonly LeaderboardClient _leaderboard;

        public void Run()
        {
            // configure http client
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", UserAgentString);

            var sequentialFailures = 0;
            while (sequentialFailures < Options.MaxFailures)
            {
                const int popCount = 800;
                CommandLine.WriteMessage(Strings.PopcatClient.Msg_TryingPops(popCount));
                var status = SendPopRequest(popCount);
                if ((int) status == 201)
                {
                    sequentialFailures = 0;
                    CommandLine.WriteSuccess(Strings.PopcatClient.SucMsg_PopSent(popCount));
                    CommandLine.WriteMessage(Strings.PopcatClient.Msg_TotalPops(TotalPops));
                    if (!_leaderboard.LeaderboardRunning && !Options.DisableLeaderboard) _leaderboard.Run();
                }
                else
                {
                    sequentialFailures++;
                    CommandLine.WriteWarning(
                        Strings.PopcatClient.WarnMsg_SequentialFailures(sequentialFailures, Options.MaxFailures));
                }

                CommandLine.WriteMessage(Strings.PopcatClient.Msg_NextPopTime(Options.WaitTime / 1000,
                    DateTime.Now.AddMilliseconds(Options.WaitTime)));
                System.Threading.Thread.Sleep(Options.WaitTime);
            }
            if (sequentialFailures == Options.MaxFailures)
            {
                // failed for maximum times
                CommandLine.WriteError(Strings.PopcatClient.ErrMsg_MaxFailuresReached(Options.MaxFailures));
            }
            End();
        }

        private HttpStatusCode SendPopRequest(int count)
        {
            CommandLine.WriteMessageVerbose($"POST {RequestUrl}?pop_count={count}&captcha_token={Token}");
            var parameters = new Dictionary<string, string>
            {
                {"pop_count", count.ToString()},
                {"captcha_token", Token}
            };
            var content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response;
            try
            {
                response = _client.PostAsync(RequestUrl, content).Result;
            }
            catch
            {
                CommandLine.WriteError(Strings.PopcatClient.ErrMsg_PopFailedNetwork());
                return HttpStatusCode.BadRequest;
            }
            
            var responseString = response.Content.ReadAsStringAsync().Result;
            CommandLine.WriteMessageVerbose(Strings.Common.Verbose_Msg_ServerResponse(responseString));
            if ((int)response.StatusCode == 201)
            {
                // extract token from response if success
                CommandLine.WriteSuccess(Strings.Common.Msg_ResponseStatus("Pops sent.", (int)response.StatusCode,
                    response.StatusCode.ToString()));
                TotalPops += count;

                var jo = JToken.Parse(responseString);
                if (jo["Token"] is null)
                {
                    CommandLine.WriteError(Strings.PopcatClient.ErrMsg_ExtractTokenFailed());
                    End();
                }
                // get token from response
                Token = jo["Token"]?.ToString();
                CommandLine.WriteMessageVerbose(Strings.PopcatClient.Verbose_Msg_TokenExtracted(Token));
                // get location code from response
                LocationCode = jo["Location"]?["Code"]?.ToString();
                CommandLine.WriteMessageVerbose(Strings.PopcatClient.Verbose_Msg_LocationCodeExtracted(LocationCode));
            }
            else
            {
                CommandLine.WriteError(Strings.Common.Msg_ResponseStatus("Failed to send pops.", (int)response.StatusCode,
                    response.StatusCode.ToString()));
            }
            
            return response.StatusCode;
        }

        private void OnLeaderboardFetchFinished(object sender, LeaderboardFetchFinishedEventArgs eventArgs)
        {
            var leaderboard = eventArgs.Leaderboard.ToList();
            leaderboard.Sort((a, b) => b.Value.CompareTo(a.Value));

            List<LeaderboardItem> list = new();
            leaderboard.Take(3).ToList().ForEach(
                pair => list.Add(new LeaderboardItem(pair.Key, leaderboard.IndexOf(pair) + 1, pair.Value)));
            leaderboard.Skip(leaderboard.IndexOf(leaderboard.First(item => item.Key == LocationCode)) - 1)
                .Take(3).ToList().ForEach(
                    pair => list.Add(new LeaderboardItem(pair.Key, leaderboard.IndexOf(pair) + 1, pair.Value)));

            // prints leaderboard
            CommandLine.WriteMessage(Strings.Leaderboard.OpenLine());
            CommandLine.WriteMessage(Strings.Leaderboard.Format_ColumnHeaders(),
                Strings.Leaderboard.ColumnHeader_Rank(),
                Strings.Leaderboard.ColumnHeader_Location(),
                Strings.Leaderboard.ColumnHeader_Pops());
            var previousRank = 0;
            foreach (var item in list)
            {
                if (item.Rank - previousRank != 1) CommandLine.WriteMessage("  ...");
                CommandLine.WriteMessage(Strings.Leaderboard.Format_ColumnEntries(), 
                    item.Rank, item.LocationCode + (item.LocationCode == LocationCode ? " (HERE)" : ""), item.Pops);
                previousRank = item.Rank;
            }
            if (previousRank != leaderboard.Count) CommandLine.WriteMessage("  ...");
            CommandLine.WriteMessage(Strings.Leaderboard.CloseLine());
        }

        private readonly struct LeaderboardItem
        {
            public LeaderboardItem(string locationCode, int rank, long pops)
            {
                LocationCode = locationCode;
                Rank = rank;
                Pops = pops;
            }

            public string LocationCode { get; }
            public int Rank { get; }
            public long Pops { get; }
        }

        private void End()
        {
            CommandLine.WriteError(Strings.PopcatClient.ErrMsg_ApplicationStopped());
            Dispose();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}