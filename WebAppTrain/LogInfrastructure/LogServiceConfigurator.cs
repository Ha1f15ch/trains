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
            string logPath = _config["Files:LogPath"] ?? "logs";

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose() // уровень логирования
                .WriteTo.Console() // в консоль
                .WriteTo.File( // в файл
                    Path.Combine(logPath, "log.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {Message}{NewLine}{Exception}"
                )
                .CreateLogger();
        }
    }
}
