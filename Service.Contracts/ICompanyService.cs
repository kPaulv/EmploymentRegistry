using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        IEnumerable<CompanyOutputDto> GetAllCompanies(bool trackChanges);

        CompanyOutputDto GetCompany(Guid id, bool trackChanges);

        CompanyOutputDto CreateCompany(CompanyCreateDto companyInputDto);

        IEnumerable<CompanyOutputDto> GetCompaniesByIds(IEnumerable<Guid> ids, 
                                                    bool trackChanges);

        (IEnumerable<CompanyOutputDto> companyOutputDtos, string ids) CreateCompanyCollection
            (IEnumerable<CompanyCreateDto> companyCollection);

        public void DeleteCompany(Guid id, bool trackChanges);
    }
}