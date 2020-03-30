using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace Ketchup.Log4net
{
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly string _log4NetConfigFile;
        private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers =
            new ConcurrentDictionary<string, Log4NetLogger>();

        public Log4NetProvider(string log4NetConfigFile)
        {
            _log4NetConfigFile = log4NetConfigFile;
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
        }

        private Log4NetLogger CreateLoggerImplementation(string name)
        {
            return new Log4NetLogger(name, Parselog4NetConfigFile(_log4NetConfigFile));
        }

        private static XmlElement Parselog4NetConfigFile(string filename)
        {
            XmlDocument log4netConfig = new XmlDocument();
            var stream = File.OpenRead(filename);
            log4netConfig.Load(stream);
            stream.Close();
            return log4netConfig["log4net"];
        }
    }
}
