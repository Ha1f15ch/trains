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

        public SubscriptionRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        //Поиск подписок пользователя по userId
        public async Task<List<Subscription?>> GetUserSubscriptions(int userId)
        {
			try
			{
				// Получаем подписки пользователя за один запрос
				var userSubscriptions = await _appDbContext.Subscriptions
					.Where(sub => sub.UserId == userId)
					.ToListAsync();

				// Если пользователь не существует, подписок не будет
				if (!userSubscriptions.Any())
				{
					return new List<Subscription?>();
				}

				return userSubscriptions;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Не удалось получить подписки для пользователя ID {userId}.", ex);
			}
		}

        public async Task<Subscription> SubscribeUserToBook(int userId, int bookId)
        {
            try
            {
				// Валидация параметра userId
				if (userId <= 0)
                {
                    throw new ArgumentException("Параметр userId не может быть меньше или равно 0");
                }

				// Валидация параметра bookId
				if (bookId <= 0)
				{
					throw new ArgumentException("Параметр bookId не может быть меньше или равно 0");
				}

                //Проверка, есть ли в контексте для данных id записи
				var subscriber = await _appDbContext.Users.FindAsync(userId);
                var book = await _appDbContext.Books.FindAsync(bookId);

                if (subscriber is null)
                {
                    throw new NullReferenceException($"Запись по userId не найдена. subscriber = null");
                }

                if(book is null)
                {
					throw new NullReferenceException($"Запись по bookId не найдена. book = null");
				}

				var existingSubscription = subscriber.Subscriptions.SingleOrDefault(subs => subs.BookId == book.Id);

                //Если его нет
                if(existingSubscription is null)
                {
                    //Создание экземпляра подписки для сохранения в контексте
                    var newSubscription = new Subscription
                    {
                        UserId = userId,
                        BookId = bookId,
                        SubscriptionDate = DateTime.UtcNow,
                    };

                    await _appDbContext.AddAsync(newSubscription);
                    await _appDbContext.SaveChangesAsync();

                    //Создание специального типа данных для передачи в класс обработчик, ожидающие этот тип данных
                    var subscriptionDto = new SubscriptionDto
                    {
                        UserId = newSubscription.UserId,
                        BookId = newSubscription.BookId,
                        SubscriptionDate = newSubscription.SubscriptionDate
                    };

                    return newSubscription;
                }

				return existingSubscription;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
