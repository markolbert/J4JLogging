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

        public static string GetOutputTemplate(bool requiresNewline = false, string coreTemplate = DefaultCoreTemplate )
        {
            var sb = new StringBuilder(coreTemplate);

            AppendEnricher<CallingContextEnricher>(sb);
            AppendEnricher<SmsEnricher>(sb);

            if (requiresNewline)
                sb.Append("{NewLine}");

            sb.Append("{Exception}");

            return sb.ToString();
        }

        private static void AppendEnricher<T>(StringBuilder sb)
            where T : BaseEnricher, new()
        {
            var attr = typeof(T).GetCustomAttribute<J4JEnricherAttribute>( false );
            if( attr == null )
                throw new NullReferenceException(
                    $"BaseEnricher type '{typeof(T)}' is not decorated with a J4JEnricherAttribute");

            sb.Append(" {");
            sb.Append(attr.PropertyName);
            sb.Append("}");
        }

        private readonly List<BaseEnricher> _enrichers = new();

        private LogEventLevel _minLevel;

        public J4JLoggerConfiguration()
        {
            SerilogConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext();

            CallingContextToText = CallingContextEnricher.DefaultConvertToText;
            MinimumLevel = LogEventLevel.Verbose;
        }

        public LoggerConfiguration SerilogConfiguration { get; }
        public ReadOnlyCollection<BaseEnricher> Enrichers => _enrichers.AsReadOnly();

        public Func<Type?, string, int, string, string> CallingContextToText { get; set; }

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

        public J4JLoggerConfiguration AddEnricher<T>()
            where T: BaseEnricher, new()
        {
            if( !_enrichers.Any( x => x is T ) )
                _enrichers.Add(new T());

            return this;
        }

        public J4JLogger CreateLogger() => new( this );
    }
}