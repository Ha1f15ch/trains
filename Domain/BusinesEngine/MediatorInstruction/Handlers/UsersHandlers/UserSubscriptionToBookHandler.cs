using BusinesEngine.MediatorInstruction.Commands.UsersCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.UsersHandlers
{
	public class UserSubscriptionToBookHandler : IRequestHandler<UserSubscriptionToBookCommand, Subscription>
	{
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly ILogger<UserSubscriptionToBookHandler> _logger;

		public UserSubscriptionToBookHandler(ISubscriptionRepository subscriptionRepository, ILogger<UserSubscriptionToBookHandler> logger)
		{
			_subscriptionRepository = subscriptionRepository;
			_logger = logger;
		}

		public async Task<Subscription> Handle(UserSubscriptionToBookCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка подписки пользователя с ID {UserId} на книгу с ID {BookId}", request.UserId, request.BookId);

				var subscription = await _subscriptionRepository.SubscribeUserToBook(request.UserId, request.BookId);

				if (subscription is null)
				{
					_logger.LogWarning("Подписка пользователя с ID {UserId} на книгу с ID {BookId} не удалась", request.UserId, request.BookId);
				}
				else
				{
					_logger.LogInformation("Пользователь с ID {UserId} успешно подписан на книгу с ID {BookId}", request.UserId, request.BookId);
				}

				return subscription;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при подписке пользователя с ID {UserId} на книгу с ID {BookId}", request.UserId, request.BookId);
				throw;
			}
		}
	}
}
