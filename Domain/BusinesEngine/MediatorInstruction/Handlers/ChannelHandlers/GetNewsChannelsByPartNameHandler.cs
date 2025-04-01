using BusinesEngine.MediatorInstruction.Commands.ChannelCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.ChannelHandlers
{
	public class GetNewsChannelsByPartNameHandler : IRequestHandler<GetNewsChannelsByPartNameCommand, List<NewsChannel>>
	{
		private readonly INewsChannelRepository _channelRepository;
		private readonly ILogger<GetNewsChannelsByPartNameHandler> _logger;

		public GetNewsChannelsByPartNameHandler(INewsChannelRepository newsChannelRepository, ILogger<GetNewsChannelsByPartNameHandler> logger)
		{
			_channelRepository = newsChannelRepository;
			_logger = logger;
		}

		public async Task<List<NewsChannel>> Handle(GetNewsChannelsByPartNameCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка поиска новостных каналов по части названия: {PartChannelName}", request.PartChannelName);

				var newsChannels = await _channelRepository.GetNewsChannelsByPartName(request.PartChannelName);

				if (!newsChannels.Any())
				{
					_logger.LogWarning("Новостные каналы, содержащие часть названия '{PartChannelName}', не найдены", request.PartChannelName);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} новостных каналов, содержащих часть названия '{PartChannelName}'", newsChannels.Count, request.PartChannelName);
				}

				return newsChannels;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при поиске новостных каналов по части названия: {PartChannelName}", request.PartChannelName);
				throw;
			}
		}
	}
}
