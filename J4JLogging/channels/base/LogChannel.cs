using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    //extern alias SerilogConsole;

    // base class for all LogChannels
    public abstract class LogChannel : ILogChannel
    {
        protected LogChannel( 
            IJ4JChannelConfig channelConfig,
            Func<LoggerSinkConfiguration, LoggerConfiguration> configurator
            )
        {
            if( !channelConfig.IsValid)
                throw new ArgumentException($"Channel configuration is not valid");

            //LoggerConfiguration = config;

            MinimumLevel = channelConfig.MinimumLevel;
            OutputTemplate = channelConfig.OutputTemplate;
            ChannelConfigurator = configurator;
        }

        //protected IJ4JLoggerConfiguration LoggerConfiguration { get; }

        // the minimum Serilog level the channel will log
        public LogEventLevel MinimumLevel { get; }
        public string? OutputTemplate { get; }
        public Func<LoggerSinkConfiguration, LoggerConfiguration> ChannelConfigurator { get; }

        //// configures the channel
        //public abstract LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );
    }
}