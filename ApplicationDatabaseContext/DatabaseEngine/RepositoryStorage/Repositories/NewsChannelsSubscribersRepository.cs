using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class NewsChannelsSubscribersRepository : INewsChannelsSubscribersRepository
    {
        private readonly AppDbContext _appDbContext;

        public NewsChannelsSubscribersRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<NewsChannelsSubscribers> SubscribeUserToNewsChannel(int userId, int newsChannelId)
        {
            try
            {
				Console.WriteLine($"Выполняем поиск по id пользователя {userId} и новостной канал {newsChannelId}");

                var user = await _appDbContext.Users.FindAsync(userId);
                var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

                if(user is null)
                {
					Console.WriteLine($"По указанному userId = {userId} пользователь найден не был");
                    throw new NullReferenceException($"Пользователь по userId не был найден - {user} - {nameof(user)}");
                }

				if (newsChannel is null)
				{
					Console.WriteLine($"По указанному newsChannelId = {newsChannel} новостной канал найден не был");
					throw new NullReferenceException($"Новостной канал по newsChannelId не был найден - {newsChannel} - {nameof(newsChannel)}");
				}

                //Создаем экземпляр класса для сохранения в контексте 
                var subscriberValue = new NewsChannelsSubscribers
                {
                    NewsChannelId = newsChannelId,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };

                Console.WriteLine($"{nameof(SubscribeUserToNewsChannel)} \n Создаем запись подписки - {subscriberValue}");

                await _appDbContext.NewsChannelsSubscribers.AddAsync( subscriberValue );

                await _appDbContext.SaveChangesAsync();

                Console.WriteLine($"Успешно добавлено");

                return subscriberValue;
			}
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении подписки пользователя на новостной канал - {ex.Message}");
                throw;
            }
        }

		public async Task<List<User>> GetSubscribersByChannelId(int newsChannelId)
		{
			try
			{
                Console.WriteLine($"Поиск подписавшихся пользователей по {nameof(newsChannelId)} = {newsChannelId}");

				//Поиск по свойствам связанных таблиц. Поиск пользователей через newsChannelId. Подписчики
				var users = await _appDbContext.NewsChannelsSubscribers
					.Where(ncs => ncs.NewsChannelId == newsChannelId)
					.Include(ncs => ncs.User) // Загрузка связи User для получения email
					.Select(ncs => ncs.User).ToListAsync();

                Console.WriteLine($"Найдено {users.Count} записей");

                return users;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при получении подписчиков канала с Id = {newsChannelId}. {ex.Message}");
                return new List<User>();
			}
		}

	}
}
