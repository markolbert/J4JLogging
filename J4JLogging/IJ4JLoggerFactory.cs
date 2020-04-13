using System;

namespace J4JSoftware.Logging
{
    public interface IJ4JLoggerFactory
    {
        IJ4JLogger CreateLogger( Type toLog );
    }
}