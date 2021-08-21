using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PopcatClient.Updater;
// ReSharper disable StringIndexOfIsCultureSpecific.1

/*
 * This tool locates the GlobalAssemblyInfo.cs and change the file version automatically.
 */

namespace BuildTool
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var fileName = args[0];
            if (!File.Exists(fileName))
            {
                Console.WriteLine("{0} does not exist.", fileName);
                Environment.Exit(1);
            }

            // read file
            List<string> lines = null;
            try
            {
                lines = File.ReadLines(fileName).ToList();
                if (lines is null) throw new NullReferenceException();
            }
            catch
            {
                Console.WriteLine("Failed to read {0}", fileName);
                Environment.Exit(2);
            }

            // look for string between <SourceVersionName> and </SourceVersionName>
            var tagStartIndex = lines.IndexOf(lines.First(line => line.Contains("<SourceVersionName>")));
            var stringBetweenTag = lines.Skip(tagStartIndex + 1).First();
            
            var parsedVersionName = stringBetweenTag.StringBetween("\"", "\"");
            if (!VersionName.VersionNameIsValid(parsedVersionName))
            {
                Console.WriteLine("Invalid version name: {0}.", parsedVersionName);
                Environment.Exit(3);
            }
            var sourceVersion = new VersionName(parsedVersionName);
            
            tagStartIndex = lines.IndexOf(lines.First(line => line.Contains("<FileVersionName>")));
            stringBetweenTag = lines.Skip(tagStartIndex + 1).First();
            
            // for replacing original value
            var versionNameLineIndex = lines.IndexOf(stringBetweenTag);
            
            parsedVersionName = stringBetweenTag.StringBetween("\"", "\"");
            var buildNumber = 0;
            if (parsedVersionName.Split('.').Length == 4)
                if (!int.TryParse(parsedVersionName.Split('.').Last(), out buildNumber))
                    buildNumber = 0;
                else buildNumber++;
            
            // insert new file version back to lines
            lines[versionNameLineIndex] = lines[versionNameLineIndex].Replace(
                parsedVersionName, sourceVersion.GetFourDigitVersionName(buildNumber));

            try
            {
                File.Delete(fileName);
                File.WriteAllLines(fileName, lines);
            }
            catch
            {
                Console.WriteLine("Could not write new contents to the file. " +
                                  "Use version control to restore the original file.");
                Environment.Exit(4);
            }
        }

        private static string StringBetween(this string source, string begin, string end)
        {
            var positionStart = source.IndexOf(begin);
            if (positionStart == -1) throw new ArgumentException("Not found begin string in source string.");
            var positionEnd = -1;
            foreach (var index in source.AllIndexesOf(end).Where(index => index > positionStart + begin.Length - 1))
            {
                positionEnd = index;
                break;
            }
            if (positionEnd == -1) throw new ArgumentException("Not found end string in source string.");
            return source.Substring(positionStart + 1, positionEnd - positionStart - 1);
        }

        private static IEnumerable<int> AllIndexesOf(this string str, string value) {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", nameof(value));
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