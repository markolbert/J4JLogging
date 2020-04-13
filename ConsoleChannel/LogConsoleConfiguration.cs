using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{

    // needed to keep Json.Net deserializer happy
    public class LogConsoleConfiguration : LogChannelConfiguration
    {
        public LogConsoleConfiguration()
            : base( LogChannel.Console )
        {
        }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.Console( restrictedToMinimumLevel: MinimumLevel );
        }
    }
}