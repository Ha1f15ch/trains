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

var builder = WebApplication.CreateBuilder(args);

// Строка подключения к БД
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Строка подключения к БД для hangfire
var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");

// настройка Mediatr
builder.Services.AddMediatR(cfg =>
{
	cfg.RegisterServicesFromAssembly(typeof(CreateNewUserCommand).Assembly);
});

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
			builder.WithOrigins("http://localhost:5011", "https://localhost:7125")
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
// Настройка Api клиента
/*builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromMinutes(5);
});*/

/*builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IApiClient, ApiClient>();*/
builder.Services.AddScoped<JsonStringHandlerService>();
builder.Services.AddLogging();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddTransient<INewsChannelRepository, NewsChannelRepository>();
builder.Services.AddTransient<INewsChannelsPostsRepository, NewsChannelsPostsRepository>();
builder.Services.AddTransient<INewsChannelsSubscribersRepository, NewsChannelsSubscribersRepository>();
// Класс c di для джоба
/*builder.Services.AddScoped<EmailSenderWeekDelayJob>();*/
//игнорировать дефолт авторизацию в контроллере (api клиенте)
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

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

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My train API v1");
        });
}

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard(); // Включить поддержку hangfire дашбордов

// Запускаем джобы
/*JobExecution.StartJobs();*/

// Проверка доступа БД перед запуском приложения
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;

    try
    {
        var context = service.GetRequiredService<AppDbContext>();

        // проверка на то, создана ли БД и попытка подключиться к ней
        await context.Database.EnsureCreatedAsync();
        await context.Database.CanConnectAsync();

        Console.WriteLine("Подключение к БД выполнено успешно");
    }
    catch (Exception ex)
    {
		Console.WriteLine($"Подключение к БД не выполнено. Возникшая ошибка: {ex.Message}");
    }
}

// проверяем подключение к RabbitMq
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;

    try
    {
        var rabbitMqConnection = service.GetRequiredService<IConnection>();
        if(rabbitMqConnection.IsOpen)
        {
            Console.WriteLine("Подключение к RabbitMQ выполнено успешно");
		}
        else
        {
            throw new Exception("Ну удалось установить соединение с RabbitMQ");
        }

    }
    catch(Exception ex)
    {
		Console.WriteLine($"Подключение к RabbitMQ не выполнено. Возникшая ошибка: {ex.Message}");
		throw;
	}
}

    app.Run();