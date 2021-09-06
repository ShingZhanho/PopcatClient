using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class MessagesPool
        {
            public static string ErrMsg_UnexpectedMessageType(int enumVal, string originalMsg) =>
                LanguageManager.GetString("err-msg_unexpected_message_type")
                    .Substitute("enum_val", enumVal.ToString())
                    .Substitute("original_body", originalMsg);
            public static string ErrMsg_UnexpectedMessageMode(int enumVal, string originalMsg) =>
                LanguageManager.GetString("err-msg_unexpected_message_mode")
                    .Substitute("enum_val", enumVal.ToString())
                    .Substitute("original_body", originalMsg);
        }
    }
}