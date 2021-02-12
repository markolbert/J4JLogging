#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLogging' is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
// 
// This library or program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with
// this library or program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

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
        private readonly StringBuilder _sb;

        private readonly StringWriter _stringWriter;

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

            _config.OnLogEvent( new NetEventArgs( logEvent.Level, _sb.ToString() ) );
        }
    }
}