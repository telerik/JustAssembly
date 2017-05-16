using JustAssembly.Core;
using System;
using System.IO;

namespace JustAssembly.CommandLineTool
{
    public class Startup
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                WriteErrorAndSetErrorCode("Exactly 3 arguments are needed - 2 file paths for input files and 1 file path for output xml file.");
                return;
            }

            if (!FilePathValidater.ValidateInputFile(args[0]))
            {
                WriteErrorAndSetErrorCode("First file path is in incorrect format or file not found.");
                return;
            }

            if (!FilePathValidater.ValidateInputFile(args[1]))
            {
                WriteErrorAndSetErrorCode("Second file path is in incorrect format or file not found.");
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
                WriteExceptionAndSetErrorCode("There was a problem during calculation of API differences.", ex);
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
            WriteErrorAndSetErrorCode(string.Format("{0}{1}{2}", message, Environment.NewLine, ex));
        }
    }
}
