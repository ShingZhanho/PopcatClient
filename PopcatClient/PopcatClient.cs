using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace PopcatClient
{
    public class PopcatClient : IDisposable
    {
        public PopcatClient(int waitTime)
        {
            WaitTime = waitTime;
        }

        /// <summary>
        /// The captcha token being sent to the server
        /// </summary>
        public string Token { get; private set; } = "8964";
        /// <summary>
        /// The time to wait between each pop
        /// </summary>
        public int WaitTime { get; }
        /// <summary>
        /// The total pop count the application has sent.
        /// </summary>
        public int TotalPops { get; private set; }

        private readonly HttpClient _client = new();
        private const string UserAgentString =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36";
        private const string RequestUrl = "https://stats.popcat.click/pop";

        public void Run()
        {
            // configure http client
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", UserAgentString);
            
            CommandLine.WriteMessage("PopcatClient object started.");
            var firstTrySuccessful = false;
            for (var i = 0; i < 3; i++)
            {
                CommandLine.WriteMessage($"Trying to send first pop (count: 1), attempt {i + 1} of 3.");
                var status = SendPopRequest(1);
                if ((int) status != 201)
                {
                    CommandLine.WriteMessage(i + 1 < 3
                        ? $"Retrying after {WaitTime}ms."
                        : "Failed 3 times, application will exit.");
                    if (i + 1 < 3) System.Threading.Thread.Sleep(WaitTime);
                }
                else
                {
                    firstTrySuccessful = true;
                    break;
                }
            }

            var sequentialFailures = 0;
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (firstTrySuccessful && sequentialFailures < 3)
            {
                System.Threading.Thread.Sleep(WaitTime);
                var popCount = new Random().Next(790, 800);
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
                                             "Application will exit after failed for 3 times in a row.");
                }
            }
            if (sequentialFailures == 3)
            {
                // failed for three times
                CommandLine.WriteError("Failed to send pops for 3 times. You may have been blocked, " +
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
            var response = _client.PostAsync(RequestUrl, content).Result;
            
            var responseString = response.Content.ReadAsStringAsync().Result;
            CommandLine.WriteMessageVerbose($"Response:\n\n{responseString}");
            if ((int)response.StatusCode == 201)
            {
                // extract token from response if success
                CommandLine.WriteSuccess($"Success! Status: {(int) response.StatusCode} - {response.StatusCode}.");
                TotalPops += count;

                var jo = JToken.Parse(responseString);
                if (jo["Token"] is null)
                {
                    CommandLine.WriteError("Cannot extract token from response. JSON format unexpected.");
                    End();
                }
                Token = jo["Token"]?.ToString();
                CommandLine.WriteMessageVerbose($"Token extracted: {Token}");
            }
            else
            {
                CommandLine.WriteError($"Failed. Status: {(int) response.StatusCode} - {response.StatusCode}.");
            }
            
            return response.StatusCode;
        }

        private void End()
        {
            CommandLine.WriteMessage("Application has stopped due to error(s). Press any key to exit.");
            Dispose();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}