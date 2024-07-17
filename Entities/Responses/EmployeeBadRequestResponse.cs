using Entities.Responses.Base;

namespace Entities.Responses
{
    public sealed class EmployeeBadRequestResponse : BadRequestResponse
    {
        public EmployeeBadRequestResponse(Guid id) :
            base($"Error. Request for employee with id = {id} is invalid.")
        { }
    }
}
