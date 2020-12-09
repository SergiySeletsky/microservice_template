using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace MicroService.Core.Logger
{
    internal class LoggerService : ILoggerService
    {
        private readonly ILogger log;

        public LoggerService()
        {
            var config = new LoggingConfiguration();
            // Create targets and add them to the configuration
            var consoleTarget = new ColoredConsoleTarget
            {
                Layout = @"${date:format=HH\:mm\:ss} | ${level} | ${gdc:ModuleName} | ${message}"
            };
            if (Environment.UserInteractive)
            {
                config.AddTarget("console", consoleTarget);
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));
            }

            LogManager.Configuration = config;

            log = LogManager.GetLogger("Log");
        }

        public void Initialize()
        {
            log.Info("Logger service started");
        }

        public void Debug(string message, params object[] @params)
        {
            log.Debug(message, @params);
        }

        public void Error(Exception exception, string message, params object[] @params)
        {
            log.Error(exception, message, @params);
        }

        public void Info(string message, params object[] @params)
        {
            log.Info(message, @params);
        }

        public void Warn(string message, params object[] @params)
        {
            log.Warn(message, @params);
        }

        public void Dispose()
        {
            LogManager.Shutdown();
        }
    }
}
