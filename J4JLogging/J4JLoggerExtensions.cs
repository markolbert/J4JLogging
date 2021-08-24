namespace J4JSoftware.Logging
{
    public static class J4JLoggerExtensions
    {
        public static J4JLogger ReportSourceCodeFile( this J4JLogger logger )
            => logger.AddEnricher<SourceFileEnricher>();

        public static J4JLogger ReportCallingMember(this J4JLogger logger)
            => logger.AddEnricher<CallingMemberEnricher>();

        public static J4JLogger ReportLineNumber(this J4JLogger logger)
            => logger.AddEnricher<LineNumberEnricher>();

        public static J4JLogger ReportLoggedType(this J4JLogger logger)
            => logger.AddEnricher<LoggedTypeEnricher>();

        public static J4JLogger AddEnricher<T>( this J4JLogger logger )
            where T : BaseEnricher, new()
        {
            if( !logger.Built )
                logger.MessageTemplateManager.AddEnricher<T>();

            return logger;
        }
    }
}