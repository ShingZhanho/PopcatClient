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
            if (leaderboardClient.LeaderboardRunning)
                leaderboardClient.LeaderboardFetchFinished += OnLeaderboardFetchFinished;
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
            
            CommandLine.WriteMessage("PopcatClient object started.");
            var firstTrySuccessful = false;
            for (var i = 0; i < Program.Options.MaxFailures; i++)
            {
                CommandLine.WriteMessage($"Trying to send first pop (count: {Options.InitialPops}), attempt {i + 1} of {Options.MaxFailures}.");
                var status = SendPopRequest(Options.InitialPops);
                if ((int) status != 201)
                {
                    CommandLine.WriteMessage(i + 1 < Options.MaxFailures
                        ? $"Retrying after {Options.WaitTime / 1000}s " +
                          $"at {DateTime.Now.AddMilliseconds(Options.WaitTime):dd-MM-yyyy hh:mm:ss tt}."
                        : $"Failed {Program.Options.MaxFailures} times, application will exit.");
                    if (i + 1 < Options.MaxFailures) System.Threading.Thread.Sleep(Options.WaitTime);
                }
                else
                {
                    firstTrySuccessful = true;
                    // start leaderboard client after first pop
                    if (!Options.DisableLeaderboard) _leaderboard.Run();
                    break;
                }
            }

            var sequentialFailures = 0;
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (firstTrySuccessful && sequentialFailures < Options.MaxFailures)
            {
                System.Threading.Thread.Sleep(Options.WaitTime);
                var popCount = 800;
                CommandLine.WriteMessage($"Trying to send {popCount} pops.");
                var status = SendPopRequest(popCount);
                if ((int) status == 201)
                {
                    sequentialFailures = 0;
                    CommandLine.WriteSuccess($"Sent {popCount} pops successfully.");
                    CommandLine.WriteMessage($"Total pops: {TotalPops}.");
                }
                else
                {
                    sequentialFailures += 1;
                    CommandLine.WriteWarning($"Sequential failures: {sequentialFailures}. " +
                                             $"Application will exit after failed for {Options.MaxFailures} times in a row.");
                }
                CommandLine.WriteMessage($"Sending pops after {Options.WaitTime/1000}s " +
                                         $"at {DateTime.Now.AddMilliseconds(Options.WaitTime):dd-MM-yyyy hh:mm:ss tt}.");
            }
            if (sequentialFailures == Options.MaxFailures)
            {
                // failed for three times
                CommandLine.WriteError($"Failed to send pops for {Options.MaxFailures} times. You may have been blocked, " +
                                       "please try restarting the application later.");
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
                CommandLine.WriteError("Failed. Please check your network connection and firewall settings.");
                return HttpStatusCode.BadRequest;
            }
            
            var responseString = response.Content.ReadAsStringAsync().Result;
            CommandLine.WriteMessageVerbose($"Response:\n\n{responseString}");
            if ((int)response.StatusCode == 201)
            {
                // extract token from response if success
                CommandLine.WriteSuccess($"Status: {(int) response.StatusCode} - {response.StatusCode}.");
                TotalPops += count;

                var jo = JToken.Parse(responseString);
                if (jo["Token"] is null)
                {
                    CommandLine.WriteError("Cannot extract token from response. JSON format unexpected.");
                    End();
                }
                // get token from response
                Token = jo["Token"]?.ToString();
                CommandLine.WriteMessageVerbose($"Token extracted: {Token}");
                // get location code from response
                LocationCode = jo["Location"]?["Code"]?.ToString();
                CommandLine.WriteMessageVerbose($"Location code extracted: {LocationCode}");
            }
            else
            {
                CommandLine.WriteError($"Failed. Status: {(int) response.StatusCode} - {response.StatusCode}.");
            }
            
            return response.StatusCode;
        }

        private void OnLeaderboardFetchFinished(object sender, LeaderboardFetchFinishedEventArgs eventArgs)
        {
            // gets list of values of leaderboard
            var counts = eventArgs.Leaderboard.Select(entry => entry.Value).ToList();
            counts.Sort((x, y) => y.CompareTo(x));
            // Gets current location's pop count
            var indexOfLocation = counts.IndexOf(eventArgs.Leaderboard[LocationCode]);
            CommandLine.WriteMessage($"Leaderboard: {LocationCode} #{indexOfLocation + 1:D3} {counts[indexOfLocation]:0,0} POPS");
            CommandLine.WriteMessage(
                $"             {eventArgs.Leaderboard.First(entry => entry.Value == counts.First()).Key} #001 " +
                $"{counts.First():0,0} POPS"); // gets the first one
        }

        private void End()
        {
            CommandLine.WriteError("Application has stopped due to error(s). Press any key to exit.");
            Dispose();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}