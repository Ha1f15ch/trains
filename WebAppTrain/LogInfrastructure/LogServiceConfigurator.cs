using Serilog;
using Microsoft.Extensions.Configuration;

namespace WebApiApp.LogInfrastructure
{
    public class LogServiceConfigurator
    {
        private readonly IConfiguration _config;

        public LogServiceConfigurator(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void Configure()
        {
            string logPath = _config["Files:LogPath"];

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(logPath, "log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
