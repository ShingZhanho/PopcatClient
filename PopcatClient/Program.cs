﻿using System;
using System.Collections.Generic;
using System.Net.Http;

namespace PopcatClient
{
    public static class Program
    {
        public static CommandLineOptions Options { get; private set; }= CommandLineOptions.DefaultCommandLineOptions;

        public static void Main(string[] args)
        {
            Options = new CommandLineOptions(args);
            
            CommandLine.WriteWarningVerbose("Verbose mode is enabled.");
            CommandLine.WriteWarningDebug("Debug mode is enabled.");
            
            ShowStartOptionsVerbose(Options);

            var popClient = new PopcatClient(Options);
            popClient.Run();
            Console.Read();
            popClient.Dispose();
        }

        private static void ShowStartOptionsVerbose(CommandLineOptions options)
        {
            CommandLine.WriteMessageVerbose("PopcatClient started with following settings:");
            CommandLine.WriteMessageVerbose($"Waiting time: {options.WaitTime}ms");
            CommandLine.WriteMessageVerbose($"Max sequential failures: {options.MaxFailures}");
            CommandLine.WriteMessageVerbose($"Initial pops: {Options.InitialPops}");
        }
    }
}