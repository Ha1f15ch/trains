using DatabaseEngine;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using WebApiApp.LogInfrastructure;
using WebAppTrain.Controllers.Middlewares;
using WebAppTrain.Repositories.Intefaces;
using WebAppTrain.Repositories.Repository;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var logConfigurator = new LogServiceConfigurator(builder.Configuration);
logConfigurator.Configure();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);
builder.Services.AddControllers();
builder.Services.AddSingleton<LogService>();
builder.Services.AddTransient<IExampleRepository, ExampleRepository>();
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

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