using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the base configuration for a log channel
    public class J4JChannelConfig
    {
        protected J4JChannelConfig()
        {
        }

        // the minimum Serilog level the channel will log
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Verbose;
        public string? OutputTemplate { get; set; } = null;

        public virtual bool IsValid { get; } = true;
    }
}