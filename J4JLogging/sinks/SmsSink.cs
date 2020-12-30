using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace J4JSoftware.Logging
{
    public abstract class SmsSink : ILogEventSink
    {
        private readonly StringWriter _stringWriter;
        private readonly StringBuilder _sb;

        protected SmsSink()
        {
            _sb = new StringBuilder();
            _stringWriter = new StringWriter( _sb );
        }

        public string FromNumber { get; internal set; } = string.Empty;
        public List<string> RecipientNumbers { get; internal set; } = new List<string>();
        public ITextFormatter? TextFormatter { get; internal set; }

        public void Emit( LogEvent logEvent )
        {
            if( TextFormatter == null )
                return;

            _sb.Clear();
            TextFormatter.Format(logEvent, _stringWriter);
            _stringWriter.Flush();
            
            SendMessage( _sb.ToString() );
        }

        protected abstract void SendMessage( string logMessage );
    }
}
