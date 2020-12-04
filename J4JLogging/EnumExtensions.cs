using System;
using System.Collections.Generic;
using System.Linq;

namespace J4JSoftware.Logging
{
    // Utility extension methods for transforming a flag Enum value into a collection
    // of individual Enum values
    public static class EnumExtensions
    {
        public static IEnumerable<TEnum> GetUniqueFlags<TEnum>(this TEnum toParse) 
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
            foreach (var uniqueFlag in Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>())
            {
                var uniqueNum = Convert.ToUInt64(uniqueFlag);

                if (uniqueNum != 0 && (uniqueNum & (uniqueNum - 1)) == 0)
                    yield return uniqueFlag;
            }
        }
    }
}
