using Entities.Entities;

namespace Contracts.Interfaces
{
    public interface ICompanyRepository
    {
        IEnumerable<Company> GetAllCompanies(bool trackChanges);
        Company GetCompany(Guid companyId, bool trackChanges);
        IEnumerable<Company> GetCompaniesByIds(IEnumerable<Guid> ids, bool trackChanges);
        void CreateCompany(Company company);
    }
}
