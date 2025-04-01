using BusinesEngine.MediatorInstruction.Commands.ChannelPost.Queries;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.ChannelPostHandlers
{
	public class GetAllPostsByPartTitleHandle : IRequestHandler<GetAllPostsByPartTitleQuery, List<NewsChannelsPosts>>
	{
		private readonly INewsChannelsPostsRepository _newsChannelsPostsRepository;
		private readonly ILogger<GetAllPostsByPartTitleHandle> _logger;

		public GetAllPostsByPartTitleHandle(INewsChannelsPostsRepository newsChannelsPostsRepository, ILogger<GetAllPostsByPartTitleHandle> logger)
		{
			_newsChannelsPostsRepository = newsChannelsPostsRepository;
			_logger = logger;
		}

		public async Task<List<NewsChannelsPosts>> Handle(GetAllPostsByPartTitleQuery request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка поиска постов по части заголовка: {PartNewsChannelPostsName}", request.PartNewsChannelPostsName);

				var posts = await _newsChannelsPostsRepository.GetAllPostsByPartTitle(request.PartNewsChannelPostsName);

				if (!posts.Any())
				{
					_logger.LogWarning("Посты, содержащие часть заголовка '{PartNewsChannelPostsName}', не найдены", request.PartNewsChannelPostsName);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} постов, содержащих часть заголовка '{PartNewsChannelPostsName}'", posts.Count, request.PartNewsChannelPostsName);
				}

				return posts;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при поиске постов по части заголовка: {PartNewsChannelPostsName}", request.PartNewsChannelPostsName);
				throw;
			}
		}
	}
}
