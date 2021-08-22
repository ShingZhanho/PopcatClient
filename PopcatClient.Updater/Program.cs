using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PopcatClient.Updater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // arguments: <new version dir> <current version dir> <current version PID> <current process commandline arguments (Base64)>
            if (args.Length < 4) Environment.Exit(6);

            if (!int.TryParse(args[2], out var currentVersionPid)) Environment.Exit(1);
            if (currentVersionPid == 0) Environment.Exit(1);
            
            var newVersionDir = args[0];
            if (!Directory.Exists(newVersionDir)) Environment.Exit(2);
            
            var currentVersionDir = args[1];
            if (!Directory.Exists(currentVersionDir)) Environment.Exit(3);

            var commandlineArgs = args[3];

            try
            {
                var appProc = Process.GetProcesses().Any(proc => proc.Id == currentVersionPid)
                    ? Process.GetProcessById(currentVersionPid)
                    : null;
                appProc?.Kill();
                
                foreach (var file in Directory.GetFiles(currentVersionDir)) File.Delete(file);
                foreach (var directory in Directory.GetDirectories(currentVersionDir))
                    Directory.Delete(directory, true);
                
                CopyDirRecursively(newVersionDir, currentVersionDir);

                new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine(currentVersionDir, "PopcatClient.exe"),
                        Arguments = Encoding.UTF8.GetString(Convert.FromBase64String(commandlineArgs)) + " --clear-temp"
                    }
                }.Start();
            }
            catch
            {
                Environment.Exit(4);
            }
        }

        private static void CopyDirRecursively(string fromDir, string toDir)
        {
            try {
                // dirs
                foreach (var directory in Directory.GetDirectories(fromDir, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(Path.Combine(toDir, directory[(fromDir.Length + 1)..]));

                // files
                foreach (var file in Directory.GetFiles(fromDir, "*", SearchOption.AllDirectories))
                    File.Copy(file, Path.Combine(toDir, file[(fromDir.Length + 1)..]));
            } catch (Exception e) {
                throw new InvalidOperationException("Error occured while copying files.", e);
            }
        }
    }
}