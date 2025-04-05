using RabbitMQ.Client;
using ValidationService;
using Microsoft.Extensions.Configuration;
using System.IO;

class Program
{
	static async Task Main(string[] args)
	{
		// Загружаем конфигурацию из локального appsettings.json
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory()) // Текущая рабочая директория
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.Build();

		// Проверка загруженных значений
		Console.WriteLine($"RabbitMQ HostName: {configuration["RabbitMQ:HostName"]}");
		Console.WriteLine($"RabbitMQ UserName: {configuration["RabbitMQ:UserName"]}");
		Console.WriteLine($"RabbitMQ Password: {configuration["RabbitMQ:Password"]}");
		Console.WriteLine($"RabbitMQ Port: {configuration["RabbitMQ:Port"]}");

		// Загружаем конфигурацию RabbitMQ
		var factory = new ConnectionFactory
		{
			HostName = configuration["RabbitMQ:HostName"],
			UserName = configuration["RabbitMQ:UserName"],
			Password = configuration["RabbitMQ:Password"],
			Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672")
		};

		// Асинхронное создание соединения
		using var connection = await factory.CreateConnectionAsync();
		var validationConsumer = new ValidationConsumer(connection);

		// Инициализация канала и очередей
		await validationConsumer.InitializeAsync();

		// Запуск обработки сообщений
		validationConsumer.StartConsuming();

		Console.WriteLine("Validation service is running. Press [Enter] to exit.");
		Console.ReadLine();
	}
}