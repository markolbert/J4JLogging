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
    public class J4JLogger : IJ4JLogger
    {
        public const string DefaultOutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}";

        private readonly Dictionary<Type, IChannelConfig> _channels = new();

        private ILogger? _baseLogger;
        private bool _sendToSms;

        public J4JLogger()
        {
        }

        /// <summary>
        ///     The <see cref="Serilog.ILogger" />  instance that handles the actual logging. Read only.
        /// </summary>
        protected internal ILogger BaseLogger
        {
            get
            {
                if( _baseLogger == null )
                    _baseLogger = CreateBaseLogger();

                return _baseLogger;
            }

            internal set => _baseLogger = value;
        }

        internal void ResetBaseLogger() => _baseLogger = null;

        private ILogger CreateBaseLogger()
        {
            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext();

            var minLevel = LogEventLevel.Fatal;

            foreach (var kvp in _channels)
            {
                if (kvp.Value.MinimumLevel < minLevel)
                    minLevel = kvp.Value.MinimumLevel;

                kvp.Value.Configure(loggerConfig.WriteTo);
            }

            SetMinimumLevel(loggerConfig, minLevel);

            return loggerConfig.CreateLogger();
        }

        private static LoggerConfiguration SetMinimumLevel( LoggerConfiguration config, LogEventLevel minLevel)
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

            return config;
        }

        public Type? LoggedType { get; internal set; }
        public bool IncludeSourcePath { get; internal set; } = true;
        public string? SourceRootPath { get; internal set; }
        public bool MultiLineEvents { get; internal set; }
        public string OutputTemplate { get; internal set; } = DefaultOutputTemplate;
        public bool RequireNewline { get; internal set; }

        public List<ChannelConfigNG> ChannelsInternal { get; } = new();
        public ReadOnlyCollection<ChannelConfigNG> Channels => ChannelsInternal.AsReadOnly();

        public IJ4JLogger AddOutputChannel<TChannel>( TChannel channelConfig )
            where TChannel : IChannelConfig
        {
            var channelType = typeof( TChannel );

            if( _channels.ContainsKey( channelType ) )
                return this;

            _channels.Add( channelType, channelConfig  );
            _baseLogger = null;

            return this;
        }

        public IJ4JLogger RemoveOutputChannel<TChannel>()
            where TChannel : IChannelConfig
        {
            var channelType = typeof( TChannel );

            if( _channels.ContainsKey( channelType ) )
                _channels.Remove( channelType );

            return this;
        }

        public bool OutputCache( J4JLoggerCache cache )
        {
            var curLoggedType = LoggedType;

            foreach( var curContext in cache )
            {
                LoggedType = curContext.LoggedType ?? null;

                _sendToSms = curContext.OutputToSms;

                foreach( var entry in curContext.Entries )
                {
                    var contextProperties =
                        InitializeContextProperties( entry.MemberName, entry.SourcePath, entry.SourceLine );

                    BaseLogger.Write( entry.LogEventLevel, entry.Template, entry.PropertyValues );

                    DisposeContextProperties( contextProperties );
                }
            }

            cache.Clear();

            LoggedType = curLoggedType ?? curLoggedType;

            return true;
        }

        // Force the next LogEvent to be processed by any IPostProcess-implementing channels
        public IJ4JLogger OutputNextEventToSms()
        {
            _sendToSms = true;
            return this;
        }

        // Initialize the additional LogEvent properties supported by IJ4JLogger
        protected List<IDisposable> InitializeContextProperties( string memberName, string srcPath, int srcLine )
        {
            var retVal = new List<IDisposable>
            {
                LogContext.PushProperty( "SendToSms", _sendToSms ),
                LogContext.PushProperty( "MemberName", LoggedType != null ? $"::{memberName}" : "" )
            };

            if( !IncludeSourcePath ) 
                return retVal;

            if( !string.IsNullOrEmpty( SourceRootPath ) )
                srcPath = srcPath.Replace( SourceRootPath, "" );

            retVal.Add( LogContext.PushProperty( "SourceCodeInformation", $"{srcPath} : {srcLine}" ) );

            return retVal;
        }

        // Clear the additional LogEvent properties supported by IJ4JLogger. This must be done
        // after each LogEvent is processed to comply with Serilog's design.
        protected void DisposeContextProperties( List<IDisposable> contextProperties )
        {
            foreach( var contextProperty in contextProperties ) contextProperty.Dispose();

            _sendToSms = false;
        }

        #region Write() methods

        /// <summary>
        ///     Writes an event to ILogger, including the calling member and calling type, and
        ///     optionally the source code file and line number of the method.
        /// </summary>
        /// <param name="level">the <see cref="LogEventLevel" /> of the event</param>
        /// <param name="template">
        ///     the <see cref="Serilog" /> message template for constructing the
        ///     log message. Note that this will be included within the output template as the
        ///     value of the Message parameter.
        /// </param>
        /// <param name="memberName">
        ///     the name of the method calling the Write method (supplied
        ///     automatically by the compiler)
        /// </param>
        /// <param name="srcPath">
        ///     the path to the source code file in which the method calling the
        ///     Write method is defined (supplied automatically by the compiler)
        /// </param>
        /// <param name="srcLine">
        ///     the line number within the source code file at which the method calling the
        ///     Write method is defined (supplied automatically by the compiler)
        /// </param>
        public virtual void Write(
            LogEventLevel level,
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            var contextProperties = InitializeContextProperties( memberName, srcPath, srcLine );

            BaseLogger.Write( level, template );

            DisposeContextProperties( contextProperties );
        }

        /// <summary>
        ///     Writes an event to ILogger, including the calling member and calling type, and
        ///     optionally the source code file and line number of the method.
        ///     This overload
        ///     lets you pass a parameter in to the <see cref="Serilog" /> logging system to
        ///     be incorporated into the supplied message template./>
        /// </summary>
        /// <typeparam name="T0">the Type of the supplied propertyValue</typeparam>
        /// <param name="level">the <see cref="LogEventLevel" /> of the event</param>
        /// <param name="template">
        ///     the <see cref="MessageTemplate" /> message template for constructing the
        ///     log message. Note that this will be included within the output template as the
        ///     value of the Message parameter.
        /// </param>
        /// <param name="propertyValue">
        ///     the parameter value to be incorporated into the
        ///     supplied message template (see the <see cref="Serilog" /> documentation for how
        ///     the value is matched to the token in the message template)
        /// </param>
        /// <param name="memberName">
        ///     the name of the method calling the Write method (supplied
        ///     automatically by the compiler)
        /// </param>
        /// <param name="srcPath">
        ///     the path to the source code file in which the method calling the
        ///     Write method is defined (supplied automatically by the compiler)
        /// </param>
        /// <param name="srcLine">
        ///     the line number within the source code file at which the method calling the
        ///     Write method is defined (supplied automatically by the compiler)
        /// </param>
        public virtual void Write<T0>(
            LogEventLevel level,
            string template,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            var contextProperties = InitializeContextProperties( memberName, srcPath, srcLine );

            BaseLogger.Write( level, template, propertyValue );

            DisposeContextProperties( contextProperties );
        }

        /// <summary>
        ///     Writes an event to ILogger, including the calling member and calling type, and
        ///     optionally the source code file and line number of the method.
        ///     This overload
        ///     lets you pass two parameters in to the <see cref="Serilog" /> logging system to
        ///     be incorporated into the supplied message template./>
        /// </summary>
        /// <typeparam name="T0">the Type of the first supplied propertyValue</typeparam>
        /// <typeparam name="T1">the Type of the second supplied propertyValue</typeparam>
        /// <param name="level">the <see cref="LogEventLevel" /> of the event</param>
        /// <param name="template">
        ///     the <see cref="MessageTemplate" /> message template for constructing the
        ///     log message. Note that this will be included within the output template as the
        ///     value of the Message parameter.
        /// </param>
        /// <param name="propertyValue0">
        ///     the first parameter value to be incorporated into the
        ///     supplied message template (see the <see cref="Serilog" /> documentation for how
        ///     the value is matched to the token in the message template)
        /// </param>
        /// <param name="propertyValue1">
        ///     the second parameter value to be incorporated into the
        ///     supplied message template (see the <see cref="Serilog" /> documentation for how
        ///     the value is matched to the token in the message template)
        /// </param>
        /// <param name="memberName">
        ///     the name of the method calling the Write method (supplied
        ///     automatically by the compiler)
        /// </param>
        /// <param name="srcPath">
        ///     the path to the source code file in which the method calling the
        ///     Write method is defined (supplied automatically by the compiler)
        /// </param>
        /// <param name="srcLine">
        ///     the line number within the source code file at which the method calling the
        ///     Write method is defined (supplied automatically by the compiler)
        /// </param>
        public virtual void Write<T0, T1>(
            LogEventLevel level,
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            var contextProperties = InitializeContextProperties( memberName, srcPath, srcLine );

            BaseLogger.Write( level, template, propertyValue0, propertyValue1 );

            DisposeContextProperties( contextProperties );
        }

        public virtual void Write<T0, T1, T2>(
            LogEventLevel level,
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            var contextProperties = InitializeContextProperties( memberName, srcPath, srcLine );

            BaseLogger.Write( level, template, propertyValue0, propertyValue1, propertyValue2 );

            DisposeContextProperties( contextProperties );
        }

        public virtual void Write(
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

        #endregion

        #region Debug methods

        public void Debug(
            string messageTemplate,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Debug, messageTemplate, memberName, srcPath, srcLine );
        }

        public void Debug<T0>(
            string messageTemplate,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Debug, messageTemplate, propertyValue, memberName, srcPath, srcLine );
        }

        public void Debug<T0, T1>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1, memberName, srcPath,
                srcLine );
        }

        public void Debug<T0, T1, T2>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1, propertyValue2,
                memberName, srcPath, srcLine );
        }

        public void Debug(
            string messageTemplate,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Debug, messageTemplate, propertyValues, memberName, srcPath, srcLine );
        }

        #endregion

        #region Error() methods

        public void Error(
            string messageTemplate,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Error, messageTemplate, memberName, srcPath, srcLine );
        }

        public void Error<T0>(
            string messageTemplate,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Error, messageTemplate, propertyValue, memberName, srcPath, srcLine );
        }

        public void Error<T0, T1>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1, memberName, srcPath,
                srcLine );
        }

        public void Error<T0, T1, T2>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1, propertyValue2,
                memberName, srcPath, srcLine );
        }

        public void Error(
            string messageTemplate,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Error, messageTemplate, propertyValues, memberName, srcPath, srcLine );
        }

        #endregion

        #region Fatal() methods

        public void Fatal(
            string messageTemplate,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Fatal, messageTemplate, memberName, srcPath, srcLine );
        }

        public void Fatal<T0>(
            string messageTemplate,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Fatal, messageTemplate, propertyValue, memberName, srcPath, srcLine );
        }

        public void Fatal<T0, T1>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1, memberName, srcPath,
                srcLine );
        }

        public void Fatal<T0, T1, T2>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1, propertyValue2,
                memberName, srcPath, srcLine );
        }

        public void Fatal(
            string messageTemplate,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Fatal, messageTemplate, propertyValues, memberName, srcPath, srcLine );
        }

        #endregion

        #region Information() methods

        public void Information(
            string messageTemplate,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Information, messageTemplate, memberName, srcPath, srcLine );
        }

        public void Information<T0>(
            string messageTemplate,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Information, messageTemplate, propertyValue, memberName, srcPath, srcLine );
        }

        public void Information<T0, T1>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1, memberName,
                srcPath,
                srcLine );
        }

        public void Information<T0, T1, T2>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1,
                propertyValue2,
                memberName, srcPath, srcLine );
        }

        public void Information(
            string messageTemplate,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Information, messageTemplate, propertyValues, memberName, srcPath, srcLine );
        }

        #endregion

        #region Verbose() methods

        public void Verbose(
            string messageTemplate,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Verbose, messageTemplate, memberName, srcPath, srcLine );
        }

        public void Verbose<T0>(
            string messageTemplate,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Verbose, messageTemplate, propertyValue, memberName, srcPath, srcLine );
        }

        public void Verbose<T0, T1>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1, memberName, srcPath,
                srcLine );
        }

        public void Verbose<T0, T1, T2>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1, propertyValue2,
                memberName, srcPath, srcLine );
        }

        public void Verbose(
            string messageTemplate,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Verbose, messageTemplate, propertyValues, memberName, srcPath, srcLine );
        }

        #endregion

        #region Warning() methods

        public void Warning(
            string messageTemplate,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Warning, messageTemplate, memberName, srcPath, srcLine );
        }

        public void Warning<T0>(
            string messageTemplate,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Warning, messageTemplate, propertyValue, memberName, srcPath, srcLine );
        }

        public void Warning<T0, T1>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1, memberName, srcPath,
                srcLine );
        }

        public void Warning<T0, T1, T2>(
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1, propertyValue2,
                memberName, srcPath, srcLine );
        }

        public void Warning(
            string messageTemplate,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            Write( LogEventLevel.Warning, messageTemplate, propertyValues, memberName, srcPath, srcLine );
        }

        #endregion
    }
}