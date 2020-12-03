using System.Collections;
using System.Collections.Generic;
using System.Text;

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

        // flag indicating whether or not to use external sinks (i.e., logging sinks that involve
        // post-processing, like TwilioChannel)
        public bool UseExternalSinks { get; set; }

        // configuration information for the log channels
        public TChannels Channels { get; set; } = new TChannels();

        public IEnumerator<IJ4JChannelConfig> GetEnumerator() => Channels.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Gets the Serilog message template in use, augmented/enriched by optional fields
        // supported by the J4JLogger system (e.g., SourceContext, which represents the 
        // source code file's path).
        public string EnrichMessageTemplate(string mesgTemplate)
        {
            var sb = new StringBuilder(mesgTemplate);

            foreach (var element in EnumUtils.GetUniqueFlags<EventElements>())
            {
                switch (element)
                {
                    case EventElements.Type:
                        sb.Append(" {SourceContext}{MemberName}");
                        break;

                    case EventElements.SourceCode:
                        sb.Append(" {SourceCodeInformation}");
                        break;
                }
            }

            sb.Append("{NewLine}{Exception}");

            return sb.ToString();
        }
    }
}