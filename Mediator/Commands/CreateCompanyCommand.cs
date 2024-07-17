using MediatR;
using Shared.DataTransferObjects;

namespace MediatorService.Commands
{
    public sealed record CreateCompanyCommand(CompanyCreateDto CompanyCreateDto) :
        IRequest<CompanyOutputDto>;
}
