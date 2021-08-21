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
            // for logging
            var logs = new List<string>();
            
            var fileName = args[0];
            AddToLog(logs, $"File name: {fileName}");
            if (!File.Exists(fileName))
            {
                Console.WriteLine("{0} does not exist.", fileName);
                AddToLog(logs, $"File does not exist.");
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
                AddToLog(logs, "Failed to read file.");
                Environment.Exit(2);
            }

            // look for string between <SourceVersionName> and </SourceVersionName>
            var tagStartIndex = lines.IndexOf(lines.First(line => line.Contains("<SourceVersionName>")));
            var stringBetweenTag = lines.Skip(tagStartIndex + 1).First();
            
            var parsedVersionName = stringBetweenTag.StringBetween("\"", "\"");
            AddToLog(logs, $"Parsed source version name: {parsedVersionName}");
            if (!VersionName.VersionNameIsValid(parsedVersionName))
            {
                Console.WriteLine("Invalid version name: {0}.", parsedVersionName);
                AddToLog(logs, $"The parsed source version name is invalid. ({parsedVersionName})");
                Environment.Exit(3);
            }
            var sourceVersion = new VersionName(parsedVersionName);
            
            tagStartIndex = lines.IndexOf(lines.First(line => line.Contains("<FileVersionName>")));
            stringBetweenTag = lines.Skip(tagStartIndex + 1).First();
            
            // for replacing original value
            var versionNameLineIndex = lines.IndexOf(stringBetweenTag);
            
            parsedVersionName = stringBetweenTag.StringBetween("\"", "\"");
            AddToLog(logs, $"Parsed file version name: {parsedVersionName}");
            var buildNumber = 0;
            if (parsedVersionName.Split('.').Length == 4)
                if (!int.TryParse(parsedVersionName.Split('.').Last(), out buildNumber))
                    buildNumber = 0;
                else buildNumber++;
            AddToLog(logs, $"New file version name: {sourceVersion.GetFourDigitVersionName(buildNumber)}");
            
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
                AddToLog(logs, "Could not write to file.");
                Environment.Exit(4);
            }
            
            File.WriteAllLines($".\\log_{DateTime.Now:dd-MM-yyyy}_{DateTime.Now:HH-mm-ss-fff}.log", logs);
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

        private static void AddToLog(ICollection<string> logObject, string message) => 
            logObject.Add(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.fff tt") + $" {message}");
    }
}