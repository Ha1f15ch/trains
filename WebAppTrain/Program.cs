using RabbitMQ.Client;
using BusinesEngine.Services;
using DatabaseEngine;
using DatabaseEngine.RepositoryStorage.Interfaces;
using DatabaseEngine.RepositoryStorage.Repositories;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.PostgreSql;
using BusinesEngine.MediatorInstruction.Commands.UsersCommand;
using Serilog;
using Integrations.RabbitMqInfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);

// Строка подключения к БД
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Строка подключения к БД для hangfire
var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");
//Инициализация настроек логирования serilog
Common.Logging.LoggingConfiguration.Configure();

builder.Host.UseSerilog();
// настройка Mediatr
builder.Services.AddMediatR(cfg =>
{
	cfg.RegisterServicesFromAssembly(typeof(CreateNewUserCommand).Assembly);
});

try
{
    // Конфигурация подключения к сервису RabbitMQ - развернут локально на ПК 
    builder.Services.AddSingleton<IConnection>(provider =>
    {
        var factory = new ConnectionFactory
        {
            HostName = builder.Configuration["RabbitMQ:HostName"],
            UserName = builder.Configuration["RabbitMQ:UserName"],
            Password = builder.Configuration["RabbitMQ:Password"],
            Port = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672")
        };
        return Task.Run(() => factory.CreateConnectionAsync()).GetAwaiter().GetResult();
    });

	// регистрация и запуск RabbitMQ
	builder.Services.AddSingleton<RabbitMqService>(provider =>
	{
		var connection = provider.GetRequiredService<IConnection>();
		return new RabbitMqService(connection);
	});
}
catch (Exception ex)
{
	Console.WriteLine($"Ошибка инициализации подключения к сервису RabbitMQ - {ex.Message}");
}

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);
// Нужно для того, чтобы возвращаемые объекты - типа моделей из БД (у которых есть референсные значения, навигационные свойства) не зацикливались сами на себя 
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin",
		builder =>
		{
			builder.WithOrigins("http://localhost:5011", "https://localhost:7125", "http://localhost:5000")
								 .AllowAnyMethod()
								 .AllowAnyHeader();
        }
    );
});

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "My train API",
            Version = "v1",
            Description = "API для практики и применения подходов и технологий из roadmap-a"
        });
    });

builder.Services.AddScoped<JsonStringHandlerService>();
builder.Services.AddLogging();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddTransient<INewsChannelRepository, NewsChannelRepository>();
builder.Services.AddTransient<INewsChannelsPostsRepository, NewsChannelsPostsRepository>();
builder.Services.AddTransient<INewsChannelsSubscribersRepository, NewsChannelsSubscribersRepository>();

//игнорировать дефолт авторизацию в контроллере (api клиенте)
/*builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});*/

// настройка hangfire 
builder.Services.AddHangfire(options =>
{
    //Необходимо создавать БД из connectionString до того, как начнется обращение по этим данным к БД
    options.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new PostgreSqlStorageOptions
    {
        PrepareSchemaIfNecessary = true
    });
});
	
builder.Services.AddHangfireServer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My train API v1");
    });
}


app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Все контроллеры требуют аутентификацию
app.MapControllers();

app.UseHangfireDashboard(); // Включить поддержку hangfire дашбордов

// Явная инициализация RabbitMqService
using (var scope = app.Services.CreateScope())
{
	var rabbitMqService = scope.ServiceProvider.GetRequiredService<RabbitMqService>();
	await rabbitMqService.InitializeAsync();
}

// Проверка доступа БД перед запуском приложения
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;

	var logger = app.Services.GetRequiredService<ILogger<Program>>();

	try
    {
        var context = service.GetRequiredService<AppDbContext>();

        // проверка на то, создана ли БД и попытка подключиться к ней
        await context.Database.EnsureCreatedAsync();
        await context.Database.CanConnectAsync();
		logger.LogInformation("Connect to database is completed successful");

        await context.Database.MigrateAsync();
        logger.LogInformation($"Migration is completed successfully");
    }
    catch (Exception ex)
    {
		logger.LogError(ex, $"Connect to database is not complete or migration is failed, process failed. Error: {ex.Message}");
    }
}

// проверяем подключение к RabbitMq
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;

	var logger = app.Services.GetRequiredService<ILogger<Program>>();

	try
    {
        var rabbitMqConnection = service.GetRequiredService<IConnection>();
        if(rabbitMqConnection.IsOpen)
        {

			logger.LogInformation("Connect to RabbitMQ complete is successful");
		}
        else
        {
            throw new Exception("Connection to RabbitMQ is not completed successful");
        }

    }
    catch(Exception ex)
    {
		logger.LogError(ex, $"Connection to RabbitMQ is not completed successful. Error: {ex.Message}");
		throw;
	}
}

    app.Run();