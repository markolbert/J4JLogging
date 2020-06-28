using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // base class for all LogChannels
    public abstract class LogChannel : ILogChannel
    {
        protected LogChannel()
        {
            var attr = this.GetType().GetCustomAttributes( typeof(ChannelAttribute), false )
                .Cast<ChannelAttribute>()
                .FirstOrDefault();

            Channel = attr.ChannelID;
        }

        protected LogChannel( IConfigurationRoot configRoot, string loggerSection = "Logger" )
            : this()
        {
            if( configRoot == null )
                throw new NullReferenceException( nameof(configRoot) );

            var text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(MinimumLevel)}" );
            if( !string.IsNullOrEmpty( text ) )
                MinimumLevel = Enum.Parse<LogEventLevel>( text, true );
        }

        // the channel's name/ID, which should be unique
        public string Channel { get; }

        // the minimum Serilog level the channel will log
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Verbose;

        // configures the channel
        public abstract LoggerConfiguration Configure( 
            LoggerSinkConfiguration sinkConfig,
            string? outputTemplate = null );
    }
}