using System;
using System.Runtime.CompilerServices;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    /// <summary>
    ///     Wrapper for <see cref="Serilog.ILogger"/> which simplifies including calling member
    ///     (e.g., method name) and source code information, plus allows for an SMS logging
    ///     mechanism.
    /// </summary>
    public interface IJ4JSmsLogger : IJ4JLogger
    {
        /// <summary>
        ///     Forces the next event to be echoed to the defined SMS service
        /// </summary>
        /// <returns>
        ///     a reference to the wrapper object, so that event-writing
        ///     methods can be chained onto the method call
        /// </returns>
        IJ4JSmsLogger SendSms();
    }
}