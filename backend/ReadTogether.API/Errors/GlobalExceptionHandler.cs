using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReadTogether.Infrastructure.Exceptions;

namespace ReadTogether.API.Errors
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {

        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is NotFoundException e)
            {
                _logger.LogInformation("Handling NotFoundException {Exception}", e);
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = $"{e.ResourceName} Not Found",
                    Type = "https://httpstatuses.com/404",
                    Detail = e.Message
                };
                httpContext.Response.StatusCode = problem.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
                return true;
            }
            else if (exception is BookshelfBookConflictException conflictException)
            {
                _logger.LogInformation("Handling BookshelfBookConflictException {Exception}", conflictException);
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Bookshelf Book Conflict",
                    Type = "https://httpstatuses.com/409",
                    Detail = conflictException.Message
                };
                httpContext.Response.StatusCode = problem.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
                return true;
            }
            if (exception is AccessDeniedException accessDeniedException)
            {
                _logger.LogInformation("Handling AccessDeniedException {Exception}", accessDeniedException);
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Access Denied",
                    Type = "https://httpstatuses.com/409",
                    Detail = accessDeniedException.Message
                };
                httpContext.Response.StatusCode = problem.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
                return true;
            }
            else
            {
                _logger.LogError(exception, "An unhandled exception occurred.");
                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred",
                    Type = "https://httpstatuses.com/500",
                    Detail = "An unexpected error occurred. Please try again later."
                }, cancellationToken);
                return true;
            }
        }
    }
}