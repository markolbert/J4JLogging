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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    /// <summary>
    ///     Wrapper for <see cref="Serilog.ILogger" /> which simplifies including calling member
    ///     (e.g., method name) and source code information.
    /// </summary>
    public class J4JLogger : J4JBaseLogger
    {
        private record MessageTemplate( bool RequiresNewline, string Template );

        private readonly string _coreTemplate;
        private readonly List<MessageTemplate> _msgTemplates = new();

        public J4JLogger( 
            string coreTemplate = DefaultCoreTemplate,
            LogEventLevel minimumLevel = LogEventLevel.Verbose
            )
        {
            _coreTemplate = coreTemplate;
            MessageTemplateManager = new MessageTemplateManager();

            LoggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext();

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

        public bool Built => Serilogger != null;
        public ILogger? Serilogger { get; private set; }
        
        public void Create()
        {
            Serilogger = LoggerConfiguration.CreateLogger();
        }

        public IMessageTemplateManager MessageTemplateManager { get; }

        public string GetOutputTemplate(bool requiresNewline = false)
        {
            var retVal = _msgTemplates.FirstOrDefault( x => x.RequiresNewline == requiresNewline );
            if( retVal != null )
                return retVal.Template;

            var sb = new StringBuilder( _coreTemplate );

            AppendEnricher<LoggedTypeEnricher>( sb );
            AppendEnricher<CallingMemberEnricher>(sb);
            AppendEnricher<SourceFileEnricher>(sb);
            AppendEnricher<LineNumberEnricher>(sb);
            AppendEnricher<SmsEnricher>(sb);

            if( requiresNewline )
                sb.Append( "{NewLine}" );

            sb.Append( "{Exception}" );

            retVal = new MessageTemplate( requiresNewline, sb.ToString() );
            _msgTemplates.Add( retVal );

            return retVal.Template;
        }

        private void AppendEnricher<T>( StringBuilder sb )
            where T: BaseEnricher, new()
        {
            if( MessageTemplateManager.GetEnricher<T>() is not { } enricher )
                return;

            sb.Append( " {" );
            sb.Append( enricher.PropertyName );
            sb.Append( "}" );
        }

        protected override void OnLoggedTypeChanged()
        {
            if( MessageTemplateManager.GetEnricher<LoggedTypeEnricher>() is { } enricher )
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

            if( MessageTemplateManager.GetEnricher<CallingMemberEnricher>() is { } callingMemberEnricher )
                callingMemberEnricher.CallingMemberName = memberName;

            if (MessageTemplateManager.GetEnricher<SourceFileEnricher>() is { } sourceEnricher)
                sourceEnricher.SourceFilePath = srcPath;

            if (MessageTemplateManager.GetEnricher<LineNumberEnricher>() is { } lineNumEnricher)
                lineNumEnricher.LineNumber = srcLine;

            var smsEnricher = MessageTemplateManager.GetEnricher<SmsEnricher>();

            if( smsEnricher != null )
                smsEnricher.SendNextToSms = SmsHandling != SmsHandling.DoNotSend;

            MessageTemplateManager.PushToLogContext();

            Serilogger!.Write( level, template, propertyValues );

            MessageTemplateManager.DisposeFromLogContext();

            if( smsEnricher != null )
                smsEnricher.SendNextToSms = SmsHandling == SmsHandling.SendUntilReset;
        }
    }
}