using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BusinesEngine.Services
{
    public class JsonStringHandlerService
    {
        private readonly JsonSerializerSettings _jsonSerializeSettings;
        private readonly ILogger<JsonStringHandlerService> _logger;

		public JsonStringHandlerService(ILogger<JsonStringHandlerService> logger)
		{
            _jsonSerializeSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented // форматирование в привычный вид выводимой строки
            };
            _logger = logger;

			_logger.LogInformation("Инициализирован сервис сериализации JSON");
		}

		//Преобразование строки в объект json (лист/массив)
		public Task<string> SerializeList<T>(List<T> items) => Task.Run(() =>
		{
			if (items.Count == 0)
			{
				_logger.LogWarning("Попытка сериализации пустого списка объектов");
				throw new ArgumentNullException("Для сериализации объектов переданы некорректные данные");
			}

			_logger.LogInformation("Сериализация списка объектов. Количество элементов: {Count}", items.Count);
			return JsonConvert.SerializeObject(items, _jsonSerializeSettings);
		});

		// Преобразование строки в объект json (один элемент)
		public Task<string> SerializeSingle<T>(T item) => Task.Run(() =>
		{
			if (item is null)
			{
				_logger.LogWarning("Попытка сериализации null-объекта");
				throw new NullReferenceException("Для сериализации объектов переданы некорректные данные");
			}

			_logger.LogInformation("Сериализация одного объекта");
			return JsonConvert.SerializeObject(item, _jsonSerializeSettings);
		});
	}
}
