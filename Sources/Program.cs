
using WG2.Logging;

namespace WG2
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Logger.AddLoggee(new ConsoleLoggee());
            Logger.AddLoggee(new FileLoggee());

            WordGenerator.Run();
        }
    }
}
