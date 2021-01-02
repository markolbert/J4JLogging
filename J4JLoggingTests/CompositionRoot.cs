using System;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace J4JLoggingTests
{
    public class CompositionRoot<TJ4JLogger> : ICompositionRoot 
        where TJ4JLogger: IJ4JLoggerConfiguration, new()
    {
        private readonly IServiceProvider _svcProvider;

        public CompositionRoot( string configPath, string? loggerKey )
        {
            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, configPath ) )
                .AddUserSecrets<BasicTests>()
                .Build();

            var builder = new ContainerBuilder();

            var logChannels = new ChannelFactory( config, loggerKey, true );

            var configSB = new StringBuilder();

            configSB.Append( $"{loggerKey}{( string.IsNullOrEmpty( loggerKey ) ? string.Empty : ":" )}" );
            configSB.Append( "Channels:" );

            loggerKey = configSB.ToString();

            logChannels.AddChannel<ConsoleConfig>( $"{loggerKey}console" );
            logChannels.AddChannel<DebugConfig>( $"{loggerKey}debug" );
            logChannels.AddChannel<FileConfig>( $"{loggerKey}file" );
            logChannels.AddChannel<TwilioConfig>( "twilio" );

            builder.RegisterJ4JLogging<TJ4JLogger>( logChannels );

            builder.Register( c =>
                {
                    var logConfig = c.Resolve<IJ4JLoggerConfiguration>();

                    if( logConfig?.Channels == null )
                        throw new NullException(
                            $"Couldn't resolve an instance of IJ4JLoggerConfiguration with a defined Channels property" );

                    return logConfig.Channels.LastEvent!;
                } )
                .AsSelf()
                .SingleInstance();

            _svcProvider = new AutofacServiceProvider( builder.Build() );
        }

        public IJ4JLogger J4JLogger => _svcProvider.GetRequiredService<IJ4JLogger>();
        public LastEventConfig LastEventConfig => _svcProvider.GetRequiredService<LastEventConfig>();
    }
}
