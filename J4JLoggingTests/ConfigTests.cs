using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Xunit;

namespace J4JLoggingTests
{
    public class ConfigTests
    {
        [ Theory ]
        [ MemberData( nameof(SimpleData.TestData), MemberType = typeof(SimpleData) ) ]
        public void Simple( string filePath, LoggerInfo loggerInfo )
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, filePath ) )
                .Build();

            var settings = LoggerInfo.Create( configRoot );
            settings.Global.Should().NotBeNull();

            CheckCommonParameters( settings.Global!, loggerInfo!.Global! );

            settings.ChannelSpecific.Should().NotBeNull();

            if( loggerInfo.ChannelSpecific!.Count <= 0 )
                return;

            foreach( var kvp in settings.ChannelSpecific! )
            {
                var original = loggerInfo.ChannelSpecific![ kvp.Key ];
                CheckChannelParameters( kvp.Value, original );
            }
        }

        [ Theory ]
        [ MemberData( nameof(EmbeddedData.TestData), MemberType = typeof(EmbeddedData) ) ]
        public void Embedded( string filePath, LoggerInfo loggerInfo )
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, filePath ) )
                .Build();

            var settings = LoggerInfo.Create(configRoot.GetSection("LoggerInfo"));
            settings.Global.Should().NotBeNull();

            CheckCommonParameters( settings.Global!, loggerInfo!.Global! );

            settings.ChannelSpecific.Should().NotBeNull();

            if( loggerInfo.ChannelSpecific!.Count <= 0 )
                return;

            settings.ChannelSpecific.Should().NotBeNull();

            foreach( var kvp in settings.ChannelSpecific! )
            {
                var original = loggerInfo.ChannelSpecific![ kvp.Key ];
                CheckChannelParameters( kvp.Value, original );
            }
        }

        [ Theory ]
        [ MemberData( nameof(DerivedData.TestData), MemberType = typeof(DerivedData) ) ]
        public void Derived( string filePath, LoggerInfo loggerInfo )
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, filePath ) )
                .Build();

            var settings = LoggerInfo.Create(configRoot);
            settings.Global.Should().NotBeNull();

            CheckCommonParameters( settings.Global!, loggerInfo!.Global! );

            settings.ChannelSpecific.Should().NotBeNull();

            if( loggerInfo!.ChannelSpecific!.Count <= 0 )
                return;

            foreach( var kvp in settings.ChannelSpecific! )
            {
                var original = loggerInfo.ChannelSpecific![ kvp.Key ];
                CheckChannelParameters( kvp.Value, original );
            }
        }

        private static void CheckChannelParameters( ChannelParameters parsed, ChannelParameters original )
        {
            switch( parsed )
            {
                case FileParameters fileParsed:
                    if( original is not FileParameters fileOriginal )
                        throw new ArgumentException(
                            $"Original parameter type ({original.GetType()}) does not match parsed parameter type ({parsed.GetType()})" );

                    CheckFileParameters( fileParsed, fileOriginal );

                    break;

                case TwilioParameters twilioParsed:
                    if( original is not TwilioParameters twilioOriginal )
                        throw new ArgumentException(
                            $"Original parameter type ({original.GetType()}) does not match parsed parameter type ({parsed.GetType()})" );

                    CheckTwilioParameters( twilioParsed, twilioOriginal );

                    break;

                default:
                    CheckCommonParameters( parsed, original );
                    break;
            }
        }

        private static void CheckCommonParameters( ChannelParameters parsed, ChannelParameters original )
        {
            parsed.IncludeSourcePath.Should().Be( original.IncludeSourcePath );
            parsed.MinimumLevel.Should().Be( original.MinimumLevel );
            parsed.OutputTemplate.Should().Be( original.OutputTemplate );
            parsed.RequireNewLine.Should().Be( original.RequireNewLine );
            parsed.SourceRootPath.Should().Be( original.SourceRootPath );
        }

        private static void CheckFileParameters( FileParameters parsed, FileParameters original )
        {
            CheckCommonParameters( parsed, original );

            parsed.RollingInterval.Should().Be( original.RollingInterval );
            parsed.Folder.Should().Be( original.Folder );
            parsed.FileName.Should().Be( original.FileName );
        }

        private static void CheckTwilioParameters( TwilioParameters parsed, TwilioParameters original )
        {
            CheckCommonParameters( parsed, original );

            parsed.AccountToken.Should().Be( original.AccountToken );
            parsed.AccountSID.Should().Be( original.AccountSID );
            parsed.FromNumber.Should().Be( original.FromNumber );
        }
    }
}
