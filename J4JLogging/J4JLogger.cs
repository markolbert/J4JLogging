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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    /// <summary>
    ///     Wrapper for <see cref="Serilog.ILogger" /> which simplifies including calling member
    ///     (e.g., method name) and source code information.
    /// </summary>
    public class J4JLogger : J4JBaseLogger
    {
        public J4JLogger( 
            LoggerConfiguration loggerConfig
            )
        {
            LoggerConfiguration = loggerConfig;
        }

        public J4JLogger( 
            LogEventLevel minimumLevel = LogEventLevel.Verbose
            )
        {
            LoggerConfiguration = new LoggerConfiguration();

            switch( minimumLevel )
            {
                case LogEventLevel.Debug:
                    LoggerConfiguration.MinimumLevel.Debug();
                    break;

                case LogEventLevel.Error:
                    LoggerConfiguration.MinimumLevel.Error();
                    break;

                case LogEventLevel.Fatal:
                    LoggerConfiguration.MinimumLevel.Fatal();
                    break;

                case LogEventLevel.Information:
                    LoggerConfiguration.MinimumLevel.Information();
                    break;

                case LogEventLevel.Verbose:
                    LoggerConfiguration.MinimumLevel.Verbose();
                    break;

                case LogEventLevel.Warning:
                    LoggerConfiguration.MinimumLevel.Warning();
                    break;

                default:
                    throw new InvalidEnumArgumentException( $"Unsupported LogEventLevel '{minimumLevel}'" );
            }
        }

        public LoggerConfiguration LoggerConfiguration { get; }
        public ILogger? Serilogger { get; private set; }
        public bool Built => Serilogger != null;
        public List<BaseEnricher> J4JEnrichers { get; } = new();
        public void Create()
        {
            Serilogger = LoggerConfiguration.CreateLogger();
        }

        protected override void OnLoggedTypeChanged()
        {
            if( J4JEnrichers.FirstOrDefault( x => x is LoggedTypeEnricher ) is LoggedTypeEnricher enricher )
                enricher.LoggedTypeName = LoggedType?.Name;
        }

        public override bool OutputCache( J4JCachedLogger cachedLogger )
        {
            if( !Built )
                return false;

            foreach( var entry in cachedLogger.Entries )
            {
                if( LoggedType != entry.LoggedType)
                    LoggedType = entry.LoggedType;

                SmsHandling = entry.SmsHandling;

                Serilogger!.Write( entry.LogEventLevel, entry.MessageTemplate, entry.PropertyValues );
            }

            cachedLogger.Entries.Clear();

            return true;
        }

        //// Initialize the additional LogEvent properties supported by J4JLogger
        //private List<IDisposable> InitializeContextProperties( string memberName, string srcPath, int srcLine )
        //{
        //    var retVal = new List<IDisposable>
        //    {
        //        LogContext.PushProperty( "SendToSms", OutputNextToSms ),
        //        LogContext.PushProperty( "MemberName", LoggedType != null ? $"::{memberName}" : "" )
        //    };

        //    if( !IncludeSourcePath ) 
        //        return retVal;

        //    if( !string.IsNullOrEmpty(SourceRootPath ) )
        //        srcPath = srcPath.Replace(SourceRootPath, "" );

        //    retVal.Add( LogContext.PushProperty( "SourceCodeInformation", $"{srcPath} : {srcLine}" ) );

        //    return retVal;
        //}

        //// Clear the additional LogEvent properties supported by J4JLogger. This must be done
        //// after each LogEvent is processed to comply with Serilog's design.
        //private static void DisposeContextProperties( List<IDisposable> contextProperties )
        //{
        //    foreach( var contextProperty in contextProperties )
        //    {
        //        contextProperty.Dispose();
        //    }
        //}

        public override void Write(
            LogEventLevel level,
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            if( !Built )
                return;

            if( J4JEnrichers.FirstOrDefault( x => x is CallingMemberEnricher ) 
                is CallingMemberEnricher callingMemberEnricher )
                callingMemberEnricher.CallingMemberName = memberName;

            if( J4JEnrichers.FirstOrDefault( x => x is SourceFileEnricher )
                is SourceFileEnricher sourceEnricher )
                sourceEnricher.SourceFilePath = srcPath;

            if( J4JEnrichers.FirstOrDefault( x => x is LineNumberEnricher )
                is LineNumberEnricher lineNumEnricher )
                lineNumEnricher.LineNumber = srcLine;

            var smsEnricher = J4JEnrichers.FirstOrDefault( x => x is SmsEnricher) as SmsEnricher;

            if( smsEnricher != null )
                smsEnricher.SendNextToSms = SmsHandling != SmsHandling.DoNotSend;

            //var contextProperties = InitializeContextProperties( memberName, srcPath, srcLine );

            Serilogger!.Write( level, template, propertyValues );

            if( smsEnricher != null )
                smsEnricher.SendNextToSms = SmsHandling == SmsHandling.SendUntilReset;

            //DisposeContextProperties( contextProperties );
        }
    }
}