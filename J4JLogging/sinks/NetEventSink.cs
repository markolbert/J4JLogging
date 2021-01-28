using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace J4JSoftware.Logging
{
    public class NetEventSink : ILogEventSink
    {
        private readonly NetEventConfig _config;

        private readonly StringWriter _stringWriter;
        private readonly StringBuilder _sb;

        public NetEventSink( NetEventConfig config )
        {
            _config = config;

            _sb = new StringBuilder();
            _stringWriter = new StringWriter( _sb );
        }

        public ITextFormatter? TextFormatter { get; internal set; }

        public void Emit( LogEvent logEvent )
        {
            if( TextFormatter == null )
                return;

            _sb.Clear();
            TextFormatter.Format( logEvent, _stringWriter );
            _stringWriter.Flush();

            _config.OnLogEvent(new NetEventArgs(logEvent.Level, _sb.ToString()));
        }
    }
}
