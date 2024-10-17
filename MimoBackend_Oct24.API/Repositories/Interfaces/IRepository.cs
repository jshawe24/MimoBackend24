using System.Linq.Expressions;

namespace MimoBackend_Oct24.API.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}
