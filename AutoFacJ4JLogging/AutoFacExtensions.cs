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

namespace J4JSoftware.Logging
{
    public static class AutofacExtensions
    {
        public static ContainerBuilder RegisterJ4JLogging<TJ4JLogger>( this ContainerBuilder builder,
            TJ4JLogger config )
            where TJ4JLogger : IJ4JLoggerConfiguration
        {
            builder.Register( c =>
                {
                    var retVal = config.CreateBaseLogger();

                    if( retVal != null )
                        return retVal;

                    throw new NullReferenceException( "Could not create ILogger from IJ4JLoggerConfiguration" );
                } )
                .As<ILogger>()
                .SingleInstance();

            builder.Register( c =>
                {
                    var baseLogger = c.Resolve<ILogger>();

                    return new J4JLogger( config, baseLogger );
                } )
                .As<IJ4JLogger>();

            return builder;
        }

        public static ContainerBuilder RegisterJ4JLogging<TJ4JLogger>(
            this ContainerBuilder builder,
            IChannelConfigProvider provider )
            where TJ4JLogger : class, IJ4JLoggerConfiguration, new()
        {
            builder.Register( c =>
                {
                    var retVal = provider.GetConfiguration<TJ4JLogger>();

                    if( retVal == null )
                        throw new NullReferenceException( $"Couldn't create an instance of {typeof(TJ4JLogger)}" );

                    return retVal;
                } )
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            builder.Register( c =>
                {
                    var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();

                    return loggerConfig.CreateBaseLogger()!;
                } )
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLogger>()
                .As<IJ4JLogger>();

            return builder;
        }
    }
}