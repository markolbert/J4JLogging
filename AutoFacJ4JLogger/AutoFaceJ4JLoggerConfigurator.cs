using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;

namespace J4JSoftware.Logging
{
    public static class AutoFaceJ4JLoggerConfigurator
    {
        public static ContainerBuilder RegisterJ4JLoggerChannels( 
            this ContainerBuilder builder,
            params Type[] assemblyTypes )
        {
            var assemblies = assemblyTypes.Select( x => x.Assembly )
                .Distinct()
                .ToArray();

            var registrar = builder.RegisterAssemblyTypes( assemblies )
                .Where( t => !t.IsAbstract
                             && t.GetCustomAttributes( typeof(ChannelIDAttribute), false ).Length == 1
                             && typeof(IChannel).IsAssignableFrom( t )
                             && t.GetConstructors().Any( x =>
                             {
                                 var ctorParams = x.GetParameters();

                                 return ctorParams.Length == 1
                                        && typeof(J4JLogger).IsAssignableFrom( ctorParams[ 0 ].ParameterType );
                             } ) )
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<J4JLoggerConfigurator>()
                .AsSelf();

            return builder;
        }
    }
}
