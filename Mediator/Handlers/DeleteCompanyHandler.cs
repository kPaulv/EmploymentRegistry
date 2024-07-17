using AutoMapper;
using Contracts.Interfaces;
using Entities.Entities;
using Entities.Exceptions.NotFound;
using MediatorService.Commands;
using MediatorService.Queries;
using MediatR;
using Shared.DataTransferObjects;

namespace MediatorService.Handlers
{
    internal sealed class DeleteCompanyHandler :
        IRequestHandler<DeleteCompanyCommand, Unit>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public DeleteCompanyHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteCompanyCommand command,
            CancellationToken cancellationToken)
        {
            var company =
                await _repository.CompanyStorage.GetCompanyAsync(command.Id, command.TrackChanges);

            if (company is null)
                throw new CompanyNotFoundException(command.Id);

            _repository.CompanyStorage.DeleteCompany(company);

            await _repository.SaveAsync();

            return Unit.Value;
        }
    }
}
