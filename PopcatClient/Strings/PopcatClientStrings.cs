using System;
using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class PopcatClient
        {
            public static string Msg_TryingPops(int pops) => LanguageManager.GetString("msg_trying_pop")
                .SubstituteNumeric("pop_count", pops);

            public static string SucMsg_PopSent(int pops) => LanguageManager.GetString("success-msg_pop_sent")
                .SubstituteNumeric("pop_count", pops);

            public static string Msg_TotalPops(int totalPops) => LanguageManager.GetString("msg_total_pops")
                .Substitute("pop_count", totalPops.ToString());

            public static string WarnMsg_SequentialFailures(int failures, int max) => LanguageManager
                .GetString("warn-msg_sequential_failures")
                .Substitute("failure_count", failures.ToString())
                .SubstituteNumeric("max_failures", max);

            public static string Msg_NextPopTime(int waitTime, DateTime nextPopTime) => LanguageManager
                .GetString("msg-next_pop_time")
                .Substitute("wait_time", waitTime.ToString())
                .Substitute("datetime", nextPopTime.ToString(Common.Format_Datetime(), 
                    LanguageManager.Language.LanguageInfo));

            public static string ErrMsg_MaxFailuresReached(int maxFailures) => LanguageManager
                .GetString("err-msg_max_failures_reached")
                .SubstituteNumeric("max_fails", maxFailures);

            public static string ErrMsg_PopFailedNetwork() => LanguageManager.GetString("err-msg_pop_failed_network");

            public static string ErrMsg_ExtractTokenFailed() => LanguageManager
                .GetString("err-msg_extract_token_failed");

            public static string Verbose_Msg_TokenExtracted(string token) => LanguageManager
                .GetString("verbose@msg_token_extracted")
                .Substitute("token", token);

            public static string Verbose_Msg_LocationCodeExtracted(string locationCode) => LanguageManager
                .GetString("verbose@msg_location_code_extracted")
                .Substitute("code", locationCode);

            public static string ErrMsg_ApplicationStopped() => LanguageManager
                .GetString("err-msg_application_stopped");

            public static string StatusMsg_PopsSent() => LanguageManager.GetString("status-msg_pops_sent");
            
            public static string StatusMsg_PopsFailed() => LanguageManager.GetString("status-msg_pops_failed");
        }
    }
}