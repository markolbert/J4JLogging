// Copyright (c) 2021, 2022 Mark A. Olbert 
// 
// This file is part of J4JLogger.
//
// J4JLogger is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// J4JLogger is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with J4JLogger. If not, see <https://www.gnu.org/licenses/>.

using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging;

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