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

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig, string? outputTemplate = null )
        {
            return string.IsNullOrEmpty(outputTemplate)
                ? sinkConfig.Debug(restrictedToMinimumLevel: MinimumLevel)
                : sinkConfig.Debug(restrictedToMinimumLevel: MinimumLevel, outputTemplate: outputTemplate);
        }
    }
}