using J4JSoftware.Logging;

namespace J4JLogger.Examples
{
    // shows how to use the J4JLogger with a simple configuration file containing
    // nothing but logger configuration information.
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new J4JSoftware.Logging.J4JLogger();
            logger.AddDebug();
            logger.AddConsole();
            logger.AddFile();

            logger.SetLoggedType<Program>();

            logger.Information("This is an Informational logging message");
            logger.Fatal("This is a Fatal logging message");
        }
    }
}
