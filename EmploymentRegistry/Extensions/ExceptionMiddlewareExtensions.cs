using Contracts.Interfaces;
using Entities.ErrorModel;
using Entities.Exceptions.BadRequest;
using Entities.Exceptions.NotFound;
using Entities.Exceptions.Validation;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                            FluentValidationException => StatusCodes.Status422UnprocessableEntity,
                            _ => StatusCodes.Status500InternalServerError
                        };

                        // write full exception to log
                        logger.Error($"Error occured: {contextFeature.Error}");
                        // initialize response model for client
                        // if we caught Fluent Validation error form response model from dict
                        if (contextFeature.Error is FluentValidationException exception) // declaration pattern
                        {
                            // form a JSON response containing Error Dict
                            await context.Response.WriteAsync(JsonSerializer
                                                    .Serialize(new { exception.Errors }));
                        }
                        else    // otherwise form standard response
                        {
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = contextFeature.Error.Message
                            }.ToString());
                        }
                    }
                });
            });
        }
    }
}
