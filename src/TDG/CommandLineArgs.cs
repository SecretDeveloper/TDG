﻿using System.Text;
using CliParse;


namespace gk.DataGenerator.tdg
{
    [ParsableClass("TDG - Test Data Generator"
        , "A command line tool that can be used to generate data for testing or other uses. You provide it with a pattern containing symbols defining the output you want to produce and it will create random data to match that pattern."
        , Copyright = "2016 Gary Kenneally (@SecretDeveloper)"
        , FooterText = "Either a template (-t), pattern (-p) or input file (-i) value must be provided as input.")]
    internal class CommandLineArgs : Parsable
    {
        [ParsableArgument("template", ShortName = 't', DefaultValue = "", Description = "The template containing 1 or more patterns to use when producing data.")]
        public string Template { get; set; }

        [ParsableArgument("pattern", ShortName = 'p', ImpliedPosition = 0, DefaultValue = "", Description = "The pattern to use when producing data.")]
        public string Pattern { get; set; }

        [ParsableArgument("detailed", ShortName = 'd', DefaultValue = false, Description = "Show help text for pattern symbols.")]
        public bool ShowPatternHelp { get; set; }

        [ParsableArgument("inputfile", ShortName = 'i', DefaultValue = "", Description = "The path of the input file.", Required = false)]
        public string InputFilePath { get; set; }

        //[Option('headers', "headers", DefaultValue = 0, HelpText = "The line to start reading from, 0 for start of file. Can be used to skip header rows in files.", Required = false, MutuallyExclusiveSet = "fromFile")]
        public int HeaderCount { get; set; }

        [ParsableArgument("output", ShortName = 'o', DefaultValue = "", Description = "The path of the output file.", Required = false)]
        public string OutputFilePath { get; set; }

        [ParsableArgument("count", ShortName = 'c', DefaultValue = 1, Description = "The number of items to produce.", Required = false)]
        public int Count { get; set; }

        [ParsableArgument("seed", ShortName = 's', Description = "The seed value for random generation. Default is a random value.", Required = false)]
        public string Seed { get; set; }

        [ParsableArgument("verbose", ShortName = 'v', DefaultValue = false, Description = "Verbose output including debug and performance information.", Required = false)]
        public bool Verbose { get; set; }

        [ParsableArgument("namedpatterns", ShortName = 'n', DefaultValue = "", Description = "A list of ';' seperated file paths containing named patterns to be used.", Required = false)]
        public string NamedPatterns { get; set; }

        [ParsableArgument("listnamedpatterns", ShortName = 'l', DefaultValue = false, Description = "Outputs a list of the named patterns from the default.tdg-patterns file and any additionally provided named patterns (-n).", Required = false)]
        public bool ListNamedPatterns { get; set; }
        
        public string GetUsage()
        {
            //var asm = Assembly.GetExecutingAssembly();
            //return this.GetHelpInfoFromAssembly(asm, argumentTemplate: "  {shortname},  {name}, {required}, Default:'{defaultvalue}'\n       {description}");

            return this.GetHelpInfo(argumentTemplate: "    {shortname} {name} {required}, Default:'{defaultvalue}'\n        {description}");
        }

        public string GetPatternUsage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Detailed Pattern Usage");
            sb.AppendLine();

            sb.AppendLine("The following symbols can be used within a pattern to produce the desired output.");
            sb.AppendLine("\t\\. - A single random character of any type.");
            sb.AppendLine("\t\\a - A single random upper-case or lower-case letter.");
            sb.AppendLine("\t\\W - A single random character from the following list ' .,;:'\"!&?£€$%^<>{}*+-=\\@#|~/'.");
            sb.AppendLine("\t\\w - A single random upper-case, lower-case letter or number.");
            sb.AppendLine("\t\\L - A single random upper-case letter.");
            sb.AppendLine("\t\\l - A single random lower-case letter.");
            sb.AppendLine("\t\\V - A single random upper-case Vowel.");
            sb.AppendLine("\t\\v - A single random lower-case vowel.");
            sb.AppendLine("\t\\C - A single random upper-case consonant.");
            sb.AppendLine("\t\\c - A single random lower-case consonant.");
            sb.AppendLine("\t\\D - A single random non number character.");
            sb.AppendLine("\t\\d - A single random number, 1-9.");
            sb.AppendLine("\t\\S - A single random non-whitespace character.");
            sb.AppendLine("\t\\s - A whitespace character (Tab, New Line, Space, Carriage Return or Form Feed)");
            sb.AppendLine("\t\\n - A newline character.");
            sb.AppendLine("\t\\t - A tab character.");
            
            sb.AppendLine("Patterns usage");
            sb.AppendLine("\ttdg -p '\\L' - Will generate a random upper-case character.");
            sb.AppendLine("\ttdg -p '(\\L){5}' - Will generate 5 random upper-case characters.");
            sb.AppendLine("\ttdg -p '(\\L){10,20}' - Will generate between 10 and 20 random upper-case characters.");
            
            sb.AppendLine("Patterns and normal text can be combined in templates");
            sb.AppendLine("Template usage");
            sb.AppendLine("\ttdg -t 'Text containing a <<\\L>> pattern'");
            sb.AppendLine("\ttdg -t 'Text containing a <<(\\L){5}>> repeating pattern'");
            sb.AppendLine("\ttdg -t 'Text containing a <<(\\L){10,20}>> repeating pattern between 10 and 20 upper-case characters'");
            sb.AppendLine("View the Readme document for further examples");

            return sb.ToString();
        }

    }
}

       