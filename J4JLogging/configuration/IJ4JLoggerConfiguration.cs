using System.Collections.Generic;

namespace J4JSoftware.Logging
{
    // defines the functionality of a type that can be used to configure the J4JLogger
    // system
    public interface IJ4JLoggerConfiguration : IEnumerable<IChannelConfig>
    {
        // The root path of source code files. Used to eliminate redundant path information in the
        // logging output (i.e., by supressing common path elements)
        string? SourceRootPath { get; }

        // flag indicating whether or not multi line events are supported
        bool MultiLineEvents { get; }

        // flag indicating which event elements (e.g., type information, source code information)
        // will be added to the logging output
        EventElements EventElements { get; set; }
    }

    public interface IJ4JLoggerConfiguration<out TChannels> : IJ4JLoggerConfiguration
        where TChannels : ILogChannels, new()
    {
        TChannels Channels { get; }
    }
}