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
		private readonly Serilog.ILogger _logger;

		public LogSubscriber()
		{
			_logger = Log.Logger;
		}

		//Вывод в логи последнего события (того, что передали в лог)
		public async Task Update(string message)
		{
			await Task.Run(() =>
			{
				_logger.Information($"[Event] {message}");
			});
		}
	}
}
