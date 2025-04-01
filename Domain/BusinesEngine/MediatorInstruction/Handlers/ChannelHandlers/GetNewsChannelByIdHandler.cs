using BusinesEngine.MediatorInstruction.Commands.ChannelCommand;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Handlers.ChannelHandlers
{
	public class GetNewsChannelByIdHandler : IRequestHandler<GetNewsChannelByIdCommand, NewsChannel?>
	{
		private readonly INewsChannelRepository _channelRepository;
		private readonly ILogger<GetNewsChannelByIdHandler> _logger;

		public GetNewsChannelByIdHandler(INewsChannelRepository newsChannelRepository, ILogger<GetNewsChannelByIdHandler> logger)
		{
			_channelRepository = newsChannelRepository;
			_logger = logger;
		}

		public async Task<NewsChannel?> Handle(GetNewsChannelByIdCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка получения новостного канала по ID: {NewsChannelId}", request.NewsChannelId);

				var newsChannel = await _channelRepository.GetNewsChannelById(request.NewsChannelId);

				if (newsChannel is null)
				{
					_logger.LogWarning("Новостной канал с ID {NewsChannelId} не найден", request.NewsChannelId);
				}
				else
				{
					_logger.LogInformation("Новостной канал успешно найден. ID: {NewsChannelId}, Name: {Name}", newsChannel.Id, newsChannel.Name);
				}

				return newsChannel;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении новостного канала по ID: {NewsChannelId}", request.NewsChannelId);
				throw;
			}
		}
	}
}
