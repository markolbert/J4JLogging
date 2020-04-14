using System;
using System.Collections.Generic;
using System.Text;

namespace J4JSoftware.Logging
{
    public interface IPostProcess
    {
        void PostProcess();
        void Clear();
        bool Initialize( object config );
    }

    public interface IPostProcess<in TSms> : IPostProcess
        where TSms : class
    {
        bool Initialize( TSms config );
    }
}
