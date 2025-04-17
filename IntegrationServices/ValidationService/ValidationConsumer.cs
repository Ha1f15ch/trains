using DTOModels.ValidationService;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ValidationService
{
	public class ValidationConsumer : IDisposable
	{
		private readonly IConnection _connection;
		private IChannel? _channel;

		public ValidationConsumer(IConnection connection)
		{
			_connection = connection ?? throw new ArgumentNullException($"При выполнении подключения сервиса валидации к каналу Rabbit возникла ошибка {nameof(connection)}");
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
				Console.WriteLine("Канал успешно создан.");

				// Объявляем очереди
				await _channel.QueueDeclareAsync(queue: "validation.requests", durable: false, exclusive: false, autoDelete: false, arguments: null);
				Console.WriteLine("Очередь 'validation.requests' успешно объявлена.");

				await _channel.QueueDeclareAsync(queue: "validation.results", durable: false, exclusive: false, autoDelete: false, arguments: null);
				Console.WriteLine("Очередь 'validation.results' успешно объявлена.");
			}
			catch(Exception ex)
			{
				Console.WriteLine($"Ошибка при инициализации канала в RabbitMQ: {ex.Message}");
				throw;
			}
		}

		public void StartConsuming()
		{
			if(_channel == null || !_channel.IsOpen)
			{
				throw new InvalidOperationException("Канал RabbitMQ закрыт или не существует.");
			}

			var consumer = new AsyncEventingBasicConsumer(_channel);

			consumer.ReceivedAsync += async (model, ea) =>
			{
				try
				{
					var body = ea.Body.ToArray();

					var messageJson = Encoding.UTF8.GetString(body);

					// Строго типизированная десериализация
					var message = JsonSerializer.Deserialize<ValidationRequest>(messageJson);

					if (message == null || string.IsNullOrEmpty(message.RequestId))
					{
						Console.WriteLine($"Невозможно десериализовать сообщение или отсутствует RequestId: {messageJson}");
						return;
					}

					var requestId = message.RequestId;

					Console.WriteLine($"Получен запрос на валидацию. RequestId: {requestId}");

					// Выполняем валидацию
					bool validationResult = ValidateData(message.Data);

					// Формируем ответ
					var responseMessage = new ValidationResponse
					{
						RequestId = requestId,
						ValidationResult = validationResult
					};

					var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseMessage));

					if (_channel == null || !_channel.IsOpen)
					{
						throw new InvalidOperationException("Канал RabbitMQ закрыт или не существует.");
					}

					// Отправляем результат в очередь validation.results
					var properties = new BasicProperties();
					properties.Persistent = true;

					await _channel.BasicPublishAsync<BasicProperties>(
						exchange: "",
						routingKey: "validation.results",
						mandatory: false,
						basicProperties: properties,
						body: responseBytes
					);

					Console.WriteLine($"Validation result sent for RequestId: {requestId}");
					Console.WriteLine($"Отправляем подтверждение обработки сообщения RequestId: {requestId}");

					// Подтверждаем обработку сообщения
					await _channel.BasicAckAsync(ea.DeliveryTag, false);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error processing message: {ex.Message}");
					await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
				}
			};

			_channel.BasicConsumeAsync(queue: "validation.requests", autoAck: false, consumer: consumer);
		}

		private bool ValidateData(NewUserData data)
		{
			var name = data.Name?.ToString();
			var email = data.Email?.ToString();
			var password = data.Password?.ToString();

			if (string.IsNullOrEmpty(name))
			{
				Console.WriteLine($"Значение name = null");
				return false;
			}
			
			if (string.IsNullOrEmpty(email))
			{
				Console.WriteLine($"Значение email = null");
				return false;
			}
			
			if (string.IsNullOrEmpty(password))
			{
				Console.WriteLine($"Значение password = null");
				return false;
			}

			return true;
		}

		public void Dispose()
		{
			_channel?.Dispose();
			_connection.CloseAsync();
		}
	}
}
