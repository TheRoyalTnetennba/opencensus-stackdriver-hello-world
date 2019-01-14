﻿namespace Samples
{
    using CommandLine;
    using System;

    [Verb("stackdriver", HelpText = "Specify the options required to test Stackdriver exporter", Hidden = false)]
    class StackdriverOptions
    {
        [Option('p', "projectId", HelpText = "Please specify the projectId of your GCP project", Required = true)]
        public string ProjectId { get; set; }
    }

    /// <summary>
    /// Main samples entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method - invoke this using command line.
        /// For example:
        /// 
        /// Samples.dll zipkin http://localhost:9411/api/v2/spans
        /// Sample.dll appInsights
        /// Sample.dll prometheus
        /// </summary>
        /// <param name="args">Arguments from command line.</param>
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<StackdriverOptions>(args)
                .MapResult(
                    (StackdriverOptions options) => TestStackdriver.Run(options.ProjectId),
                    errs => 1);

            Console.ReadLine();
        }
    }
}