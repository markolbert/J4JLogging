using System.Collections.Generic;
using System.Collections.ObjectModel;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the functionality of a type that can be used to configure the J4JLogger
    // system
    public interface IJ4JLoggerConfiguration
    {

        // Gets the Serilog message template in use, augmented/enriched by optional fields
        // supported by the J4JLogger system (e.g., SourceContext, which represents the 
        // source code file's path).
        string EnrichMessageTemplate( string mesgTemplate );

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
    }
}