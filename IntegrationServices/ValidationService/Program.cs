using RabbitMQ.Client;
using ValidationService;
using Microsoft.Extensions.Configuration;
using System.IO;

class Program
{
	static async Task Main(string[] args)
	{
		// ��������� ������������ �� ���������� appsettings.json
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory()) // ������� ������� ����������
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.Build();

		// �������� ����������� ��������
		Console.WriteLine($"RabbitMQ HostName: {configuration["RabbitMQ:HostName"]}");
		Console.WriteLine($"RabbitMQ UserName: {configuration["RabbitMQ:UserName"]}");
		Console.WriteLine($"RabbitMQ Password: {configuration["RabbitMQ:Password"]}");
		Console.WriteLine($"RabbitMQ Port: {configuration["RabbitMQ:Port"]}");

		// ��������� ������������ RabbitMQ
		var factory = new ConnectionFactory
		{
			HostName = configuration["RabbitMQ:HostName"],
			UserName = configuration["RabbitMQ:UserName"],
			Password = configuration["RabbitMQ:Password"],
			Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672")
		};

		// ����������� �������� ����������
		using var connection = await factory.CreateConnectionAsync();
		var validationConsumer = new ValidationConsumer(connection);

		// ������������� ������ � ��������
		await validationConsumer.InitializeAsync();

		// ������ ��������� ���������
		validationConsumer.StartConsuming();

		Console.WriteLine("Validation service is running. Press [Enter] to exit.");
		Console.ReadLine();
	}
}