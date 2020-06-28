using System.Collections.Generic;
using System.Collections.ObjectModel;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the functionality of a type that can be used to configure the J4JLogger
    // system
    public interface IJ4JLoggerConfiguration
    {

        // the Serilog message template to be used to format log events
        string MessageTemplate { get; set; }
        
        // Gets the Serilog message template in use, augmented/enriched by optional fields
        // supported by the J4JLogger system (e.g., SourceContext, which represents the 
        // source code file's path).
        string GetEnrichedMessageTemplate();

        // The root path of source code files. Used to eliminate redundant path information in the
        // logging output (i.e., by supressing common path elements)
        string SourceRootPath { get; }

        // flag indicating whether or not to use external sinks (i.e., logging sinks that involve
        // post-processing, like TwilioChannel)
        bool UseExternalSinks { get; }

        // flag indicating whether or not multi line events are supported
        bool MultiLineEvents { get; }

        // flag indicating which event elements (e.g., type information, source code information)
        // will be added to the logging output
        EventElements EventElements { get; }

        // the configuration information for the channels to which logging output will be directed
        List<ILogChannel> Channels { get; }

        // the names/IDs of the channels to which logging output will be directed
        ReadOnlyCollection<string> ChannelsDefined { get; }

        bool IsChannelDefined( string channelID );

        // Determines the lowest minimum logging level for all defined channels. Needed to configure
        // Serilog's "floor" for logging.
        LogEventLevel MinimumLogLevel { get; }
    }
}