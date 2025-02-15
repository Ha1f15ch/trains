using BusinesEngine.Services.ServiceInterfaces;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class NewsChannelRepository : INewsChannelRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogService _logService;

        public NewsChannelRepository(AppDbContext appDbContext, ILogService logService)
        {
            _appDbContext = appDbContext;
            _logService = logService;
        }

        public async Task<NewsChannel?> CreateNewNewsChannel(string name, string? description)
        {
            try
            {
				_logService.LogInformation($"{nameof(CreateNewNewsChannel)} - Создание записи NewsChannel, используя метод {name}. Параметры: name = {name}, Description = {description}");

                if(string.IsNullOrEmpty(name))
                {
					_logService.LogWarning($"Название новостного канала не может быть пустым. {nameof(name)}");
                    return null;
                }

                //Есть ли уже данная запись в БД
                var existedNewsChannel = await GetNewsChannelByName(name);

                if(existedNewsChannel is not null)
                {
					_logService.LogInformation($"Новостной канал с названием '{name}' уже существует. Возвращен существующий новостной канал. Id = {existedNewsChannel.Id}");
                    return existedNewsChannel;
                }

                //Тогда создаем новый канал 
                var newNewsChannel = new NewsChannel
                {
                    Name = name,
                    Description = description,
                    CountSubscribers = 0,
                    DateCreated = DateTime.UtcNow
                };

                await _appDbContext.AddAsync(newNewsChannel);
                await _appDbContext.SaveChangesAsync();

				_logService.LogInformation($"Запись новостного канала была создана. newsChannel = {nameof(newNewsChannel)}. Где newNewsChannel.name = {newNewsChannel.Name}\nnewNewsChannel.Description = {newNewsChannel.Description}\nnewNewsChannel.CountSubscribers = {newNewsChannel.CountSubscribers}\nnewNewsChannel.DateCreated = {newNewsChannel.DateCreated}");

                return newNewsChannel;
            }
            catch (Exception ex)
            {
                _logService.LogError($"При выполнении метода {nameof(CreateNewNewsChannel)} возникла ошибка: {ex.Message}");
                throw;
            }
        }

        public async Task<List<NewsChannel>?> GetAllNewsChannels()
        {
            try
            {
                _logService.LogInformation($"Поиск всех новостных каналов, вызван метод {nameof(GetAllNewsChannels)}");

                return await _appDbContext.NewsChannels.ToListAsync();
            }
            catch (Exception ex)
            {
                _logService.LogError($"При выполнении метода {nameof(GetAllNewsChannels)} возникла ошибка: {ex.Message}");
                throw;
            }
        }

        public async Task<NewsChannel?> GetNewsChannelById(int newsChannelId)
        {
            try
            {
                if (newsChannelId <= 0)
                {
                    _logService.LogWarning($"Некорректное значение id: {newsChannelId}. Ожидается положительное число.");
                    return null;
                }

                _logService.LogInformation($"Новостного канала по id = {newsChannelId}, вызван метод {nameof(newsChannelId)}");

                var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

                if (newsChannel is null)
                {
                    _logService.LogWarning($"Новостной канал с id = {newsChannelId} не найдена.");
                }

                return newsChannel;
            }
            catch (Exception ex)
            {
                _logService.LogError($"При выполнении метода {nameof(GetNewsChannelById)} возникла ошибка: {ex.Message}");
                throw;
            }
        }

        public async Task<NewsChannel?> GetNewsChannelByName(string newsChannelName)
        {
            try
            {
                if (string.IsNullOrEmpty(newsChannelName))
                {
                    _logService.LogWarning($"Некорректное значение newsChannelName для поиска новостного канала: {newsChannelName}. Ожидается не null.");
                    return null;
                }

                _logService.LogInformation($"Поиск новостного канала по названию = {newsChannelName}, вызван метод {nameof(GetNewsChannelByName)}");

                var newsChannel = await _appDbContext.NewsChannels.SingleOrDefaultAsync(el => el.Name == newsChannelName);

                if (newsChannel is null)
                {
                    _logService.LogWarning($"Новостного канала с названием = {newsChannelName} не найдено.");
                }

                return newsChannel;
            }
            catch (Exception ex)
            {
                _logService.LogError($"При выполнении метода {nameof(GetNewsChannelByName)} возникла ошибка: {ex.Message}");
                throw;
            }
        }

        //Для поиска совпадающих по названию частично 
        public async Task<List<NewsChannel>?> GetNewsChannelsByPartName(string newsChannelName)
        {
            try
            {
                if (string.IsNullOrEmpty(newsChannelName))
                {
                    _logService.LogWarning($"Некорректное значение newsChannelName для поиска новостных каналов: {newsChannelName}. Ожидается не null.");
                    return null;
                }

                _logService.LogInformation($"Поиск новостных каналов по названию = {newsChannelName}, вызван метод {nameof(GetNewsChannelsByPartName)}");

                var newsChannel = await _appDbContext.NewsChannels.Where(el => el.Name.Contains(newsChannelName)).ToListAsync();

                if (newsChannel is null)
                {
                    _logService.LogWarning($"Новостных каналов с названием = {newsChannelName} не найдено.");
                }

                return newsChannel;
            }
            catch (Exception ex)
            {
                _logService.LogError($"При выполнении метода {nameof(GetNewsChannelsByPartName)} возникла ошибка: {ex.Message}");
                throw;
            }
        }
    }
}
