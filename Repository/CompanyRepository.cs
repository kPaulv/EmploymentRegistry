using Contracts.Interfaces;
using Entities.Entities;

namespace Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
                        ReadAll(trackChanges).OrderBy(c => c.Name).ToList();

        public Company GetCompany(Guid companyId, bool trackChanges) => 
                        ReadByCondition(c => c.Id.Equals(companyId), trackChanges)
                        .SingleOrDefault();

        public IEnumerable<Company> GetCompaniesByIds(IEnumerable<Guid> ids, 
                                                        bool trackChanges) =>
                                     ReadByCondition(c => ids.Contains(c.Id), 
                                                        trackChanges)
                                     .ToList();

        public void CreateCompany(Company company) => Create(company);

        public void DeleteCompany(Company company) => Delete(company);
    }
}
