using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;
using PopcatClient.Languages.Exceptions;
using PopcatClient.Updater;

namespace PopcatClient.Languages
{
    /// <summary>
    /// Represents a .json language pack file
    /// </summary>
    public class LanguageFile
    {
        public LanguageFile(string filename)
        {
            Filename = filename;
            ParseFile();
        }

        /// <summary>
        /// The filename of the language file
        /// </summary>
        public string Filename { get; }
        /// <summary>
        /// The LCID of the language pack
        /// </summary>
        public CultureInfo LanguageInfo { get; internal set; }
        /// <summary>
        /// The version that this language pack is designed for
        /// </summary>
        public VersionName LanguagePackVersion { get; internal set; }
        /// <summary>
        /// The author(s) of this pack, optional
        /// </summary>
        public string[] Authors { get; internal set; }

        internal Dictionary<string, string> StringEntries;
        private JObject _jo;

        private void ParseFile()
        {
            var content = File.ReadAllText(Filename);
            _jo = JObject.Parse(content);
            
            // get pack schema version
            var schemaVer = 0;
            try
            {
                _ = int.TryParse(_jo["metadata"]["pack_info"]["schema_version"].ToString(), out schemaVer);
            }
            catch (NullReferenceException nullReferenceException)
            {
                throw new InvalidLanguagePackFormatException("Key \"schema_version\" does not exist in the file.",
                    nullReferenceException);
            }

            switch (schemaVer)
            {
                case 1:
                    this.ParseSchemaVersion1(_jo);
                    break;
                default:
                    throw new InvalidLanguagePackFormatException("Value of key \"schema_version\" is invalid.");
            }
        }
    }
}