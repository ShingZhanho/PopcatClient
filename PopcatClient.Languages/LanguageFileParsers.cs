using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PopcatClient.Languages.Exceptions;
using PopcatClient.Updater;

namespace PopcatClient.Languages
{
    internal static class LanguageFileParsers
    {
        public static void ParseSchemaVersion1(this LanguageFile languageFile, JObject jObject)
        {
            // language info
            ParseCultureInfo(languageFile, jObject);
            // authors
            ParseAuthorsInfo(languageFile, jObject);
            // version info
            ParseVersionInfo(languageFile, jObject);
            // gets all string entries
            ParseStringEntries(languageFile, jObject);
        }

        // the following functions are common functions for all versions of schemas
        private static void ParseCultureInfo(LanguageFile languageFile, JObject jObject) =>
            languageFile.LanguageInfo = GetCultureInfoFromLcid(jObject["metadata"]["language_info"]["lcid"].ToString()) 
                                        ?? throw new ArgumentException("The specified LCID is invalid");

        private static CultureInfo GetCultureInfoFromLcid(string lcid) =>
            !int.TryParse(lcid, out var id)
                ? null // not an integer
                : CultureInfo.GetCultures(CultureTypes.AllCultures).Any(info => info.LCID == id)
                    ? CultureInfo.GetCultureInfo(id) // found culture info
                    : null; // lcid not found

        private static void ParseAuthorsInfo(LanguageFile languageFile, JObject jObject)
        {
            string[] authors;
            try { authors = JsonConvert.DeserializeObject<string[]>(jObject["metadata"]["authors"].ToString()); }
            catch { authors = null; }
            languageFile.Authors = authors;
        }

        private static void ParseVersionInfo(LanguageFile languageFile, JObject jObject)
        {
            try
            {
                if (VersionName.TryParse(jObject["metadata"]["pack_info"]["pack_version"].ToString(), out var results))
                    languageFile.LanguagePackVersion = results;
                else
                    throw new InvalidLanguagePackFormatException("The value of key \"pack_version\" is invalid.");
            }
            catch (NullReferenceException nullReferenceException)
            {
                throw new InvalidLanguagePackFormatException("The key \"pack_version\" does not exist.",
                    nullReferenceException);
            }
        }

        private static void ParseStringEntries(LanguageFile languageFile, JObject jObject)
        {
            languageFile.StringEntries = ReadAllStringFromFile(languageFile.Filename,
                jObject["strings"].ToString(),
                string.IsNullOrEmpty(jObject["import_files"].ToString())
                ? null
                : JArray.Parse(jObject["import_files"].ToString()));
        }

        private static Dictionary<string, string> ReadAllStringFromFile(string filename, string stringKeys,
            JArray importFilesList)
        {
            var result = new Dictionary<string, string>();
            
            // read from the "string" key
            var stringKeysDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(stringKeys);
            try
            {
                if (stringKeysDictionary != null) 
                    foreach (var (stringName, stringValue) in stringKeysDictionary)
                        result.Add(stringName, stringValue);
            }
            catch (ArgumentException keyAlreadyExistsException)
            {
                throw new InvalidLanguagePackFormatException("The language pack contains duplicated keys.",
                    keyAlreadyExistsException);
            }
            
            // read from the "import_files" list
            try
            {
                if (importFilesList != null)
                    foreach (var file in importFilesList)
                    {
                        var importFilename = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(filename), file.ToString())); // filename of file to be imported
                        var fileContent = File.ReadAllText(importFilename); // content of file to be imported
                        var keysObject = JObject.Parse(fileContent)["strings"]; // the "strings" object of the file to be imported
                        var filesArray = string.IsNullOrEmpty(JObject.Parse(fileContent)["import_files"].ToString())
                            ? null
                            : JArray.Parse(JObject.Parse(fileContent)["import_files"].ToString()); // the array of files to import in the file to be imported
                        
                        foreach (var (stringName, stringValue) in ReadAllStringFromFile(importFilename,
                            keysObject.ToString(), filesArray)) 
                            result.Add(stringName, stringValue);
                    }
            }
            catch (Exception e)
            {
                throw new InvalidLanguagePackFormatException("Failed to read an imported file of a language pack.", e);
            }

            return result;
        }
    }
}