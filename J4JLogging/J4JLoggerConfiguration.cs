using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using JsonSubTypes;
using Newtonsoft.Json;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JLoggerConfiguration : IJ4JLoggerConfiguration
    {
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

        public List<ChannelConfiguration> Channels { get; set; }

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