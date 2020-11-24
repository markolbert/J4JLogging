using System;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface IJ4JChannelConfig
    {
        Type ChannelType { get; }
        LogEventLevel MinimumLevel { get; set; }
        string? OutputTemplate { get; set; }
        bool IsValid { get; }
    }
}