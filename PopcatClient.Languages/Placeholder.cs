using System.Text.RegularExpressions;

namespace PopcatClient.Languages
{
    public static class Placeholder
    {
        public static string Substitute(this string text, string placeholderName, string value)
        {
            var regex = new Regex("%%(" + placeholderName + ")%%");
            if (!regex.IsMatch(text)) return text; // return original string if placeholder not found
            var match = regex.Match(text);
            return text.Replace(match.Value, value);
        }

        public static string SubstituteNumeric(this string text, string placeholderName, int value)
        {
            var regex = new Regex("%%" + placeholderName + @"\|(.*)\|([^%]*)%%");
            if (!regex.IsMatch(text)) return text; // return original string if placeholder not found
            var match = regex.Match(text);
            var singularForm = match.Groups[1].Value;
            var pluralForm = match.Groups[2].Value;
            return text.Replace(match.Value, value + (value == 1 ? singularForm : pluralForm));
        }
    }
}