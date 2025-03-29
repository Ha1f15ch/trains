using RabbitMQ.Client;
using BusinesEngine.Services;
using DatabaseEngine;
using DatabaseEngine.RepositoryStorage.Interfaces;
using DatabaseEngine.RepositoryStorage.Repositories;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;
using WebApiApp.LogInfrastructure;
using WebAppTrain.LogInfrastructure;
using BusinesEngine.Events;
using BusinesEngine.Services.ServiceInterfaces;
using Hangfire;
using Hangfire.PostgreSql;
using BusinesEngine.Commands.UsersCommand;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ������ ����������� � ��
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// ������ ����������� � �� ��� hangfire
var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");

var logConfigurator = new LogServiceConfigurator(builder.Configuration);
logConfigurator.Configure();

// ��������� Mediatr
builder.Services.AddMediatR(cfg =>
{
	cfg.RegisterServicesFromAssembly(typeof(CreateNewUserCommand).Assembly);
});

// ������������ ����������� � ������� RabbitMQ - ��������� �������� �� �� 
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
// ����� ��� ����, ����� ������������ ������� - ���� ������� �� �� (� ������� ���� ����������� ��������, ������������� ��������) �� ������������� ���� �� ���� 
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
            Description = "API ��� �������� � ���������� �������� � ���������� �� roadmap-a"
        });
    });
// ��������� Api �������
/*builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromMinutes(5);
});*/

/*builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IApiClient, ApiClient>();*/
builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddScoped<LogSubscriber>();
builder.Services.AddScoped<NewsPublisher>();
builder.Services.AddScoped<EmailNotificationService>();
builder.Services.AddScoped<JsonStringHandlerService>();
builder.Services.AddLogging();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddTransient<INewsChannelRepository, NewsChannelRepository>();
builder.Services.AddTransient<INewsChannelsPostsRepository, NewsChannelsPostsRepository>();
builder.Services.AddTransient<INewsChannelsSubscribersRepository, NewsChannelsSubscribersRepository>();
// ����� c di ��� �����
/*builder.Services.AddScoped<EmailSenderWeekDelayJob>();*/
//������������ ������ ����������� � ����������� (api �������)
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

// ��������� hangfire 
builder.Services.AddHangfire(options =>
{
    //���������� ��������� �� �� connectionString �� ����, ��� �������� ��������� �� ���� ������ � ��
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
app.UseHangfireDashboard(); // �������� ��������� hangfire ���������

// ��������� �����
/*JobExecution.StartJobs();*/

// �������� ������� �� ����� �������� ����������
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;

    try
    {
        var context = service.GetRequiredService<AppDbContext>();

        // �������� �� ��, ������� �� �� � ������� ������������ � ���
        await context.Database.EnsureCreatedAsync();
        await context.Database.CanConnectAsync();

        var logService = service.GetRequiredService<ILogService>();
        logService.LogInformation("����������� � �� ��������� �������");
    }
    catch (Exception ex)
    {
        var logService = service.GetRequiredService<ILogService>();
        logService.LogError($"����������� � �� �� ���������. ��������� ������: {ex.Message}");
    }
}

// ��������� ����������� � RabbitMq
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;

    try
    {
        var rabbitMqConnection = service.GetRequiredService<IConnection>();
        if(rabbitMqConnection.IsOpen)
        {
            var logService = service.GetRequiredService<ILogService>();
            logService.LogInformation("����������� � RabbitMQ ��������� �������");
		}
        else
        {
            throw new Exception("�� ������� ���������� ���������� � RabbitMQ");
        }

    }
    catch(Exception ex)
    {
        var logService = service.GetRequiredService<ILogService>();
		logService.LogError($"����������� � RabbitMQ �� ���������. ��������� ������: {ex.Message}");
		throw;
	}
}

    app.Run();