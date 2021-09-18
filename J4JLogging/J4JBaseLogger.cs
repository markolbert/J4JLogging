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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Serilog.Events;
// ReSharper disable ExplicitCallerInfoArgument
#pragma warning disable 8777

namespace J4JSoftware.Logging
{
    public abstract class J4JBaseLogger : IJ4JLogger
    {
        public event EventHandler<NetEventArgs>? LogEvent;

        protected J4JBaseLogger(
            NetEventSink? netEventSink 
            )
        {
            if (netEventSink != null)
                netEventSink.RaiseEvent = RaiseEvent;
        }

        private void RaiseEvent(NetEventArgs args) => LogEvent?.Invoke(this, args);

        #region Logged Type

        public Type? LoggedType { get; private set; }

        protected abstract void OnLoggedTypeChanged();

        public void SetLoggedType<TLogged>() => SetLoggedType( typeof( TLogged ) );

        public void SetLoggedType( Type typeToLog )
        {
            LoggedType = typeToLog;
            OnLoggedTypeChanged();
        }

        #endregion

        public void SendNextEventToSms() => SmsHandling = SmsHandling.SendNextMessage;
        public void SendAllEventsToSms() => SmsHandling = SmsHandling.SendUntilReset;
        public void StopSendingEventsToSms() => SmsHandling = SmsHandling.DoNotSend;

        public SmsHandling SmsHandling { get; set; }

        public abstract bool OutputCache( J4JCachedLogger cachedLogger );

        #region Write methods

        public void Write(
            LogEventLevel level,
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 )
            => Write( level, template, Array.Empty<object>(), memberName,
                srcPath, srcLine );

        public void Write<T0>(
            LogEventLevel level,
            string template,
            [NotNull] T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 )
            => Write( level, template, new object[] { propertyValue! }, memberName,
                srcPath, srcLine );

        public void Write<T0, T1>(
            LogEventLevel level,
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 )
            => Write( level, template, new object[] { propertyValue0!, propertyValue1! }, memberName,
                srcPath, srcLine );

        public void Write<T0, T1, T2>(
            LogEventLevel level,
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [NotNull] T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 )
            => Write( level, template, new object[] { propertyValue0!, propertyValue1!, propertyValue2! }, memberName,
                srcPath, srcLine );

        public abstract void Write(
            LogEventLevel level,
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 );

        #endregion

        #region Debug methods

        public void Debug(
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 )
            => Write( LogEventLevel.Debug, template, Array.Empty<object>(), memberName,
                srcPath, srcLine );

        public void Debug<T0>(
            string template,
            [NotNull] T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 )
            => Write( LogEventLevel.Debug, template, new object[] { propertyValue! }, memberName,
                srcPath, srcLine );

        public void Debug<T0, T1>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, new object[] { propertyValue0!, propertyValue1! }, memberName,
                srcPath, srcLine );


