using BusinesEngine.MediatorInstruction.Commands.UsersCommand;
using BusinesEngine.Services;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Integrations.RabbitMqInfrastructure;
using System.Text.Json;
using DTOModels.ValidationService;

namespace BusinesEngine.MediatorInstruction.Handlers.UsersHandlers
{
	public class CreateNewUserHandler : IRequestHandler<CreateNewUserCommand, string?>
	{
		private readonly IUserRepository _userRepository;
		private readonly JsonStringHandlerService _jsonStringHandlerService;
		private readonly ILogger<CreateNewUserHandler> _logger;
		private readonly RabbitMqService _rabbitMqService;

		public CreateNewUserHandler(IUserRepository userRepository, JsonStringHandlerService jsonStringHandlerService, ILogger<CreateNewUserHandler> logger, RabbitMqService rabbitMqService)
		{
			_userRepository = userRepository;
			_jsonStringHandlerService = jsonStringHandlerService;
			_logger = logger;
			_rabbitMqService = rabbitMqService;
		}

		public async Task<string?> Handle(CreateNewUserCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка создания нового пользователя. Параметры: Name = {Name}, Email = {Email}, IsActive = {IsActive}",
					request.Name, request.Email, request.IsActive);

				// генерируем уникальный requestId для запроса в очереди
				var requestR = Guid.NewGuid().ToString();

				//Отправляем данные на валидацию через RabbitMQ
				await _rabbitMqService.PublishValidationRequestAsync(requestR, new NewUserData
				{
					Name = request.Name,
					Email = request.Email,
					Password = request.Password
				});

				// получаем результат валидации 
				var validationResponse = await _rabbitMqService.GetValidationResultAsync(requestR);

				if (validationResponse == null || !validationResponse.ValidationResult) // Если валидация не пройдена
				{
					_logger.LogWarning($"Ошибка валидации. Валидация не пройдена. RequestId: {requestR}");
					return null;
				}

				_logger.LogInformation($"Валидация выполнена успешно. Вызываем процедуру создания пользователя по переданным параметрам . . . .");

				var newUser = await _userRepository.CreateNewUser(
					request.Name,
					request.Email,
					request.Password,
					request.IsActive,
					request.DateCreate,
					request.DateUpdate,
					request.DateDelete
				);

				if (newUser is null)
				{
					_logger.LogWarning("Создание пользователя не удалось. Параметры: Name = {Name}, Email = {Email}", request.Name, request.Email);
					return null;
				}

				_logger.LogInformation("Пользователь успешно создан. ID: {UserId}, Name: {Name}, Email: {Email}", newUser.Id, newUser.Name, newUser.Email);

				var serializedResult = await _jsonStringHandlerService.SerializeSingle(newUser);

				_logger.LogInformation("Сериализация данных пользователя завершена");

				return serializedResult;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при создании пользователя. Параметры: Name = {Name}, Email = {Email}, IsActive = {IsActive}",
					request.Name, request.Email, request.IsActive);
				throw;
			}
		}
	}
}
