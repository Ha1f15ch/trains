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

		public NewsChannelRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<NewsChannel?> CreateNewNewsChannel(NewsChannelDto newsChannelDto)
        {
            try
            {
                if(string.IsNullOrEmpty(newsChannelDto.Name))
                {
                    throw new ArgumentNullException("Значение Name не может быть null");
                }

                //Есть ли уже данная запись в БД
                var existedNewsChannel = await GetNewsChannelByName(newsChannelDto.Name);

                //Выводим ее 
                if(existedNewsChannel is not null)
                {
                    return existedNewsChannel;
                }

                //Тогда создаем новый канал 
                var newNewsChannel = new NewsChannel
                {
                    Name = newsChannelDto.Name,
                    Description = newsChannelDto.Description,
                    DateCreated = newsChannelDto.DateCreated,
                };

                await _appDbContext.AddAsync(newNewsChannel);
                await _appDbContext.SaveChangesAsync();

                return newNewsChannel;
            }
            catch (Exception ex)
            {
				Console.WriteLine($"Создать не получилось - {ex.Message}");
                return null;
            }
        }

        public async Task<List<NewsChannel>> GetAllNewsChannels()
        {
            try
            {
                var channels = await _appDbContext.NewsChannels.ToListAsync();

				return channels;
			}
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<NewsChannel?> GetNewsChannelById(int newsChannelId)
        {
            try
            {
                //проверяем входной параметр
                if (newsChannelId <= 0)
                {
                    throw new ArgumentNullException("Параметр newsChannelId должен быть > 0");
                }

                var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

                if (newsChannel is null)
                {
					Console.WriteLine($"Новостной канал с id = {newsChannelId} не найден.");
                    return null;
                }

                return newsChannel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<NewsChannel?> GetNewsChannelByName(string newsChannelName)
        {
            try
            {
                if (string.IsNullOrEmpty(newsChannelName))
                {
                    Console.WriteLine($"Некорректное значение newsChannelName для поиска новостного канала: {newsChannelName}. Ожидается не null.");
                    throw new ArgumentNullException("Значение newsChannelName не может быть null");
                }

                var newsChannel = await _appDbContext.NewsChannels.SingleOrDefaultAsync(el => el.Name == newsChannelName);

                //Если не нашли
                if (newsChannel is null)
                {
                    Console.WriteLine($"Новостного канала с названием = {newsChannelName} не найдено.");
                    return null;
                }

                return newsChannel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Для поиска совпадающих по названию частично 
        public async Task<List<NewsChannel>> GetNewsChannelsByPartName(string newsChannelName)
        {
            try
            {
                if (string.IsNullOrEmpty(newsChannelName))
                {
					throw new ArgumentNullException("Значение newsChannelName не может быть null");
				}

                var newsChannel = await _appDbContext.NewsChannels.Where(el => el.Name.Contains(newsChannelName)).ToListAsync();

                //Если ничего не найдено
                if (newsChannel is null)
                {
                    return null;
                }

                return newsChannel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
