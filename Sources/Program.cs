using CAI;
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
        private const string SourceFilesFolder = "Sources";

        private static bool DoesHaveAdminRights()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.Unicode;
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            TokenizerSettings tokenizerSettings = new TokenizerSettings();
            tokenizerSettings.MinimalTokenSize = 3;
            tokenizerSettings.MaximalTokenSize = 3;

            GeneratorSettings generatorSettings = new GeneratorSettings();
            generatorSettings.TokensGenerateCount = 200;
            generatorSettings.RandomNextTokenChance = 0;

            ITokenizer tokenizer = new SDTokenizer();
            IGenerator generator = new SGenerator();

            bool logDebugInfo = false;
            int funRecreationsCount = 0;
            string sourcesDirectory = Path.Combine(Directory.GetCurrentDirectory(), SourceFilesFolder);

            AppInterface wg2Interface = new("Word Generator 2", isCatchExceptions: true);
            #region Commands
            wg2Interface.AddCommand(new Command<string>(
                "run",
                "process input and generate result.",
                (input) =>
                {
                    string tokenizerInput = input;
                    var path = Path.Combine(sourcesDirectory, input);
                    if(File.Exists(path))
                    {
                        tokenizerInput = File.ReadAllText(path);
                    }
                    if(logDebugInfo)
                    {
                        Logger.LogDebug($"Tokenizer: {tokenizer}, Generator: {generator}\n");
                    }
                    var tokens = tokenizer.Tokenize(tokenizerSettings, tokenizerInput);

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

                    if(generatorSettings.LogDebugInfo) Logger.LogMessage($"{result}\n\n{result.Replace("|", "")}");
                    else Logger.LogMessage(result);
                },
                "\"run [path/text]\""));
            wg2Interface.AddCommand(new Command<int>
                (
                "ts",
                "set token size (same value for each min and max)",
                (value) =>
                {
                    tokenizerSettings.MinimalTokenSize = value;
                    tokenizerSettings.MaximalTokenSize = value;
                },
                "\"ts [number]\""));
            wg2Interface.AddCommand(new Command<int>
                (
                "tmin",
                "set minimal token size",
                (value) =>
                {
                    tokenizerSettings.MinimalTokenSize = value;
                },
                "\"tmin [number]\""));
            wg2Interface.AddCommand(new Command<int>
                (
                "tmax",
                "set maximal token size",
                (value) =>
                {
                    tokenizerSettings.MaximalTokenSize = value;
                },
                "\"tmax [number]\""));
            wg2Interface.AddCommand(new Command<int>
                (
                "tg",
                "set how many tokens generate",
                (value) =>
                {
                    generatorSettings.TokensGenerateCount = value;
                },
                "\"tg [number]\""));
            wg2Interface.AddCommand(new Command<double>
                (
                "tr",
                "set random token chance",
                (value) =>
                {
                    generatorSettings.RandomNextTokenChance = value;
                },
                "\"tr [number in range 0..1]\""));
            wg2Interface.AddCommand(new Command<int>
                (
                "fr",
                "set funny recreations count",
                (value) =>
                {
                    funRecreationsCount = value;
                },
                "\"fr [number]\""));
            wg2Interface.AddCommand(new Command<int>
                (
                "cap",
                "set capacity of result graph (bigger values can improve perfomance, when process big files",
                (value) =>
                {
                    tokenizerSettings.ResultCapacity = value;
                },
                "\"fr [number]\""));
            wg2Interface.AddCommand(new Command
                (
                "hp",
                "use high process priority",
                () =>
                {
                    if(!DoesHaveAdminRights())
                    {
                        Logger.LogError("Run wg2 with admin rights to use hp argument!");
                        return;
                    }
                    Process thisProcess = Process.GetCurrentProcess();
                    thisProcess.PriorityClass = ProcessPriorityClass.High;
                },
                "\"hp\""));
            wg2Interface.AddCommand(new Command
                (
                "np",
                "use normal process priority",
                () =>
                {
                    Process thisProcess = Process.GetCurrentProcess();
                    thisProcess.PriorityClass = ProcessPriorityClass.Normal;
                },
                "\"hp\""));
            wg2Interface.AddCommand(new Command<bool>
                (
                "debinf",
                "write debug info",
                (isWriteDebugInfo) =>
                {
                    generatorSettings.LogDebugInfo = isWriteDebugInfo;
                    tokenizerSettings.LogDebugInfo = isWriteDebugInfo;
                    logDebugInfo = isWriteDebugInfo;
                },
                "\"debinf [true/false]\""));
            wg2Interface.AddCommand(new Command<string>
                (
                "separate",
                "separate tokens by another type (size - fixed token size; space - every token is a word; " +
                "rand - random token size (between tmin and tmax)",
                (separateType) =>
                {
                    switch(separateType)
                    {
                        case "space":
                            tokenizer = new SSTokenizer();
                            break;
                        case "rand":
                            tokenizer = new RDTokenizer();
                            break;
                        case "size": //size separating is default
                        default:
                            tokenizer = new SDTokenizer();
                            break;
                    }
                },
                "\"separate [size/space/rand]\""));
            #endregion
            wg2Interface.Start();
        }
    }
}
