using Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        // Implementing lazy initialization for repositories
        private readonly Lazy<CompanyRepository> _companyRepository;
        private readonly Lazy<EmployeeRepository> _employeeRepository;


        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _companyRepository = new Lazy<CompanyRepository>(() => new CompanyRepository(repositoryContext));
            _employeeRepository = new Lazy<EmployeeRepository>(() => new EmployeeRepository(repositoryContext));
        }

        // Lazy initialization starts when we access the specific Entity Repository through this manager
        public ICompanyRepository CompanyStorage => _companyRepository.Value;
        public IEmployeeRepository EmployeeStorage => _employeeRepository.Value;

        // SaveChanges()
        public void Save() => _repositoryContext.SaveChanges();

    }
}
