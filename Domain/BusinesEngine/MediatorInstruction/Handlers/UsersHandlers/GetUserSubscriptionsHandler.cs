using BusinesEngine.MediatorInstruction.Commands.UsersCommand.Queries;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.UsersHandlers
{
	public class GetUserSubscriptionsHandler : IRequestHandler<GetUserSubscriptionsQuery, List<Subscription>>
	{
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly ILogger<GetUserSubscriptionsHandler> _logger;

		public GetUserSubscriptionsHandler(ISubscriptionRepository subscriptionRepository, ILogger<GetUserSubscriptionsHandler> logger)
		{
			_subscriptionRepository = subscriptionRepository;
			_logger = logger;
		}

		public async Task<List<Subscription>> Handle(GetUserSubscriptionsQuery request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка получения подписок пользователя с ID: {UserId}", request.UserId);

				var subscriptions = await _subscriptionRepository.GetUserSubscriptions(request.UserId);

				if (!subscriptions.Any())
				{
					_logger.LogWarning("Подписки для пользователя с ID {UserId} не найдены", request.UserId);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} подписок для пользователя с ID {UserId}", subscriptions.Count, request.UserId);
				}

				return subscriptions;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении подписок пользователя с ID: {UserId}", request.UserId);
				throw;
			}
		}
	}
}
