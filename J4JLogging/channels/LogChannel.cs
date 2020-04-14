using System;
using System.Linq;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class LogChannel : IChannelConfiguration
    {
        protected LogChannel()
        {
            var attr = this.GetType().GetCustomAttributes( typeof(ChannelAttribute), false )
                .Cast<ChannelAttribute>()
                .FirstOrDefault();

            Channel = attr?.ChannelID;
        }

        public string Channel { get; }
        public LogEventLevel MinimumLevel { get; set; }

        public virtual LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            throw new NotImplementedException($"This base method should not be called");
        }
    }
}