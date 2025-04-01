using MediatR;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DTOModels;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<SubscriptionRepository> _logger;

        public SubscriptionRepository(AppDbContext appDbContext, ILogger<SubscriptionRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<List<Subscription>> GetUserSubscriptions(int userId)
        {
			try
			{
				_logger.LogInformation("Попытка получения подписок для пользователя ID {UserId}", userId);

				var userSubscriptions = await _appDbContext.Subscriptions
					.Where(sub => sub.UserId == userId)
					.ToListAsync();

				if (!userSubscriptions.Any())
				{
					_logger.LogWarning("Подписки для пользователя ID {UserId} не найдены", userId);
					return new List<Subscription>();
				}

				_logger.LogInformation("Успешно получено {Count} подписок для пользователя ID {UserId}", userSubscriptions.Count, userId);
				return userSubscriptions;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении подписок для пользователя ID {UserId}", userId);
				return new List<Subscription>();
			}
		}

        public async Task<Subscription> SubscribeUserToBook(int userId, int bookId)
        {
            try
            {
				_logger.LogInformation("Попытка подписки пользователя ID {UserId} на книгу ID {BookId}", userId, bookId);


				if (userId <= 0)
				{
					_logger.LogError("Неверное значение параметра userId: {UserId}. Значение должно быть больше 0.", userId);
					throw new ArgumentException("Параметр userId не может быть меньше или равно 0");
				}

				if (bookId <= 0)
				{
					_logger.LogError("Неверное значение параметра bookId: {BookId}. Значение должно быть больше 0.", bookId);
					throw new ArgumentException("Параметр bookId не может быть меньше или равно 0");
				}

				var subscriber = await _appDbContext.Users.FindAsync(userId);
                var book = await _appDbContext.Books.FindAsync(bookId);

                if (subscriber is null)
				{
					_logger.LogError("Пользователь с ID {UserId} не найден в базе данных", userId);
					throw new NullReferenceException($"Запись по userId не найдена. subscriber = null");
				}

				if (book is null)
				{
					_logger.LogError("Книга с ID {BookId} не найдена в базе данных", bookId);
					throw new NullReferenceException($"Запись по bookId не найдена. book = null");
				}

				var existingSubscription = subscriber.Subscriptions.SingleOrDefault(subs => subs.BookId == book.Id);

                if(existingSubscription is null)
                {
					_logger.LogInformation("Создание новой подписки для пользователя ID {UserId} на книгу ID {BookId}", userId, bookId);


					var newSubscription = new Subscription
                    {
                        UserId = userId,
                        BookId = bookId,
                        SubscriptionDate = DateTime.UtcNow,
                    };

                    await _appDbContext.AddAsync(newSubscription);
                    await _appDbContext.SaveChangesAsync();

					_logger.LogInformation("Подписка успешно создана для пользователя ID {UserId} на книгу ID {BookId}", userId, bookId);

					return newSubscription;
				}

				_logger.LogInformation("Подписка уже существует для пользователя ID {UserId} на книгу ID {BookId}", userId, bookId);
				return existingSubscription;
			}
            catch(Exception ex)
            {
				_logger.LogError(ex, "Ошибка при подписке пользователя ID {UserId} на книгу ID {BookId}", userId, bookId);
				throw;
			}
        }
    }
}