        public void Debug<T0, T1, T2>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [NotNull] T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, new object[] { propertyValue0!, propertyValue1!, propertyValue2! },
                memberName,
                srcPath, srcLine );


        public void Debug(
            string template,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 )
            => Write( LogEventLevel.Debug, template, propertyValues, memberName, srcPath, srcLine );

        #endregion

        #region Error methods

        public void Error(
            string template,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Error, template, new object[0], memberName,
                srcPath, srcLine);

        public void Error<T0>(
            string template,
            [NotNull] T0 propertyValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Error, template, new object[] { propertyValue! }, memberName,
                srcPath, srcLine);

        public void Error<T0, T1>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0) =>
            Write(LogEventLevel.Error, template, new object[] { propertyValue0!, propertyValue1! }, memberName,
                srcPath, srcLine);


        public void Error<T0, T1, T2>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [NotNull] T2 propertyValue2,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0) =>
            Write(LogEventLevel.Error, template, new object[] { propertyValue0!, propertyValue1!, propertyValue2! },
                memberName,
                srcPath, srcLine);


        public void Error(
            string template,
            object[] propertyValues,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Error, template, propertyValues, memberName, srcPath, srcLine);

        #endregion

        #region Fatal methods

        public void Fatal(
            string template,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Fatal, template, new object[0], memberName,
                srcPath, srcLine);

        public void Fatal<T0>(
            string template,
            [NotNull] T0 propertyValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Fatal, template, new object[] { propertyValue! }, memberName,
                srcPath, srcLine);

        public void Fatal<T0, T1>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0) =>
            Write(LogEventLevel.Fatal, template, new object[] { propertyValue0!, propertyValue1! }, memberName,
                srcPath, srcLine);


        public void Fatal<T0, T1, T2>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [NotNull] T2 propertyValue2,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0) =>
            Write(LogEventLevel.Fatal, template, new object[] { propertyValue0!, propertyValue1!, propertyValue2! },
                memberName,
                srcPath, srcLine);


        public void Fatal(
            string template,
            object[] propertyValues,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Fatal, template, propertyValues, memberName, srcPath, srcLine);

        #endregion

        #region Information methods

        public void Information(
            string template,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Information, template, new object[0], memberName,
                srcPath, srcLine);

        public void Information<T0>(
            string template,
            [NotNull] T0 propertyValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Information, template, new object[] { propertyValue! }, memberName,
                srcPath, srcLine);

        public void Information<T0, T1>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0) =>
            Write(LogEventLevel.Information, template, new object[] { propertyValue0!, propertyValue1! }, memberName,
                srcPath, srcLine);


        public void Information<T0, T1, T2>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [NotNull] T2 propertyValue2,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0) =>
            Write(LogEventLevel.Information, template, new object[] { propertyValue0!, propertyValue1!, propertyValue2! },
                memberName,
                srcPath, srcLine);


        public void Information(
            string template,
            object[] propertyValues,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Information, template, propertyValues, memberName, srcPath, srcLine);

        #endregion

        #region Verbose methods

        public void Verbose(
            string template,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Verbose, template, new object[0], memberName,
                srcPath, srcLine);

        public void Verbose<T0>(
            string template,
            [NotNull] T0 propertyValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Verbose, template, new object[] { propertyValue! }, memberName,
                srcPath, srcLine);

        public void Verbose<T0, T1>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0) =>
            Write(LogEventLevel.Verbose, template, new object[] { propertyValue0!, propertyValue1! }, memberName,
                srcPath, srcLine);


        public void Verbose<T0, T1, T2>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [NotNull] T2 propertyValue2,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0) =>
            Write(LogEventLevel.Verbose, template, new object[] { propertyValue0!, propertyValue1!, propertyValue2! },
                memberName,
                srcPath, srcLine);


        public void Verbose(
            string template,
            object[] propertyValues,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Verbose, template, propertyValues, memberName, srcPath, srcLine);

        #endregion

        #region Warning methods

        public void Warning(
            string template,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Warning, template, new object[0], memberName,
                srcPath, srcLine);

        public void Warning<T0>(
            string template,
            [NotNull] T0 propertyValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Warning, template, new object[] { propertyValue! }, memberName,
                srcPath, srcLine);

        public void Warning<T0, T1>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0) =>
            Write(LogEventLevel.Warning, template, new object[] { propertyValue0!, propertyValue1! }, memberName,
                srcPath, srcLine);


        public void Warning<T0, T1, T2>(
            string template,
            [NotNull] T0 propertyValue0,
            [NotNull] T1 propertyValue1,
            [NotNull] T2 propertyValue2,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0) =>
            Write(LogEventLevel.Warning, template, new object[] { propertyValue0!, propertyValue1!, propertyValue2! },
                memberName,
                srcPath, srcLine);


        public void Warning(
            string template,
            object[] propertyValues,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
            => Write(LogEventLevel.Warning, template, propertyValues, memberName, srcPath, srcLine);

        #endregion

    }
}