using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class CommandLine
        {
            public static string InfoTag() => LanguageManager.GetString("output_tag_info");
            public static string SuccessTag() => LanguageManager.GetString("output_tag_success");
            public static string ErrorTag() => LanguageManager.GetString("output_tag_error");
            public static string WarningTag() => LanguageManager.GetString("output_tag_warning");
            public static string DebugTag() => LanguageManager.GetString("output_tag_debug");
            public static string VerboseTag() => LanguageManager.GetString("output_tag_verbose");
        }
    }
}