﻿using Serilog;

namespace WebApiApp.LogInfrastructure
{
    public class LogService
    {
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            Log.Information(message);
        }

        public void LogError(string message, Exception ex)
        {
            Log.Error(message, ex);
        }
    }
}
