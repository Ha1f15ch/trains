using BusinesEngine.Services.ServiceInterfaces;
using Serilog;

namespace WebAppTrain.LogInfrastructure
{
	public class LogService : ILogService
	{
		private readonly Serilog.ILogger _logger = Log.Logger;

		public void LogDebug(string message)
		{
			_logger.Debug($"|DEBUG| {message}");
		}

		public void LogError(string message)
		{
			_logger.Error($"|ERROR| {message}");
		}

		public void LogInformation(string message)
		{
			_logger.Information($"|INFO| {message}");
		}

		public void LogWarning(string message)
		{
			_logger.Warning($"|WARN| {message}");
		}
	}
}
