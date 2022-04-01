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

namespace J4JSoftware.Logging
{
    public static class SinkExtensions
    {
        public static LoggerConfiguration LastEvent( this LoggerSinkConfiguration loggerConfig,
                                                     out LastEventSink sink,
                                                     LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose )
        {
            sink = new LastEventSink();

            return loggerConfig.Sink( sink, restrictedToMinimumLevel );
        }

        //public static LoggerConfiguration NetEvent(
        //    this LoggerSinkConfiguration loggerConfig,
        //    out NetEventSink sink,
        //    string outputTemplate = NetEventSink.DefaultTemplate,
        //    LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose
        //    )
        //{
        //    sink = new NetEventSink( outputTemplate );

        //    return loggerConfig.Sink( sink, restrictedToMinimumLevel );
        //}

        public static LoggerConfiguration NetEvent( this J4JLoggerConfiguration loggerConfig,
                                                    string outputTemplate = NetEventSink.DefaultTemplate,
                                                    LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose )
        {
            var sink = new NetEventSink( outputTemplate );
            loggerConfig.NetEventSink = sink;

            return loggerConfig.SerilogConfiguration
                               .WriteTo
                               .Sink( sink, restrictedToMinimumLevel );
        }
    }
}
