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

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig, string? outputTemplate = null )
        {
            return string.IsNullOrEmpty( outputTemplate )
                ? sinkConfig.Console( restrictedToMinimumLevel : MinimumLevel )
                : sinkConfig.Console( restrictedToMinimumLevel : MinimumLevel, outputTemplate : outputTemplate );
        }
    }
}