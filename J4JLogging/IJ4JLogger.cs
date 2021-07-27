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
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface IJ4JLogger : IChannelParameters
    {
        Type? LoggedType { get; }

        #region Write methods

        void Write( 
            LogEventLevel level, 
            string template, 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Write<T0>( 
            LogEventLevel level, 
            string template, 
            T0 propertyValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0);

        void Write<T0, T1>( 
            LogEventLevel level, 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0);

        void Write<T0, T1, T2>( 
            LogEventLevel level, 
            string template, 
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0);

        void Write( 
            LogEventLevel level, 
            string template, 
            object[] propertyValues,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0);

        #endregion

        #region Debug methods

        void Debug( string template, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Debug<T0>( string template, T0 propertyValue, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Debug<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Debug<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Debug( string template, object[] propertyValues, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        #endregion

        #region Error methods

        void Error( string template, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Error<T0>( string template, T0 propertyValue, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Error<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Error<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Error( string template, object[] propertyValues, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        #endregion

        #region Fatal methods

        void Fatal( string template, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Fatal<T0>( string template, T0 propertyValue, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Fatal<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Fatal<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Fatal( string template, object[] propertyValues, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        #endregion

        #region Information methods

        void Information( string template, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Information<T0>( string template, T0 propertyValue, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Information<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Information<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Information( string template, object[] propertyValues, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        #endregion

        #region Verbose methods

        void Verbose( string template, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Verbose<T0>( string template, T0 propertyValue, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Verbose<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Verbose<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Verbose( string template, object[] propertyValues, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        #endregion

        #region Warning methods

        void Warning( string template, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Warning<T0>( string template, T0 propertyValue, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Warning<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        void Warning<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "", [CallerLineNumber] int srcLine = 0 );

        void Warning( string template, object[] propertyValues, [CallerMemberName] string memberName = "", [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0 );

        #endregion
    }
}