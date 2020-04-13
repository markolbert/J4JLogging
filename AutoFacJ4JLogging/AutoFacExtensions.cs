using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AutoFacJ4JLogging
{
    public static class AutoFacExtensions
    {
        public static ContainerBuilder AddJ4JLogging<TConfig>( this ContainerBuilder builder, string configFilePath, params Type[] assemblyDefiningTypes)
        {
            var logChannelTypes = GetLogChannelConfigurationTypes( assemblyDefiningTypes );

            foreach( var kvp in logChannelTypes )
            {
                builder.RegisterType( kvp.Value )
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            builder.Register((c, p) =>
                {
                    var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();
                    return loggerConfig.CreateLogger();
                })
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLoggerFactory>()
                .As<IJ4JLoggerFactory>()
                .SingleInstance();

            builder.Register<TConfig>( ( c, p ) =>
                    J4JLoggerConfiguration.CreateFromFile<TConfig>( configFilePath, logChannelTypes ) )
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            return builder;
        }

        public static ContainerBuilder AddJ4JLogging(this ContainerBuilder builder, IJ4JLoggerConfiguration config, params Type[] assemblyDefiningTypes)
        {
            var logChannelTypes = GetLogChannelConfigurationTypes(assemblyDefiningTypes);

            foreach (var kvp in logChannelTypes)
            {
                builder.RegisterType(kvp.Value)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            builder.Register((c, p) => config.CreateLogger() )
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLoggerFactory>()
                .As<IJ4JLoggerFactory>()
                .SingleInstance();

            return builder;
        }

        private static Dictionary<LogChannelAttribute, Type> GetLogChannelConfigurationTypes( Type[] assemblyDefiningTypes )
        {
            // create the master channel type list which guides our search for LogChannelConfiguration types to
            // register, ensuring the base types are always included
            var typesToSearch = assemblyDefiningTypes.ToList();

            var assemblies = typesToSearch.Distinct()
                .Select(t => t.Assembly)
                .Distinct()
                .ToList();

            // now scan all the assemblies and look for all types derived from
            // LogChannelConfiguration which have a public parameterless constructor
            // and are decorated with a LogChannelAttribute
            var logChannelTypes = assemblies.SelectMany( a => a.DefinedTypes )
                .Where( t => t.DeclaredConstructors.Any(
                                 c => c.IsPublic
                                      && c.GetParameters().Length == 0 )
                             && t.GetCustomAttributesData().Any( a => a.AttributeType == typeof(LogChannelAttribute) ) )
                .Select( t => new
                {
                    LCAttribute = (LogChannelAttribute) t.GetCustomAttributes(typeof(LogChannelAttribute), false).First(),
                    Type = t
                } );

            var retVal = new Dictionary<LogChannelAttribute, Type>( LogChannelAttribute.DefaultComparer );

            foreach( var typeInfo in logChannelTypes )
            {
                if( retVal.ContainsKey( typeInfo.LCAttribute ) )
                    retVal[ typeInfo.LCAttribute ] = typeInfo.Type;
                else retVal.Add( typeInfo.LCAttribute, typeInfo.Type );
            }

            return retVal;
        }
    }
}
