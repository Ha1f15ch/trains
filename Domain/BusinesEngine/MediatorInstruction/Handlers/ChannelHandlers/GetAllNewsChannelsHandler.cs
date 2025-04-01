using BusinesEngine.MediatorInstruction.Commands.ChannelCommand.Queries;
using BusinesEngine.Services;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.ChannelHandlers
{
	public class GetAllNewsChannelsHandler : IRequestHandler<GetAllNewsChannelsQuery, string>
	{
		private readonly INewsChannelRepository _newsChannelRepository;
		private readonly JsonStringHandlerService _jsonStringHandlerService;
		private readonly ILogger<GetAllNewsChannelsHandler> _logger;

		public GetAllNewsChannelsHandler(INewsChannelRepository newsChannelRepository, JsonStringHandlerService jsonStringHandlerService, ILogger<GetAllNewsChannelsHandler> logger)
		{
			_newsChannelRepository = newsChannelRepository;
			_jsonStringHandlerService = jsonStringHandlerService;
			_logger = logger;
		}

		public async Task<string> Handle(GetAllNewsChannelsQuery request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка получения всех новостных каналов");

				var newsChannels = await _newsChannelRepository.GetAllNewsChannels();

				if (!newsChannels.Any())
				{
					_logger.LogWarning("Новостные каналы в базе данных отсутствуют");
				}
				else
				{
					_logger.LogInformation("Успешно получено {Count} новостных каналов", newsChannels.Count);
				}

				var serializedResult = await _jsonStringHandlerService.SerializeList(newsChannels);

				_logger.LogInformation("Сериализация списка новостных каналов завершена");

				return serializedResult;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении или сериализации новостных каналов");
				throw;
			}
		}
	}
}
