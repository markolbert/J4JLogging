using System.Collections;
using System.Collections.Generic;

namespace J4JSoftware.Logging
{
    // a type that can be used to configure the J4JLogger system
    public class J4JLoggerConfiguration<TChannels> : IJ4JLoggerConfiguration<TChannels>
        where TChannels : ILogChannels, new()
    {
        // The root path of source code files. Used to eliminate redundant path information in the
        // logging output (i.e., by supressing common path elements)
        public string? SourceRootPath { get; set; }

        // flag indicating whether or not multi line events are supported
        public bool MultiLineEvents { get; set; }

        // flag indicating which event elements (e.g., type information, source code information)
        // will be added to the logging output
        public EventElements EventElements { get; set; } = EventElements.All;

        // configuration information for the log channels
        public TChannels Channels { get; set; } = new TChannels();

        public IEnumerator<IChannelConfig> GetEnumerator() => Channels.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}