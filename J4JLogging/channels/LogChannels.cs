using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class LogChannels<TChannels> : ILogChannels
        where TChannels : IJ4JLoggingChannels, new()
    {
        private readonly List<ILogChannel> _channels;

        public LogChannels( IJ4JLoggerConfiguration<TChannels> config )
        {
            _channels = config.Channels.GetChannelInstances( config );
        }

        public LogEventLevel MinimumLogLevel => _channels.Min( c => c.MinimumLevel );

        public IEnumerator<ILogChannel> GetEnumerator()
        {
            foreach( var channel in _channels )
            {
                yield return channel;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
