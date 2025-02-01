using MediatR;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DTOs;
using BusinesEngine.Events;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<SubscriptionRepository> _logger;
        private readonly IMediator _mediator;

        public SubscriptionRepository(AppDbContext appDbContext, ILogger<SubscriptionRepository> logger, IMediator mediator)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<List<Subscription>> GetUserSubscriptions(int userId)
        {
            try
            {
                var userExists = await _appDbContext.Users.AnyAsync(u => u.Id == userId);

                if(userExists)
                {
                    _logger.LogInformation($"{LogLevel.Information} - {GetUserSubscriptions} - {nameof(GetUserSubscriptions)} - Выполняется поиск подписок пользователя - {userId}.");
                    return await _appDbContext.Subscriptions.Where(sub => sub.UserId == userId).ToListAsync();
                }

                throw new ArgumentException($"Пользователь с ID {userId} не найден.");
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
                            SubscriptionDate = DateTime.UtcNow,
                        };

                        await _appDbContext.AddAsync(newSubscription);
                        await _appDbContext.SaveChangesAsync();

                        var subscriptionDto = new SubscriptionDto
                        {
                            UserId = newSubscription.UserId,
                            BookId = newSubscription.BookId,
                            SubscriptionDate = newSubscription.SubscriptionDate
                        };

                        await _mediator.Publish(new SubscriptionCreatedEvent(subscriptionDto));

                        _logger.LogInformation($"{LogLevel.Information} - {SubscribeUserToBook} - {nameof(SubscribeUserToBook)} - Выполняется оповещение пользователя {userId} о том, что подписка на книгу {bookId} выполнена успешно. Была создана запись подписки - {subscriptionDto} - {nameof(subscriptionDto)}");

                        return newSubscription;
                    }

                    return existingSubscription;
                }

                throw new NullReferenceException($"При выполнении поиска пользователя и книги возникла ошибка. Получен null. User = {subscriber} - {nameof(subscriber)}. Book = {book} - {nameof(book)}");
            }
            catch(Exception ex)
            {
                _logger.LogError($"{LogLevel.Error} - {SubscribeUserToBook} - {nameof(SubscribeUserToBook)} - Возникла ошибка при выполнении метода создания подписки пользователя {userId} на книгу - {bookId}. Ошибка - {ex.Message}");
                throw;
            }
        }
    }
}
