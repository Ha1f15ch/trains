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
		private readonly JsonStringHandlerService _jsonStringHandlerService;
        private readonly IMediator _mediator;

		public SimpleController(
			JsonStringHandlerService jsonStringHandlerService,
            IMediator mediator)
        {
            _jsonStringHandlerService = jsonStringHandlerService;
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("add-user")]
        public async Task<IActionResult> CreateNewUser([FromBody] UserDto userData)
        {
			if (userData is null)
			{
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

			var result = await _mediator.Send(command);

			var serializedResult = await _jsonStringHandlerService.SerializeSingle(result);

			return Ok(serializedResult);
		}

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
			var query = new GetAllUsersQuery();
			var users = await _mediator.Send(query);

			return Ok(users);
		}

        [HttpGet("get-user-by-id/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var command = new GetUserByIdCommand
            {
                UserId = userId
            };

            var user = await _mediator.Send(command);

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

				var result = _mediator.Send(command);

				return Ok(new
				{
					Message = "Подписка успешно создана.",
					Subscription = result,
					Notification = "Уведомление будет отправлено через 5 секунд."
				});
			}
            catch(Exception ex)
            {
				return BadRequest("Подписаться на книгу не получилось");
			}
        }

        [HttpGet("users/{userId}/subscriptions")]
        public async Task<IActionResult> GetSubscriptions(int userId)
        {
            var command = new GetUserSubscriptionsQuery
            {
                UserId = userId
            };

            var userSubscriptions = await _mediator.Send(command);

            if(userSubscriptions is null)
            {
				Console.WriteLine("Подписок не найдено");
				return Ok(userSubscriptions);
			}

            return Ok(userSubscriptions);
        }

        [HttpGet("books/get-all-books")]
        public async Task<IActionResult> GetAllBooks()
        {
            var command = new GetAllBooksQuery();

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("books/get-books-by-titleName/{title}")]//Поиск по частичному совпадению
        public async Task<IActionResult> GetBooksByTitle(string title)
        {
            var command = new GetBookByNameCommand { PartTitleName = title };

            var books = await _mediator.Send(command);

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

            var result = await _mediator.Send(command);

            if (result != null)
            {
                return Ok("Книга не создана");
            }

            return Ok(result);
		}
    }
}
