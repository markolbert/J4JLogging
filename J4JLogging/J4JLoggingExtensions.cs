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
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // Miscellaneous utility extension methods
    public static class J4JLoggingExtensions
    {
        // Creates an instance of Serilog's ILogger to be wrapped by IJ4JLogger. Configures the
        // Serilog ILogger instance to work with IJ4JLogger and sets up the configured channels.
        public static ILogger? CreateBaseLogger( this IJ4JLoggerConfiguration config )
        {
            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext();

            if( config.Channels == null )
                return loggerConfig.CreateLogger();

            var minLevel = LogEventLevel.Fatal;

            foreach( var channel in config.Channels )
            {
                if( channel.MinimumLevel < minLevel )
                    minLevel = channel.MinimumLevel;

                channel.Configure( loggerConfig.WriteTo );
            }

            loggerConfig.SetMinimumLevel( minLevel );

            return loggerConfig.CreateLogger();
        }

        private static LoggerConfiguration SetMinimumLevel( this LoggerConfiguration config, LogEventLevel minLevel )
        {
            switch( minLevel )
            {
                case LogEventLevel.Debug:
                    config.MinimumLevel.Debug();
                    break;

                case LogEventLevel.Error:
                    config.MinimumLevel.Error();
                    break;

                case LogEventLevel.Fatal:
                    config.MinimumLevel.Fatal();
                    break;

                case LogEventLevel.Information:
                    config.MinimumLevel.Information();
                    break;

                case LogEventLevel.Verbose:
                    config.MinimumLevel.Verbose();
                    break;

                case LogEventLevel.Warning:
                    config.MinimumLevel.Warning();
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof(minLevel), minLevel, null );
            }

            return config;
        }
    }
}