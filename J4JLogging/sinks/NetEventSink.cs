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

using System;
using System.IO;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace J4JSoftware.Logging;

public class NetEventSink : ILogEventSink
{
    public const string DefaultTemplate =
        "[{Level:u3}] {Message:lj}";

    private readonly StringBuilder _sb = new();
    private readonly StringWriter _stringWriter;
    private readonly ITextFormatter _textFormatter;

    public NetEventSink( string outputTemplate = DefaultTemplate )
    {
        _stringWriter = new StringWriter( _sb );
        _textFormatter = new MessageTemplateTextFormatter( outputTemplate );
    }

    internal Action<NetEventArgs>? RaiseEvent { get; set; }

    public void Emit( LogEvent logEvent )
    {
        _sb.Clear();
        _textFormatter.Format( logEvent, _stringWriter );
        _stringWriter.Flush();

        RaiseEvent?.Invoke( new NetEventArgs( logEvent, _sb.ToString() ) );
    }
}