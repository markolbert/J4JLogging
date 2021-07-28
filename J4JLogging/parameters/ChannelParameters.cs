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
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace J4JSoftware.Logging
{
    public class ChannelParameters : IChannelParameters
    {
        private readonly Func<bool> _globalInclSrcPath;
        private readonly Func<string?> _globalSrcPath;
        private readonly Func<string> _globalOutputTemplate;
        private readonly Func<bool> _globalRequireNewLine;
        private readonly Func<LogEventLevel> _globalMinLevel;

        private bool? _inclSrcPath;
        private string? _srcPath;
        private string? _outputTemplate;
        private bool? _requireNewLine;
        private LogEventLevel? _minLevel;

        public ChannelParameters(
            J4JBaseLogger logger
        )
        {
            Logger = logger;

            _globalInclSrcPath = logger.GetGlobalAccessor( x => ( (IChannelParameters) x ).SourcePathIncluded );
            _globalSrcPath = logger.GetGlobalAccessor( x => x.SourceRootPath );
            _globalOutputTemplate = logger.GetGlobalAccessor( x => x.OutputTemplate );
            _globalRequireNewLine = logger.GetGlobalAccessor( x => x.RequireNewLine );
            _globalMinLevel = logger.GetGlobalAccessor( x => x.MinimumLevel );
        }

        protected J4JBaseLogger? Logger { get; }

        public bool SourcePathIncluded
        {
            get => _inclSrcPath ?? _globalInclSrcPath();
            internal set => SetPropertyAndNotifyLogger( ref _inclSrcPath, value );
        }

        public void ResetIncludeSourcePath() => SetPropertyAndNotifyLogger( ref _inclSrcPath, new bool?() );

        public string? SourceRootPath
        {
            get => _srcPath ?? _globalSrcPath();
            internal set => _srcPath = value;
        }

        public void ResetSourceRootPath() => _srcPath = null;

        public string OutputTemplate
        {
            get => _outputTemplate ?? _globalOutputTemplate();
            internal set => SetPropertyAndNotifyLogger( ref _outputTemplate, value );
        }

        public void ResetOutputTemplate() => SetPropertyAndNotifyLogger( ref _outputTemplate, null );

        public bool RequireNewLine
        {
            get => _requireNewLine ?? _globalRequireNewLine();
            internal set => SetPropertyAndNotifyLogger( ref _requireNewLine, value );
        }

        public void ResetRequireNewLine() => SetPropertyAndNotifyLogger( ref _requireNewLine, new bool?() );

        public LogEventLevel MinimumLevel
        {
            get => _minLevel ?? _globalMinLevel();

            // set accessor needs to be public to support Twilio (and other downstream libraries)
            set => SetPropertyAndNotifyLogger( ref _minLevel, value );
        }

        public void ResetMinimumLevel() => SetPropertyAndNotifyLogger( ref _minLevel, new LogEventLevel?() );

        public virtual void Reset()
        {
            ResetIncludeSourcePath();
            ResetSourceRootPath();
            ResetOutputTemplate();
            ResetRequireNewLine();
            ResetMinimumLevel();
        }

        public string EnrichedMessageTemplate
        {
            get
            {
                var sb = new StringBuilder( OutputTemplate );

                if( Logger?.LoggedType != null )
                    sb.Append( " {SourceContext}{MemberName}" );

                if( SourcePathIncluded )
                    sb.Append( " {SourceCodeInformation}" );

                if( RequireNewLine )
                    sb.Append( "{NewLine}" );

                return sb.ToString();
            }
        }

        protected void SetPropertyAndNotifyLogger<TProp>( ref TProp field, TProp value )
        {
            var changed = !EqualityComparer<TProp>.Default.Equals( field, value );

            field = value;

            if( changed )
                Logger?.ResetBaseLogger();
        }
    }
}