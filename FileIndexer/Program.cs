﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileIndexer
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

                var lineIndex     = GetLineIndex(parameters);
                var stringSource  = new FileSource(parameters.FilePath, FixedParameters.Encoding);
                var commandParser = new CommandParser();

                Console.WriteLine("Type your command: ");
                while (true)
                {
                    try
                    {
                        var commandText = Console.ReadLine();
                        var command = commandParser.ParseCommandText(commandText);
                        if (command != null)
                        {
                            command.Execute(lineIndex, stringSource);
                        }
                    }
                    catch (WrongCommandOrParametersException ex)
                    {
                        PrintExceptionMessage(ex);
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
            Console.WriteLine(help);
        }

        private static LineIndex GetLineIndex(Parameters parameters)
        {
            var indexCache = new LineIndexJsonFileCache(AppDomain.CurrentDomain.BaseDirectory);
            var indexBuilder = new IndexBuilder(100*Volumes.Megabyte);

            Console.WriteLine("Checking index cache...");
            var lineIndex = indexBuilder.LoadFromCache(indexCache, parameters.FilePath);
            if (lineIndex == null)
            {
                Console.WriteLine("Index cache not found or out of date. Start indexing...");

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                using (var stream = File.OpenRead(parameters.FilePath))
                {
                    lineIndex = indexBuilder.BuildFromStream(stream, FixedParameters.Encoding);
                }
                stopwatch.Stop();

                Console.WriteLine("Indexing complete at {0}.", stopwatch.Elapsed);
                indexCache.Update(lineIndex, parameters.FilePath);
            }
            else
            {
                Console.WriteLine("Index loaded from cache.");
            }
            Console.WriteLine("File consist of {0} lines", lineIndex.Lines.Count());
            return lineIndex;
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

        private static void PrintUnhandeledException(Exception exception)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception);
            }
            finally
            {
                Console.ResetColor();
            }

            Console.WriteLine("Use parameter key {0} for help.", string.Join(", ", HelpCommands));
        }

        private static void PrintExceptionMessage(Exception exception)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.Message);
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}