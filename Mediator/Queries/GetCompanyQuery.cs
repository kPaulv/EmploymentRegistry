using MediatR;
using Shared.DataTransferObjects;

namespace MediatorService.Queries
{
    public sealed record GetCompanyQuery(Guid Id, bool trackChanges) :
        IRequest<CompanyOutputDto>;
}
