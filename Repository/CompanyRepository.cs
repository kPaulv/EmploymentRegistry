using Contracts.Interfaces;
using Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges) =>
                        await ReadAll(trackChanges).OrderBy(c => c.Name).ToListAsync();

        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges) => 
                        await ReadByCondition(c => c.Id.Equals(companyId), trackChanges)
                                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Company>> GetCompaniesByIdsAsync(IEnumerable<Guid> ids, 
                                                                       bool trackChanges) =>
                        await ReadByCondition(c => ids.Contains(c.Id), trackChanges)
                                .ToListAsync();

        public void CreateCompany(Company company) => Create(company);

        public void DeleteCompany(Company company) => Delete(company);
    }
}
