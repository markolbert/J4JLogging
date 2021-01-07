using System;
using System.Collections;
using System.Collections.Generic;

namespace J4JSoftware.Logging
{
    public class ChannelInformation : IEnumerable<KeyValuePair<string, Type>>
    {
        private readonly Dictionary<string, Type> _channels = new(StringComparer.OrdinalIgnoreCase);

        public ChannelInformation AddChannel<TChannel>(string configPath)
            where TChannel : IChannelConfig, new()
        {
            if (string.IsNullOrEmpty(configPath))
                return this;

            if (_channels.ContainsKey(configPath))
                _channels[configPath] = typeof(TChannel);
            else _channels.Add(configPath, typeof(TChannel));

            return this;
        }

        public ChannelInformation AddChannel(Type channelType, string configPath)
        {
            if (string.IsNullOrEmpty(configPath)
                || !typeof(IChannelConfig).IsAssignableFrom(channelType))
                return this;

            if (_channels.ContainsKey(configPath))
                _channels[configPath] = channelType;
            else _channels.Add(configPath, channelType);

            return this;
        }

        public IEnumerator<KeyValuePair<string, Type>> GetEnumerator()
        {
            foreach( var kvp in _channels )
            {
                yield return kvp;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}