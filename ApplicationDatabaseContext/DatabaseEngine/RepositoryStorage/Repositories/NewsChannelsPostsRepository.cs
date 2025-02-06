using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<NewsChannelsPosts?> CreateNewNewsChannelsPost(int newChannelId, string titlePost, string bodyPost, string? footerPost, string authorPost, string? surceImage)
        {
            var newsChannel = await _appDbContext.NewsChannels.FindAsync(newChannelId);

            if(newsChannel is null)
            {
                _logger.LogWarning($"Некорректное значение id новостного канала - {nameof(newsChannel)}");
                return null;
            }

            if(string.IsNullOrEmpty(titlePost) || string.IsNullOrEmpty(bodyPost) || string.IsNullOrEmpty(authorPost))
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
                SurceImage = surceImage,
                CreatedDate = DateTime.UtcNow
            };

            await _appDbContext.NewsChannelsPosts.AddAsync(newPost);
            await _appDbContext.SaveChangesAsync();

            _logger.LogInformation($"Запись новостного сообщения была создана. newPost = {nameof(newPost)}. Где newPost.NewsChannelId = {newPost.NewsChannelId}\nnewPost.TitlePost = {newPost.TitlePost}\nnewPost.BodyPost = {newPost.BodyPost}\nnewPost.FooterPost = {newPost.FooterPost}\nnewPost.AuthorPost = {newPost.AuthorPost}\nnewPost.SurceImage = {newPost.SurceImage}\nnewPost.CreatedDate = {newPost.CreatedDate}");

		    return newPost;
        }

        public async Task<List<NewsChannelsPosts>?> GetAllPosts()
        {
            throw new NotImplementedException();
        }

        public async Task<List<NewsChannelsPosts>?> GetAllPostsByNewsChannelId(int newsChannelId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<NewsChannelsPosts>?> GetAllPostsByPartTitle(string titlePost)
        {
            throw new NotImplementedException();
        }
    }
}
