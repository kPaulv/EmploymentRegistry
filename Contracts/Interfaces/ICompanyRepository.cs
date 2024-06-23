using Entities.Entities;

namespace Contracts.Interfaces
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);
        Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges);
        Task<IEnumerable<Company>> GetCompaniesByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
        void CreateCompany(Company company);
        void DeleteCompany(Company company);
    }
}
