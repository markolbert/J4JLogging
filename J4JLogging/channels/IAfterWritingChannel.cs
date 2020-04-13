using System;
using System.Collections.Generic;
using System.Text;

namespace J4JSoftware.Logging
{
    public interface IAfterWritingChannel
    {
        void AfterWriting();
        bool Initialize( object config );
    }

    public interface IAfterWritingChannel<in TSms> : IAfterWritingChannel
        where TSms : class
    {
        bool Initialize( TSms config );
    }
}
