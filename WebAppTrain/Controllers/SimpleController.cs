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

        public SimpleController(LogService logService,
                                IExampleRepository exampleRepository)
        {
            _logService = logService;
            _exampleRepository = exampleRepository;
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
    }
}
