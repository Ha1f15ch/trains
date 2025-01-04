using Microsoft.AspNetCore.Authentication.Negotiate;
using WebApiApp.LogInfrastructure;

var builder = WebApplication.CreateBuilder(args);

LogServiceConfigurator.Configure();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<LogService>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
