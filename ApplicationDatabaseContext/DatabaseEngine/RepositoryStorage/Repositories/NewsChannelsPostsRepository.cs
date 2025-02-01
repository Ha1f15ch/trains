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

        public Task<NewsChannelsPosts> CreateNewNewsChannelsPost(int newChannelId, string titlePost, string bodyPost, string footerPost, string authorPost, string? surceImage)
        {
            throw new NotImplementedException();
        }

        public Task<List<NewsChannelsPosts>> GetAllPosts()
        {
            throw new NotImplementedException();
        }

        public Task<List<NewsChannelsPosts>> GetAllPostsByNewsChannelId(int newsChannelId)
        {
            throw new NotImplementedException();
        }

        public Task<List<NewsChannelsPosts>> GetAllPostsByPartTitle(string titlePost)
        {
            throw new NotImplementedException();
        }
    }
}
