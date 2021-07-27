#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLoggingTests' is free software: you can redistribute it
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
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace J4JLoggingTests
{
    public class CompositionRoot : ICompositionRoot
    {
        public static CompositionRoot Default { get; } = new();

        private IContainer _container;

        private CompositionRoot()
        {
            var builder = new ContainerBuilder();

            builder.RegisterJ4JLoggingForUnitTesting( "c:/Programming/J4JLogger/J4JLoggingTests" );

            _container = builder.Build();
        }

        public IJ4JLogger J4JLogger => _container.Resolve<IJ4JLogger>();
        public LastEventConfig LastEventConfig => _svcProvider.GetRequiredService<LastEventConfig>();

        private IServiceProvider ConfigDynamic( string configPath, string loggerKey )
        {
            var builder = new ContainerBuilder();

            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, configPath ) )
                .AddUserSecrets<BasicTests>()
                .Build();

            var provider = new ChannelConfigProvider( loggerKey, true )
                {
                    Source = config
                }
                .AddChannel<ConsoleConfig>( "channels:console" )
                .AddChannel<DebugConfig>( "channels:debug" )
                .AddChannel<FileConfig>( "channels:file" )
                .AddChannel<TwilioConfig>( "twilio" );

            builder.RegisterJ4JLogging<TJ4JLogger>( provider );

            builder.Register( c => provider.LastEvent! )
                .AsSelf()
                .SingleInstance();

            return new AutofacServiceProvider( builder.Build() );
        }

        private IServiceProvider ConfigStatic()
        {
            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder
                .AddUserSecrets<BasicTests>()
                .Build();

            var twilioConfig = config.GetSection( "twilio" ).Get<TwilioConfig>();
            var lastConfig = new LastEventConfig();

            var builder = new ContainerBuilder();

            var j4jConfig = new J4JLoggerConfiguration();

            j4jConfig.Channels.Add( new ConsoleConfig() );
            j4jConfig.Channels.Add( new DebugConfig() );
            j4jConfig.Channels.Add( new FileConfig() );
            j4jConfig.Channels.Add( twilioConfig );
            j4jConfig.Channels.Add( lastConfig );

            builder.RegisterJ4JLogging( j4jConfig );

            builder.Register( c => lastConfig )
                .AsSelf()
                .SingleInstance();

            return new AutofacServiceProvider( builder.Build() );
        }
    }
}