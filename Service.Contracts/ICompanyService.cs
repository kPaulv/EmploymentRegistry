using Entities.Responses.Base;
using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        Task<BaseResponse> GetAllCompaniesAsync(bool trackChanges);

        Task<BaseResponse> GetCompanyAsync(Guid id, bool trackChanges);

        Task<CompanyOutputDto> CreateCompanyAsync(CompanyCreateDto companyInputDto);

        Task<IEnumerable<CompanyOutputDto>> GetCompaniesByIdsAsync(IEnumerable<Guid> ids, 
                                                    bool trackChanges);

        Task<(IEnumerable<CompanyOutputDto> companyOutputDtos, string ids)> 
            CreateCompanyCollectionAsync(IEnumerable<CompanyCreateDto> companyCollection);

        Task DeleteCompanyAsync(Guid id, bool trackChanges);

        Task UpdateCompanyAsync(Guid id, CompanyUpdateDto companyUpdateDto, bool trackChanges); 
    }
}