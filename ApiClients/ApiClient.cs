
using CommonInterfaces.ApiClients;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;

namespace ApiClients
{
	public class ApiClient : IApiClient
	{
		private readonly HttpClient _httpClient;

		public ApiClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<T?> GetAsync<T>(string url)
		{
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
			requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Negotiate");

			var response = await _httpClient.SendAsync(requestMessage);

			if (!response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStringAsync();
				// Логируем или анализируем responseBody для получения информации об ошибке
				throw new Exception($"Ошибка: {response.StatusCode}, сообщение: {responseBody}");
			}

			var responseContent = await response.Content.ReadAsStringAsync();

			return JsonSerializer.Deserialize<T>(responseContent);
		}

		public async Task<T?> PostAsync<T>(string url, T data)
		{
			var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync(url, content);

			var responseContent = await response.Content.ReadAsStringAsync();

			return JsonSerializer.Deserialize<T>(responseContent);
		}
	}
}
