using BookMS.Application.Common.Exceptions;
using BookMS.WebApi.Support;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;  
using Microsoft.Extensions.Logging;

namespace BookMS.WebApi.Middleware
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _log;
        private readonly ProblemDetailsFactory _pdFactory;

        public GlobalExceptionMiddleware(
            ILogger<GlobalExceptionMiddleware> log,
            ProblemDetailsFactory pdFactory)
        {
            _log = log;
            _pdFactory = pdFactory;
        }

        public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
        {
            try
            {
                await next(ctx);
            }
            catch (ValidationException ex) // FluentValidation
            {
                // Build a ModelStateDictionary from FV errors
                var modelState = new ModelStateDictionary();
                foreach (var e in ex.Errors)
                {
                    var key = e.PropertyName ?? string.Empty;
                    modelState.AddModelError(key, e.ErrorMessage);
                }

                _log.LogWarning(ex, "Validation failed");
                var pd = _pdFactory.CreateValidationProblemDetails(ctx, modelState);
                pd.Status = StatusCodes.Status400BadRequest;
                await Write(ctx, pd);
            }
            catch (NotFoundException ex)
            {
                _log.LogInformation(ex, "Not found");
                var pd = _pdFactory.CreateProblem(ctx, StatusCodes.Status404NotFound, "Not Found", ex.Message);
                await Write(ctx, pd);
            }
            catch (ConflictException ex)
            {
                _log.LogInformation(ex, "Conflict");
                var pd = _pdFactory.CreateProblem(ctx, StatusCodes.Status409Conflict, "Conflict", ex.Message);
                await Write(ctx, pd);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Unhandled exception");
                var pd = _pdFactory.CreateProblem(ctx, StatusCodes.Status500InternalServerError, "Server Error",
                    "An unexpected error occurred.");
                await Write(ctx, pd);
            }
        }

        private static async Task Write(HttpContext ctx, ProblemDetails pd)
        {
            ctx.Response.ContentType = "application/problem+json";
            ctx.Response.StatusCode = pd.Status ?? StatusCodes.Status500InternalServerError;
            await ctx.Response.WriteAsJsonAsync(pd);
        }
    }
}
