using MediatR;
using Shared.DataTransferObjects;

namespace MediatorService.Commands
{
    public sealed record UpdateCompanyCommand(Guid Id, 
        CompanyUpdateDto CompanyUpdateDto, bool TrackChanges) : IRequest;
}
