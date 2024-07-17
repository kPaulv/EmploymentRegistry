namespace Entities.Exceptions.BadRequest
{
    public sealed class RefreshTokenBadRequestException : BadRequestException
    {
        public RefreshTokenBadRequestException() :
            base("Retrieved Token Refresh model is invalid.")
        { }
    }
}
