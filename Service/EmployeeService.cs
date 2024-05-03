using Contracts.Interfaces;
using Service.Contracts;

namespace Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager logger;

        public EmployeeService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            this.logger = logger;
        }
    }
}
