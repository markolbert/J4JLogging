using System;
using Serilog.Core;
using Serilog.Events;

#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    public class LastEventSink : ILogEventSink
    {
        public event EventHandler<string> LogEvent;

        public string? LastLogMessage { get; private set; }

        public void Emit( LogEvent logEvent )
        {
            LastLogMessage = logEvent.RenderMessage();

            RaiseLogEvent( LastLogMessage );
        }

        private void RaiseLogEvent( string logMessage )
        {
            var handler = LogEvent;

            handler?.Invoke(this, logMessage);
        }
    }
}
