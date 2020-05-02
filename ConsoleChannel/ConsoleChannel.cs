using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{

    [Channel("Console")]
    public class ConsoleChannel : LogChannel
    {
        public ConsoleChannel()
        {
        }

        public ConsoleChannel( IConfigurationRoot configRoot, string loggerSection = "Logger")
            : base( configRoot, loggerSection )
        {
        }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.Console( restrictedToMinimumLevel: MinimumLevel );
        }
    }
}