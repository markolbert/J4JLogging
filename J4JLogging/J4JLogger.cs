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
        private readonly SmsEnricher _smsEnricher = new();
        private readonly SourceFileEnricher _srcFileEnricher = new();
        private readonly CallingMemberEnricher _callingMemberEnricher = new();
        private readonly LineNumberEnricher _lineNumEnricher = new();
        private readonly LoggedTypeEnricher _loggedTypeEnricher = new();

        private readonly List<IDisposable> _pushedProperties = new();

        public J4JLogger(
            ILogger seriLogger
        )
        {
            Serilogger = seriLogger;
        }

        internal J4JLogger(
            J4JLoggerConfiguration loggerConfig
        )
            : this( loggerConfig.SerilogConfiguration.CreateLogger() )
        {
        }

        public ILogger? Serilogger { get; }
        
        protected override void OnLoggedTypeChanged()
        {
            if( LoggedType != null )
                Serilogger!.ForContext( LoggedType );
        }

        public override bool OutputCache( J4JCachedLogger cachedLogger )
        {
            var initialLoggedType = LoggedType;

            foreach( var entry in cachedLogger.Entries )
            {
                if( entry.LoggedType != null && LoggedType != entry.LoggedType )
                    SetLoggedType( entry.LoggedType );

                SmsHandling = entry.SmsHandling;

                Serilogger!.Write( entry.LogEventLevel, entry.MessageTemplate, entry.PropertyValues );
            }

            if( initialLoggedType != null )
                SetLoggedType( initialLoggedType );

            cachedLogger.Entries.Clear();

            return true;
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
            _callingMemberEnricher.CallingMemberName = memberName;
            _srcFileEnricher.SourceFilePath = srcPath;
            _lineNumEnricher.LineNumber = srcLine;
            _smsEnricher.SendNextToSms = SmsHandling != SmsHandling.DoNotSend;
            _loggedTypeEnricher.LoggedType = LoggedType;

            PushToLogContext();

            Serilogger!.Write( level, template, propertyValues );

            DisposeFromLogContext();

            _smsEnricher.SendNextToSms = SmsHandling == SmsHandling.SendUntilReset;
        }

        private void PushToLogContext()
        {
            _pushedProperties.Clear();

            foreach( var enricher in new BaseEnricher[]
                { _srcFileEnricher, _callingMemberEnricher, _lineNumEnricher, _smsEnricher, _loggedTypeEnricher } )
            {
                if( !enricher.EnrichContext )
                    continue;

                _pushedProperties.Add( LogContext.PushProperty( enricher.PropertyName, enricher.GetValue() ) );
            }
        }

        private void DisposeFromLogContext()
        {
            foreach (var disposable in _pushedProperties)
            {
                disposable.Dispose();
            }

            _pushedProperties.Clear();
        }
    }
}