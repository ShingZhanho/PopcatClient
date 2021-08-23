using System.Reflection;
using PopcatClient;

// Version information
[assembly: AssemblyInformationalVersion(AssemblyData.InformationalVersion)]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion(AssemblyData.FileVersion)]

// Metadata
[assembly: AssemblyTitle("Popcat Client")]
[assembly: AssemblyDescription("A command line tool to send pops to popcat automatically.")]
[assembly: AssemblyCopyright("ShingZhanho (c) 2021  All Rights Reserved")]
[assembly: AssemblyProduct("Popcat Client")]


namespace PopcatClient
{
    internal static class AssemblyData
    {
        public const string InformationalVersion = 
            // <SourceVersionName> (this tag is required for build tools to run)
            "v0.1-beta.6"
            // </SourceVersionName>
            ;
        public const string FileVersion = 
            // <FileVersionName> (this tag is required for build tools to run)
            "0.1.0.50"
            // </FileVersionName>
            ;

        // Specifies the minimum version of language pack,
        // loading packs that are older than this version will cause loading fallback language
        public const string MinimumLanguagePackVersion = "v0.1-beta.6";
    }
}
