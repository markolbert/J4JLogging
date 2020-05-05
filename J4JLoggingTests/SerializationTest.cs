using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoFacJ4JLogging;
using FluentAssertions;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace J4JLoggingTests
{
    public class SerializationTest
    {
        [Fact]
        public void Serialization_and_deserialization_multichannels()
        {
            var channels = new List<ILogChannel>
            {
                new ConsoleChannel(),
                new DebugChannel(),
                new FileChannel()
            };

            var config = new J4JLoggerConfiguration
            {
                EventElements = EventElements.All,
                Channels = channels,
                UseExternalSinks = true,
                MultiLineEvents = true
            };

            var channelTypes = new Dictionary<ChannelAttribute, Type>()
            {
                { new ChannelAttribute("Console"), typeof(ConsoleChannel) },
                { new ChannelAttribute("Debug"), typeof(DebugChannel) },
                { new ChannelAttribute("File"), typeof(FileChannel) },
            };

            var builder = new J4JLoggerConfigurationJsonBuilder();

            builder.AddChannel<ConsoleChannel>()
                .AddChannel<DebugChannel>()
                .AddChannel<FileChannel>();

            var settings = builder.BuildSerializerSettings();
            settings.WriteIndented = true;

            var text = JsonSerializer.Serialize(config, settings);

            var check = JsonSerializer.Deserialize<J4JLoggerConfiguration>(text, settings);

            check.MultiLineEvents.Should().Be( config.MultiLineEvents );
            check.UseExternalSinks.Should().Be(config.UseExternalSinks);
            check.SourceRootPath.Should().Be( config.SourceRootPath );
            check.EventElements.Should().Be( config.EventElements );
            check.MessageTemplate.Should().Be( config.MessageTemplate );
            check.MinimumLogLevel.Should().Be( config.MinimumLogLevel );
            check.Channels.Should().BeEquivalentTo( config.Channels );
        }

        [Theory]
        [InlineData("Console", typeof(ConsoleChannel))]
        [InlineData("Debug", typeof(DebugChannel))]
        [InlineData("File", typeof(FileChannel))]
        public void Serialization_and_deserialization_single_channel( string channelID, Type channelType )
        {
            var channels = new List<ILogChannel>();
            channels.Add( (ILogChannel) Activator.CreateInstance( channelType ) );

            var config = new J4JLoggerConfiguration
            {
                EventElements = EventElements.All,
                Channels = channels,
                UseExternalSinks = true,
                MultiLineEvents = true
            };

            var channelTypes = new Dictionary<ChannelAttribute, Type>()
            {
                { new ChannelAttribute(channelID), channelType },
            };

            var builder = new J4JLoggerConfigurationJsonBuilder();
            builder.AddChannel( channelType );

            var settings = builder.BuildSerializerSettings();
            settings.WriteIndented = true;

            var text = JsonSerializer.Serialize(config, settings);

            var check = JsonSerializer.Deserialize<J4JLoggerConfiguration>(text, settings);

            check.MultiLineEvents.Should().Be(config.MultiLineEvents);
            check.UseExternalSinks.Should().Be(config.UseExternalSinks);
            check.SourceRootPath.Should().Be(config.SourceRootPath);
            check.EventElements.Should().Be(config.EventElements);
            check.MessageTemplate.Should().Be(config.MessageTemplate);
            check.MinimumLogLevel.Should().Be(config.MinimumLogLevel);
            check.Channels.Should().BeEquivalentTo(config.Channels);
        }
    }
}
