using BusinesEngine.MediatorInstruction.Commands.UsersCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.UsersHandlers
{
	public class GetUserByUserIdHandler : IRequestHandler<GetUserByIdCommand, User?>
	{
		private readonly IUserRepository _userRepository;
		private readonly ILogger<GetUserByUserIdHandler> _logger;

		public GetUserByUserIdHandler(IUserRepository userRepository, ILogger<GetUserByUserIdHandler> logger)
		{
			_userRepository = userRepository;
			_logger = logger;
		}

		public async Task<User?> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка получения пользователя по ID: {UserId}", request.UserId);

				var user = await _userRepository.GetUserById(request.UserId);

				if (user is null)
				{
					_logger.LogWarning("Пользователь с ID {UserId} не найден", request.UserId);
				}
				else
				{
					_logger.LogInformation("Пользователь успешно найден. ID: {UserId}, Name: {Name}, Email: {Email}",
						user.Id, user.Name, user.Email);
				}

				return user;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении пользователя по ID: {UserId}", request.UserId);
				throw;
			}
		}
	}
}
