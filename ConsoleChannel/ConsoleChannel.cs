using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{

    [Channel("Console")]
    public class ConsoleChannel : LogChannel
    {
        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.Console( restrictedToMinimumLevel: MinimumLevel );
        }
    }
}