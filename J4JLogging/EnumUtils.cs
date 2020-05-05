using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace J4JSoftware.Logging
{
    public static class EnumUtils
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
