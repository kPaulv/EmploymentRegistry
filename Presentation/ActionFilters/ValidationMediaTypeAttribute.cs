using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace Presentation.ActionFilters
{
    public class ValidationMediaTypeAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var acceptHeaderReceived = 
                context.HttpContext.Request.Headers.ContainsKey("Accept");

            if (!acceptHeaderReceived)
            {
                context.Result = 
                    new BadRequestObjectResult("No Accept header received.");
                return;
            }

            var mediaType = context.HttpContext
                .Request.Headers["Accept"].FirstOrDefault();

            if(!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? outParsedMediaType))
            {
                context.Result =
                    new BadRequestObjectResult("No Media type received from Accept header.");

                return;
            }

            context.HttpContext.Items.Add("AcceptHeaderMediaType", outParsedMediaType);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
