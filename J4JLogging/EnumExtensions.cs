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
using System.Linq;

namespace J4JSoftware.Logging
{
    // Utility extension methods for transforming a flag Enum value into a collection
    // of individual Enum values
    public static class EnumExtensions
    {
        public static IEnumerable<TEnum> GetUniqueFlags<TEnum>( this TEnum toParse )
            where TEnum : Enum
        {
            var toParseNum = Convert.ToUInt64( toParse );

            foreach( var uniqueFlag in Enum.GetValues( typeof(TEnum) )
                .Cast<TEnum>() )
            {
                var uniqueNum = Convert.ToUInt64( uniqueFlag );

                if( uniqueNum != 0 && ( uniqueNum & ( uniqueNum - 1 ) ) == 0 )
                    yield return uniqueFlag;
            }
        }

        public static IEnumerable<TEnum> GetUniqueFlags<TEnum>()
            where TEnum : Enum
        {
            foreach( var uniqueFlag in Enum.GetValues( typeof(TEnum) )
                .Cast<TEnum>() )
            {
                var uniqueNum = Convert.ToUInt64( uniqueFlag );

                if( uniqueNum != 0 && ( uniqueNum & ( uniqueNum - 1 ) ) == 0 )
                    yield return uniqueFlag;
            }
        }
    }
}