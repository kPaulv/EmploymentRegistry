using AutoMapper;
using Contracts.Interfaces;
using Service.Contracts;
using Shared.DataTransferObjects;
using Entities.Exceptions;
using Entities.Entities;
using Entities.Exceptions.BadRequest;
using Entities.Exceptions.NotFound;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

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

        public async Task<IEnumerable<CompanyOutputDto>> GetAllCompaniesAsync(bool trackChanges)
        {
            var companies = await _repository.CompanyStorage.GetAllCompaniesAsync(trackChanges);

            var companiesDto = _mapper.Map<IEnumerable<CompanyOutputDto>>(companies);

            return companiesDto;
        }

        public async Task<CompanyOutputDto> GetCompanyAsync(Guid id, bool trackChanges)
        {
            var company = await _repository.CompanyStorage.GetCompanyAsync(id, trackChanges);
            if (company == null)
                throw new CompanyNotFoundException(id);

            var companyDto = _mapper.Map<CompanyOutputDto>(company);
            return companyDto;
        }

        public async Task<IEnumerable<CompanyOutputDto>> GetCompaniesByIdsAsync(IEnumerable<Guid> ids,
                                                                bool trackChanges)
        {
            if (ids is null)
                throw new IdParametersBadRequestException();

            var companies = await _repository.CompanyStorage.GetCompaniesByIdsAsync(ids, trackChanges);
            if (ids.Count() != companies.Count())
                throw new CollectionByIdsBadRequestException();

            var companyOutputDtos = _mapper.Map<IEnumerable<CompanyOutputDto>>(companies);

            return companyOutputDtos;
        }

        public async Task<CompanyOutputDto> CreateCompanyAsync(CompanyCreateDto companyInputDto)
        {
            var company = _mapper.Map<Company>(companyInputDto);

            _repository.CompanyStorage.CreateCompany(company);
            await _repository.SaveAsync();

            var companyOutputDto = _mapper.Map<CompanyOutputDto>(company);

            return companyOutputDto;
        }

        public async Task<(IEnumerable<CompanyOutputDto> companyOutputDtos, string ids)> 
            CreateCompanyCollectionAsync(IEnumerable<CompanyCreateDto> companyInputDtos)
        {
            if (companyInputDtos is null)
                throw new CompanyCollectionBadRequest();

            var companyCollection = _mapper.Map<IEnumerable<Company>>(companyInputDtos);
            foreach (var company in companyCollection)
            {
                _repository.CompanyStorage.CreateCompany(company);
            }
            await _repository.SaveAsync();

            var companyOutputDtos = _mapper.Map<IEnumerable<CompanyOutputDto>>(companyCollection);
            var ids = string.Join(',', companyOutputDtos.Select(c => c.Id));

            return (companyOutputDtos: companyOutputDtos, ids: ids);
        }

        public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.CompanyStorage.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            _repository.CompanyStorage.DeleteCompany(company);
            await _repository.SaveAsync();
        }

        public async Task UpdateCompanyAsync(Guid companyId, CompanyUpdateDto companyUpdateDto,
                                    bool trackChanges)
        {
            var company = await _repository.CompanyStorage.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            _mapper.Map(companyUpdateDto, company);
            await _repository.SaveAsync();
        }
    }
}