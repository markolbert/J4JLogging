using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Xunit.Sdk;

namespace J4JLoggingTests
{
    public class CompositionRoot<TJ4JLogger> : ICompositionRoot
        where TJ4JLogger: class, IJ4JLoggerConfiguration, new()
    {
        private readonly IServiceProvider _svcProvider;

        public CompositionRoot()
        {
            _svcProvider = ConfigStatic();
        }

        public CompositionRoot( string configPath, string loggerKey )
        {
            _svcProvider = ConfigDynamic( configPath, loggerKey );
        }

        private IServiceProvider ConfigDynamic( string configPath, string loggerKey )
        {
            var builder = new ContainerBuilder();

            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, configPath ) )
                .AddUserSecrets<BasicTests>()
                .Build();

            var provider = new ChannelConfigProvider(loggerKey, true)
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

        public IJ4JLogger J4JLogger => _svcProvider.GetRequiredService<IJ4JLogger>();
        public LastEventConfig LastEventConfig => _svcProvider.GetRequiredService<LastEventConfig>();
    }
}
