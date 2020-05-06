using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autofac;
using AutoFacJ4JLogging;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace J4JLoggingTests
{
    public class EmbeddedFileTest : ConfigurationFileTest<EmbeddedConfiguration>
    {
        [Theory]
        [InlineData("Embedded.json")]
        public override void Log_event_derived_config_class(string filePath)
        {
            base.Log_event_derived_config_class(filePath);
        }

        protected override void Register( ContainerBuilder containerBuilder, string filePath )
        {
            base.Register( containerBuilder, filePath );

            var configRoot = new ConfigurationBuilder()
                .SetBasePath( Path.Combine( Environment.CurrentDirectory, "config-files" ) )
                .AddJsonFile( filePath )
                .Build();

            containerBuilder.AddJ4JLogging<J4JLoggerConfiguration>(
                configRoot,
                "Logger",
                typeof(ConsoleChannel), typeof(DebugChannel), typeof(FileChannel), typeof(TwilioChannel)
            );

            containerBuilder.RegisterLogger();
        }
    }
}