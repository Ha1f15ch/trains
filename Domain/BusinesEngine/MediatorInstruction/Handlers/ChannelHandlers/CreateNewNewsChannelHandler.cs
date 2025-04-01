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
	public class CreateNewNewsChannelHandler : IRequestHandler<CreateNewNewsChannelCommand, NewsChannel?>
	{
		private readonly INewsChannelRepository _newsChannelRepository;
		private readonly ILogger<CreateNewNewsChannelHandler> _logger;

		public CreateNewNewsChannelHandler(INewsChannelRepository newsChannelRepository, ILogger<CreateNewNewsChannelHandler> logger)
		{
			_newsChannelRepository = newsChannelRepository;
			_logger = logger;
		}

		public async Task<NewsChannel?> Handle(CreateNewNewsChannelCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка создания нового новостного канала. Параметры: Name = {Name}, Description = {Description}, DateCreated = {DateCreated}",
					request.NewsChannel.Name, request.NewsChannel.Description, request.NewsChannel.DateCreated);

				var newsChannel = await _newsChannelRepository.CreateNewNewsChannel(request.NewsChannel);

				if (newsChannel is null)
				{
					_logger.LogWarning("Создание новостного канала не удалось. Параметры: Name = {Name}", request.NewsChannel.Name);
				}
				else
				{
					_logger.LogInformation("Новостной канал успешно создан. ID: {NewsChannelId}, Name: {Name}", newsChannel.Id, newsChannel.Name);
				}

				return newsChannel;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при создании новостного канала. Параметры: Name = {Name}, Description = {Description}, DateCreated = {DateCreated}",
					request.NewsChannel.Name, request.NewsChannel.Description, request.NewsChannel.DateCreated);
				throw;
			}
		}
	}
}
