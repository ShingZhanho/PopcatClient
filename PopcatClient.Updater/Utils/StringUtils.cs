using System;
using System.Collections.Generic;
using System.Linq;

namespace PopcatClient.Updater.Utils
{
    public static class StringUtils
    {
        /// <summary>
        /// Gets the string between two given strings.
        /// </summary>
        /// <param name="source">The string to be searched in.</param>
        /// <param name="beginningStr">The begging string.</param>
        /// <param name="endingStr">The ending string.</param>
        /// <returns>The found string. Empty string is returned if nothing found.</returns>
        public static string StringBetween(this string source, string beginningStr, string endingStr)
        {
            if (string.IsNullOrEmpty(beginningStr) || string.IsNullOrEmpty(endingStr))
                throw new ArgumentException("beginningStr or endingStr cannot be null.");
            if (!source.Contains(beginningStr) || !source.Contains(endingStr)) return string.Empty;
            
            var positionStart = source.IndexOf(beginningStr, StringComparison.Ordinal);
            var positionEnd = -1;
            foreach (var index in source.AllIndexesOf(endingStr).Where(index => index > positionStart + beginningStr.Length - 1))
            {
                positionEnd = index;
                break;
            }
            return positionEnd == -1 
                ? string.Empty 
                : source.Substring(positionStart + beginningStr.Length, positionEnd - positionStart - beginningStr.Length);
        }
        
        /// <summary>
        /// Gets all indexes of matching occurrences.
        /// </summary>
        /// <param name="str">The string to be searched in.</param>
        /// <param name="value">The string to be matched.</param>
        /// <returns>The matching indexes.</returns>
        public static IEnumerable<int> AllIndexesOf(this string str, string value) {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("The string to find may not be empty", nameof(value));
            var indexes = new List<int>();
            for (var index = 0;; index += value.Length) {
                index = str.IndexOf(value, index, StringComparison.Ordinal);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}