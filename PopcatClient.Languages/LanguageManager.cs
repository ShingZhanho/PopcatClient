namespace PopcatClient.Languages
{
    /// <summary>
    /// Manages the app's display language
    /// </summary>
    public static class LanguageManager
    {
        /// <summary>
        /// The default display language.
        /// </summary>
        public static LanguageFile Language { get; set; }
        /// <summary>
        /// The fallback language to display when default language has errors.
        /// </summary>
        public static LanguageFile FallbackLanguage { get; set; }

        /// <summary>
        /// Gets string from the default language file by the key.
        /// If the default language file does not contain that key, get the string from fallback language.
        /// If the key still cannot be found, the given fallback string or empty string is returned.
        /// </summary>
        /// <param name="key">The key of string to get.</param>
        /// <param name="fallback">The fallback string to return if not found. Default is empty.</param>
        /// <returns>The string got.</returns>
        public static string GetString(string key, string fallback = "")
            => string.IsNullOrEmpty(Language[key])
                ? string.IsNullOrEmpty(FallbackLanguage[key])
                    ? fallback
                    : FallbackLanguage[key]
                : Language[key];
    }
}