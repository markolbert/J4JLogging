using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the base configuration for a log channel
    public class J4JChannelConfig<TChannel> : IJ4JChannelConfig
        where TChannel : LogChannel
    {
        // the default Serilog message template to be used by the system
        public const string DefaultMessageTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}";

        protected J4JChannelConfig()
        {
        }

        public Type ChannelType => typeof(TChannel);

        // the minimum Serilog level the channel will log
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Verbose;
        public string? OutputTemplate { get; set; } = DefaultMessageTemplate;

        public virtual bool IsValid { get; } = true;
    }
}