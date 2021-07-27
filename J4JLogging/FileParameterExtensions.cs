using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class FileParameterExtensions
    {
        public static FileParameters SetFileNameStub(this FileParameters container, string fileNameStub )
        {
            container.FileName = fileNameStub;
            return container;
        }

        public static FileParameters SetRollingInterval( this FileParameters container, RollingInterval interval)
        {
            container.RollingInterval = interval;
            return container;
        }

        public static FileParameters SetLoggingFolder( this FileParameters container, string loggingFolder )
        {
            container.Folder = loggingFolder;
            return container;
        }

        public static FileParameters LogToExecutionFolder( this FileParameters container )
        {
            container.SetLoggingFolder( Environment.CurrentDirectory );
            return container;
        }

        public static FileParameters LogToLocalApplicationData( this FileParameters container, string publisher, string appName )
        {
            var specialFolder = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
            container.SetLoggingFolder( Path.Combine( specialFolder, publisher, appName ) );

            return container;
        }

        public static FileParameters LogToApplicationData(this FileParameters container, string publisher, string appName)
        {
            var specialFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            container.SetLoggingFolder(Path.Combine(specialFolder, publisher, appName));

            return container;
        }

        public static FileParameters LogToDesktop(this FileParameters container)
        {
            var specialFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            container.SetLoggingFolder( specialFolder );

            return container;
        }
    }
}