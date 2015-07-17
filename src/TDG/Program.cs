using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TestDataGenerator.Core;
using TestDataGenerator.Core.Exceptions;
using TestDataGenerator.Core.Generators;

namespace gk.DataGenerator.tdg
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cla = new CommandLineArgs();
            Stopwatch sw = sw = new Stopwatch();
            sw.Start();

#if DEBUG
            Debugger.Launch();
#endif
            try
            {
                var result = CommandLine.Parser.Default.ParseArguments(args, cla);
                if (!result)
                {
                    Console.WriteLine("Parse failed!  Use --help flag for instructions on usage.");
                    return;
                }

                if (cla.ShowPatternHelp)
                {
                    Console.Write(cla.GetPatternUsage());
                    return;
                }

                if(cla.Verbose)Console.WriteLine("{0} - ");

                if (cla.ListNamedPatterns)
                {
                    var paths = new List<string>();
                    paths.Add("default");

                    if (!string.IsNullOrEmpty(cla.NamedPatterns)) cla.NamedPatterns.Split(';').ToList().ForEach(paths.Add);

                    Console.WriteLine("Named Parameters:");
                    foreach (var file in paths)
                    {
                        var correctedPath = FileReader.GetPatternFilePath(file);
                        var namedParameters = FileReader.LoadNamedPatterns(correctedPath);
                        foreach (var namedParameter in namedParameters.Patterns)
                        {
                            Console.WriteLine(namedParameter.Name);   
                        }
                    }
                }
                else
                {
                    var template = GetTemplateValue(cla);

                    if (!string.IsNullOrEmpty(template) && !string.IsNullOrEmpty(cla.OutputFilePath)) // output path provided.
                    {
                        OutputToFile(cla, template);
                    }
                    else if (!string.IsNullOrEmpty(template))
                    {
                        OutputToConsole(cla, template);
                    }
                    else
                    {
                        Console.WriteLine(cla.GetUsage());
                    }
                }

                if (cla.Verbose)
                {
                    if (sw != null)
                    {
                        sw.Stop();
                        Console.WriteLine("Generation took {0} milliseconds", sw.ElapsedMilliseconds);
                    }
                }
            }
            catch (GenerationException gex)
            {
                Console.WriteLine("Error:\n{0}", gex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:{0}\n\nStackTrace:{1}",ex.Message, ex.StackTrace);
            }
        }

        private static string GetTemplateValue(CommandLineArgs cla)
        {
            string template="";
            if (!string.IsNullOrEmpty(cla.Template)) // template provided -- no header skipping required
            {
                template = cla.Template;
                if (cla.Verbose) Console.WriteLine("Provided template was '" + template + "'");
            }
            if (!string.IsNullOrEmpty(cla.Pattern)) // template provided -- no header skipping required
            {
                template = cla.Pattern;
                if (cla.Verbose) Console.WriteLine("Provided pattern was '" + template + "'");
            }
            if (!string.IsNullOrEmpty(cla.InputFilePath)) // input file provided
            {
                template = File.ReadAllText(cla.InputFilePath);
                if (cla.Verbose) Console.WriteLine("Provided template was '" + template + "'");
            }
            return template;
        }

        private static void OutputToConsole(CommandLineArgs cla, string template)
        {
            Func<string, GenerationConfig, string> generateFrom = AlphaNumericGenerator.GenerateFromTemplate;
            if (!string.IsNullOrEmpty(cla.Pattern))
            {
                generateFrom = AlphaNumericGenerator.GenerateFromPattern;
            }

            GenerationConfig config = null;
            if (cla.Seed.HasValue || !string.IsNullOrEmpty(cla.NamedPatterns))
            {
                config = new GenerationConfig();
                if (cla.Seed.HasValue) config.Seed = cla.Seed;
                if (!string.IsNullOrEmpty(cla.NamedPatterns)) cla.NamedPatterns.Split(';').ToList().ForEach(config.PatternFiles.Add);
            }

            int ct = 0;
            while (ct < cla.Count)
            {
                var item = generateFrom(template, config);
                Console.WriteLine(item);
                ct++;
            }
        }

        private static void OutputToFile(CommandLineArgs cla, string template)
        {
            Func<string, GenerationConfig, string> generateFrom = AlphaNumericGenerator.GenerateFromTemplate;
            if (!string.IsNullOrEmpty(cla.Pattern))
            {
                generateFrom = AlphaNumericGenerator.GenerateFromPattern;
            }

            GenerationConfig config = null;
            if (cla.Seed.HasValue || !string.IsNullOrEmpty(cla.NamedPatterns))
            {
                if (cla.Seed.HasValue)
                {
                    if (config == null) config = new GenerationConfig();
                    config.Seed = cla.Seed;
                }
                if (!string.IsNullOrEmpty(cla.NamedPatterns))
                {
                    if (config == null) config = new GenerationConfig();
                    cla.NamedPatterns.Split(';').ToList().ForEach(config.PatternFiles.Add);
                }
            }

            using (var fs = new StreamWriter(cla.OutputFilePath))
            {
                int ct = 0;
                while (ct < cla.Count)
                {
                    var item = generateFrom(template, config);
                    fs.WriteLine(item);
                    ct++;
                }
            }
        }

    }
}
