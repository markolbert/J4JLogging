using System;
using System.Text;

namespace J4JSoftware.Logging
{
    public class J4JLoggerTemplate : IJ4JLoggerTemplate
    {
        public string GetTemplate( string baseTemplate, IJ4JLoggerConfiguration config )
        {
            var sb = new StringBuilder(
                string.IsNullOrEmpty( baseTemplate )
                    ? J4JLoggerConfiguration.DefaultMessageTemplate
                    : baseTemplate
            );

            foreach( var element in EnumUtils.GetUniqueFlags<EventElements>() )
            {
                sb.Append( config.MultiLineEvents ? "{NewLine}" : " " );

                switch( element )
                {
                    case EventElements.Type:
                        sb.Append( "{SourceContext}::{CallingMember}" );
                        break;

                    case EventElements.SourceCode:
                        sb.Append( "{SourceFile}:{LineNumber}" );
                        break;
                }
            }

            sb.Append( "{NewLine}{Exception}" );

            return sb.ToString();
        }
    }
}