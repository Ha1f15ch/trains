using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Services
{
	public class EmailNotificationService
	{
		private readonly ILogger<EmailNotificationService> _logger;

		public EmailNotificationService(ILogger<EmailNotificationService> logger)
		{
			_logger = logger;
		}

		public async Task SendEmailAsync(string recipientEmail, string subject, string body)
		{
			try
			{
				using (var smtpClient = new SmtpClient("smtp.yandex.ru", 587))
				{
					smtpClient.Credentials = new NetworkCredential("H41f1sch@yandex.ru", "sulofqstnandgroc");//MyTestSiteForNotification - в яндексе, название пароля (sulofqstnandgroc)
					smtpClient.EnableSsl = true;

					var mailMessage = new MailMessage
					{
						From = new MailAddress("H41f1sch@yandex.ru"),
						Subject = "Новое уведомление от новостного канала",
						Body = body,
						IsBodyHtml = true
					};
					mailMessage.To.Add(recipientEmail);

					await smtpClient.SendMailAsync(mailMessage);
					_logger.LogInformation($"Успешно отправлено письмо на {recipientEmail}");
				}
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, $"Ошибка при отправке письма на {recipientEmail}");
			}
		}
	}
}
