using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApiApp.LogInfrastructure;

namespace WebApiApp.Controllers
{
    [ApiController]
    [Route("main/")]
    public class SimpleController : Controller
    {
        private readonly LogService _logService;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SimpleController(LogService logService
                                , IUserRepository userRepository
                                , ISubscriptionRepository subscriptionRepository)
        {
            _logService = logService;
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        [HttpGet("add-user")]
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

            return Ok(subscription);
        }

        [HttpGet("subscriptions/{userId}")]
        public async Task<IActionResult> GetSubscriptions(int userId)
        {
            var subscriptions = await _subscriptionRepository.GetUserSubscriptions(userId);

            return Ok(subscriptions);
        }
    }
}
