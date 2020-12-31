using System.Collections.Generic;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class DefaultLogChannels : LogChannels
    {
        public AvailableChannels ActiveChannels { get; set; } = AvailableChannels.Basic;
        public EventElements EventElements { get; set; } = EventElements.All;
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Verbose;
        public string OutputTemplate { get; set; } = ChannelConfig.DefaultOutputTemplate;

        public bool IncludeLastEvent { get; set; }

        public ConsoleConfig? Console { get; set; }
        public DebugConfig? Debug { get; set; }
        public FileConfig? File { get; set; }
        public TwilioConfig? Twilio { get; set; }
        public LastEventConfig LastEvent { get; } = new LastEventConfig();

        public override IEnumerator<IChannelConfig> GetEnumerator()
        {
            if( ( ActiveChannels & AvailableChannels.Console ) == AvailableChannels.Console )
            {
                yield return Console ?? new ConsoleConfig
                {
                    EventElements = EventElements,
                    MinimumLevel = MinimumLevel,
                    OutputTemplate = OutputTemplate
                };
            }

            if ((ActiveChannels & AvailableChannels.Debug) == AvailableChannels.Debug)
            {
                yield return Debug ?? new DebugConfig
                {
                    EventElements = EventElements,
                    MinimumLevel = MinimumLevel,
                    OutputTemplate = OutputTemplate
                };
            }

            if ((ActiveChannels & AvailableChannels.File) == AvailableChannels.File)
            {
                yield return File ?? new FileConfig
                {
                    EventElements = EventElements,
                    MinimumLevel = MinimumLevel,
                    OutputTemplate = OutputTemplate
                };
            }

            if( ( ActiveChannels & AvailableChannels.Twilio ) == AvailableChannels.Twilio
                && Twilio != null && Twilio.IsValid )
            {
                Twilio.EventElements = EventElements;
                Twilio.MinimumLevel = MinimumLevel;
                Twilio.OutputTemplate = OutputTemplate;

                yield return Twilio;
            }

            if ( IncludeLastEvent )
            {
                LastEvent.EventElements = EventElements;
                LastEvent.MinimumLevel = MinimumLevel;
                LastEvent.OutputTemplate = OutputTemplate;

                yield return LastEvent;
            }
        }
    }
}