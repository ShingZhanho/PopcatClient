﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

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
            for (var i = 0; i < 3; i++)
            {
                CommandLine.WriteMessage($"Trying to send first pop (count: 1), attempt {i + 1} of 3.");
                var status = SendPopRequest(1);
                if ((int) status != 201)
                {
                    CommandLine.WriteError($"Failed. Status code: {(int) status} - {status.ToString()}.");
                    CommandLine.WriteMessage(i + 1 < 3
                        ? $"Retrying after {WaitTime}ms."
                        : "Failed 3 times, application will exit.");
                    if (i + 1 == 3) End();
                }
                else break;
                System.Threading.Thread.Sleep(WaitTime);
            }
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
            CommandLine.WriteMessageVerbose($"Response: {responseString}");
            // extract token from response
            if ((int)response.StatusCode == 201)
            {
                
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