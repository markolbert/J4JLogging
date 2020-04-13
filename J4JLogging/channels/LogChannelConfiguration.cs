using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class LogChannelConfiguration : ILogChannelConfiguration
    {
        private readonly LogChannel _channel;

        protected LogChannelConfiguration(LogChannel channel)
        {
            _channel = channel;
        }

        public LogChannel GetChannelType() => _channel;
        public LogEventLevel MinimumLevel { get; set; }

        public virtual LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            throw new NotImplementedException($"This base method should not be called");
        }
    }
}