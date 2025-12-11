
using System.Collections.Generic;

namespace WG2.Logging;

/// <summary>
/// The mechanism to tell all loggees "hey, you all need to log this info! 
/// Use this when the output is intended to be read by the developer
/// </summary>
public static class Logger
{
    private static readonly List<ILoggee> Loggees = new();

    public static void AddLoggee(ILoggee loggee)
    {
        Loggees.Add(loggee);
    }

    public static void Log(string message, LogType logType = LogType.Info)
    {
        foreach(var loggee in Loggees)
        {
            loggee.Log(message, logType);
        }
    }
}
