using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
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

        protected LogChannel( IConfigurationRoot configRoot, string loggerSection = "Logger" )
            : this()
        {
            if( configRoot == null )
                throw new NullReferenceException( nameof(configRoot) );

            var text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(MinimumLevel)}" );
            if( !string.IsNullOrEmpty( text ) )
                MinimumLevel = Enum.Parse<LogEventLevel>( text, true );
        }

        public string Channel { get; }
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Verbose;

        public virtual LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            throw new NotImplementedException($"This base method should not be called");
        }
    }
}