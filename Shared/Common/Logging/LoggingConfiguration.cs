using Serilog;
using Serilog.Events;

namespace Common.Logging
{
	public class LoggingConfiguration
	{
		public static void Configure()
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.Enrich.FromLogContext()
				.Enrich.WithProperty("Application", "WebAppTrain")
				.WriteTo.Console(
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
				)
				.WriteTo.File(
					"logs/log.txt",
					rollingInterval: RollingInterval.Day,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
					retainedFileCountLimit: 7
				)
				.CreateLogger();
		}
	}
}
