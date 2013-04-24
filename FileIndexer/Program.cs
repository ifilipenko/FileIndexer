using System;
using System.Diagnostics;
using System.IO;

namespace FileIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parameters    = ProcessParameters(args);
                var lineIndex     = GetLineIndex(parameters);
                var stringSource  = new FileSource(parameters.FilePath);
                var commandParser = new CommandParser();

                while (true)
                {
                    var commandText = Console.ReadLine();
                    var command = commandParser.ParseCommandText(commandText);
                    if (command != null)
                    {
                        command.Execute(lineIndex, stringSource);
                    }
                }
            }
            catch (Exception ex)
            {
                PrintException(ex);
            }
        }

        private static LineIndex GetLineIndex(Parameters parameters)
        {
            var indexCache = new LineIndexJsonFileCache(AppDomain.CurrentDomain.BaseDirectory);
            var indexBuilder = new IndexBuilder();

            Console.WriteLine("Checking index cache...");
            var lineIndex = indexBuilder.LoadFromCache(indexCache, parameters.FilePath);
            if (lineIndex == null)
            {
                Console.WriteLine("Index cache not found. Start indexing...");

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                using (var stream = File.OpenRead(parameters.FilePath))
                {
                    lineIndex = indexBuilder.BuildFromStream(stream, IndexBuilder.DefaultEncoding);
                }
                stopwatch.Stop();

                Console.WriteLine("Indexing complete at {0}.", stopwatch.Elapsed);
                indexCache.Update(lineIndex, parameters.FilePath);
            }
            else
            {
                Console.WriteLine("Index loaded from cache.");
            }
            return lineIndex;
        }

        private static Parameters ProcessParameters(string[] args)
        {
            if (args.Length != 1)
                throw new ArgumentException("Expected 1 parameter but was " + args.Length, "args");

            var filePath = args[0];
            if (!File.Exists(filePath))
                throw new ArgumentException("Given file is not exists", "args");

            return new Parameters(filePath);
        }

        private static void PrintException(Exception exception)
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

            Console.WriteLine("Use parameter key -h, -help or ? for help.");
        }
    }
}