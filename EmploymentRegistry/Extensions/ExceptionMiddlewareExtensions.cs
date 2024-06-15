using Contracts.Interfaces;
using Entities.ErrorModel;
using Entities.Exceptions.BadRequest;
using Entities.Exceptions.NotFound;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace EmploymentRegistry.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager logger)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        // if NotFoundException thrown - 404, default - 500
                        context.Response.StatusCode = contextFeature.Error switch
                        {
                            NotFoundException => StatusCodes.Status404NotFound,
                            BadRequestException => StatusCodes.Status400BadRequest,
                            _ => StatusCodes.Status500InternalServerError
                        };

                        // write full exception to log
                        logger.Error($"Error occured: {contextFeature.Error}");
                        // initialize response model for client
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message
                        }.ToString());
                    }
                });
            });
        }
    }
}
