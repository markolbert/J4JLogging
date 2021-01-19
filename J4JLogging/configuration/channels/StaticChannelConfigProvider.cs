using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public class StaticChannelConfigProvider : ChannelConfigProviderBase
    {
        private readonly Dictionary<Type, IChannelConfig> _channels = new();

        public StaticChannelConfigProvider(
            bool includeLastEvent,
            IJ4JLogger? logger = null )
            : base( includeLastEvent, logger )
        {
        }

        public StaticChannelConfigProvider AddChannel(IChannelConfig channelConfig)
        {
            var configType = channelConfig.GetType();

            if( !_channels.ContainsKey(configType  ))
                _channels.Add(configType, channelConfig  );

            return this;
        }

        public override void AddChannelsToLoggerConfiguration<TJ4JLogger>( TJ4JLogger? loggerConfig = null )
            where TJ4JLogger: class
        {
            loggerConfig ??= new TJ4JLogger();
            loggerConfig.Channels.AddRange( EnumerateChannels() );
        }

        protected override IEnumerable<IChannelConfig> EnumerateChannels()
        {
            if( _channels.Count == 0 )
            {
                Logger?.Error("No IConfiguration Source is defined");
                yield break;
            }

            foreach( var kvp in _channels )
            {
                yield return kvp.Value;
            }

            if( LastEvent != null )
                yield return LastEvent;
        }
    }
}