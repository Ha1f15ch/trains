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
    public class NewsChannelsSubscribersRepository : INewsChannelsSubscribersRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<NewsChannelsSubscribersRepository> _logger;

        public NewsChannelsSubscribersRepository(AppDbContext appDbContext, ILogger<NewsChannelsSubscribersRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public Task<NewsChannelsSubscribers> SubscribeUserToNewsChannel(int userId, int newsChannelId)
        {
            throw new NotImplementedException();
        }
    }
}
