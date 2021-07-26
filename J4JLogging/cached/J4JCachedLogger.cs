﻿#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLogging' is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
// 
// This library or program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with
// this library or program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using Serilog.Events;

#pragma warning disable 8604

namespace J4JSoftware.Logging
{
    public class J4JCachedLogger : IJ4JLogger
    {
        private bool _outputNextToSms;
        private readonly J4JLoggerCache _cache;

        public J4JCachedLogger( J4JLoggerCache? cache = null )
        {
            Cache = cache ?? new J4JLoggerCache();

            _cache = new J4JLoggerCache();
        }

        public J4JLoggerCache Cache { get; }

        public IJ4JLogger SetLoggedType<TLogged>() => SetLoggedType( typeof( TLogged ) );

        public IJ4JLogger SetLoggedType( Type typeToLog )
        {
            _cache.Context = _cache.Context with { LoggedType = typeToLog };

            return this;
        }

        public IJ4JLogger ClearLoggedType()
        {
            _cache.Context = _cache.Context with { LoggedType = null };

            return this;
        }

        public IJ4JLogger IncludeSourcePath()
        {
            _cache.Context = _cache.Context with { IncludeSourcePath = true };

            return this;
        }

        public IJ4JLogger ExcludeSourcePath()
        {
            _cache.Context = _cache.Context with { IncludeSourcePath = false };

            return this;
        }

        public IJ4JLogger SetSourceRootPath( string path )
        {
            _cache.Context = _cache.Context with { SourcePathRoot = path };

            return this;
        }

        public IJ4JLogger ClearSourceRootPath()
        {
            _cache.Context = _cache.Context with { SourcePathRoot = null };

            return this;
        }

        public IJ4JLogger OutputMultiLineEvents()
        {
            _cache.Context = _cache.Context with { OutputMultiLineEvents = true };

            return this;
        }

        public IJ4JLogger OutputSingleLineEvents()
        {
            _cache.Context = _cache.Context with { OutputMultiLineEvents = false };

            return this;
        }

        public IJ4JLogger AddOutputChannel<TChannel>( TChannel channelConfig ) where TChannel: IChannelConfig
        {
            // irrelevant in a cached logging situation
            return this;
        }

        public IJ4JLogger RemoveOutputChannel<TChannel>() where TChannel: IChannelConfig
        {
            // irrelevant in a cached logging situation
            return this;
        }

        // do nothing; no reason to output cache when we're caching entries
        public bool OutputCache( J4JLoggerCache cache )
        {
            return false;
        }

        public IJ4JLogger OutputNextEventToSms()
        {
            _outputNextToSms = true;

            _cache.Context = _cache.Context with { OutputToSms = true };

            return this;
        }

        private void ResetSms()
        {
            if( _outputNextToSms )
            {
                _cache.Context = _cache.Context with { OutputToSms = false };
                _outputNextToSms = false;
            }
        }

        public void Write( LogEventLevel level, string template, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            _cache.Context.Add( level, template, memberName, srcPath, srcLine );
            ResetSms();
        }

        public void Write<T0>( LogEventLevel level, string template, T0 propertyValue, string memberName = "",
            string srcPath = "",
            int srcLine = 0 )
        {
            _cache.Context.Add( level, template, memberName, srcPath, srcLine, propertyValue );
            _outputNextToSms = false;
        }

        public void Write<T0, T1>( LogEventLevel level, string template, T0 propertyValue0, T1 propertyValue1,
            string memberName = "",
            string srcPath = "", int srcLine = 0 )
        {
            _cache.Context.Add( level, template, memberName, srcPath, srcLine, propertyValue0, propertyValue1 );
            ResetSms();
        }

        public void Write<T0, T1, T2>( LogEventLevel level, string template, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            _cache.Context.Add( level, template, memberName, srcPath, srcLine, propertyValue0, propertyValue1,
                propertyValue2 );
            ResetSms();
        }

        public void Write( LogEventLevel level, string template, object[] propertyValues, string memberName = "",
            string srcPath = "",
            int srcLine = 0 )
        {
            _cache.Context.Add( level, template, memberName, srcPath, srcLine, propertyValues );
            ResetSms();
        }

        public void Debug( string template, string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Debug, template, memberName, srcPath, srcLine );
        }

        public void Debug<T0>( string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Debug, template, propertyValue, memberName, srcPath, srcLine );
        }

        public void Debug<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Debug, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );
        }

        public void Debug<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Debug, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine );
        }

        public void Debug( string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Debug, template, propertyValues, memberName, srcPath,
                srcLine );
        }

        public void Error( string template, string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Error, template, memberName, srcPath, srcLine );
        }

        public void Error<T0>( string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Error, template, propertyValue, memberName, srcPath, srcLine );
        }

        public void Error<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Error, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );
        }

        public void Error<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Error, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine );
        }

        public void Error( string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Error, template, propertyValues, memberName, srcPath,
                srcLine );
        }

        public void Fatal( string template, string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Fatal, template, memberName, srcPath, srcLine );
        }

        public void Fatal<T0>( string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Fatal, template, propertyValue, memberName, srcPath, srcLine );
        }

        public void Fatal<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Fatal, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );
        }

        public void Fatal<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Fatal, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine );
        }

        public void Fatal( string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Fatal, template, propertyValues, memberName, srcPath,
                srcLine );
        }

        public void Information( string template, string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Information, template, memberName, srcPath, srcLine );
        }

        public void Information<T0>( string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Information, template, propertyValue, memberName, srcPath, srcLine );
        }

        public void Information<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Information, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );
        }

        public void Information<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Information, template, propertyValue0, propertyValue1, propertyValue2, memberName,
                srcPath,
                srcLine );
        }

        public void Information( string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Information, template, propertyValues, memberName, srcPath,
                srcLine );
        }

        public void Verbose( string template, string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Verbose, template, memberName, srcPath, srcLine );
        }

        public void Verbose<T0>( string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Verbose, template, propertyValue, memberName, srcPath, srcLine );
        }

        public void Verbose<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Verbose, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );
        }

        public void Verbose<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Verbose, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine );
        }

        public void Verbose( string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Verbose, template, propertyValues, memberName, srcPath,
                srcLine );
        }

        public void Warning( string template, string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Warning, template, memberName, srcPath, srcLine );
        }

        public void Warning<T0>( string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Warning, template, propertyValue, memberName, srcPath, srcLine );
        }

        public void Warning<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Warning, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );
        }

        public void Warning<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            Write( LogEventLevel.Warning, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine );
        }

        public void Warning( string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            Write( LogEventLevel.Warning, template, propertyValues, memberName, srcPath,
                srcLine );
        }
    }
}