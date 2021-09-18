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
using System.Linq;
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
        private readonly List<J4JEnricher> _enrichers;
        private readonly List<IDisposable> _pushedProperties = new();

        public J4JLogger(
            ILogger seriLogger,
            NetEventSink? netEventSink = null,
            params J4JEnricher[] enrichers
        )
        :base(netEventSink)
        {
            Serilogger = seriLogger;
            _enrichers = enrichers.ToList();
        }

        internal J4JLogger(
            J4JLoggerConfiguration loggerConfig
        )
            : this(
                loggerConfig.SerilogConfiguration.CreateLogger(),
                loggerConfig.NetEventSink,
                loggerConfig.Enrichers.ToArray()
            )
        {
            if( _enrichers.FirstOrDefault( x => x is CallingContextEnricher )
                is CallingContextEnricher callingEnricher )
                callingEnricher.FilePathTrimmer = loggerConfig.FilePathTrimmer;
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
            if( _enrichers.FirstOrDefault( x => x is CallingContextEnricher )
                is CallingContextEnricher callingEnricher )
            {
                callingEnricher.LoggedType = LoggedType;
                callingEnricher.CallingMemberName = memberName;
                callingEnricher.LineNumber = srcLine;
                callingEnricher.SourceFilePath = srcPath;
            }

            var smsEnricher = _enrichers.FirstOrDefault(x => x is SmsEnricher) as SmsEnricher;
            if( smsEnricher != null )
                smsEnricher.SendNextToSms = SmsHandling != SmsHandling.DoNotSend;

            PushToLogContext();

            Serilogger!.Write( level, template, propertyValues );

            DisposeFromLogContext();

            if( smsEnricher != null )
                smsEnricher.SendNextToSms = SmsHandling == SmsHandling.SendUntilReset;
        }

        private void PushToLogContext()
        {
            _pushedProperties.Clear();

            foreach( var enricher in _enrichers )
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