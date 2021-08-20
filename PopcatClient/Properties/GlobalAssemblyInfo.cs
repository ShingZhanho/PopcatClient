using System.Reflection;
using PopcatClient;
using PopcatClient.Updater;

// Version information
[assembly: AssemblyInformationalVersion(AssemblyData.InformationalVersion)]
[assembly: AssemblyVersion("1.0.0.0")]

// Metadata
[assembly: AssemblyTitle("Popcat Client")]
[assembly: AssemblyDescription("A command line tool to send pops to popcat automatically.")]
[assembly: AssemblyCopyright("ShingZhanho (c) 2021  All Rights Reserved")]


namespace PopcatClient
{
    internal static class AssemblyData
    {
        public const string InformationalVersion = "v0.1-beta.3";
        public static readonly VersionName InformationalVersionName = InformationalVersion;
    }
}