using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public abstract class SmsSink : ILogEventSink
    {
        public string FromNumber { get; internal set; } = string.Empty;
        public List<string> RecipientNumbers { get; internal set; } = new List<string>();

        public void Emit( LogEvent logEvent ) => SendMessage( logEvent.RenderMessage() );

        protected abstract void SendMessage( string logMessage );
    }
}
