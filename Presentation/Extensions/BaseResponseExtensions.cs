using Entities.Responses;
using Entities.Responses.Base;

namespace Presentation.Extensions
{
    public static class BaseResponseExtensions
    {
        public static TResultType GetResult<TResultType>(this BaseResponse baseResponse) =>
            ((OkResponse<TResultType>)baseResponse).Result;
    }
}
