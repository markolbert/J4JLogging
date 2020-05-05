using System.Collections.Generic;
using System.Collections.ObjectModel;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface IJ4JLoggerConfiguration
    {
        string GetEnrichedMessageTemplate();

        string SourceRootPath { get; }
        bool UseExternalSinks { get; }
        bool MultiLineEvents { get; }
        EventElements EventElements { get; }

        List<ILogChannel> Channels { get; }
        ReadOnlyCollection<string> ChannelsDefined { get; }

        bool IsChannelDefined( string channelID );

        LogEventLevel MinimumLogLevel { get; }
    }
}