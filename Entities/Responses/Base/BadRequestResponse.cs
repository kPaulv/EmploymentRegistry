using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Responses.Base
{
    public abstract class BadRequestResponse : BaseResponse
    {
        public string? Message { get; set; }

        public BadRequestResponse(string message) : base(false)
        {
            Message = message;
        }
    }
}
