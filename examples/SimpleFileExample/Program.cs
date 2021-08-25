using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using J4JSoftware.Logging;
using Serilog;

namespace J4JLogger.Examples
{
    // shows how to use J4JLogger without a configuration file
    class Program
    {
        static void Main(string[] args)
        {
            var loggerConfig = new J4JLoggerConfiguration()
                {
                    CallingContextToText = ConvertCallingContextToText
                }
                .AddEnricher<CallingContextEnricher>();

            loggerConfig.SerilogConfiguration
                .WriteTo.Debug( outputTemplate: J4JLoggerConfiguration.GetOutputTemplate( true ) )
                .WriteTo.Console( outputTemplate: J4JLoggerConfiguration.GetOutputTemplate( true ) )
                .WriteTo.File(
                    path: Path.Combine( Environment.CurrentDirectory, "log.txt" ),
                    outputTemplate: J4JLoggerConfiguration.GetOutputTemplate( true ),
                    rollingInterval: RollingInterval.Day );

            var logger = loggerConfig.CreateLogger();
            logger.SetLoggedType( typeof(Program) );

            logger.Information("This is an Informational logging message");
            logger.Fatal("This is a Fatal logging message");
        }

        private static string ConvertCallingContextToText( 
            Type? loggedType, 
            string callerName, 
            int lineNum,
            string srcFilePath )
        {
            return CallingContextEnricher.DefaultConvertToText( loggedType,
                callerName,
                lineNum,
                CallingContextEnricher.RemoveProjectPath( srcFilePath, GetProjectPath() ) );
        }

        private static string GetProjectPath( [ CallerFilePath ] string filePath = "" )
        {
            var dirInfo = new DirectoryInfo( Path.GetDirectoryName( filePath )! );
 
            while( dirInfo.Parent != null )
            {
                if( dirInfo.EnumerateFiles("*.csproj").Any())
                    break;

                dirInfo = dirInfo.Parent;
            }

            return dirInfo.FullName;
        }
    }
}
