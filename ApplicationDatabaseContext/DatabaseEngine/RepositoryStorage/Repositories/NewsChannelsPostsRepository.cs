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
				var newsChannel = await _appDbContext.NewsChannels.FindAsync(newChannelId);

				if (newsChannel is null)
				{
					_logger.LogWarning($"Некорректное значение id новостного канала - {nameof(newsChannel)}");
					return null;
				}

				if (string.IsNullOrEmpty(titlePost) || string.IsNullOrEmpty(bodyPost) || string.IsNullOrEmpty(authorPost))
				{
					_logger.LogWarning($"Некорректное значение для параметра titlePost = {titlePost} - {nameof(titlePost)}\nили bodyPost = {bodyPost} - {nameof(bodyPost)}\nили authorPost = {authorPost} - {nameof(authorPost)}\nили");
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

				_logger.LogInformation($"Запись новостного сообщения была создана. newPost = {nameof(newPost)}. Где newPost.NewsChannelId = {newPost.NewsChannelId}\nnewPost.TitlePost = {newPost.TitlePost}\nnewPost.BodyPost = {newPost.BodyPost}\nnewPost.FooterPost = {newPost.FooterPost}\nnewPost.AuthorPost = {newPost.AuthorPost}\nnewPost.SurceImage = {newPost.SurceImage}\nnewPost.CreatedDate = {newPost.CreatedDate}");

				return newPost;
			}
            catch( Exception ex )
            {
				_logger.LogError(ex, $"При выполнении метода {nameof(CreateNewNewsChannelsPost)} возникла ошибка: {ex.Message}");
				throw;
			}
        }

        public async Task<List<NewsChannelsPosts>?> GetAllPosts()
        {
			try
			{
				_logger.LogInformation($"Поиск всех постов, написанных новостными каналами, вызван метод {nameof(GetAllPosts)}");

				return await _appDbContext.NewsChannelsPosts.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"При выполнении метода {nameof(GetAllPosts)} возникла ошибка: {ex.Message}");
				throw;
			}
		}

        public async Task<List<NewsChannelsPosts>?> GetAllPostsByNewsChannelId(int newsChannelId)
        {
            try
            {
                var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

                if(newsChannel is null)
                {
                    _logger.LogWarning($"Передано некорректное значение id новостного канала - {newsChannelId} - {nameof(newsChannelId)}. Не найден новостной канал по значению id - {newsChannelId}");
                    return null;
                }

                _logger.LogInformation($"Поиск постов для новостного канала - {newsChannel} - {nameof(newsChannel)}");
                var listPosts = await _appDbContext.NewsChannelsPosts.Where(el => el.NewsChannelId == newsChannel.Id).ToListAsync();

                if(!listPosts.Any())
                {
					_logger.LogWarning($"Постов от новостного канала с id = {newsChannelId} не найдено.");
					return null;
                }

                return listPosts;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Возникла ошибка при поиске постов для новостного канала id = {newsChannelId} - {ex.Message}");
                throw;
            }
        }

        public async Task<List<NewsChannelsPosts>?> GetAllPostsByPartTitle(string titlePost)
        {
			try
            {
				if (string.IsNullOrEmpty(titlePost))
				{
					_logger.LogWarning($"Некорректное значение titlePost поста новостного канала: {titlePost}. Ожидается не null.");
					return null;
				}

				_logger.LogInformation($"Поиск постов по заголовку = {titlePost}, вызван метод {nameof(GetAllPostsByPartTitle)}");
                var listPostByPartTitlePost = await _appDbContext.NewsChannelsPosts.Where(el => el.TitlePost.Contains(titlePost)).ToListAsync();

                if(!listPostByPartTitlePost.Any())
                {
                    _logger.LogWarning($"Не найдено совпадений с заданной частью заголовка - {titlePost}");
                }

                return listPostByPartTitlePost;
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, $"При выполнении метода {nameof(GetAllPostsByPartTitle)} возникла ошибка: {ex.Message}");

				throw;
            }
        }
    }
}
