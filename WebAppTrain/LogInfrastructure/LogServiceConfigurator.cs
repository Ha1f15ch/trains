using Serilog;

namespace WebApiApp.LogInfrastructure
{
    public class LogServiceConfigurator
    {
        public static void Configure()
        {
            string logPath = "F:\\les_folder_csh\\WebApiApp\\LogInfrastructure\\Logs\\";

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
