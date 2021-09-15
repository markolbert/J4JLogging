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
using System.Reflection;
using System.Text;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JLoggerConfiguration
    {
        public const string DefaultCoreTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}";

        private readonly CallingContextEnricher _ccEnricher = new();
        private List<J4JEnricher> _enrichers;
        private LogEventLevel _minLevel;

        public J4JLoggerConfiguration()
        {
            SerilogConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext();

            _enrichers = new List<J4JEnricher>() { _ccEnricher };

            MinimumLevel = LogEventLevel.Verbose;
        }

        public LoggerConfiguration SerilogConfiguration { get; }
        
        public ReadOnlyCollection<J4JEnricher> Enrichers => _enrichers.AsReadOnly();

        public void AddSmsSink( SmsSink sink, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose )
        {
            // we only need to add the SmsEnricher once to support whatever SMS sinks may be specified
            var enricher = new SmsEnricher();

            if( !_enrichers.Any( x => x.EnricherID.Equals( enricher.EnricherID, StringComparison.OrdinalIgnoreCase ) ) )
                _enrichers.Add( enricher );

            SerilogConfiguration.WriteTo
                .Logger( lc =>
                    lc.Filter
                        .ByIncludingOnly( "SendToSms" )
                        .WriteTo.Sink( sink, restrictedToMinimumLevel )
                );
        }

        public Func<Type?, string, int, string, string> FilePathTrimmer
        {
            get => _ccEnricher.FilePathTrimmer;
            set => _ccEnricher.FilePathTrimmer = value;
        }

        public LogEventLevel MinimumLevel
        {
            get => _minLevel;

            set
            {
                _minLevel = value;

                switch( _minLevel )
                {
                    case LogEventLevel.Debug:
                        SerilogConfiguration.MinimumLevel.Debug();
                        break;

                    case LogEventLevel.Error:
                        SerilogConfiguration.MinimumLevel.Error();
                        break;

                    case LogEventLevel.Fatal:
                        SerilogConfiguration.MinimumLevel.Fatal();
                        break;

                    case LogEventLevel.Information:
                        SerilogConfiguration.MinimumLevel.Information();
                        break;

                    case LogEventLevel.Verbose:
                        SerilogConfiguration.MinimumLevel.Verbose();
                        break;

                    case LogEventLevel.Warning:
                        SerilogConfiguration.MinimumLevel.Warning();
                        break;

                    default:
                        throw new InvalidEnumArgumentException( $"Unsupported LogEventLevel '{_minLevel}'" );
                }
            }
        }

        public string GetOutputTemplate(bool requiresNewline = false, string coreTemplate = DefaultCoreTemplate)
        {
            var sb = new StringBuilder(coreTemplate);

            foreach (var enricher in _enrichers)
            {
                sb.Append(" {");
                sb.Append(enricher.PropertyName);
                sb.Append("}");
            }

            if (requiresNewline)
                sb.Append("{NewLine}");

            sb.Append("{Exception}");

            return sb.ToString();
        }

        public J4JLogger CreateLogger()
        {
            // eliminate any duplicate enrichers
            _enrichers = _enrichers.Distinct( J4JEnricher.DefaultComparer ).ToList();

            foreach( var enricher in _enrichers )
            {
                SerilogConfiguration.Enrich.With( enricher );
            }

            return new(this);
        }
    }
}