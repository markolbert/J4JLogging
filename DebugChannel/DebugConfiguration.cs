using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    [Channel("Debug")]
    public class DebugConfiguration : ChannelConfiguration
    {
        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.Debug( restrictedToMinimumLevel: MinimumLevel );
        }
    }
}