using CAI;
using CAI.Reflection;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using WG2.Generation;
using WG2.Logging;
using WG2.Tokenization;
namespace WG2;

public struct WG2Settings
{
    public bool LogDebugInfo;
    public int FunRecreationsCount;
}

public static class WordGenerator
{
    private const string SourceFilesFolder = "Sources";
    private const string AppInterfaceName = "Word Generator 2";

    private static ITokenizer Tokenizer = new SDTokenizer();
    private static IGenerator Generator = new SGenerator();

    private static TokenizerSettings TokenizerSettings = new();
    private static GeneratorSettings GeneratorSettings = new();
    private static WG2Settings WG2Settings;

    private static string SourcesDirectory =
        Path.Combine(Directory.GetCurrentDirectory(), SourceFilesFolder);

    private static bool DoesHaveAdminRights()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void Run()
    {
        Console.InputEncoding = System.Text.Encoding.Unicode;
        Console.OutputEncoding = System.Text.Encoding.Unicode;

        AppInterface wg2Interface = new("Word Generator 2", isCatchExceptions: true);
        wg2Interface.LoadAttributedCommands();
        wg2Interface.Start();
    }

    /*
     * We HAVE to use public here, compiler could just trim unused private code.
     */

    [Command(AppInterfaceName, "run", "process input and generate result.", "run [pathToFile/text]")]
    public static void RunCommand(string input)
    {
        string tokenizerInput = input;
        var path = Path.Combine(SourcesDirectory, input);
        if(File.Exists(path))
        {
            tokenizerInput = File.ReadAllText(path);
        }
        if(WG2Settings.LogDebugInfo)
        {
            Logger.Log($"Tokenizer: {Tokenizer}, Generator: {Generator}\n", LogType.Debug);
        }
        var tokens = Tokenizer.Tokenize(TokenizerSettings, tokenizerInput);

        string result = "";

        if(WG2Settings.FunRecreationsCount > 0)
        {
            for(int i = 0; i < WG2Settings.FunRecreationsCount; i++)
            {
                result = Generator.Generate(GeneratorSettings, tokens);
                tokens = Tokenizer.Tokenize(TokenizerSettings, result.Replace("|", ""));
            }
        }
        else
        {
            result = Generator.Generate(GeneratorSettings, tokens);
        }

        if(GeneratorSettings.LogDebugInfo)
        {
            Logger.Log(result, LogType.Debug);
            WGConsole.WriteLine(result.Replace("|", ""));
        }
        else WGConsole.WriteLine(result);
    }

    [Command(AppInterfaceName, "ts", "set token size (tmin = tmax)", "ts [value]")]
    public static void SetTokenSize(int size)
    {
        TokenizerSettings.MinimalTokenSize = size;
        TokenizerSettings.MaximalTokenSize = size;
    }

    [Command(AppInterfaceName, "gik", "set generator's top K", "gik [value]")]
    public static void SetGeneratorTopK(int value)
    {
        GeneratorSettings.TopK = value;
    }

    [Command(AppInterfaceName, "tmin", "set minimal token size", "tmin [value]")]
    public static void SetMinTokenSize(int size) => TokenizerSettings.MinimalTokenSize = size;

    [Command(AppInterfaceName, "tmax", "set maximal token size", "tmax [value]")]
    public static void SetMaxTokenSize(int size) => TokenizerSettings.MaximalTokenSize = size;

    [Command(AppInterfaceName, "tg", "set generated tokens count", "tg [value]")]
    public static void SetGeneratedTokensCount(int count) =>
        GeneratorSettings.TokensGenerateCount = count;

    [Command(AppInterfaceName, "tr", "set generated tokens count", "tr [number in range 0..1]")]
    public static void SetRandTokenChance(double chance) =>
        GeneratorSettings.RandomNextTokenChance = chance;

    [Command(AppInterfaceName, "tri", "set iterations of random tokenizer " +
        "(bigger values = more token variants, but slower performance)", "tri [value]")]
    public static void SetRandTokenizerIterations(int count) =>
        TokenizerSettings.RandomIterations = count;

    [Command(AppInterfaceName, "fr", "set funny recreations count " +
        "(generator will generate text from source and then [value] times tokenize generated text," +
        " generate new text and so on)", "fr [value]")]
    public static void SetRecreationsCount(int count) =>
    WG2Settings.FunRecreationsCount = count;

    [Command(AppInterfaceName, "cap", "set capacity of tokenizer's graph. " +
        "Big values can be a huge memory pressure.", "cap [value]")]
    public static void SetGraphCapacity(int capacity) =>
    TokenizerSettings.ResultCapacity = capacity;

    [Command(AppInterfaceName, "hp", "set high process priority", "hp")]
    public static void SepHighPriority()
    {
        if(!DoesHaveAdminRights())
        {
            Logger.Log("Run wg2 with admin rights to use hp argument!", LogType.Error);
            return;
        }
        Process thisProcess = Process.GetCurrentProcess();
        thisProcess.PriorityClass = ProcessPriorityClass.High;
    }

    [Command(AppInterfaceName, "np", "set normal process priority", "np")]
    public static void SepNormalPriority()
    {
        Process thisProcess = Process.GetCurrentProcess();
        thisProcess.PriorityClass = ProcessPriorityClass.Normal;
    }

    [Command(AppInterfaceName, "debinf", "log wg2 debug info?", "debinf [true/false]")]
    public static void LogWG2DebugInfo(bool value)
    {
        WG2Settings.LogDebugInfo = value;
    }

    [Command(AppInterfaceName, "tdebinf", "log tokenizer debug info?", "tdebinf [true/false]")]
    public static void LogTokenizerDebugInfo(bool value)
    {
        TokenizerSettings.LogDebugInfo = value;
    }

    [Command(AppInterfaceName, "gdebinf", "log generator debug info?", "gdebinf [true/false]")]
    public static void LogGeneratorDebugInfo(bool value)
    {
        GeneratorSettings.LogDebugInfo = value;
    }

    [Command(AppInterfaceName, "tis", "set iterative's tokeniser samples count (more = more realistic)",
        "its [value]")]
    public static void SetIterativeSamples(int value)
    {
        TokenizerSettings.ItTokenizerSamples = value;
    }

    [Command(AppInterfaceName, "tik", "set iterative's tokenizer top K " +
        "(probably greater values can break text)", "itk [value]")]
    public static void SetIterativeTopK(int value)
    {
        TokenizerSettings.ItTokenizerTopK = value;
    }

    [Command(AppInterfaceName, "tic", "set iterative's tokenizer minimal count " +
        "(greater values can improve text when tokens look out of place)", "itk [value]")]
    public static void SetIterativeMergeCount(int value)
    {
        TokenizerSettings.ItTokenizerMinMergeCount = value;
    }

    [Command(AppInterfaceName, "separate", "set new text separator strategy (tokenizer).\n" +
        "Available tokenizers:\n" +
        "space -- each word is a token\n" +
        "rand -- random token size between tmin and tmax\n" +
        "it -- iterative tokenizer (doesn't use tmin and ts, only tmax, see tis and tik commands)" +
        "size (default) -- fixed size tokens of tg size.", "separate [space/rand/size]")]
    public static void SetTokenizer(string name)
    {
        switch(name)
        {
            case "space":
                Tokenizer = new SSTokenizer();
                break;
            case "rand":
                Tokenizer = new RDTokenizer();
                break;
            case "it":
                Tokenizer = new ItTokenizer();
                break;
            case "size": //size separating is default
            default:
                Tokenizer = new SDTokenizer();
                break;
        }
    }
}
