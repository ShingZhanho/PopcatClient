using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class Leaderboard
        {
            public static string OpenLine() => LanguageManager.GetString("leaderboard_open");
            
            public static string CloseLine() => LanguageManager.GetString("leaderboard_close");

            public static string ColumnHeader_Rank() =>
                LanguageManager.GetString("leaderboard_col_header_rank");
            
            public static string ColumnHeader_Location() =>
                LanguageManager.GetString("leaderboard_col_header_location");
            
            public static string ColumnHeader_Pops() =>
                LanguageManager.GetString("leaderboard_col_header_pops");

            public static string LocationHere() => LanguageManager.GetString("leaderboard_location_here");

            public static string Format_ColumnHeaders() => LanguageManager.GetString("format@leaderboard_col_headers");
            
            public static string Format_ColumnEntries() => LanguageManager.GetString("format@leaderboard_col_entries");

            public static string Verbose_Msg_TryingGetLeaderboard() => LanguageManager
                .GetString("verbose@msg_trying_get_leaderboard");

            public static string ErrMsg_GetLeaderboardFailedNetwork() => LanguageManager
                .GetString("err-msg_get_leaderboard_failed_network");

            public static string Verbose_MsgDeserializingJson() => LanguageManager
                .GetString("verbose@msg_deserializing_json");
        }
    }
}