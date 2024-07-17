using System.Linq.Expressions;

namespace Contracts.Interfaces
{
    public interface IRepository<T>
    {
        // CRUD

        // Create
        void Create(T entity);
        // Read
        IQueryable<T> ReadAll(bool trackChanges);   // trackChanges is used for Read-Only mode
        IQueryable<T> ReadByCondition(Expression<Func<T, bool>> condition, bool trackChanges);
        // Update 
        void Update(T entity);
        // Delete
        void Delete(T entity);
        
    }
}
