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
using Serilog.Events;

#pragma warning disable 8604

namespace J4JSoftware.Logging
{
    public class J4JCachedLogger : J4JBaseLogger
    {
        public override void Write( 
            LogEventLevel level, 
            string template,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
        {
            Entries.Add( new CachedEntry(
                level,
                template,
                memberName,
                srcPath,
                srcLine,
                OutputNextToSms,
                new object[0] )
            );

            ResetSms();
        }

        public List<CachedEntry> Entries { get; } = new();

        public override void Write<T0>( 
            LogEventLevel level, 
            string template, 
            T0 propertyValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
        {
            Entries.Add(new CachedEntry(
                level,
                template,
                memberName,
                srcPath,
                srcLine,
                OutputNextToSms,
                propertyValue)
            );

            ResetSms();
        }

        public override void Write<T0, T1>( 
            LogEventLevel level, 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
        {
            Entries.Add(new CachedEntry(
                level,
                template,
                memberName,
                srcPath,
                srcLine,
                OutputNextToSms,
                propertyValue0,
                propertyValue1 )
            );

            ResetSms();
        }

        public override void Write<T0, T1, T2>( 
            LogEventLevel level, 
            string template, 
            T0 propertyValue0, 
            T1 propertyValue1,
            T2 propertyValue2,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
        {
            Entries.Add( new CachedEntry(
                level,
                template,
                memberName,
                srcPath,
                srcLine,
                OutputNextToSms,
                propertyValue0,
                propertyValue1,
                propertyValue2 )
            );

            ResetSms();
        }

        public override void Write( 
            LogEventLevel level, 
            string template, 
            object[] propertyValues,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string srcPath = "",
            [CallerLineNumber] int srcLine = 0)
        {
            Entries.Add(new CachedEntry(
                level,
                template,
                memberName,
                srcPath,
                srcLine,
                OutputNextToSms,
                propertyValues)
            );

            ResetSms();
        }
    }
}