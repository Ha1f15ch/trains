using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApiApp.LogInfrastructure;
using WebAppTrain.Repositories.Intefaces;

namespace WebApiApp.Controllers
{
    [ApiController]
    [Route("main/")]
    public class SimpleController : Controller
    {
        private readonly LogService _logService;
        private readonly IExampleRepository _exampleRepository;
        private readonly IUserRepository _userRepository;

        public SimpleController(LogService logService
                                ,IExampleRepository exampleRepository
                                , IUserRepository userRepository)
        {
            _logService = logService;
            _exampleRepository = exampleRepository;
            _userRepository = userRepository;
        }

        [HttpGet("start")]
        public IActionResult Startmethod()
        {
            try
            {
                _logService.LogInformation("Вызван метод - {MethodName}", nameof(Startmethod));
                var items = _exampleRepository.GetItems();

                _logService.LogInformation("Репозиторий, который был использован: {RepositoryMethod}", nameof(IExampleRepository.GetItems));

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logService.LogError("Ошибка при выполнении метода {MethodName}: {Message}", nameof(Startmethod), ex.Message);

                return StatusCode(500, "Internal server error");
            }
            finally
            {
                _logService.LogInformation("Окончательное выполнение метода контроллера - Finally", "Значения нет");
            }
        }

        [HttpGet("add-user")]
        public async Task<IActionResult> CreateNewUserTestGetMethod(string name
                                                      , string email
                                                      , string password
                                                      )
        {
            Console.WriteLine($"name = {name}, \nemail = {email}, \npassword = {password}");

            return Ok(200);
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
    }
}
