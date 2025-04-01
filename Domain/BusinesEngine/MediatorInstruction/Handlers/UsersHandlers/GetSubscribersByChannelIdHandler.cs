using BusinesEngine.MediatorInstruction.Commands.UsersCommand.Queries;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.UsersHandlers
{
	public class GetSubscribersByChannelIdHandler : IRequestHandler<GetSubscribersByChannelIdQuery, List<User>>
	{
		private readonly INewsChannelsSubscribersRepository _newsChannelsSubscribersRepository;
		private readonly ILogger<GetSubscribersByChannelIdHandler> _logger;

		public GetSubscribersByChannelIdHandler(INewsChannelsSubscribersRepository newsChannelsSubscribersRepository, ILogger<GetSubscribersByChannelIdHandler> logger)
		{
			_newsChannelsSubscribersRepository = newsChannelsSubscribersRepository;
			_logger = logger;
		}

		public async Task<List<User>> Handle(GetSubscribersByChannelIdQuery request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка получения подписчиков для новостного канала с ID: {NewsChannelId}", request.NewsChannelId);

				var subscribers = await _newsChannelsSubscribersRepository.GetSubscribersByChannelId(request.NewsChannelId);

				if (!subscribers.Any())
				{
					_logger.LogWarning("Подписчики для новостного канала с ID {NewsChannelId} не найдены", request.NewsChannelId);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} подписчиков для новостного канала с ID {NewsChannelId}", subscribers.Count, request.NewsChannelId);
				}

				return subscribers;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении подписчиков для новостного канала с ID: {NewsChannelId}", request.NewsChannelId);
				throw;
			}
		}
	}
}
