using ApiClients;
using CommonInterfaces.ApiClients;
using DatabaseEngine.Models;
using DtoForApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebAppTrain.Controllers
{
    [Route("main/testApiClient/")]
    [ApiController]
    [AllowAnonymous]
    public class TestApiClientController : ControllerBase
    {
        private readonly IApiClient _apiClient;

		public TestApiClientController(IApiClient apiClient)
		{
            _apiClient = apiClient;
		}

		[HttpGet("testApiGet")]
        public async Task<IActionResult> GetQueryForApiClient()
        {
            var url = "https://localhost:7125/main/v1/channel/all";

            // пробуем получить список новостных каналов
            var data = await _apiClient.GetAsync<List<DatabaseEngine.Models.NewsChannel>>(url);

            return Ok(data);
        }

        [HttpPost("testApiPost")]
        public async Task<IActionResult> SendDataToApiClient()
        {
            var url = "https://localhost:7125/main/add-user";

            // Отправляем данные, которые либо получаем откуда-то со стороны, либо генерируем сами
            // что за объект будет передавать в внешний api ? Для этого создаем отдельно проект с dto моделями - для внешнего общения
            // а модель просто заглушкой создаю
            var data = new UserDto
            {
                Name = "TestName1",
                Email = "TestName1@mail.ru",
                Password = "12345"
			};

            var result = await _apiClient.PostAsync<UserDto, User>(url, data);

            return Ok(result);
        }
    }
}
