using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Xunit.Abstractions;

namespace J4JLoggingTests
{
    public abstract class ConfigurationFileTest<TConfig>
        where TConfig : class
    {
        private readonly StringWriter _output = new StringWriter();

        protected ConfigurationFileTest()
        {
        }

        protected virtual void Register( ContainerBuilder containerBuilder, string filePath )
        {
            containerBuilder.RegisterType<J4JLogger>()
                .As<IJ4JLogger>();

            containerBuilder.Register(c => new TwilioTestConfig())
                .As<ITwilioConfig>()
                .SingleInstance();
        }

        public virtual void Log_event_derived_config_class( string filePath )
        {
            var containerBuilder = new ContainerBuilder();

            Register(containerBuilder, filePath);

            var services = new AutofacServiceProvider(containerBuilder.Build());
            var logger = services.GetRequiredService<IJ4JLogger>();
            logger.SetLoggedType( this.GetType() );

            var config = services.GetRequiredService<IJ4JLoggerConfiguration>();

            var twilio = config.Channels.FirstOrDefault(
                    c => c.Channel.Equals("Twilio", StringComparison.OrdinalIgnoreCase))
                as TwilioChannel;

            var twilioConfig = services.GetRequiredService<ITwilioConfig>();
            twilioConfig.Should().NotBe(null);

            //twilio?.Initialize(services.GetRequiredService<ITwilioConfig>());

            var mesgTemplate =  "{0}";

            Console.SetOut( _output );

            logger.Verbose<string>(mesgTemplate, "Verbose");
            logger.Information<string>(mesgTemplate, "Information");
            logger.Debug<string>(mesgTemplate, "Debug");
            logger.Warning<string>(mesgTemplate, "Warning");
            logger.Error<string>(mesgTemplate, "Error");
            logger.Fatal<string>(mesgTemplate, "Fatal");

            logger.ForceExternal().Information<Type, string>(mesgTemplate, typeof(TConfig), "Force External");
        }
    }
}
