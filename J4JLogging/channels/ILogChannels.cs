using System.Collections.Generic;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface ILogChannels : IEnumerable<ILogChannel>
    {
        LogEventLevel MinimumLogLevel { get; }
    }
}