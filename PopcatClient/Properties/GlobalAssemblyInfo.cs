using System.Reflection;
using System.Runtime.InteropServices;
using PopcatClient;

[assembly: AssemblyInformationalVersion(AssemblyData.InformationalVersion)]

namespace PopcatClient
{
    internal static class AssemblyData
    {
        public const string InformationalVersion = "v0.1-beta.3";
        public static readonly VersionName VersionName = InformationalVersion;
    }
}