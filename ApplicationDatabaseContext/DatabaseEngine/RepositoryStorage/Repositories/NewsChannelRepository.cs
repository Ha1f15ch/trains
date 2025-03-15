﻿using AutoMapper;
using BusinesEngine.Services.ServiceInterfaces;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class NewsChannelRepository : INewsChannelRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public NewsChannelRepository(AppDbContext appDbContext, ILogService logService, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _logService = logService;
            _mapper = mapper;
        }

        public async Task<NewsChannel?> CreateNewNewsChannel(NewsChannelDto newsChannelDto)
        {
            try
            {
				_logService.LogInformation($"{nameof(CreateNewNewsChannel)} - Создание записи NewsChannel, используя метод {nameof(CreateNewNewsChannel)}. Параметры: name = {newsChannelDto.Name}, Description = {newsChannelDto.Description}");

                if(string.IsNullOrEmpty(newsChannelDto.Name))
                {
					_logService.LogWarning($"Название новостного канала не может быть пустым. {nameof(newsChannelDto.Name)}");
                    throw new ArgumentNullException("Значение Name не может быть null");
                }

                //Есть ли уже данная запись в БД
                var existedNewsChannel = await GetNewsChannelByName(newsChannelDto.Name);

                //Выводим ее 
                if(existedNewsChannel is not null)
                {
					_logService.LogInformation($"Новостной канал с названием '{newsChannelDto.Name}' уже существует. Возвращен существующий новостной канал. Id = {existedNewsChannel.Id}");
                    return existedNewsChannel;
                }

                //Тогда создаем новый канал 
                var newNewsChannel = _mapper.Map<NewsChannel>(newsChannelDto);

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
                
                var channels = await _appDbContext.NewsChannels.ToListAsync();

                _logService.LogInformation($"найдено каналов - {channels.Count}");

				return channels;
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
                //проверяем входной параметр
                if (newsChannelId <= 0)
                {
                    _logService.LogWarning($"Некорректное значение id: {newsChannelId}. Ожидается положительное число.");
                    throw new ArgumentNullException("Параметр newsChannelId должен быть > 0");
                }

                _logService.LogInformation($"Поиск новостного канала по id = {newsChannelId}, вызван метод {nameof(newsChannelId)}");

                var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

                if (newsChannel is null)
                {
                    _logService.LogWarning($"Новостной канал с id = {newsChannelId} не найден.");
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
                    throw new ArgumentNullException("Значение newsChannelName не может быть null");
                }

                _logService.LogInformation($"Поиск новостного канала по названию = {newsChannelName}, вызван метод {nameof(GetNewsChannelByName)}");

                var newsChannel = await _appDbContext.NewsChannels.SingleOrDefaultAsync(el => el.Name == newsChannelName);

                //Если не нашли
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
					throw new ArgumentNullException("Значение newsChannelName не может быть null");
				}

                _logService.LogInformation($"Поиск новостных каналов по названию = {newsChannelName}, вызван метод {nameof(GetNewsChannelsByPartName)}");

                var newsChannel = await _appDbContext.NewsChannels.Where(el => el.Name.Contains(newsChannelName)).ToListAsync();

                //Если ничего не найдено
                if (newsChannel is null)
                {
                    _logService.LogWarning($"Новостных каналов с названием = {newsChannelName} не найдено.");
                    return null;
                }

                _logService.LogInformation($"По параметру - {newsChannelName} найдено - {newsChannel.Count} записей");

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
