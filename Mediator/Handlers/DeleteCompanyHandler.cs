using AutoMapper;
using Contracts.Interfaces;
using Entities.Entities;
using Entities.Exceptions.NotFound;
using MediatorService.Commands;
using MediatorService.Notifications;
using MediatorService.Queries;
using MediatR;
using Shared.DataTransferObjects;

namespace MediatorService.Handlers
{
    internal sealed class DeleteCompanyHandler :
        INotificationHandler<CompanyDeletedNotification>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public DeleteCompanyHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Handle(CompanyDeletedNotification notification,
            CancellationToken cancellationToken)
        {
            var company =
                await _repository.CompanyStorage.GetCompanyAsync(notification.Id, 
                                                                    notification.TrackChanges);

            if (company is null)
                throw new CompanyNotFoundException(notification.Id);

            _repository.CompanyStorage.DeleteCompany(company);

            await _repository.SaveAsync();
        }
    }
}
