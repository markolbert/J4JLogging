#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLoggingTests' is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
// 
// This library or program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with
// this library or program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.ComponentModel;
using FluentAssertions;
using J4JSoftware.Logging;
using Xunit;

namespace J4JLoggingTests
{
    public class BasicTests
    {
        [ Theory ]
        [ InlineData( CompositionRootType.Dynamic, typeof(J4JLoggerConfiguration), "config-files/Simple.json", null ) ]
        [ InlineData( CompositionRootType.Dynamic, typeof(DerivedConfiguration), "config-files/Derived.json", null ) ]
        [ InlineData( CompositionRootType.Dynamic, typeof(J4JLoggerConfiguration), "config-files/Embedded.json",
            "Container:Logger" ) ]
        [ InlineData( CompositionRootType.Static, typeof(J4JLoggerConfiguration), "config-files/Simple.json", null ) ]
        [ InlineData( CompositionRootType.Static, typeof(DerivedConfiguration), "config-files/Derived.json", null ) ]
        [ InlineData( CompositionRootType.Static, typeof(J4JLoggerConfiguration), "config-files/Embedded.json",
            "Container:Logger" ) ]
        public void Uncached( CompositionRootType rootType, Type configType, string configPath, string? loggerKey )
        {
            var compRoot = GetCompositionRoot( rootType, configType, configPath, loggerKey );

            var logger = compRoot.J4JLogger;
            logger.SetLoggedType( GetType() );

            var lastEvent = compRoot.LastEventConfig;
            var template = "{0} ({1})";

            logger.Verbose<string, string>( template, "Verbose", configPath );
            lastEvent.LastLogMessage.Should().Be( format_message( "Verbose" ) );

            logger.Warning<string, string>( template, "Warning", configPath );
            lastEvent.LastLogMessage.Should().Be( format_message( "Warning" ) );

            logger.Information<string, string>( template, "Information", configPath );
            lastEvent.LastLogMessage.Should().Be( format_message( "Information" ) );

            logger.Debug<string, string>( template, "Debug", configPath );
            lastEvent.LastLogMessage.Should().Be( format_message( "Debug" ) );

            logger.Error<string, string>( template, "Error", configPath );
            lastEvent.LastLogMessage.Should().Be( format_message( "Error" ) );

            logger.Fatal<string, string>( template, "Fatal", configPath );
            lastEvent.LastLogMessage.Should().Be( format_message( "Fatal" ) );

            logger.OutputNextEventToSms().Verbose<string, string>( "{0} ({1})", "Verbose", configPath );

            string format_message( string prop1 )
            {
                return template.Replace( "{0}", $"\"{prop1}\"" ).Replace( "{1}", $"\"{configPath}\"" );
            }
        }

        [ Theory ]
        [ InlineData( CompositionRootType.Dynamic, typeof(J4JLoggerConfiguration), "config-files/Simple.json", null ) ]
        [ InlineData( CompositionRootType.Dynamic, typeof(DerivedConfiguration), "config-files/Derived.json", null ) ]
        [ InlineData( CompositionRootType.Dynamic, typeof(J4JLoggerConfiguration), "config-files/Embedded.json",
            "Container:Logger" ) ]
        [ InlineData( CompositionRootType.Static, typeof(J4JLoggerConfiguration), "config-files/Simple.json", null ) ]
        [ InlineData( CompositionRootType.Static, typeof(DerivedConfiguration), "config-files/Derived.json", null ) ]
        [ InlineData( CompositionRootType.Static, typeof(J4JLoggerConfiguration), "config-files/Embedded.json",
            "Container:Logger" ) ]
        public void Cached( CompositionRootType rootType, Type configType, string configPath, string loggerKey )
        {
            var cached = new J4JCachedLogger();
            cached.SetLoggedType( GetType() );

            var template = "{0} ({1})";

            cached.Verbose<string, string>( template, "Verbose", configPath );
            cached.Warning<string, string>( template, "Warning", configPath );
            cached.Information<string, string>( template, "Information", configPath );
            cached.Debug<string, string>( template, "Debug", configPath );
            cached.Error<string, string>( template, "Error", configPath );
            cached.Fatal<string, string>( template, "Fatal", configPath );
            cached.OutputNextEventToSms().Verbose<string, string>( "{0} ({1})", "Verbose", configPath );

            var compRoot = GetCompositionRoot( rootType, configType, configPath, loggerKey );

            var logger = compRoot.J4JLogger;
            var lastEvent = compRoot.LastEventConfig;

            foreach( var context in cached.Cache )
            {
                if( context.OutputToSms )
                    logger.OutputNextEventToSms();

                if( context.LoggedType == null )
                    logger.ClearLoggedType();
                else logger.SetLoggedType( context.LoggedType );

                foreach( var entry in context.Entries )
                {
                    logger.Write( entry.LogEventLevel, entry.Template, entry.PropertyValues, entry.MemberName,
                        entry.SourcePath, entry.SourceLine );

                    lastEvent.LastLogMessage.Should().Be( format_message( entry.LogEventLevel.ToString() ) );
                }
            }

            string format_message( string prop1 )
            {
                return template.Replace( "{0}", $"\"{prop1}\"" ).Replace( "{1}", $"\"{configPath}\"" );
            }
        }

        private static ICompositionRoot GetCompositionRoot( CompositionRootType rootType, Type configType, string configPath,
            string? loggerKey )
        {
            typeof(IJ4JLoggerConfiguration).IsAssignableFrom( configType ).Should().BeTrue();

            var compRootType = typeof(CompositionRoot<>).MakeGenericType( configType );

            var temp = rootType switch
            {
                CompositionRootType.Dynamic => Activator.CreateInstance(
                    compRootType, configPath, loggerKey ),
                CompositionRootType.Static => Activator.CreateInstance( compRootType ),
                _ => throw new InvalidEnumArgumentException(
                    $"Unsupported {typeof(CompositionRootType)} value '{rootType}'" )
            };

            temp.Should().NotBeNull();

            return (ICompositionRoot) temp!;
        }
    }
}