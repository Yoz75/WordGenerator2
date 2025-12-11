
using Spectre.Console;

namespace WG2;

public enum Color
{
    White,
    Red,
    Green,
    Blue
}

/// <summary>
/// WG2 abstraction of console.
/// Use this when the output is intended to be read by the user
/// </summary>
public static class WGConsole
{
    public static void WriteLine(string message, Color color = Color.White)
    {
        string colorPrefix = color switch
        {
            Color.White => "white",
            Color.Red => "red3",
            Color.Green => "green4",
            Color.Blue => "dodgerblue3",
            _ => throw new System.ArgumentException($"Unknown color type {System.Enum.GetName(color)}!")
        };

        AnsiConsole.MarkupLineInterpolated($"[{colorPrefix}]{message}[/]");
    }

    public static void Write(string message, Color color = Color.White)
    {
        string colorPrefix = color switch
        {
            Color.White => "white",
            Color.Red => "red3",
            Color.Green => "green4",
            Color.Blue => "dodgerblue3",
            _ => throw new System.ArgumentException($"Unknown color type {System.Enum.GetName(color)}!")
        };

        AnsiConsole.MarkupInterpolated($"[{colorPrefix}]{message}[/]");
    }
}
