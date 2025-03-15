using BusinesEngine.Services.ServiceInterfaces;
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
        private readonly ILogService _logService;

        public NewsChannelsSubscribersRepository(AppDbContext appDbContext, ILogService logService)
        {
            _appDbContext = appDbContext;
			_logService = logService;
        }

        public async Task<NewsChannelsSubscribers> SubscribeUserToNewsChannel(int userId, int newsChannelId)
        {
            try
            {
                _logService.LogInformation($"Выполняем поиск по id пользователя {userId} и новостной канал {newsChannelId}");

                var user = await _appDbContext.Users.FindAsync(userId);
                var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

                if(user is null)
                {
                    _logService.LogError($"{nameof(SubscribeUserToNewsChannel)} \nПо указанному userId = {userId} пользователь найден не был");
                    throw new NullReferenceException($"Пользователь по userId не был найден - {user} - {nameof(user)}");
                }

				if (newsChannel is null)
				{
					_logService.LogError($"{nameof(SubscribeUserToNewsChannel)} \nПо указанному newsChannelId = {newsChannel} новостной канал найден не был");
					throw new NullReferenceException($"Новостной канал по newsChannelId не был найден - {newsChannel} - {nameof(newsChannel)}");
				}

                //Создаем экземпляр класса для сохранения в контексте 
                var subscriberValue = new NewsChannelsSubscribers
                {
                    NewsChannelId = newsChannelId,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };

                _logService.LogInformation($"{nameof(SubscribeUserToNewsChannel)} \n Создаем запись подписки - {subscriberValue}");

                await _appDbContext.NewsChannelsSubscribers.AddAsync( subscriberValue );

                await _appDbContext.SaveChangesAsync();

                _logService.LogInformation($"Успешно добавлено");

                return subscriberValue;
			}
            catch (Exception ex)
            {
                _logService.LogError($"{SubscribeUserToNewsChannel} - {nameof(SubscribeUserToNewsChannel)} - {ex.Message} - Ошибка при выполнении подписки пользователя на новостной канал");
                throw;
            }
        }

		public async Task<List<User>?> GetSubscribersByChannelId(int newsChannelId)
		{
			try
			{
                _logService.LogInformation($"Поиск подписавшихся пользователей по {nameof(newsChannelId)} = {newsChannelId}");

				//Поиск по свойствам связанных таблиц. Поиск пользователей через newsChannelId. Подписчики
				var users = await _appDbContext.NewsChannelsSubscribers
					.Where(ncs => ncs.NewsChannelId == newsChannelId)
					.Include(ncs => ncs.User) // Загрузка связи User для получения email
					.Select(ncs => ncs.User).ToListAsync();

                _logService.LogInformation($"Найдено {users.Count} записей");

                return users;
			}
			catch (Exception ex)
			{
				_logService.LogError($"Ошибка при получении подписчиков канала с Id = {newsChannelId}. {ex.Message}");
				throw;
			}
		}

	}
}
