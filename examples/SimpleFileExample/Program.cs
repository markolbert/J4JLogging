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
            var logger = new J4JSoftware.Logging.J4JLogger();

            logger.LoggerConfiguration
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File( 
                    path: Path.Combine( Environment.CurrentDirectory, "log.txt" ),
                    rollingInterval: RollingInterval.Day );

            logger.ReportCallingMember()
                .ReportLineNumber()
                .ReportLoggedType()
                .ReportSourceCodeFile()
                .Create();

            logger.Information("This is an Informational logging message");
            logger.Fatal("This is a Fatal logging message");
        }
    }
}
