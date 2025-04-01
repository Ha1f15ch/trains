using BusinesEngine.MediatorInstruction.Commands.UsersCommand.Queries;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.UsersHandlers
{
	public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<User>>
	{
		private readonly IUserRepository _userRepository;
		private readonly ILogger<GetAllUsersHandler> _logger;

		public GetAllUsersHandler(IUserRepository userRepository, ILogger<GetAllUsersHandler> logger)
		{
			_userRepository = userRepository;
			_logger = logger;
		}

		public async Task<List<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка получения всех пользователей");

				var users = await _userRepository.GetAllUsers();

				if (!users.Any())
				{
					_logger.LogWarning("Пользователи в базе данных отсутствуют");
				}
				else
				{
					_logger.LogInformation("Успешно получено {Count} пользователей", users.Count);
				}

				return users;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении всех пользователей");
				throw;
			}
		}
	}
}
