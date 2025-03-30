using BusinesEngine.Services;
using BusinesEngine.Services.ServiceInterfaces;
using DatabaseEngine.RepositoryStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangfireTestProj.Jobs
{
    public class EmailSenderWeekDelayJob
    {
		private readonly EmailNotificationService _emailNotificationService;

		private readonly IUserRepository _userRepository;

		private readonly ILogService _logService;

		public EmailSenderWeekDelayJob(
			EmailNotificationService emailNotificationService,
			IUserRepository userRepository,
			ILogService logService)
		{
			_emailNotificationService = emailNotificationService;
			_userRepository = userRepository;
			_logService = logService;
		}

		public async Task SendEmail()
		{
			_logService.LogInformation($"Получаем список всех Email пользователей");
			var emailsForSendingMessages = await _userRepository.GetAllUsersEmail();

			_logService.LogInformation($"Перебираем список всех Email и отправляем на почту сообщения");
			foreach(var email in emailsForSendingMessages)
			{
				try
				{
					_logService.LogInformation($"{nameof(SendEmail)} - Выполняется отправка уведомления на почту - {email}");
					await _emailNotificationService.SendEmailAsync(email, "Мы очень рады видеть Вашу регистрацию на нашем сайте", $"Мы любим Вас! \n\nИ в честь этого дарим Вам подарок. Заходите каждый день с сегодняшнего дня и получайте призы !");
				}
				catch (Exception ex)
				{
					_logService.LogError($"Возникла ошибка при обработке действия - {nameof(SendEmail)} - {ex.Message}");
					throw;
				}
			}
		}
	}
}
