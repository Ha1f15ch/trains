using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class NewsChannelsSubscribersRepository : INewsChannelsSubscribersRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<NewsChannelsSubscribersRepository> _logger;

        public NewsChannelsSubscribersRepository(AppDbContext appDbContext, ILogger<NewsChannelsSubscribersRepository> logger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
		}

		public async Task<NewsChannelsSubscribers> SubscribeUserToNewsChannel(int userId, int newsChannelId)
        {
			try
			{
				_logger.LogInformation("Попытка подписки пользователя с ID {UserId} на новостной канал с ID {NewsChannelId}", userId, newsChannelId);

				var user = await _appDbContext.Users.FindAsync(userId);
				var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

				if (user is null)
				{
					_logger.LogError("Пользователь с ID {UserId} не найден", userId);
					throw new NullReferenceException($"Пользователь по userId не был найден");
				}

				if (newsChannel is null)
				{
					_logger.LogError("Новостной канал с ID {NewsChannelId} не найден", newsChannelId);
					throw new NullReferenceException($"Новостной канал по newsChannelId не был найден");
				}

				var subscriberValue = new NewsChannelsSubscribers
				{
					NewsChannelId = newsChannelId,
					UserId = userId,
					CreatedDate = DateTime.UtcNow
				};

				_logger.LogInformation("Создание записи подписки для пользователя ID {UserId} на новостной канал ID {NewsChannelId}", userId, newsChannelId);

				await _appDbContext.NewsChannelsSubscribers.AddAsync(subscriberValue);
				await _appDbContext.SaveChangesAsync();

				_logger.LogInformation("Подписка успешно создана для пользователя ID {UserId} на новостной канал ID {NewsChannelId}", userId, newsChannelId);

				return subscriberValue;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при подписке пользователя с ID {UserId} на новостной канал с ID {NewsChannelId}", userId, newsChannelId);
				throw;
			}
		}

		public async Task<List<User>> GetSubscribersByChannelId(int newsChannelId)
		{
			try
			{
				_logger.LogInformation("Попытка получения подписчиков для новостного канала с ID {NewsChannelId}", newsChannelId);

				// Поиск пользователей через связь с новостным каналом
				var users = await _appDbContext.NewsChannelsSubscribers
					.Where(ncs => ncs.NewsChannelId == newsChannelId)
					.Include(ncs => ncs.User) // Загрузка связи User для получения email
					.Select(ncs => ncs.User)
					.ToListAsync();

				if (!users.Any())
				{
					_logger.LogWarning("Подписчики для новостного канала с ID {NewsChannelId} не найдены", newsChannelId);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} подписчиков для новостного канала с ID {NewsChannelId}", users.Count, newsChannelId);
				}

				return users;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении подписчиков для новостного канала с ID {NewsChannelId}", newsChannelId);
				return new List<User>();
			}
		}

	}
}
