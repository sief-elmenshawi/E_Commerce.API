using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> logger;
        private readonly IHostEnvironment env;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger,IHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
        }



        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);

            var (statusCode, title) = exception switch
            {
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource NotFound"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Argument"),
                _ => (StatusCodes.Status500InternalServerError, "Server Failure")
            };

            httpContext.Response.StatusCode = statusCode;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = env.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
                Instance = httpContext.Request.Path
            };

            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;


        }
    }
}
