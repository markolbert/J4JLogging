using System;
using System.Collections.Generic;

namespace J4JSoftware.Logging
{
    public interface IChannelConfigProvider
    {
        LastEventConfig? LastEvent { get; }

        void AddChannelsToLoggerConfiguration<TJ4JLogger>( TJ4JLogger? loggerConfig = null )
            where TJ4JLogger : class, IJ4JLoggerConfiguration, new();
    }
}