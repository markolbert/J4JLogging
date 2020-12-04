using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    // defines the configuration for a debug channel
    public class DebugConfig : ChannelConfig
    {
        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig ) =>
            sinkConfig.Debug( restrictedToMinimumLevel : MinimumLevel, outputTemplate : EnrichedMessageTemplate );
    }
}