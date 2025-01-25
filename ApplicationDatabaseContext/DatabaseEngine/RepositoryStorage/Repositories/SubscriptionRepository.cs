using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<ISubscriptionRepository> _logger;

        public SubscriptionRepository(AppDbContext appDbContext, ILogger<ISubscriptionRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<List<Subscription>> GetUserSubscriptions(int userId)
        {
            try
            {
                if(userId > 0)
                {
                    _logger.LogInformation($"{LogLevel.Information} - {GetUserSubscriptions} - {nameof(GetUserSubscriptions)} - Выполняется поиск подписок пользователя - {userId}.");
                    return await _appDbContext.Subscriptions.Where(sub => sub.UserId == userId).ToListAsync();
                }

                throw new ArgumentException($"Некорректн опередан параметр поиска userId = {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{LogLevel.Error} - Возникла ошибка при выполнении метода поиска подписок - {GetUserSubscriptions} - {nameof(GetUserSubscriptions)} для пользователя {userId}. Ошибка - {ex.Message}");
                throw;
            }
        }

        public async Task<Subscription> SubscribeUserToBook(int userId, int bookId)
        {
            try
            {
                var subscriber = await _appDbContext.Users.FindAsync(userId);
                var book = await _appDbContext.Books.FindAsync(bookId);

                if(subscriber is not null && book is not null)
                {
                    var existingSubscription = subscriber.Subscriptions.SingleOrDefault(subs => subs.BookId == book.Id);

                    if(existingSubscription is null)
                    {
                        var newSubscription = new Subscription
                        {
                            UserId = userId,
                            BookId = bookId,
                            SubscriptionDate = DateTime.Now,
                        };

                        await _appDbContext.AddAsync(newSubscription);
                        await _appDbContext.SaveChangesAsync();

                        return newSubscription;
                    }

                    return existingSubscription;
                }

                throw new NullReferenceException($"При выполнении поиска пользователя и книги возникла ошибка. Получен null. User = {subscriber} - {nameof(subscriber)}. Book = {book} - {nameof(book)}");
            }
            catch(Exception ex)
            {
                _logger.LogError($"{LogLevel.Error} - {GetUserSubscriptions} - {nameof(GetUserSubscriptions)} - Возникла ошибка при выполнении метода создания подписки пользователя {userId} на книгу - {bookId}. Ошибка - {ex.Message}");
                throw;
            }
        }
    }
}
