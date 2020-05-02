using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    [Channel("Debug")]
    public class DebugChannel : LogChannel
    {
        public DebugChannel()
        {
        }

        public DebugChannel( IConfigurationRoot configRoot, string loggerSection = "Logger")
            : base( configRoot, loggerSection )
        {
        }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.Debug( restrictedToMinimumLevel: MinimumLevel );
        }
    }
}