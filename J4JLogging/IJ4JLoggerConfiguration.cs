using System.Collections.Generic;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface IJ4JLoggerConfiguration
    {
        string SourceMessageTemplate { get; }
        string MemberMessageTemplate { get; }

        string SourceRootPath { get; }
        
        EntryElements DefaultElements { get; }

        List<LogChannelConfiguration> Channels { get; }
        LogChannel ChannelsDefined { get; }

        bool IsChannelDefined( LogChannel channel );

        LogEventLevel MinimumLogLevel { get; }
    }
}