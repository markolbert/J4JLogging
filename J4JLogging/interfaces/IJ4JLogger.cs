﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    /// <summary>
    ///     Wrapper for <see cref="Serilog.ILogger"/> which simplifies including calling member
    ///     (e.g., method name) and source code information.
    /// </summary>
    public interface IJ4JLogger
    {
        /// <summary>
        /// The default elements to include in each logging entry
        /// </summary>
        EntryElements DefaultElements { get; }

        List<LogChannel> Channels { get; }

        /// <summary>
        /// The root part of the source code paths (optional). If specified,
        /// the root part will be stripped from all source code paths. Read only.
        /// </summary>
        string SourceRootPath { get; }

        /// <summary>
        ///     Forces the next event to be written with the specified EntryElements
        /// </summary>
        /// <returns>
        ///     a reference to the wrapper object, so that event-writing
        ///     methods can be chained onto the method call
        /// </returns>
        IJ4JLogger Elements( EntryElements toInclude );
        
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
        void Write(
            LogEventLevel level,
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

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
        void Write<T0>(
            LogEventLevel level,
            string template,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

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
        void Write<T0, T1>(
            LogEventLevel level,
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Write<T0, T1, T2>(
            LogEventLevel level,
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Write(
            LogEventLevel level,
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        #endregion

        #region Debug() methods

        void Debug(
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Debug<T0>(
            string template,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Debug<T0, T1>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Debug<T0, T1, T2>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Debug(
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        #endregion

        #region Error() methods

        void Error(
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Error<T0>(
            string template,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Error<T0, T1>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Error<T0, T1, T2>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Error(
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        #endregion

        #region Fatal() methods

        void Fatal(
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Fatal<T0>(
            string template,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Fatal<T0, T1>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Fatal<T0, T1, T2>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Fatal(
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        #endregion

        #region Information() methods

        void Information(
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Information<T0>(
            string template,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Information<T0, T1>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Information<T0, T1, T2>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Information(
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        #endregion

        #region Verbose() methods

        void Verbose(
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Verbose<T0>(
            string template,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Verbose<T0, T1>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Verbose<T0, T1, T2>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Verbose(
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        #endregion

        #region Warning() methods

        void Warning(
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Warning<T0>(
            string template,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Warning<T0, T1>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Warning<T0, T1, T2>(
            string template,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        void Warning(
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        );

        #endregion
    }
}