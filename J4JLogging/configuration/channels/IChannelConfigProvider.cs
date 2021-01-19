using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public interface IChannelConfigProvider
    {
        LastEventConfig? LastEvent { get; }
        IConfiguration? Source { get; set; }

        TJ4JLogger? GetConfiguration<TJ4JLogger>()
            where TJ4JLogger : class, IJ4JLoggerConfiguration, new();
    }
}