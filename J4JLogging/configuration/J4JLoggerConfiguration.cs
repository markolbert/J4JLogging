using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JLoggerConfiguration : IJ4JLoggerConfiguration
    {
        public J4JLoggerConfiguration()
        {
        }

        public J4JLoggerConfiguration( IConfigurationRoot configRoot, string loggerSection = "Logger" )
        {
            if( configRoot == null )
                throw new NullReferenceException( nameof(configRoot) );

            var text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(SourceMessageTemplate)}" );
            if( !string.IsNullOrEmpty( text ) )
                SourceMessageTemplate = text;

            text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(SourceRootPath)}" );
            if( !string.IsNullOrEmpty( text ) )
                SourceRootPath = text;

            text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(MemberMessageTemplate)}" );
            if( !string.IsNullOrEmpty( text ) )
                MemberMessageTemplate = text;

            text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(IncludeAssemblyName)}" );
            if( !string.IsNullOrEmpty( text ) )
                IncludeAssemblyName = bool.Parse( text );

            text = configRoot.GetConfigValue( $"{loggerSection}:{nameof(IncludeSource)}" );
            if( !string.IsNullOrEmpty( text ) )
                IncludeSource = bool.Parse( text );
        }

        public string SourceMessageTemplate { get; set; } = "({File}:{Line})";
        public string MemberMessageTemplate { get; set; } = "{SourceContext}::{Member}";
        public string SourceRootPath { get; set; }

        public EntryElements DefaultElements
        {
            get
            {
                var retVal = EntryElements.None;

                if( IncludeAssemblyName ) retVal |= EntryElements.Assembly;
                if( IncludeSource ) retVal |= EntryElements.SourceCode;

                return retVal;
            }
        }

        public bool IncludeSource { get; set; }
        public bool IncludeAssemblyName { get; set; }

        public List<LogChannel> Channels { get; set; } = new List<LogChannel>();

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