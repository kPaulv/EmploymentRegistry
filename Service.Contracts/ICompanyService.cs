using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        IEnumerable<CompanyOutputDto> GetAllCompanies(bool trackChanges);

        CompanyOutputDto GetCompany(Guid id, bool trackChanges);

        CompanyOutputDto CreateCompany(CompanyInputDto companyInputDto);

        IEnumerable<CompanyOutputDto> GetCompaniesByIds(IEnumerable<Guid> ids, 
                                                    bool trackChanges);

        (IEnumerable<CompanyOutputDto> companyOutputDtos, string ids) CreateCompanyCollection
            (IEnumerable<CompanyInputDto> companyCollection);
    }
}