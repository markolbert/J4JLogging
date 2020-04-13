using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace J4JSoftware.Logging
{
    public class J4JLoggingModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            base.Load( builder );

            builder.RegisterAssemblyTypes( typeof(J4JLoggingModule).Assembly )
                .Where( t => typeof(LogChannelConfiguration).IsAssignableFrom( t )
                             && !t.IsAbstract
                             && ( t.GetConstructors()?.Length > 0 ) )
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
