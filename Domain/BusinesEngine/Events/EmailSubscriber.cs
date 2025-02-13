using BusinesEngine.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Events
{
	public class EmailSubscriber : IEmailSubscriber
	{
		private readonly string _email;

		public EmailSubscriber(string email)
		{
			_email = email;
		}

		public string Email => _email;

		public async Task Update(string message)
		{
			using(var smtpClient = new SmtpClient("smtp.yandex.ru", 465))
			{
				smtpClient.Credentials = new NetworkCredential("H41f1sch", "Qazxc0987");
				smtpClient.EnableSsl = false;

				var mailMessage = new MailMessage
				{
					From = new MailAddress("H41f1sch@yandex.ru"),
					Subject = "Новое уведомление от новостного канала",
					Body = message,
					IsBodyHtml = true
				};
				mailMessage.To.Add(_email);

				await smtpClient.SendMailAsync(mailMessage);
			}
		}
	}
}
