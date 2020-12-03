using Autofac;
using Serilog;

namespace J4JSoftware.Logging
{
    public static class AutofacExtensions
    {
        public static ContainerBuilder RegisterJ4JLogging( this ContainerBuilder builder )
        {
            builder.Register(c =>
                {
                    var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();

                    return loggerConfig.CreateBaseLogger()!;
                })
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLogger>()
                .As<IJ4JLogger>();

            return builder;
        }
    }
}
