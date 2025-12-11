using System;
using System.IO;

namespace WG2.Logging;

/// <summary>
/// Loggee, that logs info into Logs folder
/// </summary>
public class FileLoggee : ILoggee
{
    private const string ErrorPrefix = "[ERROR]: ";
    private const string WarningPrefix = "[WARNING]: ";
    private const string InfoPrefix = "[INFO]: ";
    private const string SuccessPrefix = "[SUCCESS]: ";
    private const string DebugPrefix = "[DEBUG]: ";

    private readonly string LogsPath = Path.Combine(Environment.CurrentDirectory, "./", "Logs");
    private readonly string SturtUpTime;

    public FileLoggee()
    {
        SturtUpTime = DateTime.Now.ToString().Replace(':', '-');

    }

    public void Log(string message, LogType logType)
    {
        switch(logType)
        {
            case LogType.Error:
                FileLog(message, ErrorPrefix);
                break;

            case LogType.Warning:
                FileLog(message, WarningPrefix);
                break;

            case LogType.Success:
                FileLog(message, SuccessPrefix);
                break;

            case LogType.Info:
                FileLog(message, InfoPrefix);
                break;

            case LogType.Debug:
                FileLog(message, DebugPrefix);
                break;

            // stupid c# hasn't final switch statement, bruh
            default:
                throw new LogException($"ConsoleLoggee got unknown log type {Enum.GetName(logType)}!");
        }
    }

    private void FileLog(string message, string prefix)
    {
        if(!Directory.Exists(LogsPath)) Directory.CreateDirectory(LogsPath);

        File.AppendAllText(Path.Combine(LogsPath, $"{SturtUpTime}.log"), $"{prefix}{DateTime.Now}: {message}\n");
    }
}
