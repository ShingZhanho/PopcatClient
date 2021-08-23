using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class PackInfo
        {
            public static string LanguagePack() => LanguageManager.GetString("lang_pack");
            
            public static string FallbackLanguagePack() => LanguageManager.GetString("fallback_lang_pack");

            public static string PackInfo_Authors(string[] authors) => LanguageManager.GetString("pack-info_written_by")
                .Substitute("authors", string.Join(", ", authors));
        }
    }
}