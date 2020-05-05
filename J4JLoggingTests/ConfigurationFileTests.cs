using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoFacJ4JLogging;
using FluentAssertions;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Xunit.Abstractions;

namespace J4JLoggingTests
{
    public abstract class ConfigurationFileTests<TConfig>
        where TConfig : class
    {
        private readonly StringWriter _output = new StringWriter();

        public virtual void Log_event_derived_config_class( string filePath )
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.AddJ4JLoggingFromFile<TConfig>(
                    Path.Combine(Environment.CurrentDirectory, "config-files", filePath),
                    typeof(ConsoleChannel),
                    typeof(DebugChannel),
                    typeof(FileChannel),
                    typeof(TwilioChannel)
                );

            containerBuilder.RegisterType<J4JLogger>()
                .As<IJ4JLogger>();

            containerBuilder.Register(c => new TwilioTestConfig())
                .As<ITwilioConfig>()
                .SingleInstance();

            // bind supplemental stuff
            DIRegister(containerBuilder);

            var services = new AutofacServiceProvider(containerBuilder.Build());
            var logger = services.GetRequiredService<IJ4JLogger>();
            logger.SetLoggedType( this.GetType() );

            // resolve supplemental stuff
            DIResolve( services );

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

        protected virtual void DIRegister( ContainerBuilder containerBuilder )
        {
        }

        protected virtual void DIResolve( IServiceProvider services )
        {
        }
    }
}
