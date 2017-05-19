using JustAssembly.API.Analytics;
using JustAssembly.Core;
using JustAssembly.Infrastructure.Analytics;
using System;
using System.IO;

namespace JustAssembly.CommandLineTool
{
    public class Startup
    {
        private static IAnalyticsService analytics;

        static void Main(string[] args)
        {
            analytics = AnalyticsServiceImporter.Instance.Import();

            analytics.Start();
            analytics.TrackFeature("Mode.CommandLine");

            try
            {
                RunMain(args);
            }
            catch (Exception ex)
            {
                analytics.TrackException(ex);
                analytics.Stop();

                throw;
            }

            analytics.Stop();
        }

        private static void RunMain(string[] args)
        {
            if (args.Length != 3)
            {
                WriteErrorAndSetErrorCode("Wrong number of arguments." + Environment.NewLine + Environment.NewLine + "Sample:" + Environment.NewLine + "justassembly.commandlinetoool Path\\To\\Assembly1 Path\\To\\Assembly2 Path\\To\\XMLOutput");
                return;
            }

            if (!FilePathValidater.ValidateInputFile(args[0]))
            {
                WriteErrorAndSetErrorCode("First assembly path is in incorrect format or file not found.");
                return;
            }

            if (!FilePathValidater.ValidateInputFile(args[1]))
            {
                WriteErrorAndSetErrorCode("Second assembly path is in incorrect format or file not found.");
                return;
            }

            if (!FilePathValidater.ValidateOutputFile(args[2]))
            {
                WriteErrorAndSetErrorCode("Output file path is in incorrect format.");
                return;
            }

            string xml = string.Empty;
            try
            {
                IDiffItem diffItem = APIDiffHelper.GetAPIDifferences(args[0], args[1]);
                if (diffItem != null)
                {
                    xml = diffItem.ToXml();
                }
            }
            catch (Exception ex)
            {
                WriteExceptionAndSetErrorCode("A problem occurred while creating the API diff.", ex);
                return;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(args[2]))
                {
                    writer.Write(xml);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionAndSetErrorCode("There was a problem while writing output file.", ex);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("API differences calculated successfully.");
            Console.ResetColor();
        }

        private static void WriteErrorAndSetErrorCode(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            Environment.ExitCode = 1;
        }

        private static void WriteExceptionAndSetErrorCode(string message, Exception ex)
        {
            analytics.TrackException(ex);

            WriteErrorAndSetErrorCode(string.Format("{0}{1}{2}", message, Environment.NewLine, ex));
        }
    }
}
