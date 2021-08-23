using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class Common
        {
            public static string Verbose_Msg_ServerResponse(string response) => 
                LanguageManager.GetString("verbose@msg_server_response")
                .Substitute("response", response);

            public static string Msg_ResponseStatus(string message, int statusCode, string description) =>
                LanguageManager.GetString("msg_response_status")
                    .Substitute("msg", message)
                    .Substitute("http_status_code", statusCode.ToString())
                    .Substitute("status_description", description);

            public static string Format_Datetime() => LanguageManager.GetString("format@datetime");
        }
    }
}