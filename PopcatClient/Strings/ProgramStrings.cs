using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class Program
        {
            public static string WarningMsg_VerboseMode() => LanguageManager.GetString("verbose@warn-msg_verbose_mode");
            public static string WarningMsg_DebugMode() => LanguageManager.GetString("debug@warn-msg_debug_mode");

            public static string Msg_ClearTempDir() => LanguageManager.GetString("msg_clear_temp_dir");
            public static string SuccessMsg_TempDirCleared() => LanguageManager.GetString("success-msg_temp_dir_cleared");
        }
    }
}