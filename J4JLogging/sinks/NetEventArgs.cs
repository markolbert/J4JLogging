using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class NetEventArgs
    {
        public NetEventArgs( LogEventLevel level, string mesg )
        {
            Level = level;
            LogMessage = mesg;
        }

        public LogEventLevel Level { get; }
        public string LogMessage { get; }
    }
}