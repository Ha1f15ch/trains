using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
			if(_channel == null)
			{
				throw new InvalidOperationException("Канал не инициализирован.");
			}

			var consumer = new AsyncEventingBasicConsumer(_channel);

			consumer.ReceivedAsync += async (model, ea) =>
			{
				try
				{
					var body = ea.Body.ToArray();
					var message = JsonSerializer.Deserialize<dynamic>(Encoding.UTF8.GetString(body));

					var requestId = message.RequestId.ToString();
					var data = message.Data;

					// Выполняем валидацию
					bool validationResult = ValidateData(data);

					// Формируем ответ
					var responseMessage = new
					{
						RequestId = requestId,
						ValidationResult = validationResult
					};

					var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseMessage));

					// Отправляем результат в очередь validation.results
					var properties = new BasicProperties();
					await _channel.BasicPublishAsync<BasicProperties>(
						exchange: "",
						routingKey: "validation.results",
						mandatory: false,
						basicProperties: null,
						body: responseBytes
					);

					Console.WriteLine($"Validation result sent for RequestId: {requestId}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error processing message: {ex.Message}");
				}
			};

			_channel.BasicConsumeAsync(queue: "validation.requests", autoAck: true, consumer: consumer);
		}

		private bool ValidateData(dynamic data)
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
