using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace J4JLoggingTests
{
    public class CompositionRoot
    {
        private readonly IServiceProvider _svcProvider;

        public CompositionRoot( string configPath, string loggerKey, string channelsKey )
        {
            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, configPath))
                .Build();
            
            var builder = new ContainerBuilder();

            var channelConfig = config.GetSection(channelsKey).Get<ChannelConfiguration>();

            builder.Register(c => config.GetSection(loggerKey).Get<J4JLoggerConfiguration>())
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            builder.RegisterChannel(channelConfig.Console);
            builder.RegisterChannel(channelConfig.Debug);
            builder.RegisterChannel(channelConfig.File);
            builder.RegisterChannel( channelConfig.Twilio );

            builder.RegisterJ4JLogging();
            
            _svcProvider = new AutofacServiceProvider( builder.Build() );
        }

        public IJ4JLogger J4JLogger => _svcProvider.GetRequiredService<IJ4JLogger>();
    }
}
