using CAI;
using CAI.Reflection;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using WG2.Generation;
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
            Logger.LogDebug($"Tokenizer: {Tokenizer}, Generator: {Generator}\n");
        }
        var tokens = Tokenizer.Tokenize(TokenizerSettings, tokenizerInput);

        string result = "";

        if(WG2Settings.FunRecreationsCount > 0)
        {
            for(int i = 0; i < WG2Settings.FunRecreationsCount; i++)
            {
                result = Generator.Generate(GeneratorSettings, tokens);
                tokens = Tokenizer.Tokenize(TokenizerSettings, result);
            }
        }
        else
        {
            result = Generator.Generate(GeneratorSettings, tokens);
        }

        if(GeneratorSettings.LogDebugInfo)
        {
            Logger.LogMessage($"{result}\n\n{result.Replace("|", "")}");
        }
        else Logger.LogMessage(result);
    }

    [Command(AppInterfaceName, "ts", "set token size (tmin = tmax)", "ts [value]")]
    public static void SetTokenSize(int size)
    {
        TokenizerSettings.MinimalTokenSize = size;
        TokenizerSettings.MaximalTokenSize = size;
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
            Logger.LogError("Run wg2 with admin rights to use hp argument!");
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

    [Command(AppInterfaceName, "separate", "set new text separator strategy (tokenizer).\n" +
        "Available tokenizers:\n" +
        "space -- each word is a token\n" +
        "rand -- random token size between tmin and tmax\n" +
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
            case "size": //size separating is default
            default:
                Tokenizer = new SDTokenizer();
                break;
        }
    }
}
