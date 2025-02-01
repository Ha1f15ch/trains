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
    public class NewsChannelRepository : INewsChannelRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<NewsChannelRepository> _logger;

        public NewsChannelRepository(AppDbContext appDbContext, ILogger<NewsChannelRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public Task<NewsChannel> CreateNewNewsChannel(string name, string? description)
        {
            throw new NotImplementedException();
        }

        public Task<List<NewsChannel>> GetAllNewsChannels()
        {
            throw new NotImplementedException();
        }

        public Task<NewsChannel> GetNewsChannelById(int newsChannelId)
        {
            throw new NotImplementedException();
        }

        public Task<NewsChannel> GetNewsChannelByName(string newsChannelName)
        {
            throw new NotImplementedException();
        }

        public Task<List<NewsChannel>> GetNewsChannelsByPartName(string newsChannelName)
        {
            throw new NotImplementedException();
        }
    }
}
