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

// ������ ����������� � ��
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// ������ ����������� � �� ��� hangfire
var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");
//������������� �������� ����������� serilog
Common.Logging.LoggingConfiguration.Configure();

builder.Host.UseSerilog();
// ��������� Mediatr
builder.Services.AddMediatR(cfg =>
{
	cfg.RegisterServicesFromAssembly(typeof(CreateNewUserCommand).Assembly);
});

try
{
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

	// ����������� � ������ RabbitMQ
	builder.Services.AddSingleton<RabbitMqService>(provider =>
	{
		var connection = provider.GetRequiredService<IConnection>();
		return new RabbitMqService(connection);
	});
}
catch (Exception ex)
{
	Console.WriteLine($"������ ������������� ����������� � ������� RabbitMQ - {ex.Message}");
}

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
            Description = "API ��� �������� � ���������� �������� � ���������� �� roadmap-a"
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

//������������ ������ ����������� � ����������� (api �������)
/*builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});*/

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

// ��� ����������� ������� ��������������
app.MapControllers();

app.UseHangfireDashboard(); // �������� ��������� hangfire ���������

// ����� ������������� RabbitMqService
using (var scope = app.Services.CreateScope())
{
	var rabbitMqService = scope.ServiceProvider.GetRequiredService<RabbitMqService>();
	await rabbitMqService.InitializeAsync();
}

// �������� ������� �� ����� �������� ����������
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;

	var logger = app.Services.GetRequiredService<ILogger<Program>>();

	try
    {
        var context = service.GetRequiredService<AppDbContext>();

        // �������� �� ��, ������� �� �� � ������� ������������ � ���
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

// ��������� ����������� � RabbitMq
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