using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JLoggerConfiguration : IJ4JLoggerConfiguration
    {
        public const string DefaultMessageTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}";

        public J4JLoggerConfiguration()
        {
        }

        public J4JLoggerConfiguration( 
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

        public string MessageTemplate { get; set; } = DefaultMessageTemplate;

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

        public string SourceRootPath { get; set; }
        public bool MultiLineEvents { get; set; }

        public EventElements EventElements { get; set; } = EventElements.All;
        public bool UseExternalSinks { get; set; }

        public List<ILogChannel> Channels { get; set; } = new List<ILogChannel>();

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