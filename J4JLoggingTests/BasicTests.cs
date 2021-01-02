using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FluentAssertions;
using J4JSoftware.Logging;
using Xunit;

namespace J4JLoggingTests
{
    public class BasicTests
    {
        [Theory]
        [InlineData(typeof(J4JLoggerConfiguration), "config-files/Simple.json", null)]
        [InlineData(typeof(DerivedConfiguration), "config-files/Derived.json", null)]
        [InlineData(typeof(J4JLoggerConfiguration), "config-files/Embedded.json", "Container:Logger")]
        public void Uncached( Type configType, string configPath, string? loggerKey )
        {
            var compRoot = GetCompositionRoot( configType, configPath, loggerKey );

            var logger = compRoot.J4JLogger;
            logger.SetLoggedType(this.GetType());

            var lastEvent = compRoot.LastEventConfig;
            var template = "{0} ({1})";

            logger.Verbose<string, string>(template, "Verbose", configPath);
            lastEvent.LastLogMessage.Should().Be( format_message( "Verbose" ) );

            logger.Warning<string, string>(template, "Warning", configPath);
            lastEvent.LastLogMessage.Should().Be(format_message("Warning"));

            logger.Information<string, string>(template, "Information", configPath);
            lastEvent.LastLogMessage.Should().Be(format_message("Information"));

            logger.Debug<string, string>(template, "Debug", configPath);
            lastEvent.LastLogMessage.Should().Be(format_message("Debug"));

            logger.Error<string, string>(template, "Error", configPath);
            lastEvent.LastLogMessage.Should().Be(format_message("Error"));

            logger.Fatal<string, string>(template, "Fatal", configPath);
            lastEvent.LastLogMessage.Should().Be(format_message("Fatal"));

            logger.IncludeSms().Verbose<string, string>( "{0} ({1})", "Verbose", configPath );

            string format_message( string prop1 ) =>
                template.Replace( "{0}", $"\"{prop1}\"" ).Replace( "{1}", $"\"{configPath}\"" );
        }

        [Theory]
        [InlineData(typeof(J4JLoggerConfiguration), "config-files/Simple.json", null)]
        [InlineData(typeof(DerivedConfiguration), "config-files/Derived.json", null)]
        [InlineData(typeof(J4JLoggerConfiguration), "config-files/Embedded.json", "Container:Logger")]
        public void Cached(Type configType, string configPath, string loggerKey)
        {
            var compRoot = GetCompositionRoot(configType, configPath, loggerKey);

            var cached = new J4JCachedLogger();
            cached.SetLoggedType(this.GetType());

            var template = "{0} ({1})";

            cached.Verbose<string, string>(template, "Verbose", configPath);
            cached.Warning<string, string>(template, "Warning", configPath);
            cached.Information<string, string>(template, "Information", configPath);
            cached.Debug<string, string>(template, "Debug", configPath);
            cached.Error<string, string>(template, "Error", configPath);
            cached.Fatal<string, string>(template, "Fatal", configPath);
            cached.IncludeSms().Verbose<string, string>("{0} ({1})", "Verbose", configPath);

            var logger = compRoot.J4JLogger;
            var lastEvent = compRoot.LastEventConfig;

            foreach (var entry in cached.Cache)
            {
                if (entry.IncludeSms)
                    logger.IncludeSms();

                logger.Write(entry.LogEventLevel, entry.Template, entry.PropertyValues, entry.MemberName,
                    entry.SourcePath, entry.SourceLine);

                lastEvent.LastLogMessage.Should().Be(format_message(entry.LogEventLevel.ToString()));
            }

            string format_message(string prop1) =>
                template.Replace("{0}", $"\"{prop1}\"").Replace("{1}", $"\"{configPath}\"");
        }

        private ICompositionRoot GetCompositionRoot(Type configType, string configPath, string? loggerKey)
        {
            typeof(IJ4JLoggerConfiguration).IsAssignableFrom( configType ).Should().BeTrue();

            var compRootType = typeof(CompositionRoot<>).MakeGenericType(configType);

            var temp = Activator.CreateInstance( compRootType, new object?[] { configPath, loggerKey } );
            temp.Should().NotBeNull();

            return (ICompositionRoot) temp!;
        }
    }
}
