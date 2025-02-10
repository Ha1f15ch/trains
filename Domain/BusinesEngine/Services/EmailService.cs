using BusinesEngine.Services.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Services
{
	public class EmailService : IEmailService
	{
		public async Task SendEmailAsync(string email, string subject, string message)
		{
			using (var smtpClient = new SmtpClient("smtp.example.com"))
			{
				var mailMessage = new MailMessage("from@example.com", email, subject, message);
				await smtpClient.SendMailAsync(mailMessage);
			}
		}
	}
}
