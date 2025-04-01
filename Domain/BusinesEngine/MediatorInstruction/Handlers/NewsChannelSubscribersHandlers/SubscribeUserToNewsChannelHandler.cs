using BusinesEngine.MediatorInstruction.Commands.NewsChannelSubscribersCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.NewsChannelSubscribersHandlers
{
	public class SubscribeUserToNewsChannelHandler : IRequestHandler<SubscribeUserToNewsChannelCommand, NewsChannelsSubscribers>
	{
		private readonly INewsChannelsSubscribersRepository _newsChannelsSubscribersRepository;
		private readonly ILogger<SubscribeUserToNewsChannelHandler> _logger;

		public SubscribeUserToNewsChannelHandler(INewsChannelsSubscribersRepository newsChannelsSubscribersRepository, ILogger<SubscribeUserToNewsChannelHandler> logger)
		{
			_newsChannelsSubscribersRepository = newsChannelsSubscribersRepository;
			_logger = logger;
		}

		public async Task<NewsChannelsSubscribers> Handle(SubscribeUserToNewsChannelCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка подписки пользователя с ID {UserId} на новостной канал с ID {NewsChannelId}", request.UserId, request.NewsChannelId);

				var subscription = await _newsChannelsSubscribersRepository.SubscribeUserToNewsChannel(request.UserId, request.NewsChannelId);

				if (subscription is null)
				{
					_logger.LogWarning("Подписка пользователя с ID {UserId} на новостной канал с ID {NewsChannelId} не удалась", request.UserId, request.NewsChannelId);
				}
				else
				{
					_logger.LogInformation("Пользователь с ID {UserId} успешно подписан на новостной канал с ID {NewsChannelId}", request.UserId, request.NewsChannelId);
				}

				return subscription;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при подписке пользователя с ID {UserId} на новостной канал с ID {NewsChannelId}", request.UserId, request.NewsChannelId);
				throw;
			}
		}
	}
}
