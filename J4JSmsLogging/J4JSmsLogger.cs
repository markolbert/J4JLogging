using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JSmsLogger : J4JLogger, IJ4JSmsLogger
    {
        private readonly IJ4JSms _smsLogger;
        private readonly StringWriter _smsWriter;

        private bool _sendNextSms;

        public J4JSmsLogger(
            Type loggedType,
            ILogger logger,
            IJ4JSmsLoggerConfiguration config,
            IJ4JSms smsLogger
        )
        : base(loggedType, logger, config)
        {
            if( config == null ) throw new NullReferenceException( nameof(config) );

            _smsWriter = config.SmsWriter ?? throw new NullReferenceException( nameof(config.SmsWriter) );

            _smsLogger = smsLogger ?? throw new NullReferenceException( nameof(smsLogger) );
        }

        public IJ4JSmsLogger SendSms()
        {
            _sendNextSms = true;
            return this;
        }

        protected virtual void ProcessSms()
        {
            if( _smsLogger == null )
            {
                BaseLogger.Error("SMS logger not defined, can't send SMS message");
                return;
            }

            var mesg = _smsWriter.ToString();

            _smsWriter.GetStringBuilder().Clear();

            if( _sendNextSms )
            {
                _smsLogger.Send( mesg );
                _sendNextSms = false;
            }
        }

        #region Write() methods

        public override void Write(
            LogEventLevel level,
            string messageTemplate,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            base.Write(level, messageTemplate, memberName, srcPath, srcLine);
            ProcessSms();
        }

        public override void Write<T0>(
            LogEventLevel level,
            string messageTemplate,
            T0 propertyValue,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            base.Write( level, messageTemplate, propertyValue, memberName, srcPath, srcLine );
            ProcessSms();
        }

        public override void Write<T0, T1>(
            LogEventLevel level,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            base.Write(level, messageTemplate, propertyValue0, propertyValue1, memberName, srcPath, srcLine);
            ProcessSms();
        }

        public override void Write<T0, T1, T2>(
            LogEventLevel level,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            base.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath, srcLine);
            ProcessSms();
        }

        public override void Write(
            LogEventLevel level,
            string messageTemplate,
            object[] propertyValues,
            [ CallerMemberName ] string memberName = "",
            [ CallerFilePath ] string srcPath = "",
            [ CallerLineNumber ] int srcLine = 0
        )
        {
            base.Write(level, messageTemplate, propertyValues, memberName, srcPath, srcLine);
            ProcessSms();
        }

        #endregion
    }
}