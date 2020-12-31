using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace J4JLoggingTests
{
    public class FileTest
    {
        [Theory]
        [InlineData("config-files/Simple.json", null)]
        [InlineData("config-files/Derived.json", "Logger")]
        [InlineData("config-files/Embedded.json", "Container:Logger")]
        public void Simple( string configPath, string? loggerKey )
        {
            var compRoot = new CompositionRoot( configPath, loggerKey );

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
    }
}
