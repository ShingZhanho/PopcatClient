using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PopcatClient
{
    public static class Program
    {
        private static readonly HttpClient HttpClient = new();
        private const string PostUrl = "https://stats.popcat.click/pop";
        public static CommandLineOptions Options;

        public static void Main(string[] args)
        {
            Options = new CommandLineOptions(args);
            
            CommandLine.WriteWarningVerbose("Verbose mode is enabled.");
            CommandLine.WriteWarningDebug("Debug mode is enabled.");
        }

        private static (int, string) SendRequest(int popCount, string captchaToken = "8964")
        {
            var parameters = new Dictionary<string, string>
            {
                {"pop_count", popCount.ToString()},
                {"captcha_token", captchaToken}
            };
            var content = new FormUrlEncodedContent(parameters);
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36");
            var response = HttpClient.PostAsync(PostUrl, content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return ((int) response.StatusCode, responseString);
        }
    }
}