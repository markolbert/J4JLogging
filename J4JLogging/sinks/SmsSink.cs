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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace J4JSoftware.Logging
{
    public abstract class SmsSink : ILogEventSink
    {
        private readonly StringBuilder _sb;
        private readonly StringWriter _stringWriter;

        protected SmsSink(
            string template
            )
        {
            TextFormatter = new MessageTemplateTextFormatter( template );
            
            _sb = new StringBuilder();
            _stringWriter = new StringWriter( _sb );
        }

        public ITextFormatter TextFormatter { get; }

        public void Emit( LogEvent logEvent )
        {
            _sb.Clear();
            TextFormatter.Format( logEvent, _stringWriter );
            _stringWriter.Flush();

            SendMessage( _sb.ToString() );
        }

        protected abstract void SendMessage( string logMessage );
    }
}