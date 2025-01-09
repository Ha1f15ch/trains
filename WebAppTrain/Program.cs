using Microsoft.AspNetCore.Authentication.Negotiate;
using WebApiApp.LogInfrastructure;
using WebAppTrain.Controllers.Middlewares;
using WebAppTrain.Repositories.Intefaces;
using WebAppTrain.Repositories.Repository;

var builder = WebApplication.CreateBuilder(args);

var logConfigurator = new LogServiceConfigurator(builder.Configuration);
logConfigurator.Configure();

// Add services to the container.
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

app.Run();
