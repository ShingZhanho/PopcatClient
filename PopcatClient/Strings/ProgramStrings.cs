using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class Program
        {
            public static string WarningMsg_VerboseMode() => LanguageManager.GetString("warn-msg_verbose_mode");
            public static string WarningMsg_DebugMode() => LanguageManager.GetString("warn-msg_debug_mode");

            public static string WarningMsg_CannotDetermineIfUpToDate() =>
                LanguageManager.GetString("warn-msg_cannot_determine_if_uptodate");
            public static string WarningMsg_CannotDetermineIfDownloaded() =>
                LanguageManager.GetString("warn-msg_cannot_determine_if_downloaded");
            public static string WarningMsg_CannotDetermineIfReady() =>
                LanguageManager.GetString("warn-msg_cannot_determine_if_ready");
            public static string WarningMsg_AppWillRestartAfterUpdate() =>
                LanguageManager.GetString("warn-msg_update_restart_app");
        }
    }
}