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
using System.Text;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace J4JSoftware.Logging
{
    public record ChannelParameters : IChannelParameters
    {
        private readonly bool _inclSourcePath;
        private readonly string? _srcRootPath;
        private readonly bool _multiLineEvents;
        private readonly string _outputTemplate = J4JBaseLogger.DefaultOutputTemplate;
        private readonly bool _requireNewLine;
        private readonly LogEventLevel _minLevel = LogEventLevel.Verbose;

        public ChannelParameters(
            J4JBaseLogger? logger
        )
        {
            Logger = logger;
        }

        protected J4JBaseLogger? Logger { get; }

        public bool IncludeSourcePath
        {
            get => _inclSourcePath;
            init => SetProperty( ref _inclSourcePath, value );
        }

        public string? SourceRootPath
        {
            get => _srcRootPath;
            init => SetProperty(ref _srcRootPath, value);
        }

        public bool MultiLineEvents
        {
            get => _multiLineEvents;
            init => SetProperty(ref _multiLineEvents, value);
        }

        public string OutputTemplate
        {
            get => _outputTemplate;
            init => SetProperty(ref _outputTemplate, value);
        }

        public bool RequireNewLine
        {
            get => _requireNewLine;
            init => SetProperty(ref _requireNewLine, value);
        }

        public LogEventLevel MinimumLevel
        {
            get => _minLevel;
            init => SetProperty( ref _minLevel, value );
        }

        public string EnrichedMessageTemplate
        {
            get
            {
                var sb = new StringBuilder( OutputTemplate );

                if( Logger?.LoggedType != null )
                    sb.Append( " {SourceContext}{MemberName}" );

                if( IncludeSourcePath )
                    sb.Append( " {SourceCodeInformation}" );

                if( RequireNewLine )
                    sb.Append( "{NewLine}" );

                return sb.ToString();
            }
        }

        protected void SetProperty<T>( ref T field, T value )
        {
            var changed = !EqualityComparer<T>.Default.Equals( field, value );

            field = value;

            if( changed )
                Logger?.ResetBaseLogger();
        }
    }
}