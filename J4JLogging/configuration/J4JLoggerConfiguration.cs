using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // a type that can be used to configure the J4JLogger system
    public class J4JLoggerConfiguration : IJ4JLoggerConfiguration
    {
        // the default Serilog message template to be used by the system
        public const string DefaultMessageTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}";

#pragma warning disable 8618
        public J4JLoggerConfiguration()
#pragma warning restore 8618
        {
        }

#pragma warning disable 8618
        public J4JLoggerConfiguration( 
#pragma warning restore 8618
            IConfigurationRoot configRoot, 
            string loggerSection = "Logger" 
            )
        {
            if ( configRoot == null )
                throw new NullReferenceException( nameof(configRoot) );

            var text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(MessageTemplate)}" );
            if( !string.IsNullOrEmpty( text ) )
                MessageTemplate = text;

            text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(SourceRootPath)}" );
            if( !string.IsNullOrEmpty( text ) )
                SourceRootPath = text;

            text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(EventElements)}" );
            if( !string.IsNullOrEmpty( text ) )
                EventElements = Enum.Parse<EventElements>( text, true );

            text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(UseExternalSinks)}" );
            if( !string.IsNullOrEmpty( text ) )
                UseExternalSinks = bool.Parse( text );

            text = configRoot.GetConfigValue($"{loggerSection}:{nameof(MultiLineEvents)}");
            if (!string.IsNullOrEmpty(text))
                MultiLineEvents = bool.Parse(text);
        }

        // the Serilog message template to be used to format log events
        public string MessageTemplate { get; set; } = DefaultMessageTemplate;

        // Gets the Serilog message template in use, augmented/enriched by optional fields
        // supported by the J4JLogger system (e.g., SourceContext, which represents the 
        // source code file's path).
        public string GetEnrichedMessageTemplate()
        {
            var sb = new StringBuilder(
                string.IsNullOrEmpty( MessageTemplate )
                    ? J4JLoggerConfiguration.DefaultMessageTemplate
                    : MessageTemplate
            );

            foreach( var element in EnumUtils.GetUniqueFlags<EventElements>() )
            {
                switch( element )
                {
                    case EventElements.Type:
                        sb.Append( " {SourceContext}{MemberName}" );
                        break;

                    case EventElements.SourceCode:
                        sb.Append( " {SourceCodeInformation}" );
                        break;
                }
            }

            sb.Append( "{NewLine}{Exception}" );

            return sb.ToString();
        }

        // The root path of source code files. Used to eliminate redundant path information in the
        // logging output (i.e., by supressing common path elements)
        public string SourceRootPath { get; set; }

        // flag indicating whether or not multi line events are supported
        public bool MultiLineEvents { get; set; }

        // flag indicating which event elements (e.g., type information, source code information)
        // will be added to the logging output
        public EventElements EventElements { get; set; } = EventElements.All;

        // flag indicating whether or not to use external sinks (i.e., logging sinks that involve
        // post-processing, like TwilioChannel)
        public bool UseExternalSinks { get; set; }

        // the configuration information for the channels to which logging output will be directed
        public List<ILogChannel> Channels { get; set; } = new List<ILogChannel>();

        // the names/IDs of the channels to which logging output will be directed
        public ReadOnlyCollection<string> ChannelsDefined
        {
            get
            {
                var retVal = new List<string>();

                if( Channels == null || Channels.Count == 0 )
                    return retVal.AsReadOnly();

                return Channels.Aggregate( retVal, ( l, c ) =>
                    {
                        l.Add( c.Channel );
                        return l;
                    }, l => l )
                    .AsReadOnly();
            }
        }

        public bool IsChannelDefined( string channelID ) =>
            Channels?.Any( c => c.Channel.Equals( channelID, StringComparison.OrdinalIgnoreCase ) )
            ?? false;

        // Determines the lowest minimum logging level for all defined channels. Needed to configure
        // Serilog's "floor" for logging.
        public LogEventLevel MinimumLogLevel
        {
            get
            {
                if( Channels == null || Channels.Count == 0 )
                    return LogEventLevel.Verbose;

                return Channels.Min( c => c.MinimumLevel );
            }
        }
    }
}