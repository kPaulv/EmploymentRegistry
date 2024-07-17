using AutoMapper;
using Contracts.Interfaces;
using MediatorService.Queries;
using MediatR;
using Shared.DataTransferObjects;

namespace MediatorService.Handlers
{
    internal sealed class GetCompaniesHandler : 
        IRequestHandler<GetCompaniesQuery, IEnumerable<CompanyOutputDto>>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public GetCompaniesHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<CompanyOutputDto>> Handle(GetCompaniesQuery query, 
            CancellationToken cancellationToken)
        {
            var companies = 
                await _repository.CompanyStorage.GetAllCompaniesAsync(query.trackChanges);

            var companyDtos = _mapper.Map<IEnumerable<CompanyOutputDto>>(companies);

            return companyDtos;
        }
    }
}
