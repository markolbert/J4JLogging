using System;
using System.IO;
using J4JSoftware.Logging;
using Serilog;

namespace J4JLogger.Examples
{
    // shows how to use J4JLogger without a configuration file
    class Program
    {
        static void Main(string[] args)
        {
            var seriLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Debug( outputTemplate: J4JLoggerConfiguration.GetOutputTemplate( true ) )
                .WriteTo.Console( outputTemplate: J4JLoggerConfiguration.GetOutputTemplate( true ) )
                .WriteTo.File(
                    path: Path.Combine( Environment.CurrentDirectory, "log.txt" ),
                    outputTemplate: J4JLoggerConfiguration.GetOutputTemplate( true ),
                    rollingInterval: RollingInterval.Day )
                .CreateLogger();

            var logger = new J4JSoftware.Logging.J4JLogger( seriLogger );
            logger.SetLoggedType( typeof(Program) );

            logger.Information("This is an Informational logging message");
            logger.Fatal("This is a Fatal logging message");
        }
    }
}
