using System;
using System.IO;
using System.Linq;
using Autofac;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace J4JLoggingTests
{
    public abstract class DerivedFileTestBase<TConfig> : ConfigurationFileTest<TConfig>
        where TConfig : class, IJ4JLoggerConfiguration
    {
        protected override void Register( ContainerBuilder containerBuilder, string filePath )
        {
            base.Register( containerBuilder, filePath );

            containerBuilder.AddJ4JLogging<TConfig>(
                Path.Combine( Environment.CurrentDirectory, "config-files", filePath ),
                typeof(ConsoleChannel),
                typeof(DebugChannel),
                typeof(FileChannel),
                typeof(TwilioChannel)
            );
        }
    }
}