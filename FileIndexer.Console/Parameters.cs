using System;
using FileIndexer.Generator;

namespace FileIndexer.Console
{
    public class Parameters
    {
        public string FilePath { get; set; }
        public bool IsGeneratorMode { get; set; }
        public GeneratorMethod GeneratorMethod { get; private set; }
        public bool IsHelpMode { get; set; }

        public void SetGeneratorParameters(string mode, string generatedFileName)
        {
            IsGeneratorMode = true;
            FilePath = generatedFileName;
            GeneratorMethod = (GeneratorMethod) Enum.Parse(typeof (GeneratorMethod), mode, true);
        }
    }
}