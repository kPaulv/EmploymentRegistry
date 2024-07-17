using Entities.ErrorModel;
using Entities.Responses.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers.Base
{
    public class BaseController : ControllerBase
    {
        public IActionResult ProcessError(BaseResponse response)
        {
            return response switch
            {
                NotFoundResponse => NotFound(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ((NotFoundResponse)response).Message
                }),
                BadRequestResponse => BadRequest(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ((BadRequestResponse)response).Message
                }),
                _ => throw new NotImplementedException()
            };
        }
    }
}
