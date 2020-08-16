using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Users.Exceptions;

namespace Users.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void OnException(ExceptionContext context)
        {
            var errorDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Detail = context.Exception.ToString()
            };

            switch (context.Exception)
            {
                case UserNotFoundException exception:
                    errorDetails.Title = "Not Found";
                    errorDetails.Status = StatusCodes.Status404NotFound;
                    errorDetails.Detail = exception.Message;
                    break;
                case UsernameNotUniqueException exception:
                    errorDetails.Title = "Username invalid";
                    errorDetails.Status = StatusCodes.Status400BadRequest;
                    errorDetails.Detail = exception.Message;
                    break;
                case ArgumentException argumentException:
                    errorDetails.Title = "Invalid Argument";
                    errorDetails.Status = StatusCodes.Status400BadRequest;
                    errorDetails.Detail = $"Argument {argumentException.ParamName} invalid";
                    break;
            }

            var result = new ObjectResult(errorDetails) {StatusCode = errorDetails.Status};

            context.Result = result;

            _logger.LogCritical(errorDetails.Detail);
            context.ExceptionHandled = true;
        }
    }
}