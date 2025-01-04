using Microsoft.AspNetCore.Mvc;
using WebApiApp.LogInfrastructure;

namespace WebApiApp.Controllers
{
    [ApiController]
    [Route("main/")]
    public class SimpleController : Controller
    {
        private readonly LogService _logService;

        public SimpleController(LogService logService)
        {
            _logService = logService;
        }

        [HttpGet("start")]
        public IActionResult Startmethod()
        {
            try
            {
                _logService.LogInformation("Pезультат выполнения стартового метода");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи лога: {ex.Message}");

            }

            return Ok("Страница метода Startmethod");
        }
    }
}
