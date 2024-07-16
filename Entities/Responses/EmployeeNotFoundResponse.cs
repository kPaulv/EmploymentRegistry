using Entities.Responses.Base;

namespace Entities.Responses
{
    public sealed class EmployeeNotFoundResponse : NotFoundResponse
    {
        public EmployeeNotFoundResponse(Guid id) :
            base($"Error. Employee with id = {id} not found.")
        { }
    }
}
