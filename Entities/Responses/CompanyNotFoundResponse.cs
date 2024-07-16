using Entities.Responses.Base;

namespace Entities.Responses
{
    public sealed class CompanyNotFoundResponse : NotFoundResponse
    {
        public CompanyNotFoundResponse(Guid id) : 
            base($"Error. Company with id = {id} not found.") { }
    }
}
