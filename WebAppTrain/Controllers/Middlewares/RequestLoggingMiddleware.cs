using Serilog;

namespace WebAppTrain.Controllers.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate requestDelegate)
        {
            _next = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            Log.Information("Запрос: {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
            await _next(httpContext);
            Log.Information("Ответ: {StatusCode}", httpContext.Response.StatusCode);

        }
    }
}
