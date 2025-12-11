
using Spectre.Console;
using System;

namespace WG2.Logging;

/// <summary>
/// Loggee that logs info to <see cref="AnsiConsole"/>
/// </summary>
public class ConsoleLoggee : ILoggee
{
    private const string ErrorColor = "red";
    private const string WarningColor = "orange3";
    private const string InfoColor = "slateblue3";
    private const string SuccessColor = "green";
    private const string DebugColor = "mediumvioletred";

    public void Log(string message, LogType logType)
    {
        switch(logType)
        {
            case LogType.None:

            case LogType.Error:
                AnsiLog(message, ErrorColor);
                break;

            case LogType.Warning:
                AnsiLog(message, WarningColor);
                break;

            case LogType.Success:
                AnsiLog(message, SuccessColor);
                break;

            case LogType.Info:
                AnsiLog(message, InfoColor);
                break;

            case LogType.Debug:
                AnsiLog(message, DebugColor);
                break;

            // stupid c# hasn't final switch statement, bruh
            default:
                throw new LogException($"ConsoleLoggee got unknown log type {Enum.GetName(logType)}!");
        }
    }

    private void AnsiLog(string message, string color)
    {
        AnsiConsole.MarkupLine($"[{color}]{DateTime.Now}: {Markup.Escape(message)}[/]");
    }
}
