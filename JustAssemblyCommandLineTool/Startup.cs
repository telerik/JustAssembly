using JustAssembly.Core;
using System;
using System.IO;

namespace JustAssemblyCommandLineTool
{
    public class Startup
    {
        static void Main(string[] args)
        {
            string firstArg = args[0];
            string secondArg = args[1];

            string[] partsOfFirstArg = firstArg.Split(new char[] { '.' });
            string firstFileExtension = partsOfFirstArg[partsOfFirstArg.Length - 1];

            if (firstFileExtension == "exe" || firstFileExtension == "dll")
            {
                string[] partsOfSecondArg = secondArg.Split(new char[] { '.' });
                string secondFileExtension = partsOfSecondArg[partsOfSecondArg.Length - 1];
                if (secondFileExtension == "exe" || secondFileExtension == "dll")
                {
                    IDiffItem diffItem = APIDiffHelper.GetAPIDifferences(firstArg, secondArg);
                    string diff = diffItem == null ? string.Empty : diffItem.ToXml();

                    string xmlDiffPath = firstArg + "-" + secondArg + ".xml";
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(xmlDiffPath))
                        {
                            writer.Write(diff);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("There was problem while write to xml");
                        Console.WriteLine(ex.InnerException);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid file extension for second file");
                }
            }
            else
            {
                Console.WriteLine("Invalid file extension of first file");
            }
        }
    }
}
