using System.Linq.Expressions;

namespace MVCWebAppIsmane.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetByIdAsync(int id);

        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate, string? includeprops = null);

        Task Create(T entity);

        Task Update(T entity);

        Task Delete(T entity);

    }
}
