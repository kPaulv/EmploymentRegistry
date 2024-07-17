using MediatR;
using Shared.DataTransferObjects;

namespace MediatorService.Queries
{
    public sealed record GetCompaniesQuery(bool trackChanges) : 
        IRequest<IEnumerable<CompanyOutputDto>>;
}
