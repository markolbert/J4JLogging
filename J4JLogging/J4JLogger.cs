#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLogging' is free software: you can redistribute it
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    /// <summary>
    ///     Wrapper for <see cref="Serilog.ILogger" /> which simplifies including calling member
    ///     (e.g., method name) and source code information.
    /// </summary>
    public class J4JLogger : J4JBaseLogger
    {
        private ILogger? _baseLogger;

        protected internal override void ResetBaseLogger()
        {
            _baseLogger = null;
        }

        /// <summary>
        ///     The <see cref="Serilog.ILogger" />  instance that handles the actual logging. Read only.
        /// </summary>
        private ILogger BaseLogger => _baseLogger ??= CreateBaseLogger();

        private ILogger CreateBaseLogger()
        {
            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext();

            var minLevel = LogEventLevel.Fatal;

            foreach( var channel in Channels )
            {
                if( channel.MinimumLevel < minLevel )
                    minLevel = channel.MinimumLevel;

                channel.Configure( loggerConfig.WriteTo );
            }

            SetMinimumLevel(loggerConfig, minLevel);

            var retVal = loggerConfig.CreateLogger();

            if( LoggedType != null )
                retVal.ForContext( LoggedType );

            return retVal;
        }

        protected override void OnLoggedTypeChanged()
        {
            if( LoggedType == null )
                ResetBaseLogger();
            else _baseLogger?.ForContext( LoggedType );
        }

        private static void SetMinimumLevel( LoggerConfiguration config, LogEventLevel minLevel)
        {
            switch (minLevel)
            {
                case LogEventLevel.Debug:
                    config.MinimumLevel.Debug();
                    break;

                case LogEventLevel.Error:
                    config.MinimumLevel.Error();
                    break;

                case LogEventLevel.Fatal:
                    config.MinimumLevel.Fatal();
                    break;

                case LogEventLevel.Information:
                    config.MinimumLevel.Information();
                    break;

                case LogEventLevel.Verbose:
                    config.MinimumLevel.Verbose();
                    break;

                case LogEventLevel.Warning:
                    config.MinimumLevel.Warning();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(minLevel), minLevel, null);
            }
        }

        public List<IChannel> Channels { get; } = new();

        public override bool OutputCache( J4JCachedLogger cachedLogger )
        {
            var curLoggedType = LoggedType;

            foreach( var entry in cachedLogger.Entries )
            {
                if( LoggedType != entry.LoggedType)
                    LoggedType = entry.LoggedType;

                OutputNextToSms = entry.OutputToSms;

                var contextProperties =
                    InitializeContextProperties( entry.MemberName, entry.SourcePath, entry.SourceLine );

                BaseLogger.Write( entry.LogEventLevel, entry.MessageTemplate, entry.PropertyValues );

                DisposeContextProperties( contextProperties );
            }

            cachedLogger.Entries.Clear();

            LoggedType = curLoggedType;

            return true;
        }

        // Initialize the additional LogEvent properties supported by J4JLogger
        private List<IDisposable> InitializeContextProperties( string memberName, string srcPath, int srcLine )
        {
            var retVal = new List<IDisposable>
            {
                LogContext.PushProperty( "SendToSms", OutputNextToSms ),
                LogContext.PushProperty( "MemberName", LoggedType != null ? $"::{memberName}" : "" )
            };

            if( !IncludeSourcePath ) 
                return retVal;

            if( !string.IsNullOrEmpty(SourceRootPath ) )
                srcPath = srcPath.Replace(SourceRootPath, "" );

            retVal.Add( LogContext.PushProperty( "SourceCodeInformation", $"{srcPath} : {srcLine}" ) );

            return retVal;
        }

        // Clear the additional LogEvent properties supported by J4JLogger. This must be done
        // after each LogEvent is processed to comply with Serilog's design.
        private static void DisposeContextProperties( List<IDisposable> contextProperties )
        {
            foreach( var contextProperty in contextProperties )
            {
                contextProperty.Dispose();
            }
        }

        public override void Write(
            LogEventLevel level,
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            var contextProperties = InitializeContextProperties( memberName, srcPath, srcLine );

            BaseLogger.Write( level, template, propertyValues );

            DisposeContextProperties( contextProperties );
        }
    }
}