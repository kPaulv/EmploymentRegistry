using Entities.Responses.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Responses
{
    public sealed class OkResponse<TResult> : BaseResponse
    {
        public TResult Result { get; set; }

        public OkResponse(TResult result) : base(true)
        {
            Result = result;
        }
    }
}
