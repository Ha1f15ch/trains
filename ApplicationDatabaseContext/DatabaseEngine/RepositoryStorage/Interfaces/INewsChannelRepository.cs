using DatabaseEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEngine.RepositoryStorage.Interfaces
{
    public interface INewsChannelRepository
    {
        public Task<NewsChannel> CreateNewNewsChannel(string name, string? description);
        public Task<NewsChannel> GetNewsChannelById(int newsChannelId);
        public Task<NewsChannel> GetNewsChannelByName(string newsChannelName);
        public Task<List<NewsChannel>> GetAllNewsChannels();
        public Task<List<NewsChannel>> GetNewsChannelsByPartName(string newsChannelName);
    }
}
