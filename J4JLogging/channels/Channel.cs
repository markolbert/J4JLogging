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
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the base configuration for a log channel
    public abstract class Channel : IChannel
    {
        private J4JBaseLogger? _logger;
        private bool _loggerMustBeUpdated = false;

        private Func<bool>? _globalInclSrcPath;
        private Func<string?>? _globalSrcPath;
        private Func<string>? _globalOutputTemplate;
        private Func<bool>? _globalRequireNewLine;
        private Func<LogEventLevel>? _globalMinLevel;

        private bool? _inclSrcPath;
        private string? _srcPath;
        private string? _outputTemplate;
        private bool? _requireNewLine;
        private LogEventLevel? _minLevel;

        protected Channel()
        {
        }

        public void SetAssociatedLogger(J4JBaseLogger? logger)
        {
            _logger = logger;

            _globalInclSrcPath = _logger?.GetGlobalAccessor(x => x.IncludeSourcePath);
            _globalSrcPath = _logger?.GetGlobalAccessor(x => x.SourceRootPath);
            _globalOutputTemplate = _logger?.GetGlobalAccessor(x => x.OutputTemplate);
            _globalRequireNewLine = _logger?.GetGlobalAccessor(x => x.RequireNewLine);
            _globalMinLevel = _logger?.GetGlobalAccessor(x => x.MinimumLevel);

            if (!_loggerMustBeUpdated)
                return;

            _logger?.ResetBaseLogger();
            _loggerMustBeUpdated = false;
        }

        public bool IncludeSourcePath
        {
            get => _inclSrcPath ?? _globalInclSrcPath?.Invoke() ?? false;
            set => SetPropertyAndNotifyLogger(ref _inclSrcPath, value);
        }

        public void ResetIncludeSourcePath() => _inclSrcPath = null;

        public string? SourceRootPath
        {
            get => _srcPath ?? _globalSrcPath?.Invoke();
            set => _srcPath = value;
        }

        public string OutputTemplate
        {
            get => _outputTemplate ?? _globalOutputTemplate?.Invoke() ?? J4JBaseLogger.DefaultOutputTemplate;
            set => SetPropertyAndNotifyLogger(ref _outputTemplate, value);
        }

        public void ResetOutputTemplate() => _outputTemplate = null;

        public bool RequireNewLine
        {
            get => _requireNewLine ?? _globalRequireNewLine?.Invoke() ?? false;
            set => SetPropertyAndNotifyLogger(ref _requireNewLine, value);
        }

        public void ResetRequireNewLine() => _requireNewLine = null;

        public LogEventLevel MinimumLevel
        {
            get => _minLevel ?? _globalMinLevel?.Invoke() ?? LogEventLevel.Verbose;
            set => SetPropertyAndNotifyLogger(ref _minLevel, value);
        }

        public void ResetMinimumLevel() => _minLevel = null;

        public string EnrichedMessageTemplate
        {
            get
            {
                var sb = new StringBuilder(OutputTemplate);

                if (_logger?.LoggedType != null)
                    sb.Append(" {SourceContext}{MemberName}");

                if (IncludeSourcePath)
                    sb.Append(" {SourceCodeInformation}");

                if (RequireNewLine)
                    sb.Append("{NewLine}");

                return sb.ToString();
            }
        }

        public void ResetToGlobal()
        {
            ResetIncludeSourcePath();
            ResetMinimumLevel();
            ResetOutputTemplate();
            ResetRequireNewLine();
            SourceRootPath = null;
        }

        protected void SetPropertyAndNotifyLogger<TProp>(ref TProp field, TProp value)
        {
            var changed = !EqualityComparer<TProp>.Default.Equals(field, value);

            field = value;

            if (changed)
            {
                if (_logger == null)
                    _loggerMustBeUpdated = true;
                else
                {
                    _logger.ResetBaseLogger();
                    _loggerMustBeUpdated = false;
                }
            }
        }

        public abstract LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );
    }
}