using AutoMapper;
using Contracts.Interfaces;
using MediatorService.Queries;
using MediatR;
using Shared.DataTransferObjects;

namespace MediatorService.Handlers
{
    internal sealed class GetCompanyHandler :
        IRequestHandler<GetCompanyQuery, CompanyOutputDto>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public GetCompanyHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CompanyOutputDto> Handle(GetCompanyQuery query,
            CancellationToken cancellationToken)
        {
            var company =
                await _repository.CompanyStorage.GetCompanyAsync(query.Id, query.trackChanges);

            var companyDto = _mapper.Map<CompanyOutputDto>(company);

            return companyDto;
        }
    }
}
