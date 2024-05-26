using Entities.Entities;

namespace Contracts.Interfaces
{
    public interface ICompanyRepository
    {
        public IEnumerable<Company> GetAllCompanies(bool trackChanges);
    }
}
