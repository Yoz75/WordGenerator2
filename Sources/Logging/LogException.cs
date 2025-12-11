
namespace WG2.Logging;


/// <summary>
/// Exception, thrown when thrying to log <see cref="LogType.None"/>
/// </summary>
[System.Serializable]
public class LogException : System.Exception
{
	public LogException() { }
	public LogException(string message) : base(message) { }
	public LogException(string message, System.Exception inner) : base(message, inner) { }
}
