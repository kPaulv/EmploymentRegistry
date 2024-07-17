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
    internal sealed class UpdateCompanyHandler : 
        IRequestHandler<UpdateCompanyCommand, Unit>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public UpdateCompanyHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateCompanyCommand command,
            CancellationToken cancellationToken)
        {
            var companyEntity = 
                await _repository.CompanyStorage.GetCompanyAsync(command.Id, command.TrackChanges);

            if (companyEntity is null)
                throw new CompanyNotFoundException(command.Id);

            _mapper.Map(command.CompanyUpdateDto, companyEntity);

            await _repository.SaveAsync();

            return Unit.Value;
        }
    }
}
