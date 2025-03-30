using DatabaseEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEngine.RepositoryStorage.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Subscription> SubscribeUserToBook(int userId, int bookId);
        Task<List<Subscription>> GetUserSubscriptions(int userId);
    }
}
