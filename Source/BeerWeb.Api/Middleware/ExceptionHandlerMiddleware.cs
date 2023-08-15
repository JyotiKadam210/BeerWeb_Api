using BeerWeb.Api.ErrorHandling;
using System.Net;
using ILogger = Serilog.ILogger;

namespace BeerWeb.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger log;

        public ExceptionHandlerMiddleware(RequestDelegate requestDelegate, ILogger logger)
        {
            next = requestDelegate;
            log = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (ArgumentException ex)
            {
                log.Error(ex, ex.Message);

                httpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsync(new ErrorDetails()
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = $"Validation error , {ex.Message}"
                }.ToString());
            }
            catch (Exception ex)
            {
                log.Error(ex, ex.Message);

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsync(new ErrorDetails()
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = $"Internal Server Error : {ex.Message}"
                }.ToString());
            }
        }
    }
}