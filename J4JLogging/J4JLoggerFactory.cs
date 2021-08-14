using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J4JSoftware.Logging
{
    public class J4JLoggerFactory : IJ4JLoggerFactory
    {
        private readonly Func<IJ4JLogger> _loggerFactory;

        public J4JLoggerFactory(
            Func<IJ4JLogger> loggerFactory
        )
        {
            _loggerFactory = loggerFactory;
        }

        public IJ4JLogger CreateLogger( Type forType )
        {
            var retVal = _loggerFactory.Invoke();
            retVal.SetLoggedType( forType );

            return retVal;
        }

        public IJ4JLogger CreateLogger<T>() => CreateLogger( typeof(T) );
    }
}
