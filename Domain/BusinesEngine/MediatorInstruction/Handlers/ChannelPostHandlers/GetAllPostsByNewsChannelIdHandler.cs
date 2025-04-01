using BusinesEngine.MediatorInstruction.Commands.ChannelPost.Queries;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.ChannelPostHandlers
{
	public class GetAllPostsByNewsChannelIdHandler : IRequestHandler<GetAllPostsByNewsChannelIdQuery, List<NewsChannelsPosts>>
	{
		private readonly INewsChannelsPostsRepository _newsChannelsPostsRepository;
		private readonly ILogger<GetAllPostsByNewsChannelIdHandler> _logger;

		public GetAllPostsByNewsChannelIdHandler(INewsChannelsPostsRepository newsChannelsPostsRepository, ILogger<GetAllPostsByNewsChannelIdHandler> logger)
		{
			_newsChannelsPostsRepository = newsChannelsPostsRepository;
			_logger = logger;
		}

		public async Task<List<NewsChannelsPosts>> Handle(GetAllPostsByNewsChannelIdQuery request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка получения всех постов для новостного канала с ID: {NewsChannelId}", request.NewsChannelId);

				var posts = await _newsChannelsPostsRepository.GetAllPostsByNewsChannelId(request.NewsChannelId);

				if (!posts.Any())
				{
					_logger.LogWarning("Посты для новостного канала с ID {NewsChannelId} не найдены", request.NewsChannelId);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} постов для новостного канала с ID {NewsChannelId}", posts.Count, request.NewsChannelId);
				}

				return posts;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении постов для новостного канала с ID: {NewsChannelId}", request.NewsChannelId);
				throw;
			}
		}
	}
}
