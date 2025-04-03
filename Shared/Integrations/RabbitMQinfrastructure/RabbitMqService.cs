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
			try
			{
				if (_connection == null || !_connection.IsOpen)
				{
					throw new InvalidOperationException("Соединение RabbitMQ не открыто.");
				}

				_channel = await _connection.CreateChannelAsync();

				await _channel.QueueDeclareAsync(queue: "validation.requests", durable: false, exclusive: false, autoDelete: false, arguments: null);
				Console.WriteLine("Queue 'validation.requests' declared successfully.");

				await _channel.QueueDeclareAsync(queue: "validation.results", durable: false, exclusive: false, autoDelete: false, arguments: null);
				Console.WriteLine("Queue 'validation.results' declared successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при инициализации канала в RabbitMQ: {ex.Message}");
				throw;
			}
		}

		public async Task PublishValidationRequestAsync(string requestId, object data) //Отправка данных на валидацию.
		{
			// Проверяем состояние канала
			if (_channel == null || !_channel.IsOpen)
			{
				_channel = await _connection.CreateChannelAsync();
				Console.WriteLine("Channel recreated successfully.");
			}

			if (_channel == null || !_channel.IsOpen || !_connection.IsOpen)
			{
				throw new InvalidOperationException("Channel or connection is not open.");
			}
			Console.WriteLine("Channel and connection are open.");

			if (requestId == null)
			{
				throw new InvalidOperationException("Channel is not initialized.");
			}
			else
			{
				Console.WriteLine($"\nRequestId = {requestId}\n");
			}

			var message = new // Создаем объект сообщения.
			{
				RequestId = requestId, // Уникальный идентификатор запроса
				Data = data // Данные для валидации.
			};

			var jsonMessage = JsonSerializer.Serialize(message);
			Console.WriteLine($"Serialized message: {jsonMessage}");

			var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

			Console.WriteLine($"\nbody = {body}\n");

			try
			{
				// Проверяем существование очереди
				try
				{
					await _channel.QueueDeclarePassiveAsync("validation.requests");
					Console.WriteLine("Queue 'validation.requests' exists.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Queue 'validation.requests' does not exist: {ex.Message}");
					await _channel.QueueDeclareAsync(
						queue: "validation.requests",
						durable: false,
						exclusive: false,
						autoDelete: false,
						arguments: null
					);
					Console.WriteLine("Queue 'validation.requests' declared successfully.");
				}

				Console.WriteLine("Before BasicPublishAsync");
				var properties = new BasicProperties();
				await _channel.BasicPublishAsync<BasicProperties>(
					exchange: "",
					routingKey: "validation.requests",
					mandatory: false,
					basicProperties: properties,
					body: body
				);
				Console.WriteLine("After BasicPublishAsync");

				Console.WriteLine("Message published successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error publishing message: {ex.Message}");
				throw;
			}
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
