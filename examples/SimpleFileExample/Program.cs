using J4JSoftware.Logging;

namespace J4JLogger.Examples
{
    // shows how to use J4JLogger without a configuration file
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
