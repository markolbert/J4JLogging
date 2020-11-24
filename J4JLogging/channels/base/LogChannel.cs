using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // base class for all LogChannels
    public abstract class LogChannel : ILogChannel
    {
        protected LogChannel( IJ4JLoggerConfiguration config, IJ4JChannelConfig channelConfig )
        {
            if( !channelConfig.IsValid)
                throw new ArgumentException($"Channel configuration is not valid");

            LoggerConfiguration = config;

            MinimumLevel = channelConfig.MinimumLevel;
            OutputTemplate = channelConfig.OutputTemplate;
        }

        protected IJ4JLoggerConfiguration LoggerConfiguration { get; }

        // the minimum Serilog level the channel will log
        public LogEventLevel MinimumLevel { get; }
        public string? OutputTemplate { get; }

        // configures the channel
        public abstract LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );
    }
}