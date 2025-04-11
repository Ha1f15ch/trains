using BusinesEngine.MediatorInstruction.Commands.BookCommand;
using BusinesEngine.MediatorInstruction.Commands.BookCommand.Queries;
using BusinesEngine.MediatorInstruction.Commands.UsersCommand;
using BusinesEngine.MediatorInstruction.Commands.UsersCommand.Queries;
using BusinesEngine.Services;
using DTOModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiApp.Controllers
{
    [ApiController]
    [Route("main/")]
    public class SimpleController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SimpleController> _logger;

		public SimpleController(IMediator mediator, ILogger<SimpleController> logger)
		{
			_mediator = mediator;
			_logger = logger;
		}

		[AllowAnonymous]
        [HttpPost("add-user")]
        public async Task<IActionResult> CreateNewUser([FromBody] UserDto userData)
        {
            _logger.LogInformation($"Получаем {userData}:\nuserData.Name = {userData.Name}\nuserData.Email = {userData.Email}\nuserData.Password = {userData.Password}\n");

			if (userData is null)
			{
                _logger.LogWarning($"Объект User не может быть равен null");
				return BadRequest("Объект User не может быть равен null");
			}

			var command = new CreateNewUserCommand //Заменить на автомаппер, скорее всего
			{
				Name = userData.Name,
				Email = userData.Email,
				Password = userData.Password,
				IsActive = true,
				DateCreate = DateTime.UtcNow,
				DateUpdate = DateTime.UtcNow,
				DateDelete = DateTime.MinValue
			};

            _logger.LogInformation($"Создаем команду для обработчика {command}");

			var result = await _mediator.Send(command);

            _logger.LogInformation($"Получаем результат {command.Name} {result}");

			return Ok(result);
		}

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
			var query = new GetAllUsersQuery();

			_logger.LogInformation($"Формируем команду для отправки {query}");

			var users = await _mediator.Send(query);

            _logger.LogInformation($"Получили результат {users.Count} users");

			return Ok(users);
		}

        [HttpGet("get-user-by-id/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var command = new GetUserByIdCommand
            {
                UserId = userId
            };

            _logger.LogInformation($"Формируем запрос для отправки {command} с userId = {userId}");

            var user = await _mediator.Send(command);

            _logger.LogInformation($"Получили результат: name - {user?.Name}");

            return Ok(user);
        }

        [HttpPost("books/{bookId}/subscribe")]
        public async Task<IActionResult> Subscribe(int userId, int bookId)
        {
            try
            {
				var command = new UserSubscriptionToBookCommand
				{
					UserId = userId,
					BookId = bookId
				};

                _logger.LogInformation($"Формирование команды для выполнения запроса подписи {command}");

				var result = await _mediator.Send(command);

                _logger.LogInformation($"Получен результат - {result.Id}");

				return Ok(new
				{
					Message = "Подписка успешно создана.",
					Subscription = result,
					Notification = "Уведомление будет отправлено через 5 секунд."
				});
			}
            catch(Exception ex)
            {
				_logger.LogWarning(ex, $"Подписаться на книгу не получилось. {ex.Message}");
				return BadRequest($"Подписаться на книгу не получилось.");
			}
        }

        [HttpGet("users/{userId}/subscriptions")]
        public async Task<IActionResult> GetSubscriptions(int userId)
        {
            var command = new GetUserSubscriptionsQuery
            {
                UserId = userId
            };

            _logger.LogInformation($"Сформирован запрос для поиска подписок пользователя c ID = {command.UserId}");

            var userSubscriptions = await _mediator.Send(command);

            if(userSubscriptions is null)
            {
				_logger.LogInformation("Подписок не найдено");
				return Ok(userSubscriptions);
			}

            _logger.LogInformation($"Найдено {userSubscriptions.Count} subscriptions пользователя с ID = {command.UserId}");

            return Ok(userSubscriptions);
        }

        [HttpGet("books/get-all-books")]
        public async Task<IActionResult> GetAllBooks()
        {
            var command = new GetAllBooksQuery();

            _logger.LogInformation($"Получить все книги. Сформирован запрос");

            var result = await _mediator.Send(command);

            _logger.LogInformation($"Получено {result.Count} книг");

            return Ok(result);
        }

        [HttpPost("books/get-books-by-titleName/{title}")]//Поиск по частичному совпадению
        public async Task<IActionResult> GetBooksByTitle(string title)
        {
            var command = new GetBookByNameCommand { PartTitleName = title };

            _logger.LogInformation($"Сформирована команда запроса поиска книг по совпадающему TitleName- {command.PartTitleName}");

            var books = await _mediator.Send(command);

            _logger.LogInformation($"Получено {books.Count} подходящих books");

            return Ok(books);
        }

        [HttpPost("books/crate-new-book")]
        public async Task<IActionResult> CreateNewBook(string title, string? description, string? author, int? countList, string? createdAt)
        {
            var command = new CreateNewBookCommand
            {
                Title = title,
                IsActive = true,
                Description = description,
                Author = author,
                CountList = countList,
                CreatedAt = createdAt,
                UpdateDate = DateTime.UtcNow
            };

			_logger.LogInformation($"Команда для отправки сформирована - {command}");

			var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogWarning($"Книга не создана result = null");

                return Ok("Книга не создана");
            }

            _logger.LogInformation($"Получен результат - {result.Id}, {result.Title}");

            return Ok(result);
		}

        [HttpGet("hello")]
        public async Task<IActionResult> GetHelloForTest()
        {
            return Ok("API is work !");
        }
    }
}
