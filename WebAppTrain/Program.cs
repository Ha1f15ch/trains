using DatabaseEngine;
using DatabaseEngine.RepositoryStorage.Interfaces;
using DatabaseEngine.RepositoryStorage.Repositories;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;
using WebApiApp.LogInfrastructure;
using WebAppTrain.Controllers.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var logConfigurator = new LogServiceConfigurator(builder.Configuration);
logConfigurator.Configure();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

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
builder.Services.AddSingleton<LogService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddTransient<INewsChannelRepository, NewsChannelRepository>();
builder.Services.AddTransient<INewsChannelsPostsRepository, NewsChannelsPostsRepository>();
builder.Services.AddTransient<INewsChannelsSubscribersRepository, NewsChannelsSubscribersRepository>();
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

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
app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;

    try
    {
        var context = service.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();

        await context.Database.CanConnectAsync();

        var logService = service.GetRequiredService<LogService>();
        logService.LogInformation("Подключение к БД выполнено успешно");
    }
    catch (Exception ex)
    {
        var logService = service.GetRequiredService<LogService>();
        logService.LogError("Подключение к БД не выполнено", "Возникшая ошибка: ", $"{ex.Message}");
    }
}

app.Run();