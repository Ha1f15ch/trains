using BusinesEngine.Services.ServiceInterfaces;
using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApiApp.LogInfrastructure;

namespace WebApiApp.Controllers
{
    [ApiController]
    [Route("main/")]
    public class SimpleController : Controller
    {
        private readonly ILogService _logService;
		private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IBookRepository _bookRepository;

        public SimpleController(
            ILogService logService,
			IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository,
            IBookRepository bookRepository)
        {
            _logService = logService;
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
            _bookRepository = bookRepository;
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> CreateNewUser(string name
                                                      , string email
                                                      , string password
                                                      )
        {
            Console.WriteLine($"name = {name}, \nemail = {email}, \npassword = {password}");

            if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var result = await _userRepository.CreateNewUser(name, email, password, true, DateTime.UtcNow, DateTime.UtcNow, DateTime.MinValue);

                return Ok(result);
            }
            else
            {
                return BadRequest(404);
            }
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();

            return Ok(users);
        }

        [HttpGet("get-user-by-id/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var userById = await _userRepository.GetUserById(userId);

            if (userById == null) return BadRequest(405);

            return Ok(userById);
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(int userId, int bookId)
        {
            var subscription = await _subscriptionRepository.SubscribeUserToBook(userId, bookId);

            if (subscription == null) return BadRequest("Подписаться на книгу не получилось");

            return Ok(new
            {
                Message = "Подписка успешно создана.",
                Subscription = subscription,
                Notification = "Уведомление будет отправлено через 5 секунд."
            });
        }

        [HttpGet("subscriptions/{userId}")]
        public async Task<IActionResult> GetSubscriptions(int userId)
        {
            var subscriptions = await _subscriptionRepository.GetUserSubscriptions(userId);

            return Ok(subscriptions);
        }

        [HttpGet("get-all-books")]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookRepository.GetAllBooks();

            return Ok(books);
        }

        [HttpPost("get-books-by/{title}")]
        public async Task<IActionResult> GetBooksByTitle(string title)
        {
            var booksByTitle = await _bookRepository.GetBookByName(title);

            if (booksByTitle == null) return BadRequest("Для поиска переданы некорректные данные или в системе нет подходящих записей.");

            return Ok(booksByTitle);
        }

        [HttpPost("crate-new-book")]
        public async Task<IActionResult> CreateNewBook(string title, string? description, string? author, int? countList, string? createdAt)
        {
            var newBook = await _bookRepository.CreateNewBook(title, true, description, author, countList, createdAt, DateTime.UtcNow);

            if (newBook == null) return BadRequest("Для создания записи Book переданы некорректные данные. попробуйте позднее");

            return Ok(newBook);
        }
    }
}
