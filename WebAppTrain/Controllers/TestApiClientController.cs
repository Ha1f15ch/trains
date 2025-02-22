using ApiClients;
using CommonInterfaces.ApiClients;
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
    }
}
