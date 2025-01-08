using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using WG2.Generation;
using WG2.Tokenization;
namespace WG2
{
    public class Program
    {
        private static class Arguments
        {
	        public const string ReadFileName = "source=";
            public const string TokenSize = "ts=";
            public const string TokenMinimalSize = "tmis=";
            public const string TokenMaximalSize = "tmas=";
            public const string SubsequentTokensCount = "tsc=";
            public const string TokensGenerate = "tg=";
            public const string TokensNext = "tn=";
            public const string TokensRandomChance = "tr=";
            public const string TokenSpaceSeparated = "tsp";
            public const string FunRecreationsCount = "fr=";
            public const string UseHighestPriority = "hp";
            public const string DebugInfo = "debinf";
        }

        private const string SourceFilesFolder = "Sources";

        private static bool DoesHaveAdminRights()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static string GetArgumentParameter(string argName, string arg)
        {
            return arg.Substring(argName.Length, arg.Length - argName.Length);
        }

        private static void Main(string[] args)
        {
            TokenizerSettings tokenizerSettings = new TokenizerSettings();
            tokenizerSettings.SubsequentTokensCount = 1;
            tokenizerSettings.MinimalTokenSize = 3;
            tokenizerSettings.MaximalTokenSize = 3;

            GeneratorSettings generatorSettings = new GeneratorSettings();
            generatorSettings.TokensGenerateCount = 200;
            generatorSettings.NextTokensCount = 10;
            generatorSettings.RandomNextTokenChance = 0;
            generatorSettings.SubsequentTokensCount = 1;

            ITokenizer tokenizer = new SDTokenizer();
            IGenerator generator = new SGenerator();

            bool logDebugInfo = false;
            bool isShallReadFile = false;
            string currentDirectory = Directory.GetCurrentDirectory();
            string filePath = null;
            int funRecreationsCount = 0;
            #region ParseArgs
            foreach(string arg in args)
	        {
                if(arg.StartsWith(Arguments.ReadFileName))
                {
                    isShallReadFile = true;
                    filePath = Path.Combine(currentDirectory, SourceFilesFolder, GetArgumentParameter(Arguments.ReadFileName, arg));
                }

                else if(arg.StartsWith(Arguments.TokenSize))
                {
                    tokenizerSettings.MinimalTokenSize = int.Parse(GetArgumentParameter(Arguments.TokenSize, arg));
                    tokenizerSettings.MaximalTokenSize = int.Parse(GetArgumentParameter(Arguments.TokenSize, arg));
                }
                else if(arg.StartsWith(Arguments.TokenMinimalSize))
                {
                    tokenizerSettings.MinimalTokenSize = int.Parse(GetArgumentParameter(Arguments.TokenSize, arg));
                }
                else if(arg.StartsWith(Arguments.TokenMaximalSize))
                {
                    tokenizerSettings.MaximalTokenSize = int.Parse(GetArgumentParameter(Arguments.TokenSize, arg));
                }
                else if(arg.StartsWith(Arguments.SubsequentTokensCount))
                {
                    tokenizerSettings.SubsequentTokensCount = int.Parse(GetArgumentParameter(Arguments.SubsequentTokensCount, arg));
                    generatorSettings.SubsequentTokensCount = int.Parse(GetArgumentParameter(Arguments.SubsequentTokensCount, arg));
                }
                else if(arg.StartsWith(Arguments.TokensGenerate))
                {
                    generatorSettings.TokensGenerateCount = int.Parse(GetArgumentParameter(Arguments.TokensGenerate, arg)); ;
                }
                else if(arg.StartsWith(Arguments.TokensNext))
                {
                    generatorSettings.NextTokensCount = int.Parse(GetArgumentParameter(Arguments.TokensNext, arg));
                }
                else if(arg.StartsWith(Arguments.TokensRandomChance))
                {
                    generatorSettings.RandomNextTokenChance = byte.Parse(GetArgumentParameter(Arguments.TokensRandomChance, arg));
                }
                else if(arg.StartsWith(Arguments.FunRecreationsCount))
                {
                    funRecreationsCount = int.Parse(GetArgumentParameter(Arguments.FunRecreationsCount, arg));
                }
                else if(arg.StartsWith(Arguments.UseHighestPriority))
                {   
                    if(!DoesHaveAdminRights())
                    {
                        Logger.LogError("Run wg2 with admin rights to use hp argument!");
                        return;
                    }
                    Process thisProcess = Process.GetCurrentProcess();
                    thisProcess.PriorityClass = ProcessPriorityClass.High;
                }
                else if(arg.StartsWith(Arguments.DebugInfo))
                {
                    generatorSettings.LogDebugInfo = true;
                    tokenizerSettings.LogDebugInfo = true;
                    logDebugInfo = true;
                }
                else if(arg.StartsWith(Arguments.TokenSpaceSeparated))
                {
                    tokenizer = new SSTokenizer();
                }
                else
                {
                    Logger.LogWarning($"wg2 does not contains parameter \"{arg}\", is this a typo?");
                }
            }
            #endregion
            
            if(logDebugInfo)
            {
                Logger.LogDebug($"Tokenizer: {tokenizer}, Generator: {generator}\n");
            }

            string input = isShallReadFile ? File.ReadAllText(filePath) : Console.ReadLine();

            var tokens= tokenizer.Tokenize(tokenizerSettings, input);


            string result = "";

            if(funRecreationsCount > 0)
            {
                for(int i = 0; i < funRecreationsCount; i++)
                {

                    result = generator.Generate(generatorSettings, tokens);
                    tokens = tokenizer.Tokenize(tokenizerSettings, result);
                }
            }
            else
            { 
                result = generator.Generate(generatorSettings, tokens);
            }
            Logger.LogMessage(result);
            Console.ReadLine();
        }
    }
}
