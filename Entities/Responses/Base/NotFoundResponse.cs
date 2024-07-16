using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Responses.Base
{
    public abstract class NotFoundResponse : BaseResponse
    {
        public string? Message { get; set; }

        public NotFoundResponse(string message) : base(false) 
        {
            Message = message;
        }
    }
}
