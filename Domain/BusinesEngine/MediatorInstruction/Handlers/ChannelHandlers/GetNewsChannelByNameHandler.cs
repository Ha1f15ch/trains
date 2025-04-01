using BusinesEngine.MediatorInstruction.Commands.ChannelCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.ChannelHandlers
{
	public class GetNewsChannelByNameHandler : IRequestHandler<GetNewsChannelByNameCommand, NewsChannel?>
	{
		private readonly INewsChannelRepository _newsChannelRepository;
		private readonly ILogger<GetNewsChannelByNameHandler> _logger;

		public GetNewsChannelByNameHandler(INewsChannelRepository newsChannelRepository, ILogger<GetNewsChannelByNameHandler> logger)
		{
			_newsChannelRepository = newsChannelRepository;
			_logger = logger;
		}

		public async Task<NewsChannel?> Handle(GetNewsChannelByNameCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка получения новостного канала по имени: {FullNewsChannelName}", request.FullNewsChannelName);

				var newsChannel = await _newsChannelRepository.GetNewsChannelByName(request.FullNewsChannelName);

				if (newsChannel is null)
				{
					_logger.LogWarning("Новостной канал с именем '{FullNewsChannelName}' не найден", request.FullNewsChannelName);
				}
				else
				{
					_logger.LogInformation("Новостной канал успешно найден. ID: {NewsChannelId}, Name: {Name}", newsChannel.Id, newsChannel.Name);
				}

				return newsChannel;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении новостного канала по имени: {FullNewsChannelName}", request.FullNewsChannelName);
				throw;
			}
		}
	}
}
