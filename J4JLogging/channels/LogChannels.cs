using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class LogChannels : ILogChannels
    {
        private readonly List<ILogChannel> _channels;

        public LogChannels( IEnumerable<ILogChannel> channels )
        {
            _channels = channels.ToList();
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
