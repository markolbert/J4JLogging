using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{

    public class ConsoleChannel : LogChannel
    {
        public ConsoleChannel(IJ4JLoggerConfiguration config, ConsoleConfig channelConfig)
            : base(config, channelConfig)
        {
        }

        //public ConsoleChannel(IOptions<J4JLoggerConfiguration> config, IOptions<ConsoleConfig> channelConfig)
        //    : base(config.Value, channelConfig.Value)
        //{
        //}

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return string.IsNullOrEmpty( OutputTemplate )
                ? sinkConfig.Console( restrictedToMinimumLevel : MinimumLevel )
                : sinkConfig.Console( restrictedToMinimumLevel : MinimumLevel,
                    outputTemplate : LoggerConfiguration.EnrichMessageTemplate( OutputTemplate ) );
        }
    }
}