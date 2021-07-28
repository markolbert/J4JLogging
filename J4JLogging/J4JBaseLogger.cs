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
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;

#pragma warning disable 8604

namespace J4JSoftware.Logging
{
    public abstract class J4JBaseLogger : IJ4JLogger
    {
        public const string DefaultOutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}";

        private Type? _loggedType;

        protected J4JBaseLogger()
        {
        }

        protected internal virtual void ResetBaseLogger()
        {
        }

        #region Global parameters

        public Type? LoggedType
        {
            get => _loggedType;

            protected set
            {
                var changed = _loggedType != value;

                _loggedType = value;

                if( changed )
                    OnLoggedTypeChanged();
            }
        }

        protected abstract void OnLoggedTypeChanged();

        public abstract bool OutputCache( J4JCachedLogger cachedLogger );

        public J4JBaseLogger SetLoggedType<TLogged>() => SetLoggedType(typeof(TLogged));

        public J4JBaseLogger SetLoggedType(Type typeToLog)
        {
            LoggedType = typeToLog;
            return this;
        }

        public J4JBaseLogger ClearLoggedType()
        {
            LoggedType = null;
            return this;
        }

        public bool SourcePathIncluded { get; internal set; }
        public string? SourceRootPath { get; internal set; }
        public string OutputTemplate { get; internal set; } = DefaultOutputTemplate;
        public bool RequireNewLine { get; internal set; }
        public LogEventLevel MinimumLevel { get; internal set; } = LogEventLevel.Verbose;

        #endregion

        protected internal bool OutputNextToSms { get; set; }

        protected void ResetSms()
        {
            if( OutputNextToSms )
                OutputNextToSms = false;
        }

        #region Write methods

        public abstract void Write( 
            LogEventLevel level, 
            string template, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        public abstract void Write<T0>( 
            LogEventLevel level, 
            string template, 
            T0 propertyValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0);

        public abstract void Write<T0, T1>( 
            LogEventLevel level, 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0);

        public abstract void Write<T0, T1, T2>( 
            LogEventLevel level, 
            string template, 
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0);

        public abstract void Write( 
            LogEventLevel level, 
            string template, 
            object[] propertyValues,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0);

        #endregion

        #region Debug methods

        public void Debug(
            string template,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 )
            => Write( LogEventLevel.Debug, template, memberName, srcPath, srcLine );

        public void Debug<T0>( 
            string template, 
            T0 propertyValue, 
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, propertyValue, memberName, srcPath, srcLine );

        public void Debug<T0, T1>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );

        public void Debug<T0, T1, T2>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            T2 propertyValue2,
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "", 
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath, srcLine );

        public void Debug( 
            string template, 
            object[] propertyValues, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, propertyValues, memberName, srcPath, srcLine );

        #endregion

        #region Error methods

        public void Error( 
            string template, 
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "", 
            [ CallerLineNumber ] int srcLine = 0 ) =>
            Write( LogEventLevel.Error, template, memberName, srcPath, srcLine );

        public void Error<T0>( 
            string template, T0 propertyValue, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Error, template, propertyValue, memberName, srcPath, srcLine );

        public void Error<T0, T1>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Error, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );

        public void Error<T0, T1, T2>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            T2 propertyValue2,
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "", 
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Error, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath, srcLine );

        public void Error( 
            string template, 
            object[] propertyValues, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Error, template, propertyValues, memberName, srcPath, srcLine );

        #endregion

        #region Fatal methods

        public void Fatal( 
            string template, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "", 
            [CallerLineNumber] int srcLine = 0 ) => 
            Write( LogEventLevel.Fatal, template, memberName, srcPath, srcLine );

        public void Fatal<T0>( 
            string template, 
            T0 propertyValue, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Fatal, template, propertyValue, memberName, srcPath, srcLine );

        public void Fatal<T0, T1>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Fatal, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );

        public void Fatal<T0, T1, T2>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            T2 propertyValue2,
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "", 
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Fatal, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath, srcLine );

        public void Fatal( 
            string template, 
            object[] propertyValues, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Fatal, template, propertyValues, memberName, srcPath, srcLine );

        #endregion

        #region Information methods

        public void Information( 
            string template, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "", 
            [CallerLineNumber] int srcLine = 0 ) => 
            Write( LogEventLevel.Information, template, memberName, srcPath, srcLine );

        public void Information<T0>( 
            string template, 
            T0 propertyValue, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Information, template, propertyValue, memberName, srcPath, srcLine );

        public void Information<T0, T1>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Information, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );

        public void Information<T0, T1, T2>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            T2 propertyValue2,
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "", 
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Information, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath, srcLine );

        public void Information( 
            string template, 
            object[] propertyValues, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Information, template, propertyValues, memberName, srcPath, srcLine );

        #endregion

        #region Verbose methods

        public void Verbose( 
            string template, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "", 
            [CallerLineNumber] int srcLine = 0 ) => 
            Write( LogEventLevel.Verbose, template, memberName, srcPath, srcLine );

        public void Verbose<T0>( 
            string template, 
            T0 propertyValue, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Verbose, template, propertyValue, memberName, srcPath, srcLine );

        public void Verbose<T0, T1>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Verbose, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );

        public void Verbose<T0, T1, T2>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            T2 propertyValue2,
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "", 
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Verbose, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath, srcLine );

        public void Verbose( 
            string template, 
            object[] propertyValues, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Verbose, template, propertyValues, memberName, srcPath, srcLine );

        #endregion

        #region Warning methods

        public void Warning( 
            string template, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "", 
            [CallerLineNumber] int srcLine = 0 ) => 
            Write( LogEventLevel.Warning, template, memberName, srcPath, srcLine );

        public void Warning<T0>( 
            string template, 
            T0 propertyValue, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Warning, template, propertyValue, memberName, srcPath, srcLine );

        public void Warning<T0, T1>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Warning, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );

        public void Warning<T0, T1, T2>( 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1, 
            T2 propertyValue2,
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "", 
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Warning, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath, srcLine );

        public void Warning( 
            string template, 
            object[] propertyValues, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 ) =>
            Write( LogEventLevel.Warning, template, propertyValues, memberName, srcPath, srcLine );

        #endregion
    }
}