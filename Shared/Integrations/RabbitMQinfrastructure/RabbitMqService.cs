using DTOModels.ValidationService;
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
				Console.WriteLine("Очередь 'validation.requests' успешно объявлена.");

				await _channel.QueueDeclareAsync(queue: "validation.results", durable: false, exclusive: false, autoDelete: false, arguments: null);
				Console.WriteLine("Очередь 'validation.results' успешно объявлена.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при инициализации канала в RabbitMQ: {ex.Message}");
				throw;
			}
		}

		public async Task PublishValidationRequestAsync(string requestId, NewUserData data) //Отправка данных на валидацию.
		{
			// Проверяем состояние канала
			if (_channel == null || !_channel.IsOpen)
			{
				_channel = await _connection.CreateChannelAsync();
				Console.WriteLine("Канал успешно переинициализирован.");
			}

			if (_channel == null || !_channel.IsOpen || !_connection.IsOpen)
			{
				throw new InvalidOperationException("Канал или соединение не открыты.");
			}
			Console.WriteLine("Канал и соединение открыты.");

			if (requestId == null)
			{
				throw new InvalidOperationException("Канал не инициализирован.");
			}
			else
			{
				Console.WriteLine($"\nRequestId = {requestId}\n");
			}

			var message = new ValidationRequest // Создаем объект сообщения.
			{
				RequestId = requestId, // Уникальный идентификатор запроса
				Data = new NewUserData // Используем строго типизированный объект
				{
					Name = data.Name,
					Email = data.Email,
					Password = data.Password
				}
				// Данные для валидации.
			};

			var jsonMessage = JsonSerializer.Serialize(message);
			Console.WriteLine($"Сериализованное сообщение: {jsonMessage}");

			var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

			Console.WriteLine($"\nbody = {body.Count()}\n");

			try
			{
				// Проверяем существование очереди	
				var properties = new BasicProperties();
				properties.Persistent = true; // Сохранять сообщения на хосте

				await _channel.BasicPublishAsync<BasicProperties>(
					exchange: "",
					routingKey: "validation.requests",
					mandatory: false,
					basicProperties: properties,
					body: body
				);

				Console.WriteLine("Сообщение успешно опубликовалось в очереди.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка публикации сообщения в очереди: {ex.Message}");
				throw;
			}
		}

		public async Task<ValidationResponse?> GetValidationResultAsync(string requestId) // Получение результата валидации
		{
			if (_channel == null)
			{
				throw new InvalidOperationException("Канал не инициализирован");
			}

			var tcs = new TaskCompletionSource<ValidationResponse?>();

			var consumer = new AsyncEventingBasicConsumer(_channel);

			consumer.ReceivedAsync += async (model, ea) =>
			{
				try
				{
					var body = ea.Body.ToArray();
					var messageJson = Encoding.UTF8.GetString(body);

					ValidationResponse? message = null;
					try
					{
						message = JsonSerializer.Deserialize<ValidationResponse>(messageJson);

						if (message == null || string.IsNullOrEmpty(message.RequestId))
						{
							Console.WriteLine($"Невозможно десериализовать сообщение: {messageJson}");
							return;
						}
					}
					catch (JsonException ex)
					{
						Console.WriteLine($"Ошибка при десериализации JSON: {ex.Message}. Полученные данные: {messageJson}");
						return;
					}

					if (message.RequestId == requestId) // Если RequestId совпадает, завершаем ожидание (TaskCompletionSource)
					{
						tcs.TrySetResult(message);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Ошибка при обработке сообщения: {ex.Message}");
					tcs.TrySetException(ex);
				}
			};

			// Подписываемся на очередь
			var consumerTag = await _channel.BasicConsumeAsync(
				queue: "validation.results",
				autoAck: true,
				consumer: consumer
			);

			// Добавляем таймаут на случай, если сообщение не придет
			var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30)); // Таймаут 30 секунд
			var resultTask = tcs.Task;

			var completedTask = await Task.WhenAny(resultTask, timeoutTask);
			if (completedTask == timeoutTask)
			{
				// Отписка при таймауте
				await _channel.BasicCancelAsync(consumerTag);
				throw new TimeoutException("Получение результата валидации превысило время ожидания.");
			}

			await _channel.BasicCancelAsync(consumerTag);
			return await resultTask;
		}

		public void Dispose()
		{
			_channel?.Dispose();
			_connection?.CloseAsync();
		}
	}
}
