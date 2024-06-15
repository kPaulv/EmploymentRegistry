using AutoMapper;
using Contracts.Interfaces;
using Service.Contracts;
using Shared.DataTransferObjects;
using Entities.Exceptions;
using Entities.Entities;
using Entities.Exceptions.BadRequest;
using Entities.Exceptions.NotFound;

namespace Service
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public IEnumerable<CompanyOutputDto> GetAllCompanies(bool trackChanges)
        {
            var companies = _repository.CompanyStorage.GetAllCompanies(trackChanges);

            var companiesDto = _mapper.Map<IEnumerable<CompanyOutputDto>>(companies);

            return companiesDto;
        }

        public CompanyOutputDto GetCompany(Guid id, bool trackChanges) {
            var company = _repository.CompanyStorage.GetCompany(id, trackChanges);
            if(company == null) 
                throw new CompanyNotFoundException(id);

            var companyDto = _mapper.Map<CompanyOutputDto>(company);
            return companyDto;
        }

        public IEnumerable<CompanyOutputDto> GetCompaniesByIds(IEnumerable<Guid> ids,
                                                                bool trackChanges)
        {
            if (ids is null)
                throw new IdParametersBadRequestException();

            var companies = _repository.CompanyStorage.GetCompaniesByIds(ids, trackChanges);
            if (ids.Count() != companies.Count())
                throw new CollectionByIdsBadRequestException();

            var companyOutputDtos = _mapper.Map<IEnumerable<CompanyOutputDto>>(companies);

            return companyOutputDtos;
        }

        public CompanyOutputDto CreateCompany(CompanyInputDto companyInputDto)
        {
            var company = _mapper.Map<Company>(companyInputDto);

            _repository.CompanyStorage.CreateCompany(company);
            _repository.Save();

            var companyOutputDto = _mapper.Map<CompanyOutputDto>(company);

            return companyOutputDto;
        }
    }
}