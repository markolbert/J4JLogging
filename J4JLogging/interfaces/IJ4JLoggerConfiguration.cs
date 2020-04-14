using System.Collections.Generic;
using System.Collections.ObjectModel;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface IJ4JLoggerConfiguration
    {
        string SourceMessageTemplate { get; }
        string MemberMessageTemplate { get; }

        string SourceRootPath { get; }
        
        EntryElements DefaultElements { get; }

        List<LogChannel> Channels { get; }
        ReadOnlyCollection<string> ChannelsDefined { get; }

        bool IsChannelDefined( string channelID );

        LogEventLevel MinimumLogLevel { get; }
    }
}