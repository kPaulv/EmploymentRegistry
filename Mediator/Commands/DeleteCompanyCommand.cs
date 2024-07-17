using MediatR;

namespace MediatorService.Commands
{
    public sealed record DeleteCompanyCommand(Guid Id, bool TrackChanges) : IRequest;
}
