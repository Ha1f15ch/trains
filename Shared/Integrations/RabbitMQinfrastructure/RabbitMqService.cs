using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Integrations.RabbitMqInfrastructure
{
	public class RabbitMqService : IDisposable//Отвечает за отправку данных в очередь и получение результатов из очереди
	{
		private readonly IConnection _connection;
		private IChannel? _channel;

		public RabbitMqService(IConnection connection)
		{
			_connection = connection;
		}

		public async Task InitializeAsync()
		{
			_channel = await _connection.CreateChannelAsync();

			await _channel.QueueDeclareAsync(queue: "validation.requests", durable: false, exclusive: false, autoDelete: false, arguments: null);
			await _channel.QueueDeclareAsync(queue: "validation.results", durable: false, exclusive: false, autoDelete: false, arguments: null);
		}

		public async Task PublishValidationRequestAsync(string requestId, object data) //Отправка данных на валидацию.
		{
			if (requestId == null)
			{
				throw new InvalidOperationException("Channel is not initialized.");
			}

			var message = new // Создаем объект сообщения.
			{
				RequestId = requestId, // Уникальный идентификатор запроса
				Data = data // Данные для валидации.
			};

			var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

			await _channel.BasicPublishAsync<BasicProperties>( //Отправляем сообщение в очередь
				exchange: "", //Используем пустой обменник (exchange).
				routingKey: "validation.requests", //Ключ маршрутизации (имя очереди).
				mandatory: false, //Сообщение может быть потеряно, если нет потребителей.
				basicProperties: null, //Дополнительные свойства сообщения отсутствуют.
				body: body //Тело сообщения (массив байтов).сериализованное 
			);
		}

		public async Task<string> GetValidationResultAsync(string requestId) //Получение результата валидации
		{
			if(_channel == null)
			{
				throw new InvalidOperationException("Channel is not initialized");
			}

			var result = "";

			var consumer = new AsyncEventingBasicConsumer(_channel); // Создаем асинхронного потребителя для получения сообщений.
			var tcs = new TaskCompletionSource<string>(); // Создаем TaskCompletionSource для ожидания результата.

			consumer.ReceivedAsync += async (model, ea) => // Подписываемся на событие получения сообщения.
			{
				try
				{
					var body = ea.Body.ToArray(); // Извлекаем тело сообщения (массив байтов).
					var message = JsonSerializer.Deserialize<dynamic>(Encoding.UTF8.GetString(body)); // Десериализуем сообщение из JSON.

					if (message.RequestId.ToString() == requestId) // Проверяем, соответствует ли RequestId ожидаемому.
					{
						result = message.ValidationResult.ToString(); // Извлекаем ValidationResult из сообщения.
						tcs.TrySetResult(result); // Устанавливаем результат в TaskCompletionSource.
					}
				}
				catch(Exception ex)
				{
					tcs.TrySetException(ex);
				}
			};

			await _channel.BasicConsumeAsync( // Начинаем получать сообщения из очереди.
				queue: "validation.results",
				autoAck: true, // Автоматически подтверждаем получение сообщения.
				consumer: consumer
			);

			return await tcs.Task;
		}

		public void Dispose()
		{
			_channel?.Dispose();
			_connection?.CloseAsync();
		}
	}
}
