using BidMasterOnline.Application.Exceptions;
using System.Net;

namespace BidMasterOnline.API.Middlewares
{
    public class ExceptionsHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionsHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await this.HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            switch (exception)
            {
                case KeyNotFoundException notFoundException:
                    code = HttpStatusCode.NotFound;
                    break;
                case InvalidOperationException:
                case ArgumentNullException:
                case ArgumentException argumentException:
                    code = HttpStatusCode.BadRequest;
                    break;
                case ForbiddenException forbiddenException:
                    code = HttpStatusCode.Forbidden;
                    break;

            }

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)code;
            var result = exception.Message;

            return httpContext.Response.WriteAsync(result);
        }
    }

    public static class AppBuilderExt
    {
        public static IApplicationBuilder UseExceptionsHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionsHandlerMiddleware>();
        }
    }
}
