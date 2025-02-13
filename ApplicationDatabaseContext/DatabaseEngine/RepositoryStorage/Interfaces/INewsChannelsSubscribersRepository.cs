using DatabaseEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEngine.RepositoryStorage.Interfaces
{
    public interface INewsChannelsSubscribersRepository
    {
        public Task<NewsChannelsSubscribers> SubscribeUserToNewsChannel(int userId, int newsChannelId);
        public Task<List<User>?> GetSubscribersByChannelId(int newsChannelId);

	}
}
