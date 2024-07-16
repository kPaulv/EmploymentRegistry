namespace Entities.Responses.Base
{
    public abstract class BaseResponse
    {
        public bool Success { get; set; }
        protected BaseResponse(bool success) => Success = success;
    }
}
