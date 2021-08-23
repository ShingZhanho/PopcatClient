using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class CommandLineOptions
        {
            public static string WarnMsg_NoParameterSpecified(string argName, string defaultVal) =>
                LanguageManager.GetString("warn-msg_cmd_options_no_parameter_specified")
                    .Substitute("argument_name", argName)
                    .Substitute("default_val", defaultVal);

            public static string WarnMsg_InvalidParameterSpecified(string argName, string defaultVal) =>
                LanguageManager.GetString("warn-msg_cmd_options_invalid_parameter_specified")
                    .Substitute("argument_name", argName)
                    .Substitute("default_val", defaultVal);

            public static string WarnMsg_ParameterSpecifiedDoesNotFit(string argName, string defaultVal) =>
                LanguageManager.GetString("warn-msg_cmd_options_parameter_specified_not_fit")
                    .Substitute("argument_name", argName)
                    .Substitute("default_val", defaultVal);
        }
    }
}