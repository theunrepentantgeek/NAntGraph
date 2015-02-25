using System;
using NDesk.Options;

namespace NAntGraph2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var driver = new Driver();
            var showHelp = false;
            var parser
                = new OptionSet
                      {
                          { "out=", driver.SetImageFile },
                          { "help|?", v => showHelp = true },
                          { "buildFile=|b=", driver.AddBuildFile },
                          { "descriptions", v => driver.ShowDescriptions(true) },
                          { "dotscript:", driver.SetDotFile },
                          { "font:", driver.SetFont},
                          { "fontsize:", driver.SetFontSize }
                      };

            var extras = parser.Parse(args);
            foreach (var f in extras)
            {
                driver.AddBuildFile(f);
            }

            if (!showHelp)
            {
                try
                {
                    driver.Generate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Generation failed:");
                    do
                    {
                        Console.WriteLine(ex.Message);
                        ex = ex.InnerException;                        
                    } while (ex != null);

                    showHelp = true;
                }
            }

            if (showHelp)
            {
                parser.WriteOptionDescriptions(Console.Out);
            }
        }
    }
}
