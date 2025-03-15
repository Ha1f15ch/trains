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

		//Отправка на почту, настройка smtp сервера по отправке уведомлений
		public async Task SendEmailAsync(string recipientEmail, string subject, string body)
		{
			try
			{
				using (var smtpClient = new SmtpClient("smtp.yandex.ru", 587)) //Каким сервером и с какого порта будет выполняться отправка и работа сервиса
				{
					smtpClient.Credentials = new NetworkCredential("H41f1sch@yandex.ru", "sulofqstnandgroc");//MyTestSiteForNotification - в яндексе, название пароля (sulofqstnandgroc)
					smtpClient.EnableSsl = true; // Без этого параметра не работает

					//Формирование сообщения
					var mailMessage = new MailMessage
					{
						From = new MailAddress("H41f1sch@yandex.ru"),// от кого
						Subject = "Новое уведомление от новостного канала", //тема письма
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
