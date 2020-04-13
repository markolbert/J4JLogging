using System;
using System.Collections.Generic;
using System.Text;

namespace J4JSoftware.Logging
{
    public interface IPostProcessLogEvent
    {
        void PostProcessLogEventText();
        void ClearLogEventText();
        bool Initialize( object config );
    }

    public interface IPostProcessLogEvent<in TSms> : IPostProcessLogEvent
        where TSms : class
    {
        bool Initialize( TSms config );
    }
}
