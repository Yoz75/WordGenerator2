namespace WG2.Logging;

public interface ILoggee
{
    public void Log(string message, LogType type);
}
