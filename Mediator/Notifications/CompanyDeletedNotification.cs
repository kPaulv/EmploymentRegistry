using MediatR;

namespace MediatorService.Notifications
{
    public sealed record CompanyDeletedNotification
        (Guid Id, bool TrackChanges) : INotification;
}
