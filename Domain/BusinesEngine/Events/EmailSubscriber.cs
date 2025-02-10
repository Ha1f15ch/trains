using BusinesEngine.Events.Interfaces;
using BusinesEngine.Services.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Events
{
	public class EmailSubscriber : INewsSubscriber
	{
		private readonly IEmailService _emailService;

		public EmailSubscriber(IEmailService emailService)
		{
			_emailService = emailService;
		}

		public async Task Update(string message)
		{
			await Task.Run(async () =>
			{
				var subject = "Новое сообщение !";
				var body = message;

				await _emailService.SendEmailAsync("alexy.alexy98@mail.ru", subject, body);
			});
		}
	}
}
