﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    /// <summary>
    ///     Wrapper for <see cref="Serilog.ILogger"/> which simplifies including calling member
    ///     (e.g., method name) and source code information.
    /// </summary>
    // added to test SharpDoc
    [ Dummy( "", typeof(string) ) ]
    [ Dummy( "test", typeof(int) ) ]
    [ Dummy( "test", typeof(int) ) ]
    public class J4JLogger : IJ4JLogger
    {
        private bool _forceExternal = false;

        public J4JLogger(
            ILogger logger,
            IJ4JLoggerConfiguration config
        )
        {
            Configuration = config ?? throw new NullReferenceException( nameof(config) );

            BaseLogger = logger ?? throw new NullReferenceException( nameof(logger) );
        }

        /// <summary>
        /// The <see cref="Serilog.ILogger"/>  instance that handles the actual logging. Read only.
        /// </summary>
        protected ILogger BaseLogger { get; private set; }

        public IJ4JLoggerConfiguration Configuration { get; }

        protected void ProcessAfterWritingChannels()
        {
            var doExternal = _forceExternal || Configuration.UseExternalSinks;

            foreach( var channel in Configuration.Channels
                .Where( c => c is IPostProcess )
                .Cast<IPostProcess>() )
            {
                if( doExternal ) channel.PostProcess();
                else channel.Clear();
            }
        }

        protected List<IDisposable> InitializeContextProperties( string memberName, string srcPath, int srcLine )
        {
            var retVal = new List<IDisposable>();

            LogContext.PushProperty(
                "MemberName",
                ( Configuration.EventElements & EventElements.Type ) == EventElements.Type ? $"::{memberName}" : ""
            );

            if( ( Configuration.EventElements & EventElements.SourceCode ) == EventElements.SourceCode )
                LogContext.PushProperty( "SourceCodeInformation", $"{srcPath} : {srcLine}" );

            return retVal;
        }

        protected void DisposeContextProperties( List<IDisposable> contextProperties )
        {
            foreach( var contextProperty in contextProperties )
            {
                contextProperty.Dispose();
            }
        }

        public void SetLoggedType<TLogged>()
        {
            if( ( Configuration.EventElements & EventElements.Type ) == EventElements.Type )
                BaseLogger = BaseLogger.ForContext<TLogged>();
        }

        public void SetLoggedType( Type toLog )
        {
            if( toLog != null
                && ( Configuration.EventElements & EventElements.Type ) == EventElements.Type )
                BaseLogger = BaseLogger.ForContext( toLog );
        }

        public IJ4JLogger ForceExternal( bool processExternal = true )
        {
            _forceExternal = processExternal;
            return this;
        }

        #region Write() methods

        /// <summary>
        /// Writes an event to ILogger, including the calling member and calling type, and
        /// optionally the source code file and line number of the method.
        /// </summary>
        /// <param name="level">the <see cref="LogEventLevel"/> of the event</param>
        /// <param name="template">the <see cref="Serilog"/> message template for constructing the
        /// log message. Note that this will be included within the output template as the
        /// value of the Message parameter.</param>
        /// <param name="memberName">the name of the method calling the Write method (supplied
        /// automatically by the compiler)</param>
        /// <param name="srcPath">the path to the source code file in which the method calling the
        /// Write method is defined (supplied automatically by the compiler)</param>
        /// <param name="srcLine">the line number within the source code file at which the method calling the
        /// Write method is defined (supplied automatically by the compiler)</param>
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

            ProcessAfterWritingChannels();
        }

        /// <summary>
        /// Writes an event to ILogger, including the calling member and calling type, and
        /// optionally the source code file and line number of the method.
        ///
        /// This overload
        /// lets you pass a parameter in to the <see cref="Serilog"/> logging system to
        /// be incorporated into the supplied message template./>
        /// </summary>
        /// <typeparam name="T0">the Type of the supplied propertyValue</typeparam>
        /// <param name="level">the <see cref="LogEventLevel"/> of the event</param>
        /// <param name="template">the <see cref="MessageTemplate"/> message template for constructing the
        /// log message. Note that this will be included within the output template as the
        /// value of the Message parameter.</param>
        /// <param name="propertyValue">the parameter value to be incorporated into the
        /// supplied message template (see the <see cref="Serilog"/> documentation for how
        /// the value is matched to the token in the message template)</param>
        /// <param name="memberName">the name of the method calling the Write method (supplied
        /// automatically by the compiler)</param>
        /// <param name="srcPath">the path to the source code file in which the method calling the
        /// Write method is defined (supplied automatically by the compiler)</param>
        /// <param name="srcLine">the line number within the source code file at which the method calling the
        /// Write method is defined (supplied automatically by the compiler)</param>
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

            ProcessAfterWritingChannels();
        }

        /// <summary>
        /// Writes an event to ILogger, including the calling member and calling type, and
        /// optionally the source code file and line number of the method.
        ///
        /// This overload
        /// lets you pass two parameters in to the <see cref="Serilog"/> logging system to
        /// be incorporated into the supplied message template./>
        /// </summary>
        /// <typeparam name="T0">the Type of the first supplied propertyValue</typeparam>
        /// <typeparam name="T1">the Type of the second supplied propertyValue</typeparam>
        /// <param name="level">the <see cref="LogEventLevel"/> of the event</param>
        /// <param name="template">the <see cref="MessageTemplate"/> message template for constructing the
        /// log message. Note that this will be included within the output template as the
        /// value of the Message parameter.</param>
        /// <param name="propertyValue0">the first parameter value to be incorporated into the
        /// supplied message template (see the <see cref="Serilog"/> documentation for how
        /// the value is matched to the token in the message template)</param>
        /// <param name="propertyValue1">the second parameter value to be incorporated into the
        /// supplied message template (see the <see cref="Serilog"/> documentation for how
        /// the value is matched to the token in the message template)</param>
        /// <param name="memberName">the name of the method calling the Write method (supplied
        /// automatically by the compiler)</param>
        /// <param name="srcPath">the path to the source code file in which the method calling the
        /// Write method is defined (supplied automatically by the compiler)</param>
        /// <param name="srcLine">the line number within the source code file at which the method calling the
        /// Write method is defined (supplied automatically by the compiler)</param>
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

            ProcessAfterWritingChannels();
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

            ProcessAfterWritingChannels();
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

            ProcessAfterWritingChannels();
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