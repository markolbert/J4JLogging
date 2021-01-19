using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public abstract class ChannelConfigProviderBase : IChannelConfigProvider
    {
        protected ChannelConfigProviderBase( 
            bool includeLastEvent,
            IJ4JLogger? logger )
        {
            Logger = logger;
            Logger?.SetLoggedType( GetType() );

            if( includeLastEvent )
                LastEvent = new LastEventConfig();
        }

        protected IJ4JLogger? Logger { get; }

        public LastEventConfig? LastEvent { get; }

        public abstract void AddChannelsToLoggerConfiguration<TJ4JLogger>( TJ4JLogger? loggerConfig = null )
            where TJ4JLogger : class, IJ4JLoggerConfiguration, new();

        protected abstract IEnumerable<IChannelConfig> EnumerateChannels();
    }
}