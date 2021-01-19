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

            var provider = new DynamicChannelConfigProvider(loggerKey, true)
                .AddChannel<ConsoleConfig>( "channels:console" )
                .AddChannel<DebugConfig>( "channels:debug" )
                .AddChannel<FileConfig>( "channels:file" )
                .AddChannel<TwilioConfig>( "twilio" );

            var configBuilder = new ConfigurationBuilder();

            provider.Source = configBuilder
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, configPath ) )
                .AddUserSecrets<BasicTests>()
                .Build();

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

            var builder = new ContainerBuilder();

            var provider = new StaticChannelConfigProvider(true)
                .AddChannel(new ConsoleConfig())
                .AddChannel( new DebugConfig())
                .AddChannel( new FileConfig())
                .AddChannel( twilioConfig );

            builder.RegisterJ4JLogging<TJ4JLogger>( provider );

            builder.Register( c => provider.LastEvent! )
                .AsSelf()
                .SingleInstance();

            return new AutofacServiceProvider( builder.Build() );
        }

        public IJ4JLogger J4JLogger => _svcProvider.GetRequiredService<IJ4JLogger>();
        public LastEventConfig LastEventConfig => _svcProvider.GetRequiredService<LastEventConfig>();
    }
}
