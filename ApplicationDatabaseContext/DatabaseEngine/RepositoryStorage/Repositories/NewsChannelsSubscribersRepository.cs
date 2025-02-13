using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                _logger.LogInformation($"Выполняем поиск по id пользователя {userId} и новостной канал {newsChannelId}");

                var user = await _appDbContext.Users.FindAsync(userId);
                var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

                if(user is null)
                {
                    _logger.LogError($"{nameof(SubscribeUserToNewsChannel)} \nПо указанному userId = {userId} пользователь найден не был");
                    throw new NullReferenceException($"Пользователь по userId не был найден - {user} - {nameof(user)}");
                }

				if (newsChannel is null)
				{
					_logger.LogError($"{nameof(SubscribeUserToNewsChannel)} \nПо указанному newsChannelId = {newsChannel} новостной канал найден не был");
					throw new NullReferenceException($"Новостной канал по newsChannelId не был найден - {newsChannel} - {nameof(newsChannel)}");
				}

                var subscriberValue = new NewsChannelsSubscribers
                {
                    NewsChannelId = newsChannelId,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };

                _logger.LogInformation($"{nameof(SubscribeUserToNewsChannel)} \n Создаем запись подписки - {subscriberValue}");

                await _appDbContext.NewsChannelsSubscribers.AddAsync( subscriberValue );

                _logger.LogInformation($"Успешно добавлено");

                await _appDbContext.SaveChangesAsync();

                return subscriberValue;
			}
            catch (Exception ex)
            {
                _logger.LogError($"{SubscribeUserToNewsChannel} - {nameof(SubscribeUserToNewsChannel)} - {ex.Message} - Ошибка при выполнении подписки пользователя на новостной канал");
                throw;
            }
        }

		public async Task<List<User>?> GetSubscribersByChannelId(int newsChannelId)
		{
			try
			{
				return await _appDbContext.NewsChannelsSubscribers
					.Where(ncs => ncs.NewsChannelId == newsChannelId)
					.Include(ncs => ncs.User) // Загрузка связи User для получения email
					.Select(ncs => ncs.User).ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Ошибка при получении подписчиков канала с Id = {newsChannelId}");
				throw;
			}
		}

	}
}
