using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    // needed to keep Json.Net deserializer happy
    public class LogDebugConfiguration : LogChannelConfiguration
    {
        public LogDebugConfiguration()
            : base( LogChannel.Debug )
        {
        }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.Debug( restrictedToMinimumLevel: MinimumLevel );
        }
    }
}