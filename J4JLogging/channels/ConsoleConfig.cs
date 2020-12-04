using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    //extern alias SerilogConsole;

    // defines the configuration for a console channel
    public class ConsoleConfig : ChannelConfig
    {
        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig ) =>
            sinkConfig.Console( restrictedToMinimumLevel : MinimumLevel, outputTemplate : EnrichedMessageTemplate );
    }
}