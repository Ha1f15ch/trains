using MediatR;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DTOs;
using BusinesEngine.Events;
using BusinesEngine.Services.ServiceInterfaces;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogService _logService;
        private readonly IMediator _mediator;

        public SubscriptionRepository(AppDbContext appDbContext, ILogService logService, IMediator mediator)
        {
            _appDbContext = appDbContext;
            _logService = logService;
            _mediator = mediator;
        }

        //Поиск подписок пользователя по userId
        public async Task<List<Subscription>> GetUserSubscriptions(int userId)
        {
            try
            {
                //Проверяем, есть ли пользователь
                var userExists = await _appDbContext.Users.AnyAsync(u => u.Id == userId);

                if(userExists)
                {
                    _logService.LogInformation($"{nameof(GetUserSubscriptions)} - Выполняется поиск подписок пользователя - {userId}.");
                    
                    var serSubscriptions = await _appDbContext.Subscriptions.Where(sub => sub.UserId == userId).ToListAsync();

                    _logService.LogInformation($"Найдено записей - {serSubscriptions.Count}");

                    return serSubscriptions;
				}

                throw new ArgumentException($"Пользователь с ID {userId} не найден.");
            }
            catch (Exception ex)
            {
                _logService.LogError($"{nameof(GetUserSubscriptions)} - Возникла ошибка при выполнении метода поиска подписок для пользователя {userId}. Ошибка - {ex.Message}");
                throw;
            }
        }

        public async Task<Subscription> SubscribeUserToBook(int userId, int bookId)
        {
            try
            {
				_logService.LogWarning($"Проверка входных параметров");

				// Валидация параметра userId
				if (userId <= 0)
                {
                    _logService.LogWarning($"Передано некорректное значение для userId.");
                    throw new ArgumentException("Параметр userId не может быть меньше или равно 0");
                }

				// Валидация параметра bookId
				if (bookId <= 0)
				{
					_logService.LogWarning($"Передано некорректное значение для bookId.");
					throw new ArgumentException("Параметр bookId не может быть меньше или равно 0");
				}

                //Проверка, есть ли в контексте для данных id записи
				var subscriber = await _appDbContext.Users.FindAsync(userId);
                var book = await _appDbContext.Books.FindAsync(bookId);

                if (subscriber is null)
                {
                    _logService.LogWarning($"Запись - subscriber не найдена. {subscriber}");
                    throw new NullReferenceException($"Запись по userId не найдена. subscriber = null");
                }

                if(book is null)
                {
                    _logService.LogWarning($"Запись - book не найдена. {book}");
					throw new NullReferenceException($"Запись по bookId не найдена. book = null");
				}

				_logService.LogInformation($"проверка, является ли пользователь с userId = {userId} подписчиком на книгу - {bookId}");

				var existingSubscription = subscriber.Subscriptions.SingleOrDefault(subs => subs.BookId == book.Id);

                //Если его нет
                if(existingSubscription is null)
                {
                    _logService.LogInformation($"Создание новой записи подписки пользователя - {userId} на книгу - {bookId}");

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

                    //Вызов события в сервисе mediatr, передача в него специального типа данных 
                    await _mediator.Publish(new SubscriptionCreatedEvent(subscriptionDto));

                    _logService.LogInformation($"{nameof(SubscribeUserToBook)} - Выполняется оповещение пользователя {userId} о том, что подписка на книгу {bookId} выполнена успешно. Была создана запись подписки {subscriptionDto} - {nameof(subscriptionDto)}");

                    return newSubscription;
                }

				_logService.LogInformation($"Пользователь с userId = {userId} уже подписан на книгу boolId = {bookId}");

				return existingSubscription;
            }
            catch(Exception ex)
            {
                _logService.LogError($"{nameof(SubscribeUserToBook)} - Возникла ошибка при выполнении метода создания подписки пользователя {userId} на книгу - {bookId}. Ошибка - {ex.Message}");
                throw;
            }
        }
    }
}
