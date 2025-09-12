using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BookMS.WebApi.Support
{
    public static class ProblemDetailsFactoryExtensions
    {
        public static ProblemDetails CreateProblem(
            this ProblemDetailsFactory factory,
            HttpContext http,
            int statusCode,
            string title,
            string detail,
            string? type = null)
        {
            // Correct parameter order for CreateProblemDetails:
            // (HttpContext, statusCode, title, type, detail, instance)
            var pd = factory.CreateProblemDetails(
                httpContext: http,
                statusCode: statusCode,
                title: title,
                type: type,
                detail: detail,
                instance: http.Request.Path);

            return pd;
        }
    }
}
