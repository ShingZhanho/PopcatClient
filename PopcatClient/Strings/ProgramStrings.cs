using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class Program
        {
            public static string WarningMsg_VerboseMode() => 
                LanguageManager.GetString("verbose@warn-msg_verbose_mode");
            public static string WarningMsg_DebugMode() => 
                LanguageManager.GetString("debug@warn-msg_debug_mode");

            public static string Msg_ClearTempDir() => 
                LanguageManager.GetString("msg_clear_temp_dir");
            public static string SuccessMsg_TempDirCleared() => 
                LanguageManager.GetString("success-msg_temp_dir_cleared");
            
            // start options
            public static string Msg_ClientStartOptions() =>
                LanguageManager.GetString("msg_client_started_with_following");

            public static string Format_StartupOptionsEntries() =>
                LanguageManager.GetString("format@startup_option_entries");

            public static string StartupOptions_WaitTime() => 
                LanguageManager.GetString("startup_option_wait_time");

            public static string StartupOptions_MaxFailures() =>
                LanguageManager.GetString("startup_option_max_failures");

            public static string StartupOptions_Leaderboard() =>
                LanguageManager.GetString("startup_option_leaderboard");

            public static string StartupOptions_CheckUpdate() =>
                LanguageManager.GetString("startup_option_check_update");

            public static string StartupOptions_InstallBeta() =>
                LanguageManager.GetString("startup_option_install_beta");

            public static string StartupOptions_ClearTempDir() =>
                LanguageManager.GetString("startup_option_clear_temp_dir");

            public static string StartupOptions_LanguageId() => 
                LanguageManager.GetString("startup_option_lang_id");

            public static string StartupOptions_FallbackLanguageId() =>
                LanguageManager.GetString("startup_option_fallback_lang_id");
        }
    }
}