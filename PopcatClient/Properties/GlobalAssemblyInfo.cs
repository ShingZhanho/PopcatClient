using System.Reflection;
using PopcatClient;
using PopcatClient.Updater;

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
            // <SourceVersionName>
            "v0.1-beta.3"
            // </SourceVersionName>
            ;
        public const string FileVersion = 
            // <FileVersionName>
            "0.1.0.9"
            // </FileVersionName>
            ;
    }
}
