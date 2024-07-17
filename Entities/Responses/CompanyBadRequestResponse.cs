using Entities.Responses.Base;

namespace Entities.Responses
{
    public sealed class CompanyBadRequestResponse : BadRequestResponse
    {
        public CompanyBadRequestResponse(Guid id) :
            base($"Error. Request for company with id = {id} is invalid.")
        { }
    }
}
