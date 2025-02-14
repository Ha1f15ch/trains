using BusinesEngine.Events.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Events
{
	public class LogSubscriber : INewsSubscriber
	{
		private readonly ILogger<LogSubscriber> _logger;

		public LogSubscriber(ILogger<LogSubscriber> logger)
		{
			_logger = logger;
		}

		public async Task Update(string message)
		{
			await Task.Run(() =>
			{
				_logger.LogInformation($"Уведомление - {message}");
			});
		}
	}
}
