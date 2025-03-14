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
using System.Xml.Linq;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class NewsChannelsPostsRepository : INewsChannelsPostsRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogService _logService;

        public NewsChannelsPostsRepository(AppDbContext appDbContext, ILogService logService)
        {
            _appDbContext = appDbContext;
            _logService = logService;
        }

        //создаем новый пост из новостного канала 
        public async Task<NewsChannelsPosts?> CreateNewNewsChannelsPost(int newChannelId, string titlePost, string bodyPost, string? footerPost, string authorPost, string? sourceImage)
        {
            try
            {
                //Проверяем на наличие записи в БД по id канала
				var newsChannel = await _appDbContext.NewsChannels.FindAsync(newChannelId);

				if (newsChannel is null)
				{
					_logService.LogWarning($"Некорректное значение id новостного канала - {nameof(newsChannel)}");
					throw new NullReferenceException($"В параметр newChannelId передан null");
				}

				//Проверяем на null authorPost
				if (string.IsNullOrEmpty(authorPost))
				{
					_logService.LogWarning($"Некорректное значение для параметра authorPost = {authorPost} - {nameof(authorPost)}");
					throw new NullReferenceException($"Параметр authorPost не может быть null");
				}

				//Проверяем на null bodyPost
				if (string.IsNullOrEmpty(bodyPost))
                {
					_logService.LogWarning($"Некорректное значение для параметра bodyPost = {bodyPost} - {nameof(bodyPost)}");
					throw new NullReferenceException($"Параметр bodyPost не может быть null");
				}

				//Проверяем на null titlePost
				if (string.IsNullOrEmpty(titlePost))
                {
                    _logService.LogWarning($"Некорректное значение для параметра titlePost = {titlePost} - {nameof(titlePost)}");
					throw new NullReferenceException($"Параметр titlePost не может быть null");
				}

				//Создаем экземпляр новой записи для сохраненеия ее в контексте 
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

				_logService.LogInformation($"Запись новостного сообщения была создана. newPost = {nameof(newPost)}. Где newPost.NewsChannelId = {newPost.NewsChannelId}\nnewPost.TitlePost = {newPost.TitlePost}\nnewPost.BodyPost = {newPost.BodyPost}\nnewPost.FooterPost = {newPost.FooterPost}\nnewPost.AuthorPost = {newPost.AuthorPost}\nnewPost.SurceImage = {newPost.SurceImage}\nnewPost.CreatedDate = {newPost.CreatedDate}");

				return newPost;
			}
            catch( Exception ex )
            {
				_logService.LogError($"При выполнении метода {nameof(CreateNewNewsChannelsPost)} возникла ошибка: {ex.Message}");
				throw;
			}
        }

        public async Task<List<NewsChannelsPosts>?> GetAllPosts()
        {
			try
			{
				_logService.LogInformation($"Поиск всех постов, написанных новостными каналами, вызван метод {nameof(GetAllPosts)}");

				var posts = await _appDbContext.NewsChannelsPosts.ToListAsync();
				_logService.LogInformation($"Найдено {posts.Count} постов.");
				return posts;
			}
			catch (Exception ex)
			{
				_logService.LogError($"При выполнении метода {nameof(GetAllPosts)} возникла ошибка: {ex.Message}");
				throw;
			}
		}

        public async Task<List<NewsChannelsPosts>?> GetAllPostsByNewsChannelId(int newsChannelId)
        {
            try
            {
				_logService.LogInformation($"Поиск по id новостного канала");
                var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

				//Проверка на null
                if(newsChannel is null)
                {
                    _logService.LogWarning($"Передано некорректное значение id новостного канала - {newsChannelId} - {nameof(newsChannelId)}. Не найден новостной канал по значению id - {newsChannelId}");
                    return null;
                }

                _logService.LogInformation($"Поиск постов для новостного канала - {newsChannel} - {nameof(newsChannel)}");
				
				var listPosts = await _appDbContext.NewsChannelsPosts
					.Where(el => el.NewsChannelId == newsChannelId)
					.ToListAsync();

				//Проверяем, есть ли посты
				if (!listPosts.Any())
				{
					_logService.LogWarning($"Постов от новостного канала с id = {newsChannelId} не найдено.");
					return null;
				}

				return listPosts;
			}
            catch(Exception ex)
            {
                _logService.LogError($"Возникла ошибка при поиске постов для новостного канала id = {newsChannelId} - {ex.Message}");
                throw;
            }
        }

        public async Task<List<NewsChannelsPosts>?> GetAllPostsByPartTitle(string titlePost)
        {
			try
            {
				if (string.IsNullOrEmpty(titlePost))
				{
					_logService.LogWarning($"Некорректное значение titlePost поста новостного канала: {titlePost}. Ожидается не null.");
					return null;
				}

				_logService.LogInformation($"Поиск постов по заголовку = {titlePost}, вызван метод {nameof(GetAllPostsByPartTitle)}");
				var listPostByPartTitlePost = await _appDbContext.NewsChannelsPosts
					.Where(el => el.TitlePost.ToLower().Contains(titlePost.ToLower()))
					.ToListAsync();

				if (!listPostByPartTitlePost.Any())
                {
                    _logService.LogWarning($"Не найдено совпадений с заданной частью заголовка - {titlePost}");
                }

				//Если нашли
				_logService.LogInformation($"Найдено {listPostByPartTitlePost.Count} постов с частью заголовка '{titlePost}'.");

				return listPostByPartTitlePost;
			}
            catch (Exception ex)
            {
                _logService.LogError($"При выполнении метода {nameof(GetAllPostsByPartTitle)} возникла ошибка: {ex.Message}");

				throw;
            }
        }
    }
}
