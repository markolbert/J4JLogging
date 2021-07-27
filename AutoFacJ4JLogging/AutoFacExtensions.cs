#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'AutofacJ4JLogging' is free software: you can redistribute it
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
using Autofac;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class AutofacExtensions
    {
        public static ContainerBuilder RegisterJ4JLogging( 
            this ContainerBuilder builder,
            ChannelParameters? globalChannelParameters = null,
            params IChannelConfig[] channels )
        {
            builder.Register( c =>
                {
                    globalChannelParameters ??= new ChannelParameters( null );
                    
                    var retVal = new J4JLogger() { Parameters = globalChannelParameters };

                    retVal.AddChannels( channels );

                    return retVal;
                })
                .As<ILogger>()
                .SingleInstance();

            return builder;
        }

        public static ContainerBuilder RegisterJ4JLoggingForUnitTesting( 
            this ContainerBuilder builder,
            string? sourceRootPath = null,
            LogEventLevel minLevel = LogEventLevel.Verbose,
            string? logFileStub = null )
        {
            var globalChannelParameters = new ChannelParameters(null);

            builder.Register(c =>
                {
                    var retVal = new J4JLogger() { Parameters = globalChannelParameters };

                    var debug = retVal.AddChannel<DebugChannel>();

                    debug.Parameters = debug.Parameters with
                    {
                        IncludeSourcePath = !string.IsNullOrEmpty( sourceRootPath ),
                        SourceRootPath = sourceRootPath,
                        MinimumLevel = minLevel
                    };

                    if( string.IsNullOrEmpty( logFileStub ) ) 
                        return retVal;

                    var file = retVal.AddChannel<FileConfig>();

                    file.Parameters = ( (FileParameters) file.Parameters ) with
                    {
                        IncludeSourcePath = !string.IsNullOrEmpty( sourceRootPath ),
                        SourceRootPath = sourceRootPath,
                        MinimumLevel = minLevel
                    };

                    return retVal;
                })
                .As<ILogger>()
                .SingleInstance();

            return builder;
        }


        public static ContainerBuilder RegisterJ4JLoggingForConsoleApp(
            this ContainerBuilder builder,
            string? sourceRootPath = null,
            LogEventLevel minLevel = LogEventLevel.Verbose,
            string? logFileStub = null)
        {
            var globalChannelParameters = new ChannelParameters(null);

            builder.Register(c =>
                {
                    var retVal = new J4JLogger() { Parameters = globalChannelParameters };

                    var debug = retVal.AddChannel<DebugChannel>();

                    debug.Parameters = debug.Parameters with
                    {
                        IncludeSourcePath = !string.IsNullOrEmpty(sourceRootPath),
                        SourceRootPath = sourceRootPath,
                        MinimumLevel = minLevel
                    };

                    var console = retVal.AddChannel<ConsoleChannel>();

                    console.Parameters = console.Parameters with
                    {
                        IncludeSourcePath = !string.IsNullOrEmpty( sourceRootPath ),
                        SourceRootPath = sourceRootPath,
                        MinimumLevel = minLevel
                    };

                    if (string.IsNullOrEmpty(logFileStub))
                        return retVal;

                    var file = retVal.AddChannel<FileConfig>();

                    file.Parameters = ((FileParameters)file.Parameters) with
                    {
                        IncludeSourcePath = !string.IsNullOrEmpty(sourceRootPath),
                        SourceRootPath = sourceRootPath,
                        MinimumLevel = minLevel
                    };

                    return retVal;
                })
                .As<ILogger>()
                .SingleInstance();

            return builder;
        }
    }
}