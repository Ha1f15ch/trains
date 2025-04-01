using AutoMapper;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using DTOModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class NewsChannelRepository : INewsChannelRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<NewsChannelRepository> _logger;

		public NewsChannelRepository(AppDbContext appDbContext, ILogger<NewsChannelRepository> logger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
		}

		public async Task<NewsChannel?> CreateNewNewsChannel(NewsChannelDto newsChannelDto)
        {
			try
			{
				_logger.LogInformation("Попытка создания нового новостного канала с названием {Name}", newsChannelDto.Name);

				if (string.IsNullOrEmpty(newsChannelDto.Name))
				{
					_logger.LogError("Неверное значение параметра Name: {Name}. Значение не может быть null или пустым.", newsChannelDto.Name);
					throw new ArgumentNullException("Значение Name не может быть null");
				}

				var existedNewsChannel = await GetNewsChannelByName(newsChannelDto.Name);

				if (existedNewsChannel is not null)
				{
					_logger.LogWarning("Новостной канал с названием {Name} уже существует. Возвращаем существующий канал.", newsChannelDto.Name);
					return existedNewsChannel;
				}

				var newNewsChannel = new NewsChannel
				{
					Name = newsChannelDto.Name,
					Description = newsChannelDto.Description,
					DateCreated = newsChannelDto.DateCreated,
				};

				await _appDbContext.AddAsync(newNewsChannel);
				await _appDbContext.SaveChangesAsync();

				_logger.LogInformation("Новостной канал с названием {Name} успешно создан", newsChannelDto.Name);
				return newNewsChannel;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при создании новостного канала с названием {Name}", newsChannelDto.Name);
				return null;
			}
		}

        public async Task<List<NewsChannel>> GetAllNewsChannels()
        {
			try
			{
				_logger.LogInformation("Попытка получения всех новостных каналов");

				var channels = await _appDbContext.NewsChannels.ToListAsync();

				if (!channels.Any())
				{
					_logger.LogWarning("Новостные каналы в базе данных отсутствуют");
				}
				else
				{
					_logger.LogInformation("Успешно получено {Count} новостных каналов", channels.Count);
				}

				return channels;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении всех новостных каналов");
				throw;
			}
		}

        public async Task<NewsChannel?> GetNewsChannelById(int newsChannelId)
        {
			try
			{
				_logger.LogInformation("Попытка получения новостного канала по ID {Id}", newsChannelId);

				if (newsChannelId <= 0)
				{
					_logger.LogError("Неверное значение параметра newsChannelId: {Id}. Значение должно быть больше 0.", newsChannelId);
					throw new ArgumentNullException("Параметр newsChannelId должен быть > 0");
				}

				var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

				if (newsChannel is null)
				{
					_logger.LogWarning("Новостной канал с ID {Id} не найден", newsChannelId);
					return null;
				}

				_logger.LogInformation("Новостной канал с ID {Id} успешно найден. Название: {Name}", newsChannelId, newsChannel.Name);
				return newsChannel;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении новостного канала по ID {Id}", newsChannelId);
				throw;
			}
		}

        public async Task<NewsChannel?> GetNewsChannelByName(string newsChannelName)
        {
			try
			{
				_logger.LogInformation("Попытка поиска новостного канала по названию {Name}", newsChannelName);

				if (string.IsNullOrEmpty(newsChannelName))
				{
					_logger.LogError("Неверное значение параметра newsChannelName: {Name}. Значение не может быть null или пустым.", newsChannelName);
					throw new ArgumentNullException("Значение newsChannelName не может быть null");
				}

				var newsChannel = await _appDbContext.NewsChannels.SingleOrDefaultAsync(el => el.Name == newsChannelName);

				if (newsChannel is null)
				{
					_logger.LogWarning("Новостной канал с названием {Name} не найден", newsChannelName);
					return null;
				}

				_logger.LogInformation("Новостной канал с названием {Name} успешно найден. ID: {Id}", newsChannelName, newsChannel.Id);
				return newsChannel;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при поиске новостного канала по названию {Name}", newsChannelName);
				throw;
			}
		}

        public async Task<List<NewsChannel>> GetNewsChannelsByPartName(string newsChannelName)
        {
			try
			{
				_logger.LogInformation("Попытка поиска новостных каналов по части названия {Name}", newsChannelName);

				if (string.IsNullOrEmpty(newsChannelName))
				{
					_logger.LogError("Неверное значение параметра newsChannelName: {Name}. Значение не может быть null или пустым.", newsChannelName);
					throw new ArgumentNullException("Значение newsChannelName не может быть null");
				}

				var newsChannels = await _appDbContext.NewsChannels.Where(el => el.Name.Contains(newsChannelName)).ToListAsync();

				if (!newsChannels.Any())
				{
					_logger.LogWarning("Новостные каналы, содержащие часть названия {Name}, не найдены", newsChannelName);
				}
				else
				{
					_logger.LogInformation("Найдено {Count} новостных каналов, содержащих часть названия {Name}", newsChannels.Count, newsChannelName);
				}

				return newsChannels;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при поиске новостных каналов по части названия {Name}", newsChannelName);
				throw;
			}
		}
    }
}
