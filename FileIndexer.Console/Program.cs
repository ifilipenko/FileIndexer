using System;
using System.IO;
using System.Linq;
using FileIndexer.ConsoleHelpers;
using FileIndexer.Generator;

namespace FileIndexer.Console
{
    class Program
    {
        private static readonly string[] HelpCommands = new[] {"-h", "-help", "?"};

        static void Main(string[] args)
        {
            try
            {
                var parameters = ParseParameters(args);
                if (parameters.IsHelpMode)
                {
                    PrintHelp();
                    return;
                }
                if (parameters.IsGeneratorMode)
                {
                    var fileGenerator = new FileGenerator(parameters.GeneratorMethod);
                    fileGenerator.GenerateFile(parameters.FilePath);
                    return;
                }

                var lineIndex     = IndexLoader.GetLineIndexForFile(parameters.FilePath);
                var stringSource  = new FileSource(parameters.FilePath, FixedParameters.Encoding);
                var commandParser = new CommandParser(new LocalIndexCommandFactory(lineIndex, stringSource));

                System.Console.WriteLine("Type command: ");
                while (true)
                {
                    try
                    {
                        var commandText = System.Console.ReadLine();
                        var command = commandParser.ParseCommandText(commandText);
                        if (command != null)
                        {
                            command.Execute();
                        }
                    }
                    catch (WrongCommandOrParametersException ex)
                    {
                        Print.PrintExceptionMessage(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                PrintUnhandeledException(ex);
            }
        }

        private static void PrintHelp()
        {
            const string help =
@" ---------------- Main function -------------------- 
path to file        this command open specified file 
                    for query lines and words    

 ---------------- Test file generator ---------------- 
-generate-test-file mode path    this command generate test file
    mode     this parameter have folowing values:
        BigFileWithOneBigWord  25 Gb file with one big line and 
                               one big word without whitespaces
        BigFileWithOneLine     25 Gb file with one big line and 
                               many random words (limit of word len is 10 Gb)
        BigFileWithManyLine    25 Gb file with many lines and 
                               many random words (limit of word len is 10 Gb)
        EmptyFile              file with empty text
        ManyLinesWithoutWords  file with 100 lines with 
                            whiltespaces only
    path      path to generated file";
            System.Console.WriteLine(help);
        }

        private static Parameters ParseParameters(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    if (HelpCommands.Any(x => x.Equals(args[0], StringComparison.OrdinalIgnoreCase)))
                    {
                        return new Parameters {IsHelpMode = true};
                    }

                    var filePath = args[0];
                    if (!File.Exists(filePath))
                        throw new ArgumentException("Given file is not exists", "args");
                    return new Parameters {FilePath = filePath};
                case 3:
                    switch (args[0])
                    {
                        case "-generate-test-file":
                            var parameters = new Parameters();
                            parameters.SetGeneratorParameters(args[1], args[2]);
                            return parameters;
                    }
                    break;
            }
            throw new ArgumentException("Unexpected or incorrect parameters ", "args");
        }

        public static void PrintUnhandeledException(Exception exception)
        {
            Print.PrintErrorMessage(exception.ToString());

            System.Console.WriteLine("Use parameter key {0} for help.", string.Join(", ", HelpCommands));
        }
    }
}