using System.Collections;
using System.Collections.Generic;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public abstract class LogChannels : ILogChannels
    {
        protected LogChannels()
        {
        }

        public AvailableChannels ActiveChannels { get; set; } = AvailableChannels.Basic;
        public EventElements EventElements { get; set; } = EventElements.All;
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Verbose;
        public string OutputTemplate { get; set; } = ChannelConfig.DefaultOutputTemplate;

        public bool IncludeLastEvent { get; set; }

        public abstract IEnumerator<IChannelConfig> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
