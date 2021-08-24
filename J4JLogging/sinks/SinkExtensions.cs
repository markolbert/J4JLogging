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

using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;

namespace J4JSoftware.Logging
{
    public static class SinkExtensions
    {
        //public static TSmsSink CreateSmsSink<TSmsSink>(
        //    string? fromNumber,
        //    IEnumerable<string>? recipientNumbers
        //    )
        //    where TSmsSink : SmsSink, new() =>
        //    new TSmsSink
        //    {
        //        FromNumber = fromNumber,
        //        RecipientNumbers = recipientNumbers?.ToList()
        //    };

        public static LoggerConfiguration LastEvent(
            this LoggerSinkConfiguration loggerConfig,
            out LastEventSink sink,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose
            )
        {
            sink = new LastEventSink();

            return loggerConfig.Sink( sink, restrictedToMinimumLevel );
        }

        public static LoggerConfiguration NetEvent(
            this LoggerSinkConfiguration loggerConfig,
            out NetEventSink sink,
            ITextFormatter? formatter = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose
            )
        {
            sink = new NetEventSink { TextFormatter = formatter };

            return loggerConfig.Sink( sink, restrictedToMinimumLevel );
        }
    }
}