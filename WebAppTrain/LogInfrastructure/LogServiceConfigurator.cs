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
                .MinimumLevel.Verbose()
                .WriteTo.Async(a => a.Console())
                .WriteTo.Async(a => a.File(Path.Combine(logPath, "log.txt"), rollingInterval: RollingInterval.Day))
                .WriteTo.Seq("https://localhost:5431")
                .CreateLogger();
        }
    }
}
