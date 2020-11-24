using System.Collections.Generic;
using System.Collections.ObjectModel;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface ILogChannels : IEnumerable<ILogChannel>
    {
        LogEventLevel MinimumLogLevel { get; }
    }
}