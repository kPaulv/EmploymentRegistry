using Contracts.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected RepositoryContext _repositoryContext;

        public Repository(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public void Create(T entity) => _repositoryContext.Set<T>().Add(entity);

        public IQueryable<T> ReadAll(bool trackChanges) =>
            !trackChanges ?
                _repositoryContext.Set<T>().AsNoTracking() :
                _repositoryContext.Set<T>();

        public IQueryable<T> ReadByCondition(Expression<Func<T, bool>> condition, bool trackChanges) =>
            !trackChanges ?
                _repositoryContext.Set<T>().Where(condition).AsNoTracking() :
                _repositoryContext.Set<T>().Where(condition);

        public void Update(T entity) => _repositoryContext.Set<T>().Update(entity);

        public void Delete(T entity) => _repositoryContext.Set<T>().Remove(entity);

    }
}
