using BusinesEngine.MediatorInstruction.Commands.ChannelPost;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinesEngine.MediatorInstruction.Handlers.ChannelPostHandlers
{
	public class CreateNewNewsChannelsPostHandler : IRequestHandler<CreateNewNewsChannelsPostCommand, NewsChannelsPosts?>
	{
		private readonly INewsChannelsPostsRepository _newsChannelsPostsRepository;
		private readonly ILogger<CreateNewNewsChannelsPostHandler> _logger;

		public CreateNewNewsChannelsPostHandler(INewsChannelsPostsRepository newsChannelsPostsRepository, ILogger<CreateNewNewsChannelsPostHandler> logger)
		{
			_newsChannelsPostsRepository = newsChannelsPostsRepository;
			_logger = logger;
		}

		public async Task<NewsChannelsPosts?> Handle(CreateNewNewsChannelsPostCommand request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Попытка создания нового поста для новостного канала. Параметры: NewsChannelId = {NewsChannelId}, TitlePost = {TitlePost}, AuthorPost = {AuthorPost}",
					request.NewsChannelId, request.TitlePost, request.AauthorPost);

				var newPost = await _newsChannelsPostsRepository.CreateNewNewsChannelsPost(
					request.NewsChannelId,
					request.TitlePost,
					request.BodyPost,
					request.FooterPost,
					request.AauthorPost,
					request.SourceImage
				);

				if (newPost is null)
				{
					_logger.LogWarning("Создание поста для новостного канала не удалось. Параметры: NewsChannelId = {NewsChannelId}, TitlePost = {TitlePost}",
						request.NewsChannelId, request.TitlePost);
				}
				else
				{
					_logger.LogInformation("Пост успешно создан. ID: {PostId}, Title: {TitlePost}, Author: {AuthorPost}",
						newPost.Id, newPost.TitlePost, newPost.AauthorPost);
				}

				return newPost;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при создании нового поста для новостного канала. Параметры: NewsChannelId = {NewsChannelId}, TitlePost = {TitlePost}, AuthorPost = {AuthorPost}",
					request.NewsChannelId, request.TitlePost, request.AauthorPost);
				throw;
			}
		}
	}
}
