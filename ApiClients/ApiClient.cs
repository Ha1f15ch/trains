
using CommonInterfaces.ApiClients;
using DatabaseEngine.Models;
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

		public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
		{
			// Создаем отдельно пакет, который будем отправлять, настраиваем путь и метод
			var requestMessage = new HttpRequestMessage()
			{
				RequestUri = new Uri(url),
				Method = HttpMethod.Post,
			};

			// Устанавливаем заголовок авторизации
			requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Negotiate");

			// Сериализуем данные
			var jsonData = JsonSerializer.Serialize(data);
			var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
			requestMessage.Content = content;

			// Отправка запроса и ожидание ответа
			var response = await _httpClient.SendAsync(requestMessage);

			// Проверка 
			if (!response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStringAsync();
				
				throw new Exception($"Ошибка: {response.StatusCode}, сообщение: {responseBody}, содержимое запроса: {jsonData}");
			}

			// Чтение и десериализация содержимого ответа в нужный тип
			var responseContent = await response.Content.ReadAsStringAsync();

			// Попробуем десериализовать ответ и обработать возможные ошибки
			try
			{
				return JsonSerializer.Deserialize<TResponse>(responseContent);
			}
			catch (JsonException jsonEx)
			{
				// Если произошла ошибка десериализации, выбрасываем исключение с сообщением об ошибке
				throw new Exception($"Ошибка десериализации ответа: {responseContent}. Причина: {jsonEx.Message}");
			}
		}

	}
}
