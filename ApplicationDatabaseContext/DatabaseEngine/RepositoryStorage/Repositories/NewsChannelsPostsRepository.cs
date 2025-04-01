using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class NewsChannelsPostsRepository : INewsChannelsPostsRepository
    {
        private readonly AppDbContext _appDbContext;
		private readonly ILogger<NewsChannelsPostsRepository> _logger;

        public NewsChannelsPostsRepository(AppDbContext appDbContext, ILogger<NewsChannelsPostsRepository> logger)
        {
            _appDbContext = appDbContext;
			_logger = logger;
        }

        public async Task<NewsChannelsPosts?> CreateNewNewsChannelsPost(int newChannelId, string titlePost, string bodyPost, string? footerPost, string authorPost, string? sourceImage)
        {
			try
			{
				_logger.LogInformation("Попытка создания нового поста для новостного канала с ID {NewsChannelId}", newChannelId);

				var newsChannel = await _appDbContext.NewsChannels.FindAsync(newChannelId);

				if (newsChannel is null)
				{
					_logger.LogWarning("Новостной канал с ID {NewsChannelId} не найден", newChannelId);
					return null;
				}

				if (string.IsNullOrEmpty(authorPost))
				{
					_logger.LogError("Неверное значение параметра authorPost: {AuthorPost}. Значение не может быть null или пустым.", authorPost);
					return null;
				}

				if (string.IsNullOrEmpty(bodyPost))
				{
					_logger.LogError("Неверное значение параметра bodyPost: {BodyPost}. Значение не может быть null или пустым.", bodyPost);
					return null;
				}

				if (string.IsNullOrEmpty(titlePost))
				{
					_logger.LogError("Неверное значение параметра titlePost: {TitlePost}. Значение не может быть null или пустым.", titlePost);
					return null;
				}

				var newPost = new NewsChannelsPosts
				{
					NewsChannelId = newsChannel.Id,
					TitlePost = titlePost,
					BodyPost = bodyPost,
					FooterPost = footerPost,
					AuthorPost = authorPost,
					SurceImage = sourceImage,
					CreatedDate = DateTime.UtcNow
				};

				await _appDbContext.NewsChannelsPosts.AddAsync(newPost);
				await _appDbContext.SaveChangesAsync();

				_logger.LogInformation("Пост успешно создан. ID новостного канала: {NewsChannelId}, Заголовок: {TitlePost}, Автор: {AuthorPost}",
					newPost.NewsChannelId, newPost.TitlePost, newPost.AuthorPost);

				return newPost;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при создании нового поста для новостного канала с ID {NewsChannelId}", newChannelId);
				return null;
			}
		}

        public async Task<List<NewsChannelsPosts>> GetAllPosts()
        {
			try
			{
				_logger.LogInformation("Попытка получения всех постов");

				var posts = await _appDbContext.NewsChannelsPosts.ToListAsync();

				if (!posts.Any())
				{
					_logger.LogWarning("Посты в базе данных отсутствуют");
				}
				else
				{
					_logger.LogInformation("Успешно получено {Count} постов", posts.Count);
				}

				return posts;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении всех постов");
				return new List<NewsChannelsPosts>();
			}
		}

        public async Task<List<NewsChannelsPosts>> GetAllPostsByNewsChannelId(int newsChannelId)
        {
			try
			{
				_logger.LogInformation("Попытка получения постов для новостного канала с ID {NewsChannelId}", newsChannelId);

				var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

				if (newsChannel is null)
				{
					_logger.LogWarning("Новостной канал с ID {NewsChannelId} не найден", newsChannelId);
					return new List<NewsChannelsPosts>();
				}

				var listPosts = await _appDbContext.NewsChannelsPosts
					.Where(el => el.NewsChannelId == newsChannelId)
					.ToListAsync();

				if (!listPosts.Any())
				{
					_logger.LogWarning("Посты для новостного канала с ID {NewsChannelId} не найдены", newsChannelId);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} постов для новостного канала с ID {NewsChannelId}", listPosts.Count, newsChannelId);
				}

				return listPosts;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении постов для новостного канала с ID {NewsChannelId}", newsChannelId);
				return new List<NewsChannelsPosts>();
			}
		}

        public async Task<List<NewsChannelsPosts>> GetAllPostsByPartTitle(string titlePost)
        {
			try
			{
				_logger.LogInformation("Попытка поиска постов по части заголовка {TitlePost}", titlePost);

				if (string.IsNullOrEmpty(titlePost))
				{
					_logger.LogError("Неверное значение параметра titlePost: {TitlePost}. Значение не может быть null или пустым.", titlePost);
					return new List<NewsChannelsPosts>();
				}

				var listPostByPartTitlePost = await _appDbContext.NewsChannelsPosts
					.Where(el => el.TitlePost.ToLower().Contains(titlePost.ToLower()))
					.ToListAsync();

				if (!listPostByPartTitlePost.Any())
				{
					_logger.LogWarning("Посты с частью заголовка {TitlePost} не найдены", titlePost);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} постов с частью заголовка {TitlePost}", listPostByPartTitlePost.Count, titlePost);
				}

				return listPostByPartTitlePost;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при поиске постов по части заголовка {TitlePost}", titlePost);
				return new List<NewsChannelsPosts>();
			}
		}
    }
}
